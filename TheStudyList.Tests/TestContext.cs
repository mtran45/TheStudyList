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
            this.Resources = new TestDbSet<Resource>();
            this.Reviews = new TestDbSet<Review>();
            this.Users = new TestDbSet<User>();
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Review> Reviews { get; set;  }
        public IDbSet<User> Users { get; set; }

        public int SaveChangesCount { get; private set; }

        public int SaveChanges()
        {
            SaveChangesCount++;
            return 1;
        }

        public void InsertNote(Note note)
        {
            Notes.Add(note);
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

        public void InsertBook(Book book)
        {
            Books.Add(book);
        }

        public void DeleteBook(Book book)
        {
            Books.Remove(book);
        }

        public Book GetBookByID(int? id)
        {
            return Books.FirstOrDefault(b => b.Id == id);
        }

        public void InsertResource(Resource resource)
        {
            Resources.Add(resource);
        }

        public void DeleteResource(Resource resource)
        {
            Resources.Remove(resource);
        }

        public void InsertReview(Review review)
        {
            Reviews.Add(review);
        }

        public User GetUserByID(string id)
        {
            return Users.FirstOrDefault(u => u.Id == id);
        }
    }
}
