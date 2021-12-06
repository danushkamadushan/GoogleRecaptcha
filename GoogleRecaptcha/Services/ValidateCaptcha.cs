using GoogleRecaptcha.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace GoogleRecaptcha.Services
{
    public class ValidateCaptcha
    {
        //replace twilio account detail in appsetting.json
        public static bool Validate(string token, string secretkey)
        {
            bool status = false;
            var client = new WebClient();
            var response = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretkey, token));
            var result = JsonConvert.DeserializeObject<CaptchaResponse>(response);
            status = result.success;
            return status;
        }
    }
}
