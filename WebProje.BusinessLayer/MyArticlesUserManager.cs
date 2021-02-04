using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebProje.BusinessLayer.Abstract;
using WebProje.BusinessLayer.Results;
using WebProje.Common.Helpers;
using WebProje.DataAccessLayer.EntityFramework;
using WebProje.Entities;
using WebProje.Entities.Messages;
using WebProje.Entities.ValueObject;

namespace WebProje.BusinessLayer
{
    public class MyArticlesUserManager :ManagerBase<MyArticlesUser>
    {
        //veritabanı işlemleri için Repository kullanıyoruz
        //private Repository<MyArticlesUser> repo_user = new Repository<MyArticlesUser>();
        //Kullanıcı Silme işleminde ,ilişkisel tablo için Note,Like,Comment için silerken sorun oluştu bu yüzden  entitylere ulaşıp içerisindeki verileri silmemiz gerekmektedir.
        private Repository<Note> repo_note = new Repository<Note>();
        private Repository<Liked> repo_liked = new Repository<Liked>();
        private Repository<Comment> repo_comment = new Repository<Comment>();

        #region Register
        //Kayıt olunca kullanıcı BusinessLayerResult içersinden MyArticlesUser erişerek kişisel bilgiler gelicek ve RegisterViewModel içerisinden Register bilgileri gelicek
        public BusinessLayerResult<MyArticlesUser> RegisterUser(RegisterViewModel data)
        {

            //Kullanıcı  kontrolü...
            //Kayıt işlemi...
            //Aktivasyon E-postası gönderimi.

            //Kullanıcı username ve e-posta kontrolü,Gelen data içersinden var mı diye kontrolümüzü sağlıyoruz.
            //Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));
            MyArticlesUser user = Find(x => x.Username == data.Username || x.Email == data.Email);

            //BusinessLayerResult Erişerek MyArticlesUser tipinde oluşturuyoruz,birden çok uyarı verebileceğimiz yapı kurduk,burda aşşaüıdaki Username ve Email eşleşmiş ise aynı kullanıcı var ise şayet hata mesajı dönüceğimiz tip olan  BusinessLayerResult<MyArticlesUser> bu tipin içerisine Error Listesine string olarak eklenir.
            BusinessLayerResult<MyArticlesUser> layerResult = new BusinessLayerResult<MyArticlesUser>();

            //Eğer kullanıcı eşleşme var ise
            if (user != null)
            {
                //Username mi eşleşmiş bakılır
                if (user.Username == data.Username)
                {
                    //Entities içerisindeki ErrorMessageCode gidilir daha sonra burdaki kod okunur hata varsa mesaj dönülür.
                    layerResult.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }
                //Email mi eşleşmiş bakılır
                if (user.Email == data.Email)
                {
                    layerResult.AddError(ErrorMessageCode.EmailAlreadyExists, "E-Posta adresi kayıtlı");
                }
            }
            //Eğer eşleşen kullanıcı yoksa INSERT işlemini yapabilecek.
            else
            {
                int dbResult = base.Insert(new MyArticlesUser()
                {
                    Username = data.Username,
                    Email = data.Email,
                    ProfileImageFilename ="user_man.png",
                    Password = data.Password,
                    ActivateGuid = Guid.NewGuid(),
                    IsActive = false,
                    IsAdmin = false

                });
                //Kullanıcı INSERT olduysa,bulduğun kullanıcıyı bana ver bunuda layerResult.Result nesnesine aktar
                if (dbResult > 0)
                {
                    //data dan gelenlerle eşitleyip bul,ilgili User nesnesini çek
                    layerResult.Result = Find(x => x.Email == data.Email && x.Username == data.Username);

                    //layerResult.Result.ActivateGuid
                    //MailHelper class'ında işlemlerimizi yap
                    //sitenin adresini SiteRootUri bu siteUri değişkenimize aldık
                    string siteUri = ConfigHelper.Get<string>("SiteRootUri");
                    //link Guid işlemi
                    string activateUri = $"{siteUri}/Home/UserActivate/{layerResult.Result.ActivateGuid}";
                    //Mail aktifleştirmek için kullanıcı Kullanıcı Adı,farklı bir sayfada açılması
                    string body = ($"Merhaba {layerResult.Result.Username};<br/><br/> Hesabınızı aktifleştirmek için <a href='{activateUri}' target='_blank'>tıklayınız</a>");

                    MailHelper.SendMail(body, layerResult.Result.Email, "MyArticles Hesap Aktifleştirme");
                }

                //TODO:aktivasyon mail'ı atılacak kısım...

            }

            return layerResult;
        }
        #endregion


