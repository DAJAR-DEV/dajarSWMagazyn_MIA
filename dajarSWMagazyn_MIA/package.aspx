<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="package.aspx.vb" Inherits="dajarSWMagazyn_MIA.package" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Pakowanie - dajarSystemWspomaganiaMagazynu by MM</title>
    <meta name="format-detection" content="telephone=no"/>
    <link rel="apple-touch-icon" href="favicon.ico"/>
    <meta name="viewport" content="initial-scale=1.0, maximum-scale=5.0"/>
    <link rel="stylesheet" type="text/css" href="style.css"/>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
</head>

<body>
<script src="/scripts/jquery-3.6.3.min.js" type="text/javascript"></script>
<script src="/scripts/table-script.js" type="text/javascript"></script>
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
<div class="container">
<asp:Panel ID="panelMenu" runat="server">
    <div class="burger-menu-icon" onclick="toggleMenu()">&#9776;</div>
    <img src="Dajar_logo.png" alt="dajarSWMagazyn_BUR" class="dajar-logo"/>

    <hr/>

    <div class="navigation">
        <ul class="menu-list">
            <li>
                <a class="button-menu" href="login.aspx" title="Logowanie">LOGOWANIE</a>
            </li>
            <li>
                <a class="button-menu" href="address.aspx" title="Logowanie">ADRESY</a>
            </li>
            <li>
                <a class="button-menu" href="raport_stany.aspx" title="Logowanie">STAN</a>
            </li>
            <li>
                <a class="button-menu" href="storage.aspx" title="Logowanie">MAGAZYN</a>
            </li>
            <li>
                <a class="button-menu" href="package.aspx" title="Logowanie">PAKOWANIE</a>
            </li>
            <li>
                <a class="button-menu" href="logout.aspx" title="Logowanie">WYLOGOWANIE</a>
            </li>
        </ul>
    </div>

    <hr/>

    <div class="operator">
        OPERATOR: <asp:DropDownList ID="DDLOperator" runat="server" AutoPostBack="true"></asp:DropDownList>
    </div>

    <hr/>

    <div class="content">
        <div class="content-operator">
            <% Response.Write(Session("contentOperator"))%>
        </div>
        <div class="content-hash">
            <% Response.Write(Session("contentHash"))%>
        </div>
    </div>

    <hr/>

</asp:Panel>

