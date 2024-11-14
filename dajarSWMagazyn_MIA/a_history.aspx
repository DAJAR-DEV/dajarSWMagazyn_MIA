﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="a_history.aspx.vb" Inherits="dajarSWMagazyn_MIA.a_history" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Zarzadzanie - dajarSystemWspomaganiaMagazynu by MM</title>
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

        <div>
            <asp:Label runat="server" ID="labelKomunikat" ForeColor="Black" Font-Size="Large" Width="100%"><% Response.Write(Session("contentKomunikat"))%></asp:Label>
            <br/><br/>
            <asp:Panel ID="Panel2" runat="server" BorderColor="#333333" BorderStyle="None" BorderWidth="1px" Font-Size="Large" Width="100%">
                <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium">Lista operacji wykonanych w systemie :</asp:Label>
                &nbsp;<asp:Label ID="LIleDokumentow" runat="server" Font-Bold="False" Font-Size="Medium"></asp:Label>
                <br/>
                Filtrowanie danych operacji : <asp:TextBox ID="TBFiltrowanie" runat="server" AutoPostBack="true" Width="250px"></asp:TextBox>
                <br/>
                Data utworzenia wpisu historii [YYYY/MM]: <asp:TextBox ID="TBRocznik" runat="server" AutoPostBack="true" Width="55px" MaxLength="7"></asp:TextBox>
                <br/>
                <br/>
                <asp:GridView ID="GridViewDyspozycje" runat="server"
                              AutoGenerateSelectButton="True" CellPadding="5" Font-Size="Medium"
                              OnRowDataBound="GridViewDyspozycje_RowDataBound" OnPageIndexChanging="GridViewDyspozycje_PageIndexChanging"
                              Width="100%" AllowPaging="True" PageSize="100"
                              OnSorted="GridViewDyspozycje_Sorted">
                    <PagerSettings Position="TopAndBottom" Mode="NumericFirstLast"/>
                    <PagerStyle HorizontalAlign="Right"/>
                    <SelectedRowStyle ForeColor="Red"/>
                    <HeaderStyle BackColor="#E5E5E5"/>
                </asp:GridView>
                <asp:SqlDataSource ID="SqlDataSource1" runat="server"
                                   ConnectionString="<%$ ConnectionStrings:DajarOracle %>"
                                   ProviderName="<%$ ConnectionStrings:DajarOracle.ProviderName %>"
                                   SelectCommand="">
                </asp:SqlDataSource>
                <p style="text-align:left; font-size: medium;">
                    <asp:Label ID="LLogowanie" runat="server" BackColor="Khaki" Text="oper. logowania"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LWylogowanie" runat="server" BackColor="AppWorkspace" Text="oper. wylogowania"></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LZmianaStatusu" runat="server" BackColor="LightGray" Text="oper. zmiany statusu "></asp:Label>
                    &nbsp;&nbsp;
                    <asp:Label ID="LPonowneDrukowanie" runat="server" BackColor="OrangeRed" Text="ponowne drukowanie"></asp:Label>
                </p>
            </asp:Panel>
        </div>
    </div>
</form>
</body>
</html>