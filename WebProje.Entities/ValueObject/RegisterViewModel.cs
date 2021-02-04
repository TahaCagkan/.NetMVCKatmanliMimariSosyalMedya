using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProje.Entities.ValueObject
{
    //Üyelik işlemleri
    public class RegisterViewModel
    {
        //Kullanıcı Adı
        [DisplayName("Kullanıcı Adı"),
         Required(ErrorMessage = "{0} alanı boş geçilemez."),
         StringLength(25,ErrorMessage ="{0} max. {1} karekter olmalı.")]
        public string Username { get; set; }

        //Email
        [DisplayName("E-Posta"), 
         Required(ErrorMessage = "{0} alanı boş geçilemez."), 
         StringLength(70, ErrorMessage = "{0} max. {1} karekter olmalı."),
         EmailAddress(ErrorMessage ="{0} alanı için geçerli bir e-posta adresş giriniz.")]
        public string Email { get; set; }

        //Şifre
        [DisplayName("Şifre"), 
         Required(ErrorMessage = "{0} alanı boş geçilemez."),
         StringLength(25, ErrorMessage = "{0} max. {1} karekter olmalı.")]
        public string Password { get; set; }

        //Şifre Tekrar
        [DisplayName("Şifre Tekrar"), 
         Required(ErrorMessage = "{0} alanı boş geçilemez."), 
         StringLength(25, ErrorMessage = "{0} max. {1} karekter olmalı."),
         Compare("Password",ErrorMessage ="{0} ile {1} uyuşmuyor.")]
        public string RePassword { get; set; }

    }
}