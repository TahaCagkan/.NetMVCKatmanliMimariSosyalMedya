using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProje.WebApp.Models;

namespace WebProje.WebApp.Filters
{
    //Kişi yönetici değilse,yöneticiye(admin) özel sayfaya (linki) deneyerek giremez
    public class Auth : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if(CurrentSession.User == null)
            {
                //Home Control içerisindeki Login methoduna yönlendir.
                filterContext.Result = new RedirectResult("/Home/Login");
            }
        }
    }
}