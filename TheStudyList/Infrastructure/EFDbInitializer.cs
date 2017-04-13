using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;
using TheStudyList.Domain.Concrete;
using TheStudyList.Domain.Entities;

namespace TheStudyList.Infrastructure
{
    public class EFDbInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<EFDbContext>
    {
        protected override void Seed(EFDbContext context)
        {
            // Seed users
            var hasher = new PasswordHasher();
            var user = new User
            {
                UserName = "user",
                Email = "user@email.com",
                PasswordHash = hasher.HashPassword("password"),
                SecurityStamp = Guid.NewGuid().ToString(),
            };
            context.Users.AddOrUpdate(u => u.UserName, user);

            var users = new List<User>
            {
                user,
                new User {UserName = "user2"},
                new User {UserName = "user3"},
            };
            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();

            // Seed notes
            var note1 = new Note
            {
                Title = "Normalisation of DB",
                Topic = "Databases",
                Notebook = "Computer Science",
                User = user,
                FirstStudiedDate = DateTime.Parse("9/3/17"),
                DueDate = DateTime.Parse("10/4/17"),
                IntervalInDays = 14
            };

            var note2 = new Note
            {
                Title = "Alg 1-1: Union-Find",
                Topic = "Algorithms",
                Notebook = "Computer Science",
                User = user,
                FirstStudiedDate = DateTime.Parse("9/3/17"),
                DueDate = DateTime.Parse("31/3/17"),
                IntervalInDays = 14
            };

            var notes = new List<Note>
            {
                note1,
                note2
            };
            notes.ForEach(n => context.Notes.Add(n));
            context.SaveChanges();

            // Seed links
            var resources = new List<Resource>
            {
                new Resource {Note = note1, Title = "Manga guide to databases"},
                new Resource {Note = note1, Title = "Evernote", Url = "evernote:///view/1214804/s10/1f252535-71ea-42ed-83b7-9ff68245c8d1/1f252535-71ea-42ed-83b7-9ff68245c8d1/"},
                new Resource {Note = note1, Title = "Evernote (summary)", Url = "evernote:///view/1214804/s10/dd5bbb8d-7d12-4d95-827b-9fd6ae5637c1/dd5bbb8d-7d12-4d95-827b-9fd6ae5637c1/"},
                new Resource {Note = note2, Title = "Coursera slides", Url = "https://www.coursera.org/learn/algorithms-part1/supplement/bcelg/lecture-slides"},
                new Resource {Note = note2, Title = "Evernote", Url = "evernote:///view/1214804/s10/1455c1f2-edc7-4277-be25-b6e208218561/1455c1f2-edc7-4277-be25-b6e208218561/"}
            };
            resources.ForEach(l => context.Resources.Add(l));
            context.SaveChanges();
        }
    }
}