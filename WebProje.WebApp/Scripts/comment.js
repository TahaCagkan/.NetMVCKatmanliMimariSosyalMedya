//Yoruma tıklandığı zaman modal her çalıştırıldığında içerisine Comment controller içerisindeki ShowNoteComments nesnesinin içerisindeki _PartialComments döndürücek oda bize içerisindeki yorumla alakalı bilgileri döndürücek



var notid = -1;
var modalCommentBodyId = "#modal_comment_body";


$(function () {
    $('#modal_comment').on('show.bs.modal', function (e) {

        var btn = $(e.relatedTarget); //tıklanan butonu yakaladım
        noteid = btn.data("note-id");//o buton içerisindeki data-noteid aldım

        $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
    })
});
//ne olduğu button,click(e) = işlem => edit_clicked işlemi,comment id ,sonrada span id si
function doComment(btn, e, commentid, spanid) {
    //işlem edit ise butonu kırmızıdan yeşile çevir ,contenteditable true olucak

    //jquery ile yapma btn gösterdik
    var button = $(btn);
    //mode true false işlemini yapmamız için atandı,click olayı doğrumu diye
    var mode = button.data("edit-mode");

    if (e === "edit_clicked") {
        if (!mode) {
            //Düzenle butonun data nın edit-mode true yapıldı,tıklanınca
            button.data("edit-mode", true);
            button.removeClass("btn-warning"); //warning kaldır
            button.addClass("btn-success"); //yerine success koy
            var btnSpan = button.find("span"); //span tag'ını bul
            btnSpan.removeClass("fa-edit"); //edit icon kaldır
            btnSpan.addClass("fa-check"); //yerine ok icon koy

            $(spanid).addClass("editable");
            //mode edit ise bunu yap
            $(spanid).attr("contenteditable", true);
            $(spanid).focus();
        }
        //tekrar basılmış ise editlemeyi kapat
        else {
            //Düzenle butonun data nın edit-mode true yapıldı,tıklanınca
            button.data("edit-mode", false);
            button.addClass("btn-warning"); //warning ekle
            button.removeClass("btn-success"); //yerine success kaldır
            var btnSpan = button.find("span"); //span tag'ını bul
            btnSpan.addClass("fa-edit"); //edit icon ekle
            btnSpan.removeClass("fa-check"); // ok icon kaldır

            $(spanid).removeClass("editable");
            //mode edit ise bunu yap
            $(spanid).attr("contenteditable", false);

            //sayfadaki ilgili span içerisindeki text al
            $(spanid).attr("contenteditable", false);

            var txt = $(spanid).text();
            //daha sonra ajax ile POST ettir
            $.ajax({
                method: "POST",
                url: "/Comment/Edit/" + commentid,
                data: { text: txt } //text isimli veriyi gönderiyorum
            }).done(function (data) {
                // true ise
                if (data.result) {
                    // yorumlar partial tekrar yüklenir..
                    //modal'git CommentBody içerisine /Comment/ShowNoteComments/ bu URL den Comment == noteid eşit olanları çek getir.
                    $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
                }
                else {
                    alert("Yorum güncellenemedi.");
                }
                //başarısız olması durumda
            }).fail(function () {
                alert("Sunucu ile bağlantı kurulamadı.");
            });
        }
        //aksi halde silme işlemi yapılcak
    } else if (e === "delete_clicked") {
        var dialog_res = confirm("Yorum silinsin mi?");
        if (!dialog_res) return false;
        //silme işleminde GET
        $.ajax({
            method: "GET",
            url: "/Comment/Delete/" + commentid
        }).done(function (data) {
            //Başarılı ise
            if (data.result) {
                //yorumlar partial yeniden yüklenir.
                //modal'git CommentBody içerisine /Comment/ShowNoteComments/ bu URL den Comment == noteid eşit olanları çek getir.
                $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
            } else {
                alert("Yorum silinemedi.");
            }

        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı.");
        });

    } else if (e === "new_clicked") {
        var txt = $("#new_comment_text").val();
        $.ajax({
            method: "POST",
            url: "/Comment/Create/",
            data: { "text": txt, "noteid": noteid }
        }).done(function (data) {
            //Başarılı ise
            if (data.result) {
                //yorumlar partial yeniden yüklenir.
                //modal'git CommentBody içerisine /Comment/ShowNoteComments/ bu URL den Comment == noteid eşit olanları çek getir.
                $(modalCommentBodyId).load("/Comment/ShowNoteComments/" + noteid);
            } else {
                alert("Yorum eklenemedi.");
            }

        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı.");
        });
    }
}