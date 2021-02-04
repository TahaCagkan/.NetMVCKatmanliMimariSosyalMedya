using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProje.Entities.ValueObject
{ 
    public class LoginViewModel
    {
        //Kullanıcı Adı
        [DisplayName("Kullanıcı Adı"),Required(ErrorMessage ="{0} alanı boş geçilemez."), StringLength(25, ErrorMessage = "{0} max. {1} karekter olmalı.")]
        public string Username { get; set; }

        //Şifre
        [DisplayName("Şifre"),Required(ErrorMessage = "{0} alanı boş geçilemez."),DataType(DataType.Password), StringLength(25, ErrorMessage = "{0} max. {1} karekter olmalı.")]
        public string Password { get; set; }
    }
}