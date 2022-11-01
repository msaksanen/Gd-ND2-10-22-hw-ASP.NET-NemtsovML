let loginnavbar = document.getElementById('login-nav');
let adminnavbar = document.getElementById('admin-nav');

const getLoginPreviewUrl = `${window.location.origin}/Account/UserLoginPreview`;
const getAdminMenuUrl = `${window.location.origin}/AdminPanel/AdminPanelMenu`;

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

