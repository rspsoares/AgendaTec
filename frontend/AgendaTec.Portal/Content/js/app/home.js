function PageSetup() {
    var customerHours = GetCustomerHours();

    if (customerHours.startTime !== undefined || customerHours.endTime !== undefined) {
        var appointments = GetAppointments();

        var calendarEl = document.getElementById('calendar');
        var calendar = new FullCalendar.Calendar(calendarEl, {
            plugins: ['timeGrid'],

            timeZone: 'local',
            locale: 'pt-br',
            defaultView: 'timeGridDay',
            header: {
                left: '',
                center: 'title',
                right: ''
            },
            views: {
                timeGridDay: {
                    titleFormat: { year: 'numeric', month: 'long', day: 'numeric' }
                }
            },
            minTime: kendo.toString(kendo.parseDate(customerHours.startTime), 'HH:mm'),
            maxTime: kendo.toString(kendo.parseDate(customerHours.endTime), 'HH:mm'),
            allDaySlot: false,
            events: appointments
        });

        calendar.render();
    } 
}

function GetCustomerHours() {
    var customerHours = [];

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Customers/GetCustomerHours",        
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) {
                customerHours = {
                    startTime: result.Data.Start,
                    endTime: result.Data.End
                };
            }
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });

    return customerHours;
}

function GetAppointments() {
    var appointments = [];

    $.ajax({
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: "json",
        url: "/Schedules/GetTodaysAppointments",
        cache: false,
        async: false,
        success: function (result) {
            if (result.Success) 
                appointments = result.Data;            
            else {
                ShowModalAlert(result.errorMessage);
                return;
            }
        }
    });

    return appointments;
}