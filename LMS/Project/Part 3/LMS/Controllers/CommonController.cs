using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LMS.Models.LMSModels;

namespace LMS.Controllers
{
  public class CommonController : Controller
  {

    /*******Begin code to modify********/

    protected Team108LMSContext db;

    public CommonController()
    {
      db = new Team108LMSContext();
    }

    /*
     * WARNING: This is the quick and easy way to make the controller
     *          use a different LibraryContext - good enough for our purposes.
     *          The "right" way is through Dependency Injection via the constructor 
     *          (look this up if interested).
    */

    public void UseLMSContext(Team108LMSContext ctx)
    {
      db = ctx;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        db.Dispose();
      }
      base.Dispose(disposing);
    }



    /// <summary>
    /// Retreive a JSON array of all departments from the database.
    /// Each object in the array should have a field called "name" and "subject",
    /// where "name" is the department name and "subject" is the subject abbreviation.
    /// </summary>
    /// <returns>The JSON array</returns>
    public IActionResult GetDepartments()
    {
            using (db)
            {
                var query =
                from d in db.Departments
                select new { name = d.DName, subject = d.Code };

                return Json(query.ToArray());
            }

            return Json(null);
        }



    /// <summary>
    /// Returns a JSON array representing the course catalog.
    /// Each object in the array should have the following fields:
    /// "subject": The subject abbreviation, (e.g. "CS")
    /// "dname": The department name, as in "Computer Science"
    /// "courses": An array of JSON objects representing the courses in the department.
    ///            Each field in this inner-array should have the following fields:
    ///            "number": The course number (e.g. 5530)
    ///            "cname": The course name (e.g. "Database Systems")
    /// </summary>
    /// <returns>The JSON array</returns>
    public IActionResult GetCatalog()
    {
            using (db)
            {
                var query =
                from d in db.Departments
                select new
                {
                    subject = d.Code,
                    dname = d.DName,
                    courses = from c in db.Courses
                              where c.Code == d.Code
                              select new
                              {
                                  number = c.CNum,
                                  cname = c.CName
                              }
                };

                return Json(query.ToArray());
            }

            return Json(null);
    }

    /// <summary>
    /// Returns a JSON array of all class offerings of a specific course.
    /// Each object in the array should have the following fields:
    /// "season": the season part of the semester, such as "Fall"
    /// "year": the year part of the semester
    /// "location": the location of the class
    /// "start": the start time in format "hh:mm:ss"
    /// "end": the end time in format "hh:mm:ss"
    /// "fname": the first name of the professor
    /// "lname": the last name of the professor
    /// </summary>
    /// <param name="subject">The subject abbreviation, as in "CS"</param>
    /// <param name="number">The course number, as in 5530</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetClassOfferings(string subject, int number)
    {

            using (db)
            {
                var query =
                from c in db.Classes
                join p in db.Professors
                on c.Professor equals p.UId
                join co in db.Courses
                on c.CId equals co.CId
                where co.CNum == number && co.Code == subject
                select new { season = c.Semester.Substring(4), year = c.Semester.Substring(0, 4), location = c.Location, start = c.StartTime, end = c.EndTime, fname = p.FirstName, lname = p.LastName };

                return Json(query.ToArray());
            }

            return Json(null);
    }

    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <returns>The assignment contents</returns>
    public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
    {
            string semester = year + season;

            using (db)
            {
                var query =
                from c in db.Courses
                join cl in db.Classes
                on c.CId equals cl.CId
                join ac in db.AssignmentCategories
                on cl.ClassId equals ac.ClassId
                join a in db.Assignments
                on ac.CatId equals a.CatId
                where cl.Semester == semester && a.AName == asgname
                select new { contents = a.Contents};

                string contents = query.ToList().First().contents;

                return Content(contents);
            }


        }


    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment submission.
    /// Returns the empty string ("") if there is no submission.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <param name="uid">The uid of the student who submitted it</param>
    /// <returns>The submission text</returns>
    public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
    {

            string semester = year + season;
            string contents = "";

            using (db)
            {
                var query =
                from c in db.Courses
                join cl in db.Classes
                on c.CId equals cl.CId
                join ac in db.AssignmentCategories
                on cl.ClassId equals ac.ClassId
                join a in db.Assignments
                on ac.CatId equals a.CatId
                join s in db.Submissions
                on a.AssignmentId equals s.AssignmentId
                where cl.Semester == semester && a.AName == asgname && s.UId == uid
                select new { contents = s.Contents };

                if(query.ToList().Count() > 0)
                {
                    contents = query.ToList().First().contents;
                }  
            }

            return Content(contents);
        }


    /// <summary>
    /// Gets information about a user as a single JSON object.
    /// The object should have the following fields:
    /// "fname": the user's first name
    /// "lname": the user's last name
    /// "uid": the user's uid
    /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
    ///               If the user is a Professor, this is the department they work in.
    ///               If the user is a Student, this is the department they major in.    
    ///               If the user is an Administrator, this field is not present in the returned JSON
    /// </summary>
    /// <param name="uid">The ID of the user</param>
    /// <returns>
    /// The user JSON object 
    /// or an object containing {success: false} if the user doesn't exist
    /// </returns>
    public IActionResult GetUser(string uid)
    {
        using (db)
        {
                var query =
                from a in db.Administrators
                select new { fname = a.FirstName, lname = a.LastName, uid = a.UId };

                if (query.ToArray().Length > 0)
                {
                    return Json(query.ToArray());
                }

                var query2 =
                from p in db.Professors
                join d in db.Departments
                on p.WorksIn equals d.Code
                select new { fname = p.FirstName, lname = p.LastName, uid = p.UId, department = d.DName };

                if (query2.ToArray().Length > 0)
                {
                    return Json(query2.ToArray());
                }

                var query3 =
                from s in db.Students
                join d in db.Departments
                on s.Major equals d.Code
                select new { fname = s.FirstName, lname = s.LastName, uid = s.UId, department = d.DName };

                if (query3.ToArray().Length > 0)
                {
                    return Json(query3.ToArray());
                }
            }

            return Json(new { success = false });
    }


    /*******End code to modify********/

  }
}