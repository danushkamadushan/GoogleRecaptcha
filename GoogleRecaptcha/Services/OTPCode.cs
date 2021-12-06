using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleRecaptcha.Services
{
    public class OTPCode
    {
        public static string createOTP()
        {
            Random rnd = new Random();

            int numcode = rnd.Next(10000, 99999);
           
            string n1 = numcode.ToString().Substring(0, 2);
            string n2 = numcode.ToString().Substring(3);

            int charcode1 = rnd.Next(0, 26);
            char c1 = (char)('A' + charcode1);

            int charcode2 = rnd.Next(0, 26);
            char c2 = (char)('a' + charcode2);

            string otp = n1 + c1 + c2 + n2;

            char[] arr = otp.ToCharArray();
            Array.Reverse(arr);

            return new string(arr);
        }
    }
}
