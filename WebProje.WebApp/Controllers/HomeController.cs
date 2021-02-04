using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebProje.Entities.ValueObject;
using WebProje.BusinessLayer;
using WebProje.Entities;
using WebProje.Entities.Messages;
using WebProje.WebApp.ViewModels;
using WebProje.BusinessLayer.Results;
using WebProje.WebApp.Models;
using WebProje.WebApp.Filters;

namespace WebProje.WebApp.Controllers
{
    [Exc]
    public class HomeController : Controller
    {
        private NoteManager noteManager = new NoteManager();
        private CategoryManager categoryManagers = new CategoryManager();
        private MyArticlesUserManager myArticlesUserManager = new MyArticlesUserManager();

        // GET: Home
        public ActionResult Index()
        {
            //Test için oluşturulan işlemler
            /*
            //BusinessLayer Test git
            BusinessLayer.Test test = new BusinessLayer.Test();
            // test.InsertTest();
            //test.UpdateTest();
            test.DeleteTest();
            */

            //CategoryController üzerinden gelen View talebi ve model...
            //Temp Data ile verileri listeleme
            /*
            if(TempData["mm"] != null)
            {
                return View(TempData["mm"] as List<Note>)
            }
            */
            //WebProje.BusinessLayer içerisindeki NoteManager class ulaşıyoruz ordan GetAllNote methodunu çağırıyoruz bu method tüm notları bize Listeleyip getiricektir.

            //Azalana göre Notları listele
            return View(noteManager.ListQueryable().Where(x => x.IsDraft == false).OrderByDescending(x => x.ModifiedOn).ToList());
            //return View(nm.GetAllNoteQueryable().OrderByDescending(x => x.ModifiedOn).ToList());

        }

        #region Kategori Kısmı
        //Kategoriler ve içerisindeki Notları getir.
        public ActionResult ByCategory(int? id)
        {
            //id boş ise başka bu yazı yazdır
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //BusinessLayer içersindeki CategoryMananger class cm ile örnekledik 
            /*
             //Category class bağlan ,cm içerisindeki GetCategoryById methodunu kullanarak değeri getir
             // Category cat = cm.GetCategoryById(id.Value);
             Category cat = categoryManagers.Find(x => x.Id == id.Value);

             //eğer Category bulunamazsa bulunamadı desin
             if (cat == null)
             {
                 return HttpNotFound(); //sayfa bulunamadı
                 //return RedirectToAction("Index", "Home"); anasayfaya git demek
             }
             //List Not dön demek
             //Azalana göre listele
             return View("Index", cat.Notes.OrderByDescending(x =>x.ModifiedOn).ToList());
             */
            List<Note> notes = noteManager.ListQueryable().Where(
                x => x.IsDraft == false && x.CategoryId == id).OrderByDescending(
                x => x.ModifiedOn).ToList();

            return View("Index", notes);
        }
        #endregion

        //Beğeniler Kısmı
        public ActionResult MostLiked()
        {
            return View("Index",noteManager.ListQueryable().OrderByDescending(x => x.LikeCount).ToList());//Index ekranına git ve ordaki Most Like count descending yapıyoruz
        }

        //Hakkında Bölümü
        public ActionResult About()
        {
            return View();
        }

        #region Login işlemi
     
        //Giriş 
        public ActionResult Login()
        {
            return View();
        }

        //Giriş Post
        //Giriş kontrolü ve yönlendirme...
        //Session'a kullanıcı bilgi saklama...
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            //yani model kullanıcı adı ve şifreyi doldurmuş ise herşey tamam ise bana dön
            if(ModelState.IsValid)
            {

               
                //Burda modelimizi veriyoruz BusinessLayerResult içerisindeki MyArticlesUser bilgilerini
                BusinessLayerResult<MyArticlesUser> res = myArticlesUserManager.LoginUser(model);

                //Eğer Errors Count sıfırdan büyük ise hata var demektir.
                if (res.Errors.Count > 0)
                {
                    //Eğer kullanıcı aktif değil durumunu içeriyorsa,dönen Hatlar içerisinde dönen UserNotActive kodu var mı?,Eğer null değilse bu hata,müdahale edilebilir.
                    if (res.Errors.Find(x => x.Code == ErrorMessageCode.UserIsNotActive) != null)
                    {
                        ViewBag.SetLink = "http://Home/Activate/1234-4567-78990";
                    }

                    //herbir ilgili string'i ModelState bas geri döndür,o hatanın mesajını bas
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));

