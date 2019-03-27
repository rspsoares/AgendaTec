function CarregarComponentes() {
    //CarregarGridImportacao();
    //CarregarGridHistorico();
    //CarregarGridSaldos();    

    //$('#btnImportar').click(function () {
    //    VerificarArquivosImportacoes();
    //});   
}

function CarregarGridSaldos() {
    var dsSaldos = [];
    var objSaldos = [];
    var lstCred = [];
    var lstUtil = [];

    $.ajax({
        url: "/Home/PesquisarSaldos",
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            if (result.Sucesso)
                dsSaldos = result.Data;
            else
                ShowModalAlerta(result.Msg);
        }
    });   

    $.each(dsSaldos.lstCreditos, function (index, credito) {
        lstCred.push([
            credito.Periodo,
            credito.Valor
        ]);
    });

    $.each(dsSaldos.lstUtilizados, function (index, utilizado) {
        lstUtil.push([
            utilizado.Periodo,
            utilizado.Valor
        ]);
    });
    
    $.plot($("#flot-dashboard-chart"), [
    {
        label: "Saldo Acumulado",
        data: lstCred,
        bars: {
            show: true,
            barWidth: 0.2,
            align: "left"
        }
    },
    {
        label: "Crédito Utilizado",
        data: lstUtil,
        bars: {
            show: true,
            barWidth: 0.2,
            align: "right"
        }
    }],
    {
        legend: {
            noColumns: 2,
            container: $("#chartLegend")
        },
        xaxis: {
            mode: "categories",            
            min: -0.5,
            max: lstCred.length - 0.5
        },
        grid: {
            hoverable: true
        },
        yaxis: {
            allowDecimals: true,
            tickFormatter: function (v) {
                return FormatarValorGrafico(v);              
            }
        }
    }); 

    $("#flot-dashboard-chart").bind("plothover", function (event, pos, item) {
        $("#tooltip").remove();
        if (item) {
            var tooltip = 
                'Mês: ' + item.series.data[item.dataIndex][0] + '<br/>' +
                'Valor: R$ ' + item.series.data[item.dataIndex][1].FormatarMoeda(2, '', '.', ',');

            $('<div id="tooltip">' + tooltip + '</div>')
                .css({
                    position: 'absolute',
                    display: 'none',
                    top: item.pageY - 40,
                    left: item.pageX - 120,
                    border: '2px solid #fdd',
                    padding: '3px',
                    'font-size': '16px',
                    'border-radius': '5px',
                    'background-color': '#fff',                
                    opacity: 0.80
                })
                .appendTo("body").fadeIn(200);
        }
    });
}

function CarregarGridImportacao() {
    $('#gridImportacao').html("");
    $('#gridImportacao').kendoGrid({
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                }, 
                model: {
                    id: "Id",
                    fields: {
                        CodigoMatriz: { type: "text", validation: { required: false } },
                        NomeEmpresa: { type: "text", validation: { required: false } },
                        Modelo: { type: "text", validation: { required: false } },
                        Periodo: { type: "text", validation: { required: false } },
                        NomeArquivo: { type: "text", validation: { required: false } },
                        Etapa: { type: "text", validation: { required: false } },
                        Sucesso: { type: "text", validation: { required: false } },
                        dhAtualizacao: { type: "date", validation: { required: false } }
                    }
                }
            },
            transport: {
                read: {
                    url: "/Home/PesquisarImportacoes",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                }
            },
            pageSize: 5
        },
        filterable: false,
        groupable: true,
        scrollable: false,
        pageable: {
            pageSizes: [5, 10, 25, 50]
        },
        dataBound: function(e) {
            var grid = $("#gridImportacao").data("kendoGrid");

            $("#gridImportacao tbody tr").each(function () {
                var currentRowData = grid.dataItem(this);                
                if (currentRowData.Sucesso == 'FALHA') {
                    $(this).css("color", "red")
                }
            });
        },
        columns: [
            { field: "Id", hidden: true },            
            { field: "CodigoMatriz", headerTemplate: "<strong>Código da Matriz</strong>", width: "130px" },
            { field: "NomeEmpresa", headerTemplate: "<strong>Nome da Empresa</strong>", width: "210px" },
            { field: "Modelo", headerTemplate: "<strong>Modelo</strong>", width: "100px" },
            { field: "Periodo", headerTemplate: "<strong>Período</strong>", width: "100px" },
            { field: "NomeArquivo", headerTemplate: "<strong>Nome do Arquivo</strong>", width: "240px" },
            { field: "Etapa", headerTemplate: "<strong>Etapa</strong>", width: "130px", template: "<a onclick='javascript:{ExibirOcorrenciaImportacao(this);}' style='cursor: pointer;'>#=Etapa#</a>" },
            { field: "Sucesso", headerTemplate: "<strong>Status</strong>", width: "70px" },
            { field: "dhAtualizacao", headerTemplate: "<strong>Data Atualização</strong>", template: "#= kendo.toString(kendo.parseDate(dhAtualizacao, 'yyyy-MM-dd HH:mm:ss'), 'dd/MM/yyyy HH:mm:ss') #" }
        ]
    });

    RefreshImportacao();
}

