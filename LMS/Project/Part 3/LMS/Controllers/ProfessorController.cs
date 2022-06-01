using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Professor")]
  public class ProfessorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Students(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
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

    public IActionResult Categories(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
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

    public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      ViewData["uid"] = uid;
      return View();
    }

    /*******Begin code to modify********/


    /// <summary>
    /// Returns a JSON array of all the students in a class.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "dob" - date of birth
    /// "grade" - the student's grade in this class
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
    {
            using (db)
            {
                var query =
                from c in db.Courses
                join cl in db.Classes
                on c.CId equals cl.CId
                join e in db.Enrolled
                on cl.ClassId equals e.ClassId
                join s in db.Students
                on e.UId equals s.UId
                where c.Code == subject && c.CNum == num && cl.Semester == year+season
                select new
                {
                    fname = s.FirstName,
                    lname = s.LastName,
                    uid = s.UId,
                    dob = s.Dob,
                    grade = e.Grade
                };

                return Json(query.ToArray());
            }

            return Json(null);
    }



    /// <summary>
    /// Returns a JSON array with all the assignments in an assignment category for a class.
    /// If the "category" parameter is null, return all assignments in the class.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The assignment category name.
    /// "due" - The due DateTime
    /// "submissions" - The number of submissions to the assignment
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class, 
    /// or null to return assignments from all categories</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
    {
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
                where c.Code == subject && cl.Semester == year + season && (category == null ? true : ac.AcName == category) && c.CNum == num
                select new
                {
                    aname = a.AName,
                    cname = ac.AcName,
                    due = a.Due,
                    submissions = (from s in db.Submissions
                                  where s.AssignmentId == a.AssignmentId
                                  select new { s }).Count()
                                  
                };

                return Json(query.ToArray());
            }

            return Json(null);
        }


    /// <summary>
    /// Returns a JSON array of the assignment categories for a certain class.
    /// Each object in the array should have the folling fields:
    /// "name" - The category name
    /// "weight" - The category weight
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
    {
            using (db)
            {
                var query =
                from ac in db.AssignmentCategories
                join cl in db.Classes
                on ac.ClassId equals cl.ClassId
                join c in db.Courses
                on cl.CId equals c.CId
                where c.Code == subject && cl.Semester == year + season && c.CNum == num
                select new
                {
                    name = ac.AcName,
                    weight = ac.Weight
                };

                return Json(query.ToArray());
            }

            return Json(null);
        }

    /// <summary>
    /// Creates a new assignment category for the specified class.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The new category name</param>
    /// <param name="catweight">The new category weight</param>
    /// <returns>A JSON object containing {success = true/false},
    ///	false if an assignment category with the same name already exists in the same class.</returns>
    public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
    {
            uint classID = 0;

            using (db)
            {
                var query = from cl in db.Classes
                            join c in db.Courses
                            on cl.CId equals c.CId
                            where cl.Semester == year + season && c.Code == subject
                            select new
                            {
                                classID = cl.ClassId
                            };

                classID = query.ToList().First().classID;

                var ac = new AssignmentCategories();

                ac.ClassId = classID;
                ac.AcName = category;
                ac.Weight = (byte)catweight;

                db.AssignmentCategories.Add(ac);

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
    /// Creates a new assignment for the given class and category.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="asgpoints">The max point value for the new assignment</param>
    /// <param name="asgdue">The due DateTime for the new assignment</param>
    /// <param name="asgcontents">The contents of the new assignment</param>
    /// <returns>A JSON object containing success = true/false,
	/// false if an assignment with the same name already exists in the same assignment category.</returns>
    public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
    {

            uint catID = 0;
            uint classID = 0;

            using (db)
            {
                var query = from cl in db.Classes
                            join c in db.Courses
                            on cl.CId equals c.CId
                            join ac in db.AssignmentCategories
                            on cl.ClassId equals ac.ClassId
                            where cl.Semester == year + season && c.Code == subject
                            select new
                            {
                                catID = ac.CatId,
                                classID = cl.ClassId
                            };

                catID = query.ToList().First().catID;
                classID = query.ToList().First().classID;

                var a = new Assignments();
                a.CatId = catID;
                a.AName = asgname;
                a.MaxPoints = (uint)asgpoints;
                a.Contents = asgcontents;
                a.Due = asgdue;

                db.Assignments.Add(a);

                try
                {
                    db.SaveChanges();

                    var q2 = from e in db.Enrolled
                             where e.ClassId == classID
                             select e;

                    foreach(Enrolled e in q2)
                    {
                        updateStudentGrade(e.UId, classID);
                    }

                    return Json(new { success = true });
                }
                catch
                {

                }
            }

            return Json(new { success = false });
    }


    /// <summary>
    /// Gets a JSON array of all the submissions to a certain assignment.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "time" - DateTime of the submission
    /// "score" - The score given to the submission
    /// 
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
    {
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
                join st in db.Students
                on s.UId equals st.UId
                where c.Code == subject && cl.Semester == year + season && ac.AcName == category && c.CNum == num && a.AName == asgname
                select new
                {
                    fname = st.FirstName,
                    lname = st.LastName,
                    uid = st.UId,
                    time = s.SubmissionTime,
                    score = s.Score
                };

                return Json(query.ToArray());
            }

            return Json(null);
        }


    /// <summary>
    /// Set the score of an assignment submission
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <param name="uid">The uid of the student who's submission is being graded</param>
    /// <param name="score">The new score for the submission</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
    {

            uint classID = 0;

            using (db)
            {
                var query =
                from cl in db.Classes
                join c in db.Courses
                on cl.CId equals c.CId
                where c.Code == subject && cl.Semester == year + season && c.CNum == num
                select cl;

                classID = query.First().ClassId;

                var query2 =
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


                foreach (Submissions s in query2)
                {
                    s.Score = (uint)score;
                }

                try
                {
                    db.SaveChanges();

                    updateStudentGrade(uid, classID);

                    return Json(new { success = true });
                }
                catch
                {

                }
            }

            return Json(new { success = false });
    }


    /// <summary>
    /// Returns a JSON array of the classes taught by the specified professor
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 5530)
    /// "name" - The course name
    /// "season" - The season part of the semester in which the class is taught
    /// "year" - The year part of the semester in which the class is taught
    /// </summary>
    /// <param name="uid">The professor's uid</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
            using (db)
            {
                var query =
                from cl in db.Classes
                join c in db.Courses
                on cl.CId equals c.CId
                where cl.Professor == uid
                select new
                {
                    subject = c.Code,
                    number = c.CNum,
                    name = c.CName,
                    season = cl.Semester.Substring(4),
                    year = cl.Semester.Substring(0, 4)
                };

                return Json(query.ToArray());

            }
            return Json(null);
    }



    public void updateStudentGrade(string uid, uint classID)
        {
            //For any non-empty assignment category:
            //calculate percentage of all assigments in the category
            //get all grades, divide by all max points, multiply that number by weight
            //compute total of all assignment categories
            //scaling factor = 100/sum of all weights
            //total * scaling factor = grade
            //convert to letter grade
            //store letter grade

            uint weightTotal = 0;
            double pointTotal = 0;

            using (db)
            {
                var query =
                from ac in db.AssignmentCategories
                join cl in db.Classes
                on ac.ClassId equals cl.ClassId
                where cl.ClassId == classID
                select new
                {
                    weight = ac.Weight,
                    assignments = from ac in db.AssignmentCategories
                                  join a in db.Assignments
                                  on ac.CatId equals a.CatId
                                  where ac.ClassId == classID
                                  select new
                                  {
                                      max = a.MaxPoints,
                                      submissions = from s in db.Submissions
                                                    where s.UId == uid && s.AssignmentId == a.AssignmentId
                                                    select s
                                  }


                };

                foreach(var e in query.ToList())
                {
                    weightTotal += e.weight;
                    double categoryTotal = 0;
                    double pointsPerCategory = 0;
                    double pointsPossible = 0;

                    foreach(var a in e.assignments)
                    {
                        pointsPossible += a.max;

                        if(a.submissions.Count() > 0)
                        {
                            foreach (var s in a.submissions)
                            {
                                pointsPerCategory += s.Score;
                            }
                        }
                        
                    }

                    categoryTotal = (pointsPerCategory / pointsPossible) * e.weight;
                    pointTotal += categoryTotal;
                }

                double scalar = 100 / weightTotal;
                double scaledPointTotal = pointTotal * scalar;

                string grade = "--";

                if (scaledPointTotal >= 93)
                {
                    grade = "A";
                }else if (scaledPointTotal >= 90)
                {
                    grade = "A-";
                }
                else if (scaledPointTotal >= 87)
                {
                    grade = "B+";
                }
                else if (scaledPointTotal >= 83)
                {
                    grade = "B";
                }
                else if (scaledPointTotal >= 80)
                {
                    grade = "B-";
                }
                else if (scaledPointTotal >= 77)
                {
                    grade = "C+";
                }
                else if (scaledPointTotal >= 73)
                {
                    grade = "C";
                }
                else if (scaledPointTotal >= 70)
                {
                    grade = "C-";
                }
                else if (scaledPointTotal >= 67)
                {
                    grade = "D+";
                }
                else if (scaledPointTotal >= 63)
                {
                    grade = "D";
                }
                else if (scaledPointTotal >= 60)
                {
                    grade = "D-";
                }
                else{
                    grade = "E";
                }

                var en = from e in db.Enrolled
                         where e.ClassId == classID && e.UId == uid
                         select e;

                en.First().Grade = grade;

                db.SaveChanges();
            }
        }
    /*******End code to modify********/

  }
}