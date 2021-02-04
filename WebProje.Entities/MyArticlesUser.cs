using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProje.Entities
{
    //MyEntityBase türettik,Tablo adı MyArticlesUsers
    [Table("MyArticlesUsers")]
    public class MyArticlesUser:MyEntityBase
   {
        //Kişi Ad,Maks uzunluğu 30 karakter
        [DisplayName("İsim"),
            StringLength(30,ErrorMessage ="{0} alanı max. {1} karakter olmalıdır.")]
        public string Name { get; set; }
        //Kişi Soyad,  [StringLength(30)]
        [DisplayName("Soyad"), 
            StringLength(30,ErrorMessage ="{0} alanı max. {1} karakter olmalıdır.")]
        public string Surname { get; set; }
        //Kullanıcı Adı,Zorunlu,Maks 25 karakter
        [DisplayName("Kullanıcı Adı"), 
            Required(ErrorMessage ="{0} alanı gereklidir."),
            StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Username { get; set; }
        //Kullanıcı Email,Zorunlu,Maks 70 karakter
        [DisplayName("E-Posta"), 
            Required(ErrorMessage = "{0} alanı gereklidir."), 
            StringLength(70, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Email { get; set; }
        //Kullanıcı Şifresi,Zorunlu,Maks 25 karakter
        [DisplayName("Şifre"), 
            Required(ErrorMessage = "{0} alanı gereklidir."),
            StringLength(25, ErrorMessage = "{0} alanı max. {1} karakter olmalıdır.")]
        public string Password { get; set; }
        //Kullanıcı Resimi
        [StringLength(30),ScaffoldColumn(false)]
        public string ProfileImageFilename { get; set; }
        //Kullanıcı üyeliğini aktif etmiş mi?
        [DisplayName("Aktif mi?")]
        public bool IsActive { get; set; }
        //Kullanıcı aktif etme sırsında link üzerinden Guid(Random) gönderme,Zorunlu
        [Required, ScaffoldColumn(false)]
        public Guid ActivateGuid { get; set; }
        //Giriş Yapan Kullanıcı mı ? Yoksa Yönetici mi?
        [DisplayName("Admin mi?")]
        public bool IsAdmin { get; set; }

        //Bir Kullanıcının birden fazla "List<>" Not olur,başka bir class la ilişkili olduğu için virtual yapıldı
        public virtual List<Note> Notes { get; set; }
        //Bir Kullanıcının birden fazla "List<>" Yorum olur,başka bir class la ilişkili olduğu için virtual yapıldı
        public virtual List<Comment> Comments { get; set; }

    }
}
