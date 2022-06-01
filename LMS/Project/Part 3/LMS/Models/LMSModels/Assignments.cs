using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submissions = new HashSet<Submissions>();
        }

        public uint AssignmentId { get; set; }
        public uint CatId { get; set; }
        public string AName { get; set; }
        public uint MaxPoints { get; set; }
        public string Contents { get; set; }
        public DateTime Due { get; set; }

        public virtual AssignmentCategories Cat { get; set; }
        public virtual ICollection<Submissions> Submissions { get; set; }
    }
}
