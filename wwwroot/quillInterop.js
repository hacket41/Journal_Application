(function () {
    let quill = null;

    window.initQuill = function (content) {
        const editor = document.getElementById("quillEditor");

        if (!editor) {
            console.warn("quillEditor not found");
            return;
        }

        // 🔥 If quill exists but DOM was destroyed, reset it
        if (quill && !editor.firstChild) {
            quill = null;
        }

        if (quill) return;

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

    // ✅ Called when leaving the page
    window.destroyQuill = function () {
        quill = null;
    };
})();
