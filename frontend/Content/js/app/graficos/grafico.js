var tipoGrafico = '';
var tipoPeriodo = '';
var tipoRegra = '';
var url = '/Graficos/BuscarGraficosTipo';
var selectPeriodo;

function CarregarComponentes(tpGrafico, tpPeriodo) {    
    tipoGrafico = tpGrafico;
    tipoPeriodo = tpPeriodo;

    if (tipoGrafico == 'comparativo' && tipoPeriodo == 12) {
        $('#divSelectAno').hide();        
    }
    else {
        $('#divSelectAno').show();
        selectPeriodo = $('#selectPeriodo').kendoDropDownList().data("kendoDropDownList");

        $("#selectPeriodo").data("kendoDropDownList").bind("change", function (e) {
            $('#flot-type-chart').hide();
            $('#divTituloGrafico').hide();
        });
    }

    CarregarMatrizes();

    var selectMatriz = $("#selectMatriz").data("kendoDropDownList");
    selectMatriz.select(0);
    $('#tbRazaoSocial').val('');
    idMatriz = 0;

    $('#btnExibirGrafico').click(function () {
        CarregarGrafico();
    });

    switch (tipoGrafico) {
        case 'credito':
            tipoRegra = '1';
            break;
        case 'receita':
            tipoRegra = '2';
            break;
        case 'manual':
            tipoRegra = '3';
            break;
        case 'comparativo':
            tipoRegra = '4';
            url = '/Graficos/BuscarGraficosComparativo';
            break;
    } 
}

function CarregarMatrizes() {
    $('#selectMatriz').kendoDropDownList({
        dataTextField: "CodigoMatriz",
        dataValueField: "Id",
        dataSource: LoadDsMatrizes(),
        optionLabel: "Escolha ...",
        select: function (e) {
            var dataItem = this.dataItem(e.item);
            $('#tbRazaoSocial').val(dataItem.RazaoSocial);
            idMatriz = dataItem.Id;

            $('#lbTitulo').text(RetornarTituloPagina());

            if (selectPeriodo) {
                selectPeriodo.setDataSource([]);
                selectPeriodo.text('');

                selectPeriodo.setDataSource({
                    schema: {
                        data: function (result) {
                            return result.Data;
                        }
                    },
                    transport: {
                        read: {
                            url: "/Graficos/BuscarPeriodos",
                            dataType: "json",
                            type: "GET",
                            async: false,
                            cache: true
                        },
                        parameterMap: function (data, type) {
                            if (type === "read") {
                                return {
                                    idEmpresa: idMatriz,
                                    tipoPeriodo: tipoPeriodo,
                                    tipoRegra: tipoRegra
                                }
                            }
                        }
                    }
                });

                if ($("#selectPeriodo").data("kendoDropDownList").dataSource.data().length > 0)
                    $("#selectPeriodo").data("kendoDropDownList").select(0);

                $('#flot-type-chart').hide();
                $('#divTituloGrafico').hide();
            }
        }
    });

    $("#selectMatriz").data("kendoDropDownList").bind("change", function (e) {
        $('#flot-type-chart').hide();
        $('#divTituloGrafico').hide();
    });
}

function CarregarGrafico() {
    var dsGrafico = [];
    var msgErro = '';        
    var periodo = '0'; 

    if (selectPeriodo)
        periodo = selectPeriodo.text();

    msgErro = ValidarFiltro();

    if (msgErro == '') {
        $("#loading-page").show();
       
        $.ajax({
            url: url,
            data: JSON.stringify({ idEmpresa: idMatriz, tipoPeriodo: tipoPeriodo, periodo: periodo, tipoRegra: tipoRegra }),
            type: "POST",
            async: false,
            contentType: 'application/json; charset=utf-8',
            cache: false,
            success: function (result) {
                if (result.Sucesso)
                    dsGrafico = result.Content;
                else
                    ShowModalAlerta("Houve um erro ao obter as informações para o gráfico.");
            }
        });

        $("#loading-page").hide();
    }
    else {
        ShowModalAlerta(msgErro);
        return;
    }

    if (dsGrafico.length > 0)
        RenderizarGrafico(dsGrafico);
}

function ValidarFiltro() {
    var msgErro = '';

    if (idMatriz == 0)
        msgErro = 'Favor selecionar a Matriz.';
  
    return msgErro;
}

