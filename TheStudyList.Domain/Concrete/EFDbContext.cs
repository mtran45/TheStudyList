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
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Review> Reviews { get; set; }
        // Users DbSet defined in base class

        public EFDbContext() : base("name=DefaultConnection", throwIfV1Schema: false) { }

        public static EFDbContext Create()
        {
            return new EFDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new IdentityUserLoginConfiguration());
            modelBuilder.Configurations.Add(new IdentityUserRoleConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public void InsertNote(Note note)
        {
            Notes.Add(note);
        }

        public void DeleteNote(Note note)
        {
            Notes.Remove(note);
        }

        public void UpdateNote(Note note)
        {
            Entry(note).State = EntityState.Modified;
        }

        public Note GetNoteByID(int? id)
        {
            return Notes.Find(id);
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
            return Books.Find(id);
        }

        public void InsertResource(Resource resource)
        {
            Resources.Add(resource);
        }

        public void DeleteResource(Resource resource)
        {
            Resources.Attach(resource);
            Resources.Remove(resource);
        }

        public void UpdateResource(Resource resource)
        {
            Entry(resource).State = EntityState.Modified;
        }

        public void InsertReview(Review review)
        {
            Reviews.Add(review);
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
