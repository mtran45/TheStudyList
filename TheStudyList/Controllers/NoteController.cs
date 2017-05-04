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

        // GET: Note/Details/5
        public ActionResult Details(int id)
        {
            return View();
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
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Note/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Note/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Note/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Note/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Study(int? id)
        {
            Note note = db.GetNoteByID(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [HttpPost]
        public ActionResult Study(int id, int? ivl)
        {
            Note note = db.GetNoteByID(id);
            if (ivl == null || ivl <= 0)
            {
                TempData["message"] = "Invalid interval";
                return View(note);
            }
            note.UpdateInterval((int)ivl);
            note.User = db.GetUserByID(GetUserId());
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
