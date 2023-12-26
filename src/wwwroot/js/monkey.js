var $ = (s, o = document) => o.querySelector(s);

$(".monkey").addEventListener("click", function () {
    $(".password").classList.toggle("show");

    var input = $("#password-field");
    if (input.getAttribute("type") == "password") {
        input.setAttribute("type", "text");
    } else {
        input.setAttribute("type", "password");
    }

});
