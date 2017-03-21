using System;
using System.Collections.Generic;

namespace TheStudyList.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Dictionary<string, string> Links { get; set; }
        public string Progress { get; set; }
        public string Category { get; set; }
        public string Topic { get; set; }
        public BookStatus Status { get; set; }
        public DateTime Completed { get; set; }

        public User User { get; set; }
    }

    public enum BookStatus { Reading, ToRead, Finished, Postponed }
}
