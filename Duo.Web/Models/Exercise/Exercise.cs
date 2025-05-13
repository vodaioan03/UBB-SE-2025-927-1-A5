namespace Duo.Web.Models.Exercise
{
    public abstract class Exercise
    {
        public int ExerciseId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
    }
}
