var selectedRows;

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

    $("#ddlService")
        .data("kendoDropDownList")
        .bind("change", ddlServiceChange);


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

function ddlServiceChange(e) {
    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Services/GetService",
        data: { "idService": $("#ddlService").val() },
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) {               
                $("#txtPrice").val(result.Data.Price);
                $("#txtTime").val(result.Data.Time);                
            }
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });
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
                    async: false,
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
                    async: false,
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
                    async: false,
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
        toolbar: [
            { name: 'excel', text: 'Exportar Excel' },
            { template: kendo.template($("#rescheduleTemplate").html()) },
            { template: kendo.template($("#deleteTemplate").html()) }  
        ],
        excel: {
            fileName: 'Agenda_' + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + '.xlsx',
            allPages: true
        },
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                if (rowIndex % 2 == 0) {
                    var row = sheet.rows[rowIndex];
                    for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                        row.cells[cellIndex].background = "#aabbcc";
                    }
                }
            }

            var columns = e.workbook.sheets[0].columns;
            columns.forEach(function (column) {
                delete column.width;
                column.autoWidth = true;
            });
        },
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                },
                model: {
                    id: "IDSchedule"
                }
            },
            transport: {
                read: {
                    url: "/Schedules/GetGrid",
                    dataType: "json",
                    type: "GET",
                    async: true,
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
            pageSize: 10
        },
        height: 450,
        groupable: true,
        sortable: true,
        reorderable: true,
        resizable: true,        
        selectable: "multiple",
        change: onChange,
        pageable: {
            pageSizes: [10, 25, 50]
        },
        columns: [
            { field: "IDSchedule", hidden: true },            
            { field: "ProfessionalName", title: "Profissional", width: "20%" },
            { field: "ServiceName", title: "Serviço", width: "15%" },
            { field: "ConsumerName", title: "Cliente", width: "20%" },
            { field: "Date", title: "Data", width: "15%" },
            { field: "Hour", title: "Hora", width: "15%" },
            { field: "Bonus", title: "Bônus", width: "10%", template: "#:BonusDescription(Bonus)#" },
            {
                title: " ",
                template: "<a onclick='javascript:{AppointmentEdit(this);}' class='k-button'>"
                    + "<span class='glyphicon glyphicon glyphicon-pencil'></span></a>",
                width: "5%",
                attributes: { style: "text-align:center;" },
                filterable: false
            }
        ]
    });    
}

function reschedule_click() {
    alert("Reschedule");
    return false;
}

function delete_click() {
    alert("Delete");
    return false;
}

function onChange(e) {
    selectedRows = e.sender.select();   
}

function LoopIntoSelectedRows() {
    selectedRows.each(function (e) {
        var grid = $("#grid").data("kendoGrid");
        var dataItem = grid.dataItem(this);

        console.log(dataItem);
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
    $("#txtPrice").val("");
    $("#txtTime").val("");
    $('#chkBonus').bootstrapSwitch('state', true);
}

function AppointmentEdit(e) {
    var dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);
    $("#loading-page").show();

    CleanFields();

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Schedules/GetAppointment",
        data: { "idSchedule": dataItem.IDSchedule },
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) {
                $("#hiddenIDSchedule").val(result.Data.IDSchedule);
                $("#ddlCustomer").data('kendoDropDownList').value(result.Data.IDCustomer);
                ddlCustomerChange();
                $("#ddlProfessional").data('kendoDropDownList').value(result.Data.IDProfessional);
                $("#ddlService").data('kendoDropDownList').value(result.Data.IDService);
                $("#ddlConsumer").data('kendoDropDownList').value(result.Data.IDConsumer);
                $("#dtDateTime").val(kendo.toString(kendo.parseDate(result.Data.Date, 'yyyy-MM-dd HH:mm'), 'dd/MM/yyyy HH:mm'));
                $("#txtPrice").val(result.Data.Price);
                $("#txtTime").val(result.Data.Time);
                $('#chkBonus').bootstrapSwitch('state', result.Data.Bonus);                
            }
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });

    $('#modalScheduleEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalScheduleEdit .modal-dialog .modal-header center .modal-title strong').html("Controle de Agendamento");
    $('#modalScheduleEdit').modal({ backdrop: 'static', keyboard: false });

    $("#loading-page").hide();
}

function SaveAppointment() {
    var schedule = [];

    var errorMessages = ValidateRequiredFields();

    if (errorMessages !== '') {
        ShowModalAlert(errorMessages);
        return;
    }
    
    schedule = {
        IDSchedule: parseInt($("#hiddenIDSchedule").val()),
        IDCustomer: $("#ddlCustomer").val(),
        IDProfessional: $("#ddlProfessional").val(),
        IDService: $("#ddlService").val(),
        IDConsumer: $("#ddlConsumer").val(),
        Date: kendo.parseDate($("#dtDateTime").val(), "dd/MM/yyyy HH:mm"),
        Price: $("#txtPrice").val(),
        Time: $("#txtTime").val(),
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

    if ($("#dtDateTime").val() === '')
        errorMessage += 'Favor informar a Data / Hora' + '<br/>';

    if ($("#txtPrice").val() === '' || $("#txtPrice").val() === '0,00')
        errorMessage += 'Favor informar o Valor do Serviço' + '<br/>';

    if ($("#txtTime").val() === '')
        errorMessage += 'Favor informar o Tempo de execução';
    
    return errorMessage;
}