using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using TheStudyList.Domain.Abstract;
using Microsoft.AspNet.Identity.EntityFramework;
using TheStudyList.Domain.Entities;

namespace TheStudyList.Domain.Concrete
{
    public class EFDbContext : IdentityDbContext<User>, IDbContext
    {
        public DbSet<Note> Notes { get; set; }
        public DbSet<Book> Books { get; set; }
        // Users DbSet defined in base class

        public EFDbContext() : base("name=DefaultConnection") { }

        public static EFDbContext Create()
        {
            return new EFDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new IdentityUserLoginConfiguration());
            modelBuilder.Configurations.Add(new IdentityUserRoleConfiguration());
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
                    dbEntry.Links = note.Links;
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
            return Notes.Find(id);
        }

        public void SaveBook(Book book)
        {
            if (book.Id == 0)
            {
                Books.Add(book);
            }
            else
            {
                Book dbEntry = Books.Find(book.Id);
                if (dbEntry != null)
                {
                    dbEntry.Title = book.Title;
                    dbEntry.Links = book.Links;
                    dbEntry.Topic = book.Topic;
                    dbEntry.Progress = book.Progress;
                    dbEntry.Status = book.Status;
                }
            }
        }

        public void DeleteBook(Book book)
        {
            Books.Remove(book);
            SaveChanges();
        }

        public Book GetBookByID(int? id)
        {
            return Books.Find(id);
        }

        public User GetUserByID(string id)
        {
            return Users.Find(id);
        }

        #region Identity
        public class IdentityUserLoginConfiguration : EntityTypeConfiguration<IdentityUserLogin>
        {

            public IdentityUserLoginConfiguration()
            {
                HasKey(iul => iul.UserId);
            }

        }

        public class IdentityUserRoleConfiguration : EntityTypeConfiguration<IdentityUserRole>
        {

            public IdentityUserRoleConfiguration()
            {
                HasKey(iur => iur.RoleId);
            }

        }
        #endregion
    }
}
