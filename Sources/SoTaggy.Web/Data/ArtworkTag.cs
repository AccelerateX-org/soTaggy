using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoTaggy.Web.Data
{
    /// <summary>
    /// Datenklasse für die Auswertung
    /// Keine Navigation, steht für sich alleine
    /// </summary>
    public class ArtworkTag
    {
        public int ArtworkTagId { get; set; }

        /// <summary>
        /// Das eingegebene Schlagwort
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Zeitpunkt der Eingabe
        /// </summary>
        public DateTime TaggingTime { get; set; }

        /// <summary>
        /// Das verwendete Bild.
        /// </summary>
        public int? ArtworkId { get; set; }
        public Artwork Artwork { get; set; }

        /// <summary>
        /// Zurodnung zur Gruppe
        /// </summary>
        public int? TaggingGroupId { get; set;  }
        public TaggingGroup TaggingGroup { get; set; }
 
    }
}
