using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoTaggy.Web.Areas.Admin.Models
{
    public class TagAnalysisImage
    {
        [Display(Name = "Bildtitel")]
        public string Artwork { get; set; }

        [Display(Name = "Bildkennung")]
        public int ArtworkID { get; set; }

        [Display(Name = "Bildpool")]
        public string ImagePoolName { get; set; }

        [Display(Name = "Tags")]
        public List<TagList> Tags { get; set; }

        [Display(Name = "AnzahlTags")]
        public int numberTags { get; set; }

        [Display(Name = "AnzahlVerschideneTags")]
        public int numberDifferentTags { get; set; }

    }
}