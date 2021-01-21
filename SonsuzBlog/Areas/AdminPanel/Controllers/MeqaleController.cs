using SonsuzBlog.App_Classes;
using SonsuzBlog.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Data.Entity;
using System.Web.Security;

namespace SonsuzBlog.Areas.AdminPanel.Controllers
{
    [Authorize]
    public class MeqaleController : Controller
    {
        Model1 db = new Model1();
        // GET: AdminPanel/Meqale
        public ActionResult Index()
        {
            var tbl_post = db.tbl_post.Where(q => q.QebulEdildi == true).OrderByDescending(x => x.PostId)
                .Include(t => t.tbl_category).Include(t => t.tbl_sekil).Include(t => t.tbl_users);
            return View(tbl_post.ToList());
        }
        [Authorize(Roles = "Admin,Yazar")]
        [HttpGet]
        public ActionResult MeqaleYaz()
        {
            ViewBag.CategoryId = new SelectList(db.tbl_category, "CategoryId", "Ad");
            return View();
        }

        [Authorize(Roles = "Admin,Yazar")]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult MeqaleYaz([Bind(Include = "PostId,MuellifId,Baslig,Context,PhotoId,CategoryId,Tarixi,Baxis,Beyenme,Keyword")] tbl_post post, HttpPostedFileBase sekil, string etiket,string Ozet)
        {
            tbl_etiket etkt = new tbl_etiket();
            ViewBag.CategoryId = new SelectList(db.tbl_category, "CategoryId", "Ad");
            string[] etikets = etiket.Split(',');
            foreach (var e in etikets)
            {
                tbl_etiket etk = db.tbl_etiket.FirstOrDefault(x => x.Ad.ToLower() == e.ToLower().Trim());
                if (etk == null)
                {
                    etk = new tbl_etiket();
                    etk.Ad = e;
                    db.tbl_etiket.Add(etk);
                    db.SaveChanges();
                }
                post.tbl_etiket.Add(etk);
                db.SaveChanges();
            }
            var picture = new tbl_sekil();
            if (sekil != null)
            {
                string sekilaadi = Guid.NewGuid().ToString().Replace("-", "");
                string sekilsonu = Path.GetExtension(Request.Files[0].FileName);
                string tamyol = "/Upload/Sekiller/boyuk/" + sekilaadi + sekilsonu;
                Request.Files[0].SaveAs(Server.MapPath(tamyol));
                picture.Boyuk = tamyol;
                db.SaveChanges();
            }
            var saxtasekil = db.tbl_sekil.Add(picture);
            post.PhotoId = saxtasekil.PhotoId;
            post.QebulEdildi = false;
            post.Tarixi = DateTime.Now;
            post.Baxis = 0;
            post.Ozet = Ozet;
            post.Beyenme = 0;
            int yzrId = db.tbl_users.FirstOrDefault(x => x.Login == User.Identity.Name).UserId;
            post.MuellifId = yzrId;
            db.tbl_post.Add(post);
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            ViewBag.gelenetiket = db.tbl_etiket.Where(x => x.EtiketId == id).ToList();
            if (id == null)
            {
                return HttpNotFound();
            }

            var p = db.tbl_post.Where(x => x.PostId == id).SingleOrDefault();
            if (p == null)
            {
                return HttpNotFound();
            }
            ViewBag.EtiketId = new SelectList(db.tbl_etiket, "EtiketId", "Ad", p.tbl_etiket.Where(x => x.EtiketId == id));
            ViewBag.CategoryId = new SelectList(db.tbl_category, "CategoryId", "Ad", p.CategoryId);
            return View(p);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, tbl_post post, HttpPostedFileBase Sekil)
        {
            if (ModelState.IsValid)
            {
                var p = db.tbl_post.Where(x => x.PostId == id).SingleOrDefault();
                if (Sekil != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(p.tbl_sekil.Boyuk)))
                    {
                        System.IO.File.Delete(Server.MapPath(p.tbl_sekil.Boyuk));
                    }

                    WebImage img = new WebImage(Sekil.InputStream);
                    FileInfo fileinfo = new FileInfo(Sekil.FileName);
                    string imagename = Guid.NewGuid().ToString() + fileinfo.Extension;
                    img.Save("~/Upload/Sekiller/boyuk/" + imagename);
                    p.tbl_sekil.Boyuk = "/Upload/Sekiller/boyuk/" + imagename;
                }
                p.Baslig = post.Baslig;
                p.CategoryId = post.CategoryId;
                p.Context = post.Context;
                p.Ozet = post.Ozet;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var meqale = db.tbl_post.Where(i => i.PostId == id).SingleOrDefault();
            foreach (var i in meqale.tbl_etiket.Where(x => x.EtiketId == id).ToList())
            {
                db.tbl_etiket.Remove(i);
            }

            if (System.IO.File.Exists(Server.MapPath(meqale.tbl_sekil.Boyuk)))
            {
                System.IO.File.Delete(Server.MapPath(meqale.tbl_sekil.Boyuk));
            }
            db.tbl_post.Remove(meqale);
            db.SaveChanges();
            TempData["Xeber"] = "Məqalə uğurla silindi";
            return RedirectToAction("Index", "Meqale");
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