<div style="text-align: left; width: 100%;">
<asp:Label runat="server" ID="labelKomunikat" ForeColor="Black" Font-Size="Large" Width="100%"><% Response.Write(Session("contentKomunikat"))%></asp:Label>
<br/>
<asp:Button ID="BRefreshPage" runat="server" Text="ODŚWIEŻ STRONĘ" BackColor="Black" Font-Size="X-Large" ForeColor="White" Font-Bold="True" Width="250px"/>
<br/>
<asp:TextBox ID="TBEtykieta" runat="server" Width="118px" Visible="true"></asp:TextBox>
<br/>
<asp:Panel ID="PanelPakowanie" runat="server" BorderColor="#333333" BorderStyle="None" BorderWidth="1px" Font-Size="Large" Width="100%">
    <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium">Lista dyspozycji oczekujących na spakowanie :</asp:Label>
    &nbsp;<asp:Label ID="LIleDokumentow" runat="server" Font-Bold="False" Font-Size="Medium"></asp:Label>
    <br/>
    
    <asp:Panel ID="PanelDodawaniePaczki" runat="server" Visible="false" BackColor="#DCDCDC">
    <div class="package-form-container">
    <h3>Wprowadzanie parametrów paczek</h3>
    <div class="package-row">
        <div class="package-row-item">
            [numer wybranego zamowienia]
            <asp:Label ID="LNrZamowienia" runat="server" ForeColor="Red"/>
        </div>
        <div class="package-row-item">
            <asp:Label ID="LNr_list_przewozowy" runat="server" ForeColor="Red"/>
            [numer listu przewozowego]
        </div>
    </div>
    
    <div class="package-row">
        <div class="package-row-item">
            [schemat wybranego zamowienia]
            <asp:Label ID="LSchemat" runat="server" ForeColor="Red"/>
        </div>
        <div class="package-row-item">
            <asp:HyperLink ID="HLEtykietaPDF_INPOST" runat="server" Target="_blank" ForeColor="Red">link PDF</asp:HyperLink>
            [link pdf]
        </div>
    </div>
    
    <div class="package-row">
        <div class="package-row-item">
            [identyfikator paczki]
            <asp:Label ID="LPaczkaID" runat="server" ForeColor="Red"/>
        </div>
    </div>
    
    <asp:Panel ID="TBROW_DDLFirma" runat="server" CssClass="package-row">
        <div class="package-row-item">
            [firma przewozowa]
            <asp:DropDownList ID="DDLFirma" runat="server" Font-Size="X-Large" Height="30px" Width="310px" AutoPostBack="false" Enabled="false"/>
        </div>
        <div class="package-row-item">
            <asp:Button ID="BGenerujEtykiete_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="GENERUJ OFERTE" Width="250px"/>
        </div>
        <div class="package-row-item">
            <asp:Button ID="BGenerujOferte_INPOST" runat="server" BackColor="gold" Font-Bold="true" Font-Size="X-Large" ForeColor="black" Text="OPLAC OFERTE" Width="250px"/>
        </div>
        <div class="package-row-item">
            <asp:Button ID="BPobierzPDF_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ETYKIETA PDF" Width="250px"/>
        </div>
        <div class="package-row-item">
            <asp:Button ID="BDrukowaniePDF_INPOST" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="DRUKOWANIE PDF" Width="250px"/>
        </div>
        <div class="package-row-item">
            <asp:Button ID="BZerowanie_INPOST" runat="server" BackColor="red" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ZEROWANIE LP" Width="250px"/>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="TBROW_SR_COUNTRY" CssClass="package-row" runat="server">
        <div class="package-row-item">
            [kraj odbiorcy]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SR_COUNTRY" runat="server" TextMode="SingleLine" Width="250px" Enabled="false">PL</asp:TextBox>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="TBROW_TB_ST_SHIPMENT_DATE" CssClass="package-row" runat="server">
        <div class="package-row-item">
            [data nadania][RRRR-MM-DD]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_ST_SHIPMENT_DATE" runat="server" TextMode="SingleLine" Width="100px" BackColor="#FFFF80" Enabled="false"/>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="TBROW_TB_B_COSTS_CENTER" CssClass="package-row" runat="server">
        <div class="package-row-item">
            [miejsce powstawania kosztow]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_B_COSTS_CENTER" runat="server" TextMode="SingleLine" Width="300px" MaxLength="20" Enabled="false"/>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="TBROW_SR_POST_NUM" CssClass="package-row" runat="server">
        <div class="package-row-item">
            [numer klienta / paczkomaty][packstation]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SR_POST_NUM" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" AutoPostBack="false" Enabled="false"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_SERVICE_TYPE_INPOST">
        <div class="package-row-item">
            [Typ uslugi przewozowej]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SERVICE_TYPE_INPOST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" AutoPostBack="false" Enabled="false"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_LABEL_TYPE_INPOST">
        <div class="package-row-item">
            [wybor etykiety zwrotnej]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_LABEL_TYPE_INPOST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" AutoPostBack="false" Enabled="false"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_SR_NAME">
        <div class="package-row-item">
            [imie nazwisko / nazwa firmy]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SR_NAME" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_SR_COMPANY_INPOST">
        <div class="package-row-item">
            [nazwa firmy INPOST]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SR_COMPANY_INPOST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_SR_FIRSTNAME_INPOST">
        <div class="package-row-item">
            [imie INPOST]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SR_FIRSTNAME_INPOST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_SR_LASTNAME_INPOST">
        <div class="package-row-item">
            [nazwisko INPOST]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SR_LASTNAME_INPOST" runat="server" TextMode="SingleLine" Width="300px" MaxLength="60" BackColor="#FFFF80"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_SR_POSTAL_CODE">
        <div class="package-row-item">
            [kod pocztowy][xxxxx]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SR_POSTAL_CODE" runat="server" TextMode="SingleLine" Width="100px" BackColor="#FFFF80"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_SR_CITY">
        <div class="package-row-item">
            [miejscowosc]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SR_CITY" runat="server" TextMode="SingleLine" Width="300px" MaxLength="50" BackColor="#FFFF80"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_SR_STREET">
        <div class="package-row-item">
            [ulica]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SR_STREET" runat="server" TextMode="SingleLine" Width="300px" MaxLength="50" BackColor="#FFFF80"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_SR_HOUSE_NUM">
        <div class="package-row-item">
            [numer domu]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SR_HOUSE_NUM" runat="server" TextMode="SingleLine" Width="100px" MaxLength="7" BackColor="#FFFF80"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_SR_APART_NUM">
        <div class="package-row-item">
            [numer lokalu]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_SR_APART_NUM" runat="server" TextMode="SingleLine" Width="100px" MaxLength="7"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_PC_PERSON_NAME">
        <div class="package-row-item">
            [nazwa osoby kontaktowej]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_PC_PERSON_NAME" runat="server" TextMode="SingleLine" Width="300px" MaxLength="50" BackColor="#FFFF80"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_PC_PHONE_NUM">
        <div class="package-row-item">
            [numer telefonu]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_PC_PHONE_NUM" runat="server" TextMode="SingleLine" Width="300px" MaxLength="20" BackColor="#FFFF80"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TBROW_PC_EMAIL_ADD">
        <div class="package-row-item">
            [adres email]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_PC_EMAIL_ADD" runat="server" TextMode="SingleLine" Width="300px" MaxLength="100" BackColor="#FFFF80"/>
        </div>
    </asp:Panel>
    
    <div class="package-row">
        <div class="package-row-item">
            [typ][paczka/paleta]
            <asp:DropDownList ID="DDLTyp" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="1">paczka</asp:ListItem>
                <asp:ListItem Value="2">paleta</asp:ListItem>
                <asp:ListItem Value="3">koperta</asp:ListItem>
                <asp:ListItem Value="7">paczka_poczta</asp:ListItem>
                <asp:ListItem Value="9">paczka_zagranica</asp:ListItem>
                <asp:ListItem Value="10">paczka_cieszyn</asp:ListItem>
            </asp:DropDownList>
        </div>
        <asp:Panel ID="TBCELL_RodzajINPOST_ALLEGRO" CssClass="package-row-item" runat="server">
            [rodzaj][cargo exp/cargo zam]
            <asp:DropDownList ID="DDLRodzajINPOST_ALLEGRO" runat="server" Font-Size="X-Large" Height="30px" Width="350px" AutoPostBack="true" OnSelectedIndexChanged="DDLRodzajINPOST_ALLEGRO_SelectedIndexChanged">
                <asp:ListItem Value="A">rozmiar 8 x 38 x 64 cm [A]</asp:ListItem>
                <asp:ListItem Value="B">rozmiar 19 x 38 x 64 cm [B]</asp:ListItem>
                <asp:ListItem Value="C">rozmiar 41 x 38 x 64 cm [C]</asp:ListItem>
                <asp:ListItem Value="RECZNIE">rozmiar 1 x 1 x 1 cm [RECZNIE]</asp:ListItem>
            </asp:DropDownList>
        </asp:Panel>
        <div class="package-row-item">
            [zawartosc przesylki]
            <asp:DropDownList ID="DDL_CONTENT" runat="server" Font-Size="X-Large" BackColor="#FFFF80">
                <asp:ListItem Value="agd szkło">agd szkło</asp:ListItem>
                <asp:ListItem Value="poduszki">poduszki</asp:ListItem>
                <asp:ListItem Value="ogród">ogród</asp:ListItem>
                <asp:ListItem Value="paleta">paleta</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="package-row-item">
            [rodzaj][std./niestd.]
            <asp:DropDownList ID="DDLRodzajStandard" runat="server" Font-Size="X-Large" Height="30px" Width="210px">
                <asp:ListItem Value="0">standard</asp:ListItem>
                <asp:ListItem Value="1">niestandard</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="package-row-item">
            [waga][kg]
            <asp:TextBox ID="TBWagaCOM" runat="server" Enabled="false" Width="50px"></asp:TextBox>
            <asp:TextBox ID="TBWaga" runat="server" Width="100px"></asp:TextBox>
        </div>
        <div class="package-row-item">
            [ile opak][szt]
            <asp:TextBox ID="TBIlePaczek" runat="server" Width="50px">1</asp:TextBox>
        </div>
    </div>
    
    <asp:Panel CssClass="package-row" runat="server" ID="WymiarySzczegoloweINPOST" Visible="False">
        <div class="package-row-item">
            [szerokosc][cm]<asp:TextBox ID="TBSzerINPOST_ALLEGRO" runat="server" ValidationExpression="\d+"/>
        </div>
        <div class="package-row-item">
            [wysokosc][cm]<asp:TextBox ID="TBWysINPOST_ALLEGRO" runat="server" ValidationExpression="\d+"/>
        </div>
        <div class="package-row-item">
            [dlugosc][cm]<asp:TextBox ID="TBDlugINPOST_ALLEGRO" runat="server" ValidationExpression="\d+"/>
        </div>
    </asp:Panel>
    
    <div class="package-row">
        <div class="package-row-item">
            [komentarz]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_COMMENT_F_INPOST" runat="server" TextMode="MultiLine" Width="100%" MaxLength="100" BackColor="#FFFF80"/>
        </div>
    </div>
    
    <asp:Panel CssClass="package-row" runat="server" ID="UslugaCodINPOST" Visible="False">
        <div class="package-row-item">
            [kwota pobrania]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TB_COD_INPOST" runat="server" TextMode="SingleLine" Width="100%" MaxLength="100" BackColor="#FFFF80" Enabled="false"/>
        </div>
    </asp:Panel>
    
    <div class="package-row">
        <div class="package-row-item">
            [opis zamowienia]
        </div>
        <div class="package-row-item">
            <asp:TextBox ID="TBOpisZam" runat="server" Height="50px" TextMode="MultiLine" Width="100%" Enabled="false" Visible="false"/>
        </div>
    </div>
    
    <asp:Panel CssClass="package-row" runat="server" Visible="False">
        <div class="package-row-item">
            <asp:TextBox ID="TBProcessDirectory" runat="server" Width="100%" Height="25"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" Visible="False">
        <div class="package-row-item">
            <asp:TextBox ID="TBProcessOutput" runat="server" TextMode="MultiLine" Width="100%" Height="100"/>
        </div>
    </asp:Panel>
    
    <asp:Panel CssClass="package-row" runat="server" ID="TablePackageButton">
        <div class="package-row-item">
            <asp:Button ID="BDodajPaczke" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ZAPISZ PACZKE" Width="300px"/>
        </div>
        <div class="package-row-item">
            <asp:Button ID="BAnulujPaczek" runat="server" BackColor="Black" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="ANULUJ PACZKE" Width="300px"/>
        </div>
        <div class="package-row-item">
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="" Visible="false">HyperLink</asp:HyperLink>
            <asp:Button ID="BPobierzWage" runat="server" BackColor="Red" Font-Bold="true" Font-Size="X-Large" ForeColor="White" Text="POBIERZ WAGE" Width="300px" Enabled="true"/>
        </div>
    </asp:Panel>
    
    
    </div>
    <div class="table-responsive">
        <asp:GridView ID="GridViewPaczki" runat="server"
                      AutoGenerateSelectButton="True" Font-Size="Medium" Width="100%">
            <HeaderStyle BackColor="#E5E5E5"></HeaderStyle>
            <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"></SelectedRowStyle>
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
    </div>

