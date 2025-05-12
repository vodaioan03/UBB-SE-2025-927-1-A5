﻿using DuoClassLibrary.Models;

namespace DuoClassLibrary.Interfaces.Services
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(User user);
        Task<User> GetByIdAsync(int userId);
        Task<User> GetByUsernameAsync(string username);
        Task IncrementUserProgressAsync(int userId);
        Task UpdateUserSectionProgressAsync(int userId, int newNrOfSectionsCompleted, int newNrOfQuizzesInSectionCompleted);
        Task UpdateUserAsync(User user);
    }
}