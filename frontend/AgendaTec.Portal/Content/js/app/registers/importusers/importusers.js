function PageSetup() {
    var manualUploader = new qq.FineUploader({
        element: document.getElementById('uploader'),
        autoUpload: true,
        multiple: false,
        debug: true,
        template: 'qq-template-manual-trigger',
        request: {
            endpoint: '/ImportUsers/UploadFile',
            method: 'POST'
        },
        thumbnails: {
            placeholders: {
                waitingPath: '/Vendor/fineUploader/placeholders/waiting-generic.png',
                notAvailablePath: '/Vendor/fineUploader/placeholders/not_available-generic.png'
            }
        },
        callbacks: {            
            onComplete: function (id, name, errorReason, xhrOrXdr) {
                $("#lbError").val(qq.format("Error on file number {} - {}. Reason: {}", id, name, errorReason));  
            }
        }
        //validation: {
        //    allowedExtensions: ['xls', 'xlsx']
        //}        
    });

    qq(document.getElementById("trigger-upload")).attach("click", function () {
        manualUploader.uploadStoredFiles();
    });
}