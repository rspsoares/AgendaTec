﻿function PageSetup() {
    $('#btnAddCustomer').click(function () {
        AddCustomer();
    });

    $('#btnSearch').click(function () {
        LoadCustomers();
    });

    $('#btnClear').click(function () {
        document.getElementById("txtCompanyNameFilter").value = "";        
        LoadCustomers();
    });

    $('#txtName').keydown(function (e) {
        var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;
        if (key === 13) {
            e.preventDefault();
            LoadCustomers();
        }
    });    

    var dtHire = $("#dtHire");

    dtHire.kendoMaskedTextBox({
        mask: "00/00/0000"
    });

    dtHire.kendoDatePicker({
        format: "dd/MM/yyyy"
    });

    dtHire.closest(".k-datepicker")
        .add(dtHire)
        .removeClass("k-textbox");   

    $('#chkActive').bootstrapSwitch();

    LoadCustomers();  
}

function LoadCustomers() {
    $("#grid").html("");
    $("#grid").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: "/Customers/GetGrid",
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                   
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            customerName: $('#txtNameFilter').val()                            
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
            { field: "Name", title: "Razao Social", width: "60%" },
            { field: "Hire", title: "Data Contratação", template: "#= kendo.toString(kendo.parseDate(Hire, 'yyyy-MM-dd'), 'dd/MM/yyyy') #", width: "15%" },
            { field: "Active", title: "Ativo", width: "15%", template: "#:StatusDescription(Active)#"},            
            {
                title: " ",
                template: "<a onclick='javascript:{CustomerEdit(this);}' class='k-button'>"
                    + "<span class='glyphicon glyphicon glyphicon-pencil'></span></a>",
                width: "10%",
                attributes: { style: "text-align:center;" },
                filterable: false
            }          
        ]
    });   
}

function StatusDescription(varStatus) {
    if (varStatus === true)
        return "Ativo";
    else
        return "Inativo";
}

function AddCustomer() {
    CleanFields();

    $('#modalCustomerEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalCustomerEdit .modal-dialog .modal-header center .modal-title strong').html("Cadastro do Cliente");
    $('#modalCustomerEdit').modal({ backdrop: 'static', keyboard: false });   
}

function CleanFields() {
    $("#hiddenID").val(0);
    $("#txtName").val("");
    $("#txtCNPJ").val("");
    $("#txtAddress").val("");
    $("#txtPhone").val("");
    $("#dtHire").val("");
    $('#chkActive').bootstrapSwitch('state', true);
    $("#txtNote").val("");
}

function CustomerEdit(e) {
    var dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);    
    $("#loading-page").show();

    CleanFields();

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Customers/GetCustomer",
        data: { "idCustomer": dataItem.Id },
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) {
                $("#hiddenID").val(result.Data.Id);
                $("#txtName").val(result.Data.Name);
                $("#txtCNPJ").val(result.Data.CNPJ).trigger('input');
                $("#txtAddress").val(result.Data.Address);
                $("#txtPhone").val(result.Data.Phone);
                $("#dtHire").val(kendo.toString(kendo.parseDate(result.Data.Hire, 'yyyy-MM-dd'), 'dd/MM/yyyy'));
                $('#chkActive').bootstrapSwitch('state', result.Data.Active);                
                $("#txtNote").val(result.Data.Note);             
            }
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });

    $('#modalCustomerEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalCustomerEdit .modal-dialog .modal-header center .modal-title strong').html("Cadastro do Cliente");
    $('#modalCustomerEdit').modal({ backdrop: 'static', keyboard: false });   

    $("#loading-page").hide();
}

function SaveCustomer() {
    var customer = [];

    var errorMessages = ValidateRequiredFields();

    if (errorMessages !== '') {
        ShowModalAlert(errorMessages);
        return;
    }

    customer = {
        Id: parseInt($("#hiddenID").val()),
        Name: $("#txtName").val(),
        CNPJ: $("#txtCNPJ").val().replace(/[^\d]/g, ""),
        Address: $("#txtAddress").val(),
        Phone: $("#txtPhone").val(),
        Hire: kendo.parseDate($("#dtHire").val(), "dd/MM/yyyy"),
        Active: $('#chkActive').bootstrapSwitch('state'),
        Note: $("#txtNote").val()
    };

    $("#loading-page").show();

    $.ajax({
        url: '/Customers/SaveCustomer',
        data: JSON.stringify({ customer: customer }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#loading-page").hide();
            if (result.Success) {
                ShowModalSucess("Gravação concluída com sucesso.");    
                $('#modalCustomerEdit').modal("hide");
                LoadCustomers();
            }                
            else                 
                ShowModalAlert("Erro ao gravar alterações.");            
        }
    });
}

function ValidateRequiredFields() {
    var errorMessage = '';

    if ($("#txtName").val() === '')
        errorMessage += 'Favor informar a Razão Social' + '<br/>';

    if ($("#txtAddress").val() === '')
        errorMessage += 'Favor informar o Endereço' + '<br/>';

    if ($("#txtPhone").val() === '')
        errorMessage += 'Favor informar o Telefone' + '<br/>';
    
    if($("#dtHire").val() === '')
        errorMessage += 'Favor informar a Data de Contratação' + '<br/>';

    return errorMessage;
}