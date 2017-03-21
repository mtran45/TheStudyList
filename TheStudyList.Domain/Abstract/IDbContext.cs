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
        IDbSet<User> Users { get; }

        int SaveChanges();

        void SaveNote(Note note);
        void DeleteNote(Note note);
        Note GetNoteByID(int? id);

        void SaveBook(Book book);
        void DeleteBook(Book book);
        Book GetBookByID(int? id);

        User GetUserByID(string id);
    }
}
