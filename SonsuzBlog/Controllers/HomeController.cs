using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using SonsuzBlog.Models;
using SonsuzBlog.ViewModel;

namespace SonsuzBlog.Controllers
{
    public class HomeController : Controller
    {
        Model1 db = new Model1();
        // GET: Home
        [Route("")]
        [Route("Anasehife")]
        public ActionResult Index()
        {
            PostVm VM = new PostVm();
            return View(VM);
        }
        public ActionResult AllPostList()
        {
            var postlar = db.tbl_post.OrderByDescending(x => x.PostId).Where(x => x.QebulEdildi == true).ToList();
            return View("PostList", postlar);
        }

        public ActionResult Navbar()
        {
            PostVm VM = new PostVm();

            VM.categories = db.tbl_category.ToList();
            VM.users = db.tbl_users.Where(x => x.Yazar == true && x.QebulEdildi == true).ToList();
            return PartialView(VM);
        }

        [Route("Elaqe")]
        public ActionResult Elaqe()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Elaqe(string ad = null, string email = null, string baslig = null, string metn = null)
        {
            if (ad != null && email != null && metn != null)
            {
                WebMail.SmtpServer = "smtp.gmail.com";
                WebMail.EnableSsl = true;
                WebMail.UserName = "sonsuzbilgiaz@gmail.com";
                WebMail.Password = "dilman99";
                WebMail.SmtpPort = 587;
                WebMail.Send("sonsuzbilgiaz@gmail.com", baslig, email + "-" + metn);
                TempData["elaqe"] = "Mesajiniz ugurla gonderildi";
            }
            else
            {
                TempData["elaqe"] = "Xəta baş verdi ... Təkrar yoxlayın";

            }
            return View();
        }

        public ActionResult PostAxtar(string txtAxtar)
        {
            if (txtAxtar != null)
            {
                var data = db.tbl_post.Where(a => a.Baslig.Contains(txtAxtar) || a.Ozet.Contains(txtAxtar))
                .OrderByDescending(v => v.Tarixi).Where(x=>x.QebulEdildi==true).ToList();
                return View(data);
            }
            return View();
            
        }

        public ActionResult PostAxtarPartial()
        {
            return View();
        }


        [Route("Qaydalar")]
        public ActionResult Qaydalar()
        {
            return View();
        }
        [Route("Haqqimizda")]
        public ActionResult Haqqimizda()
        {
            return View();
        }

        public ActionResult AllWidgets()
        {
            return View();
        }

    }
}