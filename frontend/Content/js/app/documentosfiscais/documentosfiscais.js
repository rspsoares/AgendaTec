/*VARIÁVEIS CONFIRMAÇÃO REMOVER*/
var janelaRemocaoItem = $("#confirmacaoRemover").kendoWindow({
    title: "Remover Item",
    visible: false
}).data("kendoWindow");

var functions = {
    1: CarregarGridDocumentosFiscais,
    2: CarregarGridDocumentosFiscaisCompleto
}

var tipoGrid = 2;
var periodoEditavel = false;
var idControle = 0;

var totalGeralBaseCalculoCOFINS = 0;
var totalGeralValorCOFINS = 0;
var totalGeralBaseCalculoPIS = 0;
var totalGeralValorPIS = 0;
var totalGeralValorITEM = 0;
var totalGeralQuantidade = 0;

var lstColunasCompletoPadrao = [];
var lstColunasPadrao = [];

var colCommand;
var colEditorMovimento;
var colEditorCategoriaNF;
var colEditorCFOP;

/*CONSTRUTOR SCRIPT*/
function CarregaComponentes() {
    localStorage.clear();
    CarregarMatrizesFiltro();
    CarregarCategoriasDocumentoFiscalFiltros();
    CarregarCFOPFiltros();
    CarregarCFOPRefFiltros();

    CarregarCodigoParticipanteFiltros();
    CarregarMaterialFiltros();
    CarregarGruposMercadoriasFiltros();
    CarregarCSTFiltros();

    $('#dtEmiDe').kendoDatePicker({
        format: "dd/MM/yyyy",
        start: "year"
    });
    $('#dtEmiAte').kendoDatePicker({
        format: "dd/MM/yyyy",
        start: "year"
    });
    $('#dtLancDe').kendoDatePicker({
        format: "dd/MM/yyyy",
        start: "year"
    });
    $('#dtLancAte').kendoDatePicker({
        format: "dd/MM/yyyy",
        start: "year"
    });

    $('#ddlPeriodo').kendoDropDownList();

    $('#txtCFOPReferencia').kendoMultiSelect();

    $('#ddlMovimento').kendoDropDownList({
        dataTextField: "texto",
        dataValueField: "valor",
        dataSource: BuscarMovimento(),
        optionLabel: "Entrada ou Saída"
    });

    $('#ddlRegra').kendoDropDownList({
        dataTextField: "texto",
        dataValueField: "valor",
        dataSource: BuscarParticipacaoRegras(),
        optionLabel: "Indiferente"
    });

    $('#btnFiltrosPersonalizados').click(function () {
        ModalFiltros();
    });

    $('#tipoVisualizacao').bootstrapSwitch();

    $('#tipoVisualizacao').on('switchChange.bootstrapSwitch', function(event, state) {
        //console.log(state); // true | false
        if(state)
            tipoGrid = 2;
        else
            tipoGrid = 1;
        
        if ($("#grid").data("kendoGrid") != undefined)
        {
            $("#grid").data("kendoGrid").destroy();
            $("#grid").html("");
        }

        if($('#ddlPeriodo').val())
            functions[tipoGrid](idControle, periodoEditavel);
    });

    kendo.ui.Tooltip.fn._show = function (show) {
        return function (target) {
            var e = {
                sender: this,
                target: target,
                preventDefault: function () {
                    this.isDefaultPrevented = true;
                }
            };

            if (typeof this.options.beforeShow === "function") {
                this.options.beforeShow.call(this, e);
            }
            if (!e.isDefaultPrevented) {              
                show.call(this, target);
            }
        };
    }(kendo.ui.Tooltip.fn._show);

    CarregarColunasCompletoPadrao();
    CarregarColunasPadrao();
    $("#grid").hide();

    $('#selectCombinacao').bootstrapSwitch();

    $('#tbValor').on('blur', function () {
        var regex = /^[0-9]+([\,\.][0-9]+)?$/g;
        if (regex.test($(this).val().replace(".", "")))
            $(this).val(parseFloat($(this).val().replace(".", "").replace(",", ".")).FormatarMoeda(2, '', '.', ','));
        else
            $(this).val("0,00");
    });

    $('#tbValor').on('keypress', function (e) {
        if (e.which != 8 && e.which != 0 && e.which != 44 && (e.which < 48 || e.which > 57)) return false;
    });

    $('#tbValor2').on('blur', function () {
        var regex = /^[0-9]+([\,\.][0-9]+)?$/g;
        if (regex.test($(this).val().replace(".", "")))
            $(this).val(parseFloat($(this).val().replace(".", "").replace(",", ".")).FormatarMoeda(2, '', '.', ','));
        else
            $(this).val("0,00");
    });

    $('#tbValor2').on('keypress', function (e) {
        if (e.which != 8 && e.which != 0 && e.which != 44 && (e.which < 48 || e.which > 57)) return false;
    });
};

