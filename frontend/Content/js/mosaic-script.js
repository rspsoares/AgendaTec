var curInterval;
var Keeptrue = {};
var teste = 1;

$(document).ready(function () {
    kendo.culture("pt-BR");    
    var dsMenuLateral = null;

    if (dsMenuLateral === null) {
        menuLateral();
        localStorage.setItem('leftMenuLateral', JSON.stringify($('#Left-Container').html()));
    }
    else{
        $('#Left-Container').html('');
        $('#Left-Container').html(dsMenuLateral);        
    }

    $('li').mouseover(function (e) {
        if (e.currentTarget.id == "menuRelatorio" || e.currentTarget.id == "menuGrafico" || e.currentTarget.id == "menuGraficoComparativo" || e.currentTarget.id == "menuGraficoCredito" || e.currentTarget.id == "menuGraficoReceita" || e.currentTarget.id == "menuGraficoValorManual")
            $(this).css('background-color', 'transparent');            
    });

    $('.minimize-menu').click(function () {
        //localStorage.clear();
        if ($(this).attr('class') == 'minimize-menu disable') {
            minimizeMenuLateral();
        }
        else {
            maximizeMenuLateral();
        }

        /* -- MENU - MINI -- */
        $('.mini-nav ul.nav-list li').click(function () {
            //alert($(this).children('ul').html());
            if ($(this).children('ul').attr('style') == 'display: block;') {
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
        url: '/Roles/MenuLateral',       
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
                    if (active == 'active-item') {
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

/* -- Filtros Functions -- */
function FormatDateKendo(date) {
    date = date.split(/\//);
    date = [date[1], date[0], date[2]].join('/');
    date = new Date(date);

    return date;
}

function BuscarDadosGrid(grid, msgNenhumRegistro) {
    $filter = new Array();
    var orfilter = "";

    if (grid == undefined)
        grid = $("#grid").data("kendoGrid");
    
    if (msgNenhumRegistro == undefined) {
        msgNenhumRegistro = "Nenhum registro encontrado!";
    }

    $(".filter").each(function (index, ele) {
        switch ($(this).val()) {
            case "":
            case "Escolha ...":
                break;

            default:
                if ($(ele).data("type") == "date") {
                    $filter.push({ field: $(ele).data("field"),type: $(ele).data("type"), operator: $(ele).data("operator"), value: FormatDateKendo($(ele).val()) });
                }
                else {
                    $filter.push({ field: $(ele).data("field"), type: $(ele).data("type"), operator: ele.dataset.operator, value: $(ele).val() });
                }
                break;
        }
    });

    if ($filter.length >= 2 || ($filter.length == 1 && $filter[0].type == "date")) {
        orfilter = { logic: "and", filters: $filter };
    }
    else {
        orfilter = { filters: $filter };
    }

    grid.dataSource.filter(orfilter);

    if(grid.dataSource.total() == 0)
    {
        ShowModalAlerta(msgNenhumRegistro);
    }
}

// Grid Incrições / Holding
function LoadDsInscricoes() {
    var datasource = undefined;

    $.ajax({
        url: "/Home/PesquisarInscricoes",
        type: "GET",
        dataType: "json",
        async: false,
        cache: false,
        success: function (result) {
            if (result.length > 0) {
                datasource = result;
                GridInscricoes(datasource);
            }
            else {
                $("#gridGroupAccount").removeAttr("class");
                $("#gridGroupAccount").html("<p>Nenhuma Inscrição encontrada!</p>");
            }
        }
    });
}

function GridInscricoes(ds) {
    $("#gridGroupAccount").html("");
    $('#gridGroupAccount').kendoGrid({
        dataSource: {
            data: ds,
            schema: {
                model: {
                    fields: {
                        Id: { type: "number" },
                        Nome: { type: "string" },
                        Descricao: { type: "string" }
                    }
                }
            },
            pageSize: 10
        },
        filterable: {
            mode: "row",
            operators: {
                string: {
                    contains: "Contém"
                }
            }
        },       
        selectable: true,
        scrollable: false,
        change: SelecionarInscricao,
        pageable: {
            pageSizes: [10, 25, 50]
        },
        columns: [
            { field: "Id", hidden: true },
            {
                field: "Nome",
                title: "Conta",
                width: '25%',
                filterable: {
                    cell: {
                        operator: "contains"
                    }
                }
            },
            {
                field: "Descricao",
                title: "Descrição",
                width: '75%',
                filterable: {
                    cell: {
                        operator: "contains"
                    }
                }
            }
        ]
    });
}

function LoadDsMatrizes() {
    var dsMatriz = undefined;
    $.ajax({
        url: "/Empresa/Pesquisar?idInscricao=1",
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            dsMatriz = result;
        }
    });

    if (dsMatriz == undefined) {
        ShowModalAlerta("Não foi possível obter as Matrizes.");
        return;
    }

    return dsMatriz;
}

/* -- Formatação de Moeda -- */
Number.prototype.FormatarMoeda = function (places, symbol, thousand, decimal, negativoParenteses) {
    var valorFormatado;
    places = !isNaN(places = Math.abs(places)) ? places : 2;
    symbol = symbol !== undefined ? symbol : "";
    thousand = thousand || ".";
    decimal = decimal || ",";
    var number = this,
	    negative = number < 0 ? "-" : "",
	    i = parseInt(number = Math.abs(+number || 0).toFixed(places), 10) + "",
	    j = (j = i.length) > 3 ? j % 3 : 0;
    
    if (negative == "-" && negativoParenteses == true)   
        valorFormatado = symbol + '(' + (j ? i.substr(0, j) + thousand : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousand) + (places ? decimal + Math.abs(number - i).toFixed(places).slice(2) : "") + ')';    
    else    
        valorFormatado = symbol + negative + (j ? i.substr(0, j) + thousand : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousand) + (places ? decimal + Math.abs(number - i).toFixed(places).slice(2) : "");
    
    return valorFormatado; // symbol + negative + (j ? i.substr(0, j) + thousand : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousand) + (places ? decimal + Math.abs(number - i).toFixed(places).slice(2) : "");
};

Number.prototype.FormatarMilhar = function () {
    var parts = (this + "").split(","),
        main = parts[0],
        len = main.length,
        output = "",
        i = len - 1;

    while (i >= 0) {
        output = main.charAt(i) + output;
        if ((len - i) % 3 === 0 && i > 0) {
            output = "." + output;
        }
        --i;
    }
    // put decimal part back
    if (parts.length > 1) {
        output += "," + parts[1];
    }

    output = output.replace("..", ".");

    return output;
}

Number.prototype.formatMoney = function (c, d, t) {
    var n = this, c = isNaN(c = Math.abs(c)) ? 2 : c, d = d == undefined ? "," : d, t = t == undefined ? "." : t, s = n < 0 ? "-" : "", i = parseInt(n = Math.abs(+n || 0).toFixed(c)) + "", j = (j = i.length) > 3 ? j % 3 : 0;
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};

/* -- Gerais -- */
function OrdenarListBox(listbox) {
    var $r = $(listbox + " option");
    $r.sort(function (a, b) {
        return (a.value < b.value) ? -1 : (a.value > b.value) ? 1 : 0;        
    });
    $($r).remove();
    $(listbox).append($($r));
}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

/* -- Cookies -- */
function GerarCookie(strCookie, strValor, lngDias) {
    $.cookie(strCookie, strValor, {
        expires: lngDias
    });
}

function LerCookie(nomeCookie) {
    if ($.cookie(nomeCookie) != null) {
        return $.cookie(nomeCookie);
    }
    else {
        return undefined;
    }
}

function LimparCookie(strCookie) {
    $.cookie(strCookie, null);
}

/* -- MODAL POPUPs -- */
function CloseModal(e) {
    $('#' + e.parentElement.parentElement.parentElement.parentElement.id).data('data', null);
    $('#' + e.parentElement.parentElement.parentElement.parentElement.id).modal("hide");
}

function CloseModalResumo(e) {
    $('#' + e.parentElement.parentElement.parentElement.parentElement.parentElement.id).modal("hide");
}
 
function GerarPDF(e, orientacaoPaisagem) {
    var printHtml = '#' + e.parentElement.parentElement.parentElement.parentElement.id + ' .modal-dialog .modal-body .body-message';
    printHtml = { "html": $(printHtml).html() };    

    $.ajax({
        url: "/Home/GerarPDF",
        type: "POST",
        async: false,
        data: JSON.stringify({ html: printHtml.html, orientacaoPaisagem: orientacaoPaisagem }),
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Sucesso) {                
                window.open(result.Link, '_blank');                            
            }
            else {
                ShowModalAlerta(result.Msg);
            }  
        }        
    });      
}

function GerarXLS(e, tipoRelatorio, orientacaoPaisagem) {
    var idEmpresa = 0;
    var periodo = '';
    var tipoImposto = '';

    var printHtml = '#' + e.parentElement.parentElement.parentElement.parentElement.id + ' .modal-dialog .modal-body .body-excel';
    printHtml = { "html": $(printHtml).html() };

    idEmpresa = parseInt(printHtml.html.split(';')[0]);
    periodo = printHtml.html.split(';')[1];
    tipoImposto = IsNull(printHtml.html.split(';')[2], '');

    $.ajax({
        url: "/Home/GerarXLS",
        type: "POST",
        async: false,
        data: JSON.stringify({ idEmpresa: idEmpresa, periodo: periodo, tipoImposto: tipoImposto, tipoRelatorio: tipoRelatorio, orientacaoPaisagem: orientacaoPaisagem }),
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Sucesso) {
                window.open(result.Link, '_blank');
            }
            else {
                ShowModalAlerta(result.Msg);
            }
        }
    });
}

function ShowModalResumo(title, dataHtml) {
    $('#modalResumo').modal({ backdrop: 'static', keyboard: false });
    $('#modalResumo .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalResumo .modal-dialog .modal-header center .modal-title strong').html(title);
    $('#modalResumo .modal-dialog .modal-body').html("");
    $('#modalResumo .modal-dialog .modal-body').html(dataHtml);
}

function ShowModalAlerta(dataHtml) {
    $('#modalAlert').modal({ backdrop: 'static', keyboard: false });
    $('#modalAlert .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalAlert .modal-dialog .modal-header center .modal-title strong').html("Atenção");
    $('#modalAlert .modal-dialog .modal-body .alert').html("");
    $('#modalAlert .modal-dialog .modal-body .alert').html(dataHtml);
}

function ShowModalSucesso(dataHtml) {
    $('#modalSuccess').modal({ backdrop: 'static', keyboard: false });
    $('#modalSuccess .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalSuccess .modal-dialog .modal-header center .modal-title strong').html("Sucesso");
    $('#modalSuccess .modal-dialog .modal-body .alert').html("");
    $('#modalSuccess .modal-dialog .modal-body .alert').html(dataHtml);
}

jQuery.extend({
    compare: function (arrayA, arrayB) {
        if (arrayA.length != arrayB.length) { return false; }
        // sort modifies original array
        // (which are passed by reference to our method!)
        // so clone the arrays before sorting
        var a = jQuery.extend(true, [], arrayA);
        var b = jQuery.extend(true, [], arrayB);
        a.sort(); 
        b.sort();
        for (var i = 0, l = a.length; i < l; i++) {
            if (a[i] !== b[i]) { 
                return false;
            }
        }
        return true;
    }
});

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}


function RemoveTagsTotalizacao(original) {
    var valor = original
        .replace("<div>", "")
        .replace("<div style='float: right'>", "")
        .replace("<span style='float:right;'>", "")
        .replace("</span>", "")
        .replace("</div>", "");

    return valor;
}

/* Vigência das regras de apuração */
function CriarNovaVigencia() {
    var msgErro = '';    
    var idRegra = $('#txtIdRegraVigencia').val();
    var retroativo = $('#selectRetroativo').bootstrapSwitch('state');    
    var dtFinal = $("#tbVigenciaFinalRegra").data("kendoDatePicker").value();
    var justificativa = $('#tbJustificativaVigencia').val();
    var tipoRegra = $('#txtTipoRegra').val();

    var split = $("#tbVigenciaInicial").val().split("/");
    var dtInicial = new Date(split[2], split[1] - 1, split[0]);

    msgErro = ValidarCamposVigencia(retroativo, justificativa, dtInicial, dtFinal);
    
    if (msgErro == '') {
        if (dtFinal == undefined)
            dtFinal = new Date();

        $.ajax({
            url: "/Apuracao/CriarVigenciaRegra",
            type: "POST",
            async: false,
            data: JSON.stringify({ idRegra: idRegra, retroativo: retroativo, justificativa: justificativa, dtInicialAtual: dtInicial, dtFinalAtual: dtFinal }),
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                msgErro = result.Msg;
            }
        });
    }

    return msgErro;
}

function ValidarCamposVigencia(retroativo, justificativa, inicioFigencia, finalVigencia) {
    var msgErro = '';

    if (finalVigencia == undefined)
        msgErro = 'Favor informar a Data de Término da Vigência atual.';

    if (inicioFigencia > finalVigencia )
        msgErro += 'A data do final da vigência não pode ser anterior ao seu início.';

    if (retroativo) {
        if (justificativa == '')
            msgErro += '<br/> Favor informar a justificativa.';
    }

    return msgErro;
}

function CarregarGridVigenciaLog() {
    var dsVigencias = [];
    var idRegra = $('#txtIdRegra').val();

    $("#gridVigencia").html("");
    $("#gridVigencia").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: "/Apuracao/ObterVigenciaLog",
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idRegra: idRegra
                        }
                    }
                }
            },
            pageSize: 3,
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                }
            }
        },
        dataBound: function () {
            var grid = $("#gridVigencia").data("kendoGrid");
            grid.select($("tr:first", grid.tbody));
        },
        change: SelecionarVigencia,
        scrollable: true,
        selectable: true,
        sortable: false,
        groupable: false,
        resizable: true,
        cache: false,
        pageable: {
            refresh: true,
            pageSizes: true,
            buttonCount: 10
        },
        columns: [
            { field: "Id", hidden: true },
            { title: "Início", field: "InicioVigencia", width: "10%", template: "#= kendo.toString(kendo.parseDate(InicioVigencia, 'yyyy-MM-dd'), 'dd/MM/yyyy') #" },
            { title: "Término", field: "TerminoVigencia", width: "10%", template: "#:FormatarTermino(TerminoVigencia)#" },
            { title: "Justificativa", field: "Justificativa", width: "60%" },
            { title: "Usuário", field: "NomeUsuario", width: "15%" },
            { title: "Data Criação", field: "DataCriacao", width: "15%", template: "#= kendo.toString(kendo.parseDate(DataCriacao, 'yyyy-MM-dd HH:mm:ss'), 'dd/MM/yyyy HH:mm:ss') #" }
        ]
    });
}

