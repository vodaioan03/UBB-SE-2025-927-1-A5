using DuoClassLibrary.Models.Roadmaps;

namespace DuoClassLibrary.Interfaces.Proxies
{
    public interface IRoadmapServiceProxy
    {
        // Task<int> AddAsync(Roadmap roadmap);
        // Task DeleteAsync(Roadmap roadmap);
        // Task<List<Roadmap>> GetAllAsync();
        Task<Roadmap> GetByIdAsync(int roadmapId);
        // Task<Roadmap> GetByNameAsync(string roadmapName);
    }
}