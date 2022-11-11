let loginnavbar = document.getElementById('login-nav');
let adminnavbar = document.getElementById('admin-nav');
let doctornavbar = document.getElementById('doctor-nav');

const getLoginPreviewUrl = `${window.location.origin}/Account/UserLoginPreview`;
const getAdminMenuUrl = `${window.location.origin}/AdminPanel/AdminPanelMenu`;
const getDoctorMenuUrl = `${window.location.origin}/Doctor/DoctorMenu`;

fetch(getDoctorMenuUrl)
    .then(function (response) {
        return response.text();
    }).then(function (result) {
        doctornavbar.innerHTML = result;
    }).catch(function () {
        console.error('smth goes wrong');
    });

fetch(getAdminMenuUrl)
    .then(function (response) {
        return response.text();
    }).then(function (result) {
        adminnavbar.innerHTML = result;
    }).catch(function () {
        console.error('smth goes wrong');
    });

fetch(getLoginPreviewUrl)
    .then(function (response) {
        return response.text();
    }).then(function (result) {
        loginnavbar.innerHTML = result;
    }).catch(function () {
        console.error('smth goes wrong');
    });

