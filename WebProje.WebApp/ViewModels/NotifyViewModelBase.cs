using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProje.WebApp.ViewModels
{
    //generic class
    //List türünden verilen tiplerin birimleri bulunucak 
    public class NotifyViewModelBase<T>
    {
        //İçerisindeki Error leri tutucak List em
        public List<T> Items { get; set; }
        //Header Kısmı
        public string Header { get; set; }
        //Title Kısmı
        public string Title { get; set; }
        //script bölümünde function içerisinde Yönlendirmeler
        public bool IsRedirecting { get; set; }
        //Redirectin Url si
        public string RedirectingUrl { get; set; }
        //Redirecting Çıkış süresi
        public int RedirectingTimeout { get; set; }

        //Her defasında bilgileri girmemek için constructor yapıldı
        public NotifyViewModelBase()
        {
            Header = "Yönlendiriliyorsunuz...";
            Title = "Geçersiz İşlem";
            IsRedirecting = true;
            RedirectingUrl = "/Home/Index";
            RedirectingTimeout = 10000;
            Items = new List<T>();
        }
    }
}