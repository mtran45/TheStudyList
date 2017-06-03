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
        public void Get_Notebook_Initials()
        {
            var note1 = new Note { Notebook = "tesT" };
            var note2 = new Note { Notebook = "Test String" };
            var note3 = new Note { Notebook = "a test string" };
            var note4 = new Note { Notebook = "a 'test" };
            var note5 = new Note();

            Assert.AreEqual("T", note1.NotebookInitials);
            Assert.AreEqual("TS", note2.NotebookInitials);
            Assert.AreEqual("AT", note3.NotebookInitials);
            Assert.AreEqual("A'", note4.NotebookInitials);
            Assert.AreEqual(null, note5.NotebookInitials);
        }

        [TestMethod]
        public void Can_Filter_By_Notebook()
        {
            // Arrange - create the repo
            var context = new TestContext();

            context.Notes.Add(new Note { Id = 1, Title = "Note 1", Notebook = "NB1", User = user });
            context.Notes.Add(new Note { Id = 2, Title = "Note 2", Notebook = "NB2", User = user });
            context.Notes.Add(new Note { Id = 3, Title = "Note 3", Notebook = "NB1", User = user });
            context.Notes.Add(new Note { Id = 4, Title = "Note 4", Notebook = "NB2", User = user });
            context.Notes.Add(new Note { Id = 5, Title = "Note 5", Notebook = "NB3", User = user });

            // Arrange - create the controller
            NoteController target = new NoteController(context)
            {
                GetUserId = () => user.Id
            };

            // Act
            ViewResult vr = (ViewResult)target.StudyList("NB2");
            StudyListViewModel model = (StudyListViewModel)vr.Model;
            Note[] notes = model.Notes.ToArray();

            // Assert
            Assert.AreEqual(2, notes.Length);
            Assert.IsTrue(notes[0].Title == "Note 2" && notes[0].Notebook == "NB2");
            Assert.IsTrue(notes[1].Title == "Note 4" && notes[0].Notebook == "NB2");

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
            ViewResult vr = (ViewResult)target.StudyList(null);
            StudyListViewModel model = (StudyListViewModel)vr.Model;
            Note[] notes = model.Notes.ToArray();

            // Assert
            Assert.AreEqual(2, notes.Length);
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
            var model = new StudyViewModel
            {
                Interval = 3,
                Id = note.Id,
                Title = note.Title,
                IntervalInDays = note.IntervalInDays,
                FirstStudiedDate = note.FirstStudiedDate,
            };
            target.Study(model);

            // Assert - ensure interval and DueDate updated, and review created
            var savedNote = context.Notes.Single();
            var savedReview = context.Reviews.SingleOrDefault();
            Assert.AreEqual(3, savedNote.IntervalInDays);
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
            context.Users.Add(user);

            context.Notes.Add(new Note() { Id = 1, Title = "Note 1", DueDate = DateTime.Parse("02-01-17"), User = user });

            // Arrange - create the controller
            NoteController target = new NoteController(context)
            {
                GetUserId = () => user.Id
            };

            // Act
            EditNoteViewModel model = ((ViewResult) target.Edit(1, null)).ViewData.Model as EditNoteViewModel;

            // Assert - ensure correct note is passed to View
            Assert.AreEqual("Note 1", model.Title);
        }

        [TestMethod]
        public void Can_Import_Notes_From_TSV()
        {
            // Arrange
            var context = new TestContext();
            context.Users.Add(user);

            // Arrange - create the controller
            NoteController target = new NoteController(context)
            {
                GetUserId = () => user.Id
            };

            // Act
            string notebook = "Computer Science";
            string tsv = @"Topic	T	Due	Ivl
CTCI IV - Before the Interview	1	14 May	21d
OOP Principles	3	14 May	4w
Queue	2	14 May	27d";
            target.Import(notebook, tsv);

            // Assert
            Assert.AreEqual(3, context.Notes.Count());

            var notes = context.Notes.ToList();
            Assert.AreEqual("CTCI IV - Before the Interview", notes[0].Title);
            Assert.AreEqual(Duration.Short, notes[0].TimeEstimate);
            Assert.AreEqual(DateTime.Parse("14/5/17").ToUniversalTime(), notes[0].DueDate);
            Assert.AreEqual(21, notes[0].IntervalInDays);
            Assert.AreEqual(notebook, notes[0].Notebook);
            Assert.AreEqual(user, notes[0].User);

            Assert.AreEqual(Duration.Long, notes[1].TimeEstimate);
            Assert.AreEqual(4*7, notes[1].IntervalInDays);
        }

        [TestMethod]
        public void Can_Generate_Forecast_Count_Array()
        {
            var model1 = new StatsViewModel
            {
                Notes = new List<Note>
                {
                    new Note {DueDate = DateTime.UtcNow.AddDays(-1), User = user},
                    new Note {DueDate = DateTime.UtcNow.AddDays(-2), User = user},
                    new Note {DueDate = DateTime.UtcNow.AddDays(-3), User = user},
                }
            };

            int dayRange = 1;
            var forecast1 = model1.Forecast(dayRange);
            Assert.AreEqual(1, forecast1.Length);
            Assert.AreEqual(3, forecast1[0]);

            var model2 = new StatsViewModel
            {
                Notes = new List<Note>
                {
                    new Note {DueDate = DateTime.UtcNow.AddDays(-1), User = user},
                    new Note {DueDate = DateTime.UtcNow.AddDays(1), User = user},
                    new Note {DueDate = DateTime.UtcNow.AddDays(1).AddHours(-2), User = user},
                }
            };

            int dayRange2 = 3;
            var forecast2 = model2.Forecast(dayRange2);
            Assert.AreEqual(dayRange2, forecast2.Length);
            Assert.AreEqual(1, forecast2[0]);
            Assert.AreEqual(2, forecast2[1]);
            Assert.AreEqual(0, forecast2[2]);
        }

        [TestMethod]
        public void Can_Generate_Review_Count_Array()
        {
            var note1 = new Note
            {
                User = user
            };

            var model1 = new StatsViewModel
            {
                Reviews = new List<Review>
                {
                    new Review
                    {
                        Date = DateTime.UtcNow.AddDays(0),
                        Note = note1
                    }
                }
            };

            int dayRange = 2;
            var reviews1 = model1.ReviewCount(dayRange);
            Assert.AreEqual(2, reviews1.Length);
            Assert.AreEqual(1, reviews1[dayRange - 1]); // day 0
        }
    }
}
