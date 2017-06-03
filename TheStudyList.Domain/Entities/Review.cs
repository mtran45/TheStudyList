using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheStudyList.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public virtual Note Note { get; set; }

        // Return the number of days since this note was reviewed in the user's timezone
        public int DaysSinceReviewed()
        {
            DateTime todayLocal = DateTime.UtcNow.ToLocalTime(Note.User).Date;
            DateTime reviewDateLocal = Date.ToLocalTime(Note.User).Date;
            TimeSpan span = todayLocal.Subtract(reviewDateLocal);
            int days = (int)span.TotalDays;
            return days;
        }
    }
}
