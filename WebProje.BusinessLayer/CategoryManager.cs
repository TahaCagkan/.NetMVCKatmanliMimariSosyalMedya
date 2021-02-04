using WebProje.BusinessLayer.Abstract;
using WebProje.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebProje.BusinessLayer
{
    //CategoryController ,ManagerBase den türer tipide Category dir. ManagerBase içersindeki  private Repository oluşucak tüm tiplerde Category dönüşücek
    public class CategoryManager : ManagerBase<Category>
    {
        #region ManagerBase geldiği için iptal edildi
        //DataAccessLayer.EntityFramework içerisindeki Repository içerisindeki Category class'ına erişiyoruz
        // private Repository<Category> repo_category = new Repository<Category>();
        /*
         //Geriye Kategori Listesi dönendir.
         public List<Category> GetCategories()
         {
             //Bütün Kategorileri Listelyip Döndürücek
             return repo_category.List();
         }

         //Category id si dönen method ihtiyacından dolayı yazıldı

         public Category GetCategoryById(int id)
         {
             return repo_category.Find(x => x.Id == id); //id si eşit olanı bul varsa dön
         }
         */
        #endregion

        public override int Delete(Category category)
        {
            //Kategori ile ilişkişi notların silinmesi gerekir.
            NoteManager noteManager = new NoteManager();
            LikedManager likedManager = new LikedManager();
            CommentManager commentManager = new CommentManager();
            
            //En sonda Kategoriyi siliyoruz.
            //Listeyi ayarlamak için ToList diyoruz.

            foreach (Note note in category.Notes.ToList())
            {
                //O notlarla alakalı like silmemiz gerekir.
                foreach (Liked like in note.Likes.ToList())
                {
                    likedManager.Delete(like);
                }

                //O notlarla alakalı yorumların silmemiz gerekir.
                foreach (Comment comment in note.Comments.ToList())
                {
                    commentManager.Delete(comment);
                }

                //notların kendisi siliniyor.
                noteManager.Delete(note);
            }

            //en sonda Kategori siliniyor.
            return base.Delete(category);    
        }

    }
}
