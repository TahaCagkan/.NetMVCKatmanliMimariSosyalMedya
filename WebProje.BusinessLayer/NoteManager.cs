using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebProje.BusinessLayer.Abstract;
using WebProje.DataAccessLayer.EntityFramework;
using WebProje.Entities;

namespace WebProje.BusinessLayer
{
   public class NoteManager :ManagerBase<Note>
    {
        //DataAccessLayer.EntityFramework içerisindeki Repository içerisindeki Note class'ına erişiyoruz
        //private Repository<Note> repo_note = new Repository<Note>();
        /*
        //Notları alıp listedik
        public List<Note> GetAllNote()
        {
            return repo_note.List();
        }

        public IQueryable<Note> GetAllNoteQueryable()
        {
            return repo_note.ListQueryable();
        }
        */


        public override int Delete(Note notes)
        {
            //Note ile ilişkili olanların silinmesi gerekir.
          
            LikedManager likedManager = new LikedManager();
            CommentManager commentManager = new CommentManager();

            //En sonda Kategoriyi siliyoruz.
            //Listeyi ayarlamak için ToList diyoruz.

           
                //O notlarla alakalı like silmemiz gerekir.
                foreach (Liked like in notes.Likes.ToList())
                {
                    likedManager.Delete(like);
                }

                //O notlarla alakalı yorumların silmemiz gerekir.
                foreach (Comment comment in notes.Comments.ToList())
                {
                    commentManager.Delete(comment);
                }
            

            //en sonda notların siliniyor.
            return base.Delete(notes);
        }
    }
}
