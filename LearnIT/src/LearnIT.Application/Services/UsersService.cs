﻿using LearnIT.Domain.Entities;
using AutoMapper;
using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Application.Interfaces.Services;
using LearnIT.Application.Models;

namespace LearnIT.Application.Services
{
    public class UsersService(IUsersRepository usersRepository, IMapper mapper) : IUsersService
    {
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<List<UserDTO>> GetUsersAsync()
        {
            List<User> users = await _usersRepository.GetAllAsync();
            List<UserDTO> userDtos = _mapper.Map<List<User>, List<UserDTO>>(users);
            return userDtos;
        }

        public async Task AddUserAsync(AddUserModel addedUser)
        {
            User user = _mapper.Map<AddUserModel, User>(addedUser);
            await _usersRepository.AddAsync(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            await _usersRepository.DeleteAsync(user);
        }
    }
}