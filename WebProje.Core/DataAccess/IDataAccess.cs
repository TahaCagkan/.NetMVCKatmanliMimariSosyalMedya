using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WebProje.Core.DataAccess
{
   public interface IDataAccess<T>
    {
        //List Methodu o tablonun tüm kayıtlarını dönücek
         List<T> List();
         IQueryable<T> ListQueryable();

    //Kriterli List,Linq de Function içerisine giren tip T çıka tip bool
     List<T> List(Expression<Func<T, bool>> where);


        /*
        //Insert işlemi,Generic dönücek
        public int Insert(T obj)
        {
            db.Set<T>().Add(obj);//tipi bilmediğimizden O Set'e ait tipi bul,Add methodu ile nesneyi ekliyoruz.
            return Save();
        }
        */

    //Insert işlemi,Generic dönücek
     int Insert(T obj);


        //Update işlemi
     int Update(T obj);

        //Delete işlemi
     int Delete(T obj);


        //CRUD ta kaç tane kayıt olursa onu dön
     int Save();


    //Tekbir tip dönen Find
     T Find(Expression<Func<T, bool>> where);
      
    }
}
