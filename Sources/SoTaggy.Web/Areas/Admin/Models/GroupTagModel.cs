using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoTaggy.Web.Data;

namespace SoTaggy.Web.Areas.Admin.Models
{
    public class GroupTagModel
    {
        public string Tag { get; set; }
        public int GroupId { get; set; }

        public TaggingGroup Group { get; set; }

    }
}