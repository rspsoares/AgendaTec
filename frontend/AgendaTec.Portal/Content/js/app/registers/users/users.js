function PageSetup() {
    var defaultValue = undefined;

    $('#btnAddUser').click(function () {
        AddUser();
    });

    $('#btnSearch').click(function () {
        LoadUsers();
    });

    $('#btnClear').click(function () {
        document.getElementById("txtNameFilter").value = "";
        document.getElementById("txtEmailFilter").value = "";
        $("#ddlCustomerFilter").data("kendoDropDownList").select(0);
        $("#ddlRole").data("kendoDropDownList").select(0);      

        LoadUsers();
    });

    $("#dtBirthday").mask("99/99/9999");

    $("#txtPhone").mask("(99) 9.9999-9999");
    $('#txtCPF').mask('999.999.999-99');

    $('#chkIsEnabled').bootstrapSwitch();
    $('#chkDirectMail').bootstrapSwitch();

    if (CheckIfCompanyIsRoot())
        defaultValue = "Todas";
    
    LoadCombo("/Customers/GetCompanyNameCombo", ['#ddlCustomerFilter', '#ddlCustomer'], "Id", "Name", true, defaultValue);
    LoadCombo("/Users/GetRoleCombo", ['#ddlRoleFilter', '#ddlRole'], "IdRole", "RoleDescription", false);

    $("#ddlCustomerFilter")
        .data("kendoDropDownList")
        .bind("change", LoadUsers);

    LoadUsers();
}

function CheckIfCompanyIsRoot() {
    var dsData;

    $.ajax({
        url: "/Customers/GetCompanyIsRoot",
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            if (result.Success)
                dsData = result.Data;
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });

    return dsData;
}

function LoadUsers() {
    $("#grid").html("");
    $("#grid").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: "/Users/GetGrid",
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            name: $('#txtNameFilter').val(),
                            email: $('#txtEmailFilter').val(),
                            idCustomer: $("#ddlCustomerFilter").val() === '' ? '0' : $("#ddlCustomerFilter").val(),
                            idRole: $("#ddlRoleFilter").val() === '' ? '0' : $("#ddlRoleFilter").val()
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
            { field: "FullName", title: "Nome", width: "50%" },            
            { field: "RoleDescription", title: "Grupo", width: "15%" },
            { field: "IsEnabled", title: "Status", width: "10%", template: "#:StatusDescription(IsEnabled)#" },     
            {
                title: " ",
                template: "<a onclick='javascript:{ UserEdit(this); }' class='k-button'>"
                    + "<span class='glyphicon glyphicon glyphicon-pencil'></span></a>",
                width: "10%",
                attributes: { style: "text-align:center;" },
                filterable: false
            }
        ]
    });
}

function StatusDescription(status) {
    if (status === true)
        return "Ativo";
    else
        return "Inativo";
}

function AddUser() {
    CleanFields(true);
    LockFields(false);

    $('#modalUserEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalUserEdit .modal-dialog .modal-header center .modal-title strong').html("Cadastro do Usuário");
    $('#modalUserEdit').modal({ backdrop: 'static', keyboard: false });
}

function CleanFields(loadFilterCombos) {
    $("#hiddenId").val("");
    $("#txtFirstName").val("");
    $("#txtLastName").val("");
    $("#dtBirthday").val("");
    $('#txtCPF').val("");
    $("#txtEmail").val("");
    $("#txtPhone").val("");

    if (!$('#ddlCustomerFilter').prop('disabled') && loadFilterCombos)
        $("#ddlCustomer").data('kendoDropDownList').value($("#ddlCustomerFilter").val());

    if (loadFilterCombos) {
        $("#ddlRole").data('kendoDropDownList').value($("#ddlRoleFilter").val());
    }
    else {
        var ddlRole = $("#ddlRole").data("kendoDropDownList");
        ddlRole.select(0);
    }

    $('#chkIsEnabled').bootstrapSwitch('state', true);
    $('#chkDirectMail').bootstrapSwitch('state', true);
}

