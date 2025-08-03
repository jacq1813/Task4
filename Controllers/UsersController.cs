using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using Task4.Models;
using Microsoft.AspNetCore.Authorization;
using System;

namespace Task4.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.IsBlocked)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            var users = _userManager.Users
                .OrderByDescending(u => u.LastLogin)
                .ToList();

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> Block(string[] userIds)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.IsBlocked)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null && !user.IsBlocked)
                {
                    user.IsBlocked = true;
                    await _userManager.UpdateAsync(user);
                }
            }

            if (userIds.Contains(currentUser.Id))
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Unblock(string[] userIds)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.IsBlocked)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null && user.IsBlocked)
                {
                    user.IsBlocked = false;
                    await _userManager.UpdateAsync(user);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string[] userIds)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.IsBlocked)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }

            if (userIds.Contains(currentUser.Id))
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Index");
        }
    }
}