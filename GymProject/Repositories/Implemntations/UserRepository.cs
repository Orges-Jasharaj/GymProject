using GymProject.Data;
using GymProject.Dtos.Responses;
using GymProject.Models;
using GymProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymProject.Repositories.Implemntations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<UserDto>> GetAllUsersAsync(bool includeInactive)
        {
            IQueryable<User> query = _appDbContext.Users;

            if (includeInactive)
                query = query.IgnoreQueryFilters();

            return await query
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    DateOfBirth = u.DateOfBirth,
                    isActive = u.isActive,
                })
                .ToListAsync();
        }

        public async Task<bool> ReactivateUserAsync(string userId)
        {
            var user = await _appDbContext.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            user.isActive = true;
            await _appDbContext.SaveChangesAsync();
            return true;
        }
    }
}
