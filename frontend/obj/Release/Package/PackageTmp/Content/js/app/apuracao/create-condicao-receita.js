﻿function CFOPReceita() {
    var cfopList = $("#txtCFOP").kendoMultiSelect({
        itemTemplate: kendo.template('<span class="CFOP">#= CFOP # -</span> #= Descricao #'),
        optionLabel: "Selecione um CFOP...",
        dataTextField: "CFOP",
        dataValueField: "CFOP",
        dataSource: GetDsCFOP(),
        change: onChangeCFOP,
        select: onSelectCFOP        
    }).data("kendoMultiSelect");
}

function onSelectCFOP(e) {
   
}

function onChangeCFOP(e) {
    $('#descCFOP').text("");   
}

function GetDsCFOP() {
    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/Apuracao/GetCFOPReceita",
                type: "GET",
                dataType: "json",
                cache: false
            }
        },
        model: {
            id: "Id",
            fields: {
                CFOP: { editable: false },
                Descricao: { editable: false }
            }
        }
    });
    return dataSource;
}

function CSTReceita() {
    var cfopList = $("#txtCST").kendoDropDownList({
        optionLabel: "Selecione um CST...",
        dataTextField: "Codigo",
        dataValueField: "Codigo",
        dataSource: GetDsCST()
    }).data("kendoDropDownList");
}

function GetDsCST() {
    var dataSource = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/Apuracao/GetCSTReceita",
                type: "GET",
                dataType: "json",
                cache: false
            }
        },
        model: {
            id: "Id",
            fields: {
                Codigo: { editable: false },
                Descricao: { editable: false }
            }
        }
    });
    return dataSource;
}

function SalvarCondicao(e) {
   // $("#loading-page").show();
    var obj = [];

    var cstSelect = $("#txtCST").data("kendoDropDownList").dataItem();  //.value().length

    var consiste = $('#txtCatNota').val() + $('#txtGrupoMaterial').val() + $('#txtMaterial').val();
    consiste += $("#txtCST").data("kendoDropDownList").select() == 0 ? '' : '1';
    consiste += $("#txtCFOP").data("kendoMultiSelect").value().length == 0 ? '' : '1';
    
     if (consiste.trim() !='') {
        $('#txtCST').parent().parent().children('label').attr('style', 'color: #222222;');
        var cst = "";
        cst = cstSelect.Codigo;
        if (cst.substring(0, 3) == 'Sel') {
            cst = "";
        }

        var cfopSelect = $("#txtCFOP").data("kendoMultiSelect").dataItems();
        var cfop = "";
        for (var i in cfopSelect) {
            if (cfop == "")
                cfop += cfopSelect[i].CFOP;
            else
                cfop += "/" + cfopSelect[i].CFOP;
        }
        if (cfop.substring(0, 3) == 'Sel') {
            cfop = "";
        }
        var cfopRefSelect = $("#txtCFOPReferencia").data("kendoMultiSelect").dataItems();
        var cfopRef = "";
        for (var i in cfopRefSelect) {
            if (cfopRef == "")
                cfopRef += cfopRefSelect[i].Codigo;
            else
                cfopRef += "/" + cfopRefSelect[i].Codigo;
        }

        var idRegra = $('#txtIdRegra').val();
        var idVigencia = $('#txtIdVigencia').val();
        var idCondicao = $('#txtIdCondicao').val();
        var urlService = "";
        var msgCond = "";
        if (idCondicao == "") {
            urlService = "/Apuracao/CreateCondicao";
            obj.push({
                IdRegra: idRegra,
                IdRegraVigencia: idVigencia,
                CST: cst,
                CFOP: cfop,
                CFOPRef: cfopRef,
                CategoriaNota: $('#txtCatNota').val(),
                GrupoMaterial: $('#txtGrupoMaterial').val(),
                Material: $('#txtMaterial').val()
            });
            msgCond = "Criada";
        }
        else {
            urlService = "/Apuracao/EditCondicao";
            obj.push({
                Id: idCondicao,
                IdRegra: idRegra,
                IdRegraVigencia: idVigencia,
                CST: cst,
                CFOP: cfop,
                CFOPRef: cfopRef,
                CategoriaNota: $('#txtCatNota').val(),
                GrupoMaterial: $('#txtGrupoMaterial').val(),
                Material: $('#txtMaterial').val()
            });
            msgCond = "Alterada";
        }        

        $.ajax({
            url: urlService,
            data: JSON.stringify(obj[0]),
            type: 'POST',
            async: false,
            contentType: 'application/json; charset=utf-8',
             success: function (result) {
                if (result.Sucesso) {
                    CloseModalParam(e);
                    ShowModalSucess(result.Msg);
                }
                else {
                    SShowModalAlertresult.Msg);
                }
            }
        });
    }
    else {      
         var msg = "<p>Preencha os pelo menos um dos Campos Obrigatórios.</p>";
         SShowModalAlertmsg);
    }
}

function CloseModalParam(e) {
    $("#loading-page").show();
    $('#' + e.parentElement.parentElement.parentElement.parentElement.id).modal("hide");

    var idRegra = $('#txtIdRegra').val();
    var idVigencia = $('#txtIdVigencia').val();
    BuscarDadosGridEdit(idRegra, idVigencia);

    $('#modalViewReceita').modal('show');
    $("#loading-page").hide();
}