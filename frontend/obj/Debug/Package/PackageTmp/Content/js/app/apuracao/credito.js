var regra;

function BuscarDadosGrid(obj) {
    var matrizList = $('#txtCodMatriz').data("kendoDropDownList");
    var idMatriz = matrizList.dataItem().Id;

    $("#grid").html("");
    $("#grid").removeAttr("class");
    $("#grid").removeAttr("data-role");

    $("#grid").kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: 'RegrasCredito_' + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + '.xlsx',
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
        dataSource:{
            transport: {
                read: {
                    url: "/Apuracao/GetRules",
                    dataType: "json",
                    type: "POST",
                    async: false,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            IdEmpresa: idMatriz,
                            tipoApuracao: 1
                        }
                    }
                }
            },
            group: { field: "MosaicDescricao" },
            sort: { field: "TerminoVigencia", dir: "asc" },
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
        groupable: true,
        dataBound: gridDataBound,
        pageable: {
            refresh: true,
            pageSizes: true,
            buttonCount: 10
        },
        columns:
        [
             { field: "IdVigencia", hidden: true },
             { title: "Código", field: "Codigo", width: "55px" },
             { title: "Descrição", field: "MosaicDescricao" },
             { title: "Início Vigência", field: "InicioVigencia", width: "120px", template: "#= kendo.toString(kendo.parseDate(InicioVigencia, 'yyyy-MM-dd'), 'dd/MM/yyyy') #" },
             { title: "Término Vigência", field: "TerminoVigencia", width: "120px", template: "#= (TerminoVigencia != null) ? kendo.toString(kendo.parseDate(TerminoVigencia, 'yyyy-MM-dd'), 'dd/MM/yyyy') : '' #" },
             { title: "Ativa", field: "Ativa", width: "45px", attributes: { style: "text-align: center" }, template: "<input type='checkbox' #= Ativa ? checked='checked':'' # class='chkbx' />" },
             { title: " ", template: "<a onclick='javascript: { regra = undefined; EditRegra(this); }' class='k-button'><span title='Editar Regra' class='glyphicon glyphicon-pencil'></span></a>", width: "48px", filterable: false }
        ]
    });

    var grid = $("#grid").data('kendoGrid');
    grid.tbody.on('click', '.chkbx', function () {
        var checked = $(this).is(':checked');
        var grid = $('#grid').data().kendoGrid;
        var dataItem = grid.dataItem($(this).closest('tr'));

        $.ajax({
            url: '/Apuracao/AtivarRegra',
            data: JSON.stringify({ idRegra: dataItem.Id, ativa: checked }),
            type: 'POST',
            async: false,
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result.Sucesso) {
                    $('#grid').data('kendoGrid').dataSource.read();
                    $('#grid').data('kendoGrid').refresh();
                    ShowModalSucess(result.Msg);
                }
                else
                    SShowModalAlert"Erro: " + result.Msg);
            }
        });
    });
}

function gridDataBound(e) {
    var grid = e.sender;
    if (grid.dataSource.total() == 0) {
        var colCount = grid.columns.length;
        $(e.sender.wrapper)
            .find('tbody')
            .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" class="no-data">Nenhuma Apuração de Crédito Registrada</td></tr>');
    }
}

