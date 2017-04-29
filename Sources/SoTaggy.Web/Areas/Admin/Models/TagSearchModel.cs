using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoTaggy.Web.Areas.Admin.Models
{
    public class TagSearchModel
    {

        [Display(Name = "Bilderpool")]
        public List<int> ArtworkPoolId { get; set; }

        
        [Display(Name = "Tags der zugehörigen Gruppe")]
        public List<int> GroupTagId { get; set; }

        [Display(Name = "Startdatum")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime TaggingDateBegin { get; set; }

        [Display(Name = "Enddatum")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime TaggingDateEnd { get; set; }

        [Display(Name = "Startzeit")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan TaggingTimeBegin { get; set; }

        [Display(Name = "Endzeit")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan TaggingTimeEnd { get; set; }

        public bool IsArtworkSelection { get; set; }
        public bool IsTimeSpanSelection { get; set; }
        public bool IsGroupTagSelection { get; set; }
    }
}