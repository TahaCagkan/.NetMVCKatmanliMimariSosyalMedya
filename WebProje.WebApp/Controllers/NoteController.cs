using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebProje.BusinessLayer;
using WebProje.Entities;
using WebProje.WebApp.Filters;
using WebProje.WebApp.Models;

namespace WebProje.WebApp.Controllers
{
    [Exc]
    public class NoteController : Controller
    {

       private NoteManager noteManager = new NoteManager();
       private CategoryManager categoryManager = new CategoryManager();
       private LikedManager likedManager = new LikedManager();

        [Auth]
        public ActionResult Index()
        {
            //Kişi sadece kendi notlarını görücek,Sessiondaki id ye göre notlarını görebilecek

            //Sahibi id si eşit Sessiondaki id, Modifi tarihine göre tersten sıralama
            var notes = noteManager.ListQueryable().Include("Category").Include("Owner").Where(
                x => x.Owner.Id == CurrentSession.User.Id).OrderByDescending(
                x => x.ModifiedOn);

            return View(notes.ToList());
        }

        #region Beğendiklerim
        [Auth]
        public ActionResult MyLikedNotes()
        {
            //Liked tablosundaki liked ları nesnesini döner,o ilişkiler harici LikedUser içerisindeki ilişkili bilgileride alalım, o Sessiondaki kullanıcıya ait,hangi notlar like lanmış bunları seç nesneleri dön ordanda tarih'e göre tersten sıralama yap
            var notes = likedManager.ListQueryable().Include("LikedUser").Include("Note").Where(
                x => x.LikedUser.Id == CurrentSession.User.Id).Select(
                x => x.Note).Include("Category").Include("Owner").OrderByDescending(
                x => x.ModifiedOn);

            return View("Index",notes.ToList());
        }
        #endregion

        #region Detaylar
        [Auth]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = noteManager.Find(x => x.Id == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        #endregion

        #region Oluşturma
        [Auth]
        public ActionResult Create()
        {
            //ViewBag.CategoryId = new SelectList(categoryManager.List(), "Id", "Title");
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title");
            return View();
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Note note)
        {
            //Oluşturma Tarihi,Kimin Oluştuduğu,Oluşturanın kullanıcı adı ModelState de varsa sil
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                note.Owner = CurrentSession.User;
                noteManager.Insert(note);
                return RedirectToAction("Index");
            }

            //ViewBag.CategoryId = new SelectList(categoryManager.List(), "Id", "Title", note.CategoryId);
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }

        #endregion


        #region Düzenle

        [Auth]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Notu id ye göre bulma
            Note note = noteManager.Find(x => x.Id == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CategoryId = new SelectList(categoryManager.List(), "Id", "Title", note.CategoryId);
            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            
            return View(note);
        }

        [Auth]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Note note)
        {
            //Oluşturma Tarihi,Kimin Oluştuduğu,Oluşturanın kullanıcı adı ModelState de varsa sil
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");


            if (ModelState.IsValid)
            {
                //Not'u id sine göre bul daha sonra not bu sayfadan gelicek, Category id sini ,Text ve Title güncelleştirme için gerekli
                Note db_note = noteManager.Find(x => x.Id == note.Id);
                db_note.IsDraft = note.IsDraft;
                db_note.CategoryId = note.CategoryId;
                db_note.Text = note.Text;
                db_note.Title = note.Title;

                //güncelleştirme işlemi
                noteManager.Update(db_note);

                return RedirectToAction("Index");
            }
            //ViewBag.CategoryId = new SelectList(categoryManager.List(), "Id", "Title", note.CategoryId);

            ViewBag.CategoryId = new SelectList(CacheHelper.GetCategoriesFromCache(), "Id", "Title", note.CategoryId);
            return View(note);
        }
        #endregion

        #region Silme

        [Auth]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Notu id ye göre bulma
            Note note = noteManager.Find(x => x.Id == id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        [Auth]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Notu id ye göre bulma
            Note note = noteManager.Find(x => x.Id == id);
            noteManager.Delete(note);
            return RedirectToAction("Index");
        }
        #endregion

        #region Like(Beğenme) işlemi

        [HttpPost]
        public ActionResult GetLiked(int[] ids)
        {
            if (CurrentSession.User != null)
            {
                //Şuan içerisindeki Sessionda kullanıcıların like'ını getir
                //&& ve notların id parametreleri ids dizinde ile geliyor,bu liste içeriyor mu Notun id sini
                //Bana daha sonra notların id sini verip Listeye dönüştürüyorum
                List<int> likedNoteIds = likedManager.List(
                    x => x.LikedUser.Id == CurrentSession.User.Id && ids.Contains(x.Note.Id)).Select(
                    x => x.Note.Id).ToList();

                return Json(new { result = likedNoteIds });
            }
            else
            {
                return Json(new { result = new List<int>() });
            }
        }

        [HttpPost]
        public ActionResult SetLikeState(int noteid, bool liked)
        {
            int res = 0;

            if (CurrentSession.User == null)
                return Json(new { hasError = true, errorMessage = "Beğenme işlemi için giriş yapmalısınız.", result = 0 });
            //Like var mı o kullanıcıya ait?
            Liked like =
                likedManager.Find(x => x.Note.Id == noteid && x.LikedUser.Id == CurrentSession.User.Id);

            Note note = noteManager.Find(x => x.Id == noteid);
            //like dolu gelmiş ise bidaha like yapılmaması 
            if (like != null && liked == false)
            {
                res = likedManager.Delete(like);
            }
            else if (like == null && liked == true) //like yapmadığını like yapmamaya çalışamaz
            {
                res = likedManager.Insert(new Liked()
                {
                    LikedUser = CurrentSession.User,
                    Note = note
                });
            }

            if (res > 0)
            {
                if (liked)
                {
                    note.LikeCount++; //like yapma
                }
                else
                {
                    note.LikeCount--; //like yapmama
                }

                res = noteManager.Update(note);

                return Json(new { hasError = false, errorMessage = string.Empty, result = note.LikeCount });
            }

            return Json(new { hasError = true, errorMessage = "Beğenme işlemi gerçekleştirilemedi.", result = note.LikeCount });
        }
        #endregion
        #region Note Devam Butonu

        public ActionResult GetNoteText(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Note note = noteManager.Find(x => x.Id == id);

            if (note == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialNoteText", note);
        }

        #endregion
    }
}