using IsTakipSistemiMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IsTakipSistemiMVC.Controllers
{
    public class YoneticiController : Controller
    {
        IsTakipDBEntities entitiy = new IsTakipDBEntities();
        // GET: Yonetici
        public ActionResult Index()
        {
           int yetkiTurId=Convert.ToInt32( Session["PersonelYetkiTurId"]);
            
            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var birim=(from b in entitiy.Birimler where b.birimId == birimId select b).FirstOrDefault();
                ViewBag.birimAd = birim.birimAd;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Ata()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if(yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var calısanlar=(from p in entitiy.Personeller where p.personelBirimId == birimId && p.personelYetkiTurId==2
                                select p).ToList();

                ViewBag.personeller = calısanlar;
               
                var birim = (from b in entitiy.Birimler where b.birimId == birimId select b).FirstOrDefault();
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

            string isBaslık = formCollection["isBaslık"];
            string isAcıklama = formCollection["isAcıklama"];
            int secilenPersonelId = Convert.ToInt32(formCollection["selectPer"]);

            Isler yeniIs = new Isler();
            yeniIs.isBaslık = isBaslık;
            yeniIs.isAcıklama = isAcıklama;
            yeniIs.isPersonelId = secilenPersonelId;
            yeniIs.iletilenTarih = DateTime.Now;
            yeniIs.isDurumId = 1;
            yeniIs.isOkunma = false;
            entitiy.Isler.Add(yeniIs);
            entitiy.SaveChanges();

            return RedirectToAction("Takip", "Yonetici");
        }

        public ActionResult Takip()
        {
            int yetkiTurId = Convert.ToInt32(Session["PersonelYetkiTurId"]);
            if (yetkiTurId == 1)
            {
                int birimId = Convert.ToInt32(Session["PersonelBirimId"]);
                var calısanlar = (from p in entitiy.Personeller
                                  where p.personelBirimId == birimId && p.personelYetkiTurId == 2
                                  select p).ToList();

                ViewBag.personeller = calısanlar;

                var birim = (from b in entitiy.Birimler where b.birimId == birimId select b).FirstOrDefault();
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
            var secilenPersonel= (from p in entitiy.Personeller where p.personelId == selectPer 
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
                try
                {
                   
                    var isler = (from i in entitiy.Isler where i.isPersonelId == secilenPersonel.personelId 
                                 select i).ToList().OrderByDescending(i => i.iletilenTarih);

                    ViewBag.isler = isler;
                    ViewBag.personel = secilenPersonel;
                    ViewBag.isSayisi = isler.Count();
                    return View();
                }
                catch (Exception)
                {
                    return RedirectToAction("Takip", "Yonetici");
                }
                
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

    }
}