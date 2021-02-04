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
    //DataAnnotations yardımı ile tablo adımız Categories
    //MyEntityBase türettik gerekli nesneleri
    [Table("Categories")]
    public class Category:MyEntityBase
    {
        
        //Alt küçük Başlık,zorunlu,maks 50 karakter
        [DisplayName("Kategori"),Required(ErrorMessage ="{0} alanı gereklidir."),StringLength(50,ErrorMessage = "{0} alanı max. {1} karakter içermelidir.")]
        public string Title { get; set; }
        //Açıklama,maks 150 karakter
        [DisplayName("Açıklama"),StringLength(150, ErrorMessage = "{0} alanı max. {1} karakter içermelidir.")]
        public string Description { get; set; }

        //Kategori içerisindeki birden fazla "List<>" Not olur,başka bir class la ilişkili olduğu için virtual yapıldı
        public virtual List<Note> Notes { get; set; }

        //Category otomatik oluşturuyoruz Not eklemek istediğimizde null hata almamak için yağıyoruz
        public Category()
        {
            Notes = new List<Note>();
        }
    }
}
