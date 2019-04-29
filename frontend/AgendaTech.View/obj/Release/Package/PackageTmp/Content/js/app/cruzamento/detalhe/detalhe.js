var grid;
var gridData;
var gridColumns = [];
var totaisGrid = [];
var rodapeTotais = false;
var idRegra = '';
var nomeGrid = 'RegraCruzamentoSPED';

$(document).ready(function () {
    InitializeComponents();
});

function InitializeComponents() {
    PreencherCamposDetalhe();
    gridDs_load();
}

var GridColumn = function (col) {
    this.field = col.Coluna;
    this.title = col.Rotulo;
    this.width = '150px';

    if (rodapeTotais == false && totaisGrid.length > 0) {
        rodapeTotais = true;
        this.footerTemplate = "<div> TOTAL: </div>";
    }

    if (col.TipoValor == 'decimal') {
        this.attributes = { style: "text-align:right;" };
        this.footerTemplate = "<div style='float: right'>" + GetTotal(col.Rotulo.toUpperCase()) + "</div>";
    }
}

function GetTotal(colunaGrid) {
    var totalRodape = 0;
    totaisGrid.forEach(function (total) {
        if (total.Campo.toUpperCase() == colunaGrid) {
            totalRodape = total.Total;
            return false;
        }           
    });

    return totalRodape.FormatarMoeda(2, "", ".", ",");
}

function PreencherCamposDetalhe() { 
    $("#lbRegra").text($("#hddRegra").val());
    $("#lbDescricao").text($("#hddRegraOcorrencia").val());
    $("#lbOcorrencias").text($("#hddNumOcorrencia").val());
}

function gridDs_load() {
    var idControle = $("#hddIdControle").val();
    var idCruzamento = $("#hddIdCruzamento").val();
    idRegra = $("#hddIdRegra").val();

    var url = "/EFD/Detalhe/Get/?idControle=" + idControle + '&idCruzamento=' + idCruzamento + '&idRegra=' + idRegra + '&nomeGrid=' + nomeGrid;

    $.ajax({
        url: url,
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            gridData = result.data;
            totaisGrid = result.totais;
            rodapeTotais = false;
            setGridColumns(result.columns);
            grid_load();
        }
    });
}

function setGridColumns(cols) {
    cols.forEach(function (col) {

        if (col != null) {
            var newCol = new GridColumn(col);
            gridColumns.push(newCol);
        }

    });
}

function grid_load() {
    $("#grid").html("");

    $("#loading-page").show();

    $("#grid").kendoGrid({       
        toolbar: ["excel"],
        excel: {
            fileName: 'DetalhesSPED_' + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + '.xlsx',
            allPages: true
        },
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                var sheet = e.workbook.sheets[0];
                for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                    var row = sheet.rows[rowIndex];

                    // Tratando linha de totalização
                    if (row.type == "footer") {
                        for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                            if (row.cells[cellIndex].value != undefined) {
                                var valor = RemoveTagsTotalizacao(row.cells[cellIndex].value)
                                row.cells[cellIndex].value = kendo.toString(valor);
                            }
                        }
                    }
                    if (rowIndex % 2 == 0) {
                        var row = sheet.rows[rowIndex];
                        for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                            row.cells[cellIndex].background = "#aabbcc";
                        }
                    }
                }
            }
        },
        dataSource: {
            data: gridData,            
            pageSize: 10
        },
        scrollable: true,
        sortable: false,
        filterable: true,
        resizable: true,
        groupable: false,
        reorderable: true,
        columnReorder: onReorder,
        columnMenu: false,
        cache: false,        
        pageable: {
            pageSizes: [10, 25, 50, 100]
        },
        columns: gridColumns,
        dataBound: function () {
            if (gridData.length > 0 && gridData[0].Template !== undefined) {
                var template;

                dataView = this.dataSource.view();
                for (var i = 0; i < dataView.length; i++) {
                    var uid = dataView[i].uid;
                    template = "<div><input type='button' value='Detalhe' onclick='getDetalhe(\"" + uid + "\")' /></div>";
                    var html = $("#grid tbody").find("tr[data-uid=" + uid + "]").html();
                    $("#grid tbody").find("tr[data-uid=" + uid + "] td:last").html(template);
                }
            }
        }
    });

    $("#loading-page").hide();
}

function onReorder(e) {
    var that = this;
    setTimeout(function () {
        GravarOrdemColunas(that.columns);
    });
}

function GravarOrdemColunas(gridColunas) {    
    var coluna = [];
    var lstColunas = [];    

    gridColunas.forEach(function (colunaGrid) {     
        coluna = {
            Titulo: colunaGrid.title,
            Campo: colunaGrid.field,
            Visivel: 1,
            Travada: 0    
        };

        lstColunas.push(coluna);        
    });

    $.ajax({
        url: '/EFD/Detalhe/AtualizarSequenciaColunas',
        data: JSON.stringify({ idRegra: idRegra, nomeGrid: nomeGrid, lstColunas: lstColunas }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Sucesso == false)
                ShowModalAlert(result.Msg);
        }
    });    
}

function getDetalhe(uid) {
    $("#loading-page").show();
    var paramValueArray = [];
    var row = $("#grid tbody").find("tr[data-uid='" + uid + "']")
    var dataItem = grid.dataItem(row);

    dataItem.forEach(function (e) {
        paramValueArray.push(e);
    });

    var idCruzamento = $("#hddIdCruzamento").val();
    var idRegra = $("#hddIdRegra").val();
    var data = {
        idCruzamento: idCruzamento, 
        idRegra: idRegra,
        paramValueArray: JSON.stringify(paramValueArray)
    };

    var url = "/EFD/Detalhe/GetDetalhe/" //?idCruzamento=" + idCruzamento + '&idRegra=' + idRegra + '&paramValueArray=' + paramValueArray;

    $.ajax({
        url: url,
        type: "GET",
        async: false,
        dataType: "json",
        data: data,
        cache: false,
        success: function (result) {
            showDetalhe(result);
        }
    });
    $("#loading-page").hide();
}

function showDetalhe(vm) {
    var modal = '#modalDetalhe';

    // Cabeçalho
    var detCaptionHtml = '';
    var detValueHtml = '';

    for (var prop in vm.header) {
        detCaptionHtml = detCaptionHtml + '<div class="col-md-3"><span class="regra">' + prop + '</span></div>';
        detValueHtml = detValueHtml + '<div class="col-md-3"><span class="regra regraValue">' + vm.header[prop] + '</span></div>';
    }
    
    $(modal + ' .regraHeader #detCaption').html('');
    $(modal + ' .regraHeader #detText').html('');
    $(modal + ' .regraHeader #detCaption').html(detCaptionHtml);
    $(modal + ' .regraHeader #detText').html(detValueHtml);
    
    // Grid
    $(modal + " #grid").html("");
    var gridDet = $(modal + " #grid").kendoGrid({
        dataSource: {
            data: vm.data,
            pageSize: 10          
        },
        scrollable: true,
        sortable: true,
        filterable: true,
        resizable: true,
        groupable: false,
        pageable: {
            pageSizes: [10, 25, 50]
        },
        columns: [
            { field: 'NomeRegra', title: 'TIPO DO ERRO' },
            { field: 'Mensagem', title: 'DESCRIÇÃO'}
        ]
    }).data("kendoGrid");
    
    // Show Modal
    $(modal).modal({ backdrop: 'static', keyboard: false });
    $(modal + ' .modal-dialog .modal-header center .modal-title strong').html("");
    $(modal + ' .modal-dialog .modal-header center .modal-title strong').html("Detalhamento de Nota");    
}