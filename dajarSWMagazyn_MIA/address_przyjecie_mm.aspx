﻿<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="address_przyjecie_mm.aspx.vb" Inherits="dajarSWMagazyn_MIA.address_przyjecie_mm" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Adresy - dajarSystemWspomaganiaMagazynu by MM</title>
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

        <div style="text-align: left; width: 100%;">
            <asp:Label runat="server" ID="labelKomunikat" ForeColor="Black" Font-Size="Large" Width="100%"><% Response.Write(Session("contentKomunikat"))%></asp:Label>
            <br/>
            <asp:Panel ID="PanelMagazyn" runat="server" BorderColor="#333333" BorderStyle="None" BorderWidth="1px" Font-Size="Large" Width="100%">
                <asp:label ID="Label10" runat="server" BackColor="WhiteSmoke" Font-Bold="True" ForeColor="Red">ZAKŁADKA PRZYJĘCIE DOKUMENTU MM</asp:label>
                <br/><br/>
                <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium">Lista artykułów zatowarowanych na magazynie :</asp:Label>
                &nbsp;
                <asp:Label ID="LIleDokumentow" runat="server" Font-Bold="False"
                           Font-Size="Medium">
                </asp:Label>
                <br/>
                <br/>
                <asp:Panel id="Panel9" runat="server">

                    <div class="address-przyjecie-input-container">
                        <div class="address-przyjecie-input-container-column">
                            <div class="address-przyjecie-input-container-cell">
                                Numer dokumentu MM:
                                <asp:DropDownList ID="DDLDokumentMM" runat="server" Font-Size="X-Large" Height="30px" Width="210px" AutoPostBack="true" />
                                <asp:TextBox ID="TBDokumentMM" runat="server" Font-Size="X-Large" Width="200px" MaxLength="12"/>
                            </div>
                            <asp:Button ID="BDokumentMMWczytaj" runat="server" Text="WCZYTAJ MM" BackColor="Yellow" Font-Bold="True" Font-Size="X-Large" Width="300px"/>
                        </div>
                        <div class="address-przyjecie-input-container-column">
                            <asp:Button ID="BDokumentMMReczneZakoncz" runat="server" Text="AUTO ZAKOŃCZ MM" BackColor="#DCDCDC" Font-Bold="True" Font-Size="X-Large" Width="250px"/>
                            <asp:Button ID="BDokumentMMZakoncz" runat="server" Text="ZAKOŃCZ MM" BackColor="#FFCC00" Font-Bold="True" Font-Size="X-Large" Width="250px"/>
                        </div>
                    </div>

                    <div class="address-przyjecie-checkboxes">
                        <asp:CheckBox ID="CBPokazArchiwumMM" runat="server" AutoPostBack="True" Text="wyswietl archiwalne MM"/>
                        <asp:CheckBox ID="CBPokazZakonczoneMM" runat="server" AutoPostBack="True" Text="wyswietl zakonczone MM"/>
                    </div>

                </asp:Panel>
                <br/>
                <asp:Panel ID="Panel2" runat="server" GroupingText="Reczny wybór miejsca zamagazynowania" BackColor="#EFF3FB">
                    <asp:Table ID="Table4" runat="server" Width="100%">
                        <asp:TableRow>
                            <asp:TableCell>hala *</asp:TableCell>
                            <asp:TableCell>rzad *</asp:TableCell>
                            <asp:TableCell>regal *</asp:TableCell>
                            <asp:TableCell>polka *</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell>
                                <asp:DropDownList ID="DDLHala" runat="server" AutoPostBack="true">
                                </asp:DropDownList>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:DropDownList ID="DDLRzad" runat="server" AutoPostBack="true">
                                </asp:DropDownList>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:DropDownList ID="DDLRegal" runat="server" AutoPostBack="true">
                                </asp:DropDownList>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:DropDownList ID="DDLPolka" runat="server">
                                </asp:DropDownList>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:DropDownList ID="DDLStrefaMagazyn" runat="server">
                                    <asp:ListItem Text="PALETA" Value="P"></asp:ListItem>
                                    <asp:ListItem Text="NIEPEŁNA PALETA" Value="N"></asp:ListItem>
                                    <asp:ListItem Text="ZBIERANIE" Value="Z" Selected="True"></asp:ListItem>
                                </asp:DropDownList>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:Panel>
                <br/>
                <asp:Panel id="Panel1" runat="server">
                    <div class="address-przyjecie-controls">
                        <asp:Button ID="BDokumentMMZapisz" runat="server" Text="ZAPISZ ADRES" BackColor="Yellow" Font-Bold="True" Font-Size="X-Large" Width="300px"/>
                        <asp:Button ID="BDokumentMMAnuluj" runat="server" Text="ANULUJ ADRES" BackColor="#FFCC00" Font-Bold="True" Font-Size="X-Large" Width="300px"/>
                    </div>
                </asp:Panel>
                <br/>
                <asp:Panel ID="PanelMagazynData" runat="server">
                    <br/>
                    <asp:GridView ID="GridViewDokumentMM" runat="server" AutoGenerateSelectButton="True"
                                  CellPadding="5" Font-Size="Medium"
                                  OnRowDataBound="GridViewDokumentMM_RowDataBound" Width="100%" OnPageIndexChanging="GridViewDokumentMM_PageIndexChanging"
                                  AllowPaging="True" PageSize="100">
                        <PagerSettings Position="TopAndBottom" Mode="NumericFirstLast"
                                       PageButtonCount="5">
                        </PagerSettings>
                        <RowStyle BackColor="#EFF3FB"></RowStyle>
                        <FooterStyle BackColor="#CCCC99" ForeColor="Black"></FooterStyle>
                        <PagerStyle BackColor="FloralWhite" ForeColor="Black" Font-Size="X-Large" HorizontalAlign="Right" CssClass="paging"></PagerStyle>
                        <SelectedRowStyle BackColor="AliceBlue" BorderStyle="None" BorderWidth="0px" Font-Bold="false" ForeColor="black"></SelectedRowStyle>
                        <HeaderStyle BackColor="#333333" Font-Bold="True" ForeColor="White"></HeaderStyle>
                        <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
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
                    <br/>
                    <asp:Table runat="server" Width="100%">
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
                    Historia zatowarowania na magazynie
                    <asp:GridView ID="GridViewAdresacja" runat="server" Font-Size="Medium" Width="100%" AutoGenerateSelectButton="True">
                        <HeaderStyle BackColor="#E5E5E5"></HeaderStyle>
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
                    <br/>
                </asp:Panel>
            </asp:Panel>
        </div>
    </div>
</form>
</body>
</html>