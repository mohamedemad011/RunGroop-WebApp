using Microsoft.EntityFrameworkCore;
using RunGroopWebApp.Data;
using RunGroopWebApp.Interfaces;
using RunGroopWebApp.Models;

namespace RunGroopWebApp.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public DashboardRepository(ApplicationDbContext context,IHttpContextAccessor httpContext) { 
            _context = context;
            _contextAccessor = httpContext;
        }
        public async Task<List<Club>> GetAllUserClubs()
        {
            var CurUser = _contextAccessor.HttpContext.User.GetUserId();
            var UserClubs = _context.Clubs.Where(r => r.AppUser.Id == CurUser).ToList();
            return UserClubs;
        }

        public async Task<List<Race>> GetAllUserRaces()
        {
            var CurUser = _contextAccessor.HttpContext.User.GetUserId();
            var UserRaces = _context.Races.Where(r => r.AppUser.Id == CurUser).ToList();
            return UserRaces;
        }

        public async Task<AppUser> GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<AppUser> GetUserByIdNoTracking(string id)
        {
            return await _context.Users.Where(u=>u.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }

        public bool Save()
        {
            var saved=_context.SaveChanges();
            return saved>0?true : false;
        }

        public bool Update(AppUser user)
        {
            _context.Update(user);
            return Save();
        }
    }
}
