using System;
using System.Collections.Generic;

namespace TheStudyList.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Progress { get; set; }
        public string Category { get; set; }
        public string Topic { get; set; }
        public BookStatus Status { get; set; }
        public DateTime Completed { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<Resource> Links { get; set; }
    }

    public enum BookStatus { ToRead, Reading, Finished, Postponed }
}
