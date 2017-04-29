using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoTaggy.Web.Data
{
    public class ArtworkImage
    {
        public int ArtworkImageId { get; set; }

        /// <summary>
        /// Der Datentyp
        /// </summary>
        public string ImageFileType { get; set; }

        // Das ist erforderlich, sonst geht es z.B. in SQL-Server CE nicht
        // bzw. dort wird 4000 als maximale Länge automatisch angenommen!
        [MaxLength]
        public byte[] ImageData { get; set; }

        /// <summary>
        /// Anzahl der "erworbenen" Bildrechte
        /// </summary>
        [Display(Name = "Anzahl Bildrechte")]
        public long RightsToView { get; set; }

        /// <summary>
        /// Anzahl der Sichten in Tagging Sessions (das erste Anzeigeereignis wird gezählt)
        /// </summary>
        public long ShownFirstInSession { get; set; }

        /// <summary>
        /// Anzahl der Sichten in Sessioins insgesamt (mit vor/zurück)
        /// </summary>
        public long ShownInSessionTotal { get; set; }

        /// <summary>
        /// Anzeige in Bildpools (Lehrer, Museum, Schüler)
        /// </summary>
        public long ShownAsThumnail { get; set; }

        /// <summary>
        /// Zuordnung zum Kunstwerk
        /// </summary>
        public int ArtworkId { get; set; }
        public virtual Artwork Artwork { get; set; }
    }
}
