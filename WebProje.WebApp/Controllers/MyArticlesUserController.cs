using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebProje.BusinessLayer;
using WebProje.BusinessLayer.Results;
using WebProje.Entities;
using WebProje.WebApp.Filters;

namespace WebProje.WebApp.Controllers
{
    [Auth]
    [AuthAdmin]
    [Exc]
    public class MyArticlesUserController : Controller
    {
        private MyArticlesUserManager myArticlesUserManager = new MyArticlesUserManager();

        public ActionResult Index()
        {
            // Listeleme
            return View(myArticlesUserManager.List());
        }

        #region Detaylar

       
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //id ye göre bulma
            MyArticlesUser myArticlesUser = myArticlesUserManager.Find(x => x.Id == id.Value);
            if (myArticlesUser == null)
            {
                return HttpNotFound();
            }
            return View(myArticlesUser);
        }
        #endregion

        #region Oluşturma

        public ActionResult Create()
        {
            return View();
        }

        // POST: MyArticlesUser/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( MyArticlesUser myArticlesUser)
        {
            //Oluşturma Tarihi,Kimin Oluştuduğu,Oluşturanın kullanıcı adı ModelState de varsa sil
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                //Username ve Email kontrolleri var
                BusinessLayerResult<MyArticlesUser> layerResult = myArticlesUserManager.Insert(myArticlesUser);

                //Hata var ise,her bir error'un ModelState  ModelError ekle
                if(layerResult.Errors.Count > 0)
                {
                    layerResult.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(myArticlesUser);
                }

                return RedirectToAction("Index");
            }

            return View(myArticlesUser);
        }
        #endregion

        #region Düzenleme
        // GET: MyArticlesUser/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Kullanıcı id sine göre bul
            MyArticlesUser myArticlesUser = myArticlesUserManager.Find(x => x.Id == id.Value);
            if (myArticlesUser == null)
            {
                return HttpNotFound();
            }
            return View(myArticlesUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( MyArticlesUser myArticlesUser)
        {
            //Oluşturma Tarihi,Kimin Oluştuduğu,Oluşturanın kullanıcı adı ModelState de varsa sil
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                BusinessLayerResult<MyArticlesUser> layerResult = myArticlesUserManager.Update(myArticlesUser);

                //Hata var ise,her bir error'un ModelState  ModelError ekle
                if (layerResult.Errors.Count > 0)
                {
                    layerResult.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(myArticlesUser);
                }
                return RedirectToAction("Index");
            }

            return View(myArticlesUser);
        }
        #endregion

        #region Silme

        // GET: MyArticlesUser/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Silme işleminde Kullanıcı id sine göre bul
            MyArticlesUser myArticlesUser = myArticlesUserManager.Find(x => x.Id == id.Value);
            if (myArticlesUser == null)
            {
                return HttpNotFound();
            }
            return View(myArticlesUser);
        }

        // POST: MyArticlesUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MyArticlesUser myArticlesUser = myArticlesUserManager.Find(x => x.Id == id);
            myArticlesUserManager.Delete(myArticlesUser);

            return RedirectToAction("Index");
        }
        #endregion
    }
}
