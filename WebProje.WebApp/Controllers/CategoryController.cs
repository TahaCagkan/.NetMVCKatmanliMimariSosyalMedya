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
    [Auth]
    [AuthAdmin]
    [Exc]
    public class CategoryController : Controller
    {
        //BussinessLayer için kullanılan CategoryManager kullanılcak.
        private CategoryManager categoryManager = new CategoryManager();

        
        // GET: Category
        public ActionResult Index()
        {
            //Kategori Listesini göster
            return View(categoryManager.List());
        }
        #region Detay Sayfası

        // GET: Category Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Kategori Detayı için GetCategoryById methodunu kullanarak id ye göre buluyoruz.
            //Category category =categoryManager.GetCategoryById(id.Value);
            Category category = categoryManager.Find(x => x.Id == id.Value);
            //Category gelemişse bulunamadı hatası dönüyoruz
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        #endregion

        #region Oluşturma Sayfası


        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Category category)
        {
            //Oluşturma Tarihi,Kimin Oluştuduğu,Oluşturanın kullanıcı adı ModelState de varsa sil
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");


            if (ModelState.IsValid)
            {
                /*
                db.Categories.Add(category);
                db.SaveChanges();
                */
                categoryManager.Insert(category);
                //Insert'ten sonra yeni bir kategori eklemişsem Cache silinsinki yenibir kategori eklenebilsin
                CacheHelper.RemoveCategoriesFromCache();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        #endregion

        #region Düzenleme işlemi

        // GET: Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Category category = db.Categories.Find(id);
            Category category = categoryManager.Find(x => x.Id == id.Value); 
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Category category)
        {
            //Oluşturma Tarihi,Kimin Oluştuduğu,Oluşturanın kullanıcı adı ModelState de varsa sil
            ModelState.Remove("CreatedOn");
            ModelState.Remove("ModifiedOn");
            ModelState.Remove("ModifiedUsername");

            if (ModelState.IsValid)
            {
                /*
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                */
                //Edit yaparken ilgili kategori nesnesini bulmamız lazım
                Category cat = categoryManager.Find(x => x.Id == category.Id);
                //Başlık ve açıklamaları modelden gelenlerle değiştirip Güncelliyoruz.
                cat.Title = category.Title;
                cat.Description = category.Description;

                categoryManager.Update(cat);
                //Update'ten sonra yeni bir kategori eklemişsem Cache silinsinki yenibir kategori eklenebilsin
                CacheHelper.RemoveCategoriesFromCache();
                return RedirectToAction("Index");
            }
            return View(category);
        }
        #endregion

        #region Silme işlemi

        // GET: Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Category category = db.Categories.Find(id);
            Category category = categoryManager.Find(x => x.Id == id.Value);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            /*
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            */
            //Category id sine göre bulunup daha sonra silinmektedir.
            Category category = categoryManager.Find(x => x.Id == id);
            categoryManager.Delete(category);

            //Delete'ten sonra yeni bir kategori eklemişsem Cache silinsinki yenibir kategori eklenebilsin
            CacheHelper.RemoveCategoriesFromCache();
            return RedirectToAction("Index");
        }

        #endregion

    }
}
