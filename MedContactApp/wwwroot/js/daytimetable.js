var buttoncalc = document.getElementById("calc");
buttoncalc.addEventListener("click", e => {
    e.preventDefault();
    calctimetable();
});
function calctimetable() {
    var startWorkTime = document.getElementById("StartWorkTime").value;
    var startHrs = parseInt(startWorkTime.split(":")[0]);
    var startMins = parseInt(startWorkTime.split(":")[1]);
    var finishWorkTime = document.getElementById("FinishWorkTime").value;
    var finishHrs = parseInt(finishWorkTime.split(":")[0]);
    var finishMins = parseInt(finishWorkTime.split(":")[1]);
    var consultDuration = parseInt(document.getElementById("ConsultDuration").value);
    var ticketQty = Math.floor((finishHrs * 60 + finishMins - (startHrs * 60 + startMins)) / consultDuration); 
    alert(ticketQty);
}