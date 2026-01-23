let quillEditor;

window.initQuill = function (initialContent) {
    if (quillEditor) {
        quillEditor = null;
    }

    quillEditor = new Quill('#editor', {
        theme: 'snow',
        modules: {
            toolbar: [
                [{ 'header': [1, 2, 3, false] }],
                ['bold', 'italic', 'underline', 'strike'],
                [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                [{ 'indent': '-1' }, { 'indent': '+1' }],
                ['link'],
                [{ 'align': [] }],
                ['clean']
            ]
        },
        placeholder: 'Write your thoughts here...'
    });

    if (initialContent) {
        quillEditor.root.innerHTML = initialContent;
    }
};

window.getQuillContent = function () {
    if (quillEditor) {
        return quillEditor.root.innerHTML;
    }
    return '';
};

window.clearQuill = function () {
    if (quillEditor) {
        quillEditor.setText('');
    }
};