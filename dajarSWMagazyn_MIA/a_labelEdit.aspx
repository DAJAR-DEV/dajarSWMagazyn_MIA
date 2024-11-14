<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="a_labelEdit.aspx.vb" Inherits="dajarSWMagazyn_MIA.a_labelEdit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Zarzadzanie edycja - dajarSystemWspomaganiaMagazynu by MM</title>
    <meta name="format-detection" content="telephone=no"/>
    <link rel="apple-touch-icon" href="favicon.ico"/>
    <meta name="viewport" content="initial-scale=1.0, maximum-scale=5.0"/>
    <style type="text/css">
        .button-menu{font-size:18px;font-family:tahoma;font-weight:bold;width:288px;height:35px;text-align:center;-moz-border-radius-bottomleft:5px;-moz-border-radius-bottomright:5px;-moz-border-radius-topleft:5px;-moz-border-radius-topright:5px;-moz-box-shadow:0 1px 3px rgba(0,0,0,0.25);text-transform:uppercase;background:#000000;border:1px solid #777;border-bottom:2px solid rgba(0,0,0,0.25);color:#FFF!important;cursor:pointer;display:inline-table;line-height:1;overflow:visible;padding:8px 7px;position:relative;text-decoration:none;text-shadow:0 -1px 1px rgba(0,0,0,0.25);margin-top:7px;float:left;}
        .button-menu-bottom{font-size:18px;font-family:tahoma;font-weight:bold;width:135px;height:35px;text-align:center;-moz-border-radius-bottomleft:5px;-moz-border-radius-bottomright:5px;-moz-border-radius-topleft:5px;-moz-border-radius-topright:5px;-moz-box-shadow:0 1px 3px rgba(0,0,0,0.25);text-transform:uppercase;background:#000000;border:1px solid #777;border-bottom:2px solid rgba(0,0,0,0.25);color:#FFF!important;cursor:pointer;display:inline-table;line-height:1;overflow:visible;padding:8px 7px;position:relative;text-decoration:none;text-shadow:0 -1px 1px rgba(0,0,0,0.25);margin-top:7px;float:left;}
        .button-menu-hist{font-size:18px;font-family:tahoma;font-weight:bold;width:85px;height:35px;text-align:center;-moz-border-radius-bottomleft:5px;-moz-border-radius-bottomright:5px;-moz-border-radius-topleft:5px;-moz-border-radius-topright:5px;-moz-box-shadow:0 1px 3px rgba(0,0,0,0.25);text-transform:uppercase;background:#000000;border:1px solid #777;border-bottom:2px solid rgba(0,0,0,0.25);color:#FFF!important;cursor:pointer;display:inline-table;line-height:1;overflow:visible;padding:8px 7px;position:relative;text-decoration:none;text-shadow:0 -1px 1px rgba(0,0,0,0.25);margin-top:7px;float:left;}
        .button-menu-hist-small{font-size:18px;font-family:tahoma;font-weight:bold;width:60px;height:35px;text-align:center;-moz-border-radius-bottomleft:5px;-moz-border-radius-bottomright:5px;-moz-border-radius-topleft:5px;-moz-border-radius-topright:5px;-moz-box-shadow:0 1px 3px rgba(0,0,0,0.25);text-transform:uppercase;background:#000000;border:1px solid #777;border-bottom:2px solid rgba(0,0,0,0.25);color:#FFF!important;cursor:pointer;display:inline-table;line-height:1;overflow:visible;padding:8px 7px;position:relative;text-decoration:none;text-shadow:0 -1px 1px rgba(0,0,0,0.25);margin-top:7px;float:left;}
    </style>
</head>

<body>
<script src="/scripts/jquery-3.6.3.min.js" type="text/javascript"></script>
<script type="text/javascript">
    var wcppPingTimeout_ms = 60000; //60 sec
    var wcppPingTimeoutStep_ms = 500; //0.5 sec

    function wcppDetectOnSuccess(){
        // WCPP utility is installed at the client side
        // redirect to WebClientPrint sample page
                
        // get WCPP version
        var wcppVer = arguments[0];
        //if(wcppVer.substring(0, 1) == "6")
            //window.location.href = 'PrintPDF.aspx';
        //else //force to install WCPP v6.0
            //wcppDetectOnFailure();
    }

    function wcppDetectOnFailure() {
        // It seems WCPP is not installed at the client side
        // ask the user to install it
        //$('#msgInProgress').hide();
        //$('#msgInstallWCPP').show();
        alert("Brak zainstalowanej biblioteki WCPP Komponent")
    }

    function wcpGetPrintersOnSuccess() {
		// Display client installed printers
		if (arguments[0].length > 0) {
			var p = arguments[0].split("|");
			var options = '';
			for (var i = 0; i < p.length; i++) {
				options += '<option>' + p[i] + '</option>';
			}
			$('#installedPrinters').css('visibility', 'visible');
			$('#installedPrinterName').html(options);
			$('#installedPrinterName').focus();
			$('#loadPrinters').hide();
		} else {
			alert("No printers are installed in your system.");
		}
	}
	function wcpGetPrintersOnFailure() {
		// Do something if printers cannot be got from the client
		alert("No printers are installed in your system.");
	}

</script>
<%-- WCPP detection script code --%>
<%=Neodynamic.SDK.Web.WebClientPrint.CreateWcppDetectionScript(HttpContext.Current.Request.Url.Scheme.ToString + "://" + HttpContext.Current.Request.Url.Host.ToString + ":" + HttpContext.Current.Request.Url.Port.ToString + "/WebClientPrintAPI.ashx", HttpContext.Current.Session.SessionID)%>

<%-- Register the WebClientPrint script code --%>
<%=Neodynamic.SDK.Web.WebClientPrint.CreateScript(HttpContext.Current.Request.Url.Scheme.ToString + "://" + HttpContext.Current.Request.Url.Host.ToString + ":" + HttpContext.Current.Request.Url.Port.ToString + "/WebClientPrintAPI.ashx", HttpContext.Current.Request.Url.Scheme.ToString + "://" + HttpContext.Current.Request.Url.Host.ToString + ":" + HttpContext.Current.Request.Url.Port.ToString + "/PrintPDFHandler.ashx", HttpContext.Current.Session.SessionID)%>

<form id="formBody" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
<div style="text-align: left">
<asp:Panel ID="panelMenu" runat="server">
    <img src="Dajar_logo.png" height="45px" width="154px" alt="dajarSWMagazyn_MIA" style="display:block; margin:10px 0 0 auto;"/>
    <hr style="width:1260px" align="left"/>
    <table style="margin: 10px auto 10px 0;width:1260px" border="0" cellpadding="0" cellspacing="0">
        <tr valign="middle" align="justify" bgcolor="#FFFFCC" style="vertical-align: middle">
            <td>
                <a class="button-menu" href="login.aspx" title="logowanie">LOGOWANIE</a>
            </td>
            <td>
                <a class="button-menu" href="admin.aspx" title="magazyn">MAGAZYN</a>
            </td>
            <td>
                <a class="button-menu" href="admin.aspx" title="pakowanie">PAKOWANIE</a>
            </td>
            <td>
                <a class="button-menu" href="a_logout.aspx" title="wylogowanie">WYLOGOWANIE</a>
            </td>
        </tr>
        <tr valign="middle" align="justify" bgcolor="#FFFFCC" style="vertical-align: middle">
            <td>
                <a class="button-menu-bottom" href="a_user.aspx" title="uzytkownicy">UŻYTKOWNICY</a>
                <a class="button-menu-bottom" href="a_adresy.aspx" title="adresy">ADRESY</a>
            </td>
            <td>
                <a class="button-menu-hist" href="a_manage.aspx" title="dyspozycje">DYSP</a>
                <a class="button-menu-hist" href="a_dyspo.aspx" title="losowanie">LOSOW</a>
                <a class="button-menu-hist" href="a_dyspo_ext.aspx" title="dyspozycje_szczegoly">DYSP_EX</a>
            </td>
            <td>
                <a class="button-menu-hist-small" href="a_history.aspx" title="historia">HIST</a>
                <a class="button-menu-hist-small" href="a_magazyn.aspx" title="magazyn">MAG</a>
                <a class="button-menu-hist-small" href="a_pakowanie.aspx" title="pakowanie">PAK</a>
                <a class="button-menu-hist-small" href="a_dubel.aspx" title="dubel">DUBEL</a>
            </td>
            <td>
                <a class="button-menu-bottom" href="a_label.aspx" title="etykiety">ETYKIETY</a>
                <a class="button-menu-bottom" href="a_print.aspx" title="wydruki">WYDRUKI</a>
            </td>
        </tr>
    </table>
    <hr align="left" style="width:1260px"/>
    <table border="0" cellpadding="0" cellspacing="0" style="margin: 10px auto 10px 0;width:1260px">
        <tr align="justify" bgcolor="#FFFFCC" style="vertical-align: middle" valign="middle">
            <td>OPERATOR : <% Response.Write(Session("mylogin"))%></td>
            <td align="right">MAGAZYN : <%Response.Write(Session("contentMagazyn"))%></td>
        </tr>
    </table>
    <hr align="left" style="width:1260px"/>
    <table border="0" cellpadding="0" cellspacing="0" style="margin: 10px auto 10px 0;width:100%">
        <tr style="height:25px;">
            <td bgcolor="#FFD501"
                style="text-align: left; color: #000000; font-weight: bold; font-size: small;">
                <%                        Response.Write(Session("contentOperator"))%>&nbsp;
            </td>
            <td bgcolor="#FFD501"
                style="text-align: right; color: #000000; font-weight: bold; font-size: small;">
                <%                        Response.Write(Session("contentHash"))%>&nbsp;
            </td>
        </tr>
    </table>
</asp:Panel>
<div style="text-align: left; width: 100%;">
<asp:Label runat="server" ID="labelKomunikat" ForeColor="Black" Font-Size="Large" Width="100%"><% Response.Write(Session("contentKomunikat"))%></asp:Label>
<br/>
<asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium">Karta edycji etykiety</asp:Label>
<asp:Panel ID="Panel3" runat="server" BorderColor="#333333" BorderStyle="None" borderWidth="1px" Font-Size="Large" Width="1260px">
<asp:panel ID="panel11" runat="server">
    <asp:Table ID="Table10" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell>
                [numer aktualnie edytowanego zamowienia] : <asp:Label ID="LNrZamow" runat="server" Font-Bold="False" ForeColor="Red"></asp:Label><br/>
                [wersja schematu digitland] : <asp:Label ID="LSchemat" runat="server"></asp:Label><br/>
                [numer zamówienia klienta] : <asp:Label ID="LNr_zamow_o" runat="server" Font-Bold="True"></asp:Label> &nbsp; <asp:Button ID="BPobierzNrDigit" runat="server" Text="NR DIGIT DAJAR" BackColor="Black" ForeColor="White"/>
                <asp:Panel ID="panelDigitland" runat="server" Visible="false">
                    [numer zamowienia digitland dajar] : <asp:Label ID="LNrZamowDigit" runat="server" ForeColor="Red"></asp:Label>
                </asp:Panel>
            </asp:TableCell>
            <asp:TableCell>
                [magazyn edytowanego zamowienia] : <asp:Label ID="LMagDyspo" runat="server" Font-Bold="True"></asp:Label><br/>
                [data zamówienia digitland] :
                <asp:Label ID="LDataZam" runat="server" Text="Label"></asp:Label>
                <br/>
                [numer aktualnie edytowanego listu przewozowego] : <asp:Label ID="LNr_list_przewozowy" runat="server" Font-Bold="True"></asp:Label><br/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:panel>
<asp:Panel ID="panelZamowienia" runat="server" GroupingText="Lista zamowien powiazanych">
    &nbsp;[identyfikator zaznaczonego zamowienia]
    <asp:Label ID="LZamowienieId" runat="server" ForeColor="Red"></asp:Label>
    <asp:GridView ID="GridViewZamowienia" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
        <HeaderStyle BackColor="#E5E5E5"/>
        <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
        <Columns>
            <asp:TemplateField HeaderText="Select">
                <ItemTemplate>
                    <asp:CheckBox ID="CBKodSelect" runat="server"/>
                </ItemTemplate>
                <HeaderTemplate>
                </HeaderTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:Table ID="table8" runat="server" Width="100%">
        <asp:TableRow HorizontalAlign="Right">
            <asp:TableCell HorizontalAlign="Right">[wprowadz numer zamowienia]</asp:TableCell>
            <asp:TableCell HorizontalAlign="Right" Width="100">
                <asp:TextBox ID="TBNrZamowienia" runat="server" TextMode="SingleLine" Width="100px" BackColor="#FFFF80" MaxLength="12"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right" Width="275">[wprowadz schemat zamowienia]</asp:TableCell>
            <asp:TableCell HorizontalAlign="Right" Width="75">
                <asp:TextBox ID="TBSchemat" runat="server" TextMode="SingleLine" Width="75px" BackColor="#FFFF80" MaxLength="7"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right" Width="225">
                <asp:Button ID="BDodajZamowienie" runat="server" Text="DODAJ ZAMOWIENIE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="5">
                <asp:Button ID="BAnulujZamowienie" runat="server" Text="ANULUJ ZAMOWIENIE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<br/>
<asp:Button ID="BWyswietlArtykuly" runat="server" Text="Wyswietl liste artykulow" Visible="false"/>
<!--<br /><br />-->
<asp:GridView ID="GridViewDyspozycje" runat="server" AutoGenerateSelectButton="false" CellPadding="5" Font-Size="Medium"
              OnPageIndexChanging="GridViewDyspozycje_PageIndexChanging"
              Width="100%" AllowPaging="True" PageSize="10" Visible="false">
    <PagerSettings Position="TopAndBottom"/>
    <PagerStyle HorizontalAlign="Right"/>
    <SelectedRowStyle ForeColor="Red"/>
    <HeaderStyle BackColor="#E5E5E5"/>
</asp:GridView>
<asp:Panel ID="Panel_DHL_GENERATE" runat="server" GroupingText="Lista utworzonych przesylek DHL" Visible="false">
    <asp:Table ID="table12" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Right" ColumnSpan="5">
                <asp:TextBox ID="TBListPrzewozowy_DHL" runat="server"></asp:TextBox>
                &nbsp;
                <asp:Button ID="BWprowadzList_DHL" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="WPROWADZ DHL" Width="275px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="4" HorizontalAlign="Left">
                <asp:Button ID="BRaportPNP" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="RAPORT PNP" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BGenerujEtykiete_DHL" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="GENERUJ ETYKIETE" Width="275px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaLP" runat="server" Target="_blank">etykieta LP</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaBLP" runat="server" Target="_blank">etykieta BLP</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaZBLP" runat="server" Target="_blank">etykieta ZBLP</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell ColumnSpan="2">
                <asp:HyperLink ID="HLRaportPNP" runat="server" Target="_blank">raport PNP</asp:HyperLink>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Left" ColumnSpan="5">
                <asp:GridView ID="GridViewPrzesylki" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
                    <HeaderStyle BackColor="#E5E5E5"/>
                    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="CBKodSelect" runat="server"/>
                            </ItemTemplate>
                            <HeaderTemplate></HeaderTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Link">
                            <ItemTemplate>
                                <asp:HyperLink ID="CBLink" runat="server" Target="_blank"/>
                            </ItemTemplate>
                            <HeaderTemplate></HeaderTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="left">
                <asp:Button ID="BPobierzLP" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. LP" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Center">
                <asp:Button ID="BPobierzPdf" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. PDF" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BPobierzZebra" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. ZEBRA" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BPobierzWszystkie" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. KOMPLET" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BAnuluj_DHL" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ANULUJ LP" Width="225px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="5">
                <asp:CheckBox ID="CBWymuszenieAnulowania" runat="server" Text="czy wymusic anulowanie listu przewozowego z systemu" Checked="false" AutoPostBack="true"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_GEIS_GENERATE" runat="server" GroupingText="Lista utworzonych przesylek GEIS" Visible="false">
    <asp:Table ID="table14" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Right" ColumnSpan="5">
                <asp:TextBox ID="TBListPrzewozowy_GEIS" runat="server"></asp:TextBox>
                &nbsp;
                <asp:Button ID="BWprowadzList_GEIS" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="WPROWADZ GEIS" Width="275px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="4" HorizontalAlign="Left">
                <asp:Button ID="BRaportPickup_GEIS" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="GENERUJ PICKUP" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BGenerujEtykiete_GEIS" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="GENERUJ ETYKIETE" Width="275px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaPDF_GEIS" runat="server" Target="_blank">etykieta PDF</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaEPL_GEIS" runat="server" Target="_blank">etykieta EPL</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaZPL_GEIS" runat="server" Target="_blank">etykieta ZPL</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell ColumnSpan="2">
                <asp:HyperLink ID="HLRaportPickup_GEIS" runat="server" Target="_blank">raport Pickup</asp:HyperLink>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Left" ColumnSpan="5">
                <asp:GridView ID="GridViewPrzesylki_GEIS" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
                    <HeaderStyle BackColor="#E5E5E5"/>
                    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="CBKodSelect" runat="server"/>
                            </ItemTemplate>
                            <HeaderTemplate></HeaderTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Link">
                            <ItemTemplate>
                                <asp:HyperLink ID="CBLink" runat="server" Target="_blank"/>
                            </ItemTemplate>
                            <HeaderTemplate></HeaderTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="left">
                <asp:Button ID="BPobierzPDF_GEIS" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. PDF" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Center">
                <asp:Button ID="BPobierzEPL_GEIS" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. EPL" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BPobierzZPL_GEIS" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. ZPL" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BAnuluj_GEIS" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ANULUJ LP" Width="225px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="5">
                <asp:CheckBox ID="CBWymuszenieAnulowania_GEIS" runat="server" Text="czy wymusic anulowanie listu przewozowego z systemu" Checked="false" AutoPostBack="true"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_DHL_DE_API_GENERATE" runat="server" GroupingText="Lista utworzonych przesylek DHL_DE_API" Visible="false">
    <asp:Table ID="table23" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Right" ColumnSpan="5">
                <asp:TextBox ID="TBListPrzewozowy_DHL_DE_API" runat="server"></asp:TextBox>
                &nbsp;
                <asp:Button ID="BWprowadzList_DHL_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="WPROWADZ DHL_DE_API" Width="325px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="3" HorizontalAlign="Left">
                <asp:Button ID="BRaportPickup_DHL_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="GENERUJ PICKUP" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BGenerujEtykiete_DHL_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="GENERUJ ETYKIETE" Width="275px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BDrukowanieEtykieta_DHL_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="DRUKUJ" Width="225px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaPDF_DHL_DE_API" runat="server" Target="_blank">etykieta PDF</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaEPL_DHL_DE_API" runat="server" Target="_blank">etykieta EPL</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaZPL_DHL_DE_API" runat="server" Target="_blank">etykieta ZPL</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell ColumnSpan="2">
                <asp:HyperLink ID="HLRaportPickup_DHL_DE_API" runat="server" Target="_blank">raport Pickup</asp:HyperLink>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Left" ColumnSpan="5">
                <asp:GridView ID="GridViewPrzesylki_DHL_DE_API" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
                    <HeaderStyle BackColor="#E5E5E5"/>
                    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="CBKodSelect" runat="server"/>
                            </ItemTemplate>
                            <HeaderTemplate></HeaderTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Link">
                            <ItemTemplate>
                                <asp:HyperLink ID="CBLink" runat="server" Target="_blank"/>
                            </ItemTemplate>
                            <HeaderTemplate></HeaderTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="left">
                <asp:Button ID="BPobierzPDF_DHL_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. PDF" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Center">
                <asp:Button ID="BPobierzEPL_DHL_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. EPL" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BPobierzZPL_DHL_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. ZPL" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BAnuluj_DHL_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ANULUJ LP" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BZerowanie_DHL_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ZEROWANIE LP" Width="225px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="5">
                <asp:CheckBox ID="CBWymuszenieAnulowania_DHL_DE_API" runat="server" Text="czy wymusic anulowanie listu przewozowego z systemu" Checked="false" AutoPostBack="true"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_UPS_DE_API_GENERATE" runat="server" GroupingText="Lista utworzonych przesylek UPS_DE_API" Visible="false">
    <asp:Table ID="table24" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Right" ColumnSpan="5">
                <asp:TextBox ID="TBListPrzewozowy_UPS_DE_API" runat="server"></asp:TextBox>
                &nbsp;
                <asp:Button ID="BWprowadzList_UPS_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="WPROWADZ UPS_DE_API" Width="325px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="3" HorizontalAlign="Left">
                <asp:Button ID="BRaportPickup_UPS_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="GENERUJ PICKUP" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BGenerujEtykiete_UPS_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="GENERUJ ETYKIETE" Width="275px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BDrukowanieEtykieta_UPS_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="DRUKUJ" Width="225px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaPDF_UPS_DE_API" runat="server" Target="_blank">etykieta GIF</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaEPL_UPS_DE_API" runat="server" Target="_blank">etykieta EPL</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaZPL_UPS_DE_API" runat="server" Target="_blank">etykieta ZPL</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell ColumnSpan="2">
                <asp:HyperLink ID="HLRaportPickup_UPS_DE_API" runat="server" Target="_blank">raport Pickup</asp:HyperLink>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Left" ColumnSpan="5">
                <asp:GridView ID="GridViewPrzesylki_UPS_DE_API" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
                    <HeaderStyle BackColor="#E5E5E5"/>
                    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="CBKodSelect" runat="server"/>
                            </ItemTemplate>
                            <HeaderTemplate></HeaderTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Link">
                            <ItemTemplate>
                                <asp:HyperLink ID="CBLink" runat="server" Target="_blank"/>
                            </ItemTemplate>
                            <HeaderTemplate></HeaderTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="left">
                <asp:Button ID="BPobierzPDF_UPS_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. GIF" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Center">
                <asp:Button ID="BPobierzEPL_UPS_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. EPL" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BPobierzZPL_UPS_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. ZPL" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BAnuluj_UPS_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ANULUJ LP" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BZerowanie_UPS_DE_API" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ZEROWANIE LP" Width="225px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="5">
                <asp:CheckBox ID="CBWymuszenieAnulowania_UPS_DE_API" runat="server" Text="czy wymusic anulowanie listu przewozowego z systemu" Checked="false" AutoPostBack="true"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_INPOST_GENERATE" runat="server" GroupingText="Lista utworzonych przesylek INPOST" Visible="false">
    <asp:Table ID="table15" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Right" ColumnSpan="5">
                <asp:TextBox ID="TBListPrzewozowy_INPOST" runat="server"></asp:TextBox>
                &nbsp;
                <asp:Button ID="BWprowadzList_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="WPROWADZ INPOST" Width="275px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="3" HorizontalAlign="Left">
                <asp:Button ID="BRaportPickup_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="GENERUJ PICKUP" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BGenerujEtykiete_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="GENERUJ OFERTE" Width="275px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BGenerujOferte_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="OPLAC OFERTE" Width="275px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaPDF_INPOST" runat="server" Target="_blank">etykieta PDF</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaEPL_INPOST" runat="server" Target="_blank">etykieta EPL</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell>
                <asp:HyperLink ID="HLEtykietaZPL_INPOST" runat="server" Target="_blank">etykieta ZPL</asp:HyperLink>
            </asp:TableCell>
            <asp:TableCell ColumnSpan="2">
                <asp:HyperLink ID="HLRaportPickup_INPOST" runat="server" Target="_blank">raport Pickup</asp:HyperLink>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Left" ColumnSpan="5">
                <asp:GridView ID="GridViewPrzesylki_INPOST" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
                    <HeaderStyle BackColor="#E5E5E5"/>
                    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="CBKodSelect" runat="server"/>
                            </ItemTemplate>
                            <HeaderTemplate></HeaderTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Link">
                            <ItemTemplate>
                                <asp:HyperLink ID="CBLink" runat="server" Target="_blank"/>
                            </ItemTemplate>
                            <HeaderTemplate></HeaderTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="left">
                <asp:Button ID="BPobierzPDF_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. PDF" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Center">
                <asp:Button ID="BPobierzEPL_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. EPL" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BPobierzZPL_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYK. ZPL" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BAnuluj_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ANULUJ LP" Width="225px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="Right">
                <asp:Button ID="BZerowanie_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ZEROWANIE LP" Width="225px"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="5">
                <asp:CheckBox ID="CBWymuszenieAnulowania_INPOST" runat="server" Text="czy wymusic anulowanie listu przewozowego z systemu" Checked="false" AutoPostBack="true"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel5" runat="server" Font-Size="Medium" GroupingText="Kreator tworzenia etykiety logistycznej">
<asp:Panel ID="PanelDodawaniePaczki" runat="server" GroupingText="I.Wprowadzanie parametrow paczek" Visible="false" BackColor="#FFFFCC">
<asp:Table ID="TablePackage" runat="server" Width="100%">
<asp:TableRow>
    <asp:TableCell HorizontalAlign="Left" ColumnSpan="2">
        &nbsp;[numer wybranego zamowienia] <asp:Label ID="LNrZamowieniaPaczka" runat="server" ForeColor="Red"></asp:Label><br/>
        &nbsp;[schemat wybranego zamowienia] <asp:Label ID="LSchematPaczka" runat="server" ForeColor="Red"></asp:Label><br/>
        &nbsp;[identyfikator paczki] <asp:Label ID="LPaczkaID" runat="server" ForeColor="Red"></asp:Label>
    </asp:TableCell>
    <asp:TableCell HorizontalAlign="Right" ColumnSpan="2">
        &nbsp;[firma przewozowa] <asp:DropDownList ID="DDLFirma" runat="server" Font-Size="X-Large" Height="30px" Width="310px" AutoPostBack="true"></asp:DropDownList>
        &nbsp;<asp:Button ID="BZapiszFirme" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="USTAW FIRME" Width="250px"/>
    </asp:TableCell>
</asp:TableRow>
<asp:TableRow>
<asp:TableCell HorizontalAlign="Left" ColumnSpan="4">
<asp:Table ID="Table_DHL" runat="server" Width="100%" Visible="false" BorderWidth="1" BackColor="#E5E5E5">
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[typ][paczka/paleta]
            <asp:DropDownList ID="DDLTypDHL" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="1">paczka</asp:ListItem>
                <asp:ListItem Value="2">paleta</asp:ListItem>
                <asp:ListItem Value="3">koperta</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[rodzaj][std./niestd.]
            <asp:DropDownList ID="DDLRodzajDHL" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="0">standard</asp:ListItem>
                <asp:ListItem Value="1">niestandard</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[paleta_zwrot][nie/tak]
            <asp:DropDownList ID="DDLPaletaDHL" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="0">nie</asp:ListItem>
                <asp:ListItem Value="1">tak</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[ile_opak][szt]<asp:TextBox ID="TBIlePaczekDHL" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[waga][kg]<asp:TextBox ID="TBWagaDHL" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[szerokosc][cm]<asp:TextBox ID="TBSzerDHL" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[wysokosc][cm]<asp:TextBox ID="TBWysDHL" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[dlugosc][cm]<asp:TextBox ID="TBDlugDHL" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<asp:Table ID="Table_DHL_PS" runat="server" Width="100%" Visible="false" BorderWidth="1" BackColor="#E5E5E5">
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[typ][paczka/paleta]
            <asp:DropDownList ID="DDLTypDHL_PS" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="1">paczka</asp:ListItem>
                <asp:ListItem Value="2">paleta</asp:ListItem>
                <asp:ListItem Value="3">koperta</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[rodzaj][std./niestd.]
            <asp:DropDownList ID="DDLRodzajDHL_PS" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="0">standard</asp:ListItem>
                <asp:ListItem Value="1">niestandard</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[paleta_zwrot][nie/tak]
            <asp:DropDownList ID="DDLPaletaDHL_PS" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="0">nie</asp:ListItem>
                <asp:ListItem Value="1">tak</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[ile_opak][szt]<asp:TextBox ID="TBIlePaczekDHL_PS" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[waga][kg]<asp:TextBox ID="TBWagaDHL_PS" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[szerokosc][cm]<asp:TextBox ID="TBSzerDHL_PS" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[wysokosc][cm]<asp:TextBox ID="TBWysDHL_PS" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[dlugosc][cm]<asp:TextBox ID="TBDlugDHL_PS" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<asp:Table ID="Table_GEIS_DAJAR_KRAJ" runat="server" Width="100%" Visible="false" BorderWidth="1" BackColor="#E5E5E5">
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[typ][paczka/paleta]
            <asp:DropDownList ID="DDLTypGEIS_DAJAR_KRAJ" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="HP">półpaleta [HP]</asp:ListItem>
                <asp:ListItem Value="EP">bezzwrotna paleta [EP]</asp:ListItem>
                <asp:ListItem Value="CC">colli [CC]</asp:ListItem>
                <asp:ListItem Value="VP">1/4 paleta [VP]</asp:ListItem>
                <asp:ListItem Value="NP">paleta 1.0x2.0 [NP]</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[rodzaj][cargo exp/cargo zam]
            <asp:DropDownList ID="DDLRodzajGEIS_DAJAR_KRAJ" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="20">cargo ekspedycja [20]</asp:ListItem>
                <asp:ListItem Value="21">cargo zamówienia [21]</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[ile_opak][szt]<asp:TextBox ID="TBIlePaczekGEIS_DAJAR_KRAJ" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[waga][kg]<asp:TextBox ID="TBWagaGEIS_DAJAR_KRAJ" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[szerokosc][m]<asp:TextBox ID="TBSzerGEIS_DAJAR_KRAJ" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[wysokosc][m]<asp:TextBox ID="TBWysGEIS_DAJAR_KRAJ" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[dlugosc][m]<asp:TextBox ID="TBDlugGEIS_DAJAR_KRAJ" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<asp:Table ID="Table_GEIS_HIPO" runat="server" Width="100%" Visible="false" BorderWidth="1" BackColor="#E5E5E5">
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[typ][paczka/paleta]
            <asp:DropDownList ID="DDLTypGEIS_HIPO" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="HP">półpaleta [HP]</asp:ListItem>
                <asp:ListItem Value="EP">bezzwrotna paleta [EP]</asp:ListItem>
                <asp:ListItem Value="CC">colli [CC]</asp:ListItem>
                <asp:ListItem Value="FP">europaleta [FP]</asp:ListItem>
                <asp:ListItem Value="NP">paleta 1.0x2.0 [NP]</asp:ListItem>
                <asp:ListItem Value="VP">1/4 paleta [VP]</asp:ListItem>
                <asp:ListItem Value="PC">chep [PC]</asp:ListItem>
                <asp:ListItem Value="PEP">przekracza europalete [PEP]</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[rodzaj][cargo exp/cargo zam]
            <asp:DropDownList ID="DDLRodzajGEIS_HIPO" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="20">cargo ekspedycja [20]</asp:ListItem>
                <asp:ListItem Value="21">cargo zamówienia [21]</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[ile_opak][szt]<asp:TextBox ID="TBIlePaczekGEIS_HIPO" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[waga][kg]<asp:TextBox ID="TBWagaGEIS_HIPO" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[szerokosc][m]<asp:TextBox ID="TBSzerGEIS_HIPO" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[wysokosc][m]<asp:TextBox ID="TBWysGEIS_HIPO" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[dlugosc][m]<asp:TextBox ID="TBDlugGEIS_HIPO" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<asp:Table ID="Table_GEIS_COD_CZECHY" runat="server" Width="100%" Visible="false" BorderWidth="1" BackColor="#E5E5E5">
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[typ][paczka/paleta]
            <asp:DropDownList ID="DDLTypGEIS_COD_CZECHY" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="HP">półpaleta [HP]</asp:ListItem>
                <asp:ListItem Value="EP">bezzwrotna paleta [EP]</asp:ListItem>
                <asp:ListItem Value="CC">colli [CC]</asp:ListItem>
                <asp:ListItem Value="FP">europaleta [FP]</asp:ListItem>
                <asp:ListItem Value="NP">paleta 1.0x2.0 [NP]</asp:ListItem>
                <asp:ListItem Value="VP">1/4 paleta [VP]</asp:ListItem>
                <asp:ListItem Value="SP">standard paleta [SP]</asp:ListItem>
                <asp:ListItem Value="PEP">przekracza europalete [PEP]</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[rodzaj][cargo exp/cargo zam]
            <asp:DropDownList ID="DDLRodzajGEIS_COD_CZECHY" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="20">cargo ekspedycja [20]</asp:ListItem>
                <asp:ListItem Value="21">cargo zamówienia [21]</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[ile_opak][szt]<asp:TextBox ID="TBIlePaczekGEIS_COD_CZECHY" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[waga][kg]<asp:TextBox ID="TBWagaGEIS_COD_CZECHY" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[szerokosc][m]<asp:TextBox ID="TBSzerGEIS_COD_CZECHY" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[wysokosc][m]<asp:TextBox ID="TBWysGEIS_COD_CZECHY" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[dlugosc][m]<asp:TextBox ID="TBDlugGEIS_COD_CZECHY" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<asp:Table ID="Table_GEIS_COD_SLOWACJA" runat="server" Width="100%" Visible="false" BorderWidth="1" BackColor="#E5E5E5">
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[typ][paczka/paleta]
            <asp:DropDownList ID="DDLTypGEIS_COD_SLOWACJA" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="HP">półpaleta [HP]</asp:ListItem>
                <asp:ListItem Value="EP">bezzwrotna paleta [EP]</asp:ListItem>
                <asp:ListItem Value="CC">colli [CC]</asp:ListItem>
                <asp:ListItem Value="FP">europaleta [FP]</asp:ListItem>
                <asp:ListItem Value="NP">paleta 1.0x2.0 [NP]</asp:ListItem>
                <asp:ListItem Value="VP">1/4 paleta [VP]</asp:ListItem>
                <asp:ListItem Value="SP">standard paleta [SP]</asp:ListItem>
                <asp:ListItem Value="PEP">przekracza europalete [PEP]</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[rodzaj][cargo exp/cargo zam]
            <asp:DropDownList ID="DDLRodzajGEIS_COD_SLOWACJA" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="20">cargo ekspedycja [20]</asp:ListItem>
                <asp:ListItem Value="21">cargo zamówienia [21]</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[ile_opak][szt]<asp:TextBox ID="TBIlePaczekGEIS_COD_SLOWACJA" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[waga][kg]<asp:TextBox ID="TBWagaGEIS_COD_SLOWACJA" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[szerokosc][m]<asp:TextBox ID="TBSzerGEIS_COD_SLOWACJA" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[wysokosc][m]<asp:TextBox ID="TBWysGEIS_COD_SLOWACJA" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[dlugosc][m]<asp:TextBox ID="TBDlugGEIS_COD_SLOWACJA" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<asp:Table ID="Table_INPOST_ALLEGRO" runat="server" Width="100%" Visible="false" BorderWidth="1" BackColor="#E5E5E5">
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[typ][paczka/paleta]
            <asp:DropDownList ID="DDLTypINPOST_ALLEGRO" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="1">paczka</asp:ListItem>
                <asp:ListItem Value="2">paleta</asp:ListItem>
                <asp:ListItem Value="3">koperta</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            <asp:UpdatePanel ID="updInpostDDLRodzaj" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    &nbsp;[rodzaj][cargo exp/cargo zam]
                    <asp:DropDownList ID="DDLRodzajINPOST_ALLEGRO" runat="server" Font-Size="X-Large" Height="30px" Width="250px" AutoPostBack="true" OnSelectedIndexChanged="DDLRodzajINPOST_ALLEGRO_SelectedIndexChanged">
                        <asp:ListItem Value="A">rozmiar 8 x 38 x 64 cm [A]</asp:ListItem>
                        <asp:ListItem Value="B">rozmiar 19 x 38 x 64 cm [B]</asp:ListItem>
                        <asp:ListItem Value="C">rozmiar 41 x 38 x 64 cm [C]</asp:ListItem>
                        <asp:ListItem Value="RECZNIE">rozmiar 1 x 1 x 1 cm [RECZNIE]</asp:ListItem>
                    </asp:DropDownList>
                </ContentTemplate>
            </asp:UpdatePanel>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[rodzaj][std./niestd.]
            <asp:DropDownList ID="DDLRodzajStandardINPOST_ALLEGRO" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="0" Selected="True">standard</asp:ListItem>
                <asp:ListItem Value="1">niestandard</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[ile_opak][szt]<asp:TextBox ID="TBIlePaczekINPOST_ALLEGRO" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            <asp:UpdatePanel ID="updInpostWaga" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    &nbsp;[waga][kg]<asp:TextBox ID="TBWagaINPOST_ALLEGRO" runat="server" ValidationExpression="\d+"></asp:TextBox>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DDLRodzajINPOST_ALLEGRO" EventName="SelectedIndexChanged"/>
                </Triggers>
            </asp:UpdatePanel>
        </asp:TableCell>
        <asp:TableCell>
            <asp:UpdatePanel ID="updInpostSzerokosc" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    &nbsp;[szerokosc][cm]<asp:TextBox ID="TBSzerINPOST_ALLEGRO" runat="server" ValidationExpression="\d+"></asp:TextBox>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DDLRodzajINPOST_ALLEGRO" EventName="SelectedIndexChanged"/>
                </Triggers>
            </asp:UpdatePanel>
        </asp:TableCell>
        <asp:TableCell>
            <asp:UpdatePanel ID="updInpostWysokosc" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    &nbsp;[wysokosc][cm]<asp:TextBox ID="TBWysINPOST_ALLEGRO" runat="server" ValidationExpression="\d+"></asp:TextBox>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DDLRodzajINPOST_ALLEGRO" EventName="SelectedIndexChanged"/>
                </Triggers>
            </asp:UpdatePanel>
        </asp:TableCell>
        <asp:TableCell>
            <asp:UpdatePanel ID="updInpostDlugosc" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    &nbsp;[dlugosc][cm]<asp:TextBox ID="TBDlugINPOST_ALLEGRO" runat="server" ValidationExpression="\d+"></asp:TextBox>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="DDLRodzajINPOST_ALLEGRO" EventName="SelectedIndexChanged"/>
                </Triggers>
            </asp:UpdatePanel>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<asp:Table ID="Table_DHL_DE" runat="server" Width="100%" Visible="false" BorderWidth="1" BackColor="#E5E5E5">
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[typ][paczka/paleta]
            <asp:DropDownList ID="DDLTypDHL_DE" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="1">paczka</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[ile_opak][szt]<asp:TextBox ID="TBIlePaczekDHL_DE" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[waga][kg]<asp:TextBox ID="TBWagaDHL_DE" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<asp:Table ID="Table_DHL_DE_API" runat="server" Width="100%" Visible="false" BorderWidth="1" BackColor="#E5E5E5">
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[typ][paczka/paleta]
            <asp:DropDownList ID="DDLTypDHL_DE_API" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="1">paczka</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[ile_opak][szt]<asp:TextBox ID="TBIlePaczekDHL_DE_API" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[waga][kg]<asp:TextBox ID="TBWagaDHL_DE_API" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[szerokosc][cm]<asp:TextBox ID="TBSzerDHL_DE_API" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[wysokosc][cm]<asp:TextBox ID="TBWysDHL_DE_API" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[dlugosc][cm]<asp:TextBox ID="TBDlugDHL_DE_API" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<asp:Table ID="Table_UPS_DE_API" runat="server" Width="100%" Visible="false" BorderWidth="1" BackColor="#E5E5E5">
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[typ][paczka/paleta]
            <asp:DropDownList ID="DDLTypUPS_DE_API" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="02">Moje opakowanie</asp:ListItem>
                <asp:ListItem Value="01">UPS Letter</asp:ListItem>
                <asp:ListItem Value="03">UPS Tube</asp:ListItem>
                <asp:ListItem Value="04">UPS PAK</asp:ListItem>
                <asp:ListItem Value="21">UPS Express Box</asp:ListItem>
                <asp:ListItem Value="30">UPS Paleta</asp:ListItem>
            </asp:DropDownList>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[ile_opak][szt]<asp:TextBox ID="TBIlePaczekUPS_DE_API" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell>
            &nbsp;[waga][kg]<asp:TextBox ID="TBWagaUPS_DE_API" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[szerokosc][cm]<asp:TextBox ID="TBSzerUPS_DE_API" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[wysokosc][cm]<asp:TextBox ID="TBWysUPS_DE_API" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
        <asp:TableCell>
            &nbsp;[dlugosc][cm]<asp:TextBox ID="TBDlugUPS_DE_API" runat="server" ValidationExpression="\d+"></asp:TextBox>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>

</asp:TableCell>
</asp:TableRow>
<asp:TableRow ID="TablePackageButton" runat="server">
    <asp:TableCell ColumnSpan="2" HorizontalAlign="Left">
        <asp:Button ID="BZapiszPaczke" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ZAPISZ PACZKE" Width="250px"/>
    </asp:TableCell>
    <asp:TableCell ColumnSpan="2" HorizontalAlign="Right">
        <asp:Button ID="BAnulujPaczek" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ANULUJ PACZKE" Width="250px"/>
    </asp:TableCell>
</asp:TableRow>
</asp:Table>
<asp:GridView ID="GridViewPaczki" runat="server"
              AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
    <HeaderStyle BackColor="#E5E5E5"/>
    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
    <Columns>
        <asp:TemplateField HeaderText="Select">
            <ItemTemplate>
                <asp:CheckBox ID="CBKodSelect" runat="server"/>
            </ItemTemplate>
            <HeaderTemplate>
            </HeaderTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:Table ID="Table9" runat="server" Width="100%">
    <asp:TableRow>
        <asp:TableCell>
            <asp:Button ID="BAnulowaniePaczkomat" runat="server" Text="ANULUJ PACZKOMAT" BackColor="Black" ForeColor="White"/>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
</asp:Panel>
<asp:Panel ID="Panel2" runat="server" Font-Size="Medium" GroupingText="II.Dane adresowe odbiorcy">
    <asp:Table ID="Table1" runat="server" Width="100%" BackColor="#CCCCCC">
        <asp:TableRow>
            <asp:TableCell Width="250px">[kraj odbiorcy]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SR_COUNTRY" runat="server" BackColor="#FFFF80" AutoPostBack="true">
                </asp:DropDownList>&nbsp;<asp:TextBox ID="TB_Country" runat="server" TextMode="SingleLine" Width="250px" Enabled="false"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                [opis zamowienia w systemie] :
                <asp:TextBox ID="TBOpisZam" runat="server" Height="50px" TextMode="MultiLine" Width="100%"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow HorizontalAlign="Right">
            <asp:TableCell ColumnSpan="3" HorizontalAlign="Right">
                <asp:Button ID="BKopiujDaneDostawy" runat="server" Text="KOPIUJ DANE DOSTAWY" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[dostarczenie paczkomat][packstation]</asp:TableCell>
            <asp:TableCell>
                <asp:CheckBox ID="CB_SR_IS_PACKSTATION" runat="server" Enabled="true" AutoPostBack="true"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[dostarczenie poczta][postfile]</asp:TableCell>
            <asp:TableCell>
                <asp:CheckBox ID="CB_SR_IS_POSTFILIALE" runat="server" Enabled="true" AutoPostBack="true"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[numer klienta / paczkomaty][packstation]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_POST_NUM" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" AutoPostBack="false"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[imie nazwisko / nazwa firmy]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_NAME" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_NAME_TEST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[nazwa firmy INPOST]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_COMPANY_INPOST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_COMPANY_INPOST_TEST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[imie INPOST]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_FIRSTNAME_INPOST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_FIRSTNAME_INPOST_TEST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[nazwisko INPOST]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_LASTNAME_INPOST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_LASTNAME_INPOST_TEST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[kod pocztowy][xxxxx]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_POSTAL_CODE" runat="server" TextMode="SingleLine" Width="100px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_POSTAL_CODE_TEST" runat="server" TextMode="SingleLine" Width="100px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[miejscowosc]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_CITY" runat="server" TextMode="SingleLine" Width="300px" MaxLength="50" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_CITY_TEST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="50" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[ulica]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_STREET" runat="server" TextMode="SingleLine" Width="300px" MaxLength="50" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_STREET_TEST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="50" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px" ColumnSpan="3">[walidacja pola ulicy 35znakow]<asp:CheckBox ID="CBWalidacjaUlicaUPS" runat="server" Enabled="true" AutoPostBack="true" Checked="false"/></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[numer domu]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_HOUSE_NUM" runat="server" TextMode="SingleLine" Width="100px" MaxLength="7" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_HOUSE_NUM_TEST" runat="server" TextMode="SingleLine" Width="100px" MaxLength="7" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[numer lokalu]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_APART_NUM" runat="server" TextMode="SingleLine" Width="100px" MaxLength="7"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SR_APART_NUM_TEST" runat="server" TextMode="SingleLine" Width="100px" MaxLength="7"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel8" runat="server" Font-Size="Medium" GroupingText="III.Dane do preawizacji / kontaktu">
    <asp:Table ID="Table5" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">[nazwa osoby kontaktowej]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_PC_PERSON_NAME" runat="server" TextMode="SingleLine" Width="300px" MaxLength="50" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_PC_PERSON_NAME_TEST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="50" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[numer telefonu]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_PC_PHONE_NUM" runat="server" TextMode="SingleLine" Width="300px" MaxLength="20" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_PC_PHONE_NUM_TEST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="20" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[adres email]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_PC_EMAIL_ADD" runat="server" TextMode="SingleLine" Width="300px" MaxLength="100" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_PC_EMAIL_ADD_TEST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="100" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[wylacz syfrowanie email]</asp:TableCell>
            <asp:TableCell ColumnSpan="2" HorizontalAlign="Left">
                <asp:CheckBox ID="CBEmailSzyfrowanie" runat="server" Enabled="true" AutoPostBack="true" Checked="false"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_DHL_PRZESYLKA" runat="server" Font-Size="Medium" GroupingText="IV.Podstawowe dane logistyczne" Visible="false">
    <asp:Table ID="Table2" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Rodzaj zadania</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_DROP_OFF" runat="server" AutoPostBack="true" BackColor="#FFFF80">
                    <asp:ListItem Value="REGULAR_PICKUP">UTWORZENIE PRZESYLKI</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Typ uslugi przewozowe</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SERVICE_TYPE" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="AH">Przesylka krajowa</asp:ListItem>
                    <asp:ListItem Value="EPN">DHL Paket [EPN]</asp:ListItem>
                    <asp:ListItem Value="BPI">DHL Weltpaket [BPI]</asp:ListItem>
                    <asp:ListItem Value="EK">Przesylka Connect</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Wybor etykiety zwrotnej</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_LABEL_TYPE" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="BLP" Selected="True">ETYKIETA BLP W FORMACIE PDF</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[zawartosc przesylki]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_CONTENT" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="koperta">koperta</asp:ListItem>
                    <asp:ListItem Value="agd szkło">agd szkło</asp:ListItem>
                    <asp:ListItem Value="poduszki">poduszki</asp:ListItem>
                    <asp:ListItem Value="ogród">ogród</asp:ListItem>
                    <asp:ListItem Value="paleta">paleta</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[komentarz]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_COMMENT_F" runat="server" TextMode="MultiLine" Width="100%" MaxLength="100" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_DHL_PS_PRZESYLKA" runat="server" Font-Size="Medium" GroupingText="IV.Podstawowe dane logistyczne DHL Parcelshop" Visible="false">
    <asp:Table ID="Table20" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Rodzaj zadania</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_DROP_OFF_DHL_PS" runat="server" AutoPostBack="true" BackColor="#FFFF80">
                    <asp:ListItem Value="REGULAR_PICKUP">UTWORZENIE PRZESYLKI</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Typ uslugi przewozowe</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SERVICE_TYPE_DHL_PS" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="LM">Automatu DHL Locker</asp:ListItem>
                    <asp:ListItem Value="SP">DHL ServicePoint</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Wybor etykiety zwrotnej</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_LABEL_TYPE_DHL_PS" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="BLP" Selected="True">ETYKIETA BLP W FORMACIE PDF</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[zawartosc przesylki]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_CONTENT_DHL_PS" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="koperta">koperta</asp:ListItem>
                    <asp:ListItem Value="agd szkło">agd szkło</asp:ListItem>
                    <asp:ListItem Value="poduszki">poduszki</asp:ListItem>
                    <asp:ListItem Value="ogród">ogród</asp:ListItem>
                    <asp:ListItem Value="paleta">paleta</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[komentarz]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_COMMENT_F_DHL_PS" runat="server" TextMode="MultiLine" Width="100%" MaxLength="100" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_DHL_DE_API_PRZESYLKA" runat="server" Font-Size="Medium" GroupingText="IV.Podstawowe dane logistyczne Parcel DE Shipping" Visible="false">
    <asp:Table ID="Table19" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Rodzaj zadania</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_DROP_OFF_DHL_DE_API" runat="server" AutoPostBack="true" BackColor="#FFFF80">
                    <asp:ListItem Value="REGULAR_PICKUP">UTWORZENIE PRZESYLKI</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Typ uslugi przewozowe</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SERVICE_TYPE_DHL_DE_API" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="V01PAK">DHL PAKET</asp:ListItem>
                    <asp:ListItem Value="V53WPAK">DHL PAKET International</asp:ListItem>
                    <asp:ListItem Value="V54EPAK">DHL Europaket</asp:ListItem>
                    <asp:ListItem Value="V62WP">Warenpost</asp:ListItem>
                    <asp:ListItem Value="V66WPI">Warenpost International</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Wybor etykiety zwrotnej</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_LABEL_TYPE_DHL_DE_API" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="BLP" Selected="True">ETYKIETA BLP W FORMACIE PDF</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[zawartosc przesylki]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_CONTENT_DHL_DE_API" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="koperta">koperta</asp:ListItem>
                    <asp:ListItem Value="agd szkło">agd szkło</asp:ListItem>
                    <asp:ListItem Value="poduszki">poduszki</asp:ListItem>
                    <asp:ListItem Value="ogród">ogród</asp:ListItem>
                    <asp:ListItem Value="paleta">paleta</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[komentarz]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_COMMENT_F_DHL_DE_API" runat="server" TextMode="MultiLine" Width="100%" MaxLength="100" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_UPS_DE_API_PRZESYLKA" runat="server" Font-Size="Medium" GroupingText="IV.Podstawowe dane logistyczne UPS Shipping" Visible="false">
    <asp:Table ID="Table25" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Rodzaj zadania</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_DROP_OFF_UPS_DE_API" runat="server" AutoPostBack="true" BackColor="#FFFF80">
                    <asp:ListItem Value="REGULAR_PICKUP">UTWORZENIE PRZESYLKI</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Typ uslugi przewozowe</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SERVICE_TYPE_UPS_DE_API" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="11">UPS Standard</asp:ListItem>
                    <asp:ListItem Value="07">UPS Express</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Wybor etykiety zwrotnej</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_LABEL_TYPE_UPS_DE_API" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="GIF" Selected="True">ETYKIETA W FORMACIE GIF</asp:ListItem>
                    <asp:ListItem Value="ZPL" Selected="false">ETYKIETA W FORMACIE ZPL</asp:ListItem>
                    <asp:ListItem Value="EPL" Selected="false">ETYKIETA W FORMACIE EPL</asp:ListItem>
                    <asp:ListItem Value="SPL" Selected="false">ETYKIETA W FORMACIE SPL</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[zawartosc przesylki]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_CONTENT_UPS_DE_API" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="koperta">koperta</asp:ListItem>
                    <asp:ListItem Value="agd szkło">agd szkło</asp:ListItem>
                    <asp:ListItem Value="poduszki">poduszki</asp:ListItem>
                    <asp:ListItem Value="ogród">ogród</asp:ListItem>
                    <asp:ListItem Value="paleta">paleta</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[komentarz]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_COMMENT_F_UPS_DE_API" runat="server" TextMode="MultiLine" Width="100%" MaxLength="100" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_GEIS_PRZESYLKA" runat="server" Font-Size="Medium" GroupingText="IV.Podstawowe dane logistyczne Geis" Visible="false">
    <asp:Table ID="Table11" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Rodzaj zadania</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_DROP_OFF_GEIS" runat="server" AutoPostBack="true" BackColor="#FFFF80">
                    <asp:ListItem Value="REGULAR_PICKUP">UTWORZENIE PRZESYLKI</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Typ uslugi przewozowe</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SERVICE_TYPE_GEIS" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="20">cargo ekspedycja [20]</asp:ListItem>
                    <asp:ListItem Value="21">cargo zamówienia [21]</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Wybor etykiety zwrotnej</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_LABEL_TYPE_GEIS" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="1" Selected="True">ETYKIETA PDF</asp:ListItem>
                    <asp:ListItem Value="2">ETYKIETA EPL</asp:ListItem>
                    <asp:ListItem Value="3">ETYKIETA ZPL</asp:ListItem>
                    <asp:ListItem Value="4">ETYKIETA BMP</asp:ListItem>
                    <asp:ListItem Value="5">ETYKIETA PDF(10x15)</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[zawartosc przesylki]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_CONTENT_GEIS" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="koperta">koperta</asp:ListItem>
                    <asp:ListItem Value="agd szkło">agd szkło</asp:ListItem>
                    <asp:ListItem Value="poduszki">poduszki</asp:ListItem>
                    <asp:ListItem Value="ogród">ogród</asp:ListItem>
                    <asp:ListItem Value="paleta">paleta</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[komentarz]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_COMMENT_F_GEIS" runat="server" TextMode="MultiLine" Width="100%" MaxLength="50" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_INPOST_PRZESYLKA" runat="server" Font-Size="Medium" GroupingText="IV.Podstawowe dane logistyczne Inpost" Visible="false">
    <asp:Table ID="Table16" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Rodzaj zadania</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_DROP_OFF_INPOST" runat="server" AutoPostBack="true" BackColor="#FFFF80">
                    <asp:ListItem Value="REGULAR_PICKUP">UTWORZENIE PRZESYLKI</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Typ uslugi przewozowe</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SERVICE_TYPE_INPOST" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="inpost_locker_standard">Przesyłka paczkomatowa - standardowa</asp:ListItem>
                    <asp:ListItem Value="inpost_locker_allegro">Przesyłka paczkomatowa - Allegro Paczkomaty InPost.</asp:ListItem>
                    <asp:ListItem Value="inpost_locker_pass_thru" Enabled="false">Przesyłka paczkomatowa - Podaj Dalej</asp:ListItem>
                    <asp:ListItem Value="inpost_letter_allegro" Enabled="false">Przesyłka kurierska - Allegro MiniKurier24 InPost</asp:ListItem>
                    <asp:ListItem Value="inpost_courier_allegro">Przesyłka kurierska - Allegro Kurier24 InPost.</asp:ListItem>
                    <asp:ListItem Value="inpost_courier_c2c" Enabled="false">Przesyłka kurierska - InPost Kurier C2C (usługa dla klienta detalicznego - prepaid)</asp:ListItem>
                    <asp:ListItem Value="inpost_courier_standard">Przesyłka kurierska standardowa</asp:ListItem>
                    <asp:ListItem Value="inpost_courier_express_1000" Enabled="false">Przesyłka kurierska z doręczeniem do 10:00</asp:ListItem>
                    <asp:ListItem Value="inpost_courier_express_1200" Enabled="false">Przesyłka kurierska z doręczeniem do 12:00</asp:ListItem>
                    <asp:ListItem Value="inpost_courier_express_1700" Enabled="false">Przesyłka kurierska z doręczeniem do 17:00</asp:ListItem>
                    <asp:ListItem Value="inpost_courier_palette">Przesyłka kurierska Paleta Standard</asp:ListItem>
                    <asp:ListItem Value="inpost_courier_local_standard" Enabled="false">Przesyłka kurierska Lokalna Standardowa</asp:ListItem>
                    <asp:ListItem Value="inpost_courier_local_express" Enabled="false">Przesyłka kurierska Lokalna Expresowa</asp:ListItem>
                    <asp:ListItem Value="inpost_courier_local_super_express" Enabled="false">Przesyłka kurierska Lokalna Super Expresowa</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Wybor etykiety zwrotnej</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_LABEL_TYPE_INPOST" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="pdf" Selected="True">ETYKIETA PDF</asp:ListItem>
                    <asp:ListItem Value="epl">ETYKIETA EPL</asp:ListItem>
                    <asp:ListItem Value="zpl">ETYKIETA ZPL</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[zawartosc przesylki]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_CONTENT_INPOST" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="koperta">koperta</asp:ListItem>
                    <asp:ListItem Value="agd szkło">agd szkło</asp:ListItem>
                    <asp:ListItem Value="poduszki">poduszki</asp:ListItem>
                    <asp:ListItem Value="ogród">ogród</asp:ListItem>
                    <asp:ListItem Value="paleta">paleta</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[komentarz]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_COMMENT_F_INPOST" runat="server" TextMode="MultiLine" Width="100%" MaxLength="50" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_DHL_SS_SERVICE_TYPE" runat="server" Font-Size="Medium" GroupingText="V.Zamawianie uslug dodatkowych DHL" Visible="false">
    <asp:Table ID="Table4" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Usluga dodatkowa</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SS_SERVICE_TYPE" runat="server">
                    <asp:ListItem Value="1722">Dorecznie w godz. 18-22</asp:ListItem>
                    <asp:ListItem Value="SOBOTA">Dorecznie w sobote</asp:ListItem>
                    <asp:ListItem Value="NAD_SOBOTA">Nadanie w sobote</asp:ListItem>
                    <asp:ListItem Value="UBEZP">Ubezpiecznie przesylki</asp:ListItem>
                    <asp:ListItem Value="COD">Zwrot pobrania</asp:ListItem>
                    <asp:ListItem Value="PDI">Informacje przed doreczeniem</asp:ListItem>
                    <asp:ListItem Value="ROD">Zwrot potwierdzonych dokumentow</asp:ListItem>
                    <asp:ListItem Value="POD">Potwierdzenie doreczenia</asp:ListItem>
                    <asp:ListItem Value="SAS">Doreczenie do sasiada</asp:ListItem>
                    <asp:ListItem Value="ODB">Odbior wlasny</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="BDodaj_SS_Service_type" runat="server" Text="DODAJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[deklarowana wartosc]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SS_SERVICE_VALUE" runat="server" TextMode="SingleLine" Width="100px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[forma zwrotu pobrania]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SS_COLL_ON_FORM" runat="server">
                    <asp:ListItem Value="CASH">PLATNOSC GOTOWKA</asp:ListItem>
                    <asp:ListItem Value="BANK_TRANSFER" Selected="True">PRZELEW</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
                <asp:GridView ID="GridView_SS_Service_type" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
                    <HeaderStyle BackColor="#E5E5E5"/>
                    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="CBKodSelect" runat="server"/>
                            </ItemTemplate>
                            <HeaderTemplate>
                            </HeaderTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Button ID="BAnuluj_SS_Service_type" runat="server" Text="ANULUJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_DHL_PS_SS_SERVICE_TYPE" runat="server" Font-Size="Medium" GroupingText="V.Zamawianie uslug dodatkowych DHL POP" Visible="false">
    <asp:Table ID="Table21" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Usluga dodatkowa</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SS_SERVICE_TYPE_DHL_PS" runat="server">
                    <asp:ListItem Value="1722">Dorecznie w godz. 18-22</asp:ListItem>
                    <asp:ListItem Value="SOBOTA">Dorecznie w sobote</asp:ListItem>
                    <asp:ListItem Value="NAD_SOBOTA">Nadanie w sobote</asp:ListItem>
                    <asp:ListItem Value="UBEZP">Ubezpiecznie przesylki</asp:ListItem>
                    <asp:ListItem Value="COD">Zwrot pobrania</asp:ListItem>
                    <asp:ListItem Value="PDI">Informacje przed doreczeniem</asp:ListItem>
                    <asp:ListItem Value="ROD">Zwrot potwierdzonych dokumentow</asp:ListItem>
                    <asp:ListItem Value="POD">Potwierdzenie doreczenia</asp:ListItem>
                    <asp:ListItem Value="SAS">Doreczenie do sasiada</asp:ListItem>
                    <asp:ListItem Value="ODB">Odbior wlasny</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="BDodaj_SS_Service_type_DHL_PS" runat="server" Text="DODAJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[deklarowana wartosc]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SS_SERVICE_VALUE_DHL_PS" runat="server" TextMode="SingleLine" Width="100px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[forma zwrotu pobrania]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SS_COLL_ON_FORM_DHL_PS" runat="server">
                    <asp:ListItem Value="CASH">PLATNOSC GOTOWKA</asp:ListItem>
                    <asp:ListItem Value="BANK_TRANSFER" Selected="True">PRZELEW</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
                <asp:GridView ID="GridView_SS_Service_type_DHL_PS" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
                    <HeaderStyle BackColor="#E5E5E5"/>
                    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="CBKodSelect" runat="server"/>
                            </ItemTemplate>
                            <HeaderTemplate>
                            </HeaderTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Button ID="BAnuluj_SS_Service_type_DHL_PS" runat="server" Text="ANULUJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_DHL_DE_API_SS_SERVICE_TYPE" runat="server" Font-Size="Medium" GroupingText="V.Zamawianie uslug dodatkowych Parcel DE Shipping" Visible="false">
    <asp:Table ID="Table22" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Usluga dodatkowa</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SS_SERVICE_TYPE_DHL_DE_API" runat="server">
                    <asp:ListItem Value="1722">Dorecznie w godz. 18-22</asp:ListItem>
                    <asp:ListItem Value="SOBOTA">Dorecznie w sobote</asp:ListItem>
                    <asp:ListItem Value="NAD_SOBOTA">Nadanie w sobote</asp:ListItem>
                    <asp:ListItem Value="UBEZP">Ubezpiecznie przesylki</asp:ListItem>
                    <asp:ListItem Value="COD">Zwrot pobrania</asp:ListItem>
                    <asp:ListItem Value="PDI">Informacje przed doreczeniem</asp:ListItem>
                    <asp:ListItem Value="ROD">Zwrot potwierdzonych dokumentow</asp:ListItem>
                    <asp:ListItem Value="POD">Potwierdzenie doreczenia</asp:ListItem>
                    <asp:ListItem Value="SAS">Doreczenie do sasiada</asp:ListItem>
                    <asp:ListItem Value="ODB">Odbior wlasny</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="BDodaj_SS_Service_type_DHL_DE_API" runat="server" Text="DODAJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[deklarowana wartosc]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SS_SERVICE_VALUE_DHL_DE_API" runat="server" TextMode="SingleLine" Width="100px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[forma zwrotu pobrania]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SS_COLL_ON_FORM_DHL_DE_API" runat="server">
                    <asp:ListItem Value="CASH">PLATNOSC GOTOWKA</asp:ListItem>
                    <asp:ListItem Value="BANK_TRANSFER" Selected="True">PRZELEW</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
                <asp:GridView ID="GridView_SS_Service_type_DHL_DE_API" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
                    <HeaderStyle BackColor="#E5E5E5"/>
                    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="CBKodSelect" runat="server"/>
                            </ItemTemplate>
                            <HeaderTemplate>
                            </HeaderTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Button ID="BAnuluj_SS_Service_type_DHL_DE_API" runat="server" Text="ANULUJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_UPS_DE_API_SS_SERVICE_TYPE" runat="server" Font-Size="Medium" GroupingText="V.Zamawianie uslug dodatkowych UPS Shipping" Visible="false">
    <asp:Table ID="Table26" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Usluga dodatkowa</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SS_SERVICE_TYPE_UPS_DE_API" runat="server">
                    <asp:ListItem Value="1722">Dorecznie w godz. 18-22</asp:ListItem>
                    <asp:ListItem Value="SOBOTA">Dorecznie w sobote</asp:ListItem>
                    <asp:ListItem Value="NAD_SOBOTA">Nadanie w sobote</asp:ListItem>
                    <asp:ListItem Value="UBEZP">Ubezpiecznie przesylki</asp:ListItem>
                    <asp:ListItem Value="COD">Zwrot pobrania</asp:ListItem>
                    <asp:ListItem Value="PDI">Informacje przed doreczeniem</asp:ListItem>
                    <asp:ListItem Value="ROD">Zwrot potwierdzonych dokumentow</asp:ListItem>
                    <asp:ListItem Value="POD">Potwierdzenie doreczenia</asp:ListItem>
                    <asp:ListItem Value="SAS">Doreczenie do sasiada</asp:ListItem>
                    <asp:ListItem Value="ODB">Odbior wlasny</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="BDodaj_SS_Service_type_UPS_DE_API" runat="server" Text="DODAJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[deklarowana wartosc]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SS_SERVICE_VALUE_UPS_DE_API" runat="server" TextMode="SingleLine" Width="100px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[forma zwrotu pobrania]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SS_COLL_ON_FORM_UPS_DE_API" runat="server">
                    <asp:ListItem Value="CASH">PLATNOSC GOTOWKA</asp:ListItem>
                    <asp:ListItem Value="BANK_TRANSFER" Selected="True">PRZELEW</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
                <asp:GridView ID="GridView_SS_Service_type_UPS_DE_API" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
                    <HeaderStyle BackColor="#E5E5E5"/>
                    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="CBKodSelect" runat="server"/>
                            </ItemTemplate>
                            <HeaderTemplate>
                            </HeaderTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Button ID="BAnuluj_SS_Service_type_UPS_DE_API" runat="server" Text="ANULUJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_GEIS_SS_SERVICE_TYPE" runat="server" Font-Size="Medium" GroupingText="V.Zamawianie uslug dodatkowych GEIS" Visible="false">
    <asp:Table ID="Table13" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Usluga dodatkowa</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SS_SERVICE_TYPE_GEIS" runat="server">
                    <asp:ListItem Value="COD">Przesyłka za pobraniem [COD]</asp:ListItem>
                    <asp:ListItem Value="VDL">Dowód dostawy z powrotem [VDL]</asp:ListItem>
                    <asp:ListItem Value="POJ">Deklaracja wartości [POJ]</asp:ListItem>
                    <asp:ListItem Value="EMA">Awizacja mailowa[EMA]</asp:ListItem>
                    <asp:ListItem Value="PHO">Awizacja/zawiadomienie telefoniczne[PHO]</asp:ListItem>
                    <asp:ListItem Value="B2C">Usluga B2C[B2C]</asp:ListItem>
                    <asp:ListItem Value="B2P">Usluga B2C Wniesienie[B2P]</asp:ListItem>
                    <asp:ListItem Value="FIX">Ustalony termin[FIX]</asp:ListItem>
                    <asp:ListItem Value="HDS">Home Delivery Standard[HDS]</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="BDodaj_SS_Service_type_GEIS" runat="server" Text="DODAJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[deklarowana wartosc]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SS_SERVICE_VALUE_GEIS" runat="server" TextMode="SingleLine" Width="250px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
                <asp:GridView ID="GridView_SS_Service_type_GEIS" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
                    <HeaderStyle BackColor="#E5E5E5"/>
                    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="CBKodSelect" runat="server"/>
                            </ItemTemplate>
                            <HeaderTemplate>
                            </HeaderTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Button ID="BAnuluj_SS_Service_type_GEIS" runat="server" Text="ANULUJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_INPOST_SS_SERVICE_TYPE" runat="server" Font-Size="Medium" GroupingText="V.Zamawianie uslug dodatkowych Inpost" Visible="false">
    <asp:Table ID="Table17" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell Width="250px">Usluga dodatkowa</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_SS_SERVICE_TYPE_INPOST" runat="server">
                    <asp:ListItem Value="insurance">Ubezpieczenie [insurance]</asp:ListItem>
                    <asp:ListItem Value="cod">Pobranie [cod]</asp:ListItem>
                    <asp:ListItem Value="sms">Serwis SMS [sms]</asp:ListItem>
                    <asp:ListItem Value="email">Serwis Email [email]</asp:ListItem>
                    <asp:ListItem Value="rod">Zwrot dokumentów [rod]</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="BDodaj_SS_Service_type_INPOST" runat="server" Text="DODAJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[deklarowana wartosc]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_SS_SERVICE_VALUE_INPOST" runat="server" TextMode="SingleLine" Width="250px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
                <asp:GridView ID="GridView_SS_Service_type_INPOST" runat="server" AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
                    <HeaderStyle BackColor="#E5E5E5"/>
                    <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="CBKodSelect" runat="server"/>
                            </ItemTemplate>
                            <HeaderTemplate>
                            </HeaderTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <asp:Button ID="BAnuluj_SS_Service_type_INPOST" runat="server" Text="ANULUJ USLUGE" BackColor="Black" ForeColor="White"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel10" runat="server" Font-Size="Medium" GroupingText="VI.Informacja o czasie nadania przesylki">
    <asp:Table ID="Table7" runat="server" Width="100%" BackColor="#CCCCCC">
        <asp:TableRow>
            <asp:TableCell Width="250px">[data nadania][RRRR-MM-DD]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_ST_SHIPMENT_DATE" runat="server" TextMode="SingleLine" Width="100px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[poczatek godzina][GG:MM]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_ST_SHIPMENT_START" runat="server" TextMode="SingleLine" Width="100px" MaxLength="32" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[koniec godzina][GG:MM]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_ST_SHIPMENT_END" runat="server" TextMode="SingleLine" Width="100px" MaxLength="32" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>

    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_DHL_B_PAYMENT" runat="server" Font-Size="Medium" GroupingText="VII.Informacja o platnosci" Visible="false">
    <asp:Table ID="Table3" runat="server" Width="100%" BackColor="#CCCCCC">
        <asp:TableRow>
            <asp:TableCell Width="250px">Strona obciazona kosztami</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_B_SHIP_PAYMENT_TYPE" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="SHIPPER" Selected="True">NADAWCA</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[numer obciazanego klienta]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_B_BILL_ACC_NUM" runat="server" TextMode="SingleLine" Width="300px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Sposob platnosci</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_B_PAYMENT_TYPE" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="BANK_TRANSFER" Selected="True">PRZELEW</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[miejsce powstawania kosztow]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_B_COSTS_CENTER" runat="server" TextMode="SingleLine" Width="300px" MaxLength="20"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_INPOST_B_PAYMENT" runat="server" Font-Size="Medium" GroupingText="VII.Informacja o platnosci INPOST" Visible="false">
    <asp:Table ID="Table18" runat="server" Width="100%" BackColor="#CCCCCC">
        <asp:TableRow>
            <asp:TableCell Width="250px">Strona obciazona kosztami</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_B_SHIP_PAYMENT_TYPE_INPOST" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="SHIPPER" Selected="True">NADAWCA</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[numer obciazanego klienta]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_B_BILL_ACC_NUM_INPOST" runat="server" TextMode="SingleLine" Width="300px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Sposob platnosci</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_B_PAYMENT_TYPE_INPOST" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="BANK_TRANSFER" Selected="True">PRZELEW</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[miejsce powstawania kosztow]</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_B_COSTS_CENTER_INPOST" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="ALLEGRO" Selected="True">ALLEGRO</asp:ListItem>
                    <asp:ListItem Value="INTERNET PL" Selected="false">INTERNET PL</asp:ListItem>
                    <asp:ListItem Value="PRZESYŁKA PRYWATNA" Selected="false">PRZESYŁKA PRYWATNA</asp:ListItem>
                    <asp:ListItem Value="TORUŃ MC" Selected="false">TORUŃ MC</asp:ListItem>
                    <asp:ListItem Value="MAGAZYN HIPO" Selected="false">MAGAZYN HIPO</asp:ListItem>
                    <asp:ListItem Value="REKLAMACJE" Selected="false">REKLAMACJE</asp:ListItem>
                    <asp:ListItem Value="DAJAR" Selected="false">DAJAR</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel_UPS_DE_API_B_PAYMENT" runat="server" Font-Size="Medium" GroupingText="VII.Informacja o platnosci UPS" Visible="false">
    <asp:Table ID="Table27" runat="server" Width="100%" BackColor="#CCCCCC">
        <asp:TableRow>
            <asp:TableCell Width="250px">Strona obciazona kosztami</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_B_SHIP_PAYMENT_TYPE_UPS_DE_API" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="SHIPPER" Selected="True">NADAWCA</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[numer obciazanego klienta]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_B_BILL_ACC_NUM_UPS_DE_API" runat="server" TextMode="SingleLine" Width="300px" BackColor="#FFFF80"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">Sposob platnosci</asp:TableCell>
            <asp:TableCell>
                <asp:DropDownList ID="DDL_B_PAYMENT_TYPE_UPS_DE_API" runat="server" BackColor="#FFFF80">
                    <asp:ListItem Value="BANK_TRANSFER" Selected="True">PRZELEW</asp:ListItem>
                </asp:DropDownList>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell Width="250px">[miejsce powstawania kosztow]</asp:TableCell>
            <asp:TableCell>
                <asp:TextBox ID="TB_B_COSTS_CENTER_UPS_DE_API" runat="server" TextMode="SingleLine" Width="300px" MaxLength="20"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
<asp:Panel ID="Panel" runat="server" Font-Size="Medium" GroupingText="">
    <asp:Table ID="Table6" runat="server" Width="100%">
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell HorizontalAlign="Left">
                <asp:Button ID="BZapiszEtykiete" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ZAPISZ ETYKIETE" Width="275px"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</asp:Panel>
</asp:Panel>
</asp:Panel>
</div>
</div>
</form>
</body>
</html>