Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class a_dyspo_ext
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
                TBRocznik.Text = DateTime.Now.Year.ToString
                LadujDaneGridViewDyspozycje(TBFiltrowanie.Text)
            End If
        End If

        Session("contentKomunikat") = Session(session_id)
    End Sub

    Public Sub LadujDaneGridViewDyspozycje(ByVal filtr As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If filtr <> "" Then
                sqlexp = "select rownum as lp, z.* from (" _
                & " select db.nr_zamow,db.schemat,zg.mag,zg.nr_zamow_o,zg.data_zam,zg.stat_u1,db.typ_oper,db.dyspozycja,db.skrot,db.ilosc,db.stan_700" _
                & " from dp_swm_mia_buf db,dp_swm_mia_zog zg where" _
                & " zg.nr_zamow = db.nr_zamow And zg.schemat = db.schemat" _
                & " and substr(zg.nr_zamow,11,2) = '" & TBRocznik.Text.Substring(2, 2) & "'" _
                & " ) z where z.nr_zamow like '%" & filtr & "%' or z.nr_zamow_o like '%" & filtr & "%' or z.stat_u1 like '%" & filtr & "%' or z.typ_oper like '%" & filtr & "%' or z.schemat like '%" & filtr & "%' or z.skrot like '" & filtr & "'" _
                & " order by substr(z.nr_zamow,11,2) DESC,substr(z.nr_zamow,0,9) DESC"
            Else
                sqlexp = "select rownum as lp, z.* from (" _
                & " select db.nr_zamow,db.schemat,zg.mag,zg.nr_zamow_o,zg.data_zam,zg.stat_u1,db.typ_oper,db.dyspozycja,db.skrot,db.ilosc,db.stan_700" _
                & " from dp_swm_mia_buf db,dp_swm_mia_zog zg where" _
                & " zg.nr_zamow = db.nr_zamow And zg.schemat = db.schemat" _
                & " and substr(zg.nr_zamow,11,2) = '" & TBRocznik.Text.Substring(2, 2) & "'" _
                & " ) z " _
                & " order by substr(z.nr_zamow,11,2) DESC,substr(z.nr_zamow,0,9) DESC"
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
            Dim stat_u1 As String = e.Row.Cells(7).Text.ToString
            Dim mag As String = e.Row.Cells(4).Text.ToString

            If stat_u1 = "RP" Then
                e.Row.BackColor = LPilne.BackColor
            Else
                If mag = "700" Then : e.Row.BackColor = LMag700.BackColor
                ElseIf mag = "46" Then : e.Row.BackColor = LMag46.BackColor
                End If
            End If

        End If
    End Sub

    Protected Sub GridViewDyspozycje_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewDyspozycje.SelectedIndexChanged
        If GridViewDyspozycje.Rows.Count > 0 Then
            Session("kod_dyspo") = GridViewDyspozycje.SelectedRow.Cells(2).Text
            Session("schemat_dyspo") = GridViewDyspozycje.SelectedRow.Cells(3).Text
            Session("login_dyspo") = GridViewDyspozycje.SelectedRow.Cells(9).Text
            Session("typ_dyspo") = GridViewDyspozycje.SelectedRow.Cells(10).Text
            Session("status_dyspo") = GridViewDyspozycje.SelectedRow.Cells(11).Text
            Session("mag_dyspo") = GridViewDyspozycje.SelectedRow.Cells(12).Text
            Response.Redirect("a_manageEdit.aspx")
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