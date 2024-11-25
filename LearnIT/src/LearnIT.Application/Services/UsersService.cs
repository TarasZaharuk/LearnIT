using LearnIT.Domain.Entities;
using AutoMapper;
using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Application.Interfaces.Services;
using Shared;

namespace LearnIT.Application.Services
{
    public class UsersService(IUsersRepository usersRepository, ITutorsRepository tutorsRepository, IMapper mapper) : IUsersService
    {
        private readonly ITutorsRepository _tutorsRepository = tutorsRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<List<UserDTO>> GetAsync()
        {
            List<User> users = await _usersRepository.GetAllAsync();
            List<UserDTO> userDtos = _mapper.Map<List<User>, List<UserDTO>>(users);
            return userDtos;
        }

        public async Task<int> AddAsync(AddUserModel addedUser)
        {
            User user = _mapper.Map<AddUserModel, User>(addedUser);
            int userId = await _usersRepository.AddAsync(user);
            return userId;
        }

        public async Task DeleteByIdAsync(int id)
        {
            await _usersRepository.DeleteByIdAsync(id);
        }

        public async Task<UserDTO?> GetUserByLoginAsync(UserLoginModel userLoginModel)
        {
            User? user = await _usersRepository.GetByEmailAsync(userLoginModel.Email);
            if(user == null || user.Password != userLoginModel.Password)
                return null;

            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            Tutor? tutor = await _tutorsRepository.GetByUserIdAsync(user.Id);
            if (tutor != null)
                userDTO.TutorId = tutor.Id;

            return userDTO;
        }

        public async Task<UserDTO?> GetByIdAsync(int id)
        {
            User? user = await _usersRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            Tutor? tutor = await _tutorsRepository.GetByUserIdAsync(user.Id);
            if (tutor != null)
                userDTO.TutorId = tutor.Id;

            return userDTO;
        }
    }
}
