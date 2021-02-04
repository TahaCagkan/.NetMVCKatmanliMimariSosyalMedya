using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProje.Common;
using WebProje.Entities;
using WebProje.WebApp.Models;

namespace WebProje.WebApp.Init
{
    //ICommon miras alması gerekiyor , App.Common türü alanı(field) ICommon olduğu için 
    public class WebCommon : ICommon
    {
        public string GetCurrentUsername()
        {
            /*
            //Webte GetUsername Session da login olarak tutuyoruz.
            //Session erişim login nesnem item var mı?
            if(HttpContext.Current.Session["login"] != null)
            {
                //MyArticlesUser nesnesine çevirdim
                MyArticlesUser user = HttpContext.Current.Session["login"] as MyArticlesUser;

                return user.Username;
            }
            */
            MyArticlesUser user = CurrentSession.User;
            if(user != null)
            {
                return user.Username;
            }
            else
            return "system: Yeni Kayıt"; //Eğer yoksa null dön
        }
    }
}