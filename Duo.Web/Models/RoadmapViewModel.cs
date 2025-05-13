using DuoClassLibrary.Models.Roadmap;
using DuoClassLibrary.Models.Sections;
using System.Collections.Generic;

namespace Duo.Web.Models
{
    public class RoadmapViewModel
    {
        public string Name { get; set; }
        public List<Section> Sections { get; set; } = new();
        public int NumberOfCompletedSections { get; set; }
    }
}