function UserEdit(e) {
    var dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);
    $("#loading-page").show();

    CleanFields(false);

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Users/GetUser",
        data: { "idUser": dataItem.Id },
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) {
                $("#hiddenId").val(result.Data.Id);
                $("#txtFirstName").val(result.Data.FirstName);
                $("#txtLastName").val(result.Data.LastName);
                $("#dtBirthday").val(kendo.toString(kendo.parseDate(result.Data.Birthday, 'yyyy-MM-dd'), 'dd/MM/yyyy')).mask('99/99/9999');
                $("#txtCPF").val(result.Data.CPF).mask('999.999.999-99');
                $("#txtEmail").val(result.Data.Email);
                $("#txtPhone").val(result.Data.Phone).mask("(99) 9.9999-9999");
                $("#ddlCustomer").data('kendoDropDownList').value($("#ddlCustomerFilter").val());
                $("#ddlRole").data('kendoDropDownList').value(result.Data.IdRole);
                $('#chkIsEnabled').bootstrapSwitch('state', result.Data.IsEnabled);
                $('#chkDirectMail').bootstrapSwitch('state', result.Data.DirectMail);

                LockFields(CheckUserIsConsumer(result.Data.IdRole));
            }
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });

    $('#modalUserEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalUserEdit .modal-dialog .modal-header center .modal-title strong').html("Cadastro do Usuário");
    $('#modalUserEdit').modal({ backdrop: 'static', keyboard: false });

    $("#loading-page").hide();
}

function SaveUser() {
    var userDTO = [];

    var errorMessages = ValidateRequiredFields();

    if (errorMessages !== '') {
        ShowModalAlert(errorMessages);
        return;
    }

    userDTO = {
        Id: $("#hiddenId").val(),
        FirstName: $("#txtFirstName").val(),
        LastName: $("#txtLastName").val(),
        Birthday: $("#dtBirthday").val(),
        CPF: $("#txtCPF").val(),
        Email: $("#txtEmail").val(),
        Phone: $("#txtPhone").val(),
        IDCustomer: $("#ddlCustomer").val(),
        IdRole: $("#ddlRole").val(),
        IsEnabled: $('#chkIsEnabled').bootstrapSwitch('state'),
        DirectMail: $('#chkDirectMail').bootstrapSwitch('state')
    };

    $("#loading-page").show();

    $.ajax({
        url: '/Users/SaveUser',
        data: JSON.stringify({ userDTO: userDTO }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            $("#loading-page").hide();
            if (result.Success) {
                ShowModalSucess("Gravação concluída com sucesso.");
                $('#modalUserEdit').modal("hide");
                LoadUsers();
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

    if ($("#txtEmail").val() === '')
        errorMessage += 'Favor informar o E-mail' + '<br/>';
    else {
        if (!emailPattern.test($.trim($("#txtEmail").val())))
            errorMessage += 'E-mail inválido' + '<br/>';
    }

    if (!isValidDate($("#dtBirthday").val()))
        errorMessage += 'Data de Nascimento inválida' + '<br/>';

    if ($("#txtPhone").val() === '')
        errorMessage += 'Favor informar o Celular' + '<br/>';

    if ($("#ddlCustomer").val() === '')
        errorMessage += 'Favor informar o Cliente' + '<br/>';

    if ($("#ddlRole").val() === '')
        errorMessage += 'Favor informar o Grupo do Usuário';    

    return errorMessage;
}

function CheckUserIsConsumer(idRole) {
    var isConsumer = false;

    $.ajax({
        url: "/Users/CheckUserIsConsumer",
        data: { "idRole": idRole },
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            isConsumer = result.Data;
        }
    });

    return isConsumer;
}

function LockFields(lock) {
    $("#txtName").prop('disabled', lock);    
    $("#txtEmail").prop('disabled', lock);
    $("#txtPhone").prop('disabled', lock);
    $("#ddlCustomer").data("kendoDropDownList").enable(!lock);
    $("#ddlRole").data("kendoDropDownList").enable(!lock);    
}