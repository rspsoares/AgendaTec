function PageSetup() {
    $('#btnAddAppointment').click(function () {
        AddAppointment();
    });

    $('#btnSearch').click(function () {
    //    LoadSchedules();
    });

    $('#btnClear').click(function () {
      //  LoadSchedules();
    });

    var dtDateFromFilter = $("#dtDateFromFilter");
    dtDateFromFilter.kendoMaskedTextBox({
        mask: "00/00/0000"
    });
    dtDateFromFilter.kendoDatePicker({
        format: "dd/MM/yyyy"
    });
    dtDateFromFilter.closest(".k-datepicker")
        .add(dtDateFromFilter)
        .removeClass("k-textbox");

    var dtDateToFilter = $("#dtDateToFilter");
    dtDateToFilter.kendoMaskedTextBox({
        mask: "00/00/0000"
    });
    dtDateToFilter.kendoDatePicker({
        format: "dd/MM/yyyy"
    });
    dtDateToFilter.closest(".k-datepicker")
        .add(dtDateToFilter)
        .removeClass("k-textbox");

    $('#chkBonus').bootstrapSwitch();

    LoadCompanyNameCombo();
    
    $("#ddlCustomerFilter")
        .data("kendoDropDownList")
        .bind("select", ddlCustomerFilterChange);

    //$("#ddlCustomer")
    //    .data("kendoDropDownList")
    //    .bind("select", ddlCustomerChange);
    
    LoadProfessionalsFilterCombo();
    LoadProfessionalsCombo();

    LoadServiceFilterCombo();
    LoadServiceCombo();

    LoadConsumerFilterCombo();
    LoadConsumerCombo();

   // LoadSchedules();
}

function ddlCustomerFilterChange(e) {
    $('#ddlProfessionalFilter').data('kendoDropDownList').dataSource.read();
   // $('#ddlServiceFilter').data('kendoDropDownList').dataSource.read();
    //$('#ddlConsumerFilter').data('kendoDropDownList').dataSource.read();
}

function ddlCustomerChange(e) {
    $('#ddlProfessional').data('kendoDropDownList').dataSource.read();
    $('#ddlService').data('kendoDropDownList').dataSource.read();
    $('#ddlConsumer').data('kendoDropDownList').dataSource.read();
}

function LoadProfessionalsFilterCombo() {
    $('#ddlProfessionalFilter').kendoDropDownList({
        placeholder: "Selecione...",
        dataTextField: 'Name',
        dataValueField: 'IDProfessional',
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: "/Professionals/GetProfessionalNameCombo",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idCustomer: $("#ddlCustomerFilter").val()
                        };
                    }
                }
            }
        }
    });
}

function LoadProfessionalsCombo() {
    $('#ddlProfessional').kendoDropDownList({
        placeholder: "Selecione...",
        dataTextField: 'Name',
        dataValueField: 'IDProfessional',
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: "/Professional/GetProfessionalNameCombo",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idCustomer: $("#ddlCustomer").val()
                        };
                    }
                }
            }
        }
    });
}

function LoadServiceFilterCombo() {


}

function LoadServiceCombo() {


}

function LoadConsumerFilterCombo() {


}

function LoadConsumerCombo() {


}


//function LoadSchedules() {
//    $("#grid").html("");
//    $("#grid").kendoGrid({
//        dataSource: {
//            transport: {
//                read: {
//                    url: "/Customers/GetGrid",
//                    dataType: "json",
//                    type: "GET",
//                    async: false,
//                    cache: false

//                },
//                parameterMap: function (data, type) {
//                    if (type === "read") {
//                        return {
//                            customerName: $('#txtCompanyNameFilter').val()
//                        };
//                    }
//                }
//            },
//            pageSize: 10,
//            schema: {
//                data: function (result) {
//                    return result.Data;
//                },
//                total: function (result) {
//                    return result.Total;
//                }
//            }
//        },
//        scrollable: true,
//        resizable: true,
//        sortable: true,
//        pageable: {
//            pageSizes: [10, 25, 50]
//        },
//        columns: [
//            { field: "IDCustomer", hidden: true },
//            { field: "CompanyName", title: "Razao Social", width: "60%" },
//            { field: "HireDate", title: "Data Contratação", template: "#= kendo.toString(kendo.parseDate(HireDate, 'yyyy-MM-dd'), 'dd/MM/yyyy') #", width: "15%" },
//            { field: "Active", title: "Ativo", width: "15%", template: "#:StatusDescription(Active)#" },
//            {
//                title: " ",
//                template: "<a onclick='javascript:{CustomerEdit(this);}' class='k-button'>"
//                    + "<span class='glyphicon glyphicon glyphicon-pencil'></span></a>",
//                width: "10%",
//                attributes: { style: "text-align:center;" },
//                filterable: false
//            }
//        ]
//    });
//}

