<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="storage.aspx.vb" Inherits="dajarSWMagazyn_MIA.storage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Magazyn - dajarSystemWspomaganiaMagazynu by MM</title>
    <meta name="format-detection" content="telephone=no"/>
    <link rel="apple-touch-icon" href="favicon.ico"/>
    <meta name="viewport" content="initial-scale=1.0, maximum-scale=5.0"/>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <link rel="stylesheet" type="text/css" href="style.css"/>
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
            <asp:Button ID="BRefreshPage" runat="server" Text="ODŚWIEŻ STRONĘ" BackColor="Black" Font-Size="X-Large" ForeColor="White" Font-Bold="True" Width="250px"/>
            <asp:Panel ID="PanelMagazyn" runat="server" BorderColor="#333333" BorderStyle="None"
                       BorderWidth="1px" Font-Size="Large" Width="100%">
                
                
                <br/>
                <asp:TextBox ID="TBEtykieta" runat="server" Width="118px" Visible="true"></asp:TextBox>
                <br/>
                <asp:Panel ID="PanelMagazynData" runat="server">
                    <br/>
                    <div class="table-mobile">
                        <asp:GridView ID="GridViewIndeksy" runat="server" Font-Size="Medium" Width="100%">
                            <HeaderStyle BackColor="#E5E5E5"></HeaderStyle>
                        </asp:GridView>
                    </div>
                    <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium">
                        Lista dyspozycji oczekująca na magazynie:&nbsp;
                    </asp:Label>
                    <asp:Label ID="LIleDokumentow" runat="server" Font-Bold="False" Font-Size="Medium"></asp:Label>
                    <div class="table-mobile">
                        <asp:GridView ID="GridViewMagazyn" runat="server"
                                      CellPadding="5" Font-Size="Large"
                                      OnRowDataBound="GridViewMagazyn_RowDataBound" Width="100%" OnPageIndexChanging="GridViewMagazyn_PageIndexChanging"
                                      AllowPaging="True" PageSize="1000">
                            <PagerSettings Position="TopAndBottom" Mode="NumericFirstLast"
                                           PageButtonCount="2">
                            </PagerSettings>
                            <RowStyle BackColor="#EFF3FB"></RowStyle>
                            <FooterStyle BackColor="#CCCC99" ForeColor="Black"></FooterStyle>
                            <PagerStyle BackColor="FloralWhite" ForeColor="Black" Font-Size="X-Large" HorizontalAlign="Right" CssClass="paging"></PagerStyle>
                            <SelectedRowStyle BackColor="AliceBlue" BorderStyle="Solid" BorderWidth="5px" Font-Bold="true" ForeColor="black"></SelectedRowStyle>
                            <HeaderStyle BackColor="#333333" Font-Bold="True" ForeColor="White"></HeaderStyle>
                            <AlternatingRowStyle BackColor="White"></AlternatingRowStyle>
                            <Columns>
                                <asp:TemplateField HeaderText="Select" Visible="False">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="CBKodSelect" runat="server"/>
                                    </ItemTemplate>
                                    <HeaderTemplate>
                                    </HeaderTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Mock" Visible="False">
                                    
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <br/>
                    <div class="address-wymag-footer">
                        <div class="address-wymag-footer-legend">
                            <asp:Label ID="Ldajar" runat="server" BackColor="Khaki" Text="zamówienia dajar"/>
                            <asp:Label ID="Ldominus" runat="server" BackColor="Thistle" ForeColor="black" Text="zamówienia dominus"/>
                        </div>
                        <div class="address-wymag-footer-buttons">
                            <asp:Button ID="BZaznaczWszystkie" runat="server" Text="+" BackColor="Black" Font-Size="X-Large" ForeColor="White" Font-Bold="true" Width="135px"/>
                            <asp:Button ID="BOdznaczWszystkie" runat="server" Text="-" BackColor="Black" Font-Size="X-Large" ForeColor="White" Font-Bold="true" Width="135px"/>
                        </div>
                    </div>
                    <br/>
                    <div class="storage-buttons">
                        <asp:Button ID="BZakonczMagazyn" runat="server" Text="ZAKOŃCZ MAGAZYN" BackColor="#FFCC00" Font-Bold="True" Font-Size="X-Large" Width="300px"/>
                        <asp:Button ID="BWyswietlSzczegoly" runat="server" Text="WCZYTAJ SZCZEGÓŁY" BackColor="Black" Font-Size="X-Large" ForeColor="White" Font-Bold="True" Width="300px"/>
                    </div>


                    <br/>
                    <br/>
                    

                </asp:Panel>

            </asp:Panel>
        </div>
    </div>
</form>
</body>
</html>