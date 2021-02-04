using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebProje.Entities.Messages;

namespace WebProje.BusinessLayer.Results
{
    //generic olucak,sebebi birden çok uyarı verebilmek için erişebilmek
    public class BusinessLayerResult<T> where T :class
    {
        //Hata mesajlarını burada Liste; enum ve string ile dönüceğim,Bu listede ErrorMessageObj den oluşur KeyValuePair ile yapmamak için oluşturulmuştur.
        public List<ErrorMessageObj> Errors { get; set; }
        //Başarılı olursada bu property de dönüceğim
        public T Result { get; set; }

        //eğer liste ilkten oluşturmazsak Patlar nedeni MyArticlesUserManager içerisine List gönderiyoruz
        public BusinessLayerResult()
        {
            //ErrorMessageObj den oluşur KeyValuePair ile yapmamak için oluşturulmuştur.
            Errors = new List<ErrorMessageObj>();     
        }

        //ErrorMessageCode içerisindeki kodları ve mesajlara ulaşacağız
        public void AddError(ErrorMessageCode code,string message)
        {
            //ErrorMessageObj den oluşur KeyValuePair ile yapmamak için oluşturulmuştur.İçerisine implement(Uygulanmıştır) edilmiştir.
            Errors.Add(new ErrorMessageObj() { Code = code, Message = message });
        }
    }
}
