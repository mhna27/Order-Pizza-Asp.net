using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Order_Pizza.Tools;
using Order_Pizza.ViewModels;
using System.Text;

namespace Order_Pizza.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new IdentityUser()
            {
                UserName = model.User_Name,
                Email = model.Email,
                PhoneNumber = model.Phone
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                    return View(model);
                }
            }
            var user2 = await _userManager.FindByEmailAsync(model.Email);
            await _userManager.AddToRoleAsync(user2, Static_Details.User_ROLE);
            var mobile_Code = await _userManager.GenerateTwoFactorTokenAsync(user2, "Phone");

            return RedirectToAction("ConfirmMobile",
                new { phone = user2.PhoneNumber, token = mobile_Code, redirect_To_Action = "Login" });
        }

        public IActionResult ConfirmMobile(string phone, string token, string redirect_To_Action)
        {
            if (phone == null || token == null)
            {
                return BadRequest();
            }
            ConfirmMobileVM vm = new ConfirmMobileVM()
            {
                Phone = phone,
                Code = token,
                Redirect_To_Action = redirect_To_Action
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmMobile(ConfirmMobileVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                return View(model);
            }

            bool result = await _userManager.VerifyTwoFactorTokenAsync(user, "Phone", model.Sms_Code);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "The entered code is not valid");
                return View(model);
            }

            if (model.Redirect_To_Action == "Login")
            {
                user.PhoneNumberConfirmed = true;
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                return RedirectToAction(model.Redirect_To_Action);
            }
            else if (model.Redirect_To_Action == "ResetPassword")
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                return RedirectToAction(model.Redirect_To_Action,
                    new { user_Id = user.Id, token = token });
            }
            return RedirectToAction(model.Redirect_To_Action);

        }

        public IActionResult Login(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(model.User_Name);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User with this profile was not found");
                return View(model);
            }
            var result = await _signInManager
                .PasswordSignInAsync(model.User_Name, model.Password, model.Remember_Me, false);

            //var mobileCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");
            //return RedirectToAction("LoginConfirmation", new { phone = user.PhoneNumber, token = mobileCode });
            if (result.Succeeded)
            {
                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            else if (result.RequiresTwoFactor)
            {
                return RedirectToAction("LoginWith2fa");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Your account has been locked");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "The login attempt is invalid");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Entered phone was not found");
                return View();
            }
            var mobile_Code = await _userManager.GenerateTwoFactorTokenAsync(user, "Phone");
            return RedirectToAction("ConfirmMobile",
                new { phone = user.PhoneNumber, token = mobile_Code, redirect_To_Action = "ResetPassword" });
        }

        public IActionResult ResetPassword(string user_Id, string token)
        {
            if (string.IsNullOrEmpty(user_Id) || string.IsNullOrEmpty(token)) return BadRequest();

            ResetPasswordVM model = new ResetPasswordVM()
            {
                User_Id = user_Id,
                Token = token
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return View();

            var user = await _userManager.FindByIdAsync(model.User_Id);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Password recovery attempt failed");
                return View(model);
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                    return View(model);
                }
            }
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        #region Remote Validations

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyUserName(string user_Name)
        {
            bool any = await _userManager.Users.AnyAsync(u => u.UserName == user_Name);
            if (!any)
                return Json(true);

            return Json("The user name entered is already registered");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsAnyEmail(string email)
        {
            bool any = await _userManager.Users.AnyAsync(u => u.Email == email);
            if (!any)
                return Json(true);

            return Json("The email entered is already registered");
        }

        #endregion
    }
}
