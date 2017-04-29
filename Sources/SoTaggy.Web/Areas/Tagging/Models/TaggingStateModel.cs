using SoTaggy.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoTaggy.Web.Tagging.Models
{
    public class TaggingStateModel
    {
        public ArtworkImage Image { get; set; }

        public TaggingGroup TaggingGroup { get; set; }

        public int peopleViewing { get; set; }
    }
}