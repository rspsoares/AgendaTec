function PageSetup() {
  

}

function checkfile(sender) {
    var validExts = new Array(".xlsx", ".xls");
    var fileExt = sender.value;
    fileExt = fileExt.substring(fileExt.lastIndexOf('.'));
    if (validExts.indexOf(fileExt) < 0) {
        alert("Tipo de arquivo inválido. Os formatos aceitos são: " + validExts.toString() + ".");
        $(sender).val("");
        return false;
    }
    else
        return true;
}