using IsTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using IsTakip.Filters;

namespace IsTakip.Controllers
{

    [AuthFilter(1011)] // hepsi için çalışması için buraya yazdım
    public class SistemYoneticisiController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();
        // GET: SistemYoneticisi

        [AuthFilter(1011)]
        public ActionResult Index()
        {


            var birimler = (from b in entity.Birimler
                            where
                             b.aktiflik == true
                            select b).ToList();

            string labelBirim = "[";

            foreach (var birim in birimler)
            {
                labelBirim += "'" + birim.birimAd + "',";
            }

            labelBirim += "]";

            ViewBag.labelBirim = labelBirim;

            List<int> islerToplam = new List<int>();
            
            foreach (var birim in birimler)
            {
                int toplam = 0;
                var personeller = (from p in entity.Personeller
                                   where p.personelBirimId== birim.birimId &&
                                   p.aktiflik==true select p).ToList();

                foreach (var personel in personeller)
                {
                    var isler = (from i in entity.Isler where i.isPersonelId==personel.personelId
                                 select i).ToList();

                    toplam += isler.Count();

                }

                islerToplam.Add(toplam);

            }


            string dataIs = "[";

            foreach (var i in islerToplam)
            {
                dataIs += "'" + i + "',";
            }

            dataIs += "]";

            ViewBag.dataIs = dataIs;    



            return View();
           
        }

        public ActionResult Birim()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1011)
            {
                var birimler =(from b in entity.Birimler 
                               where b.aktiflik==true select b).ToList();
                return View(birimler);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Olustur()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1011)
            {
                
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost, ActFilter("Yeni Birim Eklendi")]
        public ActionResult Olustur(string birimAd)
        {
            Birimler yeniBirim =new Birimler();
            string yeniAd =CultureInfo.CurrentCulture.TextInfo.ToTitleCase(birimAd);
            yeniBirim.birimAd=yeniAd;  
            yeniBirim.aktiflik=true;


            entity.Birimler.Add(yeniBirim);
            entity.SaveChanges();

            TempData["bilgi"] = yeniBirim.birimAd;

            return RedirectToAction("Birim");
        }

        public ActionResult Guncelle(int? id)
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1011)
            {
                var birim = (from b in entity.Birimler where b.birimId==id
                             select b).FirstOrDefault();

                return View(birim);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost, ActFilter("Birim Güncellendi")]
        public ActionResult Guncelle(FormCollection fc)
        {
            int birimId = Convert.ToInt32(fc["birimId"]);
            string yeniAd = fc["birimAd"];

            var birim = (from b in entity.Birimler where
                         b.birimId==birimId select b).FirstOrDefault();
            birim.birimAd = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(yeniAd);
            //totitlecase ilk harfleri büyütür
            entity.SaveChanges();

            TempData["bilgi"] = birim.birimAd;

            return RedirectToAction("Birim");
        }
        [ActFilter("Birim Silindi")]
        public ActionResult Sil(int id)
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1011)
            {
                var birim = (from b in entity.Birimler
                             where b.birimId == id
                             select b).FirstOrDefault();

                // burayı silip aşşağıdaki 1 satırı ekldik- entity.Birimler.Remove(birim);
                birim.aktiflik = false;
                //yani bir birim silindiğinde false duurmda olucak

                entity.SaveChanges();

                TempData["bilgi"]= birim.birimAd;

                return RedirectToAction("Birim");
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [AuthFilter(1011)]
        public ActionResult Loglar()
        {
            var loglar = (from l in entity.Loglar
                          orderby l.tarih descending
                          select l).ToList();

            return View(loglar);

        }
    }
}