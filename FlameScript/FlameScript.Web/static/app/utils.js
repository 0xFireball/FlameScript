﻿/*jshint esversion: 6 */

//Thank you http://stackoverflow.com/questions/18405736/is-there-a-c-sharp-string-format-equivalent-in-javascript
String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] != 'undefined' ? args[number] : match;
    });
};

// Thank you StackOverflow! :D
function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    let regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

// Display a message (bootstrap material) :D
function displayMessage(messageName, messageValue, alertType) {
    $("#intro").prepend(`<div class="alert alert-dismissible alert-{0}">
                        <button type="button" class="close" data-dismiss="alert">×</button>
                        <h4>{1}</h4>
                        <p>{2}</p>
                    </div>`.format(alertType, messageName, messageValue));
}

// Pull messages from JSON file and display them
function displayAnnouncements(url) {
    $.getJSON(url, function (data) {
        let items = [];
        $.each(data, function (alertType, alertsInCategory) {
            $.each(alertsInCategory, function (messageName, messageValue) {
                displayMessage(messageName, messageValue, alertType);
            });
        });
    });
}

function saveFile(data, filename) {
    let blob = new Blob([data], {
        type: "text/plain;charset=utf-8"
    });
    saveAs(blob, filename);
}

//Thanks http://stackoverflow.com/a/20151856
function base64toBlob(base64Data, contentType) {
    contentType = contentType || '';
    var sliceSize = 1024;
    var byteCharacters = atob(base64Data);
    var bytesLength = byteCharacters.length;
    var slicesCount = Math.ceil(bytesLength / sliceSize);
    var byteArrays = new Array(slicesCount);

    for (var sliceIndex = 0; sliceIndex < slicesCount; ++sliceIndex) {
        var begin = sliceIndex * sliceSize;
        var end = Math.min(begin + sliceSize, bytesLength);

        var bytes = new Array(end - begin);
        for (var offset = begin, i = 0 ; offset < end; ++i, ++offset) {
            bytes[i] = byteCharacters[offset].charCodeAt(0);
        }
        byteArrays[sliceIndex] = new Uint8Array(bytes);
    }
    return new Blob(byteArrays, { type: contentType });
}