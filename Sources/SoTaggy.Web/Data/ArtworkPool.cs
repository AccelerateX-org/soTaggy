using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoTaggy.Web.Data
{
    /// <summary>
    /// Pool von Bildern von Kunstwerken 
    /// </summary>
    public class ArtworkPool
    {
        public int ArtworkPoolId { get; set; }

        /// <summary>
        /// Name des Bildpools
        /// </summary>
        [Display(Name="Name")]
        public string Name { get; set; }

        /// <summary>
        /// Beschreibung
        /// </summary>
        [Display(Name = "Beschreibung")]
        public string Description { get; set; }

        /// <summary>
        /// Die Bilder (von Kunstwerken)
        /// </summary>
        public virtual ICollection<Artwork> Artworks { get; set; }

        public virtual ICollection<TaggingGroup> Groups { get; set; }

    }
}
