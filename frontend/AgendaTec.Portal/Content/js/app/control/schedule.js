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
    var dtNewDate = $("#dtNewDate");
    dtNewDate.kendoMaskedTextBox({
        mask: "00/00/0000"
    });
    dtNewDate.kendoDatePicker({
        format: "dd/MM/yyyy",
        min: new Date()
    });
    dtNewDate.closest(".k-datepicker")
        .add(dtNewDate)
        .removeClass("k-textbox");

    $("#dtDateTime").kendoDateTimePicker({
        value: new Date(d.getFullYear(), d.getMonth(), d.getDate(), 9, 0, 0),
        min: new Date(d.getFullYear(), d.getMonth(), d.getDate(), 9, 0, 0)//, 
        //open: function (e) {
        //    if (e.view === "time") {
        //        e.sender.timeView.dataBind([
        //            new Date(1970, 0, 1, 9, 10),
        //            new Date(1970, 0, 1, 11, 20),
        //            new Date(1970, 0, 1, 13, 30),
        //            new Date(1970, 0, 1, 15, 40),
        //            new Date(1970, 0, 1, 17, 50),
        //            new Date(1970, 0, 1, 19, 10),
        //            new Date(1970, 0, 1, 21, 20)
        //        ]);
            //}
        //}
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

    LoadCombo("/Customers/GetCompanyNameCombo", ['#ddlCustomerFilter', '#ddlCustomer'], "Id", "Name", true, undefined);
    
    $("#ddlCustomerFilter")
        .data("kendoDropDownList")
        .bind("change", ddlCustomerFilterChange);

    $("#ddlCustomer")
        .data("kendoDropDownList")
        .bind("change", ddlCustomerChange);

    LoadComboFiltered("/Professionals/GetProfessionalNameCombo", '#ddlProfessionalFilter', 'Id', 'Name', $("#ddlCustomerFilter").val(), false, 'Selecione...');
    LoadComboFiltered("/Professionals/GetProfessionalNameCombo", '#ddlProfessional', 'Id', 'Name', $("#ddlCustomer").val(), false);

    LoadComboFiltered("/Services/GetServiceNameCombo", '#ddlServiceFilter', 'Id', 'Description', $("#ddlCustomerFilter").val(), false, 'Selecione...');
    LoadComboFiltered("/Services/GetServiceNameCombo", '#ddlService', 'Id', 'Description', $("#ddlCustomer").val(), false);

    $("#ddlService")
        .data("kendoDropDownList")
        .bind("change", ddlServiceChange);

    LoadComboFiltered("/Users/GetConsumerNamesCombo", '#ddlConsumerFilter', 'Id', 'FullName', $("#ddlCustomerFilter").val(), false, 'Selecione...');
    LoadComboFiltered("/Users/GetConsumerNamesCombo", '#ddlConsumer', 'Id', 'FullName', $("#ddlCustomer").val(), false);

    LoadSchedules();
}

function ddlCustomerFilterChange(e) {
    LoadComboFiltered("/Professionals/GetProfessionalNameCombo", '#ddlProfessionalFilter', 'Id', 'Name', $("#ddlCustomerFilter").val(), false, 'Selecione...');
    LoadComboFiltered("/Services/GetServiceNameCombo", '#ddlServiceFilter', 'Id', 'Description', $("#ddlCustomerFilter").val(), false, 'Selecione...');
    LoadComboFiltered("/Users/GetConsumerNamesCombo", '#ddlConsumerFilter', 'Id', 'FullName', $("#ddlCustomerFilter").val(), false, 'Selecione...');
}

function ddlCustomerChange(e) {
    LoadComboFiltered("/Professionals/GetProfessionalNameCombo", '#ddlProfessional', 'Id', 'Name', $("#ddlCustomer").val(), false);
    LoadComboFiltered("/Services/GetServiceNameCombo", '#ddlService', 'Id', 'Description', $("#ddlCustomer").val(), false);
    LoadComboFiltered("/Users/GetConsumerNamesCombo", '#ddlConsumer', 'Id', 'FullName', $("#ddlCustomer").val(), false);
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
                $("#txtPrice").val(result.Data.Price.FormatMoney(2, '', '.', ','));
                $("#txtTime").val(result.Data.Time);                
            }
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });
}