function RenderizarGrafico(dsGrafico) {    
    var plot;    
    var dsManual = [];
    var dsSeries = [];
    var dsTicks = [];
    var rotateTicks = 135;
    var align = 'center';
    var tituloGrafico = '';
    var margemEsquerda = 0;
    var valoresZerados = true;
    var representacao = 'R$';
    var periodo = '\n';
    var ajusteAlinhamentoTitulo = 0;
    var alinhamentoBarrasMin = undefined;
    var alinhamentoBarrasMax = undefined;

    $('#flot-type-chart').show();
    $('#divTituloGrafico').show();

    switch (tipoGrafico) {        
        case 'manual':
            representacao = 'R$';

            $.each(dsGrafico, function (index, itemTipo) {
                dsManual.push([                    
                    itemTipo.Descricao,
                    itemTipo.Valor,
                    itemTipo.baseWaterFall
                ]);

                if (itemTipo.Valor > 0)
                    valoresZerados = false;
            });
         
            ajusteAlinhamentoTitulo = 40;
            alinhamentoBarrasMin = -0.5;
            alinhamentoBarrasMax = 6.5;

            break;
        case 'comparativo':            
            if (idMatriz == 1)
                representacao = 'R$ milhões';
            else
                representacao = 'R$';

            rotateTicks = 0;

            if (tipoPeriodo == '1') {
                alinhamentoBarrasMin = -0.5;
                alinhamentoBarrasMax = 11.5;
            }
            else if (tipoPeriodo == '3') {
                alinhamentoBarrasMin = -0.5;
                alinhamentoBarrasMax = 3.5;

            }
            else if (tipoPeriodo == '12') {
                alinhamentoBarrasMin = -0.5;
                alinhamentoBarrasMax = 2.5;
            }

            $.each(dsGrafico, function (index, itemTipo) {
                dsSeries.push([
                    index,
                    itemTipo.Liquido
                ]);

                dsTicks.push([
                    index,
                    itemTipo.Periodo
                ]);

                if (itemTipo.Liquido > 0)
                    valoresZerados = false;
            });

            break;
        default: // Crédito e Receita      
            representacao = '%';
            margemEsquerda = 120;

            $.each(dsGrafico, function (index, itemTipo) {                
                dsSeries.push([
                    index,
                    itemTipo.Valor,
                    itemTipo.porcDistribuicao
                ]);

                dsTicks.push([
                    index,
                    itemTipo.Descricao
                ]);

                if (itemTipo.Valor > 0)
                    valoresZerados = false;
            });

            ajusteAlinhamentoTitulo = 70;
            break;
    }

    if (tipoGrafico == "manual") {
        RenderizarGraficoWaterfall(
            dsManual,
            align,
            alinhamentoBarrasMin,
            alinhamentoBarrasMax,
            rotateTicks,
            valoresZerados);
    }
    else {
        RenderizarGraficoBarras(
            dsSeries,
            dsTicks,
            alinhamentoBarrasMin,
            alinhamentoBarrasMax,
            rotateTicks,
            margemEsquerda,
            valoresZerados);        
    }

    tituloGrafico = RetornarTituloGrafico(representacao);
    
    var c = document.getElementsByTagName("canvas")[0];
    var canvas = c.getContext("2d");
    canvas.font = "bold 18px segoeuil";
    canvas.textAlign = 'center';  

    var cx = (c.width / 2) + ajusteAlinhamentoTitulo;
    var txt = tituloGrafico;
    var x = 30;
    var y = 30;
    var lineheight = 30;
    var lines = txt.split('\n');

    for (var i = 0; i < lines.length; i++)
        canvas.fillText(lines[i], cx, y + (i * lineheight));  
}

function RenderizarGraficoWaterfall(dsManual, align, alinhamentoBarrasMin, alinhamentoBarrasMax, rotateTicks, valoresZerados) {
    var tmp, config
    var barraBase = false;
    var valorBarraBase = 0;
    var i = -1;

    tmp = $.plot.JUMlib.prepareData.createWaterfall(
        dsManual, { fixed: "#009900", positive: "#e0c82c", negative: "#8497B0" }
    );

    config = {
        series: {
            stack: 0,
            bars: {
                show: true,
                barWidth: 0.6,
                align: align,
                fill: 1
            },
            valueLabels: {
                show: true,
                showTextLabel: true,
                yoffset: -2,
                align: 'center',
                valign: 'above',
                reverseAlignBelowZero: true,
                font: "9pt 'Trebuchet MS'",
                fontcolor: 'black',
                labelFormatter: function (v) {
                    i++;

                    if (i == 1 && barraBase == false) {
                        barraBase = true;
                        i--;
                        valorBarraBase = parseFloat(dsManual[dsManual.length - 1][1]);
                        return parseFloat(dsManual[dsManual.length - 1][1]).FormatarMoeda(2, '', '.', ',', true);
                    }
                    else {
                        if (dsManual[i] != undefined && parseFloat(dsManual[i][1]) != valorBarraBase)
                            return parseFloat(dsManual[i][1]).FormatarMoeda(2, '', '.', ',', true);
                        else
                            return '';
                    }
                }
            }
        },
        xaxis: {
            showAsHtml: true,
            ticks: tmp.ticks,
            min: alinhamentoBarrasMin,
            max: alinhamentoBarrasMax,
            rotateTicks: rotateTicks
        },
        canvas: true,
        grid: {
            margin: {
                top: 100,
                left: 80
            }//, hoverable: true
        },
        yaxis: {
            autoscaleMargin: 0.05,
            min: 0,
            tickFormatter: function (v) {
                return FormatarValorGrafico(v, valoresZerados, tipoGrafico);
            }
        }
    };

    $.plot($("#flot-type-chart"), tmp.data, config);
}

