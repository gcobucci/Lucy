$(window).scroll(function(){
    window_y = $(window).scrollTop();
    if (window_y > 50) {
    	document.getElementById("menuContainer").style.background = "#222";
    } else {
    	document.getElementById("menuContainer").style.background = "rgba(0,0,0,0.8)";
    }
});