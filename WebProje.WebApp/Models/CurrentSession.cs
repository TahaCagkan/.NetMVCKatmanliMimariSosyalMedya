using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProje.Entities;

namespace WebProje.WebApp.Models
{
    //Session helper method

    public class CurrentSession
    {
      
        public static MyArticlesUser User
        {
            //Session erişim yapıcağız
            get {
                /*
                if(HttpContext.Current.Session["login"] != null)
                {
                    return HttpContext.Current.Session["login"] as MyArticlesUser;
                }
                return null;
                */
                return Get<MyArticlesUser>("login");
            }        
        }

        //verdiğimiz anahtarı verdiğimiz Sessiona atıcak
        public static void Set<T>(string key,T obj)
        {
            HttpContext.Current.Session[key] = obj;
        }
        //geriye döndürme 
        public static T Get<T>(string key)
        {
            if(HttpContext.Current.Session[key] != null)
            {
                return (T)HttpContext.Current.Session[key];
            }

            return default(T);
        }

        //Kaldırma
        public static void Remove(string key)
        {
            if (HttpContext.Current.Session[key] != null)
            {
                HttpContext.Current.Session.Remove(key);
            }
        }

        //geriye değer döndürmeyen,tüm Session temizleme
        public static void Clear()
        {
            HttpContext.Current.Session.Clear();
        }
    }
}