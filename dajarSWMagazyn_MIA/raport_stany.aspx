<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="raport_stany.aspx.vb" Inherits="dajarSWMagazyn_MIA.raport_stany" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" manifest="dswm.appcache">
<head id="Head1" runat="server">
    <title>Adresy - dajarSystemWspomaganiaMagazynu by MM</title>
    <meta name="format-detection" content="telephone=no"/>
    <link rel="apple-touch-icon" href="favicon.ico"/>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <meta name="viewport" content="initial-scale=1.0, maximum-scale=5.0"/>
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
                <div>OPERATOR: <asp:DropDownList ID="DDLOperator" runat="server" AutoPostBack="true"></asp:DropDownList></div>
                <div>MAGAZYN : <% Response.Write(Session("contentMagazyn"))%></div>
            </div>

            <hr/>

            <div class="content">
                <div class="content-operator">
                    <%            Response.Write(Session("contentOperator"))%>
                </div>
                <div class="content-hash">
                    <%            Response.Write(Session("contentHash"))%>
                </div>
            </div>

            <hr/>

        </asp:Panel>

        <div style="text-align: left; width: 100%;">
            <asp:Label runat="server" ID="labelKomunikat" ForeColor="Black" Font-Size="Large" Width="100%"><% Response.Write(Session("contentKomunikat"))%></asp:Label>
            <br/>
            <asp:Panel ID="PanelMagazyn" runat="server" BorderColor="#333333" BorderStyle="None" BorderWidth="1px" Font-Size="Large" Width="100%">
                <asp:label ID="Label10" runat="server" BackColor="WhiteSmoke" Font-Bold="True" ForeColor="Red">ZAKŁADKA RAPORT TOWARÓW</asp:label>
                <br/><br/>
                <br/>
                <asp:Panel ID="Panel3" runat="server" GroupingText="Wprowadzenie towaru na magazyn" BackColor="#EFF3FB">
                    <div class="table-responsive">
                        <asp:Table ID="Table3" runat="server" Width="100%">
                            <asp:TableRow>
                                <asp:TableCell Width="25%">skrot towaru *</asp:TableCell>
                                <asp:TableCell Width="75%">
                                    <asp:TextBox ID="TBSkrot" runat="server" AutoPostBack="true"></asp:TextBox>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>zakres data od [YYYY-MM]</asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="TBDataFrom" runat="server" Width="55">2017-01</asp:TextBox>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>zakres data do [YYYY-MM]</asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="TBDataTo" runat="server" Width="75">2017-03-31</asp:TextBox>
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
                                <asp:TableCell>stan towaru [mag 700]</asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="TBStan700" runat="server" Enabled="false"></asp:TextBox>
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell>nazwa towaru</asp:TableCell>
                                <asp:TableCell>
                                    <asp:TextBox ID="TBNazwa" runat="server" Width="100%" Enabled="false"></asp:TextBox>
                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </div>
                    <br/>
                    <div class="stany-buttons">
                        <asp:Button ID="BTowarWyszukaj" runat="server" Text="WYSZUKAJ TOWAR" BackColor="Yellow" Font-Bold="True" Font-Size="X-Large" Width="300px"/>
                        <asp:Button ID="BGenerujRaport" runat="server" Text="GENERUJ RAPORT" BackColor="#FFCC00" Font-Bold="True" Font-Size="X-Large" Width="300px"/>
                    </div>


                    <br/>
                </asp:Panel>
                <br/>
                <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Size="Medium">Lista wyników zestawienia :</asp:Label>
                &nbsp;<asp:Label ID="LIleDokumentow" runat="server" Font-Bold="False" Font-Size="Medium"></asp:Label>

                <br/>
                <div class="table-responsive">
                    <asp:GridView ID="GridViewRaport" runat="server" AutoGenerateSelectButton="True"
                                  CellPadding="5" Font-Size="Medium"
                                  Width="100%" OnPageIndexChanging="GridViewRaport_PageIndexChanging"
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
                </div>

                <br/>
            </asp:Panel>
        </div>
    </div>
</form>
</body>
</html>