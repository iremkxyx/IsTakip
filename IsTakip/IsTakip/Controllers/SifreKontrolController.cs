using IsTakip.Filters;
using IsTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakip.Controllers
{
    public class SifreKontrolController : Controller
    {
        // GET: SifreKontrol

        IsTakipDBEntities entity = new IsTakipDBEntities();


        public ActionResult Index()
        {

            int personelId = Convert.ToInt32(Session["PersonelId"]);

            if (personelId == 0) return RedirectToAction("Index", "Login");

            var personel = (from p in entity.Personeller
                            where
                            p.personelId == personelId
                            select p).FirstOrDefault();
            //bir tane veri geleceği için firstordefault dedik cok olsaydı tolist derdik



            ViewBag.mesaj = null;

            ViewBag.yetkiTurId = null;

            ViewBag.sitil = null;



            return View(personel);
        }

        [HttpPost,ActFilter("Parola değiştirildi.")]
        public ActionResult Index(int personelId, string eskiParola,
            string yeniParola, string yeniParolaKontrol)
        {
            var personel= (from p in entity.Personeller where
                           p.personelId==personelId select p).FirstOrDefault();

            if (eskiParola != personel.personelParola)
            {
                ViewBag.mesaj = "Eski parolanızı yanlış girdiniz ";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }

            if(yeniParola!= yeniParolaKontrol)
            {
                ViewBag.mesaj = "Yeni parola ve tekrarı aynı değil. ";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }

            if(yeniParola.Length <6 || yeniParola.Length>15)
            {
                ViewBag.mesaj = "Yeni parola en az 6 en çok 15 karakter olmalıdır.";
                ViewBag.sitil = "alert alert-danger";

                return View(personel);
            }



            personel.personelParola = yeniParola;
            personel.yeniPersonel = false;
            entity.SaveChanges();

            TempData["bilgi"] = personel.personelKullaniciAd;

            ViewBag.mesaj = "Parolanız başarıyla değiştirildi. Anasayfaya yönlendiriliyorsunuz.";

            ViewBag.sitil = "alert alert-success";

            ViewBag.yetkiTurId = personel.personelYetkiTurId;

            return View(personel);

        }
    }
}