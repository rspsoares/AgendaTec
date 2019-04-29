/*CONSTRUTOR SCRIPT*/
function CarregaComponentes() {
    CarregarMatrizesFiltros();
    CarregarTiposRelatorioFiltros();
    $('#ddlPeriodo').kendoDropDownList({
        enable: false
    });

    $('#tipoExportacao').bootstrapSwitch();
    CarregarGRIDTarefas();
}

/*CRIAÇÃO - MATRIZ*/
function CarregarMatrizesFiltros() {
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
        ShowModalAlert("Não foi possível obter as Matrizes.");
        return;
    }

    $('#txtCodMatriz').kendoDropDownList({
        dataTextField: "CodigoMatriz",
        dataValueField: "Id",
        dataSource: dsMatriz,
        optionLabel: "Escolha ...",
        select: function (e) {
            var dataItem = this.dataItem(e.item);
            $('#txtRazaoSocial').val(dataItem.RazaoSocial);
            idMatriz = dataItem.Id;
            var dropdownlist = $("#ddlPeriodo").data("kendoDropDownList");

            if (idMatriz == "") {
                dropdownlist.enable(false);
                dropdownlist.select(0);
            }
            else {
                dropdownlist.enable();
                CarregarPeriodosFiltros(idMatriz);
            }
        }
    });
}

/*CRIAÇÃO - TIPOS*/
function CarregarTiposRelatorioFiltros() {
    $('#ddlTipo').kendoDropDownList({
        optionLabel: "Escolha o Tipo da Exportação ...",
        dataTextField: "texto",
        dataValueField: "valor",
        dataSource: BuscarTIPOS()
    });
}

/*CRIAÇÃO - PERIODOS*/
function CarregarPeriodosFiltros(idMatriz) {
    var dsPeriodo = undefined;
    $.ajax({
        url: "/DocumentosFiscais/RecuperarPeriodos?idmatriz=" + idMatriz,
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            dsPeriodo = result.Data;
        }
    });

    if (dsPeriodo == undefined) {
        ShowModalAlert("Nenhum período configurado.");
        return;
    }

    $('#ddlPeriodo').kendoDropDownList({
        dataTextField: "Periodo",
        dataValueField: "Id",
        dataSource: dsPeriodo,
        enable: true,
        optionLabel: "Escolha o período ...",
        select: function (e) {
            var dataItem = this.dataItem(e.item);
            $('#ddlPeriodo').val(dataItem.Periodo);
        }
    });
}

/*AÇÕES - REQUISITAR EXPORTAÇÃO DE DADOS*/
function AdicionarRequisicao() {
    var ddlPeriodo = $("#ddlPeriodo").data("kendoDropDownList");

    if (ddlPeriodo.value() == 0) {
        ShowModalAlert('Um período deve ser selecionado.');
        return;
    }

    var ddlTipo = $("#ddlTipo").data("kendoDropDownList");

    if (ddlTipo.value() == 0) {
        ShowModalAlert('O Tipo da Exportação deve ser selecionado.');
        return;
    }

    var nomeTarefa = $('#txtNomeTarefa').val();

    if (nomeTarefa == "") {
        ShowModalAlert('O Nome da Tarefa deve ser preenchido.');
        return;
    }

    var parametrizado = $("#tipoExportacao").is(":checked");

    $.ajax({
        url: "/Jobs/AdicionarJob",
        type: "POST",
        data: JSON.stringify({ idControle: ddlPeriodo.value(), idTipo: ddlTipo.value(), nome: nomeTarefa, parametrizado: parametrizado }),
        async: false,
        contentType: "application/json",
        success: function (result) {
            if (result.Sucesso)
                ShowModalSucess('Tarefa adicionada.');
            else
                SShowModalAlert'Falha na adição de Tarefa.');
        }
    });

    $('#gridTarefas').data('kendoGrid').dataSource.read();
}

/*AÇÕES - RECUPERAR EXCEL*/
function RecuperarExcel(IDJob) {
    window.location.href = '/Jobs/BaixarExcel?idjob=' + IDJob;
}

/*AÇÕES - REFRESH DE GRID*/
function AtualizarGrid() {
    $('#gridTarefas').data('kendoGrid').dataSource.read();
}

/*GRID - CARREGAR TAREFAS*/
function CarregarGRIDTarefas() {
    $("#gridTarefas").html("");
    $("#gridTarefas").kendoGrid({
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                },
                model: {
                    id: "IDJob",
                    fields: {
                        Tipo: { validation: { required: true }, nullable: true },
                        RequestDate: { type: "date", validation: { required: true } },
                        StartDate: { type: "date", validation: { required: false } },
                        FinishDate: { type: "date", validation: { required: false } },
                        Status: { validation: { required: true } },
                        Details: { validation: { required: false } }
                    }
                }
            },
            transport: {
                read: {
                    url: "/Jobs/BuscarTarefas",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                }
            },
            pageSize: 10
        },
        scrollable: true,
        sortable: true,
        resizable: true,
        filterable: true,
        pageable: {
            pageSizes: [10, 25, 50, 100, 500]
        },
        toolbar: kendo.template($("#templateToolbar").html()),
        columns: [
            { field: "IDJob", hidden: true },
            {
                field: "IDJob",
                title: "Relatórios",
                width: "100px",
                filterable: false,
                template: "<button class='k-button' onclick='javascript:{RecuperarExcel(#=IDJob#);}' #= (Status != 'Pronto para Baixar') ? 'disabled' : '' #>Baixar</button>",
                attributes: { style: "text-align:center;" }
            },
            { field: "Type", title: "Tipo", width: "100px" },
            { field: "Parametrizado", title: "Parametrizado", template: "#= (Parametrizado) ? 'Sim' : 'Não' #", width: "120px" },
            { field: "RequestDate", title: "Requisitado", template: "#= kendo.toString(kendo.parseDate(RequestDate, 'yyyy-MM-dd HH:mm:ss'), 'dd/MM/yyyy HH:mm:ss') #", width: "120px" },
            { field: "StartDate", title: "Início Geração", template: "#= (StartDate == null) ? '-' : kendo.toString(kendo.parseDate(StartDate, 'yyyy-MM-dd HH:mm:ss'), 'dd/MM/yyyy HH:mm:ss') #", width: "120px" },
            { field: "FinishDate", title: "Final Geração", template: "#= (FinishDate == null) ? '-' : kendo.toString(kendo.parseDate(FinishDate, 'yyyy-MM-dd HH:mm:ss'), 'dd/MM/yyyy HH:mm:ss') #", width: "120px" },
            { field: "Status", title: "Status", width: "80px" },
            { field: "JobName", title: "Nome da Tarefa", width: "120px" },
            { field: "Details", title: "Detalhes" }            
        ]
    });
    AtualizarTarefas();
}

/*EVENTOS - RECARREGAR GRID*/
function AtualizarTarefas() {
    setInterval(function () {
        var now = new Date();
        $('#gridTarefas').data('kendoGrid').dataSource.read();
    }, 10000);
}

/*DATASOURCE - TIPOS*/
function BuscarTIPOS() {
    var dsTipos = [
    { texto: '3224', valor: '1' }
    ];
    return dsTipos;
}