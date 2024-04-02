var dp = new DayPilot.Scheduler("dp", {
    timeHeaders: [{ "groupBy": "Month" }, { "groupBy": "Day", "format": "d" }],
    scale: "Day",
    days: DayPilot.Date.today().daysInMonth(),
    startDate: DayPilot.Date.today().firstDayOfMonth(),
    timeRangeSelectedHandling: "Enabled",
    eventMoveHandling: "Disabled",
    onBeforeCellRender: function (args) {
        if (dp.zoom.active == 0) {
            for (let i = 0; i < 24; ++i) {
                if (args.cell.x === i && args.cell.y === 0) {
                    args.cell.properties.html = `<div style="padding-top:7px;text-align:center;">${dp.events.list.filter(e => (new Date(e.start).getHours() == i || new Date(e.end).getHours() == i && new Date(e.end).getMinutes() > 0) && new Date(e.start).toDateString() == new Date(dp.zoomLevels[0].properties.startDate).toDateString()).length}</div>`;
                    args.cell.properties.backColor = "#FEADF9";
                }
            }
        }
        else if (dp.zoom.active == 1) {
            for (let i = 0; i < 7; ++i) {
                if (args.cell.x === i && args.cell.y === 0) {
                    args.cell.properties.html = `<div style="padding-top:7px;text-align:center;">${dp.events.list.filter(e => new Date(e.start).toDateString() == new Date(args.cell.start).toDateString()).length}</div>`;
                    args.cell.properties.backColor = "#FEADF9";
                }
            }
        }
    },
    zoomLevels: [
        {
            name: "Day",
            properties: {
                scale: "CellDuration",
                cellDuration: 60,
                cellWidth: 59,
                timeHeaders: [{ groupBy: "Hour", format:"htt" }],
                startDate: DayPilot.Date.today(),
                days: function () { return 1; }
            }
        },
        {
            name: "Week",
            properties: {
                scale: "Day",
                cellWidth: 200,
                timeHeaders: [{ groupBy: "Day", format: "ddd d" }],
                startDate: DayPilot.Date.today(),
                days: function () { return 7; },
            }
        },
        {
            name: "Month",
            properties: {
                scale: "CellDuration",
                cellDuration: 720,
                cellWidth: 40,
                timeHeaders: [{ groupBy: "Month" }, { groupBy: "Day", format: "ddd d" }, { groupBy: "Cell", format: "tt" }],
                startDate: function (args) { return args.date.firstDayOfMonth(); },
                days: function (args) { return args.date.daysInMonth(); },
            }
        },
    ],
    treeEnabled: true,
});
dp.zoom.setActive(0);
dp.init();

const monthCalendar = new DayPilot.Month("dp-month", {
    cellMarginBottom: 20,
    startDate: DayPilot.Date.today(),
    eventMoveHandling: "Disabled",
    onEventResized: (args) => {
        dp.message("Resized: " + args.e.text());
    },
    onEventClicked: (args) => {
        DayPilot.Modal.alert(args.e.text());
    }



});
monthCalendar.init();

const appp = {
    loadData: function () {

        const resources = [
            { html: "<b>Coverage</b>", id: "R0" },
            { name: "Resource 1", id: "R1" },
            { name: "Resource 2", id: "R2" },
            { name: "Resource 3", id: "R3" },
            { name: "Resource 4", id: "R4" },
            { name: "Resource 5", id: "R5" },
            { name: "Resource 6", id: "R6" },
            { name: "Resource 7", id: "R7" },
            { name: "Resource 8", id: "R8" },
            { name: "Resource 9", id: "R9" },
        ];

        const events = [
            {
                id: 1,
                text: "Event 1",
                start: "2024-03-31T12:00:00",
                end: "2024-03-31T13:00:00",
                resource: "R2"
            },
            {
                id: 2,
                text: "Event 2",
                start: "2024-04-03T13:00:00",
                end: "2024-04-03T14:00:00",
                resource: "R4",
                backColor: "#FEADF9"
            },
            {
                id: 4,
                text: "Event 4",
                start: "2024-04-03T14:00:00",
                end: "2024-04-03T15:00:00",
                resource: "R4",
                backColor: "#FEADF9"
            },
            {
                id: 3,
                text: "Event 3",
                start: "2024-04-01T13:30:00",
                end: "2024-04-01T14:30:00",
                resource: "R3"
            },
        ];

        dp.update({ resources, events });
        monthCalendar.update({ events });
    },
    init() {
        this.loadData();
    }
};
appp.init();

var elements = {
    day: document.getElementById("button-day"),
    week: document.getElementById("button-week"),
    month: document.getElementById("button-month")
};

const schedulerElement = document.getElementById("dp");
const monthCalendarElement = document.getElementById("dp-month");


elements.day.addEventListener("click", function (ev) {
    dp.zoom.setActive(0);
    loadHeading();

    const buttons = document.getElementsByClassName("contact-provider");
    for (let i = 0; i < buttons.length; ++i) {
        buttons[i].classList.remove("calendar-view-active");
    }

    elements.day.classList.add("calendar-view-active")

    schedulerElement.style.display = "block";
    monthCalendarElement.style.display = "none";


    document.getElementById("main-date").innerHTML = new DayPilot.Date(dp.zoomLevels[0].properties.startDate).toString("dddd, MMM d, yyyy")

});

