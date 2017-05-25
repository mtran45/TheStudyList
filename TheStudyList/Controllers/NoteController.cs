using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Text;
using CsvHelper;
using Microsoft.AspNet.Identity;
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
        public ActionResult Index()
        {
            string curUser = GetUserId();
            var notes = db.Notes
                .Include(note => note.Resources)
                .Include(note => note.User)
                .Where(note => note.User.Id == curUser)
                .OrderBy(note => note.DueDate);
            return View(notes);
        }

        // GET: Note/Create
        public ActionResult Create()
        {
            return View(new CreateNoteViewModel());
        }

        // POST: Note/Create
        [HttpPost]
        public ActionResult Create(CreateNoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var note = new Note
                {
                    User = db.GetUserByID(GetUserId()),
                    Title = model.Title,
                    Notebook = model.Notebook,
                    IntervalInDays = model.IntervalInDays,
                    DueDate = model.FirstStudiedDate + TimeSpan.FromDays(1),
                    FirstStudiedDate = model.FirstStudiedDate,
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
                return RedirectToAction("Index");
            }
            TempData["errorMsg"] = "Failed. Please check the fields and try again.";
            return View(model);
        }

        // GET: Note/Edit/5
        public ActionResult Edit(int? id)
        {
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
                DueDate = note.DueDate,
                FirstStudiedDate = note.FirstStudiedDate,
                Notebook = note.Notebook,
                TimeEstimate = note.TimeEstimate,
                Resources = note.Resources?.ToArray()
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
                return RedirectToAction("Index");
            }
            TempData["errorMsg"] = "Failed to update note. Invalid data.";
            return View(model);
        }

        private void UpdateNote(Note note, EditNoteViewModel model)
        {
            note.Title = model.Title;
            note.Notebook = model.Notebook;
            note.IntervalInDays = model.IntervalInDays;
            note.DueDate = model.DueDate;
            note.FirstStudiedDate = model.FirstStudiedDate;
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
            return RedirectToAction("Index");
        }

        // GET: Note/Study/5
        public ActionResult Study(int? id)
        {
            Note note = db.GetNoteByID(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Note/Study/5
        [HttpPost]
        public ActionResult Study(int id, int? ivl)
        {
            Note note = db.GetNoteByID(id);
            if (ivl == null || ivl <= 0)
            {
                TempData["errorMsg"] = "Invalid interval.";
                return View(note);
            }
            note.UpdateInterval((int)ivl);
            var review = new Review
            {
                Date = DateTime.UtcNow,
                Note = note
            };
            db.InsertReview(review);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Import()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(string notebook, string tsvString)
        {
            int entries = 0;
            using (TextReader tr = new StringReader(tsvString))
            {
                var tsv = new CsvReader(tr);
                tsv.Configuration.Delimiter = "\t";
                while (tsv.Read())
                {
                    var note = new Note
                    {
                        User = db.GetUserByID(GetUserId()),
                        Title = tsv.GetField<string>(0),
                        TimeEstimate = tsv.GetField<Duration>(1),
                        DueDate = tsv.GetField<DateTime>(2),
                        IntervalInDays = ParseInterval(tsv.GetField<string>(3)),
                        Notebook = notebook
                    };
                    db.InsertNote(note);
                    entries++;
                }
                db.SaveChanges();
            }
            TempData["successMsg"] = $"Successfully imported {entries} notes.";
            return RedirectToAction("Index");
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
    }
}
