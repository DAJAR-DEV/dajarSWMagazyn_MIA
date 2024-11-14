<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="login.aspx.vb" Inherits="dajarSWMagazyn_MIA.login" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Logowanie - dajarSystemWspomaganiaMagazynu by MM</title>
    <meta name="format-detection" content="telephone=no"/>
    <link rel="apple-touch-icon" href="favicon.ico"/>
    <meta name="viewport" content="initial-scale=1.0, maximum-scale=5.0"/>
    <link rel="stylesheet" type="text/css" href="style.css"/>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
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

            <hr/>

        </asp:Panel>

        <asp:Label runat="server" ID="labelKomunikat" ForeColor="Black" Font-Size="Large" Width="100%"><% Response.Write(Session("contentKomunikat"))%></asp:Label>

        <asp:Panel ID="Panel2" runat="server" BorderColor="#333333" BorderStyle="None" BorderWidth="1px" Font-Size="Large" Width="100%" HorizontalAlign="NotSet">
            <hr/>
            <div class="login">
                <div class="input">
                    Login: <asp:DropDownList ID="DDLOperatorLogowanie" runat="server" AutoPostBack="true" CssClass="inputField"/>
                </div>
                <div class="input">
                    Hasło: <asp:TextBox ID="TBPassword" runat="server" TextMode="Password"/>
                </div>
                <div class="input">
                    Rodzaj:
                    <asp:DropDownList ID="DDLTypOperatora" runat="server">
                        <asp:ListItem Value="M">MAGAZYN</asp:ListItem>
                        <asp:ListItem Value="O" Enabled="false">OGRÓD</asp:ListItem>
                        <asp:ListItem Value="W">WÓZEK</asp:ListItem>
                        <asp:ListItem Value="P">PAKOWANIE</asp:ListItem>
                        <asp:ListItem Value="MO" Enabled="false">MAGAZYN-OGRÓD</asp:ListItem>
                        <asp:ListItem Value="MP" Enabled="false">MAGAZYN-PODUSZKI</asp:ListItem>
                        <asp:ListItem Value="PM" Enabled="false">MAGAZYN-PACZKOMAT</asp:ListItem>
                        <asp:ListItem Value="PP" Enabled="false">PAKOWANIE-PACZKOMAT</asp:ListItem>
                        <asp:ListItem Value="RM" Enabled="false">REKLAMACJA-MAGAZYN</asp:ListItem>
                        <asp:ListItem Value="ME" Enabled="false">MAGAZYN-EXPORT</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="input">
                    <asp:Button ID="btnZaloguj" runat="server" Text="ZALOGUJ" OnClick="Login_Click" CssClass="button"/>
                </div>
            </div>
            <hr/>
        </asp:Panel>
    </div>
</form>
</body>
</html>