//function StatusDescription(varStatus) {
//    if (varStatus === true)
//        return "Ativo";
//    else
//        return "Inativo";
//}

function AddAppointment() {
    CleanFields();

    $('#modalScheduleEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalScheduleEdit .modal-dialog .modal-header center .modal-title strong').html("Controle de Agendamento");
    $('#modalScheduleEdit').modal({ backdrop: 'static', keyboard: false });
}

function CleanFields() {
    //$("#hiddenIDCustomer").val(0);
    //$("#txtCompanyName").val("");
    //$("#txtCNPJ").val("");
    //$("#txtAddress").val("");
    //$("#txtPhone").val("");
    //$("#dtHireDate").val("");
    //$('#chkActive').bootstrapSwitch('state', true);
    //$("#txtNote").val("");
}

//function CustomerEdit(e) {
//    var dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);
//    $("#loading-page").show();

//    CleanFields();

//    $.ajax({
//        type: "GET",
//        contentType: 'application/json; charset=utf-8',
//        dataType: "json",
//        url: "/Customers/GetCustomer",
//        data: { "idCustomer": dataItem.IDCustomer },
//        cache: false,
//        async: false,
//        success: function (result) {
//            if (result.Success) {
//                $("#hiddenIDCustomer").val(result.Data.IDCustomer);
//                $("#txtCompanyName").val(result.Data.CompanyName);
//                $("#txtCNPJ").val(result.Data.CNPJ).trigger('input');
//                $("#txtAddress").val(result.Data.Address);
//                $("#txtPhone").val(result.Data.Phone);
//                $("#dtHireDate").val(kendo.toString(kendo.parseDate(result.Data.HireDate, 'yyyy-MM-dd'), 'dd/MM/yyyy'));
//                $('#chkActive').bootstrapSwitch('state', result.Data.Active);
//                $("#txtNote").val(result.Data.Note);

//            }
//            else {
//                ShowModalAlert(result.errorMessage);
//                return;
//            }
//        }
//    });

//    $('#modalCustomerEdit .modal-dialog .modal-header center .modal-title strong').html("");
//    $('#modalCustomerEdit .modal-dialog .modal-header center .modal-title strong').html("Cadastro do Cliente");
//    $('#modalCustomerEdit').modal({ backdrop: 'static', keyboard: false });

//    $("#loading-page").hide();
//}

//function SaveCustomer() {
//    var customer = [];

//    var errorMessages = ValidateRequiredFields();

//    if (errorMessages !== '') {
//        ShowModalAlert(errorMessages);
//        return;
//    }

//    customer = {
//        IDCustomer: parseInt($("#hiddenIDCustomer").val()),
//        CompanyName: $("#txtCompanyName").val(),
//        CNPJ: $("#txtCNPJ").val().replace(/[^\d]/g, ""),
//        Address: $("#txtAddress").val(),
//        Phone: $("#txtPhone").val(),
//        HireDate: kendo.parseDate($("#dtHireDate").val(), "dd/MM/yyyy"),
//        Active: $('#chkActive').bootstrapSwitch('state'),
//        Note: $("#txtNote").val()
//    };

//    $("#loading-page").show();

//    $.ajax({
//        url: '/Customers/SaveCustomer',
//        data: JSON.stringify({ customer: customer }),
//        type: 'POST',
//        async: false,
//        contentType: 'application/json; charset=utf-8',
//        success: function (result) {
//            $("#loading-page").hide();
//            if (result.Success) {
//                ShowModalSucess("Gravação concluída com sucesso.");
//                $('#modalCustomerEdit').modal("hide");
//                LoadCustomers();
//            }
//            else
//                ShowModalAlert("Erro ao gravar alterações.");
//        }
//    });
//}

//function ValidateRequiredFields() {
//    var errorMessage = '';

//    if ($("#txtCompanyName").val() === '')
//        errorMessage += 'Favor informar a Razão Social' + '<br/>';

//    if ($("#txtCNPJ").val() === '')
//        errorMessage += 'Favor informar o CNPJ' + '<br/>';
//    else {
//        if (!CNPJCheck($("#txtCNPJ").val()))
//            errorMessage += 'CNPJ inválido' + '<br/>';
//    }

//    if ($("#txtAddress").val() === '')
//        errorMessage += 'Favor informar o Endereço' + '<br/>';

//    if ($("#txtPhone").val() === '')
//        errorMessage += 'Favor informar o Telefone' + '<br/>';

//    if ($("#dtHireDate").val() === '')
//        errorMessage += 'Favor informar a Data de Contratação' + '<br/>';

//    return errorMessage;
//}