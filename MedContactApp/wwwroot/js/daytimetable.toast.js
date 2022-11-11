var buttoncalc = document.getElementById("calc");
let startWorkTime = document.getElementById("StartWorkTime");
let finishWorkTime = document.getElementById("FinishWorkTime");
let consultDuration = document.getElementById("ConsultDuration");
let calctext = document.getElementById("calctext");
let consultTime = document.getElementById("consultTime");
//let toastBody = document.getElementById("toastBody");
//let toastElement = document.getElementById("myToast");
let toastContainer = document.getElementById("toastContainer");

function calctimetable() {

    let text;
    let startWorkTimeVal = startWorkTime.value;
    let startHrs = parseInt(startWorkTimeVal.split(":")[0]);
    let startMins = parseInt(startWorkTimeVal.split(":")[1]);

    let finishWorkTimeVal = finishWorkTime.value;
    let finishHrs = parseInt(finishWorkTimeVal.split(":")[0]);
    let finishMins = parseInt(finishWorkTimeVal.split(":")[1]);

    let consultDurationVal = parseInt(consultDuration.value);
    time = "Consult time " + consultDurationVal + " mins";  

    let ticketQty = Math.floor((finishHrs * 60 + finishMins - (startHrs * 60 + startMins)) / consultDurationVal);

    if (isNaN(ticketQty) || consultDurationVal <= 0 || ticketQty < 0)
        text = 'Input correct values!'; 
    else 
        text = `Estimated quantity of tickets is ${ticketQty}`; 

   
    let num = (Math.floor(Math.random() * 1000));
    let newId = `myToast` +num;

                     let toast = `<div class="toast" id="${newId}" data-bs-autohide="false">\
                                  <div class="toast-header">\
                                  <strong class="me-auto"><i class="bi bi-calendar2-event"></i> Appointment Quantity</strong>\
                                  <small id="consultTime">${time}</small>\
                                  <button type="button" class="btn-close" data-bs-dismiss="toast"></button>\
                                  </div>\
                                  <div class="toast-body" id="toastBody">${text}</div>\
                                  </div>`;
    let toastHtml = htmlToElement(toast);

    toastContainer.appendChild(toastHtml);
    let newToast = document.getElementById(newId);
    let myToast = new bootstrap.Toast(newToast);
    myToast.show();  

}

function htmlToElement(html) {
    var template = document.createElement('template');
    html = html.trim(); 
    template.innerHTML = html;
    return template.content.firstChild;
}