function EditRegra(e) {
    if (regra == undefined)
        regra = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);

    $('#txtIdRegra').val(regra.Id);
    $('#txtCodRegra').val(regra.Codigo);

    $('#txtIdVigencia').val(regra.IdVigencia);

    $('#txtCodMatrizEdit').val($('#txtCodMatriz').val());
    $('#txtDescMatriz').val($('#txtRazaoSocial').val());

    $('#txtTipoImpostoEdit').val('01 / 02');    
    $('#txtDescTipoImposto').val('PIS / COFINS');
  
    $('#txtNaturezaReceita').val(regra.NaturezaCodigo);
    $('#txtDescNaturezaReceita').val(regra.NaturezaDescricao.trim());

    $('#txtTipoMosaic').val(regra.MosaicTipo);
    $('#txtDescTipoMosaic').val(regra.MosaicDescricao.trim());

    if (regra.TerminoVigencia != null)
        $('#btFecharVigencia').hide();
    else
        $('#btFecharVigencia').show();

    $('#tbVigenciaInicial').val(kendo.toString(kendo.parseDate(regra.InicioVigencia, 'yyyy-MM-dd'), 'dd/MM/yyyy'));
    $('#tbVigenciaFinal').val(kendo.toString(kendo.parseDate(regra.TerminoVigencia, 'yyyy-MM-dd'), 'dd/MM/yyyy'));
    $('#tbJustificativa').val(regra.Justificativa);

    BuscarDadosGridEdit(regra.Id, regra.IdVigencia);
    $('#modalViewCredito').modal({ backdrop: 'static', keyboard: false });
    $('#modalViewCredito').modal('show');    
}

function ShowPopupVigenciaCredito() {
    var dataFinal;

    var split = $("#tbVigenciaInicial").val().split("/");
    var dataInicial = new Date(split[2], split[1] - 1, split[0]);

    if (dataInicial < new Date())
        dataFinal = new Date();
    else
        dataFinal = dataInicial;

    $("#tbVigenciaFinalRegra").kendoDatePicker({
        start: "year",
        depth: "year",
        format: "MMMM yyyy",
        value: dataFinal
    });

    $("#tbVigenciaInicialRegra").val($("#tbVigenciaInicial").val())
    var tbVigenciaFinalRegra = $("#tbVigenciaFinalRegra").data("kendoDatePicker");

    $('#selectRetroativo').bootstrapSwitch();
    $('#selectRetroativo').on('switchChange.bootstrapSwitch', function (event, retroativo) {
        if (retroativo) {
            $('#lbVigenciaNaoRetroativa').html('');
            $("#tbVigenciaFinalRegra").data("kendoDatePicker").value(dataFinal);
            tbVigenciaFinalRegra.enable(true);
            $("#divJustificativa").show();
        }
        else {
            $('#lbVigenciaNaoRetroativa').html('<br/>Para as vigências não retroativas, a data de término da vigência deverá ser no mês corrente.');
            $("#tbVigenciaFinalRegra").data("kendoDatePicker").value(new Date());
            tbVigenciaFinalRegra.enable(false);
            $("#divJustificativa").hide();
        }
    });

    $('#txtIdRegraVigencia').val($('#txtIdRegra').val());    
    $('#tbJustificativaVigencia').val('');
    $('#lbMsgErro').html('');

    $('#modalVigenciaCredito .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalVigenciaCredito .modal-dialog .modal-header center .modal-title strong').html("Vigência regra: " + $('#txtDescTipoMosaic').val());
    $('#modalVigenciaCredito').modal({ backdrop: 'static', keyboard: false });
    $('#modalVigenciaCredito').modal('show');
}

function CriarNovaVigenciaCredito() {
    var msgErro = CriarNovaVigencia();
       
    if (msgErro == '') {        
        $('#modalVigenciaCredito').data('data', null);
        $('#modalVigenciaCredito').modal("hide");

        $("#grid").data().kendoGrid.dataSource.read();

        var dsGrid = $("#grid").data().kendoGrid.dataSource.data();
        
        for (var i in dsGrid) {
            if (dsGrid[i].Id == regra.Id) {
                regra = dsGrid[i];
                break;
            }
        }

        EditRegra(regra);
    }

    $('#lbMsgErro').html(msgErro);
}

//function ShowPopupVigenciaCreditoLog() {
//    CarregarGridVigenciaLog();

//    $('#modalVigenciaCreditoLog .modal-dialog .modal-header center .modal-title strong').html("");
//    $('#modalVigenciaCreditoLog .modal-dialog .modal-header center .modal-title strong').html("Histórico de vigências da regra: " + $('#txtDescTipoMosaic').val());
//    $('#modalVigenciaCreditoLog').modal({ backdrop: 'static', keyboard: false });
//    $('#modalVigenciaCreditoLog').modal('show');
//}