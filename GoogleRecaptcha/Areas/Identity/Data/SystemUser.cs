using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GoogleRecaptcha.Areas.Identity.Data
{

    public class SystemUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string MobileNumber { get; set; }
        [Column(TypeName = "nvarchar(6)")]
        public string OTPCode { get; set; }
        [Column(TypeName = "int")]
        public int LoginAttempt { get; set; }
    }
}
