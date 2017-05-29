using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public string ReturnUrl { get; set; }
    }

    public class StudyViewModel
    {
        public Note Note { get; set; }
        public int? Interval { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class StatsViewModel
    {
        public List<Note> Notes { get; set; }
        public List<Review> Reviews { get; set; }
        public DateTime TodayLocal { get; set; }
        public string Notebook { get; set; }

        public int[] Forecast(int days)
        {
            var forecast = new int[days];
            for (int i = 0; i < days; i++)
            {
                forecast[i] = Notes.Count(note => note.DaysUntilDue() == i);
            }
            return forecast;
        }

        public int[] ReviewCount(int days)
        {
            var reviewCount = new int[days];
            for (int i = 0; i < days; i++)
            {
                reviewCount[days - 1 - i] = Reviews.Count(r => r.DaysSinceReviewed() == i);
            }
            return reviewCount;
        }

        public int[] FirstStudied(int days)
        {
            var firstStudied = new int[days];
            for (int i = 0; i < days; i++)
            {
                firstStudied[days - 1 - i] = Notes.Count(note => note.DaysSinceFirstStudied() == i);
            }
            return firstStudied;
        }
    }
}