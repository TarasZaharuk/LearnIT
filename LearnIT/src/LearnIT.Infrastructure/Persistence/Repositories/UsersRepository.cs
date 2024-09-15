using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearnIT.Infrastructure.Persistence.Repositories
{
    public class UsersRepository(LearnITDBContext usersDBContext) : IUsersRepository
    {
        private readonly LearnITDBContext _usersDBContext = usersDBContext;
        public async Task AddAsync(User user)
        {
            await _usersDBContext.AddAsync(user);
            await _usersDBContext.SaveChangesAsync();
        }

        public async Task AddUsersAsync(List<User> users) 
        {
            await _usersDBContext.AddRangeAsync(users);
            await _usersDBContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(User user)
        {
            _usersDBContext.Users.Remove(user);
            await _usersDBContext.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _usersDBContext.Users.Include(u => u.Gender).ToListAsync(); 
        }

        public async Task<List<User>> GetInDiapazonAsync(int skip, int take)
        {
            return await _usersDBContext.Users.Skip(skip).Take(take).ToListAsync(); 
        }
    }
}
