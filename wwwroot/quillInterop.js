(function () {
    window.initQuill = function (content) {
        if (window._quillInstance) {
            return;
        }

        const editor = document.getElementById("quillEditor");
        if (!editor) {
            console.error("quillEditor div not found");
            return;
        }

        window._quillInstance = new Quill(editor, {
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
            window._quillInstance.root.innerHTML = content;
        }
    };

    window.getQuillContent = function () {
        return window._quillInstance
            ? window._quillInstance.root.innerHTML
            : "";
    };
})();
