$(document).ready(function () {    
    var _autoUpload = true;
    var _dropFilesHere = "Arraste o arquivo para qualquer área dentro do retângulo";
    var _select = "Adicionar";
    var _saveUrl = "Carga/Upload"
    var _removeUrl = "";


    var filesObj = $("#files").kendoUpload({
        async: {
            autoUpload: _autoUpload,
            saveUrl: _saveUrl,
            removeUrl: _removeUrl
        },
        error: onError,
        multiple: false,
        localization: {
            dropFilesHere: _dropFilesHere,
            select: _select
        },
        
        upload: onUpload,
        complete: function () {
            this.enable();
            $("div.k-dropzone").css("display", "");
        }
    }).data("kendoUpload");
});

function onError(e) {
    if (e.operation == "upload") {
        ShowModalAlert("Erro ao realizar upload!");
    }
}

function onUpload(e) {
    var files = e.files;

    if (files == undefined) {
        e.preventDefault();
        return;
    }        

    $.each(files, function () {
        if (this.extension.toLowerCase() != ".txt") {
            ShowModalAlert("Tipo de arquivo inválido.");            
            e.preventDefault();
        }
    });
}

