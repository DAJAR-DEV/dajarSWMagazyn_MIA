<%@ Page Language="vb" AutoEventWireup="false" CodeFile="change_pass.aspx.vb" Inherits="dajarSWMagazyn_MIA.change_pass" MasterPageFile="Site.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="contentBody" Runat="Server">
    <form id="change_pass" method="post" action="change_pass.aspx">
        <div>
            <h3 style="background-color:#FFFFCC; height: 25px;">Modyfikacja hasła użytkownika systemu</h3>
            <asp:Panel ID="panel4" GroupingText="Login użytkownika" runat="server">
                <asp:TextBox ID="TBLogin" runat="server" Height="27px" Width="312px" MaxLength="50"></asp:TextBox>
            </asp:Panel>
            <asp:Panel ID="panel1" GroupingText="Hasło użytkownika" runat="server">
                <asp:TextBox ID="TBPass1" runat="server" Height="27px" Width="312px"
                             MaxLength="50" TextMode="Password">
                </asp:TextBox>
            </asp:Panel>
            <asp:Panel ID="panel2" GroupingText="Ponowne wprowadzenie hasła użytkownika" runat="server">
                <asp:TextBox ID="TBPass2" runat="server" Height="27px" Width="312px"
                             MaxLength="50" TextMode="Password">
                </asp:TextBox>
            </asp:Panel>
            <br/>
            <p>
                <asp:Button ID="BZapiszZmiany" runat="server" Text="Zapisz zmiany" Width="217px" CssClass="button-application" style="top: -12px; left: 0px; height: 34px"/>
            </p>
        </div>
    </form>
</asp:Content>