<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="a_manageEdit.aspx.vb" Inherits="dajarSWMagazyn_MIA.a_manageEdit" %>
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
    <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium">Karta edycji dyspozycji</asp:Label>
    <asp:Panel ID="Panel3" runat="server" BorderColor="#333333" BorderStyle="None" borderWidth="1px" Font-Size="Large" Width="1260px">
        <asp:panel ID="panel11" runat="server">
            numer aktualnie edytowanego zamowienia : <asp:Label ID="LNrZamow" runat="server" Font-Bold="False" ForeColor="Red"></asp:Label><br/>
            wersja schematu digitland : <asp:Label ID="LSchemat" runat="server"></asp:Label><br/>
            <asp:Panel ID="panelDigitland" runat="server" Visible="false">
                numer zamowienia digitland dajar : &nbsp;
                <asp:Label ID="LNrZamowDigit" runat="server"
                           ForeColor="Red">
                </asp:Label>&nbsp;
                <asp:Button ID="BPobierzNrDigit" runat="server" Text="pobierz digit dajar"
                            BackColor="Black" ForeColor="White"/>
            </asp:Panel>
            numer zamówienia klienta : <asp:Label ID="LNr_zamow_o" runat="server" Font-Bold="True"></asp:Label><br/>
            pierwotny status zamowienia digitland : <asp:Label ID="LStatusDigit" runat="server" Font-Bold="True"></asp:Label><br/>
            status aktualnie edytowanego zamowienia : <asp:Label ID="LStatus" runat="server" Font-Bold="True"></asp:Label><br/>
            magazyn edytowanego zamowienia : <asp:Label ID="LMagDyspo" runat="server" Font-Bold="True"></asp:Label><br/>
            aktualny numer wydruku : <asp:Label ID="LWydrukID" runat="server" Font-Bold="True"></asp:Label><br/>
        </asp:panel>
        <br/>
        <asp:GridView ID="GridViewDyspozycje" runat="server" AutoGenerateSelectButton="false" CellPadding="5" Font-Size="Medium"
                      OnRowDataBound="GridViewDyspozycje_RowDataBound" OnPageIndexChanging="GridViewDyspozycje_PageIndexChanging"
                      Width="100%" AllowPaging="True" PageSize="50">
            <PagerSettings Position="TopAndBottom"/>
            <PagerStyle HorizontalAlign="Right"/>
            <SelectedRowStyle ForeColor="Red"/>
            <HeaderStyle BackColor="#E5E5E5"/>
        </asp:GridView>
        <p style="text-align:left; font-size: medium;">
            <asp:Label ID="LMagazyn" runat="server" BackColor="Khaki" Text="MG - na magazynie"></asp:Label>
            &nbsp;
            <asp:Label ID="LPakowanie" runat="server" BackColor="LightGray" Text="PA - przygotowane do pakowania"></asp:Label>
            &nbsp;
            <asp:Label ID="LWstrzymane" runat="server" BackColor="OrangeRed" Text="ZW - wstrzymane"></asp:Label>
            &nbsp;
            <asp:Label ID="LBlokadaNaHippo" runat="server" BackColor="peru" Text="HB - blokada na hippo"></asp:Label>
            &nbsp;
            <asp:Label ID="LBlokadaNaBurek" runat="server" BackColor="sienna" Text="BB - blokada na burek"></asp:Label>
            &nbsp;&nbsp;
            <asp:Label ID="LWznowione" runat="server" BackColor="LimeGreen" Text="WN - wznowione"></asp:Label>
            &nbsp;
            <asp:Label ID="LPrzeslanoNaDocelowy" runat="server" BackColor="MediumOrchid" Text="PP - skierowano na mag.docelowy"></asp:Label>
            &nbsp;&nbsp;
            <asp:Label ID="LPodjete" runat="server" BackColor="Gold" Text="PA & ID_ET - w trakcie pakowania"></asp:Label>
            &nbsp;
            <asp:Label ID="LSpakowane" runat="server" BackColor="DarkGray" Text="PE - spakowane paragon"></asp:Label>
            &nbsp;
            <asp:Label ID="LPilne" runat="server" BackColor="GreenYellow" Text="RP - realizacja pilne"></asp:Label>
            &nbsp;
            <asp:Label ID="LRezygnacjaWlasna" runat="server" BackColor="blue" Text="RW - rezygnacja wlasna"></asp:Label>
        </p>
        <asp:Panel ID="Panel6" runat="server" Font-Size="Medium" GroupingText="lista pozycji do losowania" Visible="true">
            <asp:GridView ID="GridViewLosowanie" runat="server" AutoGenerateSelectButton="false" CellPadding="5" Font-Size="Medium"
                          Width="100%" AllowPaging="True" PageSize="50">
                <PagerSettings Position="TopAndBottom"/>
                <PagerStyle HorizontalAlign="Right"/>
                <SelectedRowStyle ForeColor="Red"/>
                <HeaderStyle BackColor="#E5E5E5"/>
            </asp:GridView>
        </asp:Panel>
        <p>
            [data zamówienia digitland] :
            <asp:Label ID="LDataZam" runat="server" Text="Label"></asp:Label>
            <br/>
            [data przyjęcia zamówienia do systemu] :
            <asp:Label ID="LDataWprowadzenia" runat="server" ForeColor="Red" Text="Label"></asp:Label>
            <br/>
            [data zamknięcia zamówienia w systemie] :
            <asp:Label ID="LDataZakonczenia" runat="server" Text="Label"></asp:Label>
            <br/>
            [zamowienie ma przypisane paczki w systemie] :
            <asp:Label ID="LEtykieta" runat="server" Text="Label"></asp:Label>
        </p>

        <asp:Panel ID="PanelEtykieta" runat="server" Font-Size="Medium" GroupingText="dane identyfikacyjne paczek" Visible="false">
            <asp:GridView ID="GridViewPaczki" runat="server"
                          AutoGenerateSelectButton="false" Font-Size="Medium" Width="100%">
                <HeaderStyle BackColor="#E5E5E5"/>
                <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"/>
            </asp:GridView>
        </asp:Panel>
        <asp:Table runat="server" Width="100%">
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Panel ID="Panel2" runat="server" BackColor="#EFF3FB" Font-Size="Medium" GroupingText="dane aktualnie przypisanego operatora">
                        aktualny operator dyspozycji : <asp:Label ID="LOperatorBiezacy" runat="server" Font-Size="Large" ForeColor="Black" Width="100px"></asp:Label>
                        <br/>
                        aktualny typ_operacji : <asp:Label ID="LOperatorTypOper" runat="server" Font-Size="Large" ForeColor="Black" Width="100px"></asp:Label>
                    </asp:Panel>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Panel ID="Panel4" runat="server" BackColor="#FFFFCC" Font-Size="Medium" GroupingText="zmiana danych operatora">
                        zmiana operatora dyspozycji : <asp:DropDownList ID="DDLOperatorDyspo" runat="server" AutoPostBack="True"></asp:DropDownList>
                        <br/>
                        zmiana typ_operacji : <asp:Label ID="LStatusUzytkownik" runat="server" Text="Label"></asp:Label>
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <asp:Panel ID="Panel7" runat="server" BackColor="#CCCCCC" Font-Size="Medium" GroupingText="reczne losowanie operatora">
                        wybierz operatora dyspozycji : <asp:DropDownList ID="DDLOperatorLosowanie" runat="server" AutoPostBack="True"></asp:DropDownList>
                        <br/>
                        aktualny typ_operacji : <asp:Label ID="LStatusUzytkownikLosowanie" runat="server" Font-Size="Large" ForeColor="Black" Width="100px"></asp:Label>
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <asp:CheckBox ID="CBOperLosowanie" runat="server" Text="zapisz operatora do recznego losowania"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <asp:Panel ID="Panel5" runat="server" Font-Size="Medium" GroupingText="informacje dodatkowe">
            <asp:TextBox ID="TBInformacjeDodatkowe" runat="server" Height="50px"
                         TextMode="MultiLine" Width="100%">
            </asp:TextBox>
        </asp:Panel>
        <asp:Panel ID="Panel1" runat="server" GroupingText="" Width="100%">
            <p>
                <asp:Button ID="BZmianaOper" runat="server" Font-Bold="True" Height="51px"
                            Text="ZMIANA OPERATORA" Width="190px" BackColor="#FFFFCC"/>
                &nbsp;&nbsp;
                <asp:Button ID="BWstrzymajDyspo" runat="server" BackColor="OrangeRed" Font-Bold="True" ForeColor="White" Height="51px" Text="WSTRZYMAJ DYSP" Width="190px"/>
                &nbsp;&nbsp;
                <asp:Button ID="BWznowDyspo" runat="server" BackColor="LimeGreen" Font-Bold="True" ForeColor="White" Height="51px" Text="WZNÓW DYSP" Width="190px"/>
                &nbsp;&nbsp;
                <asp:Button ID="BSpakowaneDyspo" runat="server" BackColor="Black" Font-Bold="True" ForeColor="White" Height="51px" Text="AUTOPAKOWANIE" Width="190px"/>
                &nbsp;&nbsp;
                <asp:Button ID="BGenerujWydruk" runat="server" BackColor="gold" Font-Bold="True" ForeColor="black" Height="51px" Text="GENERUJ WYDRUK" Width="190px"/>
                &nbsp;&nbsp;
                <asp:Button ID="BStatusPE" runat="server" BackColor="red" Font-Bold="True" ForeColor="White" Height="51px" Text="STATUS PE" Width="190px"/>
            </p>
            <p>
                <asp:Button ID="BAnulowanieDyspo" runat="server" BackColor="Blue" Font-Bold="True" ForeColor="White" Height="51px" Text="ANULOWANIE ZAM" Width="190px"/>
            </p>
        </asp:Panel>
    </asp:Panel>
</div>
</div>
</form>
</body>
</html>