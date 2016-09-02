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