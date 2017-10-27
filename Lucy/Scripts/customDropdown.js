$(document).ready(function () {
    $(".dropdown1, .dropdown2").hover(
        function () {
            $('.dropdown-menuS', this).stop(true, true).slideDown("fast");
            $(this).toggleClass('open');
        },
        function () {
            $('.dropdown-menuS', this).stop(true, true).slideUp("fast");
            $(this).toggleClass('open');
        },
    );

    $('[data-toggle="popover"]').popover();

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

    lastlogin();

    notificaciones();
    setInterval(notificaciones, 120000);

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
            case "A":
                usuariosAvatar[i].style.backgroundColor = "#333333";
                usuariosLetra[i].innerText = "A";
                break;
            case "B":
                usuariosAvatar[i].style.backgroundColor = "#d3d3d3";
                usuariosLetra[i].innerText = "B";
                break;
			case "C":
                usuariosAvatar[i].style.backgroundColor = "#FF5733";
                usuariosLetra[i].innerText = "C";
                break;
			case "D":
                usuariosAvatar[i].style.backgroundColor = "#CD5C5C";
                usuariosLetra[i].innerText = "D";
                break;
			case "E":
                usuariosAvatar[i].style.backgroundColor = "#C0C0C0";
                usuariosLetra[i].innerText = "E";
                break;
			case "F":
                usuariosAvatar[i].style.backgroundColor = "#808080";
                usuariosLetra[i].innerText = "F";
                break;
			case "G":
                usuariosAvatar[i].style.backgroundColor = "#000000";
                usuariosLetra[i].innerText = "G";
                break;
			case "H":
                usuariosAvatar[i].style.backgroundColor = "#FF0000";
                usuariosLetra[i].innerText = "H";
                break;
			case "I":
                usuariosAvatar[i].style.backgroundColor = "#800000";
                usuariosLetra[i].innerText = "I";
                break;
			case "J":
                usuariosAvatar[i].style.backgroundColor = "#FFFF00";
                usuariosLetra[i].innerText = "J";
                break;
			case "K":
                usuariosAvatar[i].style.backgroundColor = "#808000";
                usuariosLetra[i].innerText = "K";
                break;
			case "L":
                usuariosAvatar[i].style.backgroundColor = "#00FF00";
                usuariosLetra[i].innerText = "L";
                break;
			case "M":
                usuariosAvatar[i].style.backgroundColor = "#008000";
                usuariosLetra[i].innerText = "M";
                break;
			case "N":
                usuariosAvatar[i].style.backgroundColor = "#00FFFF";
                usuariosLetra[i].innerText = "N";
                break;
			case "Ñ":
                usuariosAvatar[i].style.backgroundColor = "#008080";
                usuariosLetra[i].innerText = "Ñ";
                break;
			case "O":
                usuariosAvatar[i].style.backgroundColor = "#0000FF";
                usuariosLetra[i].innerText = "O";
                break;
			case "P":
                usuariosAvatar[i].style.backgroundColor = "#000080";
                usuariosLetra[i].innerText = "P";
                break;
			case "Q":
                usuariosAvatar[i].style.backgroundColor = "#FF00FF";
                usuariosLetra[i].innerText = "Q";
                break;
			case "R":
                usuariosAvatar[i].style.backgroundColor = "#800080";
                usuariosLetra[i].innerText = "R";
                break;
			case "S":
                usuariosAvatar[i].style.backgroundColor = "#581845";
                usuariosLetra[i].innerText = "S";
                break;
			case "T":
                usuariosAvatar[i].style.backgroundColor = "#900C3F";
                usuariosLetra[i].innerText = "T";
                break;
			case "U":
                usuariosAvatar[i].style.backgroundColor = "#C70039";
                usuariosLetra[i].innerText = "U";
                break;
			case "V":
                usuariosAvatar[i].style.backgroundColor = "#FF5733";
                usuariosLetra[i].innerText = "V";
                break;
			case "W":
                usuariosAvatar[i].style.backgroundColor = "#FFC300";
                usuariosLetra[i].innerText = "W";
                break;
			case "X":
                usuariosAvatar[i].style.backgroundColor = "#DAF7A6";
                usuariosLetra[i].innerText = "X";
                break;
			case "Y":
                usuariosAvatar[i].style.backgroundColor = "#00268C";
                usuariosLetra[i].innerText = "Y";
                break;
			case "Z":
                usuariosAvatar[i].style.backgroundColor = "#C90404";
                usuariosLetra[i].innerText = "Z";
                break;
			case "1":
                usuariosAvatar[i].style.backgroundColor = "#A23232";
                usuariosLetra[i].innerText = "1";
                break;
			case "2":
                usuariosAvatar[i].style.backgroundColor = "#34495E";
                usuariosLetra[i].innerText = "2";
                break;
			case "3":
                usuariosAvatar[i].style.backgroundColor = "#E67E22";
                usuariosLetra[i].innerText = "3";
                break;
			case "4":
                usuariosAvatar[i].style.backgroundColor = "#85C1E9";
                usuariosLetra[i].innerText = "4";
                break;
			case "5":
                usuariosAvatar[i].style.backgroundColor = "#45B39D";
                usuariosLetra[i].innerText = "5";
                break;
			case "6":
                usuariosAvatar[i].style.backgroundColor = "#A569BD";
                usuariosLetra[i].innerText = "6";
                break;
			case "7":
                usuariosAvatar[i].style.backgroundColor = "#084200";
                usuariosLetra[i].innerText = "7";
                break;
			case "8":
                usuariosAvatar[i].style.backgroundColor = "#F0A5A5";
                usuariosLetra[i].innerText = "8";
                break;
			case "9":
                usuariosAvatar[i].style.backgroundColor = "#0042FF";
                usuariosLetra[i].innerText = "9";
                break;
			case "0":
                usuariosAvatar[i].style.backgroundColor = "#00B6FF";
                usuariosLetra[i].innerText = "0";
                break;
			case "@":
                usuariosAvatar[i].style.backgroundColor = "#FF0059";
                usuariosLetra[i].innerText = "@";
                break;
			case "Á":
                usuariosAvatar[i].style.backgroundColor = "#CD5C5C";
                usuariosLetra[i].innerText = "Á";
                break;
			case "É":
                usuariosAvatar[i].style.backgroundColor = "#C0C0C0";
                usuariosLetra[i].innerText = "É";
                break;
			case "Í":
                usuariosAvatar[i].style.backgroundColor = "#00B6FF";
                usuariosLetra[i].innerText = "Í";
                break;
			case "Ó":
                usuariosAvatar[i].style.backgroundColor = "#C70039";
                usuariosLetra[i].innerText = "Ó";
                break;
			case "Ú":
                usuariosAvatar[i].style.backgroundColor = "#A23232";
                usuariosLetra[i].innerText = "Ú";
                break;
			case ".":
                usuariosAvatar[i].style.backgroundColor = "#C70039";
                usuariosLetra[i].innerText = ".";
                break;
			case ",":
                usuariosAvatar[i].style.backgroundColor = "#DAF7A6";
                usuariosLetra[i].innerText = ",";
                break;
        }
    }
}

