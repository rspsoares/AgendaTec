function CarregarComponentes() {
    var dataInicio;

    CarregarMatrizes();

    $('#btnPesquisar').click(function () {
        CarregarGridRateio(idMatriz);
    });

    $('#btnLimparPequisa').click(function () {
        var selectMatriz = $("#selectMatriz").data("kendoDropDownList");
        selectMatriz.select(0);
        $('#tbRazaoSocial').val('');
        idMatriz = 0;
        CarregarGridRateio(idMatriz);
    });

    dataInicio = new Date();
    $("#tbInicioRegraRateio").kendoDatePicker({
        start: "year",
        depth: "year",
        format: "MMMM yyyy"//,
        //value: dataInicio
    });    

    $('#optRateio').bootstrapSwitch();

    $("#divDataInicioRegra").hide();
}

function CarregarMatrizes() {
    $('#selectMatriz').kendoDropDownList({
        dataTextField: "CodigoMatriz",
        dataValueField: "Id",
        dataSource: LoadDsMatrizes(),
        optionLabel: "Escolha ...",
        select: function (e) {
            var dataItem = this.dataItem(e.item);
            $('#tbRazaoSocial').val(dataItem.RazaoSocial);
            idMatriz = dataItem.Id;
        }
    });
}

function CarregarGridRateio(idMatriz) {
    $("#gridRateio").html("");
    $("#gridRateio").kendoGrid({
        dataSource: {       
            transport: {
                read: {
                    url: "/Rateio/Pesquisar",
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type == "read") {
                        return { idEmpresa: idMatriz }
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
        resizable: true,       
        sortable: true,
        pageable: {
            pageSizes: [10, 25, 50]
        },
        columns: [
           { field: "Id", hidden: true },
           { field: "InicioRegraRateio", hidden: true },
           {
               field: "Ordem",
               title: "Ordem",
               headerAttributes: { style: "text-align:center;" },
               width: "70px",
               attributes: { style: "text-align:center;" }
           },
           {
               field: "Descricao",
               title: "Descrição"
           },
           {
               field: "Rateio",
               title: "Rateio",
               width: "80px",
               headerAttributes: { style: "text-align:center;" },
               template: "#:DescricaoRateio(Rateio)#",
               attributes: { style: "text-align:center;" }
           },
           {
                title: "Editar",
                headerAttributes: { style: "text-align:center;" },
                template: "<button class='k-button' onclick='javascript:{Editar(this);}'>"
                        + "<span title='Visualizar Resumo' class='glyphicon glyphicon-pencil'></span></a>",
                width: "100px",
                attributes: { style: "text-align:center;" },
                filterable: false
            }
       ]
    });   
}

function DescricaoRateio(Rateio) {
    if (Rateio == 1)
        return "Sim";
    else
        return "Não";
}

function Editar(e) {
    var dataItem = $("#gridRateio").data("kendoGrid").dataItem(e.parentElement.parentElement);
    
    $("#tbId").val(dataItem.Id);
    $("#tbDescricao").text(dataItem.Descricao);
    $('#optRateio').bootstrapSwitch('state', dataItem.Rateio == 1 ? true : false);
    $("#tbInicioRegraRateio").data("kendoDatePicker").value(dataItem.InicioRegraRateio);

    $('#modalEditar').modal({ backdrop: 'static', keyboard: false });
    $('#modalEditar .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalEditar .modal-dialog .modal-header center .modal-title strong').html('Parametrização de Rateio do Relatório Gerencial');
}

function SalvarRateio() {
    var Id = parseInt($("#tbId").val());
    var rateio = $('#optRateio').bootstrapSwitch('state') == true ? 1 : 0;
    var dtInicioRegraRateio = $("#tbInicioRegraRateio").data("kendoDatePicker").value();

    $.ajax({
        url: '/Rateio/Alterar',
        data: JSON.stringify({ Id: Id, Rateio: rateio, InicioRegraRateio: dtInicioRegraRateio }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Sucesso) {             
                ShowModalSucesso(result.Msg);
                $('#gridRateio').data('kendoGrid').dataSource.read();
            }
            else                 
                ShowModalAlerta(result.Msg);            
        }
    });   
}