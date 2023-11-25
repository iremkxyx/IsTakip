using IsTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakip.Controllers
{
    public class YoneticiController : Controller
    {
        IsTakipDBEntities entity = new IsTakipDBEntities();
        // GET: Yonetici
        public ActionResult Index()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var birim = (from b in entity.Birimler
                             where b.birimId == birimId
                             select b).FirstOrDefault();

                ViewBag.birimAd = birim.birimAd;

                var personeller = from p in entity.Personeller
                                  join i in entity.Isler
                                 on p.personelId equals i.isPersonelId into isler
                                  where p.personelBirimId == birimId && p.personelYetkiTurId != 1
                                  select new { personelAd = p.personelAdSoyad, isler = isler };

                List<ToplamIs> list = new List<ToplamIs>();


                foreach (var personel in personeller)
                {
                    ToplamIs toplamIs = new ToplamIs();
                    toplamIs.personelAdSoyad = personel.personelAd;

                    if (personel.isler.Count() == 0)
                    {
                        toplamIs.toplamIs = 0;
                    }
                    else
                    {
                        int toplam = 0;
                        foreach (var item in personel.isler)
                        {
                            if (item.yapilanTarih != null)
                            {
                                toplam++;
                            }
                        }
                        toplamIs.toplamIs = toplam;
                    }

                    list.Add(toplamIs);
                }

                IEnumerable<ToplamIs> siraliListe = new List<ToplamIs>();  //liste olşutrduk
                siraliListe = list.OrderByDescending(i => i.toplamIs);

                return View(siraliListe);

            }
            else
            {
                
                return RedirectToAction("Index", "Login");
            }
           
        }

        public ActionResult Ata()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {

                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var calisanlar = (from p in entity.Personeller
                                  where
                                  p.personelBirimId ==birimId && p.personelYetkiTurId == 2
                                  select p).ToList();
                ViewBag.personeller = calisanlar;

                //

                var birim = (from b in entity.Birimler
                             where b.birimId == birimId
                             select b).FirstOrDefault();

                ViewBag.birimAd = birim.birimAd;


                return View();
            }
            else
            {


                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Ata(FormCollection formCollection)
        {
            string isBaslik = formCollection["isBaslik"];
            string isAciklama = formCollection["isAciklama"];
            int secilenPersonelId = Convert.ToInt32(formCollection["selectPer"]);

            Isler yeniIs = new Isler();
            yeniIs.isBaslik = isBaslik;
            yeniIs.isAciklama = isAciklama;
            yeniIs.isPersonelId = secilenPersonelId;
            yeniIs.iletilenTarih = DateTime.Now;
            yeniIs.isDurumId = 1; //yapılıyor
            yeniIs.isOkunma = false; //okunmadı başlangıç

            entity.Isler.Add(yeniIs);
            entity.SaveChanges();


            return RedirectToAction("Takip", "Yonetici");

        }

        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {

                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

                var calisanlar = (from p in entity.Personeller
                                  where
                                  p.personelBirimId == birimId && p.personelYetkiTurId == 2
                                  && p.aktiflik==true
                                  select p).ToList();

                ViewBag.personeller = calisanlar;

                //

                var birim = (from b in entity.Birimler
                             where b.birimId == birimId
                             select b).FirstOrDefault();

                ViewBag.birimAd = birim.birimAd;


                return View();
            }
            else
            {


                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public ActionResult Takip(int selectPer)
        {
            var secilenPersonel = (from p in entity.Personeller
                                   where
                                   p.personelId == selectPer
                                   select p).FirstOrDefault();

            TempData["secilen"] = secilenPersonel;

            return RedirectToAction("Listele", "Yonetici");

        }
        [HttpGet]

        public ActionResult Listele()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {

                Personeller secilenPersonel = (Personeller)TempData["secilen"];

                //bunu yapmamızdaki amaç şu urlden yonetici/listele ye basınca
                //listele açılmıcak önce takip sayfasına atması gerekiyo ki
                //önce takipten birini seçsin ve listele desin

                //burda viewbag ile gönderiyoruz calisancontrollerda model kullanıoz

                try //hata yoksa bura çalışacak
                {
                    var isler = (from i in entity.Isler
                                 where
                            i.isPersonelId == secilenPersonel.personelId
                                 select i).ToList().OrderByDescending(i => i.iletilenTarih);
                    ViewBag.isler = isler;
                    ViewBag.personel = secilenPersonel;

                    ViewBag.isSayisi = isler.Count();

                    return View();
                }
                catch (Exception) //hata olduğunda çalışacak alan burası
                {

                    return RedirectToAction("Takip", "Yonetici");
                }

              
            }
            else
            {

                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public ActionResult AyinElemani()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);

            if (yetkiTurId == 1)
            {
                int simdikiYil =DateTime.Now.Year;
                List<int> yillar = new List<int>();
                for(int i =simdikiYil; i>=2023; i--)
                {
                    yillar.Add(i);
                }
                ViewBag.yillar = yillar;
                ViewBag.ayinElemani = null;

                //yöneticiyse eğer ayın elemanlarını göstermesi için ==1
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
        public ActionResult AyinElemani(int aylar,int yillar)
        {

            int birimId = Convert.ToInt32(Session["PersonelBirimId"]);

            var personeller = entity.Personeller.Where(p => p.personelBirimId == birimId).Where(p =>
            p.personelYetkiTurId != 1);

            DateTime baslangicTarih = Convert.ToDateTime("01-" + aylar + "-" + yillar);
            DateTime bitisTarih = Convert.ToDateTime("31-" + aylar + "-" + yillar + " 23:59:59");




            var isler = entity.Isler.Where(i=>i.yapilanTarih>=
            baslangicTarih).Where(i=>i.yapilanTarih<= bitisTarih);

            var groupJoin = personeller.GroupJoin(isler, p =>
            p.personelId, i => i.isPersonelId, (p, group) => new
            {
                sonucIsler = group,
                personelAd=p.personelAdSoyad
            });
            List<ToplamIs> List = new List<ToplamIs>();
            foreach (var personel in groupJoin)
            {
                ToplamIs toplamIs = new ToplamIs();
                toplamIs.personelAdSoyad = personel.personelAd;

                if (personel.sonucIsler.Count() == 0)
                {
                    toplamIs.toplamIs = 0;
                }
                else
                {
                    int toplam = 0;
                    foreach (var item in personel.sonucIsler)
                    {
                        if (item.yapilanTarih != null)
                        {
                            toplam++;
                        }
                    }
                    toplamIs.toplamIs = toplam;
                }
                List.Add(toplamIs);

            }

            IEnumerable <ToplamIs> siraliListe = new List<ToplamIs>();
            siraliListe = List.OrderByDescending(i => i.toplamIs);

            ViewBag.ayinElemani=siraliListe.FirstOrDefault();

            int simdikiYil = DateTime.Now.Year;
            List<int> sonucYillar = new List<int>();
            for (int i = simdikiYil; i >= 2023; i--)
            {
                sonucYillar.Add(i);
            }
            ViewBag.yillar = sonucYillar;

            return View();
        }




    }
}