function aComerla() {
    var cal = document.getElementsByClassName("cal");
    var car = document.getElementsByClassName("car");
    var azucar = document.getElementsByClassName("azucar");
    var grasa = document.getElementsByClassName("grasa");
    var sodio = document.getElementsByClassName("sodio");
    var gluten = document.getElementsByClassName("gluten");
    var cant = document.getElementsByClassName("cant");

    var auxCal = "";
    var auxCar = "";
    var auxAzucar = "";
    var auxGrasa = "";
    var auxSodio = "";

    var hayGluten = false;

    document.getElementById("ComidaGluten").value = "";

    for (var i = 0; i < cant.length; i++) {
        if (cant[i].value > 0) {
            if (auxCal != "") {
                var papa = parseInt(cal[i].innerText) * cant[i].value;
                auxCal = papa + parseInt(auxCal);
            } else {
                var papa = parseInt(cal[i].innerText) * cant[i].value;
                auxCal = papa;
            }

            if (auxCar != "") {
                var papa = parseInt(car[i].innerText) * cant[i].value;
                auxCar = papa + parseInt(auxCar);
            } else {
                var papa = parseInt(car[i].innerText) * cant[i].value;
                auxCar = papa;
            }

            if (auxAzucar != "") {
                var papa = parseInt(azucar[i].innerText) * cant[i].value;
                auxAzucar = papa + parseInt(auxAzucar);
            } else {
                var papa = parseInt(azucar[i].innerText) * cant[i].value;
                auxAzucar = papa;
            }

            if (auxGrasa != "") {
                var papa = parseInt(grasa[i].innerText) * cant[i].value;
                auxGrasa = papa + parseInt(auxGrasa);
            } else {
                var papa = parseInt(grasa[i].innerText) * cant[i].value;
                auxGrasa = papa;
            }

            if (auxSodio != "") {
                var papa = parseInt(sodio[i].innerText) * cant[i].value;
                auxSodio = papa + parseInt(auxSodio);
            } else {
                var papa = parseInt(sodio[i].innerText) * cant[i].value;
                auxSodio = papa;
            }

            if (gluten[i].innerHTML == "Si") {
                document.getElementById("ComidaGluten").value = "true";
                hayGluten = true;
            }
        }
    }

    document.getElementById("ComidaCalorias").value = auxCal;
    document.getElementById("ComidaCarbohidratos").value = auxCar;
    document.getElementById("ComidaAzucar").value = auxAzucar;
    document.getElementById("ComidaGrasa").value = auxGrasa;
    document.getElementById("ComidaSodio").value = auxSodio;
    for (var i = 0; i < gluten.length; i++) {
        if (cant[i].value > 0) {
            if (gluten[i].innerHTML != "Sin asignar") {
                if (hayGluten == false) {
                    document.getElementById("ComidaGluten").value = "false";
                }
            }
        }
    }
}