/*GRID PADRÃO COMPLETA */
function CarregarColunasCompletoPadrao() {
    $("#grid").kendoGrid({
        dataSource: {
            aggregate: [
                { field: "Quantidade", aggregate: "sum" },
                { field: "TotalItem", aggregate: "sum" },
                { field: "BaseCalculoCOFINS", aggregate: "sum" },
                { field: "ValorCOFINS", aggregate: "sum" },
                { field: "BaseCalculoPIS", aggregate: "sum" },
                { field: "ValorPIS", aggregate: "sum" }
            ]
        },        
        columns: [
        { command: [ "edit", "destroy" ], title: "&nbsp;", width: "180px", locked: true },
        {
            field: '',
            title: "Logs",
            locked: true,
            template: '# if (Modificado == 1 || ModificadoItem == 1) { #<div style="text-align: center;cursor:pointer;" onclick="CarregarHistorico(#=IdDocumentoFiscal#,#=IdDocumentoFiscalItem#);"><i class="glyphicon glyphicon-time" ></i></div># } #',
            width: "40px"
        },
        {
            field: 'ItemParteRegra',
            title: "Regras",
            locked: true,
            template: '# if (ItemParteRegra == 1) { #<div style="text-align: center;cursor:pointer;" ><i class="glyphicon glyphicon-th-large" ></i></div># } #',
            width: "80px"
        },
        { field: "DocNum", title: "DOCNUM", width: "100px", locked: true, footerTemplate: '<div><span style=\'float:right;\'>SUBTOTAL:</span></div><br><div><span style=\'float:right;\'>TOTAL GERAL:</span></div>' },
        { field: "Movimento", sortable: false, locked: false, title: "Movimento", width: "90px", template: "#= MostrarMovimento(Movimento) #", editor: MostrarOpcoesMovimento },
        { field: "NFe", sortable: false, title: "NFe", width: "80px" },
        { field: "Item", sortable: false, title: "Nº Item", width: "80px" },
        { field: "Numero", title: "Número", width: "100px" },
        { field: "Serie", sortable: false, title: "Série", width: "80px" },
        { field: "IdCategoriaNF", sortable: false, title: "Categoria", width: "200px", template: "#= RecuperarCategoriaExibicaoGrid(IdCategoriaNF) #", editor: RecuperarCategoriasEdicao },
        { field: "DEmi", title: "Emissão", template: "#= kendo.toString(kendo.parseDate(DEmi, 'yyyy-MM-dd'), 'dd/MM/yyyy') #", width: "100px" },
        { field: "DLcto", title: "Lançamento", template: "#= kendo.toString(kendo.parseDate(DLcto, 'yyyy-MM-dd'), 'dd/MM/yyyy') #", width: "120px" },
        { field: "RazaoSocial", sortable: false, title: "Razão Social", width: "300px" },
        { field: "Cnpj", sortable: false, title: "Cnpj", width: "120px" },
        { field: "CodigoParticipante", sortable: false, title: "Código Participante", width: "150px" },
        { field: "Estado", sortable: false, title: "Região", width: "80px" },
        { field: "Manualmente", sortable: false, title: "Manualmente", width: "100px" },
        { field: "StatusNota", sortable: false, title: "Status", width: "80px" },
        { field: "CFOP", title: "CFOP", width: "200px", editor: MostrarOpcoesCfops },
        { field: "CFOPRef", sortable: false, title: "CFOP Ref.", width: "200px", editor: MostrarOpcoesCfopsRef },
        { field: "Material", sortable: false, title: "Material", width: "150px" },
        { field: "DescricaoMaterial", sortable: false, title: "Texto Breve de Material", width: "300px" },
        { field: "GrupoMercadoria", sortable: false, title: "Grupo de Mercadoria", width: "180px" },
        { field: "Centro", sortable: false, title: "Centro", width: "80px" },
        { field: "DocRef", sortable: false, title: "DOC Referência", width: "120px" },
        { field: "LeiCofins", sortable: false, title: "Lei COFINS", width: "120px" },
        { field: "BaseCalculoCOFINS", sortable: false, title: "Base COFINS", width: "120px", template: '#=BaseCalculoCOFINS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" }, footerTemplate: "<div style='float: right'>#= kendo.toString(sum, 'c2') #</div><br><div style='float: right'>#= totalGeralBaseCalculoCOFINS #</div>" },
        { field: "AliqCOFINS", sortable: false, title: "Alíq COFINS", width: "120px", template: '#=kendo.format("{0:p}", AliqCOFINS / 100)#', attributes: { style: "text-align:right;" } },
        { field: "ValorCOFINS", sortable: false, title: "Valor COFINS", width: "120px", template: '#=ValorCOFINS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" }, footerTemplate: "<div style='float: right'>#= kendo.toString(sum, 'c2') #</div><br><div style='float: right'>#= totalGeralValorCOFINS #</div>" },
        { field: "LeiTribPIS", sortable: false, title: "Lei PIS", width: "120px" },
        { field: "BaseCalculoPIS", sortable: false, title: "Base PIS", width: "120px", template: '#=BaseCalculoPIS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" }, footerTemplate: "<div style='float: right'>#= kendo.toString(sum, 'c2') #</div><br><div style='float: right'>#= totalGeralBaseCalculoPIS #</div>" },
        { field: "AliqPIS", sortable: false, title: "Alíq PIS", width: "120px", template: '#=kendo.format("{0:p}", AliqPIS / 100)#', attributes: { style: "text-align:right;" } },
        { field: "ValorPIS", sortable: false, title: "Valor PIS", width: "120px", template: '#=ValorPIS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" }, footerTemplate: "<div style='float: right'>#= kendo.toString(sum, 'c2') #</div><br><div style='float: right'>#= totalGeralValorPIS #</div>" },
        { field: "PedCompra", sortable: false, title: "Pedido de Compra", width: "150px" },
        { field: "Quantidade", sortable: false, title: "Quantidade", width: "120px", template: '#=Quantidade.FormatarMoeda(2, undefined, ".", ",")#', attributes: { style: "text-align:right;" }, footerTemplate: "<div style='float: right'>#= kendo.toString(sum, 'n2') #</div><br><div style='float: right'>#= totalGeralQuantidade#</div>" },
        { field: "TotalItem", sortable: false, title: "Total do Item", width: "120px", template: '#=TotalItem.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" }, footerTemplate: "<div style='float: right'>#= kendo.toString(sum, 'c2') #</div><br><div style='float: right'>#= totalGeralValorITEM #</div>" },
        { field: "CodigoIVA", sortable: false, title: "Código IVA", width: "120px" },
        { field: "NFeRefDocNum", title: "NFe Ref Nr. Documento", width: "180px" },
        { field: "NFeRefNfeNum", title: "NFe Ref Nr. NFE/NF", width: "160px" },
        { field: "NFeRefSerie", title: "NFe Ref Série", width: "140px" },
        { field: "NFeRefNCM", title: "NFe Ref NCM", width: "140px" },
        { field: "StatusNFe", sortable: false, title: "Status NFe", width: "80px" },
        { field: "Protocolo", sortable: false, title: "Protocolo", width: "80px" },
        { field: "NumeroAleatorio", sortable: false, title: "Nº Aleatório", width: "100px" },
        { field: "DigVal", sortable: false, title: "Dig Val", width: "80px" },
        { field: "StComunSis", sortable: false, title: "Status Com. Sistema", width: "160px" },
        { field: "NumLog", sortable: false, title: "Nº Log", width: "120px" },
        { field: "AuthDate", sortable: false, title: "Autenticação", template: "#= kendo.toString(kendo.parseDate(AuthDate, 'yyyy-MM-dd'), 'dd/MM/yyyy') #", width: "100px" },
        { field: "DataCriacao", sortable: false, title: "Criação", template: "#= kendo.toString(kendo.parseDate(DataCriacao, 'yyyy-MM-dd'), 'dd/MM/yyyy') #", width: "80px" },
        { field: "Usuario", sortable: false, title: "Usuario", width: "80px" }
        ]
    });

    var grid = $("#grid").data("kendoGrid");
    lstColunasCompletoPadrao = grid.columns;
    colCommand = grid.columns[0];

    lstColunasCompletoPadrao.forEach(function (colunaGrid) {
        switch (colunaGrid.field) {
            case "Movimento":
                colEditorMovimento = colunaGrid.editor;
                break;
            case "IdCategoriaNF":
                colEditorCategoriaNF = colunaGrid.editor;
                break;
            case "CFOP":
            case "CFOPRef":
                colEditorCFOP = colunaGrid.editor;
                break;
        }
    });  
}

/*GRID PADRÃO */
function CarregarColunasPadrao() {
    $("#grid").kendoGrid({       
        columns: [
        {
            command: [
            "edit","destroy"
            ], title: "&nbsp;", width: "180px", locked: true
        },                      
        {
            title: "Itens",
            template: "<a onclick='javascript:{VerItens(this);}' class='k-button'>"
                    + "<span title='Itens' class='glyphicon glyphicon-list-alt'></span></a>",
            width: "60px",
            attributes: { style: "text-align:center;" },
            filterable: false,
            locked: true
        },
        { field: "DocNum", title: "DOCNUM", width: "100px", locked: true },
        { field: "Movimento", title: "Movimento", width: "120px", template: "#= MostrarMovimento(Movimento) #", editor: MostrarOpcoesMovimento },            
        { field: "Numero", title: "Número", width: "100px" },
        { field: "Serie", title: "Série", width: "80px" },
        { field: "IdCategoriaNF", title: "Categoria", width: "200px", template: "#= RecuperarCategoriaExibicaoGrid(IdCategoriaNF) #", editor: RecuperarCategoriasEdicao },
        { field: "DEmi", title: "Emissão", template: "#= kendo.toString(kendo.parseDate(DEmi, 'yyyy-MM-dd'), 'dd/MM/yyyy') #", width: "100px" },
        { field: "DLcto", title: "Lançamento", template: "#= kendo.toString(kendo.parseDate(DLcto, 'yyyy-MM-dd'), 'dd/MM/yyyy') #", width: "120px" },
        { field: "Cnpj", title: "Cnpj", width: "120px" },
        { field: "CodigoParticipante", title: "Código Participante", width: "150px" },
        { field: "RazaoSocial", title: "Razão Social", width: "300px" },
        { field: "Estado", title: "Estado", width: "80px" },
        { field: "DocRef", title: "DOC Referência", width: "140px" },
        { field: "NFeRefDocNum", title: "NFe Ref Nr. Documento", width: "180px" },
        { field: "NFeRefNfeNum", title: "NFe Ref Nr. NFE/NF", width: "160px" },
        { field: "NFeRefSerie", title: "NFe Ref Série", width: "140px" },
        { field: "NFeRefNCM", title: "NFe Ref NCM", width: "140px" },
        { field: "Usuario", title: "Usuario", width: "120px" }
        ]
    });             

    var grid = $("#grid").data("kendoGrid");
    lstColunasPadrao = grid.columns;
}

/*FILTROS - MATRIZES*/
function CarregarMatrizesFiltro() {
    $('#txtCodMatriz').kendoDropDownList({
        dataTextField: "CodigoMatriz",
        dataValueField: "Id",
        dataSource: LoadDsMatrizes(),
        optionLabel: "Escolha ...",
        select: function (e) {
            var dataItem = this.dataItem(e.item);
            $('#txtRazaoSocial').val(dataItem.RazaoSocial);
            idMatriz = dataItem.Id;

            $("#grid").html("");           

            $("#gridHistoricoPeriodo").html("");

            CarregarPeriodos(idMatriz);
        }
    });
}

/*FILTROS - PERIODO*/
function CarregarPeriodos(idMatriz) {
    var dsPeriodo = undefined;
    $.ajax({
        url: "/DocumentosFiscais/RecuperarPeriodos?idmatriz=" + idMatriz,
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            dsPeriodo = result.Data;
        }
    });

    if (dsPeriodo == undefined) {
        ShowModalAlerta("Nenhum período configurado.");
        return;
    }

    $('#ddlPeriodo').kendoDropDownList({
        dataTextField: "Periodo",
        dataValueField: "Id",
        dataSource: dsPeriodo,
        optionLabel: "Escolha o período ...",
        select: function (e) {
            var dataItem = this.dataItem(e.item);
            $('#ddlPeriodo').val(dataItem.Periodo);

            if (!dataItem.Id)
                idControle = 0
            else
                idControle = dataItem.Id;

            VerificarStatusPeriodo(idControle);

            if (periodoEditavel == true)
                $("#lbPeriodoFechado").text('');
            else
                $("#lbPeriodoFechado").text('PERÍODO FECHADO');

            $('#idValidacao').val(0);

            functions[tipoGrid](idControle, periodoEditavel);
            CarregarGridValidacoes(idControle);
            CarregarHistoricoPeriodo(idControle);
           
            $('#kmsCodigoCliente').data('kendoMultiSelect').dataSource.read();
            $('#kmsGrpMercadoria').data('kendoMultiSelect').dataSource.read();
            $('#kmsMaterial').data('kendoMultiSelect').dataSource.read();
            $('#kmsCST').data('kendoMultiSelect').dataSource.read();
        }
    });    
}

/*FILTROS - CATEGORIA*/
function CarregarCategoriasDocumentoFiscalFiltros() {
    $('#txtCatNota').kendoMultiSelect({
        placeholder: "Selecione Categorias..",
        itemTemplate: kendo.template('#= Descricao #'),
        dataTextField: "Codigo",
        dataValueField: "Id",
        dataSource: BuscarCategorias()
    });
}

/*FILTROS - CFOP*/
function CarregarCFOPFiltros() {
    $('#kmsCFOP').kendoMultiSelect({
        placeholder: "Selecione CFOPs..",
        itemTemplate: kendo.template('<span class="cfop-codigo">#= Codigo # -</span> #= Descricao #'),
        dataTextField: "Codigo",
        dataValueField: "Codigo",
        dataSource: BuscarCFOPs()
    });
}

/*FILTROS - CFOP REFERÊNCIA*/
function CarregarCFOPRefFiltros() {
    $('#kmsCFOPRef').kendoMultiSelect({
        placeholder: "Selecione CFOPs..",
        itemTemplate: kendo.template('<span class="cfop-codigo">#= Codigo # -</span> #= Descricao #'),
        dataTextField: "Codigo",
        dataValueField: "Codigo",
        dataSource: BuscarCFOPRefs()
    });
}

/*FILTROS - SELECIONAR VALIDAÇÃO*/
function selecionandoValidacao(arg) {
    var entityGrid = $("#gridValidacoes").data("kendoGrid");
    var dataItem = entityGrid.dataItem(entityGrid.select());
    var idValidacao = dataItem == null ? 0 : dataItem.Id;
    $('#idValidacao').val(idValidacao);    
}

/*FILTROS - CÓDIGO PARTICIPANTE*/
function CarregarCodigoParticipanteFiltros() {     
    $('#kmsCodigoCliente').kendoMultiSelect({
        placeholder: "Selecione Participante(s)...",
        dataTextField: 'Codigo',
        dataValueField: 'Codigo',
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: "/DocumentosFiscais/BuscarCodigosParticipantes",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idControle: idControle,
                        }
                    }
                }
            }
        }       
    });
}

