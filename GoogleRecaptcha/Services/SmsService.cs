using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace GoogleRecaptcha.Services
{
    public class SmsService
    {
        //replace twilio account detail in appsetting.json
        public static void SendSms(string otpcode, string _accountSid, string _authToken)
        {
            string accountSid = _accountSid;
            string authToken = _authToken;

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "Please use the following code to login to the system. "+ otpcode + " Thank You",
                from: new Twilio.Types.PhoneNumber(""), //replca your twilio mobile no
                to: new Twilio.Types.PhoneNumber("") //replace your mobile to receive messeges with country code
            );
        }

    }
}
