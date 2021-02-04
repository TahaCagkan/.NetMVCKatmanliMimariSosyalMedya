using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProje.Common
{
    //Dışarıdan erişilcek olan sınıf.
    public static class App
    {
        //ICommon türünden değişken içeriyor
        public static ICommon Common = new DefaultCommon();
    }
}
