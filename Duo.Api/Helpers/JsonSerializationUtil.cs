﻿using Duo.Api.Models.Exercises;
using Duo.Api.Models.Quizzes;
using Duo.Api.Models.Sections;
using Duo.Api.Repositories;
using System.Text.Json;

namespace Duo.Api.Helpers
{
    public class JsonSerializationUtil
    {
        public static string SerializeExamWithTypedExercises(Exam exam)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
            };

            var exerciseJsonList = new List<JsonDocument>();

            foreach (var exercise in exam.Exercises)
            {
                string exerciseJson = exercise.Type switch
                {
                    "Association" => JsonSerializer.Serialize((AssociationExercise)exercise, options),
                    "Flashcard" => JsonSerializer.Serialize((FlashcardExercise)exercise, options),
                    "MultipleChoice" => JsonSerializer.Serialize((MultipleChoiceExercise)exercise, options),
                    "FillInTheBlank" => JsonSerializer.Serialize((FillInTheBlankExercise)exercise, options),
                    _ => throw new InvalidOperationException($"Unknown exercise type: {exercise.Type}")
                };

                exerciseJsonList.Add(JsonDocument.Parse(exerciseJson));
            }

            var examDto = new
            {
                exam.Id,
                exam.SectionId,
                Exercises = exerciseJsonList.Select(doc => doc.RootElement).ToList(),
            };

            return JsonSerializer.Serialize(examDto, options);
        }

        public static async Task<Exam> DeserializeExamWithTypedExercises(string json, IRepository repo)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            Console.WriteLine($"Root Element Type: {root.ValueKind}"); // Check root type


            int id = root.GetProperty("Id").GetInt32();
            int? sectionId = root.TryGetProperty("SectionId", out var sectionProp) && sectionProp.ValueKind != JsonValueKind.Null
                ? sectionProp.GetInt32()
                : null;

            var exam = new Exam
            {
                Id = id,
                SectionId = sectionId,
                Exercises = new List<Exercise>()
            };

            var exerciseIds = root.GetProperty("Exercises").EnumerateArray().Select(e => e.GetInt32());
            foreach (var exerciseId in exerciseIds)
            {
                var exercise = await repo.GetExerciseByIdAsync(exerciseId);
                if (exercise == null)
                {
                    throw new InvalidOperationException($"Exercise with ID {exerciseId} not found.");
                }

                exam.Exercises.Add(exercise);
            }

            return exam;
        }

        public static async Task<Quiz> DeserializeQuiz(string json, IRepository repo)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            Console.WriteLine($"Root Element Type: {root.ValueKind}"); // Check root type


            int id = root.GetProperty("Id").GetInt32();
            int? sectionId = root.TryGetProperty("SectionId", out var sectionProp) && sectionProp.ValueKind != JsonValueKind.Null
                ? sectionProp.GetInt32()
                : null;
            int? orderNumber = root.TryGetProperty("OrderNumber", out var orderProp) && orderProp.ValueKind != JsonValueKind.Null
                ? orderProp.GetInt32()
                : null;


            var quiz = new Quiz
            {
                Id = id,
                SectionId = sectionId,
                OrderNumber = orderNumber,
                Exercises = new List<Exercise>(),
            };

            var exerciseIds = root.GetProperty("Exercises").EnumerateArray().Select(q => q.GetInt32());
            foreach (var exerciseId in exerciseIds)
            {
                var exercise = await repo.GetExerciseByIdAsync(exerciseId);
                if (exercise == null)
                {
                    throw new InvalidOperationException($"Exercise with ID {exerciseId} not found.");
                }

                quiz.Exercises.Add(exercise);
            }

            return quiz;
        }

        public static async Task<Section> DeserializeSection(string json, IRepository repo)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            using JsonDocument doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // ID is optional since it's auto-generated
            int id = root.TryGetProperty("id", out var idProp) ? idProp.GetInt32() : 0;

            int? subjectId = root.TryGetProperty("subjectId", out var subjectProp) && subjectProp.ValueKind != JsonValueKind.Null
                ? subjectProp.GetInt32()
                : null;

            string title = root.GetProperty("title").GetString() ?? throw new InvalidOperationException("Title is required");
            string description = root.GetProperty("description").GetString() ?? throw new InvalidOperationException("Description is required");

            int roadmapId = root.TryGetProperty("roadmapId", out var roadmapProp) && roadmapProp.ValueKind != JsonValueKind.Null
                ? roadmapProp.GetInt32()
                : throw new InvalidOperationException("RoadmapId is required");

            int? orderNumber = root.TryGetProperty("orderNumber", out var orderProp) && orderProp.ValueKind != JsonValueKind.Null
                ? orderProp.GetInt32()
                : null;

            var section = new Section
            {
                Id = id,
                SubjectId = subjectId,
                Title = title,
                Description = description,
                RoadmapId = roadmapId,
                OrderNumber = orderNumber,
                Roadmap = await repo.GetRoadmapByIdAsync(roadmapId)
            };

            // Handle nested quizzes
            if (root.TryGetProperty("quizzes", out var quizzesElement) && quizzesElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var quizElement in quizzesElement.EnumerateArray())
                {
                    if (quizElement.TryGetProperty("id", out var quizIdProp))
                    {
                        int quizId = quizIdProp.GetInt32();
                        var quiz = await repo.GetQuizByIdAsync(quizId);
                        if (quiz == null)
                        {
                            throw new InvalidOperationException($"Quiz with ID {quizId} not found.");
                        }
                        section.Quizzes.Add(quiz);
                    }
                }
            }

            // Handle nested exam
            if (root.TryGetProperty("exam", out var examElement) && examElement.ValueKind == JsonValueKind.Object)
            {
                if (examElement.TryGetProperty("id", out var examIdProp))
                {
                    int examId = examIdProp.GetInt32();
                    var exam = await repo.GetExamByIdAsync(examId);
                    if (exam == null)
                    {
                        throw new InvalidOperationException($"Exam with ID {examId} not found.");
                    }
                    section.Exam = exam;
                }
            }

            return section;
        }

    }
}