        #region Login
        //Giriş kontrolü
        //Hesap Aktive edilmiş mi?
        public BusinessLayerResult<MyArticlesUser> LoginUser(LoginViewModel data)
        {

            //BusinessLayerResult Erişerek MyArticlesUser tipinde oluşturuyoruz,birden çok uyarı verebileceğimiz yapı kurduk,burda aşşağıdaki Username ve Password eşleşmiş ise aynı kullanıcı var ise şayet hata mesajı dönüceğimiz tip olan  BusinessLayerResult<MyArticlesUser> bu tipin içerisine Error Listesine string olarak eklenir.
            BusinessLayerResult<MyArticlesUser> layerResult = new BusinessLayerResult<MyArticlesUser>();

            //Kullanıcı çek,kullanıcı adı ve şifresi doğru mu,Kullanıcı eşleşsede eşleşmesede SET edildi,eşleşmemiş ise zaten null gelicek
            layerResult.Result = Find(x => x.Username == data.Username && x.Password == data.Password);

            //Kullanıcı var ise
            if (layerResult.Result != null)
            {
                //burda Kullanıcı aktif olup olmadığını kontrol diyoruz
                if (!layerResult.Result.IsActive)
                {
                    layerResult.AddError(ErrorMessageCode.UserIsNotActive, "Kullanıcı aktifleştirilmemiştir.");
                    layerResult.AddError(ErrorMessageCode.CheckYourEmail, "Lütfen E-Posta adresinizi kontrol ediniz.");
                }

            }
            else
            {
                layerResult.AddError(ErrorMessageCode.UsernameOrPassWrong, "Kullanıcı adı veya şifre uyuşmuyor.");
            }
            return layerResult;

        }



        #endregion

        #region Profil işlemleri

        public BusinessLayerResult<MyArticlesUser> GetUserById(int id)
        {
            //Geriye dönücek tipimi MyArticlesUser nesnesi içeren BusinessLayerResult oluşturuldu.
            BusinessLayerResult<MyArticlesUser> res = new BusinessLayerResult<MyArticlesUser>();
            //id si eşit olan datayı bul
            res.Result = Find(x => x.Id == id);

            //Eğer kullanıcı boş ise hata dönücek
            if(res.Result == null)
            {
                res.AddError(ErrorMessageCode.UserNotFound, "Kullanıcı bulunamadı.");
            }
            return res;
        }

        #endregion

        #region Kullanıcı Bilgilerini Güncelleme
        public BusinessLayerResult<MyArticlesUser> UpdateProfile(MyArticlesUser data)
        {
            //Kullanıcı username ve e-posta kontrolü,Gelen data içersinden var mı diye kontrolümüzü sağlıyoruz.
            //Id ye göre kullanıcı adı ve Email adresi hangi kullanıcıda var
            MyArticlesUser db_user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));

            //BusinessLayerResult Erişerek MyArticlesUser tipinde oluşturuyoruz,birden çok uyarı verebileceğimiz yapı kurduk,burda aşşaüıdaki Username ve Email eşleşmiş ise aynı kullanıcı var ise şayet hata mesajı dönüceğimiz tip olan  BusinessLayerResult<MyArticlesUser> bu tipin içerisine Error Listesine string olarak eklenir.
            BusinessLayerResult<MyArticlesUser> layerResult = new BusinessLayerResult<MyArticlesUser>();

