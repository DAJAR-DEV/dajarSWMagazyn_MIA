<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="logged.aspx.vb" Inherits="dajarSWMagazyn_MIA.logged" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Karta operatora - dajarSystemWspomaganiaMagazynu by MM</title>
    <meta name="format-detection" content="telephone=no"/>
    <link rel="apple-touch-icon" href="favicon.ico"/>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="style.css"/>
    <meta name="viewport" content="initial-scale=1.0, maximum-scale=5.0"/>
    <script src="Scripts/table-script.js" type="text/javascript"></script>
</head>

<body>
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
        </asp:Panel>

        <div style="text-align: left; width: 100%;">
            <asp:Label runat="server" ID="labelKomunikat" ForeColor="Black" Font-Size="Large" Width="100%"><% Response.Write(Session("contentKomunikat"))%></asp:Label>
            <br/>
            <asp:Panel ID="Panel2" runat="server" BorderColor="#333333" BorderStyle="None" BorderWidth="1px" Font-Size="Medium" Width="100%">
                <asp:Button ID="BRefreshPage" runat="server" Text="ODŚWIEŻ STRONĘ" BackColor="Black" Font-Size="X-Large" ForeColor="White" Font-Bold="True" Width="250px"/>
                <p style="text-align:right">
                    Twoje logowanie w systemie dajarSWMagazyn_BUR przebiegło pomyślnie.<br/>
                    Skorzystaj z menu głównego na górze ekranu w celu zarządzania aplikacją.<br/>
                </p>
                <p style="text-align:left">
                    <asp:Label ID="Label1" runat="server" Font-Bold="True" Font-Size="Medium">Lista podjętych partii zamówień :</asp:Label>
                    &nbsp;<asp:Label ID="LIleDokumentow" runat="server" Text="Label"></asp:Label><br/><br/>
                    <div class="table-responsive">
                        <asp:GridView ID="GridViewPartie" runat="server" CellPadding="5" Font-Size="Medium"
                                      OnRowDataBound="GridViewPartie_aktualizacja" AllowPaging="True"
                                      OnPageIndexChanging="GridViewPartie_PageIndexChanging" Width="100%" PageSize="20">
                            <PagerSettings Position="TopAndBottom"></PagerSettings>
                            <PagerStyle BackColor="FloralWhite" ForeColor="Black" Font-Size="X-Large" HorizontalAlign="Right" CssClass="paging"></PagerStyle>
                            <HeaderStyle BackColor="#E5E5E5"></HeaderStyle>
                        </asp:GridView>
                    </div>
                </p>
                <p style="text-align:left">
                    <asp:Label ID="LMagazyn" runat="server" BackColor="Khaki" Text="MG - na magazynie"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LPakowanie" runat="server" BackColor="LightGray" Text="PA - przygotowane do pakowania"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LWstrzymane" runat="server" BackColor="OrangeRed" Text="ZW - wstrzymane"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LBlokadaNaHippo" runat="server" BackColor="peru" Text="HB - blokada na hippo"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LBlokadaNaBurek" runat="server" BackColor="sienna" Text="BB - blokada na burek"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LWznowione" runat="server" BackColor="LimeGreen" Text="WN - wznowione"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LPodjete" runat="server" BackColor="Gold" Text="ID_ET - w trakcie pakowania"></asp:Label>
                </p>
            </asp:Panel>
        </div>
    </div>
</form>
</body>
</html>