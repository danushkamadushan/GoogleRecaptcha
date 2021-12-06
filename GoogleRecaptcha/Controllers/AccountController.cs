using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleRecaptcha.Areas.Identity.Data;
using GoogleRecaptcha.Areas.Identity.Pages.Account;
using GoogleRecaptcha.Models;
using GoogleRecaptcha.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;

namespace GoogleRecaptcha.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<SystemUser> _userManager;
        public IConfiguration _configuration;
        public AccountController(UserManager<SystemUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TwoFactorLogin(string email, bool rememberMe, int loginAttempt, string returnUrl = null)
        {
            string sitekey = _configuration.GetSection("Recaptcha").GetSection("SiteKey").Value;
            var model = new TwoFactor();
            model.Email = email;
            model.SiteKey = sitekey;
            model.LogginAttempt = loginAttempt;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TwoFactorLogin(TwoFactor twoFactor)
        {
            bool isvalidOTP = false;
            bool isCaptchaValidate = false;
            string secretkey = _configuration.GetSection("Recaptcha").GetSection("SecretKey").Value;

            var user = await _userManager.FindByEmailAsync(twoFactor.Email);

            if (user.LoginAttempt < 3)
            {
                isCaptchaValidate = true;
            }
            else if (user.LoginAttempt == 3)
            {
                isCaptchaValidate = ValidateCaptcha.Validate(twoFactor.Token, secretkey);
            }
            else if (user.LoginAttempt > 3)
            {
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddSeconds(5);
                Response.Cookies.Append("Error", "Account Blocked. Please Contact the Administrator", option);
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
           

            if (user != null && user != null)
            {
               
                if (user.OTPCode == twoFactor.TwoFactorCode)
                {
                    isvalidOTP = true;
                }
            }

            if (isvalidOTP && isCaptchaValidate)
            {
                user.LoginAttempt = 0;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                user.LoginAttempt = user.LoginAttempt + 1;
                await _userManager.UpdateAsync(user);
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddSeconds(5);
                if (!isvalidOTP)
                {
                    Response.Cookies.Append("Error", "Invalid OTP Code", option);
                }
                else if(!isCaptchaValidate)
                {
                    Response.Cookies.Append("Error", "Recaptcha Failed", option);
                }

                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
            
        }
    }
}
