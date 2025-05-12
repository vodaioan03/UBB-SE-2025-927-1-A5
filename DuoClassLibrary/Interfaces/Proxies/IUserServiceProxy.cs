using DuoClassLibrary.Models;

namespace DuoClassLibrary.Interfaces.Proxies
{
    public interface IUserServiceProxy
    {
        Task<User> GetByIdAsync(int userId);
        Task<User> GetByUsernameAsync(string username);
        Task<int> CreateUserAsync(User user);
        Task UpdateUserSectionProgressAsync(int userId, int newNrOfSectionsCompleted, int newNrOfQuizzesInSectionCompleted);
        Task IncrementUserProgressAsync(int userId);
        Task UpdateUserAsync(User user);
    }
}
