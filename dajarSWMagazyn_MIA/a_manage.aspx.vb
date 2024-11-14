Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class a_manage
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

        If Not Page.IsPostBack Then
            If przerwijLadowanie = False Then
                TBRocznik.Text = DateTime.Now.Year.ToString & "/" & DateTime.Now.Month.ToString("D2")
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
                & " select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                & " zg.data_zam, dm.login, 'M' typ_oper, " _
                & " dm.status,dm.mag,to_char(dm.autodata_zak,'RRRR/MM/DD HH24:MI:SS') data_zak,'' etykieta_id" _
                & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where " _
                & " zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat" _
                & " and zg.data_zam like '" & TBRocznik.Text.ToString & "%'" _
                & " union all" _
                & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                & " zg.data_zam, dm.login, 'P' typ_oper, " _
                & " dm.status,dm.mag,to_char(dm.autodata_zak,'RRRR/MM/DD HH24:MI:SS') data_zak, case when etykieta_id is not null then 'X' else '' end etykieta_id" _
                & " from dp_swm_mia_pak dm, dp_swm_mia_zog zg where " _
                & " zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat" _
                & " and zg.data_zam like '" & TBRocznik.Text.ToString & "%'" _
                & " ) z where z.nr_zamow like '%" & filtr & "%' or z.nr_zamow_o like '%" & filtr & "%' or z.status like '%" & filtr & "%' or z.typ_oper like '%" & filtr & "%' or z.schemat like '%" & filtr & "%' or z.login like '%" & filtr & "%' order by substr(z.nr_zamow,11,2) DESC,substr(z.nr_zamow,0,9) DESC"
            Else
                sqlexp = "select rownum as lp, z.* from (" _
                & " select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                & " zg.data_zam, dm.login, 'M' typ_oper, " _
                & " dm.status,dm.mag,to_char(dm.autodata_zak,'RRRR/MM/DD HH24:MI:SS') data_zak,'' etykieta_id" _
                & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where " _
                & " zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat" _
                & " and zg.data_zam like '" & TBRocznik.Text.ToString & "%'" _
                & " union all" _
                & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                & " zg.data_zam, dm.login, 'P' typ_oper, " _
                & " dm.status,dm.mag,to_char(dm.autodata_zak,'RRRR/MM/DD HH24:MI:SS') data_zak, case when etykieta_id is not null then 'X' else '' end etykieta_id" _
                & " from dp_swm_mia_pak dm, dp_swm_mia_zog zg where " _
                & " zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat" _
                & " and zg.data_zam like '" & TBRocznik.Text.ToString & "%'" _
                & " ) z order by substr(z.nr_zamow,11,2) DESC,substr(z.nr_zamow,0,9) DESC"
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
            Dim status As String = e.Row.Cells(8).Text.ToString
            If status = "MG" Then : e.Row.BackColor = LMagazyn.BackColor
            ElseIf status = "PA" Then : e.Row.BackColor = LPakowanie.BackColor
            ElseIf status = "PE" Then : e.Row.BackColor = LSpakowane.BackColor
            ElseIf status = "WN" Then : e.Row.BackColor = LWznowione.BackColor
            ElseIf status = "ZW" Then : e.Row.BackColor = LWstrzymane.BackColor
            ElseIf status = "RP" Then : e.Row.BackColor = LPilne.BackColor
            ElseIf status = "HB" Then : e.Row.BackColor = LBlokadaNaHippo.BackColor
            ElseIf status = "BB" Then : e.Row.BackColor = LBlokadaNaBurek.BackColor
            ElseIf status = "PP" Then : e.Row.BackColor = LPrzeslanoNaDocelowy.BackColor
            ElseIf status = "RW" Then : e.Row.BackColor = LRezygnacjaWlasna.BackColor
            End If

            Dim etykieta_id As String = e.Row.Cells(11).Text.ToString
            If etykieta_id <> "" And etykieta_id <> "&nbsp;" And status = "PA" Then
                e.Row.BackColor = LPodjete.BackColor
            End If
        End If
    End Sub

    Protected Sub GridViewDyspozycje_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewDyspozycje.SelectedIndexChanged
        If GridViewDyspozycje.Rows.Count > 0 Then
            Session("kod_dyspo") = GridViewDyspozycje.SelectedRow.Cells(2).Text
            Session("schemat_dyspo") = GridViewDyspozycje.SelectedRow.Cells(3).Text
            Session("login_dyspo") = GridViewDyspozycje.SelectedRow.Cells(6).Text
            Session("typ_dyspo") = GridViewDyspozycje.SelectedRow.Cells(7).Text
            Session("status_dyspo") = GridViewDyspozycje.SelectedRow.Cells(8).Text
            Session("mag_dyspo") = GridViewDyspozycje.SelectedRow.Cells(9).Text
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

    Protected Sub TBSzukajNrZamow_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBSzukajNrZamow.TextChanged
        Dim szukaj_data As String = TBSzukajNrZamow.Text
        If szukaj_data <> "" Then
            Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                conn.Open()

                If szukaj_data <> "" Then
                    sqlexp = "select rownum as lp, z.* from (" _
                    & " select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                    & " zg.data_zam, dm.login, 'M' typ_oper, " _
                    & " dm.status,dm.mag,to_char(dm.autodata_zak,'RRRR/MM/DD HH24:MI:SS') data_zak,'' etykieta_id" _
                    & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where " _
                    & " zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat" _
                    & " union all" _
                    & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                    & " zg.data_zam, dm.login, 'P' typ_oper, " _
                    & " dm.status,dm.mag,to_char(dm.autodata_zak,'RRRR/MM/DD HH24:MI:SS') data_zak, case when etykieta_id is not null then 'X' else '' end etykieta_id" _
                    & " from dp_swm_mia_pak dm, dp_swm_mia_zog zg where " _
                    & " zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat" _
                    & " ) z where z.nr_zamow like '%" & szukaj_data & "%' order by substr(z.nr_zamow,11,2) DESC,substr(z.nr_zamow,0,9) DESC"
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

                If dsPartie.Tables(0).Rows.Count = 0 Then : LIleDokumentow.Text = "brak"
                Else : LIleDokumentow.Text = dsPartie.Tables(0).Rows.Count.ToString
                End If
                conn.Close()
            End Using
        Else
            LadujDaneGridViewDyspozycje("")
        End If
    End Sub
End Class