function RefreshImportacao() {
    setInterval(function () {
        var now = new Date();
        var dataAgora = kendo.toString(kendo.parseDate(now, 'HH:mm:ss'), 'HH:mm:ss');
        $('#dhImportacao').text('Última atualização realizada às ' + dataAgora);
        $('#gridImportacao').data('kendoGrid').dataSource.read();
    }, 60000);
}

function ExibirOcorrenciaImportacao(e) {
    var dataHtml = '';
    var titulo = '';
    var dataItem = $("#gridImportacao").data("kendoGrid").dataItem(e.parentElement.parentElement);

    if (dataItem.NomeArquivo != null)
        titulo = "Ocorrências da Importação - Arquivo: " + dataItem.NomeArquivo
    else
        titulo = "Ocorrências da Importação";

    $('#modalOcorrenciaImportacao').modal({ backdrop: 'static', keyboard: false });
    $('#modalOcorrenciaImportacao .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalOcorrenciaImportacao .modal-dialog .modal-header center .modal-title strong').html(titulo);
    $('#modalOcorrenciaImportacao .modal-dialog .modal-body').html(CarregarOcorrencias(dataItem));
}

function CarregarOcorrencias(dataItem) {
    CarregarGridOcorrencias(dataItem);
    CarregarGridAjustes(dataItem);
}

function CarregarGridOcorrencias(e) {
    $('#gridOcorrencia').html("");
    $('#gridOcorrencia').kendoGrid({
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                },
                model: {
                    id: "Id",
                    fields: {
                        Ocorrencia: { type: "text", validation: { required: true } },
                        Sucesso: { type: "text", validation: { required: false } },
                        dhOcorrencia: { type: "date", validation: { required: true } }
                    }
                }
            },
            transport: {
                read: {
                    url: "/Home/PesquisarOcorrencias?idLog=" + e.Id,
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                }
            },
            pageSize: 10
        },
        filterable: false,
        groupable: false,
        scrollable: false,
        pageable: {
            pageSizes: [10, 25, 50]
        },
        columns: [            
            { field: "Ocorrencia", title: "Ocorrência", width: "70%" },
            { field: "Sucesso", title: "Status", width: "15%" },
            { field: "dhOcorrencia", title: "Data Atualização", template: "#= kendo.toString(kendo.parseDate(dhOcorrencia, 'yyyy-MM-dd HH:mm:ss'), 'dd/MM/yyyy HH:mm:ss') #", width: "15%" }
        ]
    });
}

function CarregarGridAjustes(e) {
    $('#gridAjustes').html("");
    $('#gridAjustes').kendoGrid({
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                },
                model: {
                    id: "IdTNALogPropagacao",
                    fields: {
                        Docnum: { type: "text", validation: { required: true } },
                        Campo: { type: "text", validation: { required: false } },
                        ValorOriginal: { type: "text", validation: { required: false } },
                        ValorAlterado: { type: "text", validation: { required: false } }                        
                    }
                }
            },
            transport: {
                read: {
                    url: "/Home/PesquisarOcorrenciasAjustes?idLog=" + e.Id,
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                }
            },
            pageSize: 5
        },        
        groupable: true,
        sortable: true,
        scrollable: false,
        pageable: {
            pageSizes: [5, 10, 25, 50]
        },
        columns: [
            { field: "Docnum", title: "Docnum"},
            { field: "Campo", title: "Campo"},
            { field: "ValorOriginal", title: "Valor Original" },
            { field: "ValorAlterado", title: "Valor Alterado"}
        ]
    });
}

/*GRID - Painel de Histórico/Ocorrências de Sistema*/
function CarregarGridHistorico() {   
    $('#gridHistorico').html("");
    $('#gridHistorico').kendoGrid({
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                },
                model: {
                    id: "Id",
                    fields: {
                        Evento: { type: "text", validation: { required: true } },
                        Descricao: { type: "text", validation: { required: false } },
                        DhOcorrencia: { type: "date", validation: { required: true } }
                    }
                }
            },
            transport: {
                read: {
                    url: "/Home/RecuperarHistorico",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                }
            },
            pageSize: 5
        },
        filterable: false,
        groupable: true,
        scrollable: false,
        pageable: {
            pageSizes: [5,10, 25, 50]
        },        
        columns: [
            { field: "Id", hidden: true },
            { field: "Evento", title: "Evento", width: "10%" },
            { field: "Descricao", title: "Descrição", width: "60%" },
            { field: "DhOcorrencia", title: "Ocorrência", template: "#= kendo.toString(kendo.parseDate(DhOcorrencia, 'yyyy-MM-dd HH:mm:ss'), 'dd/MM/yyyy HH:mm:ss') #", width: "30%" }
        ]
    });   

    RefreshHistorico();
}

/*TIMER - Mantém o Histórico Atualizado*/
function RefreshHistorico() {
    setInterval(function () {
        var now = new Date();
        var dataAgora = kendo.toString(kendo.parseDate(now, 'HH:mm:ss'), 'HH:mm:ss');
        $('#dhHistorico').text('Última atualização realizada às ' + dataAgora);
        $('#gridHistorico').data('kendoGrid').dataSource.read();
    },60000);
}

function VerificarArquivosImportacoes() {
    $.ajax({
        url: "/Home/VerificarArquivoImportacoes",
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            if (result.Sucesso)
                ShowModalSucesso(result.Msg);
            else
                ShowModalAlerta(result.Msg);
        }
    });
}