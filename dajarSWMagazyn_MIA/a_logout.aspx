﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="a_logout.aspx.vb" Inherits="dajarSWMagazyn_MIA.a_logout" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Wylogowanie - dajarSystemWspomaganiaMagazynu by MM</title>
    <meta name="format-detection" content="telephone=no"/>
    <link rel="apple-touch-icon" href="favicon.ico"/>
    <meta name="viewport" content="initial-scale=1.0, maximum-scale=5.0"/>
    <style type="text/css">
        .button-menu{font-size:18px;font-family:tahoma;font-weight:bold;width:192px;height:35px;text-align:center;-moz-border-radius-bottomleft:5px;-moz-border-radius-bottomright:5px;-moz-border-radius-topleft:5px;-moz-border-radius-topright:5px;-moz-box-shadow:0 1px 3px rgba(0,0,0,0.25);text-transform:uppercase;background:#000000;border:1px solid #777;border-bottom:2px solid rgba(0,0,0,0.25);color:#FFF!important;cursor:pointer;display:inline-table;line-height:1;overflow:visible;padding:8px 7px;position:relative;text-decoration:none;text-shadow:0 -1px 1px rgba(0,0,0,0.25);margin-top:7px;float:left;}
    </style>
</head>

<body>
<form id="formBody" runat="server">
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
                        <a class="button-menu" href="a_user.aspx" title="logowanie">UŻYTKOWNICY</a>
                    </td>
                    <td>
                        <a class="button-menu" href="a_manage.aspx" title="magazyn">DYSPOZYCJE</a>
                    </td>
                    <td>
                        <a class="button-menu" href="a_history.aspx" title="pakowanie">HISTORIA</a>
                    </td>
                    <td>
                        <a class="button-menu" href="a_print.aspx" title="pakowanie">WYDRUKI</a>
                    </td>
                </tr>
            </table>
            <hr style="width:1260px" align="left"/>
            <table style="margin: 10px auto 10px 0;width:1260px" border="0" cellpadding="0" cellspacing="0">
                <tr valign="middle" align="justify" bgcolor="#FFFFCC" style="vertical-align: middle">
                    <td>OPERATOR : <% Response.Write(Session("mylogin"))%></td>
                    <td align="right">MAGAZYN : <%Response.Write(Session("contentMagazyn"))%></td>
                </tr>
            </table>
            <hr style="width:1260px" align="left"/>
            <table style="margin: 10px auto 10px 0;width:100%" border="0" cellpadding="0" cellspacing="0">
                <tr style="height:25px;">
                    <td style="text-align: left; color: #000000; font-weight: bold; font-size: small;" bgcolor="#FFD501">
                        <%                        Response.Write(Session("contentOperator"))%>&nbsp;
                    </td>
                    <td style="text-align: right; color: #000000; font-weight: bold; font-size: small;" bgcolor="#FFD501">
                        <%                        Response.Write(Session("contentHash"))%>&nbsp;
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <div style="text-align: center; width: 100%;">
            <asp:Label runat="server" ID="labelKomunikat" ForeColor="Black" Font-Size="Large" Width="100%"><% Response.Write(Session("contentKomunikat"))%></asp:Label>
            <br/><br/>
            <asp:Panel ID="Panel2" runat="server" BorderColor="#333333" BorderStyle="None" BorderWidth="1px" Font-Size="Large" Width="100%">
                <table cellpadding="2" cellspacing="2" border="0" frame="hsides" style="width: 500px; height: 143px;">
                    <tr>
                        <td align="right" bgcolor="#EFF3FB">Aktualnie zalogowany operator</td>
                        <td bgcolor="#EFF3FB" align="left">
                            <asp:TextBox id="TBUsername" TextMode="SingleLine" runat="server"
                                         Font-Size="Medium" ReadOnly="True" Width="200px">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" bgcolor="#EFF3FB">Hasło</td>
                        <td bgcolor="#EFF3FB" align="left">
                            <asp:TextBox ID="TBPassword" TextMode="Password" runat="server" Font-Size="Medium"
                                         Width="200px">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td align="right" bgcolor="#EFF3FB">Rodzaj</td>
                        <td bgcolor="#EFF3FB" align="left">
                            <asp:DropDownList ID="DDLTypOperatora" runat="server" Font-Size="X-Large"
                                              Enabled="False" Width="200px">
                                <asp:ListItem Value="M">MAGAZYN</asp:ListItem>
                                <asp:ListItem Value="P">PAKOWANIE</asp:ListItem>
                                <asp:ListItem Value="O">OGRÓD</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="center">
                            <asp:Button ID="btnWyloguj" BackColor="Black" ForeColor="White"
                                        runat="server" Text="WYLOGUJ" CssClass="button-application"
                                        style="top: -5px; left: 30px; width: 220px" Font-Size="Large" Height="45px"/>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
    </div>
</form>
</body>
</html>