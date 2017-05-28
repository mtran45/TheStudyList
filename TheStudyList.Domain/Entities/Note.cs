using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        public virtual User User { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
        public virtual IList<Review> Reviews { get; set; }

        public Note()
        {
            DueDate = DateTime.UtcNow.Add(TimeSpan.FromDays(1));
            FirstStudiedDate = DateTime.UtcNow;
        }

        public DateTime DueDateLocal()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DueDate, User.TimeZone);
        }

        [NotMapped]
        public string NotebookInitials {
            get
            {
                // Return the first 2 initials of notebook
                if (Notebook == null) return null;
                return new string(Notebook.Split(' ').Select(w => w[0]).Take(2).ToArray()).ToUpper();
            }
        }

        public void UpdateInterval(int ivl)
        {
            IntervalInDays = ivl;
            DueDate = DateTime.UtcNow.Add(TimeSpan.FromDays(ivl));
        }

        // Return the interval in days for a "hard" review
        public int IntervalHard()
        {
            return IntervalInDays;
        }

        // Return the interval in days for a "good" review
        public int IntervalGood()
        {
            return IntervalInDays == 1 ? 2 : (int)(IntervalInDays * 1.5);
        }

        // Return the interval in days for a "easy" review
        public int IntervalEasy()
        {
            return IntervalInDays == 1 ? 3 : IntervalInDays * 2;
        }

        // Return true if due date is today in user's timezone
        public bool IsDue()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DueDate, User.TimeZone).Date
                   == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, User.TimeZone).Date;
        }

        // Return true if due date has passed in user's timezone
        public bool IsOverdue()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DueDate, User.TimeZone).Date
                < TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, User.TimeZone).Date;
        }
    }

    public enum Duration
    {
        Unset = 0,
        Short = 1,
        Medium = 2,
        Long = 3
    }
}
