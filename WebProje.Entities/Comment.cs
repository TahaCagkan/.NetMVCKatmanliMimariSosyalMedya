using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProje.Entities
{
    //MyEntityBase türettik,tablo ismi Comments
    [Table("Comments")]
    public class Comment : MyEntityBase
    {
        //Yapılan yorum içerisindeki Metin,Zorunlu,maks 300 karakter
        [Required,StringLength(300)]
        public string Text { get; set; }
        //Yorum Onaylama
        //public bool IsApproval { get; set; }

        //Hangi Not'a yorum yazıldı
        public virtual Note Note { get; set; }
        //Hangi kullanıcı bu yorumu yaptı
        public virtual MyArticlesUser Owner { get; set; }

    }
}

