using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            Enrolled = new HashSet<Enrolled>();
        }

        public uint ClassId { get; set; }
        public uint CId { get; set; }
        public string Semester { get; set; }
        public string Location { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Professor { get; set; }

        public virtual Courses C { get; set; }
        public virtual Professors ProfessorNavigation { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<Enrolled> Enrolled { get; set; }
    }
}
