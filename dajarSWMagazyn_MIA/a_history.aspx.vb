Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class a_history
    Inherits System.Web.UI.Page
    Dim sqlexp As String = ""
    Dim result As Boolean = False
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder

    Protected Sub GridViewDyspozycje_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewDyspozycje.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewDyspozycje.PageIndex = e.NewPageIndex
        GridViewDyspozycje.DataBind()
        LadujDaneGridViewDyspozycje(TBFiltrowanie.Text)
    End Sub

    Protected Sub GridViewDyspozycje_Sorted(ByVal sender As Object, ByVal e As EventArgs)
        LadujDaneGridViewDyspozycje(TBFiltrowanie.Text)
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
                TBRocznik.Text = DateTime.Now.Year.ToString & "/" & DateTime.Now.Month.ToString("D2")
                LadujDaneGridViewDyspozycje(TBFiltrowanie.Text)
            End If
        End If

        Session("contentKomunikat") = Session(session_id)
    End Sub

    Public Sub LadujDaneGridViewDyspozycje(ByVal filter As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If filter <> "" Then
                sqlexp = "select login, hash, typ_oper, to_char(autodata,'YYYY/MM/DD HH24:MI:SS') autodata,status from dp_swm_mia_oper" _
                & " where login like '%" & filter & "%' or status like '%" & filter & "%' or typ_oper like '%" & filter & "%'" _
                & " and to_char(autodata,'YYYY/MM') = '" & TBRocznik.Text & "'" _
                & " order by autodata desc"
            Else
                sqlexp = "select login, hash, typ_oper, to_char(autodata,'YYYY/MM/DD HH24:MI:SS') autodata,status from dp_swm_mia_oper " _
                & " where to_char(autodata,'YYYY/MM') = '" & TBRocznik.Text & "'" _
                & " order by autodata desc"
            End If

            GridViewDyspozycje.DataSource = Nothing
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridViewDyspozycje.DataSource = dsPartie.Tables(0)
            GridViewDyspozycje.DataBind()
            cmd.Dispose()

            If dsPartie.Tables(0).Rows.Count = 0 Then
                LIleDokumentow.Text = "brak"
            Else
                LIleDokumentow.Text = dsPartie.Tables(0).Rows.Count.ToString
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub GridViewDyspozycje_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewDyspozycje.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim typ_oper As String = e.Row.Cells(3).Text.ToString
            Dim status As String = e.Row.Cells(5).Text.ToString

            If typ_oper = "LOGIN" Then
                e.Row.BackColor = LLogowanie.BackColor
            ElseIf typ_oper = "LOGOUT" Then
                e.Row.BackColor = LWylogowanie.BackColor
            End If

            If status.Length = 12 Then
                e.Row.BackColor = LZmianaStatusu.BackColor
            End If
        End If

    End Sub

    Protected Sub GridViewDyspozycje_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewDyspozycje.SelectedIndexChanged
        If GridViewDyspozycje.Rows.Count > 0 Then
            If GridViewDyspozycje.SelectedRow.Cells(5).Text.ToString.Length = 12 Then
                Session("kod_dyspo") = GridViewDyspozycje.SelectedRow.Cells(5).Text
                Session("login_dyspo") = GridViewDyspozycje.SelectedRow.Cells(1).Text
                Dim bufor_dysp() As String = GridViewDyspozycje.SelectedRow.Cells(3).Text.Split("[")
                If bufor_dysp.Length > 1 Then
                    Session("schemat_dyspo") = bufor_dysp(1).Replace("]", "")
                    Session("typ_dyspo") = bufor_dysp(0)
                    Session("status_dyspo") = bufor_dysp(0)
                    Session("mag_dyspo") = bufor_dysp(1).Replace("]", "")
                End If
                
                Response.Redirect("a_manageEdit.aspx")
            End If
        End If
    End Sub

    Protected Sub TBFiltrowanie_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBFiltrowanie.TextChanged
        Dim filter_data As String = TBFiltrowanie.Text
        If filter_data <> "" Then
            LadujDaneGridViewDyspozycje(filter_data)
        Else
            LadujDaneGridViewDyspozycje("")
        End If
    End Sub

    Protected Sub TBRocznik_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBRocznik.TextChanged
        Dim filter_data As String = TBFiltrowanie.Text
        If filter_data <> "" Then
            LadujDaneGridViewDyspozycje(filter_data)
        Else
            LadujDaneGridViewDyspozycje("")
        End If
    End Sub
End Class