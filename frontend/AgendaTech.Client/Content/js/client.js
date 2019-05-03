function PageSetup() {
    $('#ddlService').kendoDropDownList({
        optionLabel: "Selecione..."
    });

    $('#ddlProfessional').kendoDropDownList({
        optionLabel: "Selecione..."
    });

    $("#calendar").kendoCalendar({
        culture: "pt-BR"
    });

    $('#ddlHour').kendoDropDownList({
        optionLabel: "Selecione..."
    });


}