using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoTaggy.Web.Data;

namespace SoTaggy.Web.Areas.Tagging.Models
{
    public class TaggingReviewModel
    {
        public ImageDisplayModel TaggingImage { get; set; }

        public Artwork PoolImage { get; set; }
    }
}