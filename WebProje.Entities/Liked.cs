using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebProje.Entities
{
    //Tablo Adı Likes
    [Table("Likes")]
    public class Liked
    {
        //Id,Key ve Otomatik artan yaptık
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //Notun Kendisi
        public virtual Note Note { get; set; }
        //Bu No virtual tu Beğenen Kullanıcı
        public virtual MyArticlesUser LikedUser { get; set; }
    }
}
