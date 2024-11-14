Imports Oracle.ManagedDataAccess.Client
Imports System.Data

Partial Public Class Site
    Inherits System.Web.UI.MasterPage
    Dim sqlexp As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ''If Session("mylogin") = Nothing Then
        ''    Session("contentMenu") = "[ dajarSystemMonitZamowienia ]"
        ''Else
        ''    Session("contentMenu") = "SESJA MIASTKO : " & Session("myhash").ToString
        ''    Session("contentZalogowany") = "ZALOGOWANY : " + Session("mylogin").ToString.ToUpper
        ''    Session("contentZalogowany") += " <a href=""logout.aspx"" style=""color: #000000; font-weight: bold"">[ WYLOGOWANIE ]</a>"
        ''End If

        If Not Page.IsPostBack Then
            Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                conn.Open()
                sqlexp = "select login from dp_swm_mia_uzyt where status = 'X' and blokada_konta is null"
                DDLOperator.Items.Clear()
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                If dr.HasRows Then
                    DDLOperator.DataSource = dr
                    DDLOperator.DataTextField = "LOGIN"
                    DDLOperator.DataValueField = "LOGIN"
                    DDLOperator.DataBind()
                End If
                dr.Close()
                cmd.Dispose()

                If Session("mylogin") IsNot Nothing Then
                    DDLOperator.SelectedValue = Session("mylogin")
                End If
                conn.Close()
            End Using
        End If

    End Sub

    Protected Sub DDLOperator_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLOperator.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim login As String = DDLOperator.SelectedValue.ToString
            sqlexp = "select hash from dp_swm_mia_uzyt where login = '" & login & "'"
            Dim hash As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            Session("mylogin") = login
            Session("myhash") = hash
            sqlexp = "select trim(TYP_OPER) typ_oper FROM dp_swm_mia_UZYT WHERE LOGIN = '" & login & "'"
            Session("mytyp_oper") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            Session("contentHash") = "SESJA MIASTKO : " & hash
            Session("contentOperator") = "STANOWISKO W STREFIE : " & Session("mytyp_oper")
            Response.Redirect(Request.Url.AbsoluteUri)
            conn.Close()
        End Using

    End Sub
End Class