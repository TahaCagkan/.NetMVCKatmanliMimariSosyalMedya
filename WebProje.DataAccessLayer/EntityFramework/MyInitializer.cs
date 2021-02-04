using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using WebProje.Entities;

namespace WebProje.DataAccessLayer.EntityFramework
{
    //Veritabanı Oluştur Eğer Yok ise 
    public class MyInitializer:CreateDatabaseIfNotExists<DatabaseContext>
    {
        //Database oluştuktan sonra Örnek data basımında kullanılan method,sadece Örnek Data basıyoruz
        protected override void Seed(DatabaseContext context)
        {
            //Admin bilgilerini girme
            MyArticlesUser admin = new MyArticlesUser()
            {
                Name = "Taha",
                Surname = "Cantürk",
                Email = "tahacagkann@gmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                Username = "tahacagkann",
                Password ="123456",
                ProfileImageFilename = "user_man.png",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(5),
                ModifiedUsername="tahacagkann"
            };

            //Kullanıcı bilgilerini girme
            MyArticlesUser standardUser = new MyArticlesUser()
            {
                Name = "Çağkan",
                Surname = "Cantürk",
                Email = "cagkann@gmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = false,
                Username = "cagkann",
                Password = "123456",
                ProfileImageFilename = "user_man.png",
                CreatedOn = DateTime.Now.AddHours(1),
                ModifiedOn = DateTime.Now.AddMinutes(65),
                ModifiedUsername = "tahacagkann"
            };
            //Kullanıcıları ekleme
            context.MyArticlesUsers.Add(admin);
            context.MyArticlesUsers.Add(standardUser);

            //FakeDatadan 8 tane kullanıcı oluşturuyoruz
            for (int i = 0; i < 8; i++)
            {
                MyArticlesUser user = new MyArticlesUser()
                {
                    Name = FakeData.NameData.GetFirstName(),
                    Surname = FakeData.NameData.GetSurname(),
                    Email = FakeData.NetworkData.GetEmail(),
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = true,
                    IsAdmin = false,
                    Username = $"user{i}",
                    Password = "123456",
                    ProfileImageFilename = "user_man.png",
                    CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                    ModifiedUsername = $"user{i}"
                };
                context.MyArticlesUsers.Add(user);
            }

            //Veritabanına Kaydetme
            context.SaveChanges();
            //Kullanıcı Listesini Kullanma
            List<MyArticlesUser> userlist = context.MyArticlesUsers.ToList();

            //Kategori Ekleyip oluşturuyoruz
            for (int i = 0; i < 10; i++)
            {
                Category cat = new Category()
                {
                    //Başlık Sokaka Adı
                    Title = FakeData.PlaceData.GetStreetName(),
                    // Açıklamay Adres
                    Description = FakeData.PlaceData.GetAddress(),
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    ModifiedUsername ="tahacagkann"
                };
                context.Categorys.Add(cat);

                //Herbir Kategori için Not Oluşturuyoruz
                for (int k = 0; k < FakeData.NumberData.GetNumber(5,9); k++)
                {
                    MyArticlesUser owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)]; //sahibine random atıyoruz

                    Note note = new Note()
                    {
                        Title = FakeData.TextData.GetAlphabetical(FakeData.NumberData.GetNumber(5,25)),
                        Text = FakeData.TextData.GetSentences(FakeData.NumberData.GetNumber(1, 3)),   
                        IsDraft = false, //Taslak değil
                        LikeCount = FakeData.NumberData.GetNumber(1,9), //Beğeni Sayısı
                        //Owner = (k % 2 == 0) ? admin : standardUser, //çift ise admin değil ise normal kullanıcı
                        Owner = owner,
                        CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                        ModifiedUsername = owner.Username

                    };
                    cat.Notes.Add(note);

                    //Not'a Yorum Ekleme
                    for (int j = 0; j < FakeData.NumberData.GetNumber(3, 5); j++)
                    {
                        MyArticlesUser commet_owner = userlist[FakeData.NumberData.GetNumber(0, userlist.Count - 1)]; //sahibine random atıyoruz,o yorumu sahibi olsun ve değişitirebilsin 
                        Comment commet = new Comment()
                        {
                            Text = FakeData.TextData.GetSentence(),
                            Owner = commet_owner,
                            CreatedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedOn = FakeData.DateTimeData.GetDatetime(DateTime.Now.AddYears(-1), DateTime.Now),
                            ModifiedUsername = commet_owner.Username
                        };

                        note.Comments.Add(commet);
                    }

                    //Like Ekleme işlemi
                    //Liste ile alıyoruz
                    
                    for (int m = 0; m < note.LikeCount; m++)
                    {
                        Liked liked = new Liked()
                        {
                            //oluşturduğumuz liste içerisinde index numarasına göre kullanıcı ver,Like atıyım
                            LikedUser = userlist[m]
                        };
                        /*
                        if (note.Likes == null)
                        {
                            note.Likes = new List<Liked>();
                        }
                        */
                        //Not lara Beğeni Ekleme
                        note.Likes.Add(liked);
                    }
                }
            }
            //Tüm oluşan verileri insert işlemleri yapıyoruz
            context.SaveChanges();
        }
    }
}
