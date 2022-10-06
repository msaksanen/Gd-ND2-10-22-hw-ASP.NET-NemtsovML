var buttoncalc = document.getElementById("calc");
var startWorkTime = document.getElementById("StartWorkTime");
var finishWorkTime = document.getElementById("FinishWorkTime");
var consultDuration = document.getElementById("ConsultDuration");
var calctext = document.getElementById("calctext");

buttoncalc.addEventListener("click", e => {
    e.preventDefault();
    calctimetable();
});
startWorkTime.addEventListener("click", e => {
    e.preventDefault();
    calctext.innerHTML = "";
});

finishWorkTime.addEventListener("click", e => {
    e.preventDefault();
    calctext.innerHTML = "";
});

consultDuration.addEventListener("click", e => {
    e.preventDefault();
    calctext.innerHTML = "";
});
function calctimetable() {
    var startWorkTimeVal = startWorkTime.value;
    var startHrs = parseInt(startWorkTimeVal.split(":")[0]);
    var startMins = parseInt(startWorkTimeVal.split(":")[1]);

    var finishWorkTimeVal = finishWorkTime.value;
    var finishHrs = parseInt(finishWorkTimeVal.split(":")[0]);
    var finishMins = parseInt(finishWorkTimeVal.split(":")[1]);

    var consultDurationVal = parseInt(consultDuration.value);

    var ticketQty = Math.floor((finishHrs * 60 + finishMins - (startHrs * 60 + startMins)) / consultDurationVal);
    if (isNaN(ticketQty) || consultDurationVal<=0 ) 
        calctext.innerHTML = `<b>Input correct values!</b>`;
    else 
        calctext.innerHTML = `<b>Estimated quantity of tickets is ${ticketQty}</b>`;
      
}