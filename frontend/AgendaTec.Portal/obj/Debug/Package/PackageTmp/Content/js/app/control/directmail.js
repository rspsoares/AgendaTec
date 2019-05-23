var selectedRow;

function PageSetup() {
    $('#btnAddDirectMail').click(function () {
        AddDirectMail();
    });

    $('#btnSearch').click(function () {
        LoadDirectMails();
    });

    $('#btnClear').click(function () {
        $("#ddlCustomerFilter").data("kendoDropDownList").select(0);
        $("#ddlCustomerFilter").val('');        

        LoadDirectMails();
    });

    $('#txtDescriptionFilter').keydown(function (e) {
        var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;
        if (key === 13) {
            e.preventDefault();
            LoadDirectMails();
        }
    });

    $("#txtContent").kendoEditor({
        resizable: {
            content: false,
            toolbar: false,
            encoded: false
        }
    });   

    LoadCombo("/Customers/GetCompanyNameCombo", ['#ddlCustomerFilter', '#ddlCustomer'], "Id", "Name", true);    
    GetIntervals();

    LoadDirectMails();
}

function GetIntervals() {
    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/DirectMails/GetIntervalCombo",        
        cache: true,
        async: false,
        success: function (result) {
            if (result.Success) {
                $("#ddlInterval").kendoDropDownList({
                    dataSource: result.Data
                });
            }
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });
}

function LoadDirectMails() {
    $("#grid").html("");
    $("#grid").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: "/DirectMails/GetGrid",
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idCustomer: $("#ddlCustomerFilter").val(),
                            mailType: GetURLParameter('MailType'),
                            description: $('#txtDescriptionFilter').val(),
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
            { field: "Description", title: "Descrição", width: "40%" },
            { field: "MailTypeDescription", title: "Tipo", width: "25%" },
            { field: "Last", title: "Último envio", template: "#= (Last === null) ? '' : kendo.toString(kendo.parseDate(Last, 'yyyy-MM-dd HH:mm:ss'), 'dd/MM/yyyy HH:mm:ss') #", width: "15%" },
            {
                title: " ",
                template: "<a onclick='javascript:{ DirectMailEdit(this); }' class='k-button'>"
                    + "<span class='glyphicon glyphicon glyphicon-pencil'></span></a>",
                width: "10%",
                attributes: { style: "text-align:center;" },
                filterable: false
            },
            {
                title: " ",
                template: "<a onclick='javascript:{ DirectMailDelete_click(this); }' class='k-button'>"
                    + "<span class='glyphicon glyphicon glyphicon-trash'></span></a>",
                width: "10%",
                attributes: { style: "text-align:center;" },
                filterable: false
            }
        ]
    });
}

function AddDirectMail() {
    CleanFields(true);

    $("#btnResend").prop('disabled', true);    

    $('#modalDirectMailEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalDirectMailEdit .modal-dialog .modal-header center .modal-title strong').html("Controle de Mala Direta");
    $('#modalDirectMailEdit').modal({ backdrop: 'static', keyboard: false });
}

function CleanFields(loadFilterCombos) {
    $("#hiddenId").val(0);

    if (!$('#ddlCustomerFilter').prop('disabled') && loadFilterCombos) {
        $("#ddlCustomer").data('kendoDropDownList').value($("#ddlCustomerFilter").val());        
    }

    $("#txtDescription").val("");
    $("#txtContent").data("kendoEditor").value("");
    $("#txtLast").val("");

    var dropdownlist = $("#ddlInterval").data("kendoDropDownList");
    dropdownlist.select(0);
}

function ResendDirectMail() {
    $.ajax({
        url: '/DirectMails/ResendDirectMail',
        data: JSON.stringify({ idDirectMail: $("#hiddenId").val() }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Success) 
                ShowModalSucess("Solicitação de reenvio da Mala Direta efetuada com sucesso.");            
            else
                ShowModalAlert(result.errorMessage);
        }
    });
}

function DirectMailEdit(e) {
    var dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);
    $("#loading-page").show();

    CleanFields(false);

    $("#btnResend").prop('disabled', false);    

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/DirectMails/GetDirectMail",
        data: { "idDirectMail": dataItem.Id },
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) {
                $("#hiddenId").val(result.Data.Id);
                $("#ddlCustomer").data('kendoDropDownList').value(result.Data.IdCustomer);
                $("#txtDescription").val(result.Data.Description);     
                $("#txtContent").data("kendoEditor").value(result.Data.Content);                
                $("#txtLast").val(result.Data.Last);
                $("#ddlInterval").data('kendoDropDownList').value(result.Data.Interval);
            }
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });

    $('#modalDirectMailEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalDirectMailEdit .modal-dialog .modal-header center .modal-title strong').html("Controle de Mala Direta");
    $('#modalDirectMailEdit').modal({ backdrop: 'static', keyboard: false });

    $("#loading-page").hide();
}

function DirectMailDelete_click(e) {   
    selectedRow = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);
    $('#modalDeleteConfirmation').modal({ backdrop: 'static', keyboard: false });    
}

function DirectMailDelete() {    
    $.ajax({
        url: '/DirectMails/DeleteDirectMail',        
        data: JSON.stringify({ idDirectMail: selectedRow.Id }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Success) {
                ShowModalSucess("Mala Direta excluída com sucesso.");
                LoadDirectMails();
            }
            else
                ShowModalAlert(result.errorMessage);
        }
    });

    $('#modalDeleteConfirmation').modal("hide");
}

function SaveDirectMail() {
    var directMail = [];

    var errorMessages = ValidateRequiredFields();

    if (errorMessages !== '') {
        ShowModalAlert(errorMessages);
        return;
    }

    directMail = {
        Id: parseInt($("#hiddenId").val()),
        IdCustomer: $("#ddlCustomer").val(),        
        Description: $("#txtDescription").val(),        
        Content: $("#txtContent").val(),
        Interval: $("#ddlInterval").val(),
        MailType: parseInt(GetURLParameter('MailType')),
        Resend: false
    };

    $("#loading-page").show();

    $.ajax({
        url: '/DirectMails/SaveDirectMail',
        data: JSON.stringify({ directMail: directMail }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#loading-page").hide();
            if (result.Success) {
                ShowModalSucess("Gravação concluída com sucesso.");
                $('#modalDirectMailEdit').modal("hide");
                LoadDirectMails();
            }
            else
                ShowModalAlert(result.errorMessage);
        }
    });
}

function ValidateRequiredFields() {
    var errorMessage = '';

    if ($("#txtDescription").val() === '')
        errorMessage += 'Favor informar a Descrição' + '<br/>';

    if ($("#txtContent").val() === '')
        errorMessage += 'Favor informar o Conteúdo' + '<br/>';

    return errorMessage;
}