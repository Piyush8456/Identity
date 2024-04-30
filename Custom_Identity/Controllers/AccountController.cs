using Microsoft.AspNetCore.Mvc;
using Custom_Identity.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Custom_Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace Custom_Identity.Controllers
{

    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();
            foreach (ApplicationUser user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRolesViewModel.Add(new UserRolesViewModel { User = user, Roles = roles });
            }

            return View(userRolesViewModel);
        }



        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
              var result =  await _signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid Login attept");
                return View(model);

            }
            return View(model);
        }

        //[Authorize(Roles ="Admin")]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();

            var roles = _roleManager.Roles.Select(r => r.Name).ToList();

            model.Roles = roles;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                ApplicationUser user = new()
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    Address = model.Address,
                };

                var result = await _userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    // Add the user to the specified role
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);

                    await _signInManager.SignInAsync(user, false);

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        //[Authorize(Roles ="Admin")]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditUser(string Id)
        {
           var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }
            var UserClaims = await _userManager.GetClaimsAsync(user);
            var UserRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Roles = UserRoles.ToList(),
                Claims = UserClaims.Select(c=> c.Value).ToList()

            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var existingUser = await _userManager.FindByIdAsync(model.Id);
            if (!ModelState.IsValid)
            {
                return View("EditUser", model);
            }

            
            if (existingUser == null)
            {
                return NotFound();
            }

            else
            {
                existingUser.Name = model.Name;
                existingUser.Email = model.Email;
                existingUser.Address = model.Address;

                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMasssege = $"User With id= {user.Id} cannot found";
                return View("Not Found");
            }
            var exitingUserClaims = await _userManager.GetClaimsAsync(user);

            var model = new UserClaimsViewModel
            {
                UserId = userId
            };

            foreach(var claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type,
                };

                if (exitingUserClaims.Any(claim => claim.Type == claim.Type))
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
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"USer with Id = {model.UserId} cannot found";
                return View();
            }

            var claims = await _userManager.GetClaimsAsync(user);
            foreach (var claim in claims)
            {
                var result = await _userManager.RemoveClaimAsync(user, claim);


                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "cannot remove user existing claims");
                    return View(model);
                }
            }
                foreach (var claim in model.Cliams.Where(c => c.IsSelected))
            {
                var newClaim = new Claim(claim.ClaimType, claim.ClaimType);
                var addClaimResult = await _userManager.AddClaimAsync(user, newClaim);

                if (!addClaimResult.Succeeded)
                {
                    ModelState.AddModelError("", "cannot add selected claims to the user");
                    return View(model);
                }
            }
            return RedirectToAction("EditUser", new { Id = model.UserId });
        }
    }
}
