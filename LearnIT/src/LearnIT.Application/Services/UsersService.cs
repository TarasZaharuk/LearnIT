using LearnIT.Domain.Entities;
using AutoMapper;
using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Application.Interfaces.Services;
using Shared;
using LearnIT.Application.Interfaces.Services.UsersEmailService;
using Shared.AddUserResponse;
using LearnIT.Application.Models;

namespace LearnIT.Application.Services
{
    public class UsersService(IUsersRepository usersRepository, ITutorsRepository tutorsRepository, IMapper mapper, IEmailConfirmationService emailConfirmationService) : IUsersService
    {
        private readonly ITutorsRepository _tutorsRepository = tutorsRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IEmailConfirmationService _emailConfirmationService = emailConfirmationService;

        public async Task<List<UserDTO>> GetAsync()
        {
            List<User> users = await _usersRepository.GetAllAsync();
            List<UserDTO> userDtos = _mapper.Map<List<User>, List<UserDTO>>(users);
            return userDtos;
        }

        public async Task<AddUserResponse> AddAsync(AddUserModel addedUser)
        {
            bool userWithEmaiExist = await IsExistUserWithEmail(addedUser.Email);
            if (userWithEmaiExist)
                return new AddUserResponse(AddingUserIssue.DuplicateEmail);

            User user = _mapper.Map<AddUserModel, User>(addedUser);
            int userId = await _usersRepository.AddAsync(user);

            EmailSendingIssues issue = await _emailConfirmationService.SendEmailConfirmationToAsync(userId);

            if (issue is EmailSendingIssues.None)
                return new AddUserResponse(AddingUserIssue.None);
            if (issue is EmailSendingIssues.InvalidRecipient)
                return new AddUserResponse(AddingUserIssue.EmailAddressDoesNotExist);

            return new AddUserResponse(AddingUserIssue.UnhandledError);
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

            userDTO.TutorId = tutor?.Id;

            return userDTO;
        }

        public async Task<UserDTO?> GetByIdAsync(int id)
        {
            User? user = await _usersRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            UserDTO userDTO = _mapper.Map<User, UserDTO>(user);
            Tutor? tutor = await _tutorsRepository.GetByUserIdAsync(user.Id);
            userDTO.TutorId = tutor?.Id;

            return userDTO;
        }

        public async Task<bool> IsEmailConfirmed(int userId)
        {
            User? user = await _usersRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            return user.EmailConfirmed;
        }

        private async Task<bool> IsExistUserWithEmail(string email)
        {
            User? user = await _usersRepository.GetByEmailAsync(email);
            return user != null;
        }

        public async Task<string?> GetEmailByIdAsync(int id)
        {
            User? user = await _usersRepository.GetByIdAsync(id);
            return user?.Email;
        }
    }
}
