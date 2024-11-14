<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="a_userAdd.aspx.vb" Inherits="dajarSWMagazyn_MIA.userAdd" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Uzytkownicy dodawanie - dajarSystemWspomaganiaMagazynu by MM</title>
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
                        <a class="button-menu-bottom" href="a_user.aspx" title="uzytkownicy">UŻYTKOWNICY</a>
                        <a class="button-menu-bottom" href="a_adresy.aspx" title="adresy">ADRESY</a>
                    </td>
                    <td>
                        <a class="button-menu-hist" href="a_manage.aspx" title="magazyn">DYSP</a>
                        <a class="button-menu-hist" href="a_dyspo.aspx" title="magazyn">LOSOW</a>
                        <a class="button-menu-hist" href="a_dyspo_ext.aspx" title="magazyn">DYSP_EX</a>
                    </td>
                    <td>
                        <a class="button-menu-hist" href="a_history.aspx" title="historia">HIST</a>
                        <a class="button-menu-hist" href="a_magazyn.aspx" title="magazyn">MAG</a>
                        <a class="button-menu-hist" href="a_pakowanie.aspx" title="pakowanie">PAK</a>
                    </td>
                    <td>
                        <a class="button-menu-bottom" href="a_label.aspx" title="etykiety">ETYKIETY</a>
                        <a class="button-menu-bottom" href="a_print.aspx" title="wydruki">WYDRUKI</a>
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

        <div style="text-align: left; width: 100%;">
            <asp:Label runat="server" ID="labelKomunikat" ForeColor="Black" Font-Size="Large" Width="100%"><% Response.Write(Session("contentKomunikat"))%></asp:Label>
            <br/>

            <h3 style="background-color:#FFFFCC; height: 25px;">Wprowadzanie nowego użytkownika</h3>
            <asp:Panel ID="Panel1" runat="server" GroupingText="Podstawowe informacje" BackColor="#EFF3FB">
                <p>
                    1. imie *<br/>
                    <asp:TextBox ID="TBImie" runat="server" Height="25px" Width="230px"></asp:TextBox>
                </p>
                <p>
                    2. nazwisko *<br/>
                    <asp:TextBox ID="TBNazwisko" runat="server" Height="25px" Width="230px"
                                 AutoPostBack="True">
                    </asp:TextBox>
                </p>
                <p>
                    3. adres email *<br/>
                    <asp:TextBox ID="TBAdresEmail" runat="server" Height="25px" Width="230px"></asp:TextBox>
                </p>
                <p>
                    4. telefon *<br/>
                    <asp:TextBox ID="TBTelefon" runat="server" Height="25px" Width="230px"></asp:TextBox>
                </p>
                <p>
                    5. magazyn domyślny * [700]<br/>
                    <asp:TextBox ID="TBMagazyn" runat="server" Height="25px" Width="230px"></asp:TextBox>
                </p>
                <p>
                    6. domyslny typ operatora * [M|O|P]<br/>
                    <asp:TextBox ID="TBtyp_oper" runat="server" Height="25px" Width="230px"></asp:TextBox>
                </p>
            </asp:Panel>
            <asp:Panel ID="Panel3" runat="server"
                       GroupingText="Dane dostępowe użytkownika" BackColor="#FFFFCC">
                <p>
                    5. login systemowy *<br/>
                    <asp:TextBox ID="TBLogin" runat="server" Height="25px" Width="230px"
                                 Enabled="False">
                    </asp:TextBox>
                </p>
                <p>
                    6. hasło systemowe *<br/>
                    <asp:TextBox ID="TBHaslo" runat="server" Height="25px" Width="230px" TextMode="Password"></asp:TextBox>
                </p>
            </asp:Panel>
            <asp:Panel ID="Panel2" runat="server" GroupingText="Informacje dodatkowe">
                <p>
                    7**.
                    <asp:CheckBox ID="CBBlokadaKonta" runat="server"
                                  Text="blokada konta operatora"/>
                </p>
            </asp:Panel>
            <p style="color:#FF0000;">
                * zaznaczenie tych pól na formularzu jest wymagane<br/>
                <font style="color:#000000;">** zaznaczenie tych pól jest opcjonalne</font><br >
            </p>
            <asp:Button ID="BUserAdd" runat="server" Text="Utworz konto" Height="51px"
                        Width="223px" Font-Bold="True"/>
        </div>
    </div>
</form>
</body>
</html>