<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="a_userEdit.aspx.vb" Inherits="dajarSWMagazyn_MIA.a_userEdit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Uzytkownicy edycja - dajarSystemWspomaganiaMagazynu by MM</title>
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
            <asp:Panel ID="Panel3" runat="server" BorderColor="#333333" BorderStyle="None"
                       BorderWidth="1px" Font-Size="Large" Width="100%">
                <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium">Karta edycji danych użytkownika</asp:Label>
                <p>
                    Login operatora :
                    <asp:Label ID="LOperatorLogin" runat="server" Text="Label"
                               Font-Bold="False" ForeColor="Red">
                    </asp:Label><br/>
                    Identyfikator bieżacej sesji : <asp:Label ID="LHash" runat="server" Text="Label"></asp:Label><br/>
                    Data ostatniego logowania : <asp:Label ID="LDataLogowania" runat="server" Text="Label"></asp:Label><br/>
                    Status użytkownika : <asp:Label ID="LStatusUzytkownik" runat="server" Text="Label" BackColor="Red"></asp:Label>
                </p>
                <p>Adres email : <br/><asp:TextBox ID="TBEmail" runat="server" Height="24px" Width="250px"></asp:TextBox></p>
                <p>Telefon : <br/><asp:TextBox ID="TBTelefon" runat="server" Height="24px" Width="250px"></asp:TextBox></p>
                <p>Docelowy magazyn : <br/><asp:TextBox ID="TBMagazyn" runat="server" Height="24px" Width="250px"></asp:TextBox>
                <p>Blokada konta : <asp:CheckBox ID="CBBlokada" runat="server"/><br/></p>
                <asp:Panel ID="panelKomunikatSms" runat="server"
                           GroupingText="Wysylanie komunikatow SMS [bez znakow specjalnych]">
                    <asp:TextBox ID="TBKomunikatSms" runat="server" Height="50px"
                                 TextMode="MultiLine" Width="100%">
                    </asp:TextBox>
                    <br/>
                    <asp:Button ID="BWyslijSms" runat="server" Text="WYSLIJ SMS" BackColor="Black"
                                ForeColor="White"/>
                </asp:Panel>
                <br/>
                Filtrowanie numer zamówienia : <asp:DropDownList ID="DDLListaZamowien" runat="server" AutoPostBack="True"></asp:DropDownList>
                <br/>
                <asp:Panel ID="panelListaKodow" runat="server"
                           GroupingText="Lista wprowadzonych dyspozycji dla operatora" Width="100%">
                    <asp:GridView ID="GridViewDyspozycje" runat="server" AllowPaging="True"
                                  BackColor="White" BorderColor="#CCCCCC" BorderStyle="None"
                                  BorderWidth="1px" CellPadding="4" Font-Size="Medium" ForeColor="Black"
                                  GridLines="Horizontal"
                                  OnPageIndexChanging="GridViewPrzydzieloneSklepy_PageIndexChanging"
                                  PageSize="50" Width="100%">
                        <RowStyle BackColor="#EFF3FB"/>
                        <FooterStyle BackColor="#CCCC99" ForeColor="Black"/>
                        <PagerSettings Position="TopAndBottom"/>
                        <PagerStyle BackColor="White" BorderColor="Black" BorderWidth="0px"
                                    CssClass="cssPager" Font-Overline="True" Font-Size="Large"
                                    Font-Underline="True" ForeColor="Black" HorizontalAlign="Right"/>
                        <SelectedRowStyle BackColor="#CC3333" Font-Bold="True" ForeColor="White"/>
                        <HeaderStyle BackColor="#333333" Font-Bold="True" ForeColor="White"/>
                        <AlternatingRowStyle BackColor="White"/>
                    </asp:GridView>
                </asp:Panel>
                <asp:SqlDataSource ID="SqlDataSource2" runat="server"></asp:SqlDataSource>
                <asp:Panel ID="Panel1" runat="server" GroupingText="" Width="100%">
                    <p>
                        <asp:Button ID="BZatwierdzZmiany" runat="server" Font-Bold="True" Height="51px"
                                    Text="ZATWIERDZ ZMIANY" Width="223px"/>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="BUsunKonto" runat="server" BackColor="Red" Enabled="False"
                                    Font-Bold="True" ForeColor="White" Height="51px" Text="USUN KONTO UZYTKOWNIKA"
                                    Width="223px" Visible="False"/>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="BWyloguj" runat="server" BackColor="#003300" Font-Bold="True"
                                    ForeColor="White" Height="51px" Text="WYLOGUJ Z SYSTEMU" Width="223px"/>
                    </p>
                </asp:Panel>
            </asp:Panel>
        </div>
    </div>
</form>
</body>
</html>