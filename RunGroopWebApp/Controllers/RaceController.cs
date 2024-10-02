using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Models;
using RunGroopWebApp.Repository;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _raceRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _contextAccessor;
        public RaceController(IRaceRepository raceRepository, IPhotoService photoService,IHttpContextAccessor httpContextAccessor)
        {
            _raceRepository = raceRepository;
            _photoService = photoService;
            _contextAccessor = httpContextAccessor;
        }
        public async Task<ActionResult> Index()
        {
            var race = await _raceRepository.GetAll();
            return View(race);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            return View(race);
        }
        public IActionResult Create()
        {
            var userId=_contextAccessor.HttpContext.User.GetUserId();
            var CreateRaceViewModel = new CreateRaceViewModel { AppUserId= userId };
            return View(CreateRaceViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel race)
        {
            if (ModelState.IsValid)
            {
                var res = await _photoService.AddPhotoAsync(race.Image);
                var _Race = new Race
                {
                    Title = race.Title,
                    Description = race.Description,
                    Image = res.Url.ToString(),
                    AppUserId=race.AppUserId,
                    Address = new Address
                    {
                        City = race.Address.City,
                        State = race.Address.State,
                        Street = race.Address.Street,
                    }
                };
                _raceRepository.Add(_Race);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo Upload File");
            }
            return View(race);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var race = await _raceRepository.GetByIdAsync(id);
            if (race == null) return View("Error");
            var racenw = new EditRaceViewModel
            {
                Title = race.Title,
                Description = race.Description,
                RaceCategory = race.RaceCategory,
                Address = race.Address,
                AddressId = race.AddressId,
                Url = race.Image,
            };
            return View(racenw);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel race)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed To Edit");
                return View("Edit", race);
            }
            var userClub = await _raceRepository.GetByIdAsyncNoTracking(id);
            if (userClub != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userClub.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Couldn't Delete Photo");
                    return View(race);
                }
                var photoRes = await _photoService.AddPhotoAsync(race.Image);
                var race_new = new Race
                {
                    Id = id,
                    Title = race.Title,
                    Description = race.Description,
                    Image = photoRes.Url.ToString(),
                    AddressId = race.AddressId,
                    Address = race.Address
                };
                _raceRepository.Update(race_new);
                return RedirectToAction("Index");
            }
            else
            {
                return View(race);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var raceDetails = await _raceRepository.GetByIdAsync(id);
            if (raceDetails != null)
            {
                return View(raceDetails);
            }
            return View("Error");
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            var raceDetails = await _raceRepository.GetByIdAsync(id);
            if (raceDetails == null) return View("Error");
            _raceRepository.Delete(raceDetails);
            return RedirectToAction("Index");
        }
    }
}
