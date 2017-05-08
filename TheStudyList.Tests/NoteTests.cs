using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheStudyList.Controllers;
using TheStudyList.Domain.Entities;
using TheStudyList.Models;

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
        public void Index_Returns_Users_Notes_Sorted_By_DueDate()
        {
            // Arrange - create the mock repo and its data
            var context = new TestContext();

            context.Notes.Add(new Note() {Id = 1, Title = "Note 1", DueDate = DateTime.Parse("02-01-17"), User = user});
            context.Notes.Add(new Note() {Id = 2, Title = "Note 2", DueDate = DateTime.Parse("01-01-17"), User = user});
            context.Notes.Add(new Note() {Id = 3, Title = "Note 3", DueDate = DateTime.Parse("03-01-17"), User = user2});

            // Arrange - create the controller
            NoteController target = new NoteController(context)
            {
                GetUserId = () => user.Id
            };

            // Act
            List<Note> notes = (((ViewResult) target.Index()).ViewData.Model as IEnumerable<Note>).ToList();

            // Assert
            Assert.AreEqual(2, notes.Count);
            Assert.AreEqual(2, notes[0].Id);
            Assert.AreEqual(1, notes[1].Id);
        }

        [TestMethod]
        public void Can_Create_Note()
        {
            // Arrange
            var note = new Note()
            {
                Title = "Note 1",
                IntervalInDays = 1,
                FirstStudiedDate = DateTime.Parse("01-01-17"),
                Resources = new List<Resource>()
            };
            var context = new TestContext();
            context.Users.Add(user);

            // Arrange - create the controller
            NoteController target = new NoteController(context)
            {
                GetUserId = () => user.Id
            };

            // Act
            target.Create(new CreateNoteViewModel()
            {
                Title = note.Title,
                FirstStudiedDate = note.FirstStudiedDate,
                IntervalInDays = note.IntervalInDays,
                Resources = (List<Resource>)note.Resources
            });

            // Assert - ensure note and its corresponding review is saved
            var savedNote = context.Notes.SingleOrDefault();
            var savedReview = context.Reviews.SingleOrDefault();
            Assert.IsNotNull(savedNote);
            Assert.AreEqual(note.Title, savedNote.Title);
            Assert.AreEqual(user, savedNote.User);
            Assert.IsNotNull(savedReview);
            Assert.AreEqual(savedNote, savedReview.Note);
        }

        [TestMethod]
        public void Study_Updates_Interval_And_DueDate()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                Title = "Note 1",
                IntervalInDays = 1,
                FirstStudiedDate = DateTime.Parse("01-01-17"),
                Resources = new List<Resource>()
            };
            var context = new TestContext();
            context.Users.Add(user);
            context.Notes.Add(note);

            // Arrange - create the controller
            NoteController target = new NoteController(context)
            {
                GetUserId = () => user.Id
            };

            // Act
            var newInterval = 3;
            target.Study(note.Id, newInterval);

            // Assert - ensure interval and DueDate updated, and review created
            var savedNote = context.Notes.Single();
            var savedReview = context.Reviews.SingleOrDefault();
            Assert.AreEqual(newInterval, savedNote.IntervalInDays);
            Assert.AreEqual(DateTime.Today.AddDays(3), savedNote.DueDate.Date);
            Assert.IsNotNull(savedReview);
            Assert.AreEqual(savedNote, savedReview.Note);
            Assert.AreEqual(DateTime.Today, savedReview.Date.Date);
        }

        [TestMethod]
        public void Can_Edit_Note()
        {
            // Arrange
            var context = new TestContext();

            context.Notes.Add(new Note() { Id = 1, Title = "Note 1", DueDate = DateTime.Parse("02-01-17"), User = user });

            // Arrange - create the controller
            NoteController target = new NoteController(context)
            {
                GetUserId = () => user.Id
            };

            // Act
            Note note = ((ViewResult) target.Edit(1)).ViewData.Model as Note;

            // Assert - ensure correct note is passed to View
            Assert.AreEqual("Note 1", note.Title);
        }
    }
}
