using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProje.DataAccessLayer.MySql
{
   public class RepositoryBase
    {
        //new olmasını istemiyorum,hep var olanı dönücek
        protected static object db; //DatabaseContext veritabanı işlemimiz
        //bu yüzden protected class tanımlıyoruz
        private static object _lockSyn = new object();

        protected RepositoryBase()
        {
            CreateContext(); //db oluşmasını sağlıyoruz
        }

        public static void CreateContext()
        {
            if (db == null)
            {
                lock (_lockSyn) //bir işi bitirmeden diğerine geçme
                {
                    if (db == null)
                    {
                        db = new object();
                    }
                }

            }

        }
    }
}