elements.week.addEventListener("click", function (ev) {
    dp.zoom.setActive(1);
    loadHeading();

    const buttons = document.getElementsByClassName("contact-provider");
    for (let i = 0; i < buttons.length; ++i) {
        buttons[i].classList.remove("calendar-view-active");
    }

    elements.week.classList.add("calendar-view-active")

    schedulerElement.style.display = "block";
    monthCalendarElement.style.display = "none";



    document.getElementById("main-date").innerHTML = `${new DayPilot.Date(dp.zoomLevels[1].properties.startDate).toString("MMM d")} - ${new DayPilot.Date(dp.zoomLevels[1].properties.startDate).addDays(7).toString("MMM d, yyyy")}`


});

elements.month.addEventListener("click", function (ev) {
    dp.zoom.setActive(2);

    const buttons = document.getElementsByClassName("contact-provider");
    for (let i = 0; i < buttons.length; ++i) {
        buttons[i].classList.remove("calendar-view-active");
    }

    elements.month.classList.add("calendar-view-active")

    schedulerElement.style.display = "none";
    monthCalendarElement.style.display = "block";


    document.getElementById("main-date").innerHTML = new DayPilot.Date(monthCalendar.startDate).toString("MMM yyyy")


});

const loadHeading = () => {
    const heading = document.getElementsByClassName("scheduler_default_corner_inner")
    for (let i = 0; i < heading.length; ++i) {
        heading[0].innerHTML = "Staff";
    }
}

loadHeading()

function changeStartDate(selectedDate) {
    console.log(selectedDate)
    const startDate = new DayPilot.Date(selectedDate);
    const givenDate = new Date(selectedDate);
    const mondayOfWeek = getMondayOfWeek(givenDate);
    dp.zoomLevels[0].properties.startDate = startDate;
    dp.zoomLevels[1].properties.startDate = new DayPilot.Date(mondayOfWeek);
    if (dp.zoom.active == 0) {
        dp.startDate = startDate;
    }
    else if (dp.zoom.active == 1) {
        dp.startDate = new DayPilot.Date(mondayOfWeek);
    }
    dp.update();
    monthCalendar.update({ startDate: startDate });
    if (dp.zoom.active == 0) {
        document.getElementById("main-date").innerHTML = dp.zoomLevels[0].properties.startDate.toString("dddd, MMM d, yyyy")
    }
    else if (dp.zoom.active == 1) {
        document.getElementById("main-date").innerHTML = `${new DayPilot.Date(dp.zoomLevels[1].properties.startDate).toString("MMM d")} - ${new DayPilot.Date(dp.zoomLevels[1].properties.startDate).addDays(7).toString("MMM d, yyyy")}`
    }
    else if (dp.zoom.active == 2) {
        document.getElementById("main-date").innerHTML = monthCalendar.startDate.toString("MMM yyyy")
    }
    if (dp.zoom.active == 2) {
        monthCalendarElement.style.display = "block";
    }
}


function getMondayOfWeek(date) {
    const dayOfWeek = date.getDay();
    const diff = date.getDate() - dayOfWeek + (dayOfWeek === 0 ? -6 : 1);
    return new Date(date.setDate(diff));
}

const app = {
    elements: {
        previous: document.getElementById("previous"),
        next: document.getElementById("next")
    },
    init() {
        app.elements.previous.addEventListener("click", (e) => {
            e.preventDefault();
            app.changeDate(monthCalendar.startDate.addMonths(-1), dp.zoomLevels[0].properties.startDate.addDays(-1), dp.zoomLevels[1].properties.startDate.addDays(-7));
        });
        app.elements.next.addEventListener("click", (e) => {
            e.preventDefault();
            app.changeDate(monthCalendar.startDate.addMonths(1), dp.zoomLevels[0].properties.startDate.addDays(1), dp.zoomLevels[1].properties.startDate.addDays(7));
        });
    },
    changeDate(dateM, dateD, dateW) {
        const mondayOfWeek = getMondayOfWeek(new Date(dateW));
        var startDate = dateM.firstDayOfMonth();
        const weekStartDate = new Date(mondayOfWeek);
        const days = dp.startDate.daysInMonth();
        
        if (dp.zoom.active == 0) {
            dp.startDate = dateD;
            dp.zoomLevels[0].properties.startDate = dateD;
        }
        else if (dp.zoom.active == 1) {
            dp.startDate = new DayPilot.Date(mondayOfWeek);
            dp.zoomLevels[1].properties.startDate = new DayPilot.Date(mondayOfWeek);
        }
        else if (dp.zoom.active == 2) {
            monthCalendar.startDate = startDate;
        }

        monthCalendar.update();
        dp.update();

        if (dp.zoom.active == 0) {
            document.getElementById("main-date").innerHTML = dp.zoomLevels[0].properties.startDate.toString("dddd, MMM d, yyyy")
        }
        else if (dp.zoom.active == 1) {
            document.getElementById("main-date").innerHTML = `${new DayPilot.Date(dp.zoomLevels[1].properties.startDate).toString("MMM d")} - ${new DayPilot.Date(dp.zoomLevels[1].properties.startDate).addDays(7).toString("MMM d, yyyy")}`
        }
        else if (dp.zoom.active == 2) {
            document.getElementById("main-date").innerHTML = monthCalendar.startDate.toString("MMM yyyy")
        }

        if (dp.zoom.active == 2) {
            monthCalendarElement.style.display = "block";
        }
    }
}
app.init();

document.getElementById("main-date").innerHTML = DayPilot.Date.today().toString("dddd, MMM d, yyyy")
console.log(DayPilot.Date.today().toString("dddd, MMM d, yyyy"))