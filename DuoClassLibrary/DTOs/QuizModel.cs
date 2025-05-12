namespace DuoClassLibrary.DTOs
{
    [Serializable]
    public class QuizModel
    {
        public int Id { get; set; }
        public int? SectionId { get; set; }
        public List<int>? ExerciseIds { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