/*FILTROS - GRUPO MERCADORIA*/
function CarregarGruposMercadoriasFiltros() {
    $('#kmsGrpMercadoria').kendoMultiSelect({
        placeholder: "Selecione Grupos de Mercadorias...",
        dataTextField: 'Codigo',
        dataValueField: 'Codigo',
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: "/DocumentosFiscais/BuscarGruposMercadorias",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idControle: idControle,
                        }
                    }
                }
            }
        }
    });    
}

/*FILTROS - MATERIAL*/
function CarregarMaterialFiltros() {
    $('#kmsMaterial').kendoMultiSelect({
        placeholder: "Selecione os Materiais...",
        dataTextField: 'Codigo',
        dataValueField: 'Codigo',
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: "/DocumentosFiscais/BuscarCodigosMateriais",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idControle: idControle,
                        }
                    }
                }
            }
        }        
    });
}

/*FILTROS - CST*/
function CarregarCSTFiltros() {
    $('#kmsCST').kendoMultiSelect({
        placeholder: "Selecione os CST's...",
        dataTextField: 'Codigo',
        dataValueField: 'Codigo',
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: "/DocumentosFiscais/BuscarCST",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type === "read") {
                        return {
                            idControle: idControle,
                        }
                    }
                }
            }
        }
    });
}


/*GRID DE DOCUMENTOS FISCAIS COMPLETOS (TELA PRINCIPAL)*/
//PesquisarDocumentosCompletos
function CarregarGridDocumentosFiscaisCompleto(idControle, periodoEditavel) {       
    var colunas = RecuperarSequenciaColunas(periodoEditavel);
    $("#grid").show();
    $("#grid").html("");
    $("#grid").kendoGrid({
        dataSource: {
            schema: {
                data: function (result) {
                    totalGeralBaseCalculoCOFINS = 'R$ ' + result.TotalPeriodo.totalGeralBaseCalculoCOFINS.FormatarMoeda(2, "", ".", ",");
                    totalGeralValorCOFINS = 'R$ ' + result.TotalPeriodo.totalGeralValorCOFINS.FormatarMoeda(2, "", ".", ",");
                    totalGeralBaseCalculoPIS = 'R$ ' + result.TotalPeriodo.totalGeralBaseCalculoPIS.FormatarMoeda(2, "", ".", ",");
                    totalGeralValorPIS = 'R$ ' + result.TotalPeriodo.totalGeralValorPIS.FormatarMoeda(2, "", ".", ",");
                    totalGeralValorITEM = 'R$ ' + result.TotalPeriodo.totalGeralValorITEM.FormatarMoeda(2, "", ".", ",");
                    totalGeralQuantidade = result.TotalPeriodo.totalGeralQuantidade.FormatarMoeda(2, "", ".", ",");
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                },
                model: {
                    id: "IdDocumentoFiscalItem",
                    fields: {
                        IdTNAControle: { validation: { required: false } },
                        Verificado: { validation: { required: false } },
                        Validado: { validation: { required: false } },
                        isValido: { validation: { required: false } },
                        ItemParteRegra: { validation: { required: false }, editable: false },
                        NFe: { validation: { required: false }, editable: false },
                        DocNum: { validation: { required: true }, editable: false },
                        Movimento: { validation: { required: true } },
                        Item: { format: "{0:n0}", validation: { required: true } },
                        Numero: { format: "{0:n0}", validation: { required: true } },
                        Serie: { validation: { required: false }, nullable: true },
                        IdCategoriaNF: { type: "number", validation: { required: true } },
                        DEmi: { type: "date", validation: { required: true } },
                        DLcto: { type: "date", validation: { required: true } },
                        RazaoSocial: { validation: { required: true } },
                        Cnpj: { validation: { required: true } },
                        CodigoParticipante: { validation: { required: true } },
                        Estado: { validation: { required: true } },
                        Manualmente: { validation: { required: true }, editable: false },
                        StatusNota: { validation: { required: true }, editable: false },
                        CFOP: { type: "text", validation: { required: false } },
                        CFOPRef: { type: "text", validation: { required: false } },
                        Material: { validation: { required: false } },
                        DescricaoMaterial: { validation: { min: 0, required: true } },
                        GrupoMercadoria: { validation: { required: false } },
                        Centro: { validation: { required: false } },
                        DocRef: { validation: { required: false } },
                        LeiCofins: { validation: { required: true } },
                        BaseCalculoCOFINS: { type: "number", validation: { required: false } },
                        AliqCOFINS: { type: "number", validation: { required: false } },
                        ValorCOFINS: { type: "number", validation: { required: false } },
                        LeiTribPIS: { validation: { required: true } },
                        BaseCalculoPIS: { type: "number", validation: { required: false } },
                        AliqPIS: { type: "number", validation: { required: false } },
                        ValorPIS: { type: "number", validation: { required: false } },
                        PedCompra: { validation: { required: false } },
                        Quantidade: { type: "number", validation: { required: false } },
                        TotalItem: { type: "number", validation: { required: false } },
                        NFeRefDocNum: { format: "{0:n0}", validation: { required: false } },
                        NFeRefNfeNum: { format: "{0:n0}", validation: { required: false } },
                        NFeRefSerie: { validation: { required: false }},
                        NFeRefNCM: { validation: { required: false } },
                        CodigoIVA: { validation: { required: false }, editable: false },
                        StatusNFe: { validation: { required: false }, editable: false },
                        Protocolo: { validation: { required: false }, editable: false },
                        NumeroAleatorio: { validation: { required: false }, editable: false },
                        DigVal: { validation: { required: false }, editable: false },
                        StComunSis: { validation: { required: false }, editable: false },
                        NumLog: { validation: { required: false }, editable: false },
                        AuthDate: { type: "date", validation: { required: false } },
                        DataCriacao: { type: "date", validation: { required: false } },
                        Usuario: { validation: { required: false }, editable: false },
                        DataEntrada: { type: "date", validation: { required: true } },
                        DataAtualizacao: { type: "date", validation: { required: true } }
                    }
                }
            },
            transport: {
                read: {
                    url: "/DocumentosFiscais/PesquisarDocumentosCompletos",
                    dataType: "json",
                    type: "GET",
                    async: true,//não aguardamos a busca
                    cache: false
                },
                update: {
                    url: "/DocumentosFiscais/AtualizarDocumentoCompleto",
                    dataType: "json",
                    type: "POST",
                    async: false,//aguardamos a atualização
                    contentType: "application/json",//necessário para stringfy
                    complete: function (e) {
                        $("#grid").data("kendoGrid").dataSource.read();
                        $("#gridHistoricoPeriodo").data("kendoGrid").dataSource.read();

                    }
                },
                destroy: {
                    url: "/DocumentosFiscais/RemoverDocumentoCompleto",
                    dataType: "json",
                    type: "POST",
                    async: false,
                    complete: function (e) {
                        $("#grid").data("kendoGrid").dataSource.read();
                        $("#gridHistoricoPeriodo").data("kendoGrid").dataSource.read();
                    }
                },
                parameterMap: function (data, type) {

                    //customização de requisição garante que os parâmetros dos filtros estão claros
                    if (type == "read") {
                        var pattern = /(\d{2})\/(\d{2})\/(\d{4})/;
                        var ordernar = '';
                        var ordernarDir = '';

                        if (typeof data.sort !== "undefined" && data.sort !== null) {
                            ordernar = data.sort[0]['field'];
                            ordernarDir = data.sort[0]['dir'];
                        }

                        return {
                            idControle: idControle,
                            pagina: data.page,
                            numeros: $('#txtNumero').val(),
                            cfops: $('#kmsCFOP').data('kendoMultiSelect').value().toString(),
                            cfopsRef: $('#kmsCFOPRef').data('kendoMultiSelect').value().toString(),
                            csts: $('#kmsCST').data('kendoMultiSelect').value().toString(),
                            categoria: $('#txtCatNota').data('kendoMultiSelect').value().toString(),
                            codigoclientefornecedor: $('#kmsCodigoCliente').data('kendoMultiSelect').value().toString(),
                            docnum: $('#txtDocNum').val(),
                            material: $('#kmsMaterial').data('kendoMultiSelect').value().toString(),
                            dtEmiIni: ($('#dtEmiDe').val()).replace(pattern, '$3-$2-$1'),
                            dtEmiFim: ($('#dtEmiAte').val()).replace(pattern, '$3-$2-$1'),
                            dtLancIni: ($('#dtLancDe').val()).replace(pattern, '$3-$2-$1'),
                            dtLancFim: ($('#dtLancAte').val()).replace(pattern, '$3-$2-$1'),
                            movimento: $('#ddlMovimento').data('kendoDropDownList').value().toString(),
                            associadoRegra: $('#ddlRegra').data('kendoDropDownList').value().toString(),
                            grupomercadoria: $('#kmsGrpMercadoria').data('kendoMultiSelect').value().toString(),
                            totalItem: $('#txtTotalItem').val().replace(/\./g, ''), //.replace(",", "."),
                            idvalidacao: $('#idValidacao').val(),
                            tamPagina: data.pageSize,
                            pular: data.skip,
                            recuperar: data.take,
                            ordernar: ordernar,
                            ordernarDir: ordernarDir
                        }
                    }
                    //objeto para atualização precisa ser enviado via stringfy para funcionar
                    if (type == "update") {
                        if (data.CFOP != null && data.CFOP.Codigo != undefined) {
                            var cfopAux = data.CFOP.Codigo;
                            data.CFOP = cfopAux;
                        }

                        if (data.CFOPRef != null && data.CFOPRef.Codigo != undefined) {
                            var cfopRefAux = data.CFOPRef.Codigo;
                            data.CFOPRef = cfopRefAux;
                        }

                        return JSON.stringify({ entidade: data });
                    }

                    if (type == "destroy") {
                        return { idDocumento: data.IdDocumentoFiscal, idDocumentoFiscalItem: data.IdDocumentoFiscalItem }
                    }
                }
            },
            pageSize: 25,
            serverPaging: true,
            serverSorting: true,
            serverFiltering: false,
            serverGrouping: false,
            aggregate: [
                { field: "Quantidade", aggregate: "sum" },
                { field: "TotalItem", aggregate: "sum" },
                { field: "BaseCalculoCOFINS", aggregate: "sum" },
                { field: "ValorCOFINS", aggregate: "sum" },
                { field: "BaseCalculoPIS", aggregate: "sum" },
                { field: "ValorPIS", aggregate: "sum" }
            ]
        },
        scrollable: true,
        filterable: false,
        groupable: false,
        resizable: true,
        sortable: true,
        reorderable: true,
        columnReorder: onReorder,
        columnMenu: true,        
        columnShow: onPickColumn,
        columnHide: onPickColumn,
        height: '500px',
        pageable: {
            pageSizes: [10, 25, 50, 100, 500]
        },    
        columns: colunas,
        columnMenuInit: function (e) {
            var grid = this;
            e.container.find(kendo.roleSelector("menu")).data("kendoMenu").bind("select", function(e) {
                if ($(e.item).is(".k-lock, .k-unlock"))
                    GravarOrdemColunas(grid.columns);
            });
        },
        editable: "inline"
    });

    $("#grid").kendoTooltip({
        filter: "td:nth-child(3)",
        position: "left",
        beforeShow: function (e) {
            var dataItem = $("#grid").data("kendoGrid").dataItem(e.target.closest("tr"));
            if (dataItem.ItemParteRegra != 1) {          
                e.preventDefault();
            }
        },
        content: function (e) {
            var dataItem = $("#grid").data("kendoGrid").dataItem(e.target.closest("tr"));
            var content = InfoRegrasPorItem(dataItem.IdDocumentoFiscalItem);
            return content;
        }
    }).data("kendoTooltip");   
}

