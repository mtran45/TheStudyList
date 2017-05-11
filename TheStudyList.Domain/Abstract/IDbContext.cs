using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheStudyList.Domain.Entities;

namespace TheStudyList.Domain.Abstract
{
    public interface IDbContext
    {
        DbSet<Note> Notes { get; }
        DbSet<Book> Books { get; }
        DbSet<Resource> Resources { get; }
        DbSet<Review> Reviews { get; }
        IDbSet<User> Users { get; }

        int SaveChanges();

        void InsertNote(Note note);
        void DeleteNote(Note note);
        void UpdateNote(Note note);
        Note GetNoteByID(int? id);

        void InsertBook(Book book);
        void DeleteBook(Book book);
        Book GetBookByID(int? id);

        void InsertResource(Resource resource);
        void DeleteResource(Resource resource);
        void UpdateResource(Resource resource);

        void InsertReview(Review review);

        User GetUserByID(string id);
    }
}
