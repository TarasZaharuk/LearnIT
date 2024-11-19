using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LearnIT.Infrastructure.Persistence.Repositories
{
    public class UsersRepository(LearnITDBContext usersDBContext) : IUsersRepository
    {
        private readonly LearnITDBContext _usersDBContext = usersDBContext;
        public async Task<int> AddAsync(User user)
        {
            await _usersDBContext.AddAsync(user);
            await _usersDBContext.SaveChangesAsync();
            return user.Id;
        }

        public async Task AddAsync(List<User> users) 
        {
            await _usersDBContext.AddRangeAsync(users);
            await _usersDBContext.SaveChangesAsync();
        }
        public async Task DeleteByIdAsync(int id)
        {
            User? user = await _usersDBContext.Users.FindAsync(id);
            if (user == null)
                return;

            _usersDBContext.Users.Remove(user);
            await _usersDBContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _usersDBContext.Users.Include(u => u.Gender).ToListAsync(); 
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _usersDBContext.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _usersDBContext.Users.SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<User>> GetInDiapazonAsync(int skip, int take)
        {
            return await _usersDBContext.Users.Skip(skip).Take(take).ToListAsync(); 
        }
    }
}
