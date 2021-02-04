using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WebProje.Common;
using WebProje.WebApp.Init;

namespace WebProje.WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Program ayağa kalktığında ilk çalıştırıldığında App Class içerisindeki Common ulşamasını istiyorum,Common türü ICommon oldğundan dolayı WebCommonda inherit yaptık

            //App.Common dediğim sen artık WebCommon çalışıcaksın diyorum oda bu using WebProje.WebApp.Init; altında,DefaultCommon la değil WebCommon la çalışıcaktır. WebCommon Username ile çalıcaktır.
            //Amaç DataAccessLayer içersindeki Repository patternda kullanıcının adını almak istediğimizde App class içerisindeki Common nesnenin Usernamemethodunu çağırıcağım, sana o anki Session içersindeki kullanıcı adını dönücektir,Kısacası erişimi güvenlik için yapılmaktadır.
            App.Common = new WebCommon();
        }
    }
}
