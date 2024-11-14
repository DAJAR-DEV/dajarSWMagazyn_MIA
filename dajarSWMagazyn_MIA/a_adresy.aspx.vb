Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class a_adresy
    Inherits System.Web.UI.Page
    Dim sqlexp As String = ""
    Dim result As Boolean = False
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder

    Protected Sub GridViewAdresy_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewAdresy.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewAdresy.PageIndex = e.NewPageIndex
        GridViewAdresy.DataBind()
        LadujDaneGridViewAdresy(TBFiltrowanie.Text)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Dim przerwijLadowanie As Boolean = False

        If Session("mylogin") = Nothing And Session("myhash") = Nothing Then
            Session.Abandon()
            Response.Redirect("index.aspx")
        ElseIf Session("mytyp_oper") <> "S" Then
            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
            Session(session_id) += "<br />Zaloguj sie na opowiedniego operatora - typ ADMINISTRATOR<br />"
            Session(session_id) += "</div>"
            przerwijLadowanie = True
        Else
            Session.Remove(session_id)
        End If

        ''Session.Remove("contentKomunikat")
        If Not Page.IsPostBack Then
            If przerwijLadowanie = False Then
                LadujDaneGridViewAdresy(TBFiltrowanie.Text.ToString)
            End If
        End If
        Session("contentKomunikat") = Session(session_id)
    End Sub

    Public Sub LadujDaneGridViewAdresy(ByVal filtr As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("filter_skrot") = "1" Then
                sqlexp = "select dt.adres,substr(dt.adres,0,1) hala, substr(dt.adres,2,2) rzad, substr(dt.adres,4,2) regal, substr(dt.adres,6,2) polka, dt.is_active,dt.login,dt.hash,to_char(dt.autodata, 'YYYY/MM/DD HH24:MI:SS') autodata " _
                & " from dp_swm_mia_towary dt where dt.skrot in('" & filtr & "') order by dt.adres asc"
            Else
                If filtr <> "" Then
                    sqlexp = "select adres,hala,rzad,regal,polka,is_active,login,hash,to_char(autodata, 'YYYY/MM/DD HH24:MI:SS') autodata from dp_swm_mia_adresy where adres like '%" & filtr & "%' order by adres asc"
                Else
                    sqlexp = "select adres,hala,rzad,regal,polka,is_active,login,hash,to_char(autodata, 'YYYY/MM/DD HH24:MI:SS') autodata from dp_swm_mia_adresy order by adres asc"
                End If
            End If

            GridViewAdresy.DataSource = Nothing
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridViewAdresy.DataSource = dsPartie.Tables(0)
            GridViewAdresy.DataBind()
            cmd.Dispose()

            If GridViewAdresy.Rows.Count = 0 Then
                LIleDokumentow.Text = "brak rekordow"
            Else
                LIleDokumentow.Text = GridViewAdresy.Rows.Count.ToString
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub TBFiltrowanie_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBFiltrowanie.TextChanged
        Dim filter_data As String = TBFiltrowanie.Text
        If filter_data <> "" Then
            LadujDaneGridViewAdresy(filter_data)
        Else
            LadujDaneGridViewAdresy("")
        End If
    End Sub

    Protected Sub GridViewAdresy_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewAdresy.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim status As String = e.Row.Cells(4).Text.ToString
            If status = "1" Then
                e.Row.BackColor = LUzytkownikAktywny.BackColor
            End If
        End If
    End Sub

    Protected Sub GridViewAdresy_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewAdresy.SelectedIndexChanged
        If GridViewAdresy.Rows.Count > 0 Then
            Session("kod_adres") = GridViewAdresy.SelectedRow.Cells(1).Text
            Response.Redirect("a_adresyEdit.aspx")
        End If
    End Sub

    Protected Sub CBFiltrowanieSkrot_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBFiltrowanieSkrot.CheckedChanged
        If CBFiltrowanieSkrot.Checked Then
            Session("filter_skrot") = "1"
        Else
            Session.Remove("filter_skrot")
        End If

        Dim filter_data As String = TBFiltrowanie.Text
        If filter_data <> "" Then
            LadujDaneGridViewAdresy(filter_data)
        Else
            LadujDaneGridViewAdresy("")
        End If
    End Sub
End Class