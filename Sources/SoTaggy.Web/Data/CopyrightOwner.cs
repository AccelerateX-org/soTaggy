using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoTaggy.Web.Data
{
    public class CopyrightOwner
    {
        public int CopyrightOwnerId { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public ICollection<Artwork> Artworks { get; set; }
    }
}
