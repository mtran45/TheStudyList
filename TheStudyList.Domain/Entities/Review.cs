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

        public DateTime DateLocal()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(Date, Note.User.TimeZone);
        }

        // Return the number of days since this note was reviewed in the user's timezone
        public int DaysSinceReviewed()
        {
            TimeSpan span = Note.User.NowLocal().Date.Subtract(DateLocal().Date);
            int days = (int)span.TotalDays;
            return days;
        }
    }
}
