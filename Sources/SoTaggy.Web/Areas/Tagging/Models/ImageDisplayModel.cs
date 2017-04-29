using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoTaggy.Web.Data;

namespace SoTaggy.Web.Areas.Tagging.Models
{
    public class ImageDisplayModel
    {
        public ImageDisplayModel()
        {
            Tags = new List<ArtworkTag>();
        }


        public int ImageEntryID { get; set; }

        public int Order { get; set; }

        public List<ArtworkTag> Tags { get; set; }

    }
}