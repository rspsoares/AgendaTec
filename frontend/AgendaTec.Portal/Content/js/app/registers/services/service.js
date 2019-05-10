function PageSetup() {
    $('#btnAddService').click(function () {
        AddService();
    });

    $('#btnSearch').click(function () {
        LoadServices();
    });

    $('#btnClear').click(function () {
        if (!$('#ddlCustomerFilter').prop('disabled')) {
            var dropdownlist = $("#ddlCustomerFilter").data("kendoDropDownList");
            dropdownlist.select(0);
        }

        $("#txtDescriptionFilter").val("");        
     
        LoadServices();
    });

    $('#txtDescriptionFilter').keydown(function (e) {
        var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;
        if (key === 13) {
            e.preventDefault();
            LoadServices();
        }
    });

    $('#txtPrice').on('keypress', function (e) {
        if (e.which !== 8 && e.which !== 0 && e.which !== 44 && e.which !== 45 && (e.which < 48 || e.which > 57))
            return false;
    });

    $('#txtPrice').on('blur', function () {
        var price = parseFloat($(this).val().replace(/\./g, '').replace(',', '.'));
        if (price)
            $(this).val(price.FormatMoney(2, '', '.', ','));
        else
            $(this).val("0,00");
    });   

    $('#txtTime').on('keypress', function (e) {
        if (e.which !== 8 && e.which !== 0 && (e.which < 48 || e.which > 57))
            return false;
    });

    LoadCombo("/Customers/GetCompanyNameCombo", ['#ddlCustomerFilter', '#ddlCustomer'], "Id", "Name", true); 
    LoadServices();
}

function LoadServices() {
    $("#grid").html("");
    $("#grid").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: "/Services/GetGrid",
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idCustomer: $("#ddlCustomerFilter").val(),
                            serviceName: $('#txtDescriptionFilter').val()
                        };
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
            { field: "Description", title: "Descrição",  width: "50%" },
            { field: "Price", title: "Valor", template: '#=Price.FormatMoney(2, "", ".", ",") #', attributes: { style: "text-align:right;" } , width: "15%" },
            { field: "Time", title: "Tempo (minutos)", attributes: { style: "text-align:right;" }, width: "15%" },
            {
                title: " ",
                template: "<a onclick='javascript:{ServiceEdit(this);}' class='k-button'>"
                    + "<span class='glyphicon glyphicon glyphicon-pencil'></span></a>",
                width: "10%",
                attributes: { style: "text-align:center;" },
                filterable: false
            }
        ]
    });
}

function AddService() {
    CleanFields(true);

    $('#modalServiceEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalServiceEdit .modal-dialog .modal-header center .modal-title strong').html("Cadastro do Serviço");
    $('#modalServiceEdit').modal({ backdrop: 'static', keyboard: false });
}

function CleanFields(loadFilterCombos) {
    $("#hiddenId").val(0);

    if (!$('#ddlCustomerFilter').prop('disabled') && loadFilterCombos)
        $("#ddlCustomer").data('kendoDropDownList').value($("#ddlCustomerFilter").val());

    $("#txtDescription").val("");
    $("#txtPrice").val("0,00");
    $("#txtTime").val("");    
}

function ServiceEdit(e) {
    var dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);
    $("#loading-page").show();

    CleanFields(false);

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Services/GetService",
        data: { "idService": dataItem.Id },
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) {
                $("#hiddenId").val(result.Data.Id);
                $("#ddlCustomer").data('kendoDropDownList').value(result.Data.IdCustomer);                
                $("#txtDescription").val(result.Data.Description);
                $("#txtPrice").val(result.Data.Price.FormatMoney(2, '', '.', ','));
                $("#txtTime").val(result.Data.Time);                
            }
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });

    $('#modalServiceEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalServiceEdit .modal-dialog .modal-header center .modal-title strong').html("Cadastro do Serviço");
    $('#modalServiceEdit').modal({ backdrop: 'static', keyboard: false });

    $("#loading-page").hide();
}

function SaveService() {
    var service = [];

    var errorMessages = ValidateRequiredFields();

    if (errorMessages !== '') {
        ShowModalAlert(errorMessages);
        return;
    }

    service = {
        Id: parseInt($("#hiddenId").val()),
        IdCustomer: $("#ddlCustomer").val(),
        Description: $("#txtDescription").val(),
        Price: Math.abs(parseFloat($("#txtPrice").val().replace(/\./g, '').replace(",", "."))),
        Time: parseInt($("#txtTime").val())
    };

    $("#loading-page").show();

    $.ajax({
        url: '/Services/SaveService',
        data: JSON.stringify({ service: service }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#loading-page").hide();
            if (result.Success) {
                ShowModalSucess("Gravação concluída com sucesso.");
                $('#modalServiceEdit').modal("hide");
                LoadServices();
            }
            else
                ShowModalAlert("Erro ao gravar alterações.");
        }
    });
}

function ValidateRequiredFields() {
    var errorMessage = '';

    if ($("#ddlCustomer").val() === '')
        errorMessage += 'Favor informar ao Cliente' + '<br/>';

    if ($("#txtDescription").val() === '')
        errorMessage += 'Favor informar a Descrição' + '<br/>';

    if ($("#txtPrice").val() === '' || $("#txtPrice").val() === '0,00')
        errorMessage += 'Favor informar o Valor do Serviço' + '<br/>';

    if ($("#txtTime").val() === '')
        errorMessage += 'Favor informar o Tempo de execução' + '<br/>';

    return errorMessage;
}