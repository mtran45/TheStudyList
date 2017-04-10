using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheStudyList.Domain.Abstract;
using TheStudyList.Domain.Entities;

namespace TheStudyList.Tests
{
    public class TestContext : IDbContext
    {
        public TestContext()
        {
            this.Notes = new TestDbSet<Note>();
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public IDbSet<User> Users { get; set; }

        public int SaveChangesCount { get; private set; }

        public int SaveChanges()
        {
            SaveChangesCount++;
            return 1;
        }

        public void SaveNote(Note note)
        {
            if (note.Id == 0)
            {
                Notes.Add(note);
            }
            else
            {
                Note dbEntry = Notes.Find(note.Id);
                if (dbEntry != null)
                {
                    dbEntry.Title = note.Title;
                    dbEntry.Resources = note.Resources;
                    dbEntry.Notebook = note.Notebook;
                    dbEntry.Topic = note.Topic;
                    dbEntry.TimeEstimate = note.TimeEstimate;
                }
            }
            SaveChanges();
        }

        public void DeleteNote(Note note)
        {
            Notes.Remove(note);
            SaveChanges();
        }

        public Note GetNoteByID(int? id)
        {
            return Notes.FirstOrDefault(n => n.Id == id);
        }

        public void SaveBook(Book book)
        {
            throw new NotImplementedException();
        }

        public void DeleteBook(Book book)
        {
            throw new NotImplementedException();
        }

        public Book GetBookByID(int? id)
        {
            throw new NotImplementedException();
        }

        public User GetUserByID(string id)
        {
            throw new NotImplementedException();
        }
    }
}
