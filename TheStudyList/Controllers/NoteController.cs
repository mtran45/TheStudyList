﻿using CsvHelper;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using TheStudyList.Domain.Abstract;
using TheStudyList.Domain.Entities;
using TheStudyList.Models;

namespace TheStudyList.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private IDbContext db;
        public Func<string> GetUserId;

        public NoteController(IDbContext context)
        {
            db = context;
            GetUserId = () => User.Identity.GetUserId();
        }

        // GET: Note
        public ActionResult StudyList(string notebook)
        {
            string curUser = GetUserId();
            var model = new StudyListViewModel
            {
                Notes = db.Notes
                    .Include(note => note.Resources)
                    .Include(note => note.User)
                    .Where(note => note.User.Id == curUser)
                    .Where(note => notebook == null || note.Notebook == notebook)
                    .OrderBy(note => note.DueDate)
                    .ToList(),
                CurrentNotebook = notebook
            }; 
            return View(model);
        }

        public string[] GetNotebooks()
        {
            string[] notebooks = db.Notes
                .Select(note => note.Notebook)
                .Distinct()
                .OrderBy(nb => nb).ToArray();
            return notebooks.Where(nb => !string.IsNullOrWhiteSpace(nb)).ToArray();
        }

        public PartialViewResult NotebookMenu(string notebook = null)
        {
            ViewBag.SelectedNotebook = notebook;
            return PartialView(GetNotebooks());
        }

        // GET: Note/Create
        public ActionResult Create()
        {
            return View(new CreateNoteViewModel
            {
                Notebook = GetNotebooks().Length > 0 ? GetNotebooks()[0] : null,
                Notebooks = GetNotebooks()
            });
        }

        // POST: Note/Create
        [HttpPost]
        public ActionResult Create(CreateNoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.GetUserByID(GetUserId());
                var note = new Note
                {
                    User = user,
                    Title = model.Title,
                    Notebook = model.Notebook,
                    IntervalInDays = model.IntervalInDays,
                    DueDate = model.FirstStudiedDate.FromLocalTime(user) + TimeSpan.FromDays(1),
                    FirstStudiedDate = model.FirstStudiedDate.FromLocalTime(user),
                    TimeEstimate = model.TimeEstimate
                };
                foreach (var resource in model.Resources)
                {
                    if (String.IsNullOrWhiteSpace(resource.Title) 
                        && String.IsNullOrWhiteSpace(resource.Url)) continue;
                    resource.Note = note;
                    db.InsertResource(resource);
                }
                var review = new Review
                {
                    Date = note.FirstStudiedDate,
                    Note = note
                };
                db.InsertReview(review);
                db.InsertNote(note);
                db.SaveChanges();
                TempData["successMsg"] = $"Successfully created the note \"{note.Title}\".";
                return RedirectToAction("StudyList");
            }
            TempData["errorMsg"] = "Failed. Please check the fields and try again.";
            return View(model);
        }

        // GET: Note/Edit/5
        public ActionResult Edit(int? id, string returnUrl)
        {
            var user = db.GetUserByID(GetUserId());
            Note note = db.GetNoteByID(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            var model = new EditNoteViewModel
            {
                Id = note.Id,
                Title = note.Title,
                IntervalInDays = note.IntervalInDays,
                DueDate = note.DueDate.ToLocalTime(user),
                FirstStudiedDate = note.FirstStudiedDate.ToLocalTime(user),
                Notebook = note.Notebook,
                TimeEstimate = note.TimeEstimate,
                Resources = note.Resources?.ToArray(),
                ReturnUrl = returnUrl,
                Notebooks = GetNotebooks()
            };
            return View(model);
        }

        // POST: Note/Edit/5
        [HttpPost]
        public ActionResult Edit(EditNoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                Note note = db.GetNoteByID(model.Id);
                UpdateNote(note, model);
                db.UpdateNote(note);
                db.SaveChanges();
                TempData["successMsg"] = $"Successfully updated the note \"{note.Title}\".";
                return Redirect(model.ReturnUrl);
            }
            TempData["errorMsg"] = "Failed to update note. Invalid data.";
            return View(model);
        }

        private void UpdateNote(Note note, EditNoteViewModel model)
        {
            var user = db.GetUserByID(GetUserId());
            note.Title = model.Title;
            note.Notebook = model.Notebook;
            note.IntervalInDays = model.IntervalInDays;
            note.DueDate = model.DueDate.FromLocalTime(user);
            note.FirstStudiedDate = model.FirstStudiedDate.FromLocalTime(user);
            note.TimeEstimate = model.TimeEstimate;
            UpdateNoteResources(note, model);
        }

        private void UpdateNoteResources(Note note, EditNoteViewModel model)
        {
            foreach (var resource in model.Resources)
            {
                // Update/delete existing resources
                if (resource.Id != 0)
                {
                    if (String.IsNullOrWhiteSpace(resource.Title) && String.IsNullOrWhiteSpace(resource.Url))
                        db.DeleteResource(resource);
                    else
                    {
                        db.UpdateResource(resource);
                    }
                }
                // Add new resources to db
                else if (!String.IsNullOrWhiteSpace(resource.Title))
                {
                    resource.Note = note;
                    db.InsertResource(resource);
                }
            }
        }

        // GET: Note/Delete/5
        public ActionResult Delete(int? id)
        {
            Note note = db.GetNoteByID(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Note/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Note note = db.GetNoteByID(id);
            db.DeleteNote(note);
            db.SaveChanges();
            TempData["successMsg"] = "Successfully deleted note.";
            return RedirectToAction("StudyList");
        }

        // GET: Note/Study/5
        public ActionResult Study(int? id, string returnUrl)
        {
            var user = db.GetUserByID(GetUserId());
            Note note = db.GetNoteByID(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            var model = new StudyViewModel
            {
                Id = note.Id,
                Title = note.Title,
                IntervalInDays = note.IntervalInDays,
                DueDate = note.DueDate.ToLocalTime(user),
                FirstStudiedDate = note.FirstStudiedDate.ToLocalTime(user),
                Notebook = note.Notebook,
                TimeEstimate = note.TimeEstimate,
                Reviews = note.Reviews.ToList(),
                IntervalHard = note.IntervalHard(),
                IntervalGood = note.IntervalGood(),
                IntervalEasy = note.IntervalEasy(),
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        // POST: Note/Study/5
        [HttpPost]
        public ActionResult Study(StudyViewModel model)
        {
            Note note = db.GetNoteByID(model.Id);
            if (model.Interval == null || model.Interval <= 0)
            {
                TempData["errorMsg"] = "Invalid interval.";
                return View(note);
            }
            note.UpdateInterval((int)model.Interval);
            var review = new Review
            {
                Date = DateTime.UtcNow,
                Note = note
            };
            db.InsertReview(review);
            db.SaveChanges();
            if (model.ReturnUrl == null)
                return RedirectToAction("StudyList");
            return Redirect(model.ReturnUrl);
        }

        // GET: Note/Import
        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(string notebook, string tsvString)
        {
            var user = db.GetUserByID(GetUserId());
            int entries = 0;
            using (TextReader tr = new StringReader(tsvString))
            {
                var tsv = new CsvReader(tr);
                tsv.Configuration.Delimiter = "\t";
                while (tsv.Read())
                {
                    var note = new Note
                    {
                        User = user,
                        Title = tsv.GetField<string>(0),
                        TimeEstimate = tsv.GetField<Duration>(1),
                        DueDate = tsv.GetField<DateTime>(2).FromLocalTime(user),
                        IntervalInDays = ParseInterval(tsv.GetField<string>(3)),
                        Notebook = notebook
                    };
                    db.InsertNote(note);
                    entries++;
                }
                db.SaveChanges();
            }
            TempData["successMsg"] = $"Successfully imported {entries} notes.";
            return RedirectToAction("StudyList");
        }

        private int ParseInterval(string str)
        {
            // Parses interval string of form "14d" or "3w" and returns num of days
            var match = Regex.Match(str, @"(\d+)([dw])");
            // number of days or weeks
            int count = int.Parse(match.Groups[1].Value);
            // day or week
            string period = match.Groups[2].Value;
            if (period == "d")
                return count;
            else if (period == "w")
                return count * 7;
            else return 1; // fallback to 1
        }

        // GET: Note/Stats
        public ActionResult Stats(string notebook)
        {
            var user = db.GetUserByID(GetUserId());
            string curUser = GetUserId();
            var model = new StatsViewModel
            {
                Notes = db.Notes
                    .Include(note => note.Reviews)
                    .Where(note => note.User.Id == curUser)
                    .Where(note => notebook == null || note.Notebook == notebook)
                    .ToList(),
                Reviews = db.Reviews
                    .Where(r => r.Note.User.Id == curUser)
                    .Where(r => notebook == null || r.Note.Notebook == notebook)
                    .ToList(),
                TodayLocal = DateTime.UtcNow.ToLocalTime(user).Date,
                Notebook = notebook
            };
            return View(model);
        }
    }
}
