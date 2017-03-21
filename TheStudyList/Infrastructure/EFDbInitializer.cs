using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;
using TheStudyList.Domain.Concrete;
using TheStudyList.Domain.Entities;

namespace TheStudyList.Infrastructure
{
    public class EFDbInitializer : System.Data.Entity.DropCreateDatabaseAlways<EFDbContext>
    {
        protected override void Seed(EFDbContext context)
        {
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
        }
    }
}