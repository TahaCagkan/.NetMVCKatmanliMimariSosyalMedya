using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProje.WebApp.ViewModels
{
    //miras aldığı: NotifyViewModelBase, tipi :string
    public class OkViewModel:NotifyViewModelBase<string>
    {
        public OkViewModel()
        {
            Title = "İşlem Başarılı.";
        }
    }
}