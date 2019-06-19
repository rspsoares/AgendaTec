function PageSetup() {
    $("#calendar").kendoCalendar({
        culture: "pt-BR",
        value: new Date(),
        min: new Date(),
        change: function () {            
            GetAvalailableHours(kendo.toString(kendo.parseDate(this.value(), 'yyyy-MM-dd'), 'dd/MM/yyyy'));
        }
    });
    $('#calendar').data('kendoCalendar').setOptions({ animation: false });

    $("#dtBirthday").mask("99/99/9999");
    $("#txtPhone").mask("(99) 9.9999-9999");
    $('#txtCPF').mask('999.999.999-99');

    GetServices();
    GetProfessionals();    
    GetAvalailableHours(kendo.toString(kendo.parseDate(new Date(), 'yyyy-MM-dd'), 'dd/MM/yyyy'));

    $('#btnSaveAppointment').click(function () {
        SaveAppointment();
    });
}

function GetProfessionals() {
    var idService = '';

    var service = GetSelectedRow($("#listService").data("kendoListView"));
    if (service !== undefined)
        idService = service.IdService;

    var ds = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/Home/GetProfessionals",
                type: "GET"
            },
            parameterMap: function (data, type) {
                if (type === "read") {
                    return {                        
                        idService: idService                        
                    };
                }
            }
        },
        pageSize: 5
    });

    $("#listProfessional").kendoListView({
        selectable: "single",
        dataSource: ds,
        template: "<div style='margin: 4px'>#:Name#</div>",
        change: function () {            
            GetAvalailableHours(kendo.toString(kendo.parseDate($("#calendar").data("kendoCalendar").value(), 'yyyy-MM-dd'), 'dd/MM/yyyy'));
        }
    });    
}

function GetServices() {
    var ds = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/Home/GetServices",
                type: "GET"
            }            
        },
        pageSize: 5
    });

    $("#listService").kendoListView({       
        selectable: "single",
        navigatable: true,
        dataSource: ds,
        template: "<div style='margin: 4px'>#:Description#</div>",
        change: function () {
            GetProfessionals();
            GetAvalailableHours(kendo.toString(kendo.parseDate($("#calendar").data("kendoCalendar").value(), 'yyyy-MM-dd'), 'dd/MM/yyyy'));
        }
    });
}

function GetAvalailableHours(value) {
    var idProfessional = '';
    var idService = '';
    
    var service = GetSelectedRow($("#listService").data("kendoListView"));
    if (service !== undefined)
        idService = service.IdService;

    var professional = GetSelectedRow($("#listProfessional").data("kendoListView"));
    if (professional !== undefined)
        idProfessional = professional.Id;

    var ds = new kendo.data.DataSource({
        transport: {
            read: {
                url: "/Home/GetAvailableHours",
                type: "GET"
            },
            parameterMap: function (data, type) {
                if (type === "read") {
                    return {
                        idProfessional: idProfessional,
                        idService: idService,
                        selectedDate: value
                    };
                }
            }
        }
    });

    $("#listHours").kendoListView({
        selectable: "single",
        dataSource: ds,
        template: "<div style='margin: 4px'>#=data#</div>"
    });

}

function GetSelectedRow(kendoListView) {
    var row = undefined;    
    var selected = kendoListView.select();
    var index = $(selected[0]).index();

    if (index > -1) 
        row = kendoListView.dataSource.view()[index];

    return row;
}

