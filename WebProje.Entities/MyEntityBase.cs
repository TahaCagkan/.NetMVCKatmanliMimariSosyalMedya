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
    public class MyEntityBase
    {
        //Id,Key olduğunu ve otomatik artan verdik Id'sine
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        //Admin ilişkileri ve işlemleri

        //Oluşturulma,Zorunlu
        [DisplayName("Oluşturma Tarihi"),Required]
        public DateTime CreatedOn { get; set; }
        //Düzenlendiği Tarih,Zorunlu
        [DisplayName("Düzenleme Tarihi"),Required]
        public DateTime ModifiedOn { get; set; }
        //Kimin Tarafında güncellendi veya dosya oluşturuldu yorum yaptı gibi....,Zorunlu,maks 30 karakter
        [DisplayName("Düzenleyen Kullanıcı"),Required,StringLength(30)]
        public string ModifiedUsername { get; set; }
        //Bir Kullanıcının birden çok Beğenisi vardır
        public List<Liked> Likes { get; set; }
    }
}