function RenderizarGraficoBarras(dsSeries, dsTicks, alinhamentoBarrasMin, alinhamentoBarrasMax, rotateTicks, margemEsquerda, valoresZerados) {
    var plot;
    var series;

    series =
    {
        data: dsSeries,
        lines: {
            show: false
        },
        bars: {
            show: true,
            barWidth: 0.75,
            align: 'center'
        }
    }

    plot = $.plot("#flot-type-chart",
    [series],
    {
        xaxis: {
            ticks: dsTicks,
            min: alinhamentoBarrasMin,
            max: alinhamentoBarrasMax,
            rotateTicks: rotateTicks
        },
        canvas: true,
        grid: {
            margin: {
                top: 100,
                left: margemEsquerda
            }
            //,hoverable: true
        },
        yaxis: {
            allowDecimals: false,
            min: 0,
            autoscaleMargin: 0.1,
            tickFormatter: function (v) {
                return FormatarValorGrafico(v, valoresZerados, tipoGrafico);
            }
        }
    });

    var ctx = plot.getCanvas().getContext("2d");
    var data = plot.getData()[0].data;
    var xaxis = plot.getXAxes()[0];
    var yaxis = plot.getYAxes()[0];
    var offset = plot.getPlotOffset();

    ctx.font = "12px 'Segoe UI'";
    ctx.fillStyle = "black";

    for (var i = 0; i < data.length; i++) {
        var text = '';
        if (tipoGrafico == 'credito' || tipoGrafico == 'receita')
            text = data[i][2].FormatarMoeda(2, "", ".", ",");
        else if (tipoGrafico == 'comparativo' && idMatriz == 4)
            text = '-' + data[i][1].FormatarMoeda(2, "", ".", ",");
        else
            text = data[i][1].FormatarMoeda(2, "", ".", ",");

        var metrics = ctx.measureText(text);
        var xPos = (xaxis.p2c(data[i][0]) + offset.left) - metrics.width / 2;
        var yPos = yaxis.p2c(data[i][1]) + offset.top - 5;
        ctx.fillText(text, xPos, yPos);
    }
}

function TraduzirPeriodoMensal(periodo) {
    var mesNome = ''
    var mes = '';
    var ano = '';
    var meses = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];

    ano = periodo.substring(0, 4);
    mes = periodo.substring(4);
   
    mesNome = meses[mes - 1];

    return mesNome + '/' + ano;
}

function RetornarTrimestre(periodo) {
    var numeroTrimestre = '';
    var trimestre = '';

    numeroTrimestre = periodo.substring(0, 1);

    switch (numeroTrimestre) {
        case '1':
            trimestre = numeroTrimestre + 'st';
            break;
        case '2':
            trimestre = numeroTrimestre + 'nd';
            break;
        case '3':
            trimestre = numeroTrimestre + 'rd';
            break;
        case '4':
            trimestre = numeroTrimestre + 'th';
            break;
    }

    return trimestre;
}

function RetornarTituloGrafico(representacao) {
    var tituloGrafico = '';
    var periodo = '';

    if (tipoGrafico != "manual") {
        if (selectPeriodo)
            periodo = ' - Período: ' + selectPeriodo.text() + '\n';
        else
            periodo = '\n';

        tituloGrafico =
            $('#lbTitulo').text() + '\n' +
            $('#tbRazaoSocial').val() +
            periodo +
            '(em ' + representacao + ')';
    }
    else {
        switch (tipoPeriodo) {
            case '1':
                tituloGrafico =
                    'Federal Tax Credits – ' + TraduzirPeriodoMensal(selectPeriodo.text()) + ' Variation' + '\n' +
                    $('#tbRazaoSocial').val() + '\n' +
                    '(in R$ million)';
                break;
            case '3':
                tituloGrafico =
                    'Federal Tax Credits – ' + RetornarTrimestre(selectPeriodo.text()) + ' Quarter Variation' + '\n' +
                    $('#tbRazaoSocial').val() + '\n' +
                    '(in R$ million)';
                break;
            default:
                tituloGrafico =
                    'Federal Tax Credits - Annual Variation' + '\n' +
                    $('#tbRazaoSocial').val() + '\n' +
                    '(in R$ million)';
                break;
        }
    }
    
    return tituloGrafico;
}

function RetornarTituloPagina() {
    var tituloPagina = '';
    var periodoExtenso = '';

    periodoExtenso = RetornarPeriodoExtenso();

    if (tipoGrafico == 'comparativo' && idMatriz == 1) 
        tituloPagina = 'Gráfico Comparativo ' + periodoExtenso + ' de Crédito';
    else if (tipoGrafico == 'comparativo' && idMatriz == 4) 
        tituloPagina = 'Gráfico Comparativo ' + periodoExtenso + ' de Débito';
    else
        tituloPagina = $('#lbTitulo').text();

    return tituloPagina;
}

function RetornarPeriodoExtenso(){
    switch (tipoPeriodo) {
        case '1':
            return 'Mensal';
            break;
        case '3':
            return 'Trimestral';
            break;
        case '12':
            return 'Anual';
            break;
    }
}
