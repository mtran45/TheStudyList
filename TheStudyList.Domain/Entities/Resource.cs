using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheStudyList.Domain.Entities
{
    public class Resource
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public virtual Book Book { get; set; }
        public virtual Note Note { get; set; }

        public bool IsEvernote() => Url.Contains("evernote:///");
    }
}
