var objEmpresa;
var LinhaemBranco;

function LoadDsGrid(Empresa) {
    objEmpresa = Empresa;
    var idEmpresa = Empresa.idEmpresa;
    GridApuracoes(idEmpresa);  
}

function GridApuracoes(idEmpresa) {
    $("#grid").html("");
    $("#grid").kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: "/Relatorios/BuscarPeriodosAnuais",
                    dataType: "json",
                    type: "GET",
                    async: false,
                    cache: false
                },
                parameterMap: function (data, type) {
                    if (type == "read") {
                        return { idEmpresa: idEmpresa }
                    }
                }
            },
            pageSize: 10,
            sort: {
                field: "AnoMes",
                dir: "desc"
            },
            schema: {
                data: function (result) {
                    return result.Content;
                },
                total: function (result) {
                    return result.Total;
                },
                model: {
                    id: "Id",
                    fields: {
                        IdEmpresa: { validation: { required: true } },
                        Editavel: { validation: { required: true } },
                        AnoMes: { type: "text", validation: { required: false } },                        
                        StatusId: { validation: { required: false } },
                        Status: { validation: { required: false } }
                    }
                }
            }
        },
        scrollable: true,
        sortable: true,
        resizable: true,
        groupable: false,
        pageable: {
            pageSizes: [10, 25, 50]
        },
        columns: [
            { field: "Id", hidden: true },
            { field: "StatusId", hidden: true },
            { field: "IdEmpresa", hidden: true },
            { field: "Editavel", hidden: true },
            { field: "AnoMes", hidden: true },
            { field: "MensagemErro", hidden: true },
            { field: "DescricaoPeriodo", title: "Período" },
            { field: "Status", title: "Status", template: kendo.template($("#linkLogErroTemplate").html()) },            
            {
                title: "PIS",
                headerAttributes: { style: "text-align:center;" },
                template: kendo.template($("#botaoRelatorioAnualPisTemplate").html()),
                width: "75px",
                attributes: { style: "text-align:center;" },
                filterable: false
            },
            {
                title: "COFINS",
                headerAttributes: { style: "text-align:center;" },
                template: kendo.template($("#botaoRelatorioAnualCofinsTemplate").html()),
                width: "75px",
                attributes: { style: "text-align:center;" },
                filterable: false
            }
        ]
    });
}

function AnualDetails(e, tipoImposto) {    
    var dataItem = $("#grid").data("kendoGrid").dataItem(e.parentElement.parentElement);

    $.ajax({
        url: "/Relatorios/ObterRelatorioAnual",
        data: JSON.stringify({ IdEmpresa: dataItem.IdEmpresa, Periodos: dataItem.AnoMes, TipoImposto: tipoImposto }),
        type: 'POST',
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.Sucesso) 
                ShowAnualReport(result.Content, dataItem.AnoMes, tipoImposto);
            else 
                ShowModalAlerta(result.Msg);
        }
    });
}