            //Eğer kullanıcı eşleşme var ise ve Update işlemini yapan kişili id ve Kullancı adı olan kişi doğru olan kişi mi ?
            if (db_user != null && db_user.Id != data.Id)
            {
                //Username mi eşleşmiş bakılır
                if (db_user.Username == data.Username)
                {
                    //Entities içerisindeki ErrorMessageCode gidilir daha sonra burdaki kod okunur hata varsa mesaj dönülür.
                    layerResult.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }
                //Email mi eşleşmiş bakılır
                if (db_user.Email == data.Email)
                {
                    layerResult.AddError(ErrorMessageCode.EmailAlreadyExists, "E-Posta adresi kayıtlı");
                }
                return layerResult;
            }

            //Aksi halde bir hata yoksa modelden gelen data nesnesi ile set ediliyor Güncelleniyor
            layerResult.Result = Find(x => x.Id == data.Id);
            layerResult.Result.Email = data.Email;
            layerResult.Result.Name = data.Name;
            layerResult.Result.Surname = data.Surname;
            layerResult.Result.Password = data.Password;
            layerResult.Result.Username = data.Username;

            //kullanıcı adı Güncellem var ise Resim uzantısının adını güncelle 
            if (string.IsNullOrEmpty(data.ProfileImageFilename) == false)
            {
                layerResult.Result.ProfileImageFilename = data.ProfileImageFilename;
            }
            
            //Hata yoksa profili güncelle var ise güncellemedi mesajını göster
            if(base.Update(layerResult.Result) == 0)
            {
                layerResult.AddError(ErrorMessageCode.ProfileCouldNotUpdated,"Profil güncellenemedi");
            }

