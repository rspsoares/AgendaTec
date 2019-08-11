function PageSetup() {
    $('#btnSearch').click(function () {
        LoadReport();
    });

    $('#btnClear').click(function () {
        $("#ddlCustomerFilter").data("kendoDropDownList").select(0);
        ddlCustomerFilterChange();
        $("#dtDateFromFilter").val("");
        $("#dtDateToFilter").val("");        
        LoadReport();
    });

    var date = new Date();
    var dtDateFromFilter = $("#dtDateFromFilter");
    dtDateFromFilter.kendoMaskedTextBox({
        mask: "00/00/0000"
    });
    dtDateFromFilter.kendoDatePicker({
        format: "dd/MM/yyyy",
        value: new Date(date.getFullYear(), date.getMonth(), 1)
    });
    dtDateFromFilter.closest(".k-datepicker")
        .add(dtDateFromFilter)
        .removeClass("k-textbox");

    var dtDateToFilter = $("#dtDateToFilter");
    dtDateToFilter.kendoMaskedTextBox({
        mask: "00/00/0000"
    });
    dtDateToFilter.kendoDatePicker({
        format: "dd/MM/yyyy",
        value: new Date(date.getFullYear(), date.getMonth() + 1, 0)
    });
    dtDateToFilter.closest(".k-datepicker")
        .add(dtDateToFilter)
        .removeClass("k-textbox");

    LoadCombo("/Customers/GetCompanyNameCombo", ['#ddlCustomerFilter'], "Id", "Name", true, undefined);

    LoadReport();
}

function LoadReport() {
    var dateInitial = kendo.parseDate($("#dtDateFromFilter").val(), "dd/MM/yyyy");
    var dateFinal = kendo.parseDate($("#dtDateToFilter").val(), "dd/MM/yyyy");

    var filterValidation = ValidateFilter(dateInitial, dateFinal);
    if (filterValidation !== '') {
        ShowModalAlert(filterValidation);
        return;
    }

    $("#grid").html("");
    $("#grid").kendoGrid({
        toolbar: [
            { name: 'excel', text: 'Exportar Excel' }            
        ],
        excel: {
            fileName: 'RelatorioAgendamento_' + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + '.xlsx',
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
                    url: "/SchedulesReport/GetGrid",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idCustomer: $("#ddlCustomerFilter").val(),                        
                            dateFrom: $("#dtDateFromFilter").val(),
                            dateTo: $("#dtDateToFilter").val()                            
                        };
                    }
                }
            },
            pageSize: 10
        },
        groupable: true,
        reorderable: true,        
        scrollable: true,
        resizable: true,
        sortable: true,      
        pageable: {
            pageSizes: [10, 25, 50]
        },
        columns: [
            { field: "ConsumerName", title: "Cliente", width: "20%" },
            { field: "ServiceDescription", title: "Serviço", width: "15%" },
            { field: "Price", title: "Valor", template: '#=Price.FormatMoney(2, "", ".", ",") #', attributes: { style: "text-align:right;" }, width: "15%" },
            { field: "Date", title: "Data", template: "#= kendo.toString(kendo.parseDate(Date, 'yyyy/MM/dd'), 'dd/MM/yyyy') #", width: "100px" },
            { field: "Attended", title: "Compareceu", width: "15%" }                  
        ]
    });
}

function ValidateFilter(dateInitial, dateFinal) { 
    if (dateInitial === null)
        return 'Favor informar a Data Inicial.';

    if(dateFinal === null)
        return 'Favor informar a Data Final.';

    if (dateInitial > dateFinal) 
        return "A Data Inicial não pode ser maior do que a Data Final.";

    return '';
}