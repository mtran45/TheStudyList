using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheStudyList.Controllers;
using TheStudyList.Domain.Entities;

namespace TheStudyList.Tests
{
    [TestClass]
    public class NoteTests
    {
        private User user;
        private User user2;

        [TestInitialize()]
        public void Initialize()
        {
            user = new User()
            {
                Id = "1",
                UserName = "user"
            };
            user2 = new User()
            {
                Id = "2",
                UserName = "user2"
            };
        }

        [TestMethod]
        public void Index_Returns_Users_Notes_Sorted_By_Date()
        {
            // Arrange - create the mock repo and its data
            var context = new TestContext();

            context.Notes.Add(new Note() {Id = 1, Title = "Note 1", DueDate = DateTime.Parse("02-01-17"), User = user});
            context.Notes.Add(new Note() {Id = 2, Title = "Note 2", DueDate = DateTime.Parse("01-01-17"), User = user});
            context.Notes.Add(new Note() {Id = 3, Title = "Note 3", DueDate = DateTime.Parse("01-01-17"), User = user2});

            // Arrange - create the controller
            NoteController target = new NoteController(context)
            {
                GetUserId = () => user.Id
            };

            // Act
            List<Note> notes = (((ViewResult) target.Index()).ViewData.Model as IEnumerable<Note>).ToList();

            // Assert
            Assert.AreEqual(2, notes.Count);
            Assert.AreEqual(2, notes[2].Id);
            Assert.AreEqual(1, notes[1].Id);
        }
    }
}