            return layerResult;
        }
        #endregion

        #region Kullanıcıyı Silme işlemi
        public BusinessLayerResult<MyArticlesUser> RemoveUserById(int id)
        {
            //BusinessLayerResult Erişerek MyArticlesUser tipinde oluşturuyoruz,birden çok uyarı verebileceğimiz yapı kurduk,burda aşşağıdaki Username ve Password eşleşmiş ise aynı kullanıcı var ise şayet hata mesajı dönüceğimiz tip olan  BusinessLayerResult<MyArticlesUser> bu tipin içerisine Error Listesine string olarak eklenir.
            BusinessLayerResult<MyArticlesUser> layerResult = new BusinessLayerResult<MyArticlesUser>();

            //Repository<MyArticlesUser> daki örnkelediğimiz repo_user dan silincek olan id li olan kullanıcıyı buluyoruz
            MyArticlesUser user = Find(x => x.Id == id);

            //Not sahibi id liyi bulup içerisindeki tüm notları Listele
            List<Note> notes = repo_note.List(x => x.Owner.Id == id);
            //Yorum sahibi id liyi bulup içerisindeki tüm yorumları Listele
            List<Comment> comments = repo_comment.List(x => x.Owner.Id == id);
            //Beğeni sahibi id liyi bulup içerisindeki tüm Beğenileri Listele
            List<Liked> likeds = repo_liked.List(x => x.LikedUser.Id == id);

            //Tüm yorumları item ile dön ve sil
            foreach (var item in comments)
            {
                repo_comment.Delete(item);
            }

            //Tüm beğenileri item ile dön ve sil
            foreach (var item in likeds)
            {

                repo_liked.Delete(item);
            }

            //Tüm notları item ile dön ve sil
            foreach (var item in notes)
            {
                //Tüm yorum yapılan notların id sini Listele
                List<Comment> commentss = repo_comment.List(x => x.Note.Id == item.Id);
                //Tüm beğeni yapılan notların id sini Listele
                List<Liked> likedss = repo_liked.List(x => x.Note.Id == item.Id);

                //Tüm yorum yapılan notların id sini Listeledik, daha sonra notların içerisindeki yorumları item ile dön ve sil
                foreach (var itemm in commentss)
                {
                    repo_comment.Delete(itemm);
                }

                //Tüm beğeni yapılan notların id sini Listeledik, daha sonra notların içerisindeki beğeni item ile dön ve sil
                foreach (var itemm in likedss)
                {
                    repo_liked.Delete(itemm);
                }
                //tüm notları sil
                repo_note.Delete(item);

            }

            //Kullanıcı var ise
            if (user != null)
            {
                //Kullanıcı Repository içerisindeki Delete methodu ile kullanıcı gönderiyorum siliyorum,Silme işleminde sorun olmuş ise gönderiyorum nesneimi hata mı basıyorum
                if (Delete(user) == 0)
                {
                    layerResult.AddError(ErrorMessageCode.UserCouldNotRemove, "Kullanıcı silinemedi.");
                    return layerResult;
                }
            }
            else
            //Eğer kullanıcı yok ise Kullanıcı bulunamadı diyoruz 
            {
                layerResult.AddError(ErrorMessageCode.UserCouldNotFind, "Kullanıcı Bulunamadı.");
            }

            return layerResult;
        }
        #endregion

        #region Kullanıcı Aktifleştirme 
        //BusinessLayerResult içerisinde MyArticlesUser dönüceğiz geriye
        public BusinessLayerResult<MyArticlesUser> ActivateUser(Guid activateId)
        {

            //BusinessLayerResult Erişerek MyArticlesUser tipinde oluşturuyoruz,birden çok uyarı verebileceğimiz yapı kurduk,burda aşşağıdaki Username ve Password eşleşmiş ise aynı kullanıcı var ise şayet hata mesajı dönüceğimiz tip olan  BusinessLayerResult<MyArticlesUser> bu tipin içerisine Error Listesine string olarak eklenir.
            BusinessLayerResult<MyArticlesUser> layerResult = new BusinessLayerResult<MyArticlesUser>();

            //Bu ActivateId ye göre kullanıcımız var mı onu kontrolünü sağlıyoruz
            layerResult.Result = Find(x => x.ActivateGuid == activateId);

            //Kullanıcı var ise
            if (layerResult.Result != null)
            {
                //Kullanıcı aktif edildi mi ?
                if (layerResult.Result.IsActive)
                {
                    layerResult.AddError(ErrorMessageCode.UserAlreadyActive, "Kullanıcı zaten aktif edilmiştir.");
                    return layerResult;
                }
                //Aktif değil ise aktifleştirmemiz gerektiği için ihtiyaç duyuldu

                layerResult.Result.IsActive = true;
                Update(layerResult.Result);

            }
            else
            //Eğer kullanıcı yok ise deniyorsa zorlayarak 
            {
                layerResult.AddError(ErrorMessageCode.ActivateIdDoesNotExists, "Aktifleştirilecek Kullanıcı Bulunamadı.");
            }

            return layerResult;

        }


        #endregion

        #region Update işlemici için Insert

        //İstediğim şekilde update işlerinde Insert yapmamız gerektiği için eklendi
        //new edilerek hiding gizlendi önceki miras değilde bu Insert çalış diye
        public new BusinessLayerResult<MyArticlesUser> Insert(MyArticlesUser data)
        {


            //Kullanıcı  kontrolü...
            //Kayıt işlemi...
            //Aktivasyon E-postası gönderimi.

            //Kullanıcı username ve e-posta kontrolü,Gelen data içersinden var mı diye kontrolümüzü sağlıyoruz.
            //Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));
            MyArticlesUser user = Find(x => x.Username == data.Username || x.Email == data.Email);

            //BusinessLayerResult Erişerek MyArticlesUser tipinde oluşturuyoruz,birden çok uyarı verebileceğimiz yapı kurduk,burda aşşaüıdaki Username ve Email eşleşmiş ise aynı kullanıcı var ise şayet hata mesajı dönüceğimiz tip olan  BusinessLayerResult<MyArticlesUser> bu tipin içerisine Error Listesine string olarak eklenir.
            BusinessLayerResult<MyArticlesUser> layerResult = new BusinessLayerResult<MyArticlesUser>();

            //Gelen nesne verildi
            layerResult.Result = data;

            //Eğer kullanıcı eşleşme var ise
            if (user != null)
            {
                //Username mi eşleşmiş bakılır
                if (user.Username == data.Username)
                {
                    //Entities içerisindeki ErrorMessageCode gidilir daha sonra burdaki kod okunur hata varsa mesaj dönülür.
                    layerResult.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }
                //Email mi eşleşmiş bakılır
                if (user.Email == data.Email)
                {
                    layerResult.AddError(ErrorMessageCode.EmailAlreadyExists, "E-Posta adresi kayıtlı");
                }
            }
            //Eğer eşleşen kullanıcı yoksa INSERT işlemini yapabilecek.
            else
            {
                layerResult.Result.ProfileImageFilename = "user_man.png";
                layerResult.Result.ActivateGuid = Guid.NewGuid();

                //base deki Insert çağrılcak.
                if(base.Insert(layerResult.Result) == 0)
                {
                    layerResult.AddError(ErrorMessageCode.UserCouldNotInserted, "Kullanıcı Eklenemedi.");
                }

            }

            return layerResult;
        }

        #endregion

        #region Edit için Update

        public new BusinessLayerResult<MyArticlesUser> Update(MyArticlesUser data)
        {
            //Kullanıcı username ve e-posta kontrolü,Gelen data içersinden var mı diye kontrolümüzü sağlıyoruz.
            //Id ye göre kullanıcı adı ve Email adresi hangi kullanıcıda var
            MyArticlesUser db_user = Find(x => x.Id != data.Id && (x.Username == data.Username || x.Email == data.Email));

            //BusinessLayerResult Erişerek MyArticlesUser tipinde oluşturuyoruz,birden çok uyarı verebileceğimiz yapı kurduk,burda aşşaüıdaki Username ve Email eşleşmiş ise aynı kullanıcı var ise şayet hata mesajı dönüceğimiz tip olan  BusinessLayerResult<MyArticlesUser> bu tipin içerisine Error Listesine string olarak eklenir.
            BusinessLayerResult<MyArticlesUser> layerResult = new BusinessLayerResult<MyArticlesUser>();

            layerResult.Result = data;

            //Eğer kullanıcı eşleşme var ise ve Update işlemini yapan kişili id ve Kullancı adı olan kişi doğru olan kişi mi ?
            if (db_user != null && db_user.Id != data.Id)
            {
                //Username mi eşleşmiş bakılır
                if (db_user.Username == data.Username)
                {
                    //Entities içerisindeki ErrorMessageCode gidilir daha sonra burdaki kod okunur hata varsa mesaj dönülür.
                    layerResult.AddError(ErrorMessageCode.UsernameAlreadyExists, "Kullanıcı adı kayıtlı.");
                }
                //Email mi eşleşmiş bakılır
                if (db_user.Email == data.Email)
                {
                    layerResult.AddError(ErrorMessageCode.EmailAlreadyExists, "E-Posta adresi kayıtlı");
                }
                return layerResult;
            }

            //Aksi halde bir hata yoksa modelden gelen data nesnesi ile set ediliyor Güncelleniyor
            layerResult.Result = Find(x => x.Id == data.Id);
            layerResult.Result.Email = data.Email;
            layerResult.Result.Name = data.Name;
            layerResult.Result.Surname = data.Surname;
            layerResult.Result.Password = data.Password;
            layerResult.Result.Username = data.Username;
            layerResult.Result.IsActive = data.IsActive;
            layerResult.Result.IsAdmin = data.IsAdmin;

       

            //Hata yoksa profili güncelle var ise güncellemedi mesajını göster
            if (base.Update(layerResult.Result) == 0)
            {
                layerResult.AddError(ErrorMessageCode.UserCouldNotUpdated, "Kullanıcı güncellenemedi");
            }

            return layerResult;
        }

        #endregion

    }
}
