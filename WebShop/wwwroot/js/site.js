/*!
 * Color mode toggler for Bootstrap's docs (https://getbootstrap.com/)
 * Copyright 2011-2023 The Bootstrap Authors
 * Licensed under the Creative Commons Attribution 3.0 Unported License.
 */


'use strict'
window.addEventListener('load', function () {
    document.getElementsByTagName("html")[0].style.visibility = "visible";
});


const switchLightTheme = () => {
    const rootElem = document.documentElement
    let dataTheme = rootElem.getAttribute('data-theme'),
        newTheme

        newTheme = 'light'

    rootElem.setAttribute('data-theme', newTheme)
    localStorage.setItem('theme', newTheme)
}

const switchDarkTheme = () => {
    const rootElem = document.documentElement
    let dataTheme = rootElem.getAttribute('data-theme'),
        newTheme

    newTheme = 'dark'

    rootElem.setAttribute('data-theme', newTheme)

    localStorage.setItem('theme', newTheme)
}

function showModalWin() {

    var darkLayer = document.createElement('div'); // слой затемнения
    darkLayer.id = 'shadow'; // id чтобы подхватить стиль
    document.body.appendChild(darkLayer); // включаем затемнение

    var modalWin = document.getElementById('popupWin'); // находим наше "окно"
    modalWin.style.display = 'block'; // "включаем" его

    darkLayer.onclick = function () {  // при клике на слой затемнения все исчезнет
        darkLayer.parentNode.removeChild(darkLayer); // удаляем затемнение
        modalWin.style.display = 'none'; // делаем окно невидимым
        return false;
    };
}

document.querySelector('#lightSwitcher').addEventListener('click', switchLightTheme)
document.querySelector('#darkSwitcher').addEventListener('click', switchDarkTheme)

var chat = document.getElementById('chat')
chat.scrollTop = chat.scrollHeight - chat.clientHeight