</asp:Panel>

<div class="table-mobile">
    <asp:GridView ID="GridViewIndeksy" runat="server" Font-Size="Medium" Width="100%">
        <HeaderStyle BackColor="#E5E5E5"></HeaderStyle>
    </asp:GridView>
</div>


<br/><br/>
<asp:Table ID="Table2" runat="server" Width="100%">
    <asp:TableRow>
        <asp:TableCell HorizontalAlign="Left">
            <asp:Button ID="BZakonczPakowanie" runat="server" Text="ZAKOŃCZ PAKOWANIE" BackColor="#FFCC00" Font-Bold="True" Font-Size="X-Large" Width="300px"/>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
<br/>
<asp:Panel ID="PanelPakowanieData" runat="server">
    <br/>
    <div class="table-mobile">
        <asp:GridView ID="GridViewPakowanie" runat="server" AllowPaging="True"
                      CellPadding="5" Font-Size="Large"
                      OnPageIndexChanging="GridViewPakowanie_PageIndexChanging"
                      OnRowDataBound="GridViewPakowanie_RowDataBound" Width="100%" PageSize="1000">
            <PagerSettings Position="TopAndBottom"></PagerSettings>
            <RowStyle BackColor="#EFF3FB"></RowStyle>
            <FooterStyle BackColor="#CCCC99" ForeColor="Black"></FooterStyle>
            <PagerStyle BackColor="FloralWhite" CssClass="paging" Font-Size="X-Large"
                        ForeColor="Black" HorizontalAlign="Right">
            </PagerStyle>
            <SelectedRowStyle BackColor="LightBlue" BorderStyle="Solid" BorderWidth="5px"
                              Font-Bold="True" ForeColor="black">
            </SelectedRowStyle>
            <HeaderStyle BackColor="#333333" Font-Bold="True" ForeColor="White"></HeaderStyle>
            <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
            <Columns>
                <asp:TemplateField HeaderText="Select" Visible="False">
                    <ItemTemplate>
                        <asp:CheckBox ID="CBKodSelect" runat="server"/>
                    </ItemTemplate>
                    <HeaderTemplate>
                    </HeaderTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Mock" Visible="False">

                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div class="legend">
        <asp:Label ID="Ldajar" runat="server" BackColor="Khaki" Text="&nbsp;zamówienia dajar&nbsp;"></asp:Label>
        <asp:Label ID="Ldominus" runat="server" BackColor="Thistle" ForeColor="black" Text="&nbsp;zamówienia dominus&nbsp;"></asp:Label>
        <asp:Label ID="LPodjete" runat="server" BackColor="gold" ForeColor="black" Text="&nbsp;w trakcie pakowania&nbsp;"></asp:Label>
    </div>
</asp:Panel>
</asp:Panel>
</div>
</div>
</form>
</body>
</html>