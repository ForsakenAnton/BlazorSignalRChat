'use strict'

window.OnInitialized = () => {
    console.log("===================== OnInitialized =======================");
}

var isScrolling = false;

window.ScrollToBottom = (elementName) => { // chatContainer
    console.log(isScrolling + " start ===================");
    if (isScrolling === true) return;

    var element = document.getElementById(elementName);
    console.log(element.scrollTop + " scrollTop");
    element.scrollTop = element.scrollHeight - element.clientHeight;
    console.log(element.scrollTop + " scrollTop after operation");
    console.log(isScrolling + " end ===================");
} 


var timeout = false;

window.Scroll = (elementName) => { // textAreaContainer

    isScrolling = true;

    if (timeout !== null) {
        clearTimeout(timeout);
        console.log("END of SCROLL - конец скролла и возврат непрозрачность элементов ввода");
    } //else {

    var element = document.getElementById(elementName);
    element.style.opacity = 0.1;
    console.log("SCROLL - начало скролла где становятся прозрачными элементы ввода: text-area & button");
    //} 

    timeout = setTimeout(function () {
        element.style.opacity = 1;
        isScrolling = false;
    }, 100);
}




















  //!!! ДОПИЛИТЬ !!!

//var timeout = false;

//window.Scroll = (elementName) => { // textAreaContainer

//    if (timeout !== null) {
//        clearTimeout(timeout);
//        console.log("END of SCROLL - конец скролла и возврат непрозрачность элементов ввода");
//    } //else {

//    var element = document.getElementById(elementName);
//    element.style.opacity = 0.1;
//    console.log("SCROLL - начало скролла где становятся прозрачными элементы ввода: text-area & button");
//    //} 

//    timeout = setTimeout(function () {
//        element.style.opacity = 1;      
//    }, 500);
//}

//window.onclick = function (e) {
//    console.log("SCROLL");
//}

//var timeout = false;

//window.onscroll = function () {
//    if (timeout !== null) {
//        clearTimeout(timeout);
//    } //else {
//        console.log("SCROLL");
//        var element = document.getElementById("textAreaContainer");
//        element.style.backgroundColor = "red";
//    //}

//    timeout = setTimeout(function () {
//        element.style.backgroundColor = "blue";

//    }, 500);
//}

//window.onclick = function () {
//    console.log("SCROLL");
//}