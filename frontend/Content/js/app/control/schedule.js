function PageSetup() {
    $('#btnAddAppointment').click(function () {
        AddAppointment();
    });

    $('#btnSearch').click(function () {
        LoadSchedules();
    });

    $('#btnClear').click(function () {
        $("#ddlCustomerFilter").data("kendoDropDownList").select(0);
        ddlCustomerFilterChange();
        $("#dtDateFromFilter").val("");
        $("#dtDateToFilter").val("");
        $('#chkBonusFilter').bootstrapSwitch('state', true);
        LoadSchedules();
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

    var d = new Date();
    $("#dtDateTime").kendoDateTimePicker({
        value: new Date(d.getFullYear(), d.getMonth() + 1, d.getDate(), 9, 0, 0),
        min: new Date(d.getFullYear(), d.getMonth() + 1, d.getDate(), 9, 0, 0)        
    });

    $('#chkBonusFilter').bootstrapSwitch();
    $('#chkBonus').bootstrapSwitch();

    LoadCompanyNameCombo();
    
    $("#ddlCustomerFilter")
        .data("kendoDropDownList")
        .bind("change", ddlCustomerFilterChange);

    $("#ddlCustomer")
        .data("kendoDropDownList")
        .bind("change", ddlCustomerChange);
    
    LoadProfessionalsFilterCombo();
    LoadProfessionalsCombo();
    LoadServiceFilterCombo();
    LoadServiceCombo();
    LoadConsumerFilterCombo();
    LoadConsumerCombo();
    LoadSchedules();
}

function ddlCustomerFilterChange(e) {
    $('#ddlProfessionalFilter').data('kendoDropDownList').dataSource.read();
    $('#ddlServiceFilter').data('kendoDropDownList').dataSource.read();
    $('#ddlConsumerFilter').data('kendoDropDownList').dataSource.read();
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
                    url: "/Professionals/GetProfessionalNameCombo",
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
    $('#ddlServiceFilter').kendoDropDownList({
        placeholder: "Selecione...",
        dataTextField: 'Description',
        dataValueField: 'IDService',
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: "/Services/GetServiceNameCombo",
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

function LoadServiceCombo() {
    $('#ddlService').kendoDropDownList({
        placeholder: "Selecione...",
        dataTextField: 'Description',
        dataValueField: 'IDService',
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: "/Services/GetServiceNameCombo",
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

function LoadConsumerFilterCombo() {
    $('#ddlConsumerFilter').kendoDropDownList({
        placeholder: "Selecione...",
        dataTextField: 'FullName',
        dataValueField: 'UkUser',
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: "/Users/GetConsumerNamesCombo",
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

function LoadConsumerCombo() {
    $('#ddlConsumer').kendoDropDownList({
        placeholder: "Selecione...",
        dataTextField: 'FullName',
        dataValueField: 'UkUser',
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: "/Users/GetConsumerNamesCombo",
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

function LoadSchedules() {
    $("#grid").html("");
    $("#grid").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: "/Schedules/GetGrid",
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idCustomer: $("#ddlCustomerFilter").val(),
                            idProfessional: $("#ddlProfessionalFilter").val(),
                            idService: $("#ddlServiceFilter").val(),
                            idConsumer: $("#ddlConsumerFilter").val(),
                            dateFrom: kendo.parseDate($("#dtDateFromFilter").val(), "dd/MM/yyyy"),
                            dateTo: kendo.parseDate($("#dtDateToFilter").val(), "dd/MM/yyyy"),
                            bonus: $('#chkBonusFilter').bootstrapSwitch('state')
                        };
                    }
                }
            },
            serverPaging: true,
            serverSorting: true,            
            serverGrouping: false,
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
        groupable: true,
        reorderable: true,
        pageable: {
            pageSizes: [10, 25, 50]
        },
        columns: [
            { field: "IDSchedule", hidden: true },
            { field: "ProfessionalName", title: "Profissional", width: "60%" },
            { field: "ServiceName", title: "Serviço", width: "60%" },
            { field: "ConsumerName", title: "Cliente", width: "60%" },
            { field: "Date", title: "Data", template: "#= kendo.toString(kendo.parseDate(Date, 'yyyy-MM-dd'), 'dd/MM/yyyy') #", width: "15%" },
            { field: "Time", title: "Hora", width: "15%" },
            { field: "Bonus", title: "Bônus", width: "15%", template: "#:BonusDescription(Bonus)#" },
            //{
            //    title: " ",
            //    template: "<a onclick='javascript:{CustomerEdit(this);}' class='k-button'>"
            //        + "<span class='glyphicon glyphicon glyphicon-pencil'></span></a>",
            //    width: "10%",
            //    attributes: { style: "text-align:center;" },
            //    filterable: false
            //}
        ]
    });
}

function BonusDescription(bonus) {
    if (bonus === true)
        return "Ativo";
    else
        return "Inativo";
}

function AddAppointment() {
    CleanFields();

    $('#modalScheduleEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalScheduleEdit .modal-dialog .modal-header center .modal-title strong').html("Controle de Agendamento");
    $('#modalScheduleEdit').modal({ backdrop: 'static', keyboard: false });
}

function CleanFields() {
    $("#IDSchedule").val(0);   
    $("#ddlCustomer").data("kendoDropDownList").select(0);
    ddlCustomerChange();   
    $("#dtDateTime").val("");
    $('#chkBonus').bootstrapSwitch('state', true);
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

function SaveAppointment() {
    var schedule = [];

    var errorMessages = ValidateRequiredFields();

    if (errorMessages !== '') {
        ShowModalAlert(errorMessages);
        return;
    }
    
    schedule = {
        IDSchedule: parseInt($("#IDSchedule").val()),
        IDCustomer: $("#ddlCustomer").val(),
        IDProfessional: $("#ddlProfessional").val(),
        IDService: $("#ddlService").val(),
        IDConsumer: $("#ddlConsumer").val(),
        Date: kendo.parseDate($("#dtDateTime").val(), "dd/MM/yyyy HH:mm"),
        Bonus: $('#chkBonus').bootstrapSwitch('state')        
    };

    $("#loading-page").show();

    $.ajax({
        url: '/Schedules/SaveAppointment',
        data: JSON.stringify({ schedule: schedule }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#loading-page").hide();
            if (result.Success) {
                ShowModalSucess("Gravação concluída com sucesso.");
                $('#modalScheduleEdit').modal("hide");
                LoadSchedules();
            }
            else
                ShowModalAlert("Erro ao gravar alterações.");
        }
    });
}

function ValidateRequiredFields() {
    var errorMessage = '';

    if ($("#ddlCustomer").val() === '')
        errorMessage += 'Favor informar a Razão Social' + '<br/>';

    if ($("#ddlProfessional").val() === '')
        errorMessage += 'Favor informar o Profissional' + '<br/>';

    if ($("#ddlService").val() === '')
        errorMessage += 'Favor informar o Serviço' + '<br/>';

    if ($("#ddlConsumer").val() === '')
        errorMessage += 'Favor informar o Cliente' + '<br/>';

    if ($("#dtDateTime").val() === '')
        errorMessage += 'Favor informar a Data / Hora' + '<br/>';
    
    return errorMessage;
}