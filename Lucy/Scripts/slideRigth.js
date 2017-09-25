$(document).ready(function($){
    if (document.body.clientWidth > 380) {
        $(".cruz").click(function () {
            var div_left = $("#mySidenav").position();
            if (div_left.left > -251 && div_left.left < -249) {
                document.getElementById("mySidenav").style.opacity = "1";
                document.getElementById("mySidenav").style.left = "0";
                document.getElementById("out").style.opacity = "1";
                document.getElementById("out").style.zIndex = "899";
                $('.barras').addClass('change');
            } else {
                document.getElementById("mySidenav").style.opacity = "0";
                document.getElementById("mySidenav").style.left = "-250px";
                document.getElementById("out").style.opacity = "0";
                document.getElementById("out").style.zIndex = "-1";
                $('.barras').removeClass('change');
            }
        });

        $("#out").click(function () {
            var div_left = $("#mySidenav").position();
            document.getElementById("out").style.zIndex = "-1";
            if (div_left.left != -250) {
                $('.barras').removeClass('change');

            }
        });
    } else {
        $(".cruz").click(function () {
            if (document.getElementById("mySidenav").style.opacity == 0) {
                document.getElementById("mySidenav").style.opacity = "1";
                document.getElementById("mySidenav").style.height = "100%";
                document.getElementById("out").style.opacity = "1";
                document.getElementById("out").style.zIndex = "899";
                $('.barras').addClass('change');
            } else {
                document.getElementById("mySidenav").style.opacity = "0";
                document.getElementById("mySidenav").style.height = "0";
                document.getElementById("out").style.opacity = "0";
                document.getElementById("out").style.zIndex = "-1";
                $('.barras').removeClass('change');
            }
        });

        $("#out").click(function () {
            var div_left = $("#mySidenav").position();
            document.getElementById("out").style.zIndex = "-1";
            if (div_left.left != -250) {
                $('.barras').removeClass('change');

            }
        });
    }
});  

function closeNav() {
    if (document.body.clientWidth > 380) {
        document.getElementById("mySidenav").style.opacity = "0";
        document.getElementById("mySidenav").style.left = "-250px";
        document.getElementById("out").style.opacity = "0";
    } else {
        document.getElementById("mySidenav").style.opacity = "0";
        document.getElementById("mySidenav").style.height = "0";
        document.getElementById("out").style.opacity = "0";
    }
}
