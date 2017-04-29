using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoTaggy.Web.Data
{
    public class TaggingGroup
    {
        public int TaggingGroupId { get; set; }

        [Display(Name = "Projektname")]
        public string Name { get; set; }

        /// <summary>
        /// Besitzer der Tagging Unit. Username 
        /// </summary>
        [Display(Name = "Angelegt von")]
        public string Owner { get; set; }

        [Display(Name = "Tagging aktiv ab")]
        [DataType(DataType.Date)]
        public DateTime UnitStart { get; set; }

        [Display(Name = "Tagging aktiv bis")]
        [DataType(DataType.Date)]
        public DateTime UnitEnd { get; set; }

        [Display(Name = "Angelegt am")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Bilderpool")]
        public int ArtworkPoolId { get; set; }
        public virtual ArtworkPool ArtworkPool { get; set; }
        
        /// <summary>
        /// Benutzername, unter dem die Gruppe erreichbar ist
        /// </summary>
        [Display(Name="Benutzername der Gruppe")]
        public string UserName { get; set; }

        /// <summary>
        /// Das Kennwort, entspricht dem in der Membership
        /// Muss hier separat gespeichert werden, da es von Lehrer vergeben wird
        /// </summary>
        [Display(Name="Kennwort der Gruppe")]
        public string Password { get; set; }

        // Liste von Tags, um Gruppe zu kennzeichnet
        public virtual ICollection<GroupTag> GroupTags { get; set; }


        // Liste der Taggings ist nicht erreichbar
        public virtual ICollection<ArtworkTag> ArtwortTags { get; set; }

        public string GetTagList()
        {
            StringBuilder sb = new StringBuilder();

            foreach (GroupTag tag in GroupTags)
            {
                sb.Append(tag.Tag);
                sb.Append(";");
            }
            
            return sb.ToString();
        }
    }
}
