using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using SonsuzBlog.App_Classes;
using SonsuzBlog.Models;

namespace SonsuzBlog.Areas.AdminPanel.Controllers
{

    [Authorize]
    public class KateqoriyaController : Controller
    {
        private Model1 db = new Model1();

        // GET: AdminPanel/Kateqoriya
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var tbl_category = db.tbl_category.Include(t => t.tbl_sekil);
            return View(tbl_category.ToList());
        }

        // GET: AdminPanel/Kateqoriya/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_category tbl_category = db.tbl_category.Find(id);
            if (tbl_category == null)
            {
                return HttpNotFound();
            }
            return View(tbl_category);
        }

        // GET: AdminPanel/Kateqoriya/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.PhotoId = new SelectList(db.tbl_sekil, "PhotoId", "Kicik");
            return View();
        }

  
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryId,Ad,PhotoId,Aciqlama,Keyword")] tbl_category tbl_category,HttpPostedFileBase Sekil)
        {
            if (ModelState.IsValid)
            {

                if (Sekil != null)
                {
                    var picture = new tbl_sekil();
                    if (Sekil.ContentLength > 0)
                    {
                        
                        string sekilaadi = Guid.NewGuid().ToString().Replace("-", "");
                        string sekilsonu = System.IO.Path.GetExtension(Request.Files[0].FileName);
                        string tamyol = "/Upload/Sekiller/boyuk/" + sekilaadi + sekilsonu;
                        Request.Files[0].SaveAs(Server.MapPath(tamyol));
                        picture.Boyuk = tamyol;
                        db.SaveChanges();

                        db.tbl_category.Add(tbl_category);
                        db.SaveChanges();
                    }
                    var saxtasekil= db.tbl_sekil.Add(picture);
                    tbl_category.PhotoId = saxtasekil.PhotoId;
                    db.tbl_category.Add(tbl_category);
                    db.SaveChanges();
                }

                
                return RedirectToAction("Index");
            }

            //ViewBag.PhotoId = new SelectList(db.tbl_sekil, "PhotoId", "Kicik", tbl_category.PhotoId);
            return View(tbl_category);
        }

        [Authorize(Roles = "Admin")]
        // GET: AdminPanel/Kateqoriya/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_category tbl_category = db.tbl_category.Find(id);
            if (tbl_category == null)
            {
                return HttpNotFound();
            }
            ViewBag.PhotoId = new SelectList(db.tbl_sekil, "PhotoId", "Kicik", tbl_category.PhotoId);
            return View(tbl_category);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryId,Ad,PhotoId,Aciqlama,Keyword")] tbl_category tbl_category, HttpPostedFileBase Sekil,int id)
        {
            if (ModelState.IsValid)
            {
                var p = db.tbl_category.Where(x => x.CategoryId == id).SingleOrDefault();
                if (Sekil != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(p.tbl_sekil.Boyuk)))
                    {
                        System.IO.File.Delete(Server.MapPath(p.tbl_sekil.Boyuk));
                    }

                    WebImage img = new WebImage(Sekil.InputStream);
                    FileInfo fileinfo = new FileInfo(Sekil.FileName);

                    string imagename = Guid.NewGuid().ToString() + fileinfo.Extension;
                    img.Save("~/Upload/Sekiller/boyuk" + imagename);
                    p.tbl_sekil.Boyuk = "/Upload/Sekiller/boyuk" + imagename;
                }
                p.Ad = tbl_category.Ad;
                db.SaveChanges();
                
                return RedirectToAction("Index");
            }
            ViewBag.PhotoId = new SelectList(db.tbl_sekil, "PhotoId", "Kicik", tbl_category.PhotoId);
            return View(tbl_category);
        }

        // GET: AdminPanel/Kateqoriya/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_category tbl_category = db.tbl_category.Find(id);
            if (tbl_category == null)
            {
                return HttpNotFound();
            }
            return View(tbl_category);
        }

        // POST: AdminPanel/Kateqoriya/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_category tbl_category = db.tbl_category.Find(id);
            db.tbl_category.Remove(tbl_category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