function ShowAnualReport(data, Periodo, tipoImposto) {
    var repHtml, cabecHtml, credHtml, debHtml, saldoHtml;
    var Cabec = '';

    LinhaemBranco = "<tr style='line-height: 8px;'><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>";

    Cabec = "<body>";
    Cabec += "<div id='divGerencial' style='align:center;width:100%;'>";
    Cabec += "<table style='width:100%;'>";
    Cabec += "<tr>";
    Cabec += "<td style='text-align:left;font-size:10px;font-family:Tahoma;color:black;width:33%;'>" + data.Cabecalho.CodigoMatriz + " - " + data.Cabecalho.RazaoSocial + "</td>";
    Cabec += "<td style='text-align:center;width:34%;'><strong>APURAÇÃO ANUAL DE " + tipoImposto + "</strong></td>";
    Cabec += "<td style='width:33%;text-align:right;' rowspan=2><div id='imageTela' style='display:block;'><img src='../Content/images/layout/" + data.Cabecalho.Logotipo + "' align='right'></div></td>";
    Cabec += "</tr>";
    Cabec += "<tr>";
    Cabec += "<td style='text-align:left;font-size:10px;font-family:Tahoma;color:black;width:33%;'>" + data.Cabecalho.CNPJ + "</td>";
    Cabec += "<td style='text-align:center;width:34%;'><strong>Período: " + data.Cabecalho.PeriodoGerado + " - Emitido em: " + data.Cabecalho.DataGeracao + "</strong></td>";
    Cabec += "<td style='width:33%;text-align:right;'></td>"
    Cabec += "</tr>";
    Cabec += "</table>";
    Cabec += "<p>"        

    credHtml = Creditos(data.linhasCredito);
    debHtml = Debitos(data.linhasDebito);
    saldoHtml = Saldos(data.linhasSaldo);
    
    repHtml = "<!DOCTYPE html>";
    repHtml += '<html>';
    repHtml += "<head>";
    repHtml += "<style>";
    repHtml += ".Largura-Linha {width:100%;}"
    repHtml += ".Largura-Linha-Titulo {width:100%;}"
    repHtml += ".Largura-Linha-Valor {width:100%;}"
    repHtml += ".Credito-Totalizador {border-bottom:1px solid black;border-top:1px solid black;font-weight:bold;}";
    repHtml += ".Credito-Normal {font-weight: normal;}";
    repHtml += ".Text-Rel {font-family:Tahoma;font-size:8px;text-align:left;}";
    repHtml += ".Val-Rel {font-family:Tahoma;font-size:8px;text-align:right;}";
    repHtml += ".Ger-Lin {line-height:11px;border-bottom: 1px solid black;border-top:1px solid black;}";
    repHtml += ".Ger-Bold {font-weight:bold;}";
    repHtml += ".Ger-font {font-family:Tahoma;font-size:8px;color:black;}";
    repHtml += ".Ger-fontRed {font-family:Tahoma;font-size:8px;font-weight:bold;color:red;}";
    repHtml += ".SubTit-Rel {font-family:Tahoma;font-size:8px;background-color:black;color:white;font-weight:bold;text-align:right;}";
    repHtml += ".SubTitN-Rel {font-family:Tahoma;font-size:8px;background-color:white;color:black;font-weight:bold;text-align:right;}";
    repHtml += "</style>";
    repHtml += "</head>";

    repHtml += Cabec + credHtml + LinhaemBranco + debHtml + saldoHtml;

    repHtml += '</div></body></html>';

    $('#modalAnual').modal({ backdrop: 'static', keyboard: false });
    $('#modalAnual .modal-dialog .modal-body .body-excel').html(data.Cabecalho.IdEmpresa + ';' + Periodo + ';' + tipoImposto);
    $('#modalAnual .modal-dialog .modal-body .body-message').html('Relatório Gerencial Anual');
    $('#modalAnual .modal-dialog .modal-body .body-message').html(repHtml);
}

function Creditos(lstCreditos) {
    var mClass, varHtml, LinhaemBranco, TemLinha;

    LinhaemBranco = "<tr style='line-height: 8px;'><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>";

    varHtml = '';

    varHtml += "<div style='page-break-after: always'>";

    varHtml += "<table>";
    varHtml += "<tr style='font-family:Tahoma;font-size: 9px;color:black;font-weight:bold'>";
    varHtml += "<th width='13.94%' class='Text-Rel' style='border-bottom: 1px solid black;'>Descrição</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Janeiro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Fevereiro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Março</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Abril</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Maio</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Junho</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Julho</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Agosto</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Setembro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Outubro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Novembro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Dezembro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Acumulado</th>";
    varHtml += "</tr>";
   // varHtml += LinhaemBranco;

    varHtml += "<tr>";    
    varHtml += "<td class='SubTitN-Rel Ger-font' style='text-align:center; border-bottom: 1px solid black;'>Créditos</td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "</tr>";

   // varHtml += LinhaemBranco;    

   // TemLinha = 0;

    for (var i in lstCreditos) {
        if (lstCreditos[i].Borda == 1) {
            mClass = 'Credito-Totalizador';
            varHtml += '<tr class="Credito-Totalizador">';
        }
        else if (lstCreditos[i].Vermelho == 1) {
            mClass = 'Ger-fontRed';
            varHtml += '<tr>';
        }
        else if (lstCreditos[i].Bold == 1) {
            mClass = 'Ger-Bold';
            varHtml += '<tr>';
        }        
        else {
            mClass = 'Credito-Normal';
            varHtml += '<tr>';
        }
        
        //TemLinha = 0;

        varHtml += "<td class='" + mClass + " Ger-font Text-Rel'>" + IsNull(lstCreditos[i].Descricao,' ') + ' </td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes01.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes02.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes03.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes04.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes05.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes06.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes07.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes08.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes09.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes10.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes11.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].Mes12.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstCreditos[i].ValorAcumulado.formatMoney(2, ',', '.') + '</td>';

        if (lstCreditos[i].Espacada == 1) {
            mClass = 'Ger-Lin';
            varHtml += '</tr>' + LinhaemBranco;
            //TemLinha = 1;
        }
        else
            varHtml += '</tr>';
    }

    varHtml += '</table>';
    varHtml += '</div>';

    return varHtml;
}

