using DuoClassLibrary.Models.Roadmap;
using Microsoft.AspNetCore.Mvc;

namespace Duo.Web.ViewModels
{
    public class RoadmapViewModel
    {
        public Roadmap Roadmap { get; set; }
        public List<SectionViewModel> Sections { get; set; }
    }
}
