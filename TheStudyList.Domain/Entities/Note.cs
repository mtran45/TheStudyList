using System;
using System.Collections.Generic;

namespace TheStudyList.Domain.Entities
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<DateTime> ReviewLog { get; set; }
        public DateTime DueDate { get; set; }
        public string Notebook { get; set; }
        public string Topic { get; set; }
        public Duration TimeEstimate { get; set; }
        public bool Suspended { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }

        public TimeSpan LastInterval => ReviewLog.Count >= 2 ? 
            ReviewLog[ReviewLog.Count - 1].Subtract(ReviewLog[ReviewLog.Count - 2]) 
            : TimeSpan.Zero;

        public Note()
        {
            ReviewLog = new List<DateTime>();
        }
    }

    public enum Duration
    {
        Short, Medium, Long
    }
}
