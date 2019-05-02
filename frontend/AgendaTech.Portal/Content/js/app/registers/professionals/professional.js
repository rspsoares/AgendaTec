function PageSetup() {
    $('#btnAddProfessional').click(function () {
        AddProfessional();
    });

    $('#btnSearch').click(function () {
        LoadProfessionals();
    });

    $('#btnClear').click(function () {
        $("#ddlCustomerFilter").data("kendoDropDownList").select(0);
        document.getElementById("txtNameFilter").value = "";

        LoadProfessionals();
    });

    $('#txtNameFilter').keydown(function (e) {
        var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;
        if (key === 13) {
            e.preventDefault();
            LoadProfessionals();
        }
    });

    var dtBirthday = $("#dtBirthday");

    dtBirthday.kendoMaskedTextBox({
        mask: "00/00/0000"       
    });

    dtBirthday.kendoDatePicker({
        format: "dd/MM/yyyy",
        max: new Date()
    });

    dtBirthday.closest(".k-datepicker")
        .add(dtBirthday)
        .removeClass("k-textbox");

    LoadCombo("/Customers/GetCompanyNameCombo", ['#ddlCustomerFilter', '#ddlCustomer'], "IDCustomer", "CompanyName", true);
    LoadComboFiltered("/Users/GetProfessionalNameCombo", '#ddlUserName', "Id", "FullName", $("#ddlCustomer").val(), false);

    $("#ddlCustomer")
        .data("kendoDropDownList")
        .bind("change", ddlCustomerChange);

    LoadProfessionals();
}

function ddlCustomerChange(e) {
    $('#ddlUserName').data('kendoDropDownList').dataSource.read();       
}

function LoadProfessionals() {
    $("#grid").html("");
    $("#grid").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: "/Professionals/GetGrid",
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idCustomer: $("#ddlCustomerFilter").val(),
                            name: $('#txtNameFilter').val()
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
            { field: "IDProfessional", hidden: true },
            { field: "Name", title: "Nome", width: "65%" },
            { field: "Phone", title: "Telefone", width: "25%" },            
            {
                title: " ",
                template: "<a onclick='javascript:{ ProfessionalEdit(this); }' class='k-button'>"
                    + "<span class='glyphicon glyphicon glyphicon-pencil'></span></a>",
                width: "10%",
                attributes: { style: "text-align:center;" },
                filterable: false
            }
        ]
    });
}

function AddProfessional() {
    CleanFields();

    $('#ddlUserName').data('kendoDropDownList').dataSource.read();

    $('#modalProfessionalEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalProfessionalEdit .modal-dialog .modal-header center .modal-title strong').html("Cadastro do Profissional");
    $('#modalProfessionalEdit').modal({ backdrop: 'static', keyboard: false });
}

function CleanFields() {
    $("#hiddenIDProfessional").val(0);

    if (!$('#ddlCustomerFilter').prop('disabled')) {
        var dropdownlist = $("#ddlCustomer").data("kendoDropDownList");
        dropdownlist.select(0);
    }

    $("#txtName").val("");
    $("#dtBirthday").val("");
    $("#txtPhone").val("");
    $("#txtEmail").val("");   
}

function ProfessionalEdit(e) {
    var dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);
    $("#loading-page").show();

    CleanFields();

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Professionals/GetProfessional",
        data: { "idProfessional": dataItem.IDProfessional },
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) {
                $("#hiddenIDProfessional").val(result.Data.IDProfessional);
                $("#ddlCustomer").data('kendoDropDownList').value(result.Data.IDCustomer);
                $('#ddlUserName').data('kendoDropDownList').dataSource.read();
                $("#ddlUserName").data('kendoDropDownList').value(result.Data.IDUser);
                $("#txtName").val(result.Data.Name);
                $("#dtBirthday").val(kendo.toString(kendo.parseDate(result.Data.Birthday, 'yyyy-MM-dd'), 'dd/MM/yyyy'));
                $("#txtPhone").val(result.Data.Phone);
                $("#txtEmail").val(result.Data.Email);
            }
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });

    $('#modalProfessionalEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalProfessionalEdit .modal-dialog .modal-header center .modal-title strong').html("Cadastro do Profissional");
    $('#modalProfessionalEdit').modal({ backdrop: 'static', keyboard: false });

    $("#loading-page").hide();
}

function SaveProfessional() {
    var professional = [];

    var errorMessages = ValidateRequiredFields();

    if (errorMessages !== '') {
        ShowModalAlert(errorMessages);
        return;
    }

    professional = {
        IDProfessional: parseInt($("#hiddenIDProfessional").val()),
        IDCustomer: $("#ddlCustomer").val(),
        IDUser: $("#ddlUserName").val(),
        Name: $("#txtName").val(),
        Birthday: kendo.parseDate($("#dtBirthday").val(), "dd/MM/yyyy"),
        Phone: $("#txtPhone").val(),
        Email: $("#txtEmail").val()
    };

    $("#loading-page").show();

    $.ajax({
        url: '/Professionals/SaveProfessional',
        data: JSON.stringify({ professional: professional }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#loading-page").hide();
            if (result.Success) {
                ShowModalSucess("Gravação concluída com sucesso.");
                $('#modalProfessionalEdit').modal("hide");
                LoadProfessionals();
            }
            else
                ShowModalAlert(result.errorMessage);
        }
    });
}

function ValidateRequiredFields() {
    var errorMessage = '';
    var emailPattern = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;

    if ($("#txtName").val() === '')
        errorMessage += 'Favor informar o Nome' + '<br/>';

    if ($("#ddlUserName").val() === '')
        errorMessage += 'Favor informar o Usuário' + '<br/>';

    if ($("#dtBirthday").val() === '')
        errorMessage += 'Favor informar a Data de Nascimento' + '<br/>';

    if ($("#txtPhone").val() === '')
        errorMessage += 'Favor informar o Telefone' + '<br/>';

    if ($("#txtEmail").val() === '')
        errorMessage += 'Favor informar o E-mail' + '<br/>';
    else {
        if (!emailPattern.test($.trim($("#txtEmail").val())))
            errorMessage += 'E-mail inválido' + '<br/>';
    }

    return errorMessage;
}