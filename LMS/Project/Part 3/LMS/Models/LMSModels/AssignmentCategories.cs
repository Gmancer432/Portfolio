using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCategories
    {
        public AssignmentCategories()
        {
            Assignments = new HashSet<Assignments>();
        }

        public uint CatId { get; set; }
        public uint ClassId { get; set; }
        public string AcName { get; set; }
        public byte Weight { get; set; }

        public virtual Classes Class { get; set; }
        public virtual ICollection<Assignments> Assignments { get; set; }
    }
}
