﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IsTakip.Filters;
using IsTakip.Models;


namespace IsTakip.Controllers
{
    public class LoginController : Controller
    {
        //bu entity sayesinde veritabanıyla bağlantı kurabiliriz
        IsTakipDBEntities entity = new IsTakipDBEntities();
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.mesaj = null;
            return View();
        }
        [HttpPost,ExcFilter]
        public ActionResult Index(string kullaniciAd, string parola)
        {
            var personel = (from p in entity.Personeller
                            where
                             p.personelKullaniciAd == kullaniciAd &&
                             p.personelParola == parola && p.aktiflik==true
                            select p).FirstOrDefault();


            if (personel != null)
            {

                var birim = (from b in entity.Birimler
                             where
                             b.birimId == personel.personelBirimId &&
                             b.aktiflik == true
                             select b).FirstOrDefault();



                Session["PersonelAdSoyad"] = personel.personelAdSoyad;
                Session["PersonelId"] = personel.personelId;
                Session["PersonelBirimId"] = personel.personelBirimId;
                Session["PersonelYetkiTurId"] = personel.personelYetkiTurId;





                if (birim == null)
                {
                    return RedirectToAction("Index", "SistemYoneticisi");
                }



                if (birim.aktiflik == true)
                {


                    if (personel.yeniPersonel == true)
                    {
                        return RedirectToAction("Index", "SifreKontrol");
                    }


                    switch (personel.personelYetkiTurId)
                    {
                        case 1:
                            return RedirectToAction("Index", "Yonetici");
                        case 2:
                            return RedirectToAction("Index", "Calisan");
                        
                        default:
                            return View();
                    }

                }
                else
                {
                    ViewBag.mesaj = "Biriminiz silindiği için giriş yapamazsınız.";
                    return View();
                }
            }
            else
            {
                ViewBag.mesaj = "Kullanıcı adı ya da parola yanlış";
                return View();
            }


        }
    }
}
    