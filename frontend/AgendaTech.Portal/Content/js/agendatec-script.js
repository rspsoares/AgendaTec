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
        ShowModalAlert(msgNenhumRegistro);
    }
}

Number.prototype.FormatMoney = function (places, symbol, thousand, decimal, negativoParenteses) {
    var value;
    places = !isNaN(places = Math.abs(places)) ? places : 2;
    symbol = symbol !== undefined ? symbol : "";
    thousand = thousand || ".";
    decimal = decimal || ",";
    var number = this,
        negative = number < 0 ? "-" : "",
        i = parseInt(number = Math.abs(+number || 0).toFixed(places), 10) + "",
        j = (j = i.length) > 3 ? j % 3 : 0;
    
    if (negative === "-" && negativoParenteses === true)   
        value = symbol + '(' + (j ? i.substr(0, j) + thousand : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousand) + (places ? decimal + Math.abs(number - i).toFixed(places).slice(2) : "") + ')';    
    else    
        value = symbol + negative + (j ? i.substr(0, j) + thousand : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousand) + (places ? decimal + Math.abs(number - i).toFixed(places).slice(2) : "");
    
    return value;
};

Number.prototype.FormatThousand = function () {
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
                ShowModalAlert(result.Msg);
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
                ShowModalAlert(result.Msg);
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

function ShowModalAlert(dataHtml) {
    $('#modalAlert').modal({ backdrop: 'static', keyboard: false });
    $('#modalAlert .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalAlert .modal-dialog .modal-header center .modal-title strong').html("Atenção");
    $('#modalAlert .modal-dialog .modal-body .alert').html("");
    $('#modalAlert .modal-dialog .modal-body .alert').html(dataHtml);
}

function ShowModalSucess(dataHtml) {
    $('#modalSuccess').modal({ backdrop: 'static', keyboard: false });
    $('#modalSuccess .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalSuccess .modal-dialog .modal-header center .modal-title strong').html("Sucesso");
    $('#modalSuccess .modal-dialog .modal-body .alert').html("");
    $('#modalSuccess .modal-dialog .modal-body .alert').html(dataHtml);
}

jQuery.extend({
    compare: function (arrayA, arrayB) {
        if (arrayA.length !== arrayB.length) { return false; }
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

function CNPJCheck(c) {
    var b = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    if ((c = c.replace(/[^\d]/g, "")).length !== 14)
        return false;

    if (/0{14}/.test(c))
        return false;

    for (var i = 0, n = 0; i < 12; n += c[i] * b[++i]);
    if (c[12] !== ((n %= 11) < 2 ? 0 : 11 - n))
        return false;

    for (var i = 0, n = 0; i <= 12; n += c[i] * b[i++]);
    if (c[13] !== ((n %= 11) < 2 ? 0 : 11 - n))
        return false;

    return true;
}

function LoadCombo(url, comboNames, idField, textField, selectFirstItem, defaultValue) {
    var dsData = undefined;

    $.ajax({
        url: url,
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            if (result.Success)
                dsData = result.Data;
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });

    jQuery.each(comboNames, function (i, comboName) {
        $(comboName).kendoDropDownList({
            dataTextField: textField,
            dataValueField: idField,            
            dataSource: dsData,
            optionLabel: defaultValue
        });

        if (selectFirstItem)
            $(comboName).data("kendoDropDownList").select(0);
    });
}

function LoadComboFiltered(url, comboName, idField, textField, valueFilter, selectFirstItem, defaultValue) {    
    $(comboName).kendoDropDownList({        
        dataTextField: textField,
        dataValueField: idField,     
        optionLabel: defaultValue,
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: url,
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            filter: valueFilter
                        };
                    }
                }
            }
        }
    });

    if (selectFirstItem)
        $(comboName).data("kendoDropDownList").select(0);
}