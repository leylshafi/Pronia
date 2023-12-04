using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Models;
using Pronia.ViewModels;

namespace Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _manager;
        private readonly SignInManager<AppUser> _signIn;
        public AccountController(UserManager<AppUser> manager, SignInManager<AppUser> signIn)
        {
            _manager = manager;
            _signIn = signIn;
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
            await _signIn.SignInAsync(user,isPersistent:false);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
