using DuoClassLibrary.Models.Roadmaps;

namespace DuoClassLibrary.Interfaces.Services
{
    public interface IRoadmapService
    {
        // Task<int> AddAsync(Roadmap roadmap);
        // Task DeleteAsync(Roadmap roadmap);
        // Task<List<Roadmap>> GetAllAsync();
        // Task<Roadmap> GetByNameAsync(string roadmapName);
        Task<Roadmap> GetByIdAsync(int roadmapId);
    }
}