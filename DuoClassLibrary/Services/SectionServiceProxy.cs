using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using DuoClassLibrary.Models.Sections;
using DuoClassLibrary.Services.Interfaces;

namespace DuoClassLibrary.Services
{
    public class SectionServiceProxy : ISectionServiceProxy
    {
        private readonly HttpClient httpClient;
        private readonly string url = "https://localhost:7174";

        public SectionServiceProxy(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<int> AddSection(Section section)
        {
            var response = await httpClient.PostAsJsonAsync($"{url}/api/section/add", section);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<Section>();
            if (result == null)
                throw new InvalidOperationException("Failed to add section.");

            return result.Id;
        }

        public async Task<int> CountSectionsFromRoadmap(int roadmapId)
        {
            var response = await httpClient.GetAsync($"{url}/api/section/get-section-count-on-roadmap?roadmapId={roadmapId}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<int>();
            return result;
        }

        public async Task DeleteSection(int sectionId)
        {
            var response = await httpClient.DeleteAsync($"{url}/api/section/{sectionId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<Section>> GetAllSections()
        {
            var response = await httpClient.GetAsync($"{url}/api/Section/get-all");
            response.EnsureSuccessStatusCode();

            // Directly deserialize the response into a List<Section>
            var sections = await response.Content.ReadFromJsonAsync<List<Section>>();
            if (sections == null)
                throw new InvalidOperationException("No sections found.");

            return sections;
        }

        public async Task<List<Section>> GetByRoadmapId(int roadmapId)
        {
            var response = await httpClient.GetAsync($"{url}/api/section/list/roadmap/{roadmapId}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(jsonResponse);
            var resultElement = jsonDoc.RootElement.GetProperty("result");

            var sections = JsonSerializer.Deserialize<List<Section>>(resultElement.ToString(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return sections ?? new List<Section>();
        }


        public async Task<Section> GetSectionById(int sectionId)
        {
            var response = await httpClient.GetAsync($"{url}/api/section/get-section-by-id?id={sectionId}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(jsonResponse);
            var section = jsonDoc.RootElement.GetProperty("result").Deserialize<Section>();
            if (section == null)
                throw new InvalidOperationException($"Section with ID {sectionId} not found.");

            return section;
        }

        public async Task<int> LastOrderNumberFromRoadmap(int roadmapId)
        {
            var response = await httpClient.GetAsync($"{url}/api/section/get-last-from-roadmap?roadmapId={roadmapId}");
            response.EnsureSuccessStatusCode();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(jsonResponse);
            var lastOrderNumber = jsonDoc.RootElement.GetProperty("result").GetInt32();
            return lastOrderNumber;
        }

        public async Task UpdateSection(Section section)
        {
            var response = await httpClient.PatchAsync($"{url}/api/section/patch", JsonContent.Create(section));
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> TrackCompletion(int sectionId, bool isCompleted)
        {
            var response = await httpClient.PostAsJsonAsync($"{url}/api/sections/completion/{sectionId}/{isCompleted}", new { });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task<List<SectionDependency>> GetSectionDependencies(int sectionId)
        {
            var response = await httpClient.GetAsync($"{url}/api/sections/dependencies/{sectionId}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<SectionDependency>>() ?? new List<SectionDependency>();
        }
    }
}