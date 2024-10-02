using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository dashrepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photService;
        public DashboardController(IDashboardRepository dashboardRepository,
            IHttpContextAccessor httpContextAccessor,IPhotoService photoService)
        {
            dashrepo = dashboardRepository;
            _httpContextAccessor = httpContextAccessor;
            _photService = photoService;
        }
        public async Task<IActionResult> Index()
        {
            var userClubs=await dashrepo.GetAllUserClubs();
            var userRaces=await dashrepo.GetAllUserRaces();
            var UserViewModel = new DashboardViewModel()
            {
                Clubs = userClubs,
                Races = userRaces
            }
            ;
            return View(UserViewModel);
        }
        public async Task<IActionResult> EditUserProfile()
        {
            var curUserID = _httpContextAccessor.HttpContext.User.GetUserId();
            var user = await dashrepo.GetUserById(curUserID);
            if (user == null) { return View("Error"); }
            var editUserVM = new EditUserDashboardViewModel()
            {
                Id = curUserID,
                Pace = user.Pace,
                Mileage = user.Mileage,
                ProfileImageUrl=user.ProfileImageUrl,
                City=user.City,
                State=user.State
            };
            return View(editUserVM);
        }
        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserDashboardViewModel EditUserVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed To Edit");
                return View("EditUserProfile", EditUserVM);
            }
            var user = await dashrepo.GetUserByIdNoTracking(EditUserVM.Id);
            if (user.ProfileImageUrl == "" || user.ProfileImageUrl == null)
            {
                var photo = await _photService.AddPhotoAsync(EditUserVM.Image);
                user.Id=EditUserVM.Id;
                user.Pace = EditUserVM.Pace;
                user.Mileage= EditUserVM.Mileage;
                user.ProfileImageUrl=photo.Url.ToString();
                user.City=EditUserVM.City;
                user.State=EditUserVM.State;
                dashrepo.Update(user);
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    await _photService.DeletePhotoAsync(user.ProfileImageUrl);
                }
                catch (Exception ex) {
                    ModelState.AddModelError("", "Couldn't Delete");
                    return View(EditUserVM);
                }
                var photo = await _photService.AddPhotoAsync(EditUserVM.Image);
                user.Id = EditUserVM.Id;
                user.Pace = EditUserVM.Pace;
                user.Mileage = EditUserVM.Mileage;
                user.ProfileImageUrl = photo.Url.ToString();
                user.City = EditUserVM.City;
                user.State = EditUserVM.State;
                dashrepo.Update(user);
                return RedirectToAction("Index");
            }
        }
    }
}
