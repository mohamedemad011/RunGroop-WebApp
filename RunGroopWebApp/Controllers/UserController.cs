using Microsoft.AspNetCore.Mvc;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.ViewModels;

namespace RunGroopWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var users= await _userRepository.GetAllUsers();
            var res = new List<UserViewModel>();
            foreach (var user in users) {
                var cur = new UserViewModel()
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Pace = user.Pace,
                    Mileage = user.Mileage,
                    ProfileImageUrl = user.ProfileImageUrl
                };
                res.Add(cur);
            }
            return View(res);
        }
        public async Task<IActionResult>Detail(string id)
        {
            var user =await _userRepository.GetUserById(id);
            var cur = new UserDetailViewModel()
            {
                Id=user.Id,
                Username=user.UserName,
                Mileage=user.Mileage,
                Pace=user.Pace,
            };
            return View(cur);
        }
    }
}
