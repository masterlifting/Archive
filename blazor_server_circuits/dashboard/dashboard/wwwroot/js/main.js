window.InternalFunctions = class InternalFunctions {

    static OpenPdfInNewTabWithArray(array) {
        var blob = new Blob([array], { type: 'application/pdf' });
        var url = URL.createObjectURL(blob);
        window.open(url, '_blank');
    }

    static async OpenPdfInNewTabWithStream(contentStreamReference) {
        const buffer = await contentStreamReference.arrayBuffer();
        const blob = new Blob([buffer], { type: 'application/pdf' });
        const url = URL.createObjectURL(blob);
        window.open(url, '_blank');
    }
};
