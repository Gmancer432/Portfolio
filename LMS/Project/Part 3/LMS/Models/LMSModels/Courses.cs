using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Courses
    {
        public Courses()
        {
            Classes = new HashSet<Classes>();
        }

        public uint CId { get; set; }
        public string CName { get; set; }
        public ushort CNum { get; set; }
        public string Code { get; set; }

        public virtual Departments CodeNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
