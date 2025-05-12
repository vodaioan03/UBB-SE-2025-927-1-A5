using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using DuoClassLibrary.Models.Exercises;
using DuoClassLibrary.Models.Sections;

namespace DuoClassLibrary.Models.Quizzes
{
    /// <summary>
    /// Represents an exam, a specialized type of quiz with additional constraints or behaviors.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Serializable]
    public class Exam
    {
        #region Fields and Properties

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? SectionId { get; set; }

        public Section? Section { get; set; }

        public ICollection<Exercise> Exercises { get; set; } = [];

        private const int MAX_EXERCISES = 20;

        #endregion

        #region Constructors

        public Exam()
        {
        }

        public Exam(int id, int? sectionId)
        {
            Id = id;
            SectionId = sectionId;
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

        public override string ToString()
        {
            return $"Exam ID: {Id}, Exercises Count: {Exercises.Count}";
        }

        #endregion
    }
}