function LoadSchedules() {
    var dateInitial = kendo.parseDate($("#dtDateFromFilter").val(), "dd/MM/yyyy");
    var dateFinal = kendo.parseDate($("#dtDateToFilter").val(), "dd/MM/yyyy");

    if (dateInitial !== null && dateFinal !== null && dateInitial > dateFinal) {
        ShowModalAlert("A Data Inicial não pode ser maior do que a Data Final.");
        return;
    }

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
                if (rowIndex % 2 === 0) {
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
                    id: "Id"
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
                            dateFrom: $("#dtDateFromFilter").val(),
                            dateTo: $("#dtDateToFilter").val(),
                            bonus: $('#chkBonusFilter').bootstrapSwitch('state')
                        };
                    }
                }
            },            
            pageSize: 10
        },        
        groupable: true,        
        reorderable: true,
        change: onChange,
        scrollable: true,
        resizable: true,
        sortable: true,
        selectable: "multiple",
        dataBound: function (e) {
            var grid = this;
            grid.tbody.find("tr").dblclick(function (e) {                                
                AppointmentEdit(undefined, grid.dataItem(this));                
            });
        },
        pageable: {
            pageSizes: [10, 25, 50]
        },
        columns: [
            { field: "Id", hidden: true },
            { field: "IdProfessional", hidden: true },
            { field: "Time", hidden: true },
            { field: "Price", hidden: true },
            { field: "ProfessionalName", title: "Profissional", width: "20%" },
            //{ field: "ServiceName", title: "Serviço", width: "15%" },
            { field: "ConsumerName", title: "Cliente", width: "20%" },
            { field: "Date", title: "Data", width: "15%" },
            { field: "Hour", title: "Início", width: "15%" },
            { field: "Finish", title: "Término", width: "15%" },
            //{ field: "Bonus", title: "Bônus", width: "10%", template: "#:BonusDescription(Bonus)#" },
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

function Reschedule_click() {    
    var checkDifferentDates = true;

    if (!CheckAnyRowSelected()) {
        ShowModalAlert("Favor selecionar algum compromisso para ser reagendado.");
        return;
    }

    var appointments = GetSelectedAppointments();
    var referenceDate = appointments[0].Date.setHours(0, 0, 0, 0);

    jQuery.each(appointments, function (i, item) {
        if (item.Date.setHours(0, 0, 0, 0) !== referenceDate) {
            ShowModalAlert("Somente compromissos do mesmo dia podem ser reagendados em lote.");
            checkDifferentDates = false;
        }
    });

    if (!checkDifferentDates)
        return;

    ShowModalReschedule();

    return true;
}

function ShowAvailabilityPopup() {
    $('#modalAvailability .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalAvailability .modal-dialog .modal-header center .modal-title strong').html("Sobreposição de compromisso");    
    $('#modalAvailability').modal({ backdrop: 'static', keyboard: false });
}

function RescheduleAppointments() {    
    var appointments = GetSelectedAppointments();

    $('#modalReschedule').modal("hide");

    $.ajax({
        url: '/Schedules/RescheduleAppointment',
        data: JSON.stringify({ schedules: appointments, newDate: kendo.parseDate($("#dtNewDate").val(), "dd/MM/yyyy") }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {           
            if (result.Success) {
                ShowModalSucess("Reagendamento concluído com sucesso.");                
                LoadSchedules();
            }
            else
                ShowModalAlert(result.errorMessage);
        }
    });
}

function ShowModalReschedule() {
    $("#dtNewDate").val("");

    $('#modalReschedule .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalReschedule .modal-dialog .modal-header center .modal-title strong').html("Reagendamento");
    $('#modalReschedule').modal({ backdrop: 'static', keyboard: false });
}

function Delete_click() {
    if (!CheckAnyRowSelected()) {
        ShowModalAlert("Favor selecionar algum compromisso para ser excluído.");
        return;
    }
        
    $('#modalDeleteConfirmation').modal({ backdrop: 'static', keyboard: false });
}

function DeleteAppointments() {
    var appointments = GetSelectedAppointments();

    $.ajax({
        url: '/Schedules/DeleteAppointments',
        data: JSON.stringify({ schedules: appointments }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Success) {
                ShowModalSucess("Compromissos excluídos com sucesso.");                
                LoadSchedules();
            }
            else
                ShowModalAlert("Erro ao excluir compromissos.");
        }
    });

    $('#modalDeleteConfirmation').modal("hide");

    return true;
}

function CheckAnyRowSelected() {
    return selectedRows !== undefined && selectedRows.length > 0;
}

function GetSelectedAppointments() {
    var appointments = [];
    selectedRows.each(function (e) {
        var appointment = {};
        var row = $("#grid").data("kendoGrid").dataItem(this);

        appointment = {
            Id: row.Id,     
            IdProfessional: row.IdProfessional,
            Date: kendo.parseDate(row.Date + ' ' + row.Hour, "yyyy-MM-dd HH:mm"),         
            Time: row.Time,
            Price: row.Price
        };

        appointments.push(appointment);        
    });    

    return appointments;
}

function onChange(e) {
    selectedRows = e.sender.select();   
}

function BonusDescription(bonus) {
    if (bonus === true)
        return "Sim";
    else
        return "Não";
}

function AddAppointment() {
    CleanFields(true);

    $('#modalScheduleEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalScheduleEdit .modal-dialog .modal-header center .modal-title strong').html("Controle de Agendamento");
    $('#modalScheduleEdit').modal({ backdrop: 'static', keyboard: false });
}

function CleanFields(loadFilterCombos) {
    $("#hiddenId").val(0);   

    if (loadFilterCombos) {
        $("#ddlCustomer").data('kendoDropDownList').value($("#ddlCustomerFilter").val());
        ddlCustomerChange();
        $("#ddlProfessional").data('kendoDropDownList').value($("#ddlProfessionalFilter").val());
        $("#ddlConsumer").data('kendoDropDownList').value($("#ddlConsumerFilter").val());
    }
    
    $("#dtDateTime").val("");
    $("#txtPrice").val("");
    $("#txtTime").val("");
    $('#chkBonus').bootstrapSwitch('state', true);
}

function AppointmentEdit(e, selectedRow) {
    var dataItem;

    if (selectedRow === undefined)
        dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);
    else 
        dataItem = selectedRow;
    
    $("#loading-page").show();

    CleanFields(false);

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Schedules/GetAppointment",
        data: { "idSchedule": dataItem.Id },
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) {
                $("#hiddenId").val(result.Data.Id);
                $("#ddlCustomer").data('kendoDropDownList').value(result.Data.IdCustomer);
                ddlCustomerChange();
                $("#ddlProfessional").data('kendoDropDownList').value(result.Data.IdProfessional);
                $("#ddlService").data('kendoDropDownList').value(result.Data.IdService);
                $("#ddlConsumer").data('kendoDropDownList').value(result.Data.IdConsumer);
                $("#dtDateTime").val(kendo.toString(kendo.parseDate(result.Data.Date + ' ' + result.Data.Hour, 'yyyy-MM-dd HH:mm'), 'dd/MM/yyyy HH:mm'));
                $("#txtPrice").val(result.Data.Price.FormatMoney(2, '', '.', ','));
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

function SaveAppointment_click() {
    var errorMessages = ValidateRequiredFields();

    if (errorMessages !== '') {
        ShowModalAlert(errorMessages);
        return;
    }

    var appointment = GetAppointment();

    $.ajax({
        url: '/Schedules/CheckAvailability',
        data: JSON.stringify({ schedule: appointment }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Success && result.errorMessage !== '') {                                
                result.errorMessage = result.errorMessage + '<br />' + 'Deseja continuar?';
                
                $('#modalOverlapConfirmation').modal({ backdrop: 'static', keyboard: false });                
                $('#modalOverlapConfirmation .modal-dialog .modal-body .row .form-group').html(result.errorMessage);
            }
            else 
                ShowModalAlert("Erro ao verificar disponibilidade.");                            
        }
    });    
}

function GetAppointment() {
    var schedule = {
        Id: parseInt($("#hiddenId").val()),
        IdCustomer: $("#ddlCustomer").val(),
        IdProfessional: $("#ddlProfessional").val(),
        IdService: $("#ddlService").val(),
        IdConsumer: $("#ddlConsumer").val(),
        Date: kendo.parseDate($("#dtDateTime").val(), "dd/MM/yyyy HH:mm"),
        Price: Math.abs(parseFloat($("#txtPrice").val().replace(/\./g, '').replace(",", "."))),
        Time: $("#txtTime").val(),
        Bonus: $('#chkBonus').bootstrapSwitch('state')
    };

    return schedule;

}

function SaveAppointment() {    

    $('#modalOverlapConfirmation').modal("hide");

    var appointment = GetAppointment();

    $("#loading-page").show();

    $.ajax({
        url: '/Schedules/SaveAppointment',
        data: JSON.stringify({ schedule: appointment }),
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
                ShowModalAlert(result.errorMessage);
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