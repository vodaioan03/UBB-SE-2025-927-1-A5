using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using DuoClassLibrary.Models.Exercises;
using DuoClassLibrary.Models.Sections;

namespace DuoClassLibrary.Models.Quizzes
{
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class Quiz
    {
        #region Fields and Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? SectionId { get; set; }

        public Section? Section { get; set; }

        public int? OrderNumber { get; set; }

        public ICollection<Exercise> Exercises { get; set; } = [];

        private int numberOfAnswersGiven = 0;
        private int numberOfCorrectAnswers = 0;

        private const int MAX_EXERCISES = 10;
        private const double PASSING_THRESHOLD = 75;

        #endregion

        #region Constructors

        public Quiz()
        {
        }

        public Quiz(int id, int? sectionId, int? orderNumber)
        {
            Id = id;
            SectionId = sectionId;
            OrderNumber = orderNumber;
        }

        #endregion

        #region Methods

        public bool AddExercise(Exercise exercise)
        {
            if (Exercises.Count < MAX_EXERCISES)
            {
                Exercises.Add(exercise);
                return true;
            }
            return false;
        }

        public bool RemoveExercise(Exercise exercise)
        {
            return Exercises.Remove(exercise);
        }

        public bool IsValid()
        {
            return Exercises.Count == MAX_EXERCISES;
        }

        public double GetPassingThreshold()
        {
            return PASSING_THRESHOLD;
        }

        public int GetNumberOfAnswersGiven()
        {
            return numberOfAnswersGiven;
        }

        public int GetNumberOfCorrectAnswers()
        {
            return numberOfCorrectAnswers;
        }

        public void IncrementCorrectAnswers()
        {
            numberOfCorrectAnswers++;
            numberOfAnswersGiven++;
        }

        public void IncrementAnswersGiven()
        {
            numberOfAnswersGiven++;
        }

        public override string ToString()
        {
            var progress = numberOfAnswersGiven > 0
                ? $"Progress: {numberOfCorrectAnswers}/{numberOfAnswersGiven} ({((double)numberOfCorrectAnswers / numberOfAnswersGiven) * 100:F1}%)"
                : "Not started";
            return $"Quiz ID: {Id}, Exercises Count: {Exercises.Count}, {progress}, Order: {OrderNumber ?? 0}";
        }

        #endregion
    }
}
