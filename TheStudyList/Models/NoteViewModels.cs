using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TheStudyList.Domain.Entities;

namespace TheStudyList.Models
{
    public class StudyListViewModel
    {
        public List<Note> Notes { get; set; }
        public string CurrentNotebook { get; set; }
    }

    public class CreateNoteViewModel
    {
        [Required]
        public string Title { get; set; }

        public string Notebook { get; set; }

        [Required]
        [DisplayName("Interval (Days)")]
        public int IntervalInDays { get; set; }

        [Required]
        [DisplayName("First Studied Date")]
        public DateTime FirstStudiedDate { get; set; }

        public List<Resource> Resources { get; set; }

        [DisplayName("Time Estimate")]
        public Duration TimeEstimate { get; set; }

        public CreateNoteViewModel()
        {
            IntervalInDays = 1;
            FirstStudiedDate = DateTime.UtcNow;
        }
    }

    public class EditNoteViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Notebook { get; set; }

        [Required]
        [DisplayName("Interval (Days)")]
        public int IntervalInDays { get; set; }

        [Required]
        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        [DisplayName("First Studied Date")]
        public DateTime FirstStudiedDate { get; set; }

        [DisplayName("Time Estimate")]
        public Duration TimeEstimate { get; set; }

        public Resource[] Resources { get; set; }
    }
}