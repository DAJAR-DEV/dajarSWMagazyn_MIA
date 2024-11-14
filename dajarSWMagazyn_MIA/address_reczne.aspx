<%@ Page Language="VB" AutoEventWireup="false" CodeBehind="address_reczne.aspx.vb" Inherits="dajarSWMagazyn_MIA.address_reczne" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Adresy - dajarSystemWspomaganiaMagazynu by MM</title>
    <meta name="format-detection" content="telephone=no"/>
    <link rel="apple-touch-icon" href="favicon.ico"/>
    <meta name="viewport" content="initial-scale=1.0, maximum-scale=5.0"/>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="style.css"/>
    <script src="Scripts/table-script.js" type="text/javascript"></script>
</head>

<body>
<form id="formBody" runat="server">

<asp:ScriptManager ID="ScriptManager1" runat="server"/>
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
    <asp:Panel ID="PanelMagazyn" runat="server" BorderColor="#333333" BorderStyle="None" BorderWidth="1px" Font-Size="Large" Width="100%">
        <asp:label ID="Label10" runat="server" BackColor="WhiteSmoke" Font-Bold="True" ForeColor="Red">ZAKŁADKA PRZYJĘCIE RĘCZNE TOWARU</asp:label>
        <br/><br/>
        <br/>
        <asp:Panel ID="Panel3" runat="server" GroupingText="Wprowadzenie towaru na magazyn" BackColor="#EFF3FB">
            <asp:Table ID="Table3" runat="server" Width="100%">
                <asp:TableRow>
                    <asp:TableCell Width="25%">adresacja magazynu *</asp:TableCell>
                    <asp:TableCell Width="75%">
                        <asp:TextBox ID="TBAdresacja" runat="server" AutoPostBack="true" onfocus="this.select()"/>
                        &nbsp;<asp:DropDownList ID="DDLAdresacjaTowary" runat="server" AutoPostBack="true"/>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell Width="25%">skrot towaru / kod ean *</asp:TableCell>
                    <asp:TableCell Width="75%">
                        <asp:TextBox ID="TBSkrot" runat="server" AutoPostBack="true" onfocus="this.select()"/>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>indeks towaru</asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="TBIndeks" runat="server" Enabled="false"/>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>prefix towaru</asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="TBPrefix" runat="server" Enabled="false"/>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>kod ean</asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="TBEan" runat="server" Enabled="false"/>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>stan towaru [mag 700]</asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="TBStan700" runat="server" Enabled="false"/>
                    </asp:TableCell>
                </asp:TableRow>
                <asp:TableRow>
                    <asp:TableCell>nazwa towaru</asp:TableCell>
                    <asp:TableCell>
                        <asp:TextBox ID="TBNazwa" runat="server" Width="100%" Enabled="false"/>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <br/>
            <asp:Table ID="Table5" runat="server" Width="100%">
                <asp:TableRow>
                    <asp:TableCell HorizontalAlign="Left">
                        <asp:Button ID="BTowarWyszukaj" runat="server" Text="WYSZUKAJ TOWAR" BackColor="#FFCC00" Font-Bold="True" Font-Size="X-Large" Width="300px"/>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <br/>
        </asp:Panel>
        <br/>
        <asp:Panel ID="PanelMagazynData" runat="server">
            <asp:Panel ID="Panel2" runat="server" GroupingText="Reczny wybór miejsca zamagazynowania" BackColor="#EFF3FB">
                <div class="address-reczne-address-controls">
                    <div class="address-reczne-address-controls-input">
                        hala *
                        <asp:DropDownList ID="DDLHala" runat="server" AutoPostBack="true"/>
                    </div>
                    <div class="address-reczne-address-controls-input">
                        rzad *
                        <asp:DropDownList ID="DDLRzad" runat="server" AutoPostBack="true"/>
                    </div>
                    <div class="address-reczne-address-controls-input">
                        regal *
                        <asp:DropDownList ID="DDLRegal" runat="server" AutoPostBack="true"/>
                    </div>
                    <div class="address-reczne-address-controls-input">
                        polka *
                        <asp:DropDownList ID="DDLPolka" runat="server"/>
                    </div>
                    <div class="address-reczne-address-controls-input">
                        &nbsp;
                        <asp:DropDownList ID="DDLStrefaMagazyn" runat="server">
                            <asp:ListItem Text="PALETA" Value="P"></asp:ListItem>
                            <asp:ListItem Text="NIEPEŁNA PALETA" Value="N"></asp:ListItem>
                            <asp:ListItem Text="ZBIERANIE" Value="Z" Selected="True"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </asp:Panel>
            <br/>
            <asp:Panel id="Panel1" runat="server">
                <div class="address-reczne-controls">
                    <asp:Button ID="BDodajTowar" runat="server" Text="DODAJ TOWAR" BackColor="Yellow" Font-Bold="True" Font-Size="X-Large" Width="300px"/>
                    <asp:Button ID="BUsunTowar" runat="server" Text="USUŃ TOWAR" BackColor="#FFCC00" Font-Bold="True" Font-Size="X-Large" Width="300px"/>
                </div>
            </asp:Panel>
        </asp:Panel>
        <br/>
        Historia zatowarowania na magazynie
        <br/>
        <div class="table-responsive">
            <asp:GridView ID="GridViewAdresacja" runat="server"
                          CellPadding="5" Font-Size="Medium"
                          OnRowDataBound="GridViewAdresacja_RowDataBound" Width="100%"
                          OnPageIndexChanging="GridViewAdresacja_PageIndexChanging"
                          OnSelectedIndexChanged="GridViewIndeksy_SelectedIndexChanged"
                          AllowPaging="True" PageSize="100">
                <PagerSettings Position="TopAndBottom" Mode="NumericFirstLast"
                               PageButtonCount="5">
                </PagerSettings>
                <RowStyle BackColor="#EFF3FB"></RowStyle>
                <FooterStyle BackColor="#CCCC99" ForeColor="Black"></FooterStyle>
                <PagerStyle BackColor="FloralWhite" ForeColor="Black" Font-Size="X-Large" HorizontalAlign="Right" CssClass="paging"></PagerStyle>
                <SelectedRowStyle BackColor="LightBlue" Font-Bold="True"></SelectedRowStyle>
                <HeaderStyle BackColor="#333333" Font-Bold="True" ForeColor="White"></HeaderStyle>
                <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
                <Columns>
                    <asp:TemplateField HeaderText="Select" Visible="False">
                        <ItemTemplate>
                            <asp:CheckBox ID="CBKodSelect" runat="server"/>
                        </ItemTemplate>
                        <HeaderTemplate>
                            SELECT
                        </HeaderTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <br/>
        <asp:Table ID="Table2" runat="server" Width="100%">
            <asp:TableRow>
                <asp:TableCell HorizontalAlign="Left">
                    <asp:Label ID="LTowarPusty" runat="server" BackColor="orangered" Text="&nbsp;towary niezamagazynowane&nbsp;"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LTowarZbieranie" runat="server" BackColor="Khaki" Text="&nbsp;strefa zbierania&nbsp;"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LTowarNiepelna" runat="server" BackColor="lightgray" ForeColor="black" Text="&nbsp;strefa nadmiar niepełna paleta&nbsp;"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LTowarPaleta" runat="server" BackColor="YellowGreen" ForeColor="black" Text="&nbsp;strefa nadmiar paleta&nbsp;"></asp:Label>
                    &nbsp;&nbsp;
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br/>

    </asp:Panel>
</div>
</div>
</form>
</body>
</html>