                    return View(model);
                }
                //ilgili kullanıcı bilgilerini Session'a atıyoruz
                CurrentSession.Set<MyArticlesUser>("login",res.Result);
                //yönlendirme...
                return RedirectToAction("Index");
            }

            return View(model);
        }
        #endregion

        #region Profil işlemleri

        //Profil gösterme kısmı
        [Auth]
        public ActionResult ShowProfile()
        {
            //Sessiondaki MyArticlesUser login nesnesini alıyoruz
            // MyArticlesUser currentUser = Session["login"] as MyArticlesUser;
            //MyArticlesUserManager new ledik

            //BusinessLayerResult içerisindeki <MyArticlesUser> res değişkeni = Kullanıcı veren id si bu method
            //Session işlemlerini Model içerisindek CurrentSession daki class'ında bilgileri çekiyoruz.
            BusinessLayerResult<MyArticlesUser> res = myArticlesUserManager.GetUserById(CurrentSession.User.Id);

            //TODO :Eğer Hata var ise Kullanıcı Hata ekranına yönlendiriceğiz
            if(res.Errors.Count > 0)
            {
                //View Modelim
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Hata oluştu",
                    Items = res.Errors
                };

                //TempData["errors"] = res.Errors;
                return View("Error", errorNotifyObj);//hata alırsa bu sayfaya yönlendirme yapıyoruz
            }

            return View(res.Result);
        }

        //Profil Güncelleme Kısmı
        [Auth]
        public ActionResult EditProfile()
        {
            //Sessiondaki MyArticlesUser login nesnesini alıyoruz
            //MyArticlesUser currentUser = Session["login"] as MyArticlesUser;         
            //BusinessLayerResult içerisindeki <MyArticlesUser> res değişkeni = Kullanıcı veren id si bu method
            BusinessLayerResult<MyArticlesUser> res = myArticlesUserManager.GetUserById(CurrentSession.User.Id);

            //TODO :Eğer Hata var ise Kullanıcı Hata ekranına yönlendiriceğiz
            if (res.Errors.Count > 0)
            {
                //View Modelim
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Hata oluştu",
                    Items = res.Errors
                };

                //TempData["errors"] = res.Errors;
                return View("Error", errorNotifyObj);//hata alırsa bu sayfaya yönlendirme yapıyoruz
            }

            return View(res.Result);
        }

        //Profil Güncelleme Kısmı Post'u
        [Auth]
        [HttpPost]
        public ActionResult EditProfile(MyArticlesUser model,HttpPostedFileBase ProfileImage)
        {
            //Fotoğraf yüklemezsek bizden post işlemi bekliyor bu hatayı engellemek için yazıldı.
            ModelState.Remove("ModifiedUsername");

            //Gönderdiğimiz model geçerli mi ?
            if(ModelState.IsValid)
            {
                //ContentType fotoğraf türü için şartımızı yazıyoruz
                if (ProfileImage != null &&
                    (ProfileImage.ContentType == "image/jpeg" ||
                    ProfileImage.ContentType == "image/jpg" ||
                    ProfileImage.ContentType == "image/png"))
                {
                    //null değilse dosya adı oluştuluyor,model id si alınıyor, dosya uzantısını split ile ayırıldı
                    string filename = $"user_{model.Id}.{ProfileImage.ContentType.Split('/')[1]}";
                    //Resim image Klasör altında oluşturduğum dosya adıyla Server.MapPath methodunu kullanarak kaydedildi.
                    ProfileImage.SaveAs(Server.MapPath($"~/images/{filename}"));
                    //model deki ProfileImageFilename bularak şuandaki atadığımız filename eşitleyerek işlemi gerçekleştir.
                    model.ProfileImageFilename = filename;
                }             
                //BusinessLayerResult içerisindeki <MyArticlesUser> res değişkeni = Kullanıcı veren id si bu method
                BusinessLayerResult<MyArticlesUser> res = myArticlesUserManager.UpdateProfile(model);

                //TODO :Eğer Hata var ise Kullanıcı Hata ekranına yönlendiriceğiz
                if (res.Errors.Count > 0)
                {
                    //View Modelim,Hata var ise profilin güncellenemediğini ve Gidiceği Linki
                    ErrorViewModel errorNotifyObj = new ErrorViewModel()
                    {
                        Items = res.Errors,
                        Title = "Profil Güncellenemedi",
                        RedirectingUrl = "/Home/EditProfile"
                    };

                    //TempData["errors"] = res.Errors;
                    return View("Error", errorNotifyObj);//hata alırsa bu sayfaya yönlendirme yapıyoruz
                }

                //Profil güncellendiği için Session güncellendi
                CurrentSession.Set<MyArticlesUser>("login",res.Result);

                return RedirectToAction("ShowProfile");
            }

            return View(model);
        }

        //Profili Silme 
        [Auth]
        public ActionResult DeleteProfile()
        {
            //Sessiondaki kullanıcı al
            //MyArticlesUser currentUser = Session["login"] as MyArticlesUser;

      
            //BusinessLayerResult içerisindeki <MyArticlesUser> üzerinde kullanıcıyı RemoveUserById methodu kullanarak  sil
            BusinessLayerResult<MyArticlesUser> res = myArticlesUserManager.RemoveUserById(CurrentSession.User.Id);

            //TODO :Eğer Hata var ise Kullanıcı Hata ekranına yönlendiriceğiz
            if (res.Errors.Count > 0)
            {
                //View Modelim,Hata var ise profilin güncellenemediğini ve Gidiceği Linki
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Items = res.Errors,
                    Title = "Profil Silinemedi",
                    RedirectingUrl = "/Home/ShowProfile"
                };

                //TempData["errors"] = res.Errors;
                return View("Error", errorNotifyObj);//hata alırsa bu sayfaya yönlendirme yapıyoruz
            }

            //Session Temizle
            Session.Clear();

            return RedirectToAction("Index");
        }

        #endregion

        #region Register

        //Kullanıcı username kontrolü...
        //Kullanıcı e-posta kontrolü...
        //Kayıt işlemi...
        //Aktivasyon E-postası gönderimi.

        public ActionResult Register()
        {
            return View();
        }
        //Kayıt Post işlemi
        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
 

            //DataAnnotations da RegisterViewModel yazdığımız veriler geçerli olup olmadığını ModelState ile kontrol ediyoruz.Geçerli değilse model geri gidicek

            if (ModelState.IsValid)
            {

                //myArticlesUserManager içerisindeki RegisterUser'a model'imi göndericeğim buda bana  BusinessLayerResult<MyArticlesUser> dönüyor.
                BusinessLayerResult<MyArticlesUser> res = myArticlesUserManager.RegisterUser(model);
                
                //Eğer Errors Count sıfırdan büyük ise hata var demektir.
                if(res.Errors.Count > 0)
                {
                    //herbir ilgili string'i ModelState bas geri döndür,o hatanın mesajını bas
                    res.Errors.ForEach(x => ModelState.AddModelError("", x.Message));
                    return View(model);
                }

                #region try catch ile örnek test yapılması
                //Eğer kullanıcı bilgileri null gelirse
                /*
                //Exception yakalamamız için try catch methodu kullanıyoruz
                
                try
                {
                    user= maum.RegisterUser(model); 
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("",ex.Message);//bu hatayı set ettik ModelState
                }
                */
                /*
                //Kullanıcı adı kontrolü var mı yok mu?
                if(model.Username =="aaa")
                {
                    ModelState.AddModelError("","Kullanıcı adı kullanılıyor.");
                }
                
                //Email var mı yok mu 
                if(model.Email == "aaa@aa.com")
                {
                    ModelState.AddModelError("", "E-Posta adresi kullanılıyor.");
                }
                */
                //ModelState her item dönüceğiz hata işlemleri için;item için Values içerisindeki Count'u 0 dan büyükse o itemda hata var demektir.
                /*
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Count > 0)
                    {
                        return View(model);
                    }
                }
               
                //Eğer Kullanıcı bilgilerinde hata varsa try catch den dönen işlemleri
                if (user == null)
                {
                    return View(model);
                }
                 */
                #endregion

                //OkViewModel View de tanımladığımız işlemleri yap
                OkViewModel notifyObj = new OkViewModel()
                {
                    Title ="Kayıt Başarılı", //Title
                    RedirectingUrl = "/Home/Login" //gidiceği Url
                };

                notifyObj.Items.Add("  Lütfen e-posta adresinize gönderidiğimiz aktivasyon link'ine tıklayarak hesabınızı aktive ediniz.Hesabınızı aktive etmeden not ekleyemez ve beğenme yapamazsınız.");

                //eğer hasError true ise adamı aynı sayfaya(Register) geri gönder, değilse Ok View gönder,notifyObj yanında göster
                return View("Ok",notifyObj);
            }
            return View(model);
        }

      
        #endregion

        #region User Aktivasyon
        //Kullanıcı Aktivasyon işlemleri
        public ActionResult UserActivate(Guid id)
        {
            //Kullanıcı Aktivasyonu sağlanacak
            
            //MyArticlesUserManager gelen ActivateUser içerisiene gelen activate_id aktif ediyoruz.
            BusinessLayerResult<MyArticlesUser> res = myArticlesUserManager.ActivateUser(id);

            //Kullanıcı aktifleştirme işlemlerinde hata dönerse
            if(res.Errors.Count > 0)
            {
                //View Modelim
                ErrorViewModel errorNotifyObj = new ErrorViewModel()
                {
                    Title = "Geçersiz işlem",
                    Items = res.Errors
                };

                //TempData["errors"] = res.Errors;
                return View("Error", errorNotifyObj);//hata alırsa bu sayfaya yönlendirme yapıyoruz.
            }
            //işlem başarılı ise
            OkViewModel okNotifyObj = new OkViewModel()
            {
                Title = "Hesap Aktifleştirildi", //Title
                RedirectingUrl ="Home/Login" //gidiceği Url
            };
            okNotifyObj.Items.Add("Hesabınız Aktifleştirildi.Artık not paylaşabilir ve beğenme yapabilirsiniz.");

            return RedirectToAction("Ok",okNotifyObj);
        }

        //ViewModel tanımlandığı için gerek kalmadı
        //Kullanıcı aktifleştirip UserActivateOk sayfasına yönlendiriyoruz
        /*
        public ActionResult UserActivateOk()
        {
            return View();
        }

        //Kullanıcı aktifleştirme iptali
        public ActionResult UserActivateCancel()
        {
            //ErrorMessageObj içerisinde Liste var
            List<ErrorMessageObj> errors = null;

            //TempData kontrol ediyoruz,TempData bana anahtar değer gelmiş ise
            if (TempData["errors"] != null)
            {
                //ErrorMessageObj içerisinde Liste var daha sonra tip dönüşümü yapıyoruz.
                 errors = TempData["errors"] as List<ErrorMessageObj>;
            }

            return View(errors);
        }
        */
        #endregion

        //Çıkış
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        //Admin Yetkisi olmayan giremez
        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult HassError()
        {
            return View();
        }
    }
}