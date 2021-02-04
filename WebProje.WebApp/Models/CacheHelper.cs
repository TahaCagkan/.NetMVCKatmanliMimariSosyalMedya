using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using WebProje.BusinessLayer;
using WebProje.Entities;

namespace WebProje.WebApp.Models
{
    public class CacheHelper
    {
        public static List<Category> GetCategoriesFromCache()
        {
            var result = WebCache.Get("category-cache");
            //Cach boş ise
            if(result == null)
            {
                CategoryManager categoryManager = new CategoryManager();
                result = categoryManager.List();

                WebCache.Set("category-cache", result, 20, true);
            }

            return result;
        }

        public static void RemoveCategoriesFromCache()
        {
            Remove("category-cache");
        }

        //Insert ederken sorun çıkmaması için Cache siliyoruz
        public static void Remove(string key)
        {
            WebCache.Remove(key);
        }
    }
}