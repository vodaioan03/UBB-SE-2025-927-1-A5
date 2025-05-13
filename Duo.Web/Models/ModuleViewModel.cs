using DuoClassLibrary.Models;

namespace Duo.Web.Models
{
    public class ModuleViewModel
    {
        public Module Module { get; set; } = null!;
        public int CourseId { get; set; }

        public string TimeSpent { get; set; } = "00:00:00";
        public int CoinBalance { get; set; }

        public bool IsCompleted { get; set; }
    }
}
