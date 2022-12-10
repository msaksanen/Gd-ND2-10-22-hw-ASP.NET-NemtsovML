
    var elems = document.getElementsByClassName('confirmation2');
    var confirmIt = function (e) {
        if (!confirm('Do you really wish to remove selected data?')) e.preventDefault();
    };
    for (var i = 0, l = elems.length; i < l; i++) {
        elems[i].addEventListener('click', confirmIt, false);
    }