function notificaciones() {
    $.ajax({
        type: 'POST',
        dataType: 'json',
        url: "/notificaciones/CountNot",
        success: function (data) {
            if (data > 0) {
                $("#btnNotificacion .badge").text(data);
            } else {
                if (data < 1) {
                    $("#btnNotificacion .badge").text("");
                }
            }
        }
    });
}

function calcularResultado(valorControl, hidratos) {
    $.ajax({
        type: 'POST',
        dataType: 'json',
        data: {
            valorControl: valorControl,
            hidratos: hidratos
        },
        url: "/registros/control_diabetico/calcularResultado",
        success: function (data) {
            //alert(JSON.stringify(data));
            //alert(data.totalInsulina);
            document.getElementById("msgResultadoControl").style.display = "block";
            $("#msgResultadoControl").text(data.resultadoInsulinaMensaje);
            $("#ResultadoTotalInsulinaCorreccion").val(data.totalInsulina);
        }
    });
}

function eliminarNot(idPanel, idNot) {
    document.getElementById(idPanel).style.left = "-1000px";
    var x = setInterval(function () {
        document.getElementById(idPanel).style.display = "none";
        document.getElementById(idPanel).innerHTML = null;
        $.ajax({
            type: 'POST',
            dataType: 'json',
            data: { id: idNot },
            url: "/notificaciones/eliminar"
        });
        clearInterval(x);
    }, 400);
}

function lastlogin() {
    $.ajax({
        dataType: 'json',
        type: 'POST',
        url: "/usuarios/lastlogin"
    }); 
}

function eliminarFav(idPanel, idCont, idUsu) {
    document.getElementById(idPanel).style.left = "-1000px";
    var x = setInterval(function () {
        document.getElementById(idPanel).style.display = "none";
        document.getElementById(idPanel).innerHTML = null;
        $.ajax({
            dataType: 'json',
            data: {
                idCont: idCont,
                idUsu: idUsu
            },
            type: 'POST',
            url: "/favorito/deleteFav"
        });
        clearInterval(x);
    }, 400);
}
