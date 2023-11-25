using IsTakip.Filters;
using IsTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakip.Controllers
{
    public class PersonelController : Controller
    {
        // GET: Personel
        IsTakipDBEntities entity = new IsTakipDBEntities();

        [AuthFilter(1011)]
        public ActionResult Index()
        {

            var personeller = (from p in entity.Personeller where
                               p.personelYetkiTurId!=1011 && p.aktiflik==true select p).ToList();


            return View(personeller);
        }
        [AuthFilter(1011)]
        public ActionResult Olustur()
        {
            BirimYetkiTurler by = birimYetkiTurlerDoldur();
            ViewBag.mesaj = null;

            return View(by);

        }
        [HttpPost,ActFilter("Yeni Personel Eklendi")]
        public ActionResult Olustur(FormCollection fc)
        {
            string personelKullaniciAd = fc["kullaniciAd"];
            var personel = (from p in entity.Personeller
                            where
                            p.personelKullaniciAd == personelKullaniciAd
                            select p).FirstOrDefault();


            int birimId = Convert.ToInt32(fc["birim"]);
            if (Convert.ToInt32(fc["yetkiTur"]) == 1)
            {
                var birimYoneticiKontrol = (from p in entity.Personeller where
                             p.personelBirimId == birimId &&  p.personelYetkiTurId==1
                                            select p).FirstOrDefault();

                if (birimYoneticiKontrol != null)
                {
                    BirimYetkiTurler by = birimYetkiTurlerDoldur();

                    ViewBag.mesaj = "Bu birimin sadece bir yöneticisi olabilir.";
                    TempData["bilgi"] = null;

                    return View(by);
                }

            }


            if (personel == null)
            {
                Personeller yeniPersonel = new Personeller();
                yeniPersonel.personelAdSoyad = fc["adSoyad"];
                yeniPersonel.personelKullaniciAd = personelKullaniciAd;
                yeniPersonel.personelParola = fc["parola"];
                yeniPersonel.personelBirimId =Convert.ToInt32(fc["birim"]);
                yeniPersonel.personelYetkiTurId =Convert.ToInt32(fc["yetkiTur"]);
                yeniPersonel.yeniPersonel = true;
                yeniPersonel.aktiflik = true;

                entity.Personeller.Add(yeniPersonel);

                entity.SaveChanges();

                TempData["bilgi"] = yeniPersonel.personelKullaniciAd;
                return RedirectToAction("Index");
            }
            else
            {
                BirimYetkiTurler by = birimYetkiTurlerDoldur();

                ViewBag.mesaj = "Kullanıcı adı bulunmaktadır.";
                TempData["bilgi"] = null;

                return View(by);

            }
        }

        [AuthFilter(1011)]
        public ActionResult Guncelle(int id)
        {
            var personel = (from p in entity.Personeller where
                          p.personelId==id select p).FirstOrDefault();

            return View(personel);
        }

        [HttpPost,ActFilter("Personel Güncellendi")]
        public ActionResult Guncelle(int id,string adSoyad)
        {
            Personeller personel = (from p in entity.Personeller
                                    where p.personelId == id
                                    select p).FirstOrDefault();

            personel.personelAdSoyad = adSoyad;

            entity.SaveChanges();

            TempData["bilgi"] = personel.personelKullaniciAd;

            return RedirectToAction("Index");
        }


        [AuthFilter(1011)]
        public ActionResult Sil(int id)
        {
            Personeller personel = (from p in entity.Personeller
                                    where
                                   p.personelId == id
                                    select p).FirstOrDefault();
            return View(personel);

        }

        [HttpPost,ActFilter("Personel Silindi.")]

        public ActionResult Sil(FormCollection fc)
        {
            int personelId = Convert.ToInt32(fc["personelId"]);
            var personel = (from p in entity.Personeller where
                          p.personelId == personelId select p).FirstOrDefault();

            personel.aktiflik = false;

            entity.SaveChanges();

            TempData["bilgi"] = personel.personelKullaniciAd;

            return RedirectToAction("Index");
        }

        public BirimYetkiTurler birimYetkiTurlerDoldur()
        {
            BirimYetkiTurler by = new BirimYetkiTurler();
            by.birimlerList = (from b in entity.Birimler
                               where b.aktiflik == true
                               select b).ToList();
            by.yetkiTurlerList = (from y in entity.YetkiTurler
                                  where y.yetkiTurId != 1011
                                  select y).ToList();

            return by;

        }
    }
}