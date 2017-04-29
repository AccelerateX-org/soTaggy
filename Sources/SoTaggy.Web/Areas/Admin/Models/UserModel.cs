using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoTaggy.Web.Areas.Admin.Models
{
    public class UserModel
    {
        public string UserName { get; set; }
        
        public bool IsUserAdmin { get; set; }
        public bool IsArtworkAdmin { get; set; }
        public bool IsTaggingAdmin { get; set; }
        public bool IsAnalyst { get; set; }

        public bool IsTagger { get; set; }
    }


    public class UserRegisterModel
    {
        public string UserName { get; set; }

        public bool IsUserAdmin { get; set; }
        public bool IsArtworkAdmin { get; set; }
        public bool IsTaggingAdmin { get; set; }
        public bool IsAnalyst { get; set; }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}