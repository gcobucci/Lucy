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

    // footer
    
}