function FormatarTermino(dataTermino) {
    if (dataTermino)
        return kendo.toString(kendo.parseDate(dataTermino, 'yyyy-MM-dd'), 'dd/MM/yyyy')
    else
        return '';
}

function SelecionarVigencia() {
    var gridVigencia = $("#gridVigencia").data("kendoGrid");
    var linhaGrid = gridVigencia.dataItem(gridVigencia.select());

    $("#gridCondicoes").html("");
    $("#gridCondicoes").kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: 'CondicoesVigencia_' + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + '.xlsx',
            allPages: true
        },
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                if (rowIndex % 2 == 0) {
                    var row = sheet.rows[rowIndex];
                    for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                        row.cells[cellIndex].background = "#aabbcc";
                    }
                }
            }
        },
        dataSource: {
            transport: {
                read: {
                    url: "/Apuracao/ObterCondicoesVigenciaLog",
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idVigencia: linhaGrid.Id
                        }
                    }
                }
            },
            pageSize: 10,
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                }
            }
        },
        scrollable: true,
        sortable: true,
        resizable: true,
        dataBound: gridDataBound,
        pageable: {
            refresh: true,
            pageSizes: true,
            buttonCount: 10
        },
        columns: [
            { title: "CFOP", field: "CFOP", width: "55px" },
            { title: "Descrição", field: "Descricao", width: "200px" },
            { title: "Dígito", field: "Digito", width: "50px" },
            { title: "CFOP Referência", field: "CFOPRef", width: "110px" },
            { title: "Categoria Nota", field: "CategoriaNota", width: "100px" },
            { title: "CST", field: "CST", width: "50px" },
            { title: "Grupo Material", field: "GrupoMaterial", width: "250px" },
            { title: "Material", field: "Material" }
        ]
    });
}

function FormatarValorGrafico(valor, valoresZerados, tipoGrafico) {
    var ret = 0;

    if (valoresZerados == false) {
        if (valor != 0) {
            if (tipoGrafico == "manual" || tipoGrafico == 'comparativo') 
                ret = valor;            
            else 
                ret = valor / 1000;                        
        }

        return ret.FormatarMilhar();
    }
    else 
        return '';
}