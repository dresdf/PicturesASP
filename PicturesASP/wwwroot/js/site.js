// Write your JavaScript code.
$(document).ready(function () {

    //MENU ACCORDION
    $('#left .menu li a').click(function () {
        if ($(this).hasClass('closed')) {
            $(this).toggleClass('opened closed');
            $(this).parent().siblings().find('.sub-menu').slideUp();
            $(this).parent().siblings().find('a.opened').toggleClass('opened closed');
            $(this).parent().children().find('a.active').toggleClass('active');
            $(this).next().slideDown();
            return false;
        } else if ($(this).hasClass('opened')) {
            $(this).toggleClass('opened closed');
            $(this).next().slideUp();
            return false;
        }
    });

    //SUB-MENU "ACTIVE" MARKUP
    $('#left .menu li ul li a').click(function () {
        if (!$(this).hasClass('active')) {
            $(this).toggleClass('active');
            $(this).parent().siblings().find('a.active').toggleClass('active');
        }
    });
});