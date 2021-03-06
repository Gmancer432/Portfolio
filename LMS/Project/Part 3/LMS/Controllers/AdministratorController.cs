using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Administrator")]
  public class AdministratorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Department(string subject)
    {
      ViewData["subject"] = subject;
      return View();
    }

    public IActionResult Course(string subject, string num)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }

    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of all the courses in the given department.
    /// Each object in the array should have the following fields:
    /// "number" - The course number (as in 5530)
    /// "name" - The course name (as in "Database Systems")
    /// </summary>
    /// <param name="subject">The department subject abbreviation (as in "CS")</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetCourses(string subject)
    {

            using (db)
            {
                var query =
                from c in db.Courses
                where c.Code == subject
                select new
                {
                    number = c.CNum,
                    name = c.CName
                };

                return Json(query.ToArray());
            }

            return Json(null);
    }


    


    /// <summary>
    /// Returns a JSON array of all the professors working in a given department.
    /// Each object in the array should have the following fields:
    /// "lname" - The professor's last name
    /// "fname" - The professor's first name
    /// "uid" - The professor's uid
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <returns>The JSON result</returns>
    public IActionResult GetProfessors(string subject)
    {
            using (db)
            {
                var query =
                from d in db.Departments
                join p in db.Professors
                on d.Code equals p.WorksIn
                where d.Code == subject
                select new
                {
                    lname = p.LastName,
                    fname = p.FirstName,
                    uid = p.UId
                };

                return Json(query.ToArray());
            }

            return Json(null);
    }



    /// <summary>
    /// Creates a course.
    /// A course is uniquely identified by its number + the subject to which it belongs
    /// </summary>
    /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
    /// <param name="number">The course number</param>
    /// <param name="name">The course name</param>
    /// <returns>A JSON object containing {success = true/false},
	/// false if the Course already exists.</returns>
    public IActionResult CreateCourse(string subject, int number, string name)
    {
            using (db)
            {
                var c = new Courses();
                c.CName = name;
                c.CNum = (ushort)number;
                c.Code = subject;

                db.Courses.Add(c);

                try
                {
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch
                {

                }
            }

            return Json(new { success = false });
    }



    /// <summary>
    /// Creates a class offering of a given course.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="number">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="start">The start time</param>
    /// <param name="end">The end time</param>
    /// <param name="location">The location</param>
    /// <param name="instructor">The uid of the professor</param>
    /// <returns>A JSON object containing {success = true/false}. 
    /// false if another class occupies the same location during any time 
    /// within the start-end range in the same semester, or if there is already
    /// a Class offering of the same Course in the same Semester.</returns>
    public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
    {

        using (db)
        {
                var lt = from cl in db.Classes
                         where cl.Semester == year + season && cl.Location == location && ((cl.StartTime <= start.TimeOfDay && cl.EndTime >= start.TimeOfDay) || (cl.StartTime <= end.TimeOfDay && cl.EndTime >= end.TimeOfDay) || (cl.StartTime > start.TimeOfDay && cl.EndTime < end.TimeOfDay))
                         select cl;

                if (lt.ToList().Count() > 0)
                {
                    return Json(new { success = false });
                }

                var q = from co in db.Courses
                        where co.CNum == number && co.Code == subject
                        select co;

                uint mycid = 0;

                if(q.ToList().Count() > 0)
                {
                    mycid = q.ToList().First().CId;
                }

                var c = new Classes();
                c.CId = mycid;
                c.Semester = year + season;
                c.StartTime = start.TimeOfDay;
                c.EndTime = end.TimeOfDay;
                c.Location = location;
                c.Professor = instructor;

                db.Classes.Add(c);

            try
            {
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch
            {

            }
        }

        return Json(new { success = false });
    }


    /*******End code to modify********/

  }
}