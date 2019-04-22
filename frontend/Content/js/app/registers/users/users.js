function PageSetup() {
    $('#btnAddUser').click(function () {
        AddUser();
    });

    $('#btnSearch').click(function () {
        LoadUsers();
    });

    $('#btnClear').click(function () {
        document.getElementById("txtNameFilter").value = "";
        document.getElementById("txtLoginFilter").value = "";

        $("#ddlCustomerFilter").data("kendoDropDownList").select(0);
        $("#ddlUserGroup").data("kendoDropDownList").select(0);      

        LoadUsers();
    });

    LoadCombo("/Customers/GetCompanyNameCombo", ['#ddlCustomerFilter', '#ddlCustomer'], "IDCustomer", "CompanyName", true);
    LoadCombo("/Users/GetUserGroupCombo", ['#ddlUserGroupFilter', '#ddlUserGroup'], "IDUserGroup", "GroupDescription", false);

    LoadUsers();
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
                            login: $('#txtLoginFilter').val(),
                            idCustomer: $("#ddlCustomerFilter").val() === '' ? '0' : $("#ddlCustomerFilter").val(),
                            idUserGroup: $("#ddlUserGroupFilter").val() === '' ? '0' : $("#ddlUserGroupFilter").val()
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
            { field: "IDUser", hidden: true },
            { field: "FullName", title: "Nome", width: "50%" },
            { field: "CustomerName", title: "Razão Social", width: "20%" },
            { field: "GroupDescription", title: "Grupo", width: "20%" },
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

function AddUser() {
    CleanFields();

    $('#modalUserEdit .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalUserEdit .modal-dialog .modal-header center .modal-title strong').html("Cadastro do Usuário");
    $('#modalUserEdit').modal({ backdrop: 'static', keyboard: false });
}

function CleanFields() {
    $("#hiddenIDUser").val(0);

    $("#txtName").val("");
    $("#txtLogin").val("");
    $("#txtEmail").val("");

    if (!$('#ddlCustomerFilter').prop('disabled')) {
        var ddlCustomer = $("#ddlCustomer").data("kendoDropDownList");
        ddlCustomer.select(0);
    }

    var ddlUserGroup = $("#ddlUserGroup").data("kendoDropDownList");
    ddlUserGroup.select(0);

    $('#chkActive').bootstrapSwitch('state', true);
}

function UserEdit(e) {
    var dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);
    $("#loading-page").show();

    CleanFields();

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Users/GetUser",
        data: { "idUser": dataItem.IDUser },
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) {
                $("#hiddenIDUser").val(result.Data.IDUser);
                $("#txtName").val(result.Data.FullName);
                $("#txtLogin").val(result.Data.UserName);
                $("#txtEmail").val(result.Data.Email);
                $("#ddlCustomer").data('kendoDropDownList').value(result.Data.IDCustomer);
                $("#ddlUserGroup").data('kendoDropDownList').value(result.Data.IDUserGroup);
                $('#chkActive').bootstrapSwitch('state', result.Data.Active);

                LockFields(CheckUserIsConsumer(result.Data.IDUserGroup));
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
        IDUser: parseInt($("#hiddenIDUser").val()),
        FullName: $("#txtName").val(),
        UserName: $("#txtLogin").val(),
        Email: $("#txtEmail").val(),
        IDCustomer: $("#ddlCustomer").val(),
        IDUserGroup: $("#ddlUserGroup").val(),
        Active: $('#chkActive').bootstrapSwitch('state')
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
                ShowModalAlert("Erro ao gravar alterações.");
        }
    });
}

function ValidateRequiredFields() {
    var errorMessage = '';
    var emailPattern = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;

    if ($("#txtName").val() === '')
        errorMessage += 'Favor informar o Nome' + '<br/>';

    if ($("#txtLogin").val() === '')
        errorMessage += 'Favor informar o Login' + '<br/>';

    if ($("#txtEmail").val() === '')
        errorMessage += 'Favor informar o E-mail' + '<br/>';
    else {
        if (!emailPattern.test($.trim($("#txtEmail").val())))
            errorMessage += 'E-mail inválido' + '<br/>';
    }

    if ($("#ddlCustomer").val() === '')
        errorMessage += 'Favor informar o Cliente' + '<br/>';

    if ($("#ddlUserGroup").val() === '')
        errorMessage += 'Favor informar o Grupo do Usuário';    

    return errorMessage;
}

function CheckUserIsConsumer(idUserGroup) {
    var isConsumer = false;

    $.ajax({
        url: "/Users/CheckUserIsConsumer",
        data: { "idUserGroup": idUserGroup },
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
    $("#txtLogin").prop('disabled', lock);
    $("#txtEmail").prop('disabled', lock);
    $("#ddlCustomer").data("kendoDropDownList").enable(!lock);
    $("#ddlUserGroup").data("kendoDropDownList").enable(!lock);    
}