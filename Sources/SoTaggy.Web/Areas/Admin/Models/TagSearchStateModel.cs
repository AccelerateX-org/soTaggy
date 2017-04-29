using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoTaggy.Web.Areas.Admin.Models
{
    public class TagSearchStateModel
    {
        [Display(Name = "Tagging Gruppe")]
        public int TaggingGroupId { get; set; }

    }
}