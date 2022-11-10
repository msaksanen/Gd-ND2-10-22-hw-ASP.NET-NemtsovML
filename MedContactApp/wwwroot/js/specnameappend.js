function update_href(value) {

    let list = document.querySelectorAll('.deletelink');

    for (let i = 0; i < list.length; i++) {
        if (list[i].href.includes('newspeciality')) {
            let index = list[i].href.indexOf('newspeciality');
            let link = list[i].href.substring(0, index);
            let newlink = 'newspeciality=' + value;
            link = link.concat(newlink);
            list[i].href = link;
        }
    }
}