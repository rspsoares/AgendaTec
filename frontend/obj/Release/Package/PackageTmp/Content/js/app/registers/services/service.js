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

        document.getElementById("txtDescriptionFilter").value = "";
     
        LoadServices();
    });

    $('#txtDescriptionFilter').keydown(function (e) {
        var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;
        if (key === 13) {
            e.preventDefault();
            LoadServices();
        }
    });

    $('#txtValue').on('keypress', function (e) {
        if (e.which !== 8 && e.which !== 0 && e.which !== 44 && e.which !== 45 && (e.which < 48 || e.which > 57))
            return false;
    });

    $('#txtValue').on('blur', function () {
        var valor = parseFloat($(this).val().replace(/\./g, '').replace(',', '.'));
        if (valor)
            $(this).val(valor.FormatMoney(2, '', '.', ','));
        else
            $(this).val("0,00");
    });   

    $('#txtTime').kendoMaskedTextBox({ mask: "000" });

    LoadSocialNameCombo();
   
    var currentCustomer = GetCurrentCustomer();
    if (currentCustomer !== '') {
        $("#ddlCustomerFilter").data('kendoDropDownList').text(currentCustomer);
        $("#ddlCustomerFilter").data("kendoDropDownList").enable(false);

        $("#ddlCustomer").data('kendoDropDownList').text(currentCustomer);
        $("#ddlCustomer").data("kendoDropDownList").enable(false);
    }

    LoadServices();
}

function LoadSocialNameCombo() {
    var dsData = undefined;

    $.ajax({
        url: "/Customers/GetSocialNameCombo",
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            if (result.Success)
                dsData = result.Data;
            else {
                ShowModalAlert("Erro ao recuperar clientes.");
                return;
            }                
        }
    });

    $('#ddlCustomerFilter').kendoDropDownList({
        dataTextField: "SocialName",
        dataValueField: "IDCustomer",
        dataSource: dsData,
        optionLabel: "Selecione..."
    });

    $('#ddlCustomer').kendoDropDownList({
        dataTextField: "SocialName",
        dataValueField: "IDCustomer",
        dataSource: dsData,
        optionLabel: "Selecione..."
    });
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
            { field: "IDService", hidden: true },            
            { field: "Description", title: "Descrição",  width: "50%" },
            { field: "Value", title: "Valor", template: '#=Value.FormatMoney(2, "", ".", ",") #', attributes: { style: "text-align:right;" } , width: "15%" },
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
    CleanFields();

    $('#modalServiceEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalServiceEdit .modal-dialog .modal-header center .modal-title strong').html("Cadastro do Serviço");
    $('#modalServiceEdit').modal({ backdrop: 'static', keyboard: false });
}

function CleanFields() {
    $("#hiddenIDService").val(0);

    if (!$('#ddlCustomerFilter').prop('disabled')) {
        var dropdownlist = $("#ddlCustomer").data("kendoDropDownList");
        dropdownlist.select(0);
    }

    $("#txtDescription").val("");
    $("#txtValue").val("0,00");
    $("#txtTime").val("");    
}

function ServiceEdit(e) {
    var dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);
    $("#loading-page").show();

    CleanFields();

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Services/GetService",
        data: { "idService": dataItem.IDService },
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) {
                $("#hiddenIDService").val(result.Data.IDService);
                $("#ddlCustomer").data('kendoDropDownList').value(result.Data.IDCustomer);                
                $("#txtDescription").val(result.Data.Description);
                $("#txtValue").val(result.Data.Value.FormatMoney(2, '', '.', ','));
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
        IDService: parseInt($("#hiddenIDService").val()),
        IDCustomer: $("#ddlCustomer").val(),
        Description: $("#txtDescription").val(),
        Value: Math.abs(parseFloat($("#txtValue").val().replace(/\./g, '').replace(",", "."))),
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

    if ($("#txtValue").val() === '' || $("#txtValue").val() === '0,00')
        errorMessage += 'Favor informar o Valor do Serviço' + '<br/>';

    if ($("#txtTime").val() === '')
        errorMessage += 'Favor informar o Tempo de execução' + '<br/>';

    return errorMessage;
}