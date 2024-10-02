using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class ClubController : Controller
    {
        
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _contextAccessor;

        public ClubController(IClubRepository clubRepository , IPhotoService photoService,IHttpContextAccessor httpContextAccessor) {
           
            _clubRepository = clubRepository;
            _photoService = photoService;
            _contextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            var clubs = await _clubRepository.GetAll();
            return View(clubs);
        }
        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetByIdAsync(id);
            if(club == null)
            {
                return View("Error");
            }
            return View(club);
        }
        public IActionResult Create()
        {
            var user = _contextAccessor.HttpContext.User.GetUserId();
            var CreateClubViewModel = new CreateClubViewModel {AppUserId=user};
            return View(CreateClubViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel club) {
            if (ModelState.IsValid)
            {
                var res = await _photoService.AddPhotoAsync(club.Image);
                var _club = new Club
                {
                    Title = club.Title,
                    Description = club.Description,
                    Image = res.Url.ToString(),
                    AppUserId=club.AppUserId,
                    Address = new Address
                    {
                        City = club.Address.City,
                        State = club.Address.State,
                        Street = club.Address.Street,
                    }
                };
                _clubRepository.Add(_club);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo Upload File");
            }
            return View(club);
            
        }
        [HttpGet]
        public async Task<IActionResult>Edit(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if(club == null) return View("Error");
            var clubnw = new EditClubViewModel
            {
                Title=club.Title,
                Description=club.Description,
                ClubCategory = club.ClubCategory,
                Address=club.Address,
                AddressId=club.AddressId,
                Url=club.Image,
            };
            return View(clubnw);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel club)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed To Edit");
                return View("Edit", club);
            }
            var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);
            if (userClub != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userClub.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Couldn't Delete Photo");
                    return View(club);
                }
                var photoRes = await _photoService.AddPhotoAsync(club.Image);
                var club_new = new Club
                {
                    Id = id,
                    Title = club.Title,
                    Description = club.Description,
                    Image = photoRes.Url.ToString(),
                    AddressId = club.AddressId,
                    Address = club.Address
                };
                _clubRepository.Update(club_new);
                return RedirectToAction("Index");
            }
            else
            {
                return View(club);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var clubDetails = await _clubRepository.GetByIdAsync(id);
            if (clubDetails != null) { 
                return View(clubDetails);
            }
            return View("Error");
        }
        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var clubDetails = await _clubRepository.GetByIdAsync(id);
            if(clubDetails == null) return View("Error");
            _clubRepository.Delete(clubDetails);
            return RedirectToAction("Index");
        }

    }
}
