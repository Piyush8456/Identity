using Custom_Identity.Models.ViewModel;
using Custom_Identity.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Custom_Identity.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {UserId} cannot be found";
                return View("NotFound");
            }

            ViewBag.UserName = user.UserName;

            var model = new UserClaimsViewModel
            {
                UserId = UserId
            };

            var existingUserClaims = await _userManager.GetClaimsAsync(user);

            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };

                if (existingUserClaims.Any(c => c.Type == claim.Type))
                {
                    userClaim.IsSelected = true;
                }

                model.Cliams.Add(userClaim);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            //First fetch the User Details
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.UserId} cannot be found";
                return View("NotFound");
            }

            // Get all the user existing claims and delete them
            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            // Add all the claims that are selected on the UI
            var AllSelectedClaims = model.Cliams.Where(c => c.IsSelected)
                        .Select(c => new Claim(c.ClaimType, c.ClaimType))
                        .ToList();

            //If At least 1 Claim is assigned, Any Method will return true
            if (AllSelectedClaims.Any())
            {
                //add a user to multiple claims simultaneously
                result = await _userManager.AddClaimsAsync(user, AllSelectedClaims);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Cannot add selected claims to user");
                    return View(model);
                }
            }

            return RedirectToAction("Index","Home", new { UserId = model.UserId });
        }
    }
}