/*EVENTO DE ORDENAÇÃO DAS COLUNAS*/
function onReorder(e) {
    var that = this;    
    setTimeout(function () {            
        GravarOrdemColunas(that.columns);
    });   
}

/*EVENTO DE EXIBIR / OCULTAR COLUNAS*/
function onPickColumn(e) {    
    var that = this;
    setTimeout(function () {
        GravarOrdemColunas(that.columns);
    });
}

/*GRID DE DOCUMENTOS FISCAIS (TELA PRINCIPAL)*/
//PesquisarDocumentos
function CarregarGridDocumentosFiscais(idControle, periodoEditavel) {
    var colunas = RecuperarSequenciaColunas(periodoEditavel);
    $("#grid").show();
    $("#grid").html("");    
    $("#grid").kendoGrid({
        dataSource: {
            serverPaging: true,
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                },
                model: {
                    id: "Id",
                    fields: {
                        IdTNAControle: { validation: { required: false } },
                        Verificado: { validation: { required: false } },
                        Validado: { validation: { required: false } },
                        DocNum: { validation: { required: true }, editable: false },
                        Movimento: { validation: { required: true } },
                        Numero: { format: "{0:n0}", validation: { required: true } },
                        Serie: { validation: { required: false }, nullable: true },
                        IdCategoriaNF: { type: "number", validation: { required: true } },
                        DEmi: { type: "date", validation: { required: true } },
                        DLcto: { type: "date", validation: { required: true } },
                        Cnpj: { validation: { required: true } },
                        CodigoParticipante: { validation: { required: true } },                        
                        RazaoSocial: { validation: { required: true } },
                        Estado: { validation: { required: true } },
                        DocRef: { validation: { required: false } },
                        NFeRefDocNum: { format: "{0:n0}", validation: { required: false } },
                        NFeRefNfeNum: { format: "{0:n0}", validation: { required: false } },
                        NFeRefSerie: { validation: { required: false } },
                        NFeRefNCM: { validation: { required: false } },
                        StatusNFe: { validation: { required: false } },
                        Protocolo: { validation: { required: false }, editable: false },
                        NIF_Regional: { validation: { required: false } },
                        Usuario: { validation: { required: false }, editable: false },
                        DataCriacao: { type: "date", validation: { required: false } },
                        DataEntrada: { type: "date", validation: { required: true } },
                        DataAtualizacao: { type: "date", validation: { required: true } }
                    }
                }
            },
            pageSize: 25,
            transport: {
                read: {
                    url: "/DocumentosFiscais/PesquisarDocumentos",
                    dataType: "json",
                    type: "GET",
                    async: true,//não aguardamos a busca
                    cache: false
                },
                update: {
                    url: "/DocumentosFiscais/AtualizarDocumento",
                    dataType: "json",
                    type: "POST",
                    async: false,//aguardamos a atualização
                    contentType: "application/json",//necessário para stringfy
                    complete: function (e) {
                        $("#grid").data("kendoGrid").dataSource.read();
                        $("#gridHistoricoPeriodo").data("kendoGrid").dataSource.read();
                    }
                },
                destroy: {
                    url: "/DocumentosFiscais/RemoverDocumento",
                    dataType: "json",
                    type: "POST",
                    async: false,
                    complete: function (e) {
                        $("#grid").data("kendoGrid").dataSource.read();
                        $("#gridHistoricoPeriodo").data("kendoGrid").dataSource.read();
                    }
                },
                parameterMap: function (data, type) {
                    //customização de requisição garante que os parâmetros dos filtros estão claros
                    if (type == "read") {
                        var pattern = /(\d{2})\/(\d{2})\/(\d{4})/;
                        
                        return {
                            idControle: idControle,
                            pagina: data.page,
                            numeros: $('#txtNumero').val(),
                            cfops: $('#kmsCFOP').data('kendoMultiSelect').value().toString(),
                            cfopsRef: $('#kmsCFOPRef').data('kendoMultiSelect').value().toString(),
                            csts: $('#kmsCST').data('kendoMultiSelect').value().toString(),
                            categoria: $('#txtCatNota').data('kendoMultiSelect').value().toString(),
                            codigoclientefornecedor: $('#kmsCodigoCliente').data('kendoMultiSelect').value().toString(),
                            docnum: $('#txtDocNum').val(),
                            material: $('#kmsMaterial').data('kendoMultiSelect').value().toString(),
                            dtEmiIni: ($('#dtEmiDe').val()).replace(pattern, '$3-$2-$1'),
                            dtEmiFim: ($('#dtEmiAte').val()).replace(pattern, '$3-$2-$1'),
                            dtLancIni: ($('#dtLancDe').val()).replace(pattern, '$3-$2-$1'),
                            dtLancFim: ($('#dtLancAte').val()).replace(pattern, '$3-$2-$1'),
                            movimento: $('#ddlMovimento').data('kendoDropDownList').value().toString(),
                            grupomercadoria: $('#kmsGrpMercadoria').data('kendoMultiSelect').value().toString(),
                            totalItem: $('#txtTotalItem').val().replace(/\./g, ''), //.replace(",", "."),
                            idvalidacao: $('#idValidacao').val(),
                            tamPagina: data.pageSize,
                            pular: data.skip,
                            recuperar: data.take
                        }
                    }
                    //objeto para atualização precisa ser enviado via stringfy para funcionar
                    if (type == "update") {
                        return JSON.stringify({ entidade: data });
                    }

                    if (type == "destroy") {
                        return { idDocumento: data.Id }
                    }
                }
            }           
        },
        scrollable: true,
        height: '500px',
        sortable: true,
        resizable: true,
        reorderable: true,
        columnReorder: onReorder,
        columnMenu: true,
        columnShow: onPickColumn,
        columnHide: onPickColumn,
        pageable: {
            pageSizes: [10, 25, 50, 100, 500]
        },        
        columns: colunas,
        columnMenuInit: function (e) {
            var grid = this;
            e.container.find(kendo.roleSelector("menu")).data("kendoMenu").bind("select", function (e) {
                if ($(e.item).is(".k-lock, .k-unlock"))
                    GravarOrdemColunas(grid.columns);
            });
        },          
        editable: "inline"
    });
}

/*GRAVAR ORDEM COLUNAS*/
function GravarOrdemColunas(gridColunas) {
    var nomeGrid;
    var coluna = [];
    var lstColunas = [];
    
    if (tipoGrid == 1)
        nomeGrid = 'DocumentosFiscais';
    else
        nomeGrid = 'DocumentosFiscaisCompleto';

    gridColunas.forEach(function (colunaGrid) {
        if (colunaGrid.command == null) {
            coluna = {                
                Titulo: colunaGrid.title,
                Campo: colunaGrid.field,
                Visivel: !colunaGrid.hidden,
                Travada: colunaGrid.locked
            };

            lstColunas.push(coluna);
        }    
    });

    $.ajax({
        url: '/DocumentosFiscais/AtualizarSequenciaColunas',
        data: JSON.stringify({ nomeGrid: nomeGrid, lstColunas: lstColunas }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Sucesso == false) 
                ShowModalAlerta(result.Msg);
        }
    });  
}

