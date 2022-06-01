using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Student")]
  public class StudentController : CommonController
  {

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Catalog()
    {
      return View();
    }

    public IActionResult Class(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }


    public IActionResult ClassListings(string subject, string num)
    {
      System.Diagnostics.Debug.WriteLine(subject + num);
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }


    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of the classes the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 5530)
    /// "name" - The course name
    /// "season" - The season part of the semester
    /// "year" - The year part of the semester
    /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
            using (db)
            {
                var query =
                from cl in db.Classes
                join c in db.Courses
                on cl.CId equals c.CId
                join e in db.Enrolled
                on cl.ClassId equals e.ClassId
                where e.UId == uid
                select new
                {
                    subject = c.Code,
                    number = c.CNum,
                    name = c.CName,
                    season = cl.Semester.Substring(4),
                    year = cl.Semester.Substring(0, 4),
                    grade = e.Grade
                };

                return Json(query.ToArray());

            }
            return Json(null);
        }

    /// <summary>
    /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The category name that the assignment belongs to
    /// "due" - The due Date/Time
    /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="uid"></param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
    {
            using (db)
            {
                var query =
                from cl in db.Classes
                join c in db.Courses
                on cl.CId equals c.CId
                join e in db.Enrolled
                on cl.ClassId equals e.ClassId
                join ac in db.AssignmentCategories
                on cl.ClassId equals ac.ClassId
                join a in db.Assignments
                on ac.CatId equals a.CatId
                join s in db.Submissions
                on a.AssignmentId equals s.AssignmentId
                into t
                from k in t.DefaultIfEmpty()
                where e.UId == uid && cl.Semester == year + season && c.Code == subject
                select new
                {
                    aname = a.AName,
                    cname = ac.AcName,
                    due = a.Due,
                    score = k.Score == null ? null : (int?)k.Score

                };

                return Json(query.ToArray());

            }
            return Json(null);
        }



    /// <summary>
    /// Adds a submission to the given assignment for the given student
    /// The submission should use the current time as its DateTime
    /// You can get the current time with DateTime.Now
    /// The score of the submission should start as 0 until a Professor grades it
    /// If a Student submits to an assignment again, it should replace the submission contents
    /// and the submission time (the score should remain the same).
	/// Does *not* automatically reject late submissions.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="uid">The student submitting the assignment</param>
    /// <param name="contents">The text contents of the student's submission</param>
    /// <returns>A JSON object containing {success = true/false}.</returns>
    public IActionResult SubmitAssignmentText(string subject, int num, string season, int year, 
      string category, string asgname, string uid, string contents)
    {

            bool exists = false;

            using (db)
            {
                var query =
                from ac in db.AssignmentCategories
                join cl in db.Classes
                on ac.ClassId equals cl.ClassId
                join c in db.Courses
                on cl.CId equals c.CId
                join a in db.Assignments
                on ac.CatId equals a.CatId
                join s in db.Submissions
                on a.AssignmentId equals s.AssignmentId
                where c.Code == subject && cl.Semester == year + season && ac.AcName == category && c.CNum == num && s.UId == uid && a.AName == asgname
                select s;



                foreach (Submissions s in query)
                {
                    exists = true;
                    s.Contents = contents;
                }

                if (exists)
                {
                    try
                    {
                        db.SaveChanges();

                    }
                    catch
                    {
                        return Json(new { success = false });
                    }
                }

                if (!exists)
                {
                    var query2 =
                    from ac in db.AssignmentCategories
                    join cl in db.Classes
                    on ac.ClassId equals cl.ClassId
                    join c in db.Courses
                    on cl.CId equals c.CId
                    join a in db.Assignments
                    on ac.CatId equals a.CatId
                    where c.Code == subject && cl.Semester == year + season && ac.AcName == category && c.CNum == num && a.AName == asgname
                    select new { assignmentID = a.AssignmentId };

                    uint assignID = query2.ToList().First().assignmentID;

                    var s = new Submissions();
                    s.AssignmentId = assignID;
                    s.Contents = contents;
                    s.Score = 0;
                    s.UId = uid;
                    s.SubmissionTime = DateTime.Now;

                    db.Submissions.Add(s);

                    try
                    {
                        db.SaveChanges();

                    }
                    catch
                    {
                        return Json(new { success = false });
                    }
                }
            }

            return Json(new { success = true });
        }

    
    /// <summary>
    /// Enrolls a student in a class.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing {success = {true/false},
	/// false if the student is already enrolled in the Class.</returns>
    public IActionResult Enroll(string subject, int num, string season, int year, string uid)
    {

            uint classID = 0;

            using (db)
            {
                var query =
                from cl in db.Classes
                join c in db.Courses
                on cl.CId equals c.CId
                where c.Code == subject && cl.Semester == year + season && c.CNum == num
                select new { classID = cl.ClassId };

                classID = query.ToList().First().classID;
 
                var e = new Enrolled();
                e.UId = uid;
                e.ClassId = classID;
                e.Grade = "--";

                try
                {
                    db.Enrolled.Add(e);
                    db.SaveChanges();
                }
                catch
                {
                    return Json(new { success = false });
                }

                return Json(new { success = true });
            }
            
    }



    /// <summary>
    /// Calculates a student's GPA
    /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
    /// Assume all classes are 4 credit hours.
    /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
    /// If a student does not have any grades, they have a GPA of 0.0.
    /// Otherwise, the point-value of a letter grade is determined by the table on this page:
    /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
    public IActionResult GetGPA(string uid)
    {
            List<double> grades = new List<double>();


            using (db)
            {
                var query =
                from cl in db.Classes
                join c in db.Courses
                on cl.CId equals c.CId
                join e in db.Enrolled
                on cl.ClassId equals e.ClassId
                where e.UId == uid
                select e.Grade;

                foreach (string s in query)
                {
                    switch (s){
                        case "A":
                            grades.Add(4.0);
                            break;
                        case "A-":
                            grades.Add(3.7);
                            break;
                        case "B+":
                            grades.Add(3.3);
                            break;
                        case "B":
                            grades.Add(3.0);
                            break;
                        case "B-":
                            grades.Add(2.7);
                            break;
                        case "C+":
                            grades.Add(2.3);
                            break;
                        case "C":
                            grades.Add(2.0);
                            break;
                        case "C-":
                            grades.Add(1.7);
                            break;
                        case "D+":
                            grades.Add(1.3);
                            break;
                        case "D":
                            grades.Add(1.0);
                            break;
                        case "D-":
                            grades.Add(0.7);
                            break;
                        case "E":
                            grades.Add(0.0);
                            break;
                        default:
                            break;
                    }
                   
                }
            }

            return Json(new { gpa = grades.Count() > 0 ? grades.Average() : 0 });
        }

    /*******End code to modify********/

  }
}