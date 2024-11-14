<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="a_adresyEdit.aspx.vb" Inherits="dajarSWMagazyn_MIA.a_adresyEdit" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Adresacja magazynu edycja - dajarSystemWspomaganiaMagazynu by MM</title>
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

    <h3 style="background-color:#FFFFCC; height: 25px;">Wprowadzanie nowego adresu magazynowego</h3>
    <asp:Panel ID="Panel1" runat="server" GroupingText="Podstawowe informacje" BackColor="#EFF3FB">
        <asp:Table ID="Table4" runat="server" Width="100%">
            <asp:TableRow>
                <asp:TableCell>hala *</asp:TableCell>
                <asp:TableCell>rzad *</asp:TableCell>
                <asp:TableCell>regal *</asp:TableCell>
                <asp:TableCell>polka *</asp:TableCell>
                <asp:TableCell>is_active *</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>
                    <asp:DropDownList ID="DDLHala" runat="server" Enabled="false">
                    </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="DDLRzad" runat="server" Enabled="false">
                    </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="DDLRegal" runat="server" AutoPostBack="true" Enabled="false">
                    </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="DDLPolka" runat="server" Enabled="false">
                    </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:Label ID="LISActive" runat="server" Enabled="false"></asp:Label>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <br/>
    <asp:Panel ID="Panel2" runat="server" GroupingText="Wprowadzone towaru na magazyn" BackColor="#EFF3FB">
        <asp:Table runat="server" Width="100%">
            <asp:TableRow>
                <asp:TableCell Width="25%">skrot towaru *</asp:TableCell>
                <asp:TableCell Width="75%">
                    <asp:TextBox ID="TBSkrot" runat="server" AutoPostBack="true"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>strefa magazynowa</asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="DDLStrefaMagazyn" runat="server">
                        <asp:ListItem Text="PALETA" Value="P"></asp:ListItem>
                        <asp:ListItem Text="NIEPEŁNA PALETA" Value="N"></asp:ListItem>
                        <asp:ListItem Text="ZBIERANIE" Value="Z" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>indeks towaru</asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="TBIndeks" runat="server" Enabled="false"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>prefix towaru</asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="TBPrefix" runat="server" Enabled="false"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>kod ean</asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="TBEan" runat="server" Enabled="false"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell>nazwa towaru</asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="TBNazwa" runat="server" Width="100%" Enabled="false"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br/>
        <asp:Table ID="Table3" runat="server" Width="100%">
            <asp:TableRow>
                <asp:TableCell HorizontalAlign="Left">
                    <asp:Button ID="BDodajTowar" runat="server" Text="DODAJ TOWAR" BackColor="#FFCC00" Font-Bold="True" Font-Size="X-Large" Width="300px"/>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <br/>
    </asp:Panel>
    <br/>
    <asp:Panel ID="panelListaKodow" runat="server"
               GroupingText="Lista wprowadzonych towarow na magazynie" Width="100%">
        <asp:GridView ID="GridViewTowary" runat="server" AllowPaging="True"
                      BackColor="White" BorderColor="#CCCCCC" BorderStyle="None"
                      BorderWidth="1px" CellPadding="4" Font-Size="Medium" ForeColor="Black"
                      GridLines="Horizontal"
                      OnRowDataBound="GridViewTowary_RowDataBound"
                      OnPageIndexChanging="GridViewTowary_PageIndexChanging"
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
    </asp:Panel>
    <p style="color:#FF0000;">
        * zaznaczenie tych pól na formularzu jest wymagane<br/>
        <font style="color:#000000;">** zaznaczenie tych pól jest opcjonalne</font><br >
    </p>

    <asp:Table ID="Table1" runat="server" Width="100%">
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="Left">
                <asp:Label ID="LZbieranie" runat="server" BackColor="Khaki" Text="&nbsp;strefa zbierania&nbsp;"></asp:Label>
                &nbsp;&nbsp;
                <asp:Label ID="LNiePaleta" runat="server" BackColor="Thistle" ForeColor="black" Text="&nbsp;strefa niepełna paleta&nbsp;"></asp:Label>
                &nbsp;&nbsp;
                <asp:Label ID="LPaleta" runat="server" BackColor="Azure" ForeColor="black" Text="&nbsp;strefa paleta&nbsp;"></asp:Label>
                &nbsp;&nbsp;
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="right">
                <asp:Button ID="BZaznaczWszystkie" runat="server" Text="+" BackColor="Black" Font-Size="X-Large" ForeColor="White" Font-Bold="true" Width="135px"/> &nbsp;
                &nbsp;&nbsp;
                <asp:Button ID="BOdznaczWszystkie" runat="server" Text="-" BackColor="Black" Font-Size="X-Large" ForeColor="White" Font-Bold="true" Width="135px"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <br/>
    <asp:Table ID="Table2" runat="server" Width="100%">
        <asp:TableRow>
            <%--                <asp:TableCell HorizontalAlign="Left">
                <asp:Button ID="BAktywacjaTowary" runat="server" Text="AKT/DEAKT TOWARÓW" BackColor="#FFCC00" Font-Bold="True" Font-Size="X-Large" Width="300px" />
                </asp:TableCell>
--%>
            <asp:TableCell HorizontalAlign="left">
                <asp:Button ID="BUsunTowar" runat="server" Text="USUŃ TOWAR" BackColor="Black" Font-Size="X-Large" ForeColor="White" Font-Bold="True" Width="300px"/>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="right">
                <asp:Button ID="BAktywacjaAdres" runat="server" Text="AKT/DEAKT ADRESU" BackColor="Black" Font-Size="X-Large" ForeColor="White" Font-Bold="True" Width="300px"/>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</div>
</div>
</form>
</body>
</html>