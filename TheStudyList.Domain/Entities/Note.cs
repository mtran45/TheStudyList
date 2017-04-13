using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheStudyList.Domain.Entities
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int IntervalInDays { get; set; } // Num of days until next review
        public DateTime DueDate { get; set; }
        public DateTime FirstStudiedDate { get; set; }
        public string Notebook { get; set; }
        public string Topic { get; set; }
        public Duration TimeEstimate { get; set; }
        public bool Suspended { get; set; }

        // Navigation properties
        [Required]
        public virtual User User { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
        public virtual IList<Review> Reviews { get; set; }

        public Note()
        {
            DueDate = DateTime.UtcNow.Date.Add(TimeSpan.FromDays(1));
        }
    }

    public enum Duration
    {
        Short, Medium, Long
    }
}