function Debitos(lstDebitos) {
    var mClass, varHtml, LinhaemBranco, TemLinha;

    LinhaemBranco = "<tr style='line-height: 8px;'><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>";

    varHtml = '';

    varHtml += "<div style='page-break-after: always'>";

    varHtml += "<table>";
    varHtml += "<tr style='font-family:Tahoma;font-size: 9px;color:black;font-weight:bold'>";
    varHtml += "<th width='13.94%' class='Text-Rel' style='border-bottom: 1px solid black;'>Descrição</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Janeiro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Fevereiro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Março</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Abril</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Maio</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Junho</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Julho</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Agosto</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Setembro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Outubro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Novembro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Dezembro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Acumulado</th>";
    varHtml += "</tr>";
   // varHtml += LinhaemBranco;

    varHtml += "<tr>";
    varHtml += "<td class='SubTitN-Rel Ger-font' style='text-align:center; border-bottom: 1px solid black;'>Débitos</td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "</tr>";

    //varHtml += LinhaemBranco;

    // TemLinha = 0;

    for (var i in lstDebitos) {
        if (lstDebitos[i].Borda == 1) {
            mClass = 'Credito-Totalizador';
            varHtml += '<tr class="Credito-Totalizador">';
        }
        else if (lstDebitos[i].Vermelho == 1) {
            mClass = 'Ger-fontRed';
            varHtml += '<tr>';
        }
        else if (lstDebitos[i].Bold == 1) {
            mClass = 'Ger-Bold';
            varHtml += '<tr>';
        }
        else {
            mClass = 'Credito-Normal';
            varHtml += '<tr>';
        }

        //TemLinha = 0;

        varHtml += "<td class='" + mClass + " Ger-font Text-Rel'>" + IsNull(lstDebitos[i].Descricao, ' ') + ' </td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes01.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes02.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes03.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes04.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes05.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes06.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes07.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes08.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes09.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes10.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes11.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].Mes12.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstDebitos[i].ValorAcumulado.formatMoney(2, ',', '.') + '</td>';

        if (lstDebitos[i].Espacada == 1) {
            mClass = 'Ger-Lin';
            varHtml += '</tr>' + LinhaemBranco;
            //TemLinha = 1;
        }
        else
            varHtml += '</tr>';
    }

    varHtml += '</table>';
    varHtml += '</div>';

    return varHtml;
}

function Saldos(lstSaldos) {
    var mClass, varHtml, LinhaemBranco, TemLinha;

    LinhaemBranco = "<tr style='line-height: 8px;'><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>";

    varHtml = '';

    varHtml += "<div style='page-break-after: always'>";

    varHtml += "<table>";
    varHtml += "<tr style='font-family:Tahoma;font-size: 9px;color:black;font-weight:bold'>";
    varHtml += "<th width='13.94%' class='Text-Rel' style='border-bottom: 1px solid black;'>Descrição</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Janeiro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Fevereiro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Março</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Abril</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Maio</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Junho</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Julho</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Agosto</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Setembro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Outubro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Novembro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Dezembro</th>";
    varHtml += "<th width='6.62%' class='Val-Rel' style='border-bottom: 1px solid black;'>Acumulado</th>";
    varHtml += "</tr>";
   // varHtml += LinhaemBranco;

    varHtml += "<tr>";
    varHtml += "<td class='SubTitN-Rel Ger-font' style='text-align:center;border-bottom: 1px solid black;'>Saldos</td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "<td class='Text-Rel Ger-font' style='border-bottom: 1px solid black;'> </td>";
    varHtml += "</tr>";

    //varHtml += LinhaemBranco;

    // TemLinha = 0;

    for (var i in lstSaldos) {
        if (lstSaldos[i].Borda == 1) {
            mClass = 'Credito-Totalizador';
            varHtml += '<tr class="Credito-Totalizador">';
        }
        else if (lstSaldos[i].Vermelho == 1) {
            mClass = 'Ger-fontRed';
            varHtml += '<tr>';
        }
        else if (lstSaldos[i].Bold == 1) {
            mClass = 'Ger-Bold';
            varHtml += '<tr>';
        }
        else {
            mClass = 'Credito-Normal';
            varHtml += '<tr>';
        }

        //TemLinha = 0;

        varHtml += "<td class='" + mClass + " Ger-font Text-Rel'>" + IsNull(lstSaldos[i].Descricao, ' ') + ' </td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes01.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes02.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes03.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes04.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes05.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes06.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes07.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes08.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes09.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes10.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes11.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].Mes12.formatMoney(2, ',', '.') + '</td>';
        varHtml += "<td class='" + mClass + " Ger-font Val-Rel' >" + lstSaldos[i].ValorAcumulado.formatMoney(2, ',', '.') + '</td>';

        if (lstSaldos[i].Espacada == 1) {
            mClass = 'Ger-Lin';
            varHtml += '</tr>' + LinhaemBranco;
            //TemLinha = 1;
        }
        else
            varHtml += '</tr>';
    }

    varHtml += '</table>';
    varHtml += '</div>';

    return varHtml;
}