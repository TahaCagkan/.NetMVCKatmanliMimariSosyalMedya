using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebProje.Entities;

namespace WebProje.DataAccessLayer.EntityFramework
{
    //EntityFramework
    public class DatabaseContext: DbContext
    {
        //Veritabanımıza karşılık gelen DbSetleri tanımlıyoruz
       
        //Kullanıcılar
        public DbSet<MyArticlesUser> MyArticlesUsers { get; set; }
        //Notlar
        public DbSet<Note> Notes { get; set; }
        //Yorumlar
        public DbSet<Comment> Comments { get; set; }
        //Kategoriler
        public DbSet<Category> Categorys { get; set; }
        //Beğeniler
        public DbSet<Liked> Likes { get; set; }

        //MyInitializer set edilmesi gerekmektedir
        public DatabaseContext()
        {
            Database.SetInitializer(new MyInitializer());
        }
        /*
        //FluentAPI
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Note>()
                 .HasMany(n => n.Comments)
                 .WithRequired(c => c.Note)
                 .WillCascadeOnDelete(true);

            modelBuilder.Entity<Note>()
                .HasMany(n => n.Likes)
                .WithRequired(c => c.Note)
                .WillCascadeOnDelete(true);
        }

    */
    }
}