/*RECUPERAR COLUNAS*/
function RecuperarSequenciaColunas(periodoEditavel) {
    var gridColunas = [];
    var lstColunas = [];
   
    if (tipoGrid == 1)
        nomeGrid = 'DocumentosFiscais';
    else
        nomeGrid = 'DocumentosFiscaisCompleto';
    
    $.ajax({
        url: "/DocumentosFiscais/RecuperarSequenciaColunas?nomeGrid=" + nomeGrid,
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            gridColunas = result.Data;
        }
    });

    if (gridColunas != '') {
        if (periodoEditavel) {            
            lstColunas.push(colCommand);
        }

        if (tipoGrid == 1) {
            gridColunas.forEach(function (col) {
                lstColunasPadrao.forEach(function (colPadrao) {
                    if (col.Titulo == colPadrao.title) {
                        var newCol = new GridColumn(colPadrao, col.Visivel, col.Travada)
                        lstColunas.push(newCol);
                        return;
                    }
                });
            });
        }
        else {
            gridColunas.forEach(function (col) {
                lstColunasCompletoPadrao.forEach(function (colPadrao) {
                    if (col.Titulo == colPadrao.title) {
                        var newCol = new GridColumn(colPadrao, col.Visivel, col.Travada)
                        lstColunas.push(newCol);
                        return;
                    }
                });                           
            });
        }
    }
    else {
        if (tipoGrid == 1)
            lstColunasPadrao.forEach(function (col) {
                lstColunas.push(col);
            });
        else
            lstColunasCompletoPadrao.forEach(function (col) {
                lstColunas.push(col);
            });        

        if (!periodoEditavel) 
            lstColunas.splice(0, 1);        
    }

    return lstColunas;
}

/*CRIAR COLUNA*/
var GridColumn = function (e, visivel, travada) {
    this.hidden = !visivel;

    if (e.command != null)
        this.command = e.command;

    if (e.field != null)
        this.field = e.field;

    if (e.title != null)
        this.title = e.title;

    if (e.width != null)
        this.width = e.width;
    
    if (e.template != null)
        this.template = e.template;

    if (e.attributes != null)
        this.attributes = e.attributes;

    if (e.footerTemplate != null)
        this.footerTemplate = e.footerTemplate;

    if (e.editor != null) {
        switch (e.field) {
            case "Movimento":
                this.editor = colEditorMovimento;
                break;
            case "IdCategoriaNF":
                this.editor = colEditorCategoriaNF;
                break;
            case "CFOP":
            case "CFOPRef":
                this.editor = colEditorCFOP;
                break;
        }        
    }        

    if (e.sortable != null)
        this.sortable = e.sortable;

    if(e.locked != null)
        this.locked = travada;
}

