

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
    public class TagAnalysisModel
    {

        [Display(Name = "Schlagwort")]
        public string Tag { get; set; }

        [Display(Name = "Bildkennung")]
        public string Artwork { get; set; }

        [Display(Name = "Bildpool")]
        public string ImagePoolName { get; set; }

        [Display(Name = "Tags der zugehörigen Gruppe")]
        public string GroupTags { get; set; }
        
        [Display(Name = "Zeitpunkt")]
        public DateTime TaggingTime { get; set; }
        
    }

}
