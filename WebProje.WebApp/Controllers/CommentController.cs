using System;
using System.Collections.Generic;
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
    public class CommentController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        private CommentManager commentManager = new CommentManager();

        #region Not daki yorumu gösterme

        public ActionResult ShowNoteComments(int? id)
        {
            //eğer null ise geriye bunu dön
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //eğer değil ise id gelmiştir.Bana id olan notu bana ver

            //Note note = noteManager.Find(x => x.Id == id);
            //not  ile Commend ile çek
            Note note = noteManager.ListQueryable().Include("Comments").FirstOrDefault(x => x.Id == id);
            //Not boş gelmiş ise
            if (note == null)
            {
                return HttpNotFound();
            }

            return PartialView("_PartialComments", note.Comments);
        }

        #endregion

        #region Düzenleme

        //Düzenleme Post
        [Auth]
        [HttpPost]
        public ActionResult Edit(int? id, string text)
        {
            //id null ise
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Comment id si olanı bana getir
            Comment commet = commentManager.Find(x => x.Id == id);

            //Comment null ise
            if (commet == null)
            {
                return new HttpNotFoundResult();
            }

            //eğer null değilse comment içerisindeki Text'i parametre ile gelen text ile güncelle
            commet.Text = text;

            //sonuç başarılı ise Update et
            if (commentManager.Update(commet) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }

            //işlem başarızsa false
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Silme işlemi
        [Auth]
        public ActionResult Delete(int? id)
        {
            //id null ise
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Comment id si olanı bana getir
            Comment commet = commentManager.Find(x => x.Id == id);

            //Comment null ise
            if (commet == null)
            {
                return new HttpNotFoundResult();
            }
            //sonuç başarılı ise Delete et
            if (commentManager.Delete(commet) > 0)
            {
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            //işlem başarızsa false
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Oluşturma İşlemi
        [Auth]
        [HttpPost]
        public ActionResult Create(Comment comment, int? noteid)
        {

            //Oluşturma Tarihi,Kimin Oluştuduğu,Oluşturanın kullanıcı adı ModelState de varsa sil
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                //id null ise
                if (noteid == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                //Note id si olanı bana getir
                Note note = noteManager.Find(x => x.Id == noteid);

                //Comment null ise
                if (note == null)
                {
                    return new HttpNotFoundResult();
                }

                //Yorumun notu
                comment.Note = note;
                //Yorumun sahibi
                comment.Owner = CurrentSession.User;

                //sonuç başarılı ise INSERT et
                if (commentManager.Insert(comment) > 0)
                {
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }

            }
            //işlem başarızsa false
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}