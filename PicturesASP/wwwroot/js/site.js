﻿$(document).ready(function () {

    //show image in modal
    $('.thumbnails .photoImg').click(function () {
        var name = $(this).find('img').attr('src');

        $('.item img').attr('src', name);
        $('.item img').attr('name', name);
        $('#myModal').modal('show');
    });

});