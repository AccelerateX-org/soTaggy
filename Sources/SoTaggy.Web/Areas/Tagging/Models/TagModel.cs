using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoTaggy.Web.Areas.Tagging.Models
{
    public class TagModel
    {
        public string Tag { get; set; }

        public ImageDisplayModel Image { get; set; }
    }
}