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
    }
}
