using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProje.Entities.Messages;

namespace WebProje.WebApp.ViewModels
{
    //Miras aldığı : NotifyViewModelBase , tip aldığı : ErrorMessageObj
    //burada ErrorViewModel içerisindeki List oldu
    public class ErrorViewModel :NotifyViewModelBase<ErrorMessageObj>
    {

    }
}