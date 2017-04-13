using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TheStudyList.Domain.Entities;

namespace TheStudyList.Models
{
    public class CreateNoteViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public DateTime FirstStudiedDate { get; set; }
        public List<Resource> Resources { get; set; } 
    }
}