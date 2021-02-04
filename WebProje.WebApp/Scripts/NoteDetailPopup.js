    $(function () {
        $('#modal_notedetail').on('show.bs.modal', function (e) {

            var btn = $(e.relatedTarget); //tıklanan butonu yakaladım
            noteid = btn.data("note-id");//o buton içerisindeki data-noteid aldım

            $("#modal_notedetail_body").load("/Note/GetNoteText/" + noteid);
        })
    });