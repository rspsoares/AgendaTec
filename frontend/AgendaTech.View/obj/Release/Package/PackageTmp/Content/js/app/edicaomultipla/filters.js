/*CONSTRUCTOR - INITIALIZE COMPONENTS*/
var idControle = 0;
function CarregaComponentes() {
    InitializeFilterComponents();
    InitializeFilterModalComponents();
    CarregarGridEditavelDocumentosFiscais();
    CarregarGridDocumentosFiscais(0);
    InitializeGridComponents();
}

/*CONSTRUCTOR - INITIALIZE FILTERS*/

/*CLASS INITIALIZE*/
function InitializeFilterComponents() {
    // ativar filtros previamente ativos
    AtivarFiltros();

    // matriz
    $('#iCodMatriz').kendoDropDownList({
        dataTextField: "RazaoSocial",
        dataValueField: "Id",
        dataSource: LoadDsMatrizes(),
        optionLabel: "Matriz",
        template: "#= CodigoMatriz # - #=RazaoSocial #",
        select: function (e) {
            var dataItem = this.dataItem(e.item);
            idMatriz = dataItem.Id;
            CarregarPeriodos(idMatriz);
        }
    });

    // período
    $('#iPeriodo').kendoDropDownList({
        optionLabel: "Período",
    });

    // movimentação
    $('#iMovimento').kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: BuscarMovimento(),
        optionLabel: "Entrada ou Saída"
    });

    // data de emissão (DE)
    $('#iDtEmiDe').kendoDatePicker({
        format: "dd/MM/yyyy",
        start: "year"
    });

    // data de emissão (ATÉ)
    $('#iDtLancDe').kendoDatePicker({
        format: "dd/MM/yyyy",
        start: "year"
    });

    // data de lançamento (DE)
    $('#iDtEmiAte').kendoDatePicker({
        format: "dd/MM/yyyy",
        start: "year"
    });

    // data de lançamento (ATÉ)
    $('#iDtLancAte').kendoDatePicker({
        format: "dd/MM/yyyy",
        start: "year"
    });

    // categoria da nota
    $('#iCategoria').kendoMultiSelect({
        placeholder: "Categoria",
        itemTemplate: kendo.template('#= Descricao #'),
        dataTextField: "Codigo",
        dataValueField: "Id",
        dataSource: BuscarCategorias()
    });

    // código do participante
    CarregarCodigoParticipantes();

    // estados
    $('#iEstado').kendoMultiSelect({
        placeholder: "Selecione Estado(s)",
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: BuscarEstados()
    });

    // cfop
    $('#iCFOP').kendoMultiSelect({
        placeholder: "Selecione CFOPs..",
        itemTemplate: kendo.template('<span class="cfop-codigo">#= Codigo # -</span> #= Descricao #'),
        dataTextField: "Codigo",
        dataValueField: "Codigo",
        dataSource: BuscarCFOPs()
    });

    // cfop referenciado
    $('#iCFOPRef').kendoMultiSelect({
        placeholder: "Selecione CFOPs..",
        itemTemplate: kendo.template('<span class="cfop-codigo">#= Codigo # -</span> #= Descricao #'),
        dataTextField: "Codigo",
        dataValueField: "Codigo",
        dataSource: BuscarCFOPRefs()
    });

    // materiais
    CarregarMaterial();

    // grupo de mercadoria
    CarregarGrupoMercadoria();

    //lei pis
    $('#iLeiPis').kendoMultiSelect({
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

    //lei cofins
    $('#iLeiCofins').kendoMultiSelect({
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

    // botão de esconder todos os filtros
    $('#btnHideAll').click(function () {
        $('#filterList').find('div .form-group').each(function () {
            $("#" + $(this).attr('id') + "").hide();
        });
    });

    // botão de pesquisa
    $('#btnSearch').click(function () {
        $('#gridResultadosPesquisa').data('kendoGrid').dataSource.read()
    });
}

/*CONSTRUCTOR - INITIALIZE MODAL*/
function InitializeFilterModalComponents() {
    // evento de abertura da modal
    $('#modalCamposFiltros').on('shown.bs.modal', function (e) {
        $('#ativados').find('option').remove();
        $('#desativados').find('option').remove();

        $('#filterList').find('div .form-group').each(function () {
            if ($(this).is(":visible")){
                $("#ativados").append("<option value='" + $(this).attr('id') + "'>" + $(this).attr('name') + "</option>");
            }                
            else {
                $("#desativados").append("<option value='" + $(this).attr('id') + "'>" + $(this).attr('name') + "</option>");
            }                
        });
    })

    // adicionar item desativado na lista de ativados
    $('#btnAdd').click(function () {
        var selecionados = $('#desativados').val();
        if (selecionados !== null) {
            for (var i = 0; i < selecionados.length; i++) {
                console.debug(selecionados[i]);
                $("#desativados option[value='" + selecionados[i] + "']").remove().appendTo('#ativados');
            }
        }
    });

    // adicionar item ativo na lista de desativados
    $('#btnRemove').click(function () {
        var selecionados = $('#ativados').val();
        if (selecionados !== null) {
            for (var i = 0; i < selecionados.length; i++) {
                console.debug(selecionados[i]);
                $("#ativados option[value='" + selecionados[i] + "']").remove().appendTo('#desativados');
            }
        }
    });

    // mover todos os itens para ativados
    $('#btnAddAll').click(function () {
        $('#desativados option').each(function () {
            console.debug($(this).val());
            $("#desativados option[value='" + $(this).val() + "']").remove().appendTo('#ativados');
        });
    });

    // remover todos os itens ativados
    $('#btnRemoveAll').click(function () {
        $('#ativados option').each(function () {
            console.debug($(this).val());
            $("#ativados option[value='" + $(this).val() + "']").remove().appendTo('#desativados');
        });
    });

    // aplicar as alterações de filtros selecionadas
    $('#btnAplicarFiltros').click(function () {
        var ativos = [];
        $('#ativados option').each(function () {
            $("#" + $(this).val() + "").show();
            ativos.push($(this).val());
        });
        sessionStorage.setItem('EditMultipleActive', JSON.stringify(ativos));
        $('#desativados option').each(function () {
            $("#" + $(this).val() + "").hide();
        });
    });

    // inicializer construtor de filtros
    PrepararConstrutor();
}

/*CONSTRUCTOR - INITIALIZE GRID*/
function InitializeGridComponents() {
    $('#btnReplicar').click(function () {
        if ($('#grid').data('kendoGrid').dataSource.hasChanges()) {
            console.debug('Modificado');
            var idx = 0;
            $('#grid').find('td').each(function () {                
                if ($(this).hasClass('k-dirty-cell')) {
                    var dataOrig = $('#grid').data('kendoGrid').dataSource.data();
                    var columnsOrig = $('#grid').data('kendoGrid').options.columns;
                    //var columns = $('#gridResultadosPesquisa').data('kendoGrid').options.columns;
                    var data = $('#gridResultadosPesquisa').data('kendoGrid').dataSource.data();
                    for (i = 0; i < data.length ; i++) {
                        var campo = dataOrig[0].get(columnsOrig[idx].field);
                        if (columnsOrig[idx].field == 'LeiCofins')
                            campo = 'C' + campo;

                        if (columnsOrig[idx].field == 'LeiTribPIS')
                            campo = 'P' + campo;

                        var row = data.at(i);
                        // marcar o primeiro registro como alterado
                        if (i == 0) {
                            row.dirty = true;
                        }                            
                        row[columnsOrig[idx].field] = campo;
                    }              
                }                    
                idx = idx + 1;
            });
            $('#gridResultadosPesquisa').data('kendoGrid').refresh();
        }        
    });

    $('#btnExportar').click(function () {
        ExportarExcel();
    });    
}

/*INITIALIZE FILTER - ACTIVATE*/
function AtivarFiltros() {
    var ativos = JSON.parse(sessionStorage.getItem('EditMultipleActive'));;
    $.each(ativos, function (index, value) {
        $("#" + value + "").show();
    });
}

/*INITIALIZE FILTER - PERIODO*/
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
        ShowModalAlert("Nenhum período configurado.");
        return;
    }

    $('#iPeriodo').kendoDropDownList({
        dataTextField: "Periodo",
        dataValueField: "Id",
        dataSource: dsPeriodo,
        optionLabel: "Período",
        select: function (e) {
            var dataItem = this.dataItem(e.item);
            $('#iPeriodo').val(dataItem.Periodo);

            if (!dataItem.Id)
                idControle = 0
            else
                idControle = dataItem.Id;

            //VerificarStatusPeriodo(idControle);

            //if (periodoEditavel == true)
            //    $("#lbPeriodoFechado").text('');
            //else
            //    $("#lbPeriodoFechado").text('PERÍODO FECHADO');

            //$('#idValidacao').val(0);

            // atualizar participantes
            $('#iCodParticipante').data('kendoMultiSelect').dataSource.read();
            // atualizar materiais
            $('#iMaterial').data('kendoMultiSelect').dataSource.read();
            // atualizar itens do grupo de materiais
            $('#iGrupoMercadoria').data('kendoMultiSelect').dataSource.read();            
            // atualizar csts
            $('#iLeiCofins').data('kendoMultiSelect').dataSource.read();
            $('#iLeiPis').data('kendoMultiSelect').dataSource.read();
        }
    });
}

/*INITIALIZE FILTER - PARTICIPANTE*/
function CarregarCodigoParticipantes() {
    $('#iCodParticipante').kendoMultiSelect({
        placeholder: "Participante(s) [selecione o período para adicionar as opções]",
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

/*INITIALIZE FILTER - GRUPO DE MERCADORIA*/
function CarregarGrupoMercadoria() {
    $('#iGrupoMercadoria').kendoMultiSelect({
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

/*INITIALIZE FILTER - MATERIAL*/
function CarregarMaterial() {
    $('#iMaterial').kendoMultiSelect({
        placeholder: "Materiai(s) [selecione o período para adicionar as opções]",
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

/*INITIALIZE FILTER - COMPARADOR DE GRUPOS*/
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

/*ACTION - VALORES FILTROS*/
function CarregarValoresFiltros(campo) {
    $('#fieldEdit').val(campo);
    $('#modalContrutorFiltro').modal({ backdrop: 'static', keyboard: false });
}

/*ACTION - VALORES FILTROS*/
function AplicarFiltroValores() {
    var campo = $('#fieldEdit').val();
    $('#' + campo).val(RecuperarValorCarregar());
    $('#modalContrutorFiltro').modal('hide');
}

/* ACTION - APLICAR FILTROS*/
function RecuperarValorCarregar() {
    var filtro = '';
    var comparacao1 = $('#ddlComparacao').data('kendoDropDownList').value().toString();
    var valor1 = $('#tbValor').val();
    var combinacao = $('#selectCombinacao').bootstrapSwitch('state') == true ? "E" : "OU";
    var comparacao2 = $('#ddlComparacao2').data('kendoDropDownList').value().toString();
    var valor2 = $('#tbValor2').val();

    if (comparacao1 == '' && valor1 != '') {
        ShowModalAlert('Favor selecionar a primeira comparação.');
        return;
    }

    if (comparacao2 == '' && valor2 != '') {
        ShowModalAlert('Favor selecionar a segunda comparação.');
        return;
    }

    if (comparacao1 != '')
        filtro = comparacao1 + ' ' + valor1;

    if (comparacao2 != '') {
        filtro += ' ' + combinacao + ' ' + comparacao2 + ' ' + valor2;
    }

    return filtro;
}

/* ACTION - EXPORTAR EXCEL */
function ExportarExcel() {
    ShowModalSucess('Exportação solicitada com sucesso. Em alguns instantes o download da planilha será executado.');
    var pattern = /(\d{2})\/(\d{2})\/(\d{4})/;

    var tna = $('#iPeriodo').data('kendoDropDownList').value().toString();
    if (tna === '')
        tna = 0;

    $.ajax({
        url: "/DocumentosFiscais/ExcelExport",
        type: "POST",
        async: true,
        data: JSON.stringify({
            idControle: tna,
            movimento: $('#iMovimento').data('kendoDropDownList').value().toString(),
            docnum: $('#iDocNum').val(),
            numeros: $('#iNumero').val(),
            series: $('#iSerie').val(),
            categoria: $('#iCategoria').data('kendoMultiSelect').value().toString(),
            dtEmiIni: ($('#iDtEmiDe').val()).replace(pattern, '$3-$2-$1'),
            dtEmiFim: ($('#iDtEmiAte').val()).replace(pattern, '$3-$2-$1'),
            dtLancIni: ($('#iDtLancDe').val()).replace(pattern, '$3-$2-$1'),
            dtLancFim: ($('#iDtLancAte').val()).replace(pattern, '$3-$2-$1'),
            cnpjs: $('#iCnpj').val(),
            codigoclientefornecedor: $('#iCodParticipante').data('kendoMultiSelect').value().toString(),
            razaosocial: $('#iRazaoSocial').val(),
            estados: $('#iEstado').data('kendoMultiSelect').value().toString(),
            cfops: $('#iCFOP').data('kendoMultiSelect').value().toString(),
            cfopsRef: $('#iCFOPRef').data('kendoMultiSelect').value().toString(),
            material: $('#iMaterial').data('kendoMultiSelect').value().toString(),
            descmaterial: $('#iDescMaterial').val(),
            grupomercadoria: $('#iGrupoMercadoria').data('kendoMultiSelect').value().toString(),
            centros: $('#iCentro').val(),
            docfiscalreferenciado: $('#iDocRef').val(),
            leicofins: $('#iLeiCofins').data('kendoMultiSelect').value().toString(),
            basecofins: $('#iBaseCofins').val().replace(/\./g, ''),
            aliqcofins: $('#iAliqCofins').val().replace(/\./g, ''),
            valorcofins: $('#iValorCofins').val().replace(/\./g, ''),
            leipis: $('#iLeiPis').data('kendoMultiSelect').value().toString(),
            basepis: $('#iBasePis').val().replace(/\./g, ''),
            aliqpis: $('#iAliqPis').val().replace(/\./g, ''),
            valorpis: $('#iValorPis').val().replace(/\./g, ''),
            quantidade: $('#iQtde').val().replace(/\./g, ''),
            totalItem: $('#iTotalItem').val().replace(/\./g, ''),
            codiva: $('#iCodIVA').val()
        }),
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Sucesso)
                window.open(result.Link, '_blank');
            else
                SShowModalAlertresult.Msg);
        }
    });
}

/*GRID - TEMPLATE DE EDIÇÃO DE MÚLTIPLAS NOTAS*/
function CarregarGridEditavelDocumentosFiscais() {
    $("#grid").kendoGrid({
        toolbar: [{ template: '<a id="btnReplicar" class="k-button k-grid-custom-command" "><span class="k-icon k-update"></span> Replicar </a>' },
        "cancel"],
        dataSource: {            
            schema: {
                data: function (result) {
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
                        DocNum: { validation: { required: false }, editable: false },
                        Movimento: { validation: { required: false } },
                        Item: { format: "{0:n0}", validation: { required: false } },
                        Numero: { format: "{0:n0}", validation: { required: false } },
                        Serie: { validation: { required: false }, nullable: true },
                        IdCategoriaNF: { type: "number", validation: { required: false } },
                        DEmi: { type: "date", validation: { required: false } },
                        DLcto: { type: "date", validation: { required: false } },
                        RazaoSocial: { validation: { required: false } },
                        Cnpj: { validation: { required: false } },
                        CodigoParticipante: { validation: { required: false } },
                        Estado: { validation: { required: false } },
                        Manualmente: { validation: { required: false }, editable: false },
                        StatusNota: { validation: { required: false }, editable: false },
                        CFOP: { type: "text", validation: { required: false } },
                        CFOPRef: { type: "text", validation: { required: false } },
                        Material: { validation: { required: false } },
                        DescricaoMaterial: { validation: { min: 0, required: false } },
                        GrupoMercadoria: { validation: { required: false } },
                        Centro: { validation: { required: false } },
                        DocRef: { validation: { required: false } },
                        LeiCofins: { validation: { required: false } },
                        BaseCalculoCOFINS: { type: "number", validation: { required: false } },
                        AliqCOFINS: { type: "number", validation: { required: false } },
                        ValorCOFINS: { type: "number", validation: { required: false } },
                        LeiTribPIS: { validation: { required: false } },
                        BaseCalculoPIS: { type: "number", validation: { required: false } },
                        AliqPIS: { type: "number", validation: { required: false } },
                        ValorPIS: { type: "number", validation: { required: false } },
                        PedCompra: { validation: { required: false } },
                        Quantidade: { type: "number", validation: { required: false } },
                        TotalItem: { type: "number", validation: { required: false } },
                        CodigoIVA: { validation: { required: false }, editable: false },
                        NFeRefDocNum: { validation: { required: false } },
                        NFeRefNfeNum: { validation: { required: false } },
                        NFeRefSerie: { validation: { required: false } },
                        NFeRefNCM: { validation: { required: false } },
                        StatusNFe: { validation: { required: false } },
                        Protocolo: { validation: { required: false } },
                        NumeroAleatorio: { validation: { required: false }, editable: false },
                        DigVal: { validation: { required: false }, editable: false },
                        StComunSis: { validation: { required: false }, editable: false },
                        NumLog: { validation: { required: false }, editable: false },
                        AuthDate: { type: "date", validation: { required: false } },
                        DataCriacao: { type: "date", validation: { required: false } },
                        Usuario: { validation: { required: false }, editable: false },
                        DataEntrada: { type: "date", validation: { required: false } },
                        DataAtualizacao: { type: "date", validation: { required: false } }
                    }
                }
            },
            transport: {
                read: {
                    url: "/DocumentosFiscais/RecuperarDocumentoFake",
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
                parameterMap: function (data, type) {
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
            pageSize: 1,
            serverPaging: true,
            serverSorting: true,
            serverFiltering: false,
        },
        scrollable: true,
        resizable: false,
        reorderable: true,
        height: '150px',
        columns: [
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
                    { field: "CFOPRef", sortable: false, title: "CFOP Ref.", width: "200px", editor: MostrarOpcoesCfops },
                    { field: "Material", sortable: false, title: "Material", width: "150px" },
                    { field: "DescricaoMaterial", sortable: false, title: "Texto Breve de Material", width: "300px" },
                    { field: "GrupoMercadoria", sortable: false, title: "Grupo de Mercadoria", width: "180px" },
                    { field: "Centro", sortable: false, title: "Centro", width: "80px" },
                    { field: "DocRef", sortable: false, title: "DOC Referência", width: "120px" },
                    { field: "LeiCofins", sortable: false, title: "Lei COFINS", width: "120px" },
                    { field: "BaseCalculoCOFINS", sortable: false, title: "Base COFINS", width: "120px", template: '#=BaseCalculoCOFINS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
                    { field: "AliqCOFINS", sortable: false, title: "Alíq COFINS", width: "120px", template: '#=kendo.format("{0:p}", AliqCOFINS / 100)#', attributes: { style: "text-align:right;" } },
                    { field: "ValorCOFINS", sortable: false, title: "Valor COFINS", width: "120px", template: '#=ValorCOFINS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
                    { field: "LeiTribPIS", sortable: false, title: "Lei PIS", width: "120px" },
                    { field: "BaseCalculoPIS", sortable: false, title: "Base PIS", width: "120px", template: '#=BaseCalculoPIS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
                    { field: "AliqPIS", sortable: false, title: "Alíq PIS", width: "120px", template: '#=kendo.format("{0:p}", AliqPIS / 100)#', attributes: { style: "text-align:right;" } },
                    { field: "ValorPIS", sortable: false, title: "Valor PIS", width: "120px", template: '#=ValorPIS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
                    { field: "PedCompra", sortable: false, title: "Pedido de Compra", width: "150px" },
                    { field: "Quantidade", sortable: false, title: "Quantidade", width: "120px", template: '#=Quantidade.FormatarMoeda(2, undefined, ".", ",")#', attributes: { style: "text-align:right;" } },
                    { field: "TotalItem", sortable: false, title: "Total do Item", width: "120px", template: '#=TotalItem.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
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
        ],
        editable: true
    });
}

/*GRID - RESULTADOS DA PESQUISA*/
function CarregarGridDocumentosFiscais(idControle) {
    $("#gridResultadosPesquisa").kendoGrid({
        toolbar: ["save", "cancel", { template: '<a id="btnExportar" class="k-button k-grid-custom-command" "><span class="k-icon k-i-excel"></span> Exportar Excel </a>' }],
        dataSource: {
            schema: {
                data: function (result) {
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
                        DocNum: { validation: { required: false }, editable: false },
                        Movimento: { validation: { required: false } },
                        Item: { format: "{0:n0}", validation: { required: false } },
                        Numero: { format: "{0:n0}", validation: { required: false } },
                        Serie: { validation: { required: false }, nullable: true },
                        IdCategoriaNF: { type: "number", validation: { required: false } },
                        DEmi: { type: "date", validation: { required: false } },
                        DLcto: { type: "date", validation: { required: false } },
                        RazaoSocial: { validation: { required: false } },
                        Cnpj: { validation: { required: false } },
                        CodigoParticipante: { validation: { required: false } },
                        Estado: { validation: { required: false } },
                        Manualmente: { validation: { required: false }, editable: false },
                        StatusNota: { validation: { required: false }, editable: false },
                        CFOP: { type: "text", validation: { required: false } },
                        CFOPRef: { type: "text", validation: { required: false } },
                        Material: { validation: { required: false } },
                        DescricaoMaterial: { validation: { min: 0, required: false } },
                        GrupoMercadoria: { validation: { required: false } },
                        Centro: { validation: { required: false } },
                        DocRef: { validation: { required: false } },
                        LeiCofins: { validation: { required: false } },
                        BaseCalculoCOFINS: { type: "number", validation: { required: false } },
                        AliqCOFINS: { type: "number", validation: { required: false } },
                        ValorCOFINS: { type: "number", validation: { required: false } },
                        LeiTribPIS: { validation: { required: false } },
                        BaseCalculoPIS: { type: "number", validation: { required: false } },
                        AliqPIS: { type: "number", validation: { required: false } },
                        ValorPIS: { type: "number", validation: { required: false } },
                        PedCompra: { validation: { required: false } },
                        Quantidade: { type: "number", validation: { required: false } },
                        TotalItem: { type: "number", validation: { required: false } },
                        CodigoIVA: { validation: { required: false }, editable: false },
                        NFeRefDocNum: { validation: { required: false }, editable: false },
                        NFeRefNfeNum: { validation: { required: false }, editable: false },
                        NFeRefSerie: { validation: { required: false }, editable: false },
                        NFeRefNCM: { validation: { required: false }, editable: false },
                        StatusNFe: { validation: { required: false }, editable: false },
                        Protocolo: { validation: { required: false }, editable: false },
                        NumeroAleatorio: { validation: { required: false }, editable: false },
                        DigVal: { validation: { required: false }, editable: false },
                        StComunSis: { validation: { required: false }, editable: false },
                        NumLog: { validation: { required: false }, editable: false },
                        AuthDate: { type: "date", validation: { required: false } },
                        DataCriacao: { type: "date", validation: { required: false } },
                        Usuario: { validation: { required: false }, editable: false },
                        DataEntrada: { type: "date", validation: { required: false } },
                        DataAtualizacao: { type: "date", validation: { required: false } }
                    }
                }
            },
            transport: {
                read: {
                    url: "/DocumentosFiscais/PesquisarDocumentosEdicaoMultipla",
                    dataType: "json",
                    type: "GET",
                    async: true,//não aguardamos a busca
                    cache: false
                },
                update: {
                    url: "/DocumentosFiscais/AtualizarMultiplosDocumentos",
                    dataType: "json",
                    type: "POST",
                    async: false,//aguardamos a atualização
                    contentType: "application/json",//necessário para stringfy
                    complete: function (e) {
                        // pesquisar novamente os itens do filtro
                        $("#gridResultadosPesquisa").data("kendoGrid").dataSource.read();
                        // atualizar filtro de cofins
                        $('#iLeiCofins').data('kendoMultiSelect').dataSource.read();
                        // atualizar filtro de pis
                        $('#iLeiPis').data('kendoMultiSelect').dataSource.read();
                        // atualizar campos da grid de edição para remover os itens modificados
                        $('#grid').data("kendoGrid").cancelChanges();
                        // mostrar mensagem de finalização
                        ShowModalSucess('Atualizações Salvas.');
                        $('#fieldEdit').val(0);
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

                        var tna = $('#iPeriodo').data('kendoDropDownList').value().toString();
                        if(tna === '')
                            tna = 0;


                        return {
                            idControle: tna,
                            pagina: data.page,
                            movimento: $('#iMovimento').data('kendoDropDownList').value().toString(),
                            docnum: $('#iDocNum').val(),
                            numeros: $('#iNumero').val(),
                            series: $('#iSerie').val(),
                            categoria: $('#iCategoria').data('kendoMultiSelect').value().toString(),
                            dtEmiIni: ($('#iDtEmiDe').val()).replace(pattern, '$3-$2-$1'),
                            dtEmiFim: ($('#iDtEmiAte').val()).replace(pattern, '$3-$2-$1'),
                            dtLancIni: ($('#iDtLancDe').val()).replace(pattern, '$3-$2-$1'),
                            dtLancFim: ($('#iDtLancAte').val()).replace(pattern, '$3-$2-$1'),
                            cnpjs: $('#iCnpj').val(),
                            codigoclientefornecedor: $('#iCodParticipante').data('kendoMultiSelect').value().toString(),
                            razaosocial: $('#iRazaoSocial').val(),
                            estados: $('#iEstado').data('kendoMultiSelect').value().toString(),
                            cfops: $('#iCFOP').data('kendoMultiSelect').value().toString(),
                            cfopsRef: $('#iCFOPRef').data('kendoMultiSelect').value().toString(),
                            material: $('#iMaterial').data('kendoMultiSelect').value().toString(),
                            descmaterial: $('#iDescMaterial').val(),
                            grupomercadoria: $('#iGrupoMercadoria').data('kendoMultiSelect').value().toString(),
                            centros: $('#iCentro').val(),
                            docfiscalreferenciado: $('#iDocRef').val(),
                            leicofins: $('#iLeiCofins').data('kendoMultiSelect').value().toString(),
                            basecofins: $('#iBaseCofins').val().replace(/\./g, ''),
                            aliqcofins: $('#iAliqCofins').val().replace(/\./g, ''),
                            valorcofins: $('#iValorCofins').val().replace(/\./g, ''),
                            leipis: $('#iLeiPis').data('kendoMultiSelect').value().toString(),
                            basepis: $('#iBasePis').val().replace(/\./g, ''),
                            aliqpis: $('#iAliqPis').val().replace(/\./g, ''),
                            valorpis: $('#iValorPis').val().replace(/\./g, ''),
                            quantidade: $('#iQtde').val().replace(/\./g, ''),
                            totalItem: $('#iTotalItem').val().replace(/\./g, ''),
                            codiva: $('#iCodIVA').val(),                            
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

                        var pattern = /(\d{2})\/(\d{2})\/(\d{4})/;
                        var ordernar = '';
                        var ordernarDir = '';

                        if (typeof data.sort !== "undefined" && data.sort !== null) {
                            ordernar = data.sort[0]['field'];
                            ordernarDir = data.sort[0]['dir'];
                        }

                        var tna = $('#iPeriodo').data('kendoDropDownList').value().toString();
                        if (tna === '')
                            tna = 0;

                        return JSON.stringify({
                            idControle: tna,
                            pagina: data.page,
                            movimento: $('#iMovimento').data('kendoDropDownList').value().toString(),
                            docnum: $('#iDocNum').val(),
                            numeros: $('#iNumero').val(),
                            series: $('#iSerie').val(),
                            categoria: $('#iCategoria').data('kendoMultiSelect').value().toString(),
                            dtEmiIni: ($('#iDtEmiDe').val()).replace(pattern, '$3-$2-$1'),
                            dtEmiFim: ($('#iDtEmiAte').val()).replace(pattern, '$3-$2-$1'),
                            dtLancIni: ($('#iDtLancDe').val()).replace(pattern, '$3-$2-$1'),
                            dtLancFim: ($('#iDtLancAte').val()).replace(pattern, '$3-$2-$1'),
                            cnpjs: $('#iCnpj').val(),
                            codigoclientefornecedor: $('#iCodParticipante').data('kendoMultiSelect').value().toString(),
                            razaosocial: $('#iRazaoSocial').val(),
                            estados: $('#iEstado').data('kendoMultiSelect').value().toString(),
                            cfops: $('#iCFOP').data('kendoMultiSelect').value().toString(),
                            cfopsRef: $('#iCFOPRef').data('kendoMultiSelect').value().toString(),
                            material: $('#iMaterial').data('kendoMultiSelect').value().toString(),
                            descmaterial: $('#iDescMaterial').val(),
                            grupomercadoria: $('#iGrupoMercadoria').data('kendoMultiSelect').value().toString(),
                            centros: $('#iCentro').val(),
                            docfiscalreferenciado: $('#iDocRef').val(),
                            leicofins: $('#iLeiCofins').data('kendoMultiSelect').value().toString(),
                            basecofins: $('#iBaseCofins').val().replace(/\./g, ''),
                            aliqcofins: $('#iAliqCofins').val().replace(/\./g, ''),
                            valorcofins: $('#iValorCofins').val().replace(/\./g, ''),
                            leipis: $('#iLeiPis').data('kendoMultiSelect').value().toString(),
                            basepis: $('#iBasePis').val().replace(/\./g, ''),
                            aliqpis: $('#iAliqPis').val().replace(/\./g, ''),
                            valorpis: $('#iValorPis').val().replace(/\./g, ''),
                            quantidade: $('#iQtde').val().replace(/\./g, ''),
                            totalItem: $('#iTotalItem').val().replace(/\./g, ''),
                            codiva: $('#iCodIVA').val(),
                            nferefdocnum: $('#iNFeRefDocNum').val(),
                            nferefnfenum: $('#iNFeRefNfeNum').val(),
                            nferefserie: $('#iNFeRefSerie').val(),
                            nferefncm: $('#iNFeRefNCM').val(),
                            entidade: data
                        });
                    }
                }
            },
            pageSize: 25,
            serverPaging: true,
            serverSorting: true,
            serverFiltering: false,
        },
        scrollable: true,
        resizable: false,
        reorderable: true,
        height: '500px',
        saveChanges: function(e){
            if (!confirm("Você tem certeza que deseja salvar as alterações? (Ao final do processo uma mensagem de confirmação será apresentada.)")) {
                e.preventDefault();
            }
        },
        pageable: {
            pageSizes: [10, 25, 50, 100, 500, 1000, 5000, 10000]
        },
        columns: [
            { field: "DocNum", title: "DOCNUM", width: "100px", locked: true },
            { field: "Movimento", sortable: false, title: "Movimento", width: "90px", template: "#= MostrarMovimento(Movimento) #" },
            { field: "NFe", sortable: false, title: "NFe", width: "80px" },
            { field: "Item", sortable: false, title: "Nº Item", width: "80px" },
            { field: "Numero", title: "Número", width: "100px" },
            { field: "Serie", sortable: false, title: "Série", width: "80px" },
            { field: "IdCategoriaNF", sortable: false, title: "Categoria", width: "200px", template: "#= RecuperarCategoriaExibicaoGrid(IdCategoriaNF) #" },
            { field: "DEmi", title: "Emissão", template: "#= kendo.toString(kendo.parseDate(DEmi, 'yyyy-MM-dd'), 'dd/MM/yyyy') #", width: "100px" },
            { field: "DLcto", title: "Lançamento", template: "#= kendo.toString(kendo.parseDate(DLcto, 'yyyy-MM-dd'), 'dd/MM/yyyy') #", width: "120px" },
            { field: "RazaoSocial", sortable: false, title: "Razão Social", width: "300px" },
            { field: "Cnpj", sortable: false, title: "Cnpj", width: "120px" },
            { field: "CodigoParticipante", sortable: false, title: "Código Participante", width: "150px" },
            { field: "Estado", sortable: false, title: "Região", width: "80px" },
            { field: "Manualmente", sortable: false, title: "Manualmente", width: "100px" },
            { field: "StatusNota", sortable: false, title: "Status", width: "80px" },
            { field: "CFOP", title: "CFOP", width: "200px", editor: MostrarOpcoesCfops },
            { field: "CFOPRef", sortable: false, title: "CFOP Ref.", width: "200px", editor: MostrarOpcoesCfops },
            { field: "Material", sortable: false, title: "Material", width: "150px" },
            { field: "DescricaoMaterial", sortable: false, title: "Texto Breve de Material", width: "300px" },
            { field: "GrupoMercadoria", sortable: false, title: "Grupo de Mercadoria", width: "180px" },
            { field: "Centro", sortable: false, title: "Centro", width: "80px" },
            { field: "DocRef", sortable: false, title: "DOC Referência", width: "120px" },
            { field: "LeiCofins", sortable: false, title: "Lei COFINS", width: "120px" },
            { field: "BaseCalculoCOFINS", sortable: false, title: "Base COFINS", width: "120px", template: '#=BaseCalculoCOFINS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "AliqCOFINS", sortable: false, title: "Alíq COFINS", width: "120px", template: '#=kendo.format("{0:p}", AliqCOFINS / 100)#', attributes: { style: "text-align:right;" } },
            { field: "ValorCOFINS", sortable: false, title: "Valor COFINS", width: "120px", template: '#=ValorCOFINS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "LeiTribPIS", sortable: false, title: "Lei PIS", width: "120px" },
            { field: "BaseCalculoPIS", sortable: false, title: "Base PIS", width: "120px", template: '#=BaseCalculoPIS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "AliqPIS", sortable: false, title: "Alíq PIS", width: "120px", template: '#=kendo.format("{0:p}", AliqPIS / 100)#', attributes: { style: "text-align:right;" } },
            { field: "ValorPIS", sortable: false, title: "Valor PIS", width: "120px", template: '#=ValorPIS.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
            { field: "PedCompra", sortable: false, title: "Pedido de Compra", width: "150px" },
            { field: "Quantidade", sortable: false, title: "Quantidade", width: "120px", template: '#=Quantidade.FormatarMoeda(2, undefined, ".", ",")#', attributes: { style: "text-align:right;" } },
            { field: "TotalItem", sortable: false, title: "Total do Item", width: "120px", template: '#=TotalItem.FormatarMoeda(2, "", ".", ",") #', attributes: { style: "text-align:right;" } },
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
        ],
        editable: true
    });
}

/*DATASOURCE - ENTRADAS*/
function BuscarMovimento() {
    var dsMovimentos = [
        { Text: 'Entrada', Value: 'E' },
        { Text: 'Saída', Value: 'S' }
    ];
    return dsMovimentos;
}

/*DATASOURCE - ENTRADAS*/
function BuscarEstados() {
    var dsEstados = [
        { Text: 'Acre', Value: 'AC' },
        { Text: 'Alagoas', Value: 'AL' },
        { Text: 'Amapá', Value: 'AP' },
        { Text: 'Amazonas', Value: 'AM' },
        { Text: 'Bahia', Value: 'BA' },
        { Text: 'Ceará', Value: 'CE' },
        { Text: 'Distrito Federal', Value: 'DF' },
        { Text: 'Espírito Santo', Value: 'ES' },
        { Text: 'Goiás', Value: 'GO' },
        { Text: 'Maranhão', Value: 'MA' },
        { Text: 'Mato Grosso', Value: 'MT' },
        { Text: 'Mato Grosso do Sul', Value: 'MS' },
        { Text: 'Minas Gerais', Value: 'MG' },
        { Text: 'Pará', Value: 'PA' },
        { Text: 'Paraíba', Value: 'PB' },
        { Text: 'Paraná', Value: 'PR' },
        { Text: 'Pernambuco', Value: 'PE' },
        { Text: 'Piauí', Value: 'PI' },
        { Text: 'Rio de Janeiro', Value: 'RJ' },
        { Text: 'Rio Grande do Norte', Value: 'RN' },
        { Text: 'Rio Grande do Sul', Value: 'RS' },
        { Text: 'Rondônia', Value: 'RO' },
        { Text: 'Roraima', Value: 'RR' },
        { Text: 'Santa Catarina', Value: 'SC' },
        { Text: 'São Paulo', Value: 'SP' },
        { Text: 'Sergipe', Value: 'SE' },
        { Text: 'Tocantins', Value: 'TO' }
    ];
    return dsEstados;
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
                if (result.Sucesso) {
                    localStorage.setItem('CategoriasStorage', JSON.stringify(result.Data));
                    dsCategorias = result.Data;
                }
                else
                    ShShowModalAlertesult.Msg);
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
                    ShShowModalAlertesult.Msg);
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
                    ShShowModalAlertesult.Msg);
            }
        });
    }

    return dsCfops;
};

/*TEMPLATE DATABIND DE ENTRADA OU SAÍDA*/
function MostrarMovimento(movimento) {
    if (movimento !== null)
    {
        if (movimento.Value == 'E' || movimento == 'E') {
            return 'Entrada';
        }
        else if (movimento.Value == 'S' || movimento == 'S') {
            return 'Saída';
        }
    }
    else {
        return '';
    }
}

/*TEMPLATE DE EDIÇÃO DE ENTRADA OU SAÍDA*/
function MostrarOpcoesMovimento(container, options) {
    $('<input data-bind="value:' + options.field + '"/>')
    .appendTo(container)
    .kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        dataSource: BuscarMovimento()
    });
}

/*TEMPLATE DATABIND DE CATEGORIA*/
function RecuperarCategoriaExibicaoGrid(idCategoria) {
    var dsCategorias = BuscarCategorias();

    for (var i = 0, length = dsCategorias.length; i < length; i++) {
        if (dsCategorias[i].Id === idCategoria) {
            return dsCategorias[i].Descricao;
        }
    }

    return '';
}

/*TEMPLATE DE EDIÇÃO DE CATEGORIA*/
function RecuperarCategoriasEdicao(container, options) {
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