using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SoTaggy.Web.Data
{
    public class TaggingOrg
    {
        public TaggingOrg()
        {
            Groups = new HashSet<TaggingGroup>();
        }


        public int TaggingOrgId { get; set; }

        [Display(Name = "Name der Institution / Lehrer")]
        public string Name { get; set; }


        /// <summary>
        /// Benutzername, unter dem die Gruppe erreichbar ist
        /// </summary>
        [Display(Name = "Benutzername der Institution / Lehrer")]
        public string UserName { get; set; }

        /// <summary>
        /// Das Kennwort, entspricht dem in der Membership
        /// Muss hier separat gespeichert werden, da es von Lehrer vergeben wird
        /// </summary>
        [Display(Name = "Kennwort der Institution / Lehrer")]
        public string Password { get; set; }

        public virtual ICollection<TaggingGroup> Groups { get; set; }

    }
}