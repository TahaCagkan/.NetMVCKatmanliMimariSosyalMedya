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
    //MyEntityBase türettik,Tablo Adı Notes
    [Table("Notes")]
    public class Note:MyEntityBase
    {
        //Not Başlığı,Zorunlu,Maks 60 karakter
        [DisplayName("Not Başlığı"),Required, StringLength(60)]
        public string Title { get; set; }
        //Not içerisindeki Metin,Zorunlu,Maks 2000 karakter
        [DisplayName("Not Metni"), Required,StringLength(2000)]
        public string Text { get; set; }
        //Taslak mı?
        [DisplayName("Taslak")]
        public bool IsDraft { get; set; }
        //Beğenme Sayacı
        [DisplayName("Beğenilme")]
        public int LikeCount { get; set; }
        //İlişkili olduğu tablonun(nesnenin) ismi daha sonra ilişkili olduğu property nin adı,Category tablosunda çoka çok ilişki olduğu için yapıldı,Category Id tutcağız burada istediğimiz Category daha çabuk ulaşıcağız
        [DisplayName("Kategori")]
        public int CategoryId { get; set; }


        //Bir yorumun bir kullanıcısı vardır sahibi anlamında,başka bir class la ilişkili olduğu için virtual yapıldı
        public virtual MyArticlesUser Owner { get; set; }
        //Bir yorumun birden fazla Kategorisi vardır ,başka bir class la ilişkili olduğu için virtual yapıldı
        public virtual Category Category { get; set; }
        //Bir yorumun birden fazla yorumu "List<Comment>" vardır ,başka bir class la ilişkili olduğu için virtual yapıldı
        public virtual List<Comment> Comments { get; set; }
        //Bir Notun birden çok Beğenileni vardır,,başka bir class la ilişkili olduğu için virtual yapıldı
        public virtual List<Liked> Likes { get; set; }


        //Not otomatik oluşturuyoruz Yorum ve Like  eklemek istediğimizde null hata almamak için yağıyoruz
        public Note()
        {
            Comments = new List<Comment>();
            Likes = new List<Liked>();
        }

    }
}
