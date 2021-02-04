using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebProje.Common;
using WebProje.Core.DataAccess;
using WebProje.Entities;

namespace WebProje.DataAccessLayer.EntityFramework
{
    //Repository Pattern ile CRUD işlemlerimizi gerçekleştiriceğiz
    //Generic bir class olarak tanımladık Ör: Herbir class için Comment,Category için tek tek yazmıyıcağız kod tekrarını önlemek için yapıldı,programlarkene Repository<T> içerisindeki <T> tipinde olucak,Ayrıca Where yazmızın nedeni int değer olarak dönmesi değil instance olarak class olucak new lenen bir tip olmak zorunda.
    //class RepositoryBase miras aldık
    public class Repository<T> : RepositoryBase, IDataAccess<T> where T:class
    {
        //DataAccessLayer,içerisindeki DatabaseContext'e db örnekledik
        // private DatabaseContext db;
        private DbSet<T> _objectSet;

        //Repository construct;amacı git hep db.Set<T> bul dediğim için performans kaybını önlemek için yapıldı.
        public Repository()
        {
            //static class CreateContext bu yüzden new yapamayız,static tanımlamamızın amacı DatabaseContext bana vermesini sağlayabilirim,RepositoryBase içerisindeki Context kullanılıcak
           // db =  RepositoryBase.CreateContext();

            _objectSet = db.Set<T>();//bikere O objectSet git Set<T>() çek,her defasında sorgulamaktan kurtarıldı.
        }

        //List Methodu o tablonun tüm kayıtlarını dönücek
        public List<T> List()
        {
           return _objectSet.ToList();
        }
        //Sadece List dönememk için IQueryable yapıldı
        public IQueryable<T> ListQueryable()
        {
            return _objectSet.AsQueryable<T>();
        }

        //Kriterli List,Linq de Function içerisine giren tip T çıka tip bool
        public List<T> List(Expression<Func<T,bool>> where)
        {
            return _objectSet.Where(where).ToList();
        }

        /*
        //Insert işlemi,Generic dönücek
        public int Insert(T obj)
        {
            db.Set<T>().Add(obj);//tipi bilmediğimizden O Set'e ait tipi bul,Add methodu ile nesneyi ekliyoruz.
            return Save();
        }
        */

        //Insert işlemi,Generic dönücek
        public int Insert(T obj)
        {
            _objectSet.Add(obj);
            //obj bir MyEntityBase ise miras almış ise  Category MyEntityBase dir.
            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;
                DateTime nowTime = DateTime.Now; //değişkene atama sebebi ikisininde saati aynı olmalı.

                o.CreatedOn = nowTime;
                o.ModifiedOn = nowTime;
                o.ModifiedUsername = App.Common.GetCurrentUsername(); //App class'ında Common verilen nesnenin GetUserName methodu bize kullanıcı adı dönücektir. //TODO: işlem yapan kullanıcı adı yazılmalıdır.
            }
            return Save();
        }

        //Update işlemi
        public int Update(T obj)
        {
            //obj bir MyEntityBase ise miras almış ise  Category MyEntityBase dir.
            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;

                o.ModifiedOn = DateTime.Now;
                o.ModifiedUsername = App.Common.GetCurrentUsername(); //TODO: işlem yapan kullanıcı adı yazılmalıdır.
            }
            return Save();
        }
        //Delete işlemi
        public int Delete(T obj)
        {

            _objectSet.Remove(obj); //Kaldırma yapıyoruz.
            return Save();
        }

        //CRUD ta kaç tane kayıt olursa onu dön
        public int Save()
        {
            return db.SaveChanges();
        }

        //Tekbir tip dönen Find
        public T Find(Expression<Func<T, bool>> where)
        {
            return _objectSet.FirstOrDefault(where); //bulabilirse nesneyi döner bulamazsa Null döner.
        }
    }
}
