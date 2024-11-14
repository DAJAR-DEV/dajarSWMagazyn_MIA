Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class Logged
    Inherits System.Web.UI.Page
    Dim sqlexp As String = ""
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder

    Protected Sub GridViewPartie_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewPartie.PageIndexChanging
        GridViewPartie.PageIndex = e.NewPageIndex
        GridViewPartie.DataBind()
        LadujDaneGridViewPartie()
    End Sub

    Protected Sub DDLOperator_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLOperator.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim login As String = DDLOperator.SelectedValue.ToString

            If login <> "WYBIERZ OPERATORA" Then
                Session.RemoveAll()
                sqlexp = "select hash from dp_swm_mia_uzyt where login = '" & login & "'"
                Dim hash As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                Session("mylogin") = login
                Session("myhash") = hash
                sqlexp = "select trim(TYP_OPER) typ_oper FROM dp_swm_mia_UZYT WHERE LOGIN = '" & login & "'"
                Session("mytyp_oper") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                Session("contentHash") = "SESJA MIASTKO : " & hash
                Session("contentOperator") = "STANOWISKO W STREFIE : " & Session("mytyp_oper")
                Session.Remove("contentKomunikat")

                ''####2022.10.11 / automatycznie przenoszenie operatora P do zakladki pakowanie
                If Session("mytyp_oper") = "P" Then
                    Response.Redirect("package.aspx")
                End If

                Response.Redirect("logged.aspx")
            Else
                dajarSWMagazyn_MIA.MyFunction.ClearSessionInformation(Session)
                Response.Redirect("logged.aspx")
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)

        If Session("mylogin") = Nothing And Session("myhash") = Nothing Then
            Session.Abandon()
            Response.Redirect("index.aspx")
        End If

        If Session("mytyp_oper") = "W" Then
            Response.Redirect("address.aspx")
        End If

        If Session("mytyp_oper") = "O" Then
            Response.Redirect("storage.aspx")
        End If

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session("contentMagazyn") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN = '" & Session("mylogin") & "'", conn)
            conn.Close()
        End Using

        If Not Page.IsPostBack Then
            dajarSWMagazyn_MIA.MyFunction.RefreshDDLOperator(DDLOperator, Session)
            LadujDaneGridViewPartie()
        End If

        Session("contentKomunikat") = Session(session_id)
    End Sub

    Public Sub LadujDaneGridViewPartie()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If Session("contentMagazyn") = "700" Then
                If Session("mytyp_oper") = "M" Or Session("mytyp_oper") = "O" Or Session("mytyp_oper") = "W" Or Session("mytyp_oper") = "MO" Or Session("mytyp_oper") = "MP" Or Session("mytyp_oper") = "PM" Then
                    sqlexp = "select z.nr_zamow,z.schemat,z.nr_zamow_o,z.ile_pozycji,z.ile_szt,to_char(z.data_mag,'YYYY/MM/DD') data_mag,z.typ_oper,z.status,z.mag,z.mag_cel,z.wydruk_id from (" _
                    & " select w.nr_zamow, w.schemat, w.nr_zamow_o, count(w.skrot) ile_pozycji, sum(w.ile_poz) ile_szt, w.data_mag, 'O' typ_oper, w.status, w.mag," _
                    & " (select dz.mag from dp_swm_mia_zog dz where dz.nr_zamow = w.nr_zamow and dz.schemat = w.schemat and dz.schemat = w.schemat) mag_cel," _
                    & " (select count(db.nr_zamow) from dp_swm_mia_buf db where db.nr_zamow = w.nr_zamow and db.schemat = w.schemat and db.dyspozycja='N') ile_do_losowania," _
                    & " (select dw.wydruk_id from dp_swm_mia_wydruk dw where dw.nr_zamow = w.nr_zamow and dw.schemat = w.schemat and dw.login='" & Session("mylogin") & "' and dw.typ_wydruk='BI' and rownum=1) wydruk_id from (" _
                    & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                    & " zg.autodata data_mag, 'O' typ_oper, " _
                    & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag" _
                    & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.login = '" & Session("mylogin") & "' and dm.mag = 700 and dm.status in('MG','HB','BB','WN','ZW')" _
                    & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat" _
                    & " ) w group by w.nr_zamow, w.schemat, w.nr_zamow_o, w.data_mag, w.status, w.mag " _
                    & " ) z where z.ile_do_losowania = 0" _
                    & " order by z.nr_zamow desc"
                ElseIf Session("mytyp_oper") = "ME" Then
                    sqlexp = "select z.nr_zamow,z.schemat,z.nr_zamow_o,z.ile_pozycji,z.ile_szt,to_char(z.data_mag,'YYYY/MM/DD') data_mag,z.typ_oper,z.status,z.mag,z.mag_cel,z.wydruk_id from (" _
                    & " select w.nr_zamow, w.schemat, w.nr_zamow_o, count(w.skrot) ile_pozycji, sum(w.ile_poz) ile_szt, w.data_mag, 'ME' typ_oper, w.status, w.mag," _
                    & " (select dz.mag from dp_swm_mia_zog dz where dz.nr_zamow = w.nr_zamow and dz.schemat = w.schemat and dz.schemat = w.schemat) mag_cel," _
                    & " (select count(db.nr_zamow) from dp_swm_mia_buf db where db.nr_zamow = w.nr_zamow and db.schemat = w.schemat and db.dyspozycja='N') ile_do_losowania," _
                    & " (select dw.wydruk_id from dp_swm_mia_wydruk dw where dw.nr_zamow = w.nr_zamow and dw.schemat = w.schemat and dw.login='" & Session("mylogin") & "' and dw.typ_wydruk='BI' and rownum=1) wydruk_id from (" _
                    & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                    & " zg.autodata data_mag, 'ME' typ_oper, " _
                    & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag" _
                    & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.login = '" & Session("mylogin") & "' and dm.mag = 700 and dm.status in('MG','HB','BB','WN','ZW')" _
                    & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat" _
                    & " ) w group by w.nr_zamow, w.schemat, w.nr_zamow_o, w.data_mag, w.status, w.mag " _
                    & " ) z where z.ile_do_losowania = 0" _
                    & " order by z.nr_zamow desc"
                ElseIf Session("mytyp_oper") = "RM" Then
                    sqlexp = "select z.nr_zamow,z.schemat,z.nr_zamow_o,z.ile_pozycji,z.ile_szt,to_char(z.data_mag,'YYYY/MM/DD') data_mag,z.typ_oper,z.status,z.mag,z.mag_cel,z.wydruk_id from (" _
                    & " select w.nr_zamow, w.schemat, w.nr_zamow_o, count(w.skrot) ile_pozycji, sum(w.ile_poz) ile_szt, w.data_mag, 'RM' typ_oper, w.status, w.mag," _
                    & " (select dz.mag from dp_swm_mia_zog dz where dz.nr_zamow = w.nr_zamow and dz.schemat = w.schemat and dz.schemat = w.schemat) mag_cel," _
                    & " (select count(db.nr_zamow) from dp_swm_mia_buf db where db.nr_zamow = w.nr_zamow and db.schemat = w.schemat and db.dyspozycja='N') ile_do_losowania," _
                    & " (select dw.wydruk_id from dp_swm_mia_wydruk dw where dw.nr_zamow = w.nr_zamow and dw.schemat = w.schemat and dw.login='" & Session("mylogin") & "' and dw.typ_wydruk='BI' and rownum=1) wydruk_id from (" _
                    & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                    & " zg.autodata data_mag, 'RM' typ_oper, " _
                    & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag" _
                    & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.login = '" & Session("mylogin") & "' and dm.mag = 700 and dm.status in('MG','HB','BB','WN','ZW')" _
                    & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat" _
                    & " ) w group by w.nr_zamow, w.schemat, w.nr_zamow_o, w.data_mag, w.status, w.mag " _
                    & " ) z where z.ile_do_losowania = 0" _
                    & " order by z.nr_zamow desc"
                ElseIf Session("mytyp_oper") = "P" Then
                    sqlexp = "select w.nr_zamow,w.schemat,w.nr_zamow_o,w.ile_poz ile_700,w.ile_obcy ile_46,to_char(w.data_mag,'YYYY/MM/DD') data_mag,w.typ_oper,w.stat_u1,w.etykieta_id,w.mag,(select dz.mag from dp_swm_mia_zog dz where dz.nr_zamow = w.nr_zamow and dz.schemat = w.schemat) mag_cel  from (" _
                    & " select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dm.nr_zamow and schemat = dm.schemat and status in ('PA') and mag = 700) ile_poz," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dm.nr_zamow and schemat = dm.schemat and mag <> 700) ile_obcy," _
                    & " zg.autodata data_mag, 'P' typ_oper,'' etykieta_id, dm.mag,zg.stat_u1" _
                    & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.status in ('PA') and zg.nr_zamow not in(select distinct nr_zamow from dp_swm_mia_pak where nr_zamow = dm.nr_zamow and schemat = dm.schemat)" _
                    & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat and dm.mag = 700" _
                    & " union all" _
                    & " select distinct dp.nr_zamow,dp.schemat,zg.nr_zamow_o," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dp.nr_zamow and status in ('PA') and mag=700) ile_poz," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dp.nr_zamow and mag <> 700) ile_obcy," _
                    & " zg.autodata data_mag, 'P' typ_oper,etykieta_id,'700',zg.stat_u1" _
                    & " from dp_swm_mia_pak dp, dp_swm_mia_zog zg where dp.login = '" & Session("mylogin") & "' and dp.status in ('PA')" _
                    & " and zg.nr_zamow = dp.nr_zamow and zg.schemat = dp.schemat" _
                    & " ) w left join dp_rest_mag_order o on w.nr_zamow=o.nr_zamow_dt where (o.shipping_desc not like '%Paczkomaty%' or o.shipping_address not like '%Paczkomat%' or o.shipping_address not like '%PaczkoPunkt%')" _
                    & " order by w.nr_zamow desc"
                ElseIf Session("mytyp_oper") = "PP" Then
                    sqlexp = "select w.nr_zamow,w.schemat,w.nr_zamow_o,w.ile_poz ile_700,w.ile_obcy ile_46,to_char(w.data_mag,'YYYY/MM/DD') data_mag,w.typ_oper,w.stat_u1,w.etykieta_id,w.mag,(select dz.mag from dp_swm_mia_zog dz where dz.nr_zamow = w.nr_zamow and dz.schemat = w.schemat) mag_cel  from (" _
                    & " select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dm.nr_zamow and schemat = dm.schemat and status in ('PA') and mag = 700) ile_poz," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dm.nr_zamow and schemat = dm.schemat and mag <> 700) ile_obcy," _
                    & " zg.autodata data_mag, 'P' typ_oper,'' etykieta_id, dm.mag,zg.stat_u1" _
                    & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.status in ('PA') and zg.nr_zamow not in(select distinct nr_zamow from dp_swm_mia_pak where nr_zamow = dm.nr_zamow and schemat = dm.schemat)" _
                    & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat and dm.mag = 700" _
                    & " union all" _
                    & " select distinct dp.nr_zamow,dp.schemat,zg.nr_zamow_o," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dp.nr_zamow and status in ('PA') and mag=700) ile_poz," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dp.nr_zamow and mag <> 700) ile_obcy," _
                    & " zg.autodata data_mag, 'P' typ_oper,etykieta_id,'700',zg.stat_u1" _
                    & " from dp_swm_mia_pak dp, dp_swm_mia_zog zg where dp.login = '" & Session("mylogin") & "' and dp.status in ('PA')" _
                    & " and zg.nr_zamow = dp.nr_zamow and zg.schemat = dp.schemat" _
                    & " ) w left join dp_rest_mag_order o on w.nr_zamow=o.nr_zamow_dt where (o.shipping_desc like '%Paczkomaty%' or o.shipping_address like '%Paczkomat%' or o.shipping_address like '%PaczkoPunkt%')" _
                    & " order by w.nr_zamow desc"
                End If
            ElseIf Session("contentMagazyn") = "46" Then
                If Session("mytyp_oper") = "M" Or Session("mytyp_oper") = "O" Or Session("mytyp_oper") = "W" Or Session("mytyp_oper") = "MO" Or Session("mytyp_oper") = "MP" Then
                    sqlexp = "select z.nr_zamow,z.schemat,z.nr_zamow_o,z.ile_pozycji,z.ile_szt,to_char(z.data_mag,'YYYY/MM/DD') data_mag,z.typ_oper,z.status,z.mag,z.mag_cel,z.wydruk_id from (" _
                    & " select w.nr_zamow, w.schemat, w.nr_zamow_o, count(w.skrot) ile_pozycji, sum(w.ile_poz) ile_szt, w.data_mag, 'O' typ_oper, w.status, w.mag," _
                    & " (select dz.mag from dp_swm_mia_zog dz where dz.nr_zamow = w.nr_zamow and dz.schemat = w.schemat) mag_cel," _
                    & " (select count(db.nr_zamow) from dp_swm_mia_buf db where db.nr_zamow = w.nr_zamow and db.schemat = w.schemat and db.dyspozycja='N') ile_do_losowania," _
                    & " (select dw.wydruk_id from dp_swm_mia_wydruk dw where dw.nr_zamow = w.nr_zamow and dw.schemat = w.schemat and dw.login='" & Session("mylogin") & "' and dw.typ_wydruk='HI' and rownum=1) wydruk_id from (" _
                    & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                    & " zg.autodata data_mag, 'O' typ_oper, " _
                    & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag" _
                    & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.login = '" & Session("mylogin") & "' and dm.mag = 46 and dm.status in('MG','HB','BB','WN','ZW')" _
                    & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat" _
                    & " ) w group by w.nr_zamow, w.schemat, w.nr_zamow_o, w.data_mag, w.status, w.mag " _
                    & " ) z where z.ile_do_losowania = 0" _
                    & " order by z.nr_zamow desc"
                ElseIf Session("mytyp_oper") = "P" Then
                    sqlexp = "select w.nr_zamow,w.schemat,w.nr_zamow_o,w.ile_poz ile_46,w.ile_obcy ile_43,to_char(w.data_mag,'YYYY/MM/DD') data_mag,w.typ_oper,w.stat_u1,w.etykieta_id,w.mag,(select dz.mag from dp_swm_mia_zog dz where dz.nr_zamow = w.nr_zamow and dz.schemat = w.schemat) mag_cel  from (" _
                    & " select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dm.nr_zamow and schemat = dm.schemat and status in ('PA') and mag = 46) ile_poz," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dm.nr_zamow and schemat = dm.schemat and mag <> 46) ile_obcy," _
                    & " zg.autodata data_mag, 'P' typ_oper,'' etykieta_id, dm.mag,zg.stat_u1" _
                    & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.status in ('PA') and zg.nr_zamow not in(select distinct nr_zamow from dp_swm_mia_pak where nr_zamow = dm.nr_zamow and schemat = dm.schemat)" _
                    & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat and dm.mag = 46" _
                    & " union all" _
                    & " select distinct dp.nr_zamow,dp.schemat,zg.nr_zamow_o," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dp.nr_zamow and status in ('PA') and mag=46) ile_poz," _
                    & " (select count(*) from dp_swm_mia_mag where nr_zamow = dp.nr_zamow and mag <> 46) ile_obcy," _
                    & " zg.autodata data_mag, 'P' typ_oper,etykieta_id,'46',zg.stat_u1" _
                    & " from dp_swm_mia_pak dp, dp_swm_mia_zog zg where dp.login = '" & Session("mylogin") & "' and dp.status in ('PA')" _
                    & " and zg.nr_zamow = dp.nr_zamow and zg.schemat = dp.schemat" _
                    & " ) w" _
                    & " order by w.nr_zamow desc"
                End If

            End If

            GridViewPartie.DataSource = Nothing
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridViewPartie.DataSource = dsPartie.Tables(0)
            GridViewPartie.DataBind()
            cmd.Dispose()

            If dsPartie.Tables(0).Rows.Count = 0 Then : LIleDokumentow.Text = "brak"
            Else : LIleDokumentow.Text = dsPartie.Tables(0).Rows.Count.ToString
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub GridViewPartie_aktualizacja(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewPartie.RowDataBound
        ''AktualizujGridViewPartieStatusy()
        If e.Row.RowType = DataControlRowType.DataRow Then
            If Session("mytyp_oper") = "M" Or Session("mytyp_oper") = "O" Or Session("mytyp_oper") = "P" Or Session("mytyp_oper") = "MO" Or Session("mytyp_oper") = "MP" Or Session("mytyp_oper") = "ME" Then
                Dim status As String = e.Row.Cells(7).Text.ToString
                If status = "MG" Then : e.Row.BackColor = LMagazyn.BackColor
                ElseIf status = "PA" Then : e.Row.BackColor = LPakowanie.BackColor
                ElseIf status = "WN" Then : e.Row.BackColor = LWznowione.BackColor
                ElseIf status = "ZW" Then : e.Row.BackColor = LWstrzymane.BackColor
                ElseIf status = "HB" Then : e.Row.BackColor = LBlokadaNaHippo.BackColor
                ElseIf status = "BB" Then : e.Row.BackColor = LBlokadaNaBurek.BackColor
                End If
            ElseIf Session("mytyp_oper") = "P" Then
                Dim et_id As String = e.Row.Cells(7).Text.ToString
                If et_id.Length = 12 Then e.Row.BackColor = LPodjete.BackColor
            End If
        End If
        
        
        If Session("mytyp_oper") = "P" Then
'            If e.Row.RowType = DataControlRowType.DataRow Then
'                Dim dataToSend = e.Row.Cells(0).Text
'                e.Row.Attributes("onclick") = String.Format("location.href='package.aspx?data={0}';", dataToSend)
'                e.Row.Style("cursor") = "pointer"
'            End If
            Response.Redirect("package.aspx")
        End If
    End Sub

    Protected Sub BRefreshPage_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BRefreshPage.Click
        Response.Redirect(Request.RawUrl)
    End Sub
End Class