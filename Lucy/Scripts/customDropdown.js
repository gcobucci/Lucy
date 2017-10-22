$(document).ready(function () {
    $(".dropdown1, .dropdown2").hover(
        function () {
            $('.dropdown-menuS', this).stop(true, true).slideDown("fast");
            $(this).toggleClass('open');
        },
        function () {
            $('.dropdown-menuS', this).stop(true, true).slideUp("fast");
            $(this).toggleClass('open');
        }
    );
});

function getUsuPos() {
    usuHalf = (document.getElementById("ulUsu").clientWidth) / 2;
    espacio = document.body.clientWidth - (document.getElementById("liUsu").offsetLeft);

    if (document.body.clientWidth > 401) {
        document.getElementById("ulUsu").style.left = "unset";
        document.getElementById("ulUsu").style.width = "240px";
        if (usuHalf > (espacio - 5)) {
            usuPos = usuHalf - (espacio - 5);
            document.getElementById("ulUsu").style.marginLeft = "-" + (usuHalf + usuPos) + "px";
        } else {
            document.getElementById("ulUsu").style.marginLeft = "-81.5px";
        }
    } else {
        document.getElementById("ulUsu").style.marginLeft = "0";
        document.getElementById("ulUsu").style.left = "0";
        document.getElementById("ulUsu").style.width = "100%";
    }

    $.ajax({
        dataType: 'json',
        type: 'POST',
        url: "/usuarios/lastlogin"
    }); 
}

function responder(ComId, UsuNom) {
    $('#ComentarioPadreId').val(ComId);
    $("#ComentarioCuerpo").focus();
    document.getElementById("btn-CancelarRespuesta").style.display = "inline-block";
}

function cancelarRespuesta() {
    $('#ComentarioPadreId').val(null);
    document.getElementById("btn-CancelarRespuesta").style.display = "none";
}

function btnComentar(idContenido) {
    var token = $('[name=__RequestVerificationToken]').val();
    var obj = {
        ComentarioCuerpo: $('#ComentarioCuerpo').val(),
        ComentarioPadreId: $('#ComentarioPadreId').val(),
        ContenidoId: $('#ContenidoId').val()
    };
    $.ajax({
        type: 'POST',
        data: {
            __RequestVerificationToken: token,
            datos: obj
        },
        url: "/_crear",
        success: function () {
            $("#comentarios").load("/_listado/?idContenido=" + idContenido);
            $("#crear_comentario").load("/_crear/?idContenido=" + idContenido);
        }
    });
    
}