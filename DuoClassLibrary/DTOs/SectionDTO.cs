﻿using DuoClassLibrary.Models.Sections;

namespace DuoClassLibrary.DTOs
{
    public class SectionDTO
    {
        public int Id { get; set; }
        public int? SubjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int RoadmapId { get; set; }
        public int? OrderNumber { get; set; }
        public List<int> QuizIds { get; set; }
        public int? ExamId { get; set; }

        public static SectionDTO ToDto(Section section)
        {
            return new SectionDTO
            {
                Id = section.Id,
                SubjectId = section.SubjectId,
                Title = section.Title,
                Description = section.Description,
                RoadmapId = section.RoadmapId,
                OrderNumber = section.OrderNumber,
                QuizIds = section.Quizzes?.Select(q => q.Id).ToList() ?? [],
                ExamId = section.Exam?.Id
            };
        }
    }
}
