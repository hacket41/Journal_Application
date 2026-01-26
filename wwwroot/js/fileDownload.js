
window.downloadFile = function (filename, content) {
    // Create a blob from the content
    const blob = new Blob([content], { type: 'text/html' });

    // Create a temporary URL for the blob
    const url = window.URL.createObjectURL(blob);

    // Create a temporary anchor element and trigger download
    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = filename;
    anchor.click();

    // Clean up
    window.URL.revokeObjectURL(url);
}

// Optional: Function to print the HTML (for PDF conversion)
window.printHtml = function (content) {
    const printWindow = window.open('', '_blank');
    printWindow.document.write(content);
    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
}