function SaveAppointment() {
    var schedule = [];

    var service = GetSelectedRow($("#listService").data("kendoListView"));  
    var professional = GetSelectedRow($("#listProfessional").data("kendoListView"));    

    var dateTimeAppointment =
        kendo.toString(kendo.parseDate($("#calendar").data("kendoCalendar").value(), 'yyyy-MM-dd'), 'dd/MM/yyyy') +
        ' ' +
        GetSelectedRow($("#listHours").data("kendoListView"));

    schedule = {
        Id: 0,        
        IdProfessional: professional.Id,
        IdService: service.Id,        
        Date: dateTimeAppointment,
        Price: service.Price,
        Time: service.Time,
        Bonus: false
    };

    $("#loading-page").show();

    $.ajax({
        url: '/Home/SaveAppointment',
        data: JSON.stringify({ schedule: schedule }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#loading-page").hide();
            if (result.Success) {
                GetAvalailableHours(kendo.toString(kendo.parseDate($("#calendar").data("kendoCalendar").value(), 'yyyy-MM-dd'), 'dd/MM/yyyy'));
                ShowModalSucess("Agendamento realizado com sucesso.");
            }
            else {

                if (result.errorMessage === 'Required fields')
                    ShowModalUserRequiredFields();                
                else
                    ShowModalAlert(result.errorMessage);
            }                
        }
    });
}

function CloseModal(e) {
    $('#' + e.parentElement.parentElement.parentElement.parentElement.id).data('data', null);
    $('#' + e.parentElement.parentElement.parentElement.parentElement.id).modal("hide");
}

function ShowModalAlert(dataHtml) {
    $('#modalAlert').modal({ backdrop: 'static', keyboard: false });
    $('#modalAlert .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalAlert .modal-dialog .modal-header center .modal-title strong').html("Atenção");
    $('#modalAlert .modal-dialog .modal-body .alert').html("");
    $('#modalAlert .modal-dialog .modal-body .alert').html(dataHtml);
}

function ShowModalSucess(dataHtml) {
    $('#modalSuccess').modal({ backdrop: 'static', keyboard: false });
    $('#modalSuccess .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalSuccess .modal-dialog .modal-header center .modal-title strong').html("Sucesso");
    $('#modalSuccess .modal-dialog .modal-body .alert').html("");
    $('#modalSuccess .modal-dialog .modal-body .alert').html(dataHtml);
}

function ShowModalUserRequiredFields() {    
    $("#loading-page").show();

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Home/GetUserRequiredFields",        
        cache: false,
        async: false,
        success: function (result) {
            $("#loading-page").hide();
            if (result.Success) {
                $("#dtBirthday").val(result.Data.Birthday).mask('99/99/9999');
                $("#txtCPF").val(result.Data.CPF).mask('999.999.999-99');
                $("#txtPhone").val(result.Data.Phone).mask("(99) 9.9999-9999");
            }
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });
    
    $('#modalRequiredFields').modal({ backdrop: 'static', keyboard: false });
}

function SaveUserRequiredFields() {
    var errorMessages = ValidateRequiredFields();

    if (errorMessages !== '') {
        ShowModalAlert(errorMessages);
        return;
    }

    var consumer = {        
        Birthday: $("#dtBirthday").val(),
        CPF: $("#txtCPF").val(),        
        Phone: $("#txtPhone").val()        
    };

    $("#loading-page").show();

    $.ajax({
        url: '/Home/SaveUserRequiredFields',
        data: JSON.stringify({ consumer: consumer }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#loading-page").hide();
            if (result.Success) {
                $('#modalRequiredFields').modal("hide");
                SaveAppointment();
            }
            else
                ShowModalAlert(result.errorMessage);
        }
    });
}

function ValidateRequiredFields() {
    var errorMessage = '';
   
    if (!isValidDate($("#dtBirthday").val()))
        errorMessage += 'Data de Nascimento inválida' + '<br/>';

    if ($("#txtCPF").val() === '')
        errorMessage += 'Favor informar o CPF' + '<br/>';

    if ($("#txtPhone").val() === '')
        errorMessage += 'Favor informar o Celular' + '<br/>';

    return errorMessage;
}

function isValidDate(s) {
    var bits = s.split('/');
    var d = new Date(bits[2] + '/' + bits[1] + '/' + bits[0]);
    return !!(d && (d.getMonth() + 1) == bits[1] && d.getDate() == Number(bits[0]));
}
