$(document).ready(function () {
    kendo.culture("pt-BR");
    var dsMenuLateral = null;

    if (dsMenuLateral === null) {
        menuLateral();
        localStorage.setItem('leftMenuLateral', JSON.stringify($('#Left-Container').html()));
    }
    else {
        $('#Left-Container').html('');
        $('#Left-Container').html(dsMenuLateral);
    }

    $('.minimize-menu').click(function () {
        localStorage.clear();
        if ($(this).attr('class') === 'minimize-menu disable') {
            minimizeMenuLateral();
        }
        else {
            maximizeMenuLateral();
        }

        /* -- MENU - MINI -- */
        $('.mini-nav ul.nav-list li').click(function () {
            if ($(this).children('ul').attr('style') === 'display: block;') {
                $(this).children('ul').hide();
                $(this).removeAttr('style');
            }
            else {
                $(this).children('ul').show();
                $(this).attr('style', 'background-color: #00549F; color: #fff;');
            }
        });
    });

    // Function Collapse Icon
    $('#accordion .panel .panel-heading').click(function () {
        var iconCollapse = $(this).children('h4').children('small').children('span').attr('class');
        if (iconCollapse == "glyphicon glyphicon-chevron-right") {
            $(this).children('h4').children('small').children('span').removeAttr('class');
            $(this).children('h4').children('small').children('span').attr('class', 'glyphicon glyphicon-chevron-down');
        }
        else {
            $(this).children('h4').children('small').children('span').removeAttr('class');
            $(this).children('h4').children('small').children('span').attr('class', 'glyphicon glyphicon-chevron-right');
        }
    });
});

function menuLateral() {
    $.ajax({
        url: '/Menu/MenuLateral',
        type: 'GET',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            this.menuLateral = result;

            /* -- MENU LATERAL -- */
            $('#Left-Container .nav-menu .nav-list').html(this.menuLateral);

            /* -- MENU MOBILE -- */
            $('#nav-menu-mobile .nav-list-mobile').html(this.menuLateral);
            var listmenu = $(function () {
                //Menu lista ao clicar no icone
                $('#icone-lista').on('click', this, function () {
                    //$('#nav-menu-mobile').addClass('active');
                    $('.nav-list-mobile').slideToggle();
                });

                //Efeito Drop no menu lista
                $('.nav-list-mobile > li > a').on('click', this, function () {
                    var active = $(this).attr('class');
                    if (active === 'active-item') {
                        $(this).next('.submenu-menu-lista').slideToggle();
                    }
                    else {
                        $('.submenu-menu-lista').slideUp();
                        $(this).next('.submenu-menu-lista').slideToggle();
                    }
                    $('.nav-list-mobile > li > a').removeAttr('class', 'active-item');
                    $(this).attr('class', 'active-item');
                });
            });
        }
    });
}

function minimizeMenuLateral() {
    $('#btnMinimizarMenuLateral').insertAfter('#menuLateralLiteral');
    $('#Left-Container .nav-menu').addClass("mini-nav");
    $('body').attr('style', 'background: url("/Content/images/layout/bg-menu-mini.jpg") left top repeat-y;');
    $('html').attr('style', 'background: transparent !important;');

    $('ul.nav-list li ul').each(function () {
        $(this).hide();
    });
    $('#Left-Container').animate({
        width: '40px'
    }, 30);
    $('#Center-Container').animate({
        paddingLeft: '40px'
    }, 30);

    $('.minimize-menu').removeClass("disable");
    $('.minimize-menu').addClass("active");
    $('.minimize-menu span').removeClass('glyphicon-circle-arrow-left');
    $('.minimize-menu span').addClass('glyphicon-circle-arrow-right');
}

function maximizeMenuLateral() {
    $('.mini-nav ul.nav-list li').each(function () {
        $(this).removeAttr('style');
    });

    $('#Left-Container').animate({
        width: '220px'
    }, 30, function () {
        $('#Left-Container .nav-menu').removeClass("mini-nav");
        $('body').attr('style', 'background: url("/Content/images/layout/bg-menu-big.jpg") left top repeat-y;');
    });
    $('#Center-Container').animate({
        paddingLeft: '220px'
    }, 30);

    $('.minimize-menu').removeClass("active");
    $('.minimize-menu').addClass("disable");
    $('.minimize-menu span').removeClass('glyphicon-circle-arrow-right');
    $('.minimize-menu span').addClass('glyphicon-circle-arrow-left');
    $('.mini-nav ul.nav-list li ul').each(function () {
        $(this).show();
    });
    $('#btnMinimizarMenuLateral').insertBefore('#menuLateralLiteral');
}