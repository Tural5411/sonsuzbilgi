using SonsuzBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SonsuzBlog.Controllers
{
    public class MuellifController : Controller
    {
        Model1 db = new Model1();
        // GET: Muellif
        
        [Route("Muellif/{Ad}-{id=int}")]
        public ActionResult Index(int id)
        {
            ViewBag.muellifImg = db.tbl_users.FirstOrDefault(x => x.UserId == id).Haqqinda;
            ViewBag.muellifAd = db.tbl_users.FirstOrDefault(x => x.UserId == id).Ad;
            ViewBag.muellifSoyad = db.tbl_users.FirstOrDefault(x => x.UserId == id).Soyad;
            return View(id);
        }
        public ActionResult PostList(int id)
        {
            var data = db.tbl_post.OrderByDescending(x=>x.PostId).Where(x => x.MuellifId == id && x.QebulEdildi==true);
            return View("PostList", data);
        }
    }
}