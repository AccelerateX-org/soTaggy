using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoTaggy.Web.Data
{
    public class Artwork
    {
        public int ArtworkId { get; set; }

        /// <summary>
        /// Titel
        /// </summary>
        [Display(Name="Titel")]
        public string Title { get; set; }

        /// <summary>
        /// Künstler (auch Mehrzahl)
        /// </summary>
        [Display(Name = "Künstlername")]
        public string Artist { get; set; }

        /// <summary>
        /// Datierung
        /// </summary>
        [Display(Name = "Datierung")]
        public string Dating { get; set; }

        /// <summary>
        /// Technik
        /// </summary>
        [Display(Name = "Material / Technik")]
        public string Technique { get; set; }

        /// <summary>
        /// Höhe
        /// </summary>
        [Display(Name = "Maße")]
        public string Dimensions { get; set; }

        /// <summary>
        /// Standort, Besitzer oder Eigentümer
        /// </summary>
        [Display(Name="Sammlung")]
        public int ArtworkLibraryId { get; set; }
        public virtual ArtworkLibrary ArtworkLibrary { get; set; }

        /// <summary>
        /// Copyright Inhaber
        /// </summary>
        [Display(Name="Rechteinhaber")]
        public int CopyrightOwnerId { get; set; }
        public virtual CopyrightOwner CopyrightOwner { get; set; }
        
        /// <summary>
        /// Bilddateien
        /// </summary>
        public virtual ICollection<ArtworkImage> Images { get; set; }

        /// <summary>
        /// 1-n: Ein Pool besteht aus n Bildern. physikalische Verbindung
        /// Wenn ein Eintrag im Pool gelöscht wird, dann wird das Bild auch gelöscht
        /// Wenn das identische Bild in zwei Pools vorkommen soll, muss es zweimal hochgeladen werden
        /// </summary>
        public int ArtworkPoolId { get; set; }
        public virtual ArtworkPool ArtworkPool { get; set; }
    }
}
