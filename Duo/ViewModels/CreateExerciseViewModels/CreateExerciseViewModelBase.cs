using DuoClassLibrary.Models.Exercises;
using DuoClassLibrary.Models;
using Duo.ViewModels.Base;

namespace Duo.ViewModels.ExerciseViewModels
{
    internal abstract class CreateExerciseViewModelBase : ViewModelBase
    {
        public abstract Exercise CreateExercise(string question, Difficulty difficulty);
    }
}
