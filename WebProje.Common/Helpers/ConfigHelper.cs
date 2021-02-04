using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProje.Common.Helpers
{
   public class ConfigHelper
   {
        //Generic oluacak ben ne tip verirsem o tipi döndürsün
        //Web.config içerisinde appSettings SMTP yardımı ile mail işlemleri yapıldı
        public static T Get<T>(string key)
        {
            //generic olduğu için nesne için tip dönüşüşmü yapıldı. 
           return (T)Convert.ChangeType(ConfigurationManager.AppSettings[key], typeof(T));
        }
   }
}
