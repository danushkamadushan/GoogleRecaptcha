using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using GoogleRecaptcha.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using GoogleRecaptcha.Services;
using Microsoft.Extensions.Configuration;

namespace GoogleRecaptcha.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<SystemUser> _userManager;
        private readonly SignInManager<SystemUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private IConfiguration _configuration;

        public LoginModel(SignInManager<SystemUser> signInManager, 
            ILogger<LoginModel> logger,
            UserManager<SystemUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string error, string returnUrl = null)
        {
            ErrorMessage = Request.Cookies["Error"];
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            SystemUser user = new SystemUser();
            string twilioAcc = _configuration.GetSection("Twilio").GetSection("AccountId").Value;
            string twilioAuth = _configuration.GetSection("Twilio").GetSection("AuthToken").Value;

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    Random rnd = new Random();
                    string otp = OTPCode.createOTP();

                    user = await _userManager.FindByEmailAsync(Input.Email);
                    if (user != null)
                    {
                        if (user.LoginAttempt <= 3)
                        {
                            user.OTPCode = otp.ToString();
                            user.TwoFactorEnabled = true;
                            var isUpdated = await _userManager.UpdateAsync(user);

                            if (isUpdated.Succeeded)
                            {
                                SmsService.SendSms(otp, twilioAcc, twilioAuth);
                                return RedirectToAction("TwoFactorLogin", "Account", new { Input.Email, Input.RememberMe, user.LoginAttempt, returnUrl });
                            }
                        }
                        else if(user.LoginAttempt > 3)
                        {
                            CookieOptions option = new CookieOptions();
                            option.Expires = DateTime.Now.AddSeconds(5);
                            Response.Cookies.Append("Error", "User blocked.Please contact the administrator", option);
                            return RedirectToPage("./Login");
                        }
                       
                    }
                   
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    user.LoginAttempt = user.LoginAttempt + 1;
                    await _userManager.UpdateAsync(user);
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            return Page();
        }
    }
}
