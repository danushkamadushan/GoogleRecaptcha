using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRecaptcha.Models
{
    public class TwoFactor
    {
        [Required]
        [DataType(DataType.Text)]
        public string TwoFactorCode { get; set; }
        public string Email { get; set; }
        public bool RememberMe { get; set; }
        public string Token { get; set; }
        public string SiteKey { get; set; }
        public int LogginAttempt { get; set; }
    }
}
