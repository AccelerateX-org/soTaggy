using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SoTaggy.Web.Data;

namespace SoTaggy.Web.Areas.Admin.Models
{
    public class TaggingOrgViewModel
    {
        public TaggingOrg Org { get; set; }


        /// <summary>
        /// Steht die Gruppe generell zur Verfügung
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool IsGroupAvailable(TaggingGroup group)
        {
            var now = DateTime.Now;
            return group.UnitStart <= now && now <= group.UnitEnd.AddDays(1);
        }

        public bool IsGroupActive(TaggingGroup group)
        {
            var now = DateTime.Now;
            return now <= group.CreatedAt;
        }
    }
}