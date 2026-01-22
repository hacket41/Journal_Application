(function () {
    let quill = null;

    window.initQuill = function (content) {
        if (quill) return;

        const editor = document.getElementById("quillEditor");
        if (!editor) {
            console.error("quillEditor not found");
            return;
        }

        quill = new Quill(editor, {
            theme: "snow",
            placeholder: "Write your thoughts...",
            modules: {
                toolbar: [
                    ["bold", "italic", "underline", "strike"],
                    [{ header: [1, 2, 3, false] }],
                    [{ list: "ordered" }, { list: "bullet" }],
                    ["blockquote", "code-block"],
                    ["link"],
                    ["clean"]
                ]
            }
        });

        if (content) {
            quill.root.innerHTML = content;
        }
    };

    window.getQuillContent = function () {
        return quill ? quill.root.innerHTML : "";
    };
})();
