Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class a_label
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
                TBRocznik.Text = DateTime.Now.Year.ToString & "/" & DateTime.Now.Month.ToString("D2")
                LadujDaneGridViewDyspozycje(TBFiltrowanie.Text)
                If Session("shipment_date") IsNot Nothing Then
                    TBShippmentDate.Text = Session("shipment_date")
                Else
                    TBShippmentDate.Text = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("D2") & "-" & DateTime.Now.Day.ToString("D2")
                End If
            End If
        End If

        Session("contentKomunikat") = Session(session_id)
    End Sub

    Public Sub LadujDaneGridViewDyspozycje(ByVal filtr As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If filtr <> "" Then
                sqlexp = "select rownum as lp,w1.nr_zamow,w1.schemat,w1.status,w1.mag,w1.shipment_id,w1.autodata_zak,w1.ile_paczek ile_opak" _
                & " ,(select sum(di.pl_weight) from dp_swm_mia_paczka_info di where di.shipment_id=w1.shipment_id) s_waga" _
                & " from (" _
                & " select w.*" _
                & " ,(select count(*) from dp_swm_mia_paczka_info where shipment_id=w.shipment_id) ile_paczek" _
                & " ,(select count(*) from dp_swm_mia_paczka_info where shipment_id=w.shipment_id and pl_type between '1' and '3') ile_dhl" _
                & " from (" _
                & " select dp.nr_zamow,dp.schemat,dp.status,dp.mag,dt.shipment_id,dp.autodata_zak" _
                & " from dp_swm_mia_pak dp, dp_swm_mia_paczka dt where dp.status='PE' and dp.etykieta_id='X' and dp.autodata_zak is not null " _
                & " and dp.nr_zamow = dt.nr_zamow and dp.schemat = dt.schemat" _
                & " order by dt.shipment_id desc,dt.nr_zamow desc) w" _
                & " )w1 where w1.shipment_id is not null and w1.ile_paczek >= 0 and w1.ile_dhl >= 0 and to_char(w1.autodata_zak,'YYYY/MM') = '" & TBRocznik.Text & "'" _
                & " and (w1.nr_zamow like '%" & filtr & "%' or w1.schemat like '%" & filtr & "%' or w1.status like '%" & filtr & "%' or w1.shipment_id like '%" & filtr & "%'" & Session("filter") & ")"
            Else
                sqlexp = "select rownum as lp,w1.nr_zamow,w1.schemat,w1.status,w1.mag,w1.shipment_id,w1.autodata_zak,w1.ile_paczek ile_opak" _
                & " ,(select sum(di.pl_weight) from dp_swm_mia_paczka_info di where di.shipment_id=w1.shipment_id) s_waga" _
                & " from (" _
                & " select w.*" _
                & " ,(select count(*) from dp_swm_mia_paczka_info where shipment_id=w.shipment_id) ile_paczek" _
                & " ,(select count(*) from dp_swm_mia_paczka_info where shipment_id=w.shipment_id and pl_type between '1' and '3') ile_dhl" _
                & " from (" _
                & " select dp.nr_zamow,dp.schemat,dp.status,dp.mag,dt.shipment_id,dp.autodata_zak" _
                & " from dp_swm_mia_pak dp, dp_swm_mia_paczka dt where dp.status='PE' and dp.etykieta_id='X' and dp.autodata_zak is not null " _
                & " and dp.nr_zamow = dt.nr_zamow and dp.schemat = dt.schemat" _
                & " order by dt.shipment_id desc,dt.nr_zamow desc) w" _
                & " )w1 where w1.shipment_id is not null and w1.ile_paczek >= 0 and w1.ile_dhl >= 0 and to_char(w1.autodata_zak,'YYYY/MM') = '" & TBRocznik.Text & "'" & Session("filter")
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
            Dim status As String = e.Row.Cells(6).Text.ToString
            If status.Contains("SH") Then
                e.Row.BackColor = LPaczkaStandard.BackColor
            Else
                e.Row.BackColor = LPaczkaDHL.BackColor
            End If
        End If
    End Sub

    Protected Sub GridViewDyspozycje_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewDyspozycje.SelectedIndexChanged
        If GridViewDyspozycje.Rows.Count > 0 Then
            Session("kod_dyspo") = GridViewDyspozycje.SelectedRow.Cells(2).Text
            Session("schemat_dyspo") = GridViewDyspozycje.SelectedRow.Cells(3).Text
            Session("login_dyspo") = GridViewDyspozycje.SelectedRow.Cells(2).Text
            Session("mag_dyspo") = GridViewDyspozycje.SelectedRow.Cells(5).Text
            Session("shipment_id") = GridViewDyspozycje.SelectedRow.Cells(6).Text
            Response.Redirect("a_labelEdit.aspx")
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

    Protected Sub TBShippmentDate_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBShippmentDate.TextChanged
        Dim shippmentDate As String = TBShippmentDate.Text
        If shippmentDate <> "" Then
            Session("shipment_date") = TBShippmentDate.Text.ToString
        Else
            Session.Remove("shipment_date")
        End If
    End Sub

    Protected Sub CBFiltrowanieDHL_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBFiltrowanieDHL.CheckedChanged
        If CBFiltrowanieDHL.Checked Then
            Session("filter") = " and w1.shipment_id not like 'S%'"
        Else
            Session("filter") = " and 1=1"
        End If

        LadujDaneGridViewDyspozycje("")
    End Sub
End Class