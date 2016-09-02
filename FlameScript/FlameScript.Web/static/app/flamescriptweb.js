/*jshint esversion: 6 */

let codeMirrorOpts = {
    mode: "text/plain",
    matchBrackets: true,
    theme: "3024-day",
    lineNumbers: true,
    scrollbarStyle: "simple",
    extraKeys: {
        "Alt-F": "findPersistent"
    }
};

// Initialize CodeMirror editor
let cmEditor = CodeMirror.fromTextArea($("#cm-editor")[0], codeMirrorOpts);

let autosaveKey = 'autosavedSource';
$("body").ready(() => {
    let autosavedSource = localStorage.getItem(autosaveKey);
    if (autosavedSource) {
        cmEditor.setValue(autosavedSource);
    }
});

cmEditor.on("change", function (cm, change) {
    let autosavedSource = cmEditor.getValue();
    //console.log(change);
    localStorage.setItem(autosaveKey, autosavedSource);
});

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

$("#navbar-compile").click(() => {
    let editorSource = cmEditor.getValue();
    let postData = {
        code: editorSource,
        compileMode: "compiler",
    };
    $.post("/processCode", postData, (data, status, xhr) => {
        /*
        //Data is the output from processCode
        let ast = JSON.parse(data);
        console.log(ast);
        */
        let compiledCodeBlob = base64toBlob(data, "application/octet-stream");
        console.log(compiledCodeBlob);
        saveAs(compiledCodeBlob, "compiledprogram.penguin");
    });
});