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

    //$.validator.methods.number = function (value, element) {
    //    value = floatValue(value);
    //    return this.optional(element) || !isNaN(value);
    //}
    //$.validator.methods.range = function (value, element, param) {
    //    value = floatValue(value);
    //    return this.optional(element) || (value >= param[0] && value <= param[1]);
    //}

    //function floatValue(value) {
    //    return parseFloat(value.replace(",", "."));
    //}

    poneleColorJC();
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
    document.getElementById("ComentarioCuerpo").placeholder = "Responder a " + UsuNom + "...";
    document.getElementById("btn-CancelarRespuesta").style.display = "inline-block";
}

function cancelarRespuesta() {
    $('#ComentarioPadreId').val(null);
    document.getElementById("ComentarioCuerpo").placeholder = "Escribe un comentario...";
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

function poneleColorJC() {
    var usuariosAvatar = document.getElementsByClassName("comment-avatar");
    var usuariosNombre = document.getElementsByClassName("comment-name");
    var usuariosLetra = document.getElementsByClassName("comment-avatar-letter");
    for (var i = 0; i < usuariosAvatar.length; i++) {
        var nombre = usuariosNombre[i].innerText;
        var auxLetra = nombre.charAt(0);
        var letra = auxLetra.toUpperCase();
        switch (letra) {
            case "T":
                usuariosAvatar[i].style.backgroundColor = "#333333";
                usuariosLetra[i].innerText = "T";
                break;
            case "M":
                usuariosAvatar[i].style.backgroundColor = "#d3d3d3";
                usuariosLetra[i].innerText = "M";
                break;
        }
    }
}
