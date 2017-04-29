using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoTaggy.Web.Data
{
    public class GroupTag
    {
        public int GroupTagId { get; set; }

        public string Tag { get; set; }

        public virtual ICollection<TaggingGroup> Groups { get; set; }
    }
}
