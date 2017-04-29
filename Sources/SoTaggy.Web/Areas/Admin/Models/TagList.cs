
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SoTaggy.Web.Data;
using System.Text;
using SoTaggy.Web.Areas.Admin.Models;

namespace SoTaggy.Web.Areas.Admin.Models
{
    public class TagList
    {
        [Display(Name = "Schlagwort")]
        public string Tag { get; set; }

        [Display(Name = "Anzahl")]
        public int Anzahl { get; set; }

    }
}