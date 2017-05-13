using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            var notes = db.Notes.Where(note => note.User.Id == curUser)
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
                    IntervalInDays = model.IntervalInDays,
                    DueDate = model.FirstStudiedDate + TimeSpan.FromDays(1),
                    FirstStudiedDate = model.FirstStudiedDate,
                    TimeEstimate = model.TimeEstimate
                };
                foreach (var resource in model.Resources)
                {
                    if (string.IsNullOrWhiteSpace(resource.Title)) continue;
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
            return View(note);
        }

        // POST: Note/Edit/5
        [HttpPost]
        public ActionResult Edit(Note note, List<Resource> resourcesList)
        {
            if (ModelState.IsValid)
            {
                UpdateNoteResources(note, resourcesList);
                db.UpdateNote(note);
                db.SaveChanges();
                TempData["successMsg"] = $"Successfully updated the note \"{note.Title}\".";
                return RedirectToAction("Index");
            }
            TempData["errorMsg"] = "Failed to update note. Invalid data.";
            return View(note);
        }

        private void UpdateNoteResources(Note note, List<Resource> resources)
        {
            // Reverse so items are inserted in correct order
            resources.Reverse();
            foreach (var resource in resources)
            {
                if (resource.Id != 0)
                {
                    if (String.IsNullOrEmpty(resource.Title))
                        db.DeleteResource(resource);
                    else
                    {
                        db.UpdateResource(resource);
                    }
                }
                else if (!String.IsNullOrEmpty(resource.Title))
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
    }
}
