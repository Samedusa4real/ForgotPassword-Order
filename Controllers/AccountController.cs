using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using PustokTemplate.DAL;
using PustokTemplate.Models;
using PustokTemplate.ViewModels;

namespace PustokTemplate.Controllers
{
    public class AccountController : Controller
    {
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly PustokDbContext _context;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,PustokDbContext context)
        {
			_userManager = userManager;
			_signInManager = signInManager;
			_context = context;
		}


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(MemberLoginViewModel loginVM, string returnUrl=null)
        {
            if (!ModelState.IsValid)
                return View();

            AppUser user = await _userManager.FindByNameAsync(loginVM.UserName);

            if (user == null || user.IsAdmin)
            {
                ModelState.AddModelError("", "Username or password incorrect!");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password,false,false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt!");
                return View();
            }

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(MemberRegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
                return View();

            if (_userManager.Users.Any(x => x.UserName == registerVM.UserName))
            {
                ModelState.AddModelError("UserName", "Username is already used!");
                return View();
            }

            if (_userManager.Users.Any(x => x.Email == registerVM.Email))
            {
                ModelState.AddModelError("Email", "Email is already used!");
                return View();
            }

            AppUser appUser = new AppUser
            {
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                FulName = registerVM.FullName,
                IsAdmin = false
            };

            var result = await _userManager.CreateAsync(appUser, registerVM.Password);

            if(!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("",err.Description);
                
                return View();
            }

            await _userManager.AddToRoleAsync(appUser, "Member");

            await _signInManager.SignInAsync(appUser, false);

            return RedirectToAction("login", "account");
		}

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("login", "account");
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if(user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("login");
            }


            AccountProfileViewModel apvm = new AccountProfileViewModel
            {
                Profile = new ProfileEditViewModel
                {
                    FullName = user.FulName,
                    Email = user.Email,
                    UserName = user.UserName,
                    Address = user.Address,
                    Phone = user.Phone,
                },

                Orders = _context.Orders.Include(x=>x.OrderItems).ThenInclude(x=>x.Book).Where(x=>x.AppUserId == user.Id).ToList(),
            };

            return View(apvm);
        }
        [Authorize(Roles = "Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileEditViewModel profileVM)
        {
            if (!ModelState.IsValid)
            {
                AccountProfileViewModel vm = new AccountProfileViewModel
                {
                    Profile = profileVM
                };
                return View(vm);
            }

            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);

            appUser.FulName = profileVM.FullName;
            appUser.Email = profileVM.Email;
            appUser.UserName = profileVM.UserName;
            appUser.Address = profileVM.Address;
            appUser.Phone = profileVM.Phone;

            if (!string.IsNullOrEmpty(profileVM.CurrentPassword) && !string.IsNullOrEmpty(profileVM.NewPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(appUser, profileVM.CurrentPassword, profileVM.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    AccountProfileViewModel vm = new AccountProfileViewModel
                    {
                        Profile = profileVM
                    };

                    return View(vm);
                }
            }

            var result = await _userManager.UpdateAsync(appUser);

            if(result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }

                AccountProfileViewModel vm = new AccountProfileViewModel
                {
                    Profile = profileVM
                };

                return View(vm);
            }

            await _signInManager.SignInAsync(appUser, false);

            return RedirectToAction("profile");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordVM)
        {
            if(!ModelState.IsValid)
                return View();

            AppUser user = await _userManager.FindByEmailAsync(forgotPasswordVM.Email);

            if (user == null || user.IsAdmin)
            {
                ModelState.AddModelError("Email", "Email is not correct!");
                return View();
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string url = Url.Action("resetpassword", "account", new { email = forgotPasswordVM.Email, token = token }, Request.Scheme);


			return Json(new { url = url });
            return RedirectToAction("login");
        }

        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.IsAdmin || !await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider,"ResetPassword",token))
                return RedirectToAction("login");

            ViewBag.Email = email;
            ViewBag.Token = token;

            return View();
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetVM)
        {
			AppUser user = await _userManager.FindByEmailAsync(resetVM.Email);

			if (user == null || user.IsAdmin)
				return RedirectToAction("login");

            var result = await _userManager.ResetPasswordAsync(user, resetVM.Token, resetVM.Password);

            if (!result.Succeeded)
            {
				foreach (var err in result.Errors)
					ModelState.AddModelError("", err.Description);

				return RedirectToAction("login");
			}

			return RedirectToAction("login");
        }
	}
}
