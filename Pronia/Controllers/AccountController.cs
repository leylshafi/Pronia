﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Enumerations;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _manager;
        private readonly SignInManager<AppUser> _signIn;
        private readonly RoleManager<IdentityRole> _roleManager;
		public AccountController(UserManager<AppUser> manager, SignInManager<AppUser> signIn, RoleManager<IdentityRole> roleManager)
		{
			_manager = manager;
			_signIn = signIn;
			_roleManager = roleManager;
		}

		public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {
            if(!ModelState.IsValid) return View();
            userVM.Name = userVM.Name.Trim();
            userVM.Surname = userVM.Surname.Trim();

            string name = Char.ToUpper(userVM.Name[0]) + userVM.Name.Substring(1).ToLower();
            string surname = Char.ToUpper(userVM.Surname[0]) + userVM.Surname.Substring(1).ToLower();

			AppUser user = new()
            {
                Name= name,
                Surname =surname,
                Email=userVM.Email,
                UserName = userVM.Username,
                Gender = userVM.Gender
            };
            var result = await _manager.CreateAsync(user, userVM.Password);
            if(!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View();
            }
            await _manager.AddToRoleAsync(user, UserRole.Member.ToString());
            await _signIn.SignInAsync(user,isPersistent:false);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM userVM, string? returnUrl)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _manager.FindByNameAsync(userVM.UsernameOrEmail);
            if(user is null)
            {
                user = await _manager.FindByEmailAsync(userVM.UsernameOrEmail);
                if(user is null)
                {
                    ModelState.AddModelError(String.Empty, "Username, Email or Password is incorrect");
                    return View();

                }
            }
            var result = await _signIn.PasswordSignInAsync(user, userVM.Password,userVM.IsRemembered,true);
            if (result.IsLockedOut)
            {
				ModelState.AddModelError(String.Empty, "Too many attempts, try again later");
				return View();
			}
            if(!result.Succeeded)
            {
				ModelState.AddModelError(String.Empty, "Username, Email or Password is incorrect");
				return View();
            }
            if(returnUrl is null)
            {
				return RedirectToAction("Index", "Home");
			}
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> CreateRole()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if(!await _roleManager.RoleExistsAsync(role.ToString()))
                {
					await _roleManager.CreateAsync(new IdentityRole
					{
						Name = role.ToString()
					});
				}
               
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
