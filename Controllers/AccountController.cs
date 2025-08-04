using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Task4.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Task4.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
        
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Users");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                
                if (user != null && !user.IsBlocked)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    
                    if (result.Succeeded)
                    {
                        user.LastLogin = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Index", "Users");
                    }
                }
                
                ModelState.AddModelError(string.Empty, "Invalid login attempt or account is blocked.");
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
             if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Users");
            }
            return View(); 
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try{
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    RegistrationDate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                 foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                }
                catch (Exception ex)
                {

                    if(ex.InnerException?.Message.Contains("duplicate key") == true || 
                       ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true
                       || ex.InnerException?.Message.Contains("Violation of UNIQUE KEY constraint") == true)

                    {
                        ModelState.AddModelError(string.Empty, "Email already exists. Please use a different email.");
                    }else{

                    ModelState.AddModelError(string.Empty, "An error occurred while registering the user: " + ex.Message);
                    }
                }
               
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}