/*GRID DE ITENS (MODAL ITENS)*/
function CarregarGridItens(idDocumentoFiscal, periodoEditavel) {
    var windowTemplate = kendo.template($("#RemoverTemplate").html());
    $("#gridItens").html("");
    $("#gridItens").kendoGrid({
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                },
                model: {
                    id: "Id",
                    fields: {
                        IdTNAControle: { validation: { required: false } },
                        NumeroItem: { validation: { required: true } },
                        CFOP: { type: "text", validation: { required: false } },
                        CFOPRef: { type: "text", validation: { required: false } },
                        Material: { validation: { required: false } },
                        DescricaoMaterial: { validation: { min: 0, required: true } },
                        GrupoMercadoria: { validation: { required: false } },
                        PedCompra: { validation: { required: false } },
                        Centro: { validation: { required: false } },
                        Quantidade: { type: "number", validation: { required: false } },
                        TotalItem: { type: "number", validation: { required: false } },
                        ValorLiquido: { type: "number", validation: { required: false } },
                        TextoICMS: { validation: { required: false } },
                        TextoIPI: { validation: { required: false } },
                        LeiCofins: { validation: { required: true } },
                        LeiTribPIS: { validation: { required: true } },
                        BaseICMS: { type: "number", validation: { required: false } },
                        AliqICMS: { validation: { required: false }, editable: false },
                        ValorICMS: { type: "number", validation: { required: false } },
                        BaseExcluida: { type: "number", validation: { required: false } },
                        OutraBase: { type: "number", validation: { required: false } },
                        AliqIPI: { type: "number", validation: { required: false } },
                        ValorIPI: { type: "number", validation: { required: false } },
                        AliqCOFINS: { type: "number", validation: { required: false }},
                        ValorCOFINS: { type: "number", validation: { required: false } },
                        AliqPIS: { type: "number", validation: { required: false } },
                        ValorPIS: { type: "number", validation: { required: false } },
                        CodigoIVA: { validation: { required: false } },
                        Total: { type: "number", validation: { required: false } },
                        DataEntrada: { type: "date", validation: { required: true } },
                        DataAtualizacao: { type: "date", validation: { required: true } }
                    }
                }
            },
            pageSize: 10,
            transport: {
                read: {
                    url: "/DocumentosFiscais/RecuperarItens",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                },
                update: {
                    url: "/DocumentosFiscais/AtualizarItem",
                    dataType: "json",
                    type: "POST",
                    async: false,//aguardamos a atualização
                    contentType: "application/json",//necessário para stringfy
                    complete: function (e) {
                        $("#gridItens").data("kendoGrid").dataSource.read();
                    }
                },
                destroy: {
                    url: "/DocumentosFiscais/RemoverItem",
                    dataType: "json",
                    type: "POST",
                    async: false,
                    contentType: "application/json" //necessário para stringfy
                },
                parameterMap: function (data, type) {
                    if (type == "read") {
                        return {
                            idDocumentoFiscal: idDocumentoFiscal
                        }
                    }
                    else if (type == "update") {
                        return JSON.stringify({ entidade: data });
                    }
                    else if (type == "destroy") {
                        return JSON.stringify({ idDocumentoFiscal: idDocumentoFiscal, idDocumentoFiscalItem: data.Id })
                    }
                }
            },
            aggregate: [
                { field: "TotalItem", aggregate: "sum" },
                { field: "BaseCalculoCOFINS", aggregate: "sum" },
                { field: "ValorCOFINS", aggregate: "sum" },
                { field: "BaseCalculoPIS", aggregate: "sum" },
                { field: "ValorPIS", aggregate: "sum" }
            ]
        },
        scrollable: true,
        sortable: true,
        resizable: true,
        filterable: true,
        reorderable: true,
        pageable: {
            pageSizes: [10, 25, 50, 100, 500]
        },
        columns: [
            {
                command: ["edit",
                {
                    name: "Excluir",
                    text: " <span class='k-icon k-delete'></span>Excluir",
                    //className: "k-edit",
                    click: function (e) {  //add a click event listener on the delete button
                        var grid = $("#gridItens").data("kendoGrid");
                        var dataSource = grid.dataSource;
                        var recordsOnCurrentView = dataSource.view().length;
                        if (recordsOnCurrentView > 1)
                        {
                            var tr = $(e.target).closest("tr"); //get the row for deletion
                            var data = this.dataItem(tr); //get the row data so it can be referred later
                            janelaRemocaoItem.content(windowTemplate(data)); //send the row data object to the template and render it
                            janelaRemocaoItem.open().center();

                            $("#yesButton").click(function () {
                                grid.dataSource.remove(data)  //prepare a "destroy" request 
                                grid.dataSource.sync()  //actually send the request (might be ommited if the autoSync option is enabled in the dataSource)
                                janelaRemocaoItem.close();
                                grid.dataSource.read();
                            })
                            $("#noButton").click(function () {
                                janelaRemocaoItem.close();
                            })
                        }
                        else {
                            ShowModalAlerta("Não é possível excluir quando existe apenas 1 item.");                            
                        }
                    }
                }],
                title: "&nbsp;",
                width: "180px",
                hidden: !periodoEditavel,
                locked: true
            },
            { field: "Id", hidden: true },
            { field: "IdTNAControle", hidden: true },
            { field: "NumeroItem", title: "Número", width: "100px", locked: true, footerTemplate: '<span style=\'float:right;\'>TOTAIS:</span>' },
            { field: "CFOP", title: "CFOP", width: "200px", editor: MostrarOpcoesCfops },            
            { field: "CFOPRef", title: "CFOP Ref.", width: "200px", editor: MostrarOpcoesCfopsRef },
            { field: "Material", title: "Material", width: "150px" },
            { field: "DescricaoMaterial", title: "Descrição do Material", width: "300px" },
            { field: "GrupoMercadoria", title: "Grupo de Mercadoria", width: "180px" },
            { field: "PedCompra", title: "Pedido de Compra", width: "150px" },
            { field: "Centro", title: "Centro", width: "80px" },
            { field: "Quantidade", title: "Quantidade", width: "120px", template: '#=Quantidade.FormatarMoeda(2, undefined, ".", ",")#', attributes: { style: "text-align:right;" } },
            { field: "TotalItem", title: "Total do Item", width: "120px", template: '#=TotalItem.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" }, footerTemplate: "<div style='float: right'>#= kendo.toString(sum, 'c2') #</div>" },
            { field: "ValorLiquido", title: "Valor Líquido", width: "120px", template: '#=ValorLiquido.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "TextoICMS", title: "Texto ICMS", width: "120px" },
            { field: "TextoIPI", title: "Texto IPI", width: "120px" },
            { field: "LeiCofins", title: "Lei COFINS", width: "120px" },
            { field: "LeiTribPIS", title: "Lei PIS", width: "120px" },
            { field: "BaseICMS", title: "Base ICMS", width: "120px", template: '#=BaseICMS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "AliqICMS", title: "Alíq ICMS", width: "120px", template: '#=kendo.format("{0:p}", AliqICMS / 100)#', attributes: { style: "text-align:right;" } },
            { field: "ValorICMS", title: "Valor ICMS", width: "120px", template: '#=ValorICMS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "BaseExcluida", title: "Base Excluída", width: "150px", template: '#=BaseExcluida.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "OutraBase", title: "Outra Base", width: "120px", template: '#=OutraBase.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "AliqIPI", title: "Alíq IPI", width: "120px", template: '#=kendo.format("{0:p}", AliqIPI / 100)#', attributes: { style: "text-align:right;" } },
            { field: "ValorIPI", title: "Valor IPI", width: "120px", template: '#=ValorIPI.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "AliqCOFINS", title: "Alíq COFINS", width: "120px", template: '#=kendo.format("{0:p}", AliqCOFINS / 100)#', attributes: { style: "text-align:right;" } },
            { field: "ValorCOFINS", title: "Valor COFINS", width: "120px",template: '#=ValorCOFINS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" }, footerTemplate: "<div style='float: right'>#= kendo.toString(sum, 'c2') #</div>" },
            { field: "AliqPIS", title: "Alíq PIS", width: "120px", template: '#=kendo.format("{0:p}", AliqPIS / 100)#', attributes: { style: "text-align:right;" } },
            { field: "ValorPIS", title: "Valor PIS", width: "120px", template: '#=ValorPIS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" }, footerTemplate: "<div style='float: right'>#= kendo.toString(sum, 'c2') #</div>" },
            { field: "CodigoIVA", title: "Código IVA", width: "120px" },
            { field: "Total", title: "Total", width: "80px", template: '#=Total.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } }
        ],
        editable: "inline"
    });
}

/*GRID DE VALIDAÇÕES (FILTROS)*/
function CarregarGridValidacoes(idControle) {
    $("#gridValidacoes").html("");
    $("#gridValidacoes").kendoGrid({
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                }
            },
            pageSize: 25,
            transport: {
                read: {
                    url: "/Validacoes/PesquisarSumariosPorPeriodo",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type == "read") {
                        return {
                            idTnaControle: idControle
                        }
                    }
                }
            }
        },
        selectable: true,
        change: selecionandoValidacao,
        scrollable: true,
        sortable: true,
        columns: [
            { field: "Id", hidden: true },
            { field: "Nome", title: "Campo", width: "40px" },
            { field: "Descricao", title: "Mensagem", width: "80px" },
            { field: "Ocorrencias", title: "Ocorrências", width: "20px" }
        ]
    });
}

/*GRID DE FILTROS (MODAL FILTROS)*/
function CarregarFiltros() {
    $("#gridFiltros").html("");
    $("#gridFiltros").kendoGrid({
        dataSource: {
            schema: {
                model: {
                    id: "Id",
                    fields: {
                        Nome: {
                            type: 'text', validation: {
                                required: true,
                                nomevalidation: function (input) {
                                    if (input.is("[name='Nome']") && input.val() != "") {
                                        input.attr("data-nomevalidation-msg", "Algum campo do filtro deve estar preenchido para ser salvo!");
                                        var cfops = $('#kmsCFOP').data('kendoMultiSelect').value().toString();
                                        var csts = $('#kmsCST').val();
                                        var categoria = $('#txtCatNota').data('kendoMultiSelect').value().toString();
                                        var codigoParticipante = $('#kmsCodigoCliente').data('kendoMultiSelect').value().toString();
                                        var material = $('#kmsMaterial').data('kendoMultiSelect').value().toString();
                                        var grupoMercadoria = $('#kmsGrpMercadoria').data('kendoMultiSelect').value().toString();
                                        var movimento = $('#ddlMovimento').data('kendoDropDownList').value().toString();
                                        var totalItem = $('#txtTotalItem').val();

                                        if (cfops.length == 0 && csts.length == 0 && categoria.length == 0 && codigoParticipante.length == 0 && material.length == 0 && grupoMercadoria.length == 0 && movimento.length == 0 && totalItem == "0")
                                            return false
                                        else
                                            return true;
                                    }

                                    return true;
                                }
                            }
                        },
                        Descricao: { type: 'text', validation: { required: true } },
                        IdUsuario: { validation: { required: false } },
                        DataEntrada: { validation: { required: false } },
                        DataAtualizacao: { validation: { required: false } }
                    }
                },
                data: function (result) {
                    return result.Data;
                }
            },
            transport: {
                read: {
                    url: "/DocumentosFiscais/PesquisarFiltros",
                    dataType: "json",
                    type: "GET",
                    async: true,
                    cache: false
                },
                create: {
                    url: "/DocumentosFiscais/CriarFiltro",
                    dataType: "json",
                    type: "POST",
                    async: false,//aguardamos a atualização
                    contentType: "application/json"//necessário para stringfy
                },
                update: {
                    url: "/DocumentosFiscais/AtualizarFiltro",
                    dataType: "json",
                    type: "POST",
                    async: false,//aguardamos a atualização
                    contentType: "application/json"//necessário para stringfy
                },
                destroy: {
                    url: "/DocumentosFiscais/RemoverFiltro",
                    dataType: "json",
                    type: "POST",
                    async: false,//aguardamos a atualização
                    contentType: "application/json"//necessário para stringfy
                },
                parameterMap: function (data, type) {
                    if (type == "create") {
                        return JSON.stringify({
                            entidade: data,
                            cfops: $('#kmsCFOP').data('kendoMultiSelect').value().toString(),
                            csts: $('#kmsCST').val(),
                            categoria: $('#txtCatNota').data('kendoMultiSelect').value().toString(),
                            codigoclientefornecedor: $('#kmsCodigoCliente').data('kendoMultiSelect').value().toString(),
                            material: $('#kmsMaterial').data('kendoMultiSelect').value().toString(),
                            movimento: $('#ddlMovimento').data('kendoDropDownList').value().toString(),
                            grupomercadoria: $('#kmsGrpMercadoria').data('kendoMultiSelect').value().toString()
                        });
                    }
                    else if (type == "destroy") {
                        return JSON.stringify({ idFiltro: data.Id });
                    }
                    else if (type == "update") {
                        return JSON.stringify({
                            entidade: data,
                            cfops: $('#kmsCFOP').data('kendoMultiSelect').value().toString(),
                            csts: $('#kmsCST').val(),
                            categoria: $('#txtCatNota').data('kendoMultiSelect').value().toString(),
                            codigoclientefornecedor: $('#kmsCodigoCliente').data('kendoMultiSelect').value().toString(),
                            material: $('#kmsMaterial').data('kendoMultiSelect').value().toString(),
                            movimento: $('#ddlMovimento').data('kendoDropDownList').value().toString(),
                            grupomercadoria: $('#kmsGrpMercadoria').data('kendoMultiSelect').value().toString()
                        });
                    }
                }
            }
        },
        scrollable: true,
        sortable: true,
        resizable: true,
        filterable: true,
        selectable: true,
        change: selecionandoFiltro,
        pageSize: 10,
        pageable: {
            pageSizes: [10, 25, 50, 100, 500]
        },
        height: '400px',
        editable: "inline",
        toolbar: ["create"],
        columns: [
            {
                command: [
                  "destroy","edit"
                ], title: "&nbsp;", width: "40px"
            },
            { field: "Id", hidden: true },
            { field: "Nome", title: "Nome do Filtro", width: "40px" },
            { field: "Descricao", title: "Descrição", width: "80px" }
        ]
    });
}

/*GRID-MODAL DE HISTÓRICO POR DOCUMENTO*/
function CarregarHistorico(idDoc,idItem) {
    $('#modalHistorico').modal();
    $('#modalHistorico .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalHistorico .modal-dialog .modal-header center .modal-title strong').html('Histórico de Modificações');
    $("#gridHistorico").html("");

    var gridHistorico =
    $("#gridHistorico").kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: 'HistoricoModificao_' + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + '.xlsx',
            allPages: true
        },
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                if (rowIndex % 2 == 0) {
                    var row = sheet.rows[rowIndex];
                    for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                        row.cells[cellIndex].background = "#aabbcc";
                    }
                }
            }
        },
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                },
                model: {
                    id: "Id",
                    fields: {
                        NomeUsuario: { type: "text", validation: { required: true } },
                        DLcto: { type: "date", validation: { required: true } },
                        DataModificacao: { type: "date", validation: { required: true } },
                        Modificacao: { type: "text", validation: { required: true } }
                    }
                }
            },
            transport: {
                read: {
                    url: "/DocumentosFiscais/PesquisarDocumentosLog",
                    dataType: "json",
                    type: "GET",
                    async: true,//não aguardamos a busca
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type == "read") {
                        return {
                            idDocumentoFiscal: idDoc,
                            idDocumentoFiscalItem: idItem
                        }
                    }
                }
            },
            pageSize: 10
        },
        scrollable: true,
        sortable: true,
        pageable: true,
        resizable: true,
        columns: [
            { field: "Id", hidden: true },
            { field: "DocNum", title: "DocNum", width: "120px"},
            { field: "Movimento", title: "Movimento", width: "120px" },
            { field: "NumeroSerie", title: "Nota Fiscal", width: "120px" },
            { field: "CategoriaNF", title: "Categoria", width: "180px" },
            { field: "DLcto", title: "Data Lançamento", width: "120px", template: "#= kendo.toString(kendo.parseDate(DLcto, 'yyyy-MM-dd'), 'dd/MM/yyyy') #" },
            { field: "TotalItem", title: "Total do Item", width: "120px", template: '#=TotalItem.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "BaseCalculoPIS", title: "Base de PIS", width: "120px", template: '#=BaseCalculoPIS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "ValorPIS", title: "Valor de PIS", width: "120px", template: '#=ValorPIS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "BaseCalculoCOFINS", title: "Base de COFINS", width: "120px", template: '#=BaseCalculoCOFINS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "ValorCOFINS", title: "Valor de COFINS", width: "120px", template: '#=ValorCOFINS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "Centro", title: "Centro", width: "120px" },
            { field: "CodigoIVA", title: "Código do IVA", width: "120px" },
            { field: "NomeUsuario", title: "Usuário", width: "120px" },
            { field: "DataModificacao", title: "Data/Hora da Modificação", width: "180px", template: "#= kendo.toString(kendo.parseDate(DataModificacao, 'yyyy-MM-dd HH:mm:ss'), 'dd/MM/yyyy HH:mm:ss') #" },
            { field: "Modificacao", title: "Modificação", width: "300px" }
        ]
    }).data("kendoGrid");

    gridHistorico.thead.kendoTooltip({
        filter: "th",
        content: function (e) {
            var target = e.target;
            return $(target).text();
        }
    });
}

/*GRID-SUB HISTÓRICO POR PERÍODO*/
function CarregarHistoricoPeriodo(idControle) {
    $("#gridHistoricoPeriodo").html("");

    var gridHistoricoPeriodo = 
    $("#gridHistoricoPeriodo").kendoGrid({
        toolbar: ["excel"],
        excel: {
            fileName: 'HistoricoModificaoPeriodo_' + Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1) + '.xlsx',
            allPages: true
        },
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            for (var rowIndex = 1; rowIndex < sheet.rows.length; rowIndex++) {
                if (rowIndex % 2 == 0) {
                    var row = sheet.rows[rowIndex];
                    for (var cellIndex = 0; cellIndex < row.cells.length; cellIndex++) {
                        row.cells[cellIndex].background = "#aabbcc";
                    }
                }
            }
        },
        dataSource: {
            schema: {
                data: function (result) {
                    return result.Data;
                },
                total: function (result) {
                    return result.Total;
                },
                model: {
                    id: "Id",
                    fields: {
                        NomeUsuario: { type: "text", validation: { required: true } },
                        DLcto: { type: "date", validation: { required: true } },
                        DataModificacao: { type: "date", validation: { required: true } },
                        Modificacao: { type: "text", validation: { required: true } }
                    }
                }
            },
            transport: {
                read: {
                    url: "/DocumentosFiscais/PesquisarControleLog",
                    dataType: "json",
                    type: "GET",
                    async: true,//não aguardamos a busca
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type == "read") {
                        return {
                            idControle: idControle
                        }
                    }
                }
            },
            pageSize: 10
        },
        scrollable: true,
        sortable: true,
        pageable: true,
        resizable: true,
        columns: [
            { field: "Id", hidden: true },
            { field: "DocNum", title: "DocNum", width: "120px" },
            { field: "Movimento", title: "Movimento", width: "120px" },
            { field: "NumeroSerie", title: "Nota Fiscal", width: "120px" },
            { field: "CategoriaNF", title: "Categoria", width: "180px" },
            { field: "DLcto", title: "Data Lançamento", width: "120px", template: "#= kendo.toString(kendo.parseDate(DLcto, 'yyyy-MM-dd'), 'dd/MM/yyyy') #" },
            { field: "TotalItem", title: "Total do Item", width: "120px", template: '#=TotalItem.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "BaseCalculoPIS", title: "Base de PIS", width: "120px", template: '#=BaseCalculoPIS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "ValorPIS", title: "Valor de PIS", width: "120px", template: '#=ValorPIS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "BaseCalculoCOFINS", title: "Base de COFINS", width: "120px", template: '#=BaseCalculoCOFINS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "ValorCOFINS", title: "Valor de COFINS", width: "120px", template: '#=ValorCOFINS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "Centro", title: "Centro", width: "120px" },
            { field: "CodigoIVA", title: "Código do IVA", width: "120px" },
            { field: "NomeUsuario", title: "Usuário", width: "120px" },
            { field: "DataModificacao", title: "Data/Hora da Modificação", width: "180px", template: "#= kendo.toString(kendo.parseDate(DataModificacao, 'yyyy-MM-dd HH:mm:ss'), 'dd/MM/yyyy HH:mm:ss') #" },
            { field: "Modificacao", title: "Modificação", width: "300px" }
        ]
    }).data("kendoGrid");

    gridHistoricoPeriodo.thead.kendoTooltip({
        filter: "th",
        content: function (e) {
            var target = e.target;
            return $(target).text();
        }
    });
}

/*AÇÕES - PESQUISA NA TELA (FILTROS)*/
function PesquisarDocs() {
    $('#grid').data('kendoGrid').dataSource.read();
    $("#gridHistoricoPeriodo").data("kendoGrid").dataSource.read();
}

/*AÇÕES - REQUISITAR MODAL ITENS (MODAL ITENS)*/
function VerItens(e) {
    var dataHtml = '';
    var dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);

    CarregarGridItens(dataItem.Id, periodoEditavel);

    if (periodoEditavel == true)
        $("#lbPeriodoFechadoItem").text('');
    else
        $("#lbPeriodoFechadoItem").text('PERÍODO FECHADO');

    CarregarPopUpItens();
}

/*AÇÕES - CONTRUTOR MODAL ITENS (MODAL ITENS)*/
function CarregarPopUpItens() {
    $('#modalVerItens').modal({ backdrop: 'static', keyboard: false });
    $('#modalVerItens .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalVerItens .modal-dialog .modal-header center .modal-title strong').html('Itens do Documento Fiscal');
}

/*AÇÕES - DESTRUIR MODAL ITENS (MODAL ITENS)*/
function DestruirModalItem(e) {
    $("#gridItens").data("kendoGrid").destroy();
    $("#gridItens").html("");
    $('#modalVerItens').modal('hide');
}

/*AÇÕES - REQUISITAR MODAL FILTROS (MODAL FILTROS PERSONALIZADOS)*/
function ModalFiltros() {
    $('#btnAplicarFiltros').attr('disabled', 'disabled');
    CarregarFiltros();
    $('#modalFiltros').modal({ backdrop: 'static', keyboard: false });
    $('#modalFiltros .modal-dialog .modal-header center .modal-title strong').html("");
    $('#modalFiltros .modal-dialog .modal-header center .modal-title strong').html('Filtros Pré-Configurados');
}

/*AÇÕES - SELECIONAR FILTRO PERSONALIZADO*/
function selecionandoFiltro(arg) {
    var entityGrid = $("#gridFiltros").data("kendoGrid");
    var dataItem = entityGrid.dataItem(entityGrid.select());
    if(dataItem == null)
        $('#btnAplicarFiltros').attr('disabled', 'disabled')
    else
        $('#btnAplicarFiltros').removeAttr('disabled');    
}

/*AÇÕES - APLICAR FILTROS (MODAL FILTROS PERSONALIZADOS)*/
function AplicarFiltros() {
    var entityGrid = $("#gridFiltros").data("kendoGrid");
    var dataItem = entityGrid.dataItem(entityGrid.select());
    var idFiltro = dataItem == null ? 0 : dataItem.Id;
    var dsFiltros = undefined;
    $.ajax({
        url: "/DocumentosFiscais/BuscarParametros?idFiltro=" + idFiltro,
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            dsFiltros = result.Data;
        }
    });

    $('#txtCatNota').data('kendoMultiSelect').value('');
    $('#kmsCFOP').data('kendoMultiSelect').value('');
    $('#kmsCST').val('');
    $('#kmsCodigoCliente').data('kendoMultiSelect').value('');
    $('#kmsMaterial').data('kendoMultiSelect').value('');
    $('#kmsGrpMercadoria').data('kendoMultiSelect').value('');
    $('#ddlMovimento').data('kendoDropDownList').value('');

    for (i = 0; i < dsFiltros.length ; i++) {
        if (dsFiltros[i].Campo == 'CATEGORIA')
        {
            var valor = JSON.parse('[' + dsFiltros[i].Valor + ']');
            $('#txtCatNota').data('kendoMultiSelect').value(valor);
        }

        if (dsFiltros[i].Campo == 'CFOP') {
            var valor = JSON.parse('[' + dsFiltros[i].Valor + ']');
            $('#kmsCFOP').data('kendoMultiSelect').value(valor);
        }

        if (dsFiltros[i].Campo == 'CST') {
            var valor =  dsFiltros[i].Valor;
            $('#kmsCST').val(valor);
        }

        if (dsFiltros[i].Campo == 'CODIGOPARTICIPANTE') {
            var valor = dsFiltros[i].Valor;
            $('#kmsCodigoCliente').data('kendoMultiSelect').value(valor);
        }

        if (dsFiltros[i].Campo == 'MATERIAL') {
            var valor = dsFiltros[i].Valor;
            $('#kmsMaterial').data('kendoMultiSelect').value(valor);
        }

        if (dsFiltros[i].Campo == 'GRUPOMERCADORIA') {
            var valor = dsFiltros[i].Valor;
            $('#kmsGrpMercadoria').data('kendoMultiSelect').value(valor);
        }

        if (dsFiltros[i].Campo == 'MOVIMENTO') {
            var valor = dsFiltros[i].Valor;
            $('#ddlMovimento').data('kendoDropDownList').value(valor);
        }
    }
}

/*AÇÕES - DESTRUIR MODAL FILTROS (MODAL FILTROS PERSONALIZADOS)*/
function DestruirModalFiltros() {
    $('#gridFiltros').data('kendoGrid').destroy();
    $('#gridFiltros').html('');
    $('#modalFiltros').modal('hide');
}

/*TEMPLATE DATABIND DE CATEGORIA*/
function RecuperarCategoriaExibicaoGrid(idCategoria) {
    var dsCategorias = BuscarCategorias();
    for (var i = 0, length = dsCategorias.length; i < length; i++) {
        if (dsCategorias[i].Id === idCategoria) {
            return dsCategorias[i].Descricao;
        }
    }
}

/*TEMPLATE DATABIND DE ENTRADA OU SAÍDA*/
function MostrarMovimento(movimento){
    if (movimento == 'E') {
        return 'Entrada';
    }
    else if (movimento == 'S') {
        return 'Saída';
    }
    else {
        return movimento;
    }
}

/*TEMPLATE DE EDIÇÃO DE ENTRADA OU SAÍDA*/
function MostrarOpcoesMovimento(container, options) {
    $('<input data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        dataTextField: "texto",
        dataValueField: "valor",
        dataSource: BuscarMovimento()
    });
}

/*TEMPLATE DE EDIÇÃO DE CATEGORIA*/
function RecuperarCategoriasEdicao(container,options) {
    $('<input data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        dataTextField: "Descricao",
        dataValueField: "Id",
        dataSource: BuscarCategorias()
    });
}

/*TEMPLATE DE EDIÇÃO DE CFOPS*/
function MostrarOpcoesCfops(container, options) {
    $('<input data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        template: kendo.template('<span class="cfop-codigo">#= Codigo # -</span> #= Descricao #'),
        dataTextField: "Codigo",
        dataValueField: "Codigo",        
        dataSource: BuscarCFOPs(),
        optionLabel: {
            Id: 999,
            Codigo: "",
            Descricao: "Sem CFOP"
        }
    });
}

function MostrarOpcoesCfopsRef(container, options) {
    $('<input data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        template: kendo.template('<span class="cfop-codigo">#= Codigo # -</span> #= Descricao #'),
        dataTextField: "Codigo",
        dataValueField: "Codigo",
        dataSource: BuscarCFOPRefs(),
        optionLabel: {
            Id: 999,
            Codigo: "",
            Descricao: "Sem CFOP"
        }
    });
}

/*TEMPLATE DE EDIÇÃO DE CST*/
function MostrarOpcoesCST(container, options) {
    $('<input data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        template: kendo.template('<span class="cfop-codigo">#= Codigo # -</span> #= Descricao #'),
        dataTextField: "Codigo",
        dataValueField: "Codigo",
        dataSource: BuscarCFOPs()
    });
}

/*DATASOURCE - BUSCAR INFO REGRAS ASSOCIADAS*/
function InfoRegrasPorItem(idItem) {
    var ds = "";
    $.ajax({
        url: "/DocumentosFiscais/ObterInfoRegras",
        data: { idItem: idItem },
        type: "GET",
        async: false,
        dataType: "json",
        cache: true,
        success: function (result) {
            if (result.Sucesso) {
                ds = result.Data;
            }
        }
    });
    return ds;
}

/*DATASOURCE - CATEGORIAS*/
function BuscarCategorias() {
    var dsCategorias = JSON.parse(localStorage.getItem('CategoriasStorage'));

    if (dsCategorias == null) {
        $.ajax({
            url: "/DocumentosFiscais/BuscarCategoriasNotaFiscal",
            type: "GET",
            async: false,
            dataType: "json",
            cache: true,
            success: function (result) {
                if (result.Sucesso)
                {
                    localStorage.setItem('CategoriasStorage', JSON.stringify(result.Data));
                    dsCategorias = result.Data;
                }
                else
                    ShowModalAlerta(result.Msg);
            }
        });
    }

    return dsCategorias;
};

/*DATA SOURCE - CFOP*/
function BuscarCFOPs() {
    var dsCfops = JSON.parse(localStorage.getItem('CFOPsStorage'));

    if (dsCfops == null) {
        $.ajax({
            url: "/DocumentosFiscais/BuscarCFOPs",
            type: "GET",
            async: false,
            dataType: "json",
            cache: true,
            success: function (result) {
                if (result.Sucesso) {
                    localStorage.setItem('CFOPsStorage', JSON.stringify(result.Data));
                    dsCfops = result.Data;
                }
                else
                    ShowModalAlerta(result.Msg);
            }
        });
    }

    return dsCfops;
};

/*DATA SOURCE - CFOP*/
function BuscarCFOPRefs() {
    var dsCfops = JSON.parse(localStorage.getItem('CFOPRefsStorage'));

    if (dsCfops == null) {
        $.ajax({
            url: "/DocumentosFiscais/BuscarCFOPRefs",
            type: "GET",
            async: false,
            dataType: "json",
            cache: true,
            success: function (result) {
                if (result.Sucesso) {
                    localStorage.setItem('CFOPRefsStorage', JSON.stringify(result.Data));
                    dsCfops = result.Data;
                }
                else
                    ShowModalAlerta(result.Msg);
            }
        });
    }

    return dsCfops;
};

/*DATA SOURCE - GRUPO MERCADORIA*/
function BuscarGruposMercadorias(idControle) {
    var dsGrupos;

    if (idControle == undefined)
        return;

    $.ajax({
        url: "/DocumentosFiscais/BuscarGruposMercadorias?idControle=" + idControle,
        type: "GET",
        async: false,
        dataType: "json",
        cache: true,
        success: function (result) {
            if (result.Sucesso) 
                dsGrupos = result.Data;            
            else
                ShowModalAlerta(result.Msg);
        }
    });

    return dsGrupos;
};

/*DATA SOURCE - CODIGO MATERIAIS*/
function BuscarCodigosMateriais(idControle) {
    var dsCodigos;

    if (idControle == undefined)
        return;

    $.ajax({
        url: "/DocumentosFiscais/BuscarCodigosMateriais?idControle=" + idControle,
        type: "GET",
        async: false,
        dataType: "json",
        cache: true,
        success: function (result) {
            if (result.Sucesso) {
                dsCodigos = result.Data;
            }
            else
                ShowModalAlerta(result.Msg);
        }
    });

    return dsCodigos;
};

/*DATA SOURCE - MOVIMENTO*/
function BuscarMovimento() {
    var dsMovimentos = [
    { texto: 'Entrada', valor: 'E' },
    { texto: 'Saída', valor: 'S' }
    ];
    return dsMovimentos;
}

/*DATA SOURCE - REGRAS ASSOCIADAS FILTRO*/
function BuscarParticipacaoRegras() {
    var dsPRegra = [
    { texto: 'Sim', valor: true },
    { texto: 'Não', valor: false }
    ];
    return dsPRegra;
}

/*DATA SOURCE - CST*/
function BuscarCSTs(empresa, movimento){

    if (empresa == "") {
        if (movimento == "E") {

        }
        else {

        }
    }
    else if (empresa == "") {
        if (movimento == "E") {

        }
        else {

        }
    }
}

/*VERIFICAÇÃO DO STATUS DO PERÍODO SELECIONADO*/
function VerificarStatusPeriodo(idControle) {
    $.ajax({
        url: "/DocumentosFiscais/ObterStatusPeriodo?idControle=" + idControle,
        type: "GET",
        async: false,
        dataType: "json",
        cache: false,
        success: function (result) {
            if (result.Sucesso) 
                periodoEditavel = result.Data;
            else
                ShowModalAlerta(result.Msg);
        }
    });
}


/* Construtor de Filtro Total Item */
function ContrutorFiltroTotalItem() {
    PrepararConstrutor();
    $('#modalContrutorFiltro').modal({ backdrop: 'static', keyboard: false });   
}

function PrepararConstrutor() {
    var dsComparacao = [];
    
    dsComparacao = [
        { Texto: 'Igual', Valor: '=' },
        { Texto: 'Diferente', Valor: '<>' },
        { Texto: 'Maior', Valor: '>' },
        { Texto: 'Maior Igual', Valor: '>=' },
        { Texto: 'Menor', Valor: '<' },
        { Texto: 'Menor Igual', Valor: '<=' }
    ];

    $('#ddlComparacao').kendoDropDownList({
        dataTextField: "Texto",
        dataValueField: "Valor",
        dataSource: dsComparacao,
        optionLabel: "Selecione"
    });

    $('#ddlComparacao2').kendoDropDownList({
        dataTextField: "Texto",
        dataValueField: "Valor",
        dataSource: dsComparacao,
        optionLabel: "Selecione"
    });
}

function AplicarFiltros() {
    var filtro = '0,00';
    var comparacao1 = $('#ddlComparacao').data('kendoDropDownList').value().toString();
    var valor1 = $('#tbValor').val();
    var combinacao = $('#selectCombinacao').bootstrapSwitch('state') == true ? "E" : "OU";
    var comparacao2 = $('#ddlComparacao2').data('kendoDropDownList').value().toString();
    var valor2 = $('#tbValor2').val();

    if (comparacao1 == '' && valor1 != '0,00') {
        ShowModalAlerta('Favor selecionar a primeira comparação.');
        return;
    }

    if (comparacao2 == '' && valor2 != '0,00') {
        ShowModalAlerta('Favor selecionar a segunda comparação.');
        return;
    }

    if (comparacao1 != '') 
        filtro = comparacao1 + ' ' + valor1;

    if (comparacao2 != '') {
        filtro += ' ' + combinacao + ' ' + comparacao2 + ' ' + valor2;
    }

    $('#txtTotalItem').val(filtro);
    $('#modalContrutorFiltro').modal('hide');
}

/**Edição de Campos**/

/*Esconder Campo de Edição*/
function HideInputGroup(e) {
    $(e).closest('.form-group').hide();
    return false;
}