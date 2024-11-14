Imports Oracle.ManagedDataAccess.Client
Imports System.Data

Partial Public Class storage
    Inherits System.Web.UI.Page
    Dim sqlexp As String = ""
    Dim result As Boolean = False
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder

    Class ZamowieniaInformacje
        Public nr_zamow As String = ""
        Public schemat As String = ""

        Public Sub New(ByVal _nr_zamow As String, ByVal _schemat As String)
            nr_zamow = _nr_zamow
            schemat = _schemat
        End Sub
    End Class
    
    Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
        For i As Integer = 0 To GridViewMagazyn.Rows.Count - 1
            ClientScript.RegisterForEventValidation(GridViewMagazyn.UniqueID, "Select$" & i)
        Next
        MyBase.Render(writer)
    End Sub

    Protected Sub GridViewMagazyn_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewMagazyn.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewMagazyn.PageIndex = e.NewPageIndex
        GridViewMagazyn.DataBind()
        LadujDaneGridViewMagazyn()
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
                Response.Redirect("logged.aspx")
            Else
                dajarSWMagazyn_MIA.MyFunction.ClearSessionInformation(Session)
                Response.Redirect("logged.aspx")
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub page_loaded() Handles Me.LoadComplete
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session("contentKomunikat") = Session(session_id)

        If Session("mm_sel_index") IsNot Nothing And GridViewMagazyn.Rows.Count > 0 Then
            Dim lista_czytnik As New List(Of ZamowieniaInformacje)
            lista_czytnik = Session("mm_sel_index")

            For Each l In lista_czytnik
                For Each row As GridViewRow In GridViewMagazyn.Rows
                    Dim r_nr_zamow As String = row.Cells(2).Text.ToString
                    Dim r_schemat As String = row.Cells(3).Text.ToString

                    If l.nr_zamow = r_nr_zamow Then
                        GridViewMagazyn.SelectedIndex = row.RowIndex
                        Dim cb As CheckBox = GridViewMagazyn.Rows(row.RowIndex).FindControl("CBKodSelect")
                        cb.Checked = True
                    End If
                Next
            Next

            WyswietlSzczegolyZamowienia()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Dim przerwijLadowanie As Boolean = False

        BZakonczMagazyn.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zamknąć wybrane dokumenty?\n') == false) return false")

        If Session("mylogin") = Nothing And Session("myhash") = Nothing Then
            Session.Abandon()
            Response.Redirect("index.aspx")
        ElseIf Session("mytyp_oper") <> "M" And Session("mytyp_oper") <> "O" And Session("mytyp_oper") <> "MO" And Session("mytyp_oper") <> "MP" And Session("mytyp_oper") <> "PM" And Session("mytyp_oper") <> "RM" And Session("mytyp_oper") <> "ME" Then
            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
            Session(session_id) += "<br />Zaloguj sie na opowiedniego operatora - typ MAGAZYN/OGROD/MAGAZYN-OGROD/MAGAZYN-PODUSZKA/MAGAZYN-PACZKOMAT/PAKOWANIE-PACZKOMAT/REKLAMACJA-MAGAZYN/MAGAZYN-EXPORT<br />"
            Session(session_id) += "</div>"
            PanelMagazyn.Visible = False
            przerwijLadowanie = True
            ''Else
            ''Session.Remove(session_id)
        End If

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session("contentMagazyn") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN = '" & Session("mylogin") & "'", conn)
            conn.Close()
        End Using

        If Not Page.IsPostBack Then
            dajarSWMagazyn_MIA.MyFunction.RefreshDDLOperator(DDLOperator, Session)
            If przerwijLadowanie = False Then
                LadujDaneGridViewMagazyn()
            End If
        End If

        BZaznaczWszystkie.Enabled = False
        BOdznaczWszystkie.Enabled = False
        Page.SetFocus(TBEtykieta)
        Session("contentKomunikat") = Session(session_id)
    End Sub

    Public Sub LadujDaneGridViewMagazyn()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("contentMagazyn") = "700" Then
                If Session("mytyp_oper") = "M" Or Session("mytyp_oper") = "O" Or Session("mytyp_oper") = "MO" Or Session("mytyp_oper") = "MP" Or Session("mytyp_oper") = "PM" Or Session("mytyp_oper") = "RM" Or Session("mytyp_oper") = "ME" Then
                    sqlexp = "select z.nr_zamow,z.schemat,z.nr_zamow_o,z.typ_oper,z.wydruk_id from (" _
                    & " select w.nr_zamow, w.schemat, w.nr_zamow_o, count(w.skrot) ile_pozycji, sum(w.ile_poz) ile_szt, w.data_mag, '" & Session("mytyp_oper") & "' typ_oper, w.status, w.mag," _
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
                End If
            ElseIf Session("contentMagazyn") = "46" Then
                If Session("mytyp_oper") = "M" Or Session("mytyp_oper") = "O" Or Session("mytyp_oper") = "MO" Or Session("mytyp_oper") = "MP" Then
                    sqlexp = "select z.nr_zamow,z.schemat,z.nr_zamow_o,z.ile_pozycji,z.ile_szt,z.typ_oper,z.status,z.mag from (" _
                    & " select w.nr_zamow, w.schemat, w.nr_zamow_o, count(w.skrot) ile_pozycji, sum(w.ile_poz) ile_szt, '" & Session("mytyp_oper") & "' typ_oper, w.status, w.mag," _
                    & " (select count(db.nr_zamow) from dp_swm_mia_buf db where db.nr_zamow = w.nr_zamow and db.schemat = w.schemat and db.dyspozycja='N') ile_do_losowania from (" _
                    & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                    & " zg.data_zam, '" & Session("mytyp_oper") & "' typ_oper, " _
                    & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag,(select dz.mag from dp_swm_mia_zog dz where dz.nr_zamow = dm.nr_zamow and dz.schemat = dm.schemat) mag_cel" _
                    & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.login = '" & Session("mylogin") & "' and dm.status in('MG','WN')" _
                    & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat and dm.mag = 46" _
                    & " ) w where w.status = 'WN' or w.status = 'MG' group by w.nr_zamow, w.schemat, w.nr_zamow_o, w.status, w.mag, w.mag_cel" _
                    & " ) z where z.ile_do_losowania = 0 order by z.nr_zamow desc"
                End If
            End If

            GridViewMagazyn.DataSource = Nothing
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridViewMagazyn.DataSource = dsPartie.Tables(0)
            GridViewMagazyn.DataBind()
            cmd.Dispose()

            If dsPartie.Tables(0).Rows.Count = 0 Then
                LIleDokumentow.Text = "brak"
                PanelMagazynData.Visible = False
            Else
                LIleDokumentow.Text = dsPartie.Tables(0).Rows.Count.ToString
                PanelMagazynData.Visible = True
            End If
            conn.Close()
        End Using
    End Sub


    Protected Sub GridViewMagazyn_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles GridViewMagazyn.RowCommand
        If e.CommandName = "Select" Then
            Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
            GridViewMagazyn.SelectedIndex = rowIndex
            For Each row As GridViewRow In GridViewMagazyn.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                cb.Checked = False
            Next
        End If
    End Sub

    Public Sub WyswietlSzczegolyZamowienia()
        Dim generujGridView As Boolean = False
        Dim lista_zamowien As New List(Of ZamowieniaInformacje)

        If GridViewMagazyn.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewMagazyn.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                If cb IsNot Nothing And cb.Checked Then
                    generujGridView = True
                    Dim schemat As String = row.Cells(3).Text.ToString
                    Dim nr_zamow As String = row.Cells(2).Text.ToString
                    lista_zamowien.Add(New ZamowieniaInformacje(nr_zamow, schemat))

                    Session("kod_dyspo") = nr_zamow
                    Session("schemat_dyspo") = schemat
                End If
            Next
        End If

        If generujGridView = True Then
            AktualizujGridViewIndeksy(lista_zamowien)
        Else
            GridViewIndeksy.DataSource = Nothing
            GridViewIndeksy.DataBind()
        End If
    End Sub

    Protected Sub BWyswietlSzczegoly_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWyswietlSzczegoly.Click
        WyswietlSzczegolyZamowienia()
    End Sub

    Public Sub AktualizujGridViewIndeksy(ByVal lista_zamowien As List(Of ZamowieniaInformacje))
        GridViewIndeksy.DataSource = Nothing
        Dim indeksy_dtable As New DataTable
        Dim polaDataTable As String() = {"LP", "NR_ZAMOW", "SCHEMAT", "NR_ZAMOW_O", "SKROT", "NAZWA", "KOD_TOW", "ILE_POZ", "JM", "MAG", "STATUS"}
        For Each pole In polaDataTable
            indeksy_dtable.Columns.Add(New DataColumn(pole, Type.GetType("System.String")))
        Next

        Dim sqlexp_DAJAR As String = ""
        Dim sqlexp_DOMINUS As String = ""

        For Each obj_zam In lista_zamowien
            If obj_zam.schemat = "DAJAR" Or obj_zam.schemat = "DOMINUS" Then
                sqlexp_DAJAR &= " union all" _
                & " select rownum as lp,w.nr_zamow, w.schemat, w.nr_zamow_o, w.skrot, desql_japa_nwa.fsql_japa_rnaz(get_index_tow(w.skrot)) nazwa, " _
                & " desql_japa_nwa.fsql_japa_rkod(get_index_tow(w.skrot)) kod_tow, w.ile_poz, w.jm, w.mag, w.status from (" _
                & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                & " zg.data_zam, '" & Session("mytyp_oper") & "' typ_oper," _
                & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag," _
                & " (select distinct zd.jm from ht_zod zd where zd.ie$0 like dm.nr_zamow||'%' and zd.is_deleted = 'N' and zd.skrot = dm.skrot) jm" _
                & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.login = '" & Session("mylogin") & "' and dm.status in('MG','WN')" _
                & " and dm.nr_zamow in('" & obj_zam.nr_zamow & "') and dm.schemat='" & obj_zam.schemat & "'" _
                & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat order by dm.nr_zamow" _
                & " ) w"
            End If
        Next

        If sqlexp_DAJAR <> "" Then
            sqlexp_DAJAR = sqlexp_DAJAR.Substring(10, sqlexp_DAJAR.Length - 10)
            Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                conn.Open()
                Dim cmd As New OracleCommand(sqlexp_DAJAR, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                dr = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        Dim indeksy_drow As DataRow
                        indeksy_drow = indeksy_dtable.NewRow
                        Dim pole_pozycja As Integer = 0
                        For Each pole In polaDataTable
                            indeksy_drow(pole) = dr.Item(pole_pozycja).ToString
                            pole_pozycja += 1
                        Next

                        indeksy_dtable.Rows.Add(indeksy_drow)
                    End While

                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()
                conn.Close()
            End Using
        End If

        GridViewIndeksy.DataSource = indeksy_dtable
        GridViewIndeksy.DataBind()
    End Sub

    Protected Sub BOdznaczWszystkie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BOdznaczWszystkie.Click
        If GridViewMagazyn.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewMagazyn.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                cb.Checked = False
            Next
        End If
    End Sub

    Protected Sub BZaznaczWszystkie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BZaznaczWszystkie.Click
        If GridViewMagazyn.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewMagazyn.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                cb.Checked = True
            Next
        End If
    End Sub

    Protected Sub BZakonczMagazyn_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BZakonczMagazyn.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("kod_dyspo") IsNot Nothing And Session("schemat_dyspo") IsNot Nothing Then
                Dim nr_zamow As String = Session("kod_dyspo").ToString
                Dim schemat As String = Session("schemat_dyspo").ToString
                Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()

                sqlexp = "update dp_swm_mia_mag set status = 'PA', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS')" _
                & " where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and login = '" & Session("mylogin") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                ''''result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PA", nr_zamow)
                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PA" & "[" & schemat & "][" & Session("contentMagazyn") & "]", nr_zamow)

                dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(nr_zamow, schemat, "PA")
            End If

            If GridViewMagazyn.Rows.Count > 0 Then
                For Each row As GridViewRow In GridViewMagazyn.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim schemat As String = row.Cells(3).Text.ToString
                        Dim nr_zamow As String = row.Cells(2).Text.ToString
                        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                        sqlexp = "update dp_swm_mia_mag set status = 'PA', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS')" _
                        & " where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and login = '" & Session("mylogin") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        ''''result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PA", nr_zamow)
                        result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PA" & "[" & schemat & "][" & Session("contentMagazyn") & "]", nr_zamow)

                        dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(nr_zamow, schemat, "PA")

                        '#####sprawdzenie warunku czy generowac email do biura burek / tylko magazyn 46
                        ''If Session("contentMagazyn") = 46 Then
                        ''    sqlexp = "select count(*) from dp_swm_mia_mag where nr_zamow = '" & nr_zamow & "' and mag <> 46"
                        ''    Dim ileNaMagazynieObcym As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        ''    If ileNaMagazynieObcym <> "0" Then
                        ''        '##########################################################TWORZENIE WIADOMOSCI EMAIL##################################################
                        ''        ''Dim email_adres As New List(Of String)
                        ''        ''email_adres = New List(Of String)(New String() {"michal.michalak@dajar.pl"})
                        ''        ''Dim email_titla As String = "[dajarSWMagazyn_MIA] zakonczenie przygotowywania dla zamowienia " & nr_zamow & " na magazynie 46"
                        ''        ''sqlexp = "select w.nr_zamow, w.nr_zamow_o, w.schemat, w.data_zam, w.mag, TO_CHAR(sysdate,'YYYY/MM/DD HH24:MM:SS') from (" _
                        ''        ''& " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                        ''        ''& " to_char(zg.data_fakt, 'YYYY/MM/DD') data_zam, 'M' typ_oper," _
                        ''        ''& " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag" _
                        ''        ''& " from dp_swm_mia_mag dm, ht_zog zg where dm.login = '" & Session("mylogin") & "' and dm.status in('PA')" _
                        ''        ''& " and dm.nr_zamow = '" & nr_zamow & "'" _
                        ''        ''& " and zg.ie$14 = desql_graf.df11_2(dm.nr_zamow) order by dm.nr_zamow" _
                        ''        ''& " ) w group by w.nr_zamow, w.schemat, w.nr_zamow_o, w.status, w.mag, w.data_zam"

                        ''        ''Dim email_template As String = "zakonczenie_zbierania_46.htm"
                        ''        ''Dim form_field As New List(Of String)(New String() {"(NR_ZAMOW)", "(NR_ZAMOW_O)", "(SCHEMAT)", "(DATA_ZAM)", "(MAG_ID)", "(DATA_SYSTEM)"})
                        ''        ''dajarSWMagazyn_MIA.MyFunction.PrepareToSendEmailMessage(email_adres, email_titla, email_template, form_field, sqlexp, nr_zamow, schemat, Session("mylogin"), conn, Session)
                        ''        '##########################################################TWORZENIE WIADOMOSCI EMAIL##################################################

                        ''    End If
                        ''End If
                    End If
                Next
            End If

            LadujDaneGridViewMagazyn()
            AktualizujGridViewIndeksy(New List(Of ZamowieniaInformacje))
            conn.Close()
        End Using
    End Sub

    Protected Sub GridViewMagazyn_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewMagazyn.RowDataBound
        ''AktualizujGridViewMagazynStatusy()
        If e.Row.RowType = DataControlRowType.DataRow Then
        
            Dim rowIndex = e.Row.RowIndex
            e.Row.Attributes("onclick") = Page.ClientScript.GetPostBackClientHyperlink(GridViewMagazyn, "Select$"&rowIndex)
            e.Row.Style("cursor") = "pointer"
        
        
            Dim cb As CheckBox = e.Row.FindControl("CBKodSelect")
            If cb IsNot Nothing And cb.Checked Then
                e.Row.BackColor = GridViewMagazyn.SelectedRowStyle.BackColor
            Else
                Dim status As String = e.Row.Cells(3).Text.ToString
                If status = "DAJAR" Then : e.Row.BackColor = Ldajar.BackColor
                ElseIf status = "DOMINUS" Then : e.Row.BackColor = Ldominus.BackColor
                End If
            End If
        End If
    End Sub

    Protected Sub GridViewMagazyn_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewMagazyn.SelectedIndexChanged
        Dim cb As CheckBox = GridViewMagazyn.SelectedRow.FindControl("CBKodSelect")
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
        Else
            cb.Checked = True
        End If

        WyswietlSzczegolyZamowienia()
    End Sub

    Protected Sub BRefreshPage_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BRefreshPage.Click
        Response.Redirect(Request.RawUrl)
    End Sub

    Public Function Aktualizacja_Session_Czytnik(ByVal ob_czytnik As ZamowieniaInformacje) As String
        Dim list_czytnik As New List(Of ZamowieniaInformacje)
        If Session("mm_sel_index") IsNot Nothing Then
            Dim odznaczanieWydrukuj As Boolean = False
            list_czytnik = Session("mm_sel_index")

            For Each l In list_czytnik
                If l.nr_zamow = ob_czytnik.nr_zamow Then
                    list_czytnik.RemoveAll(Function(czytnik) czytnik.nr_zamow = ob_czytnik.nr_zamow)
                    odznaczanieWydrukuj = True
                    Exit For
                End If
            Next

            If odznaczanieWydrukuj = False Then
                list_czytnik.Add(ob_czytnik)
            End If
        Else
            list_czytnik.Add(ob_czytnik)
        End If

        Session("mm_sel_index") = list_czytnik
        Return "1"
    End Function


    Protected Sub TBEtykieta_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBEtykieta.TextChanged
        If GridViewMagazyn.Rows.Count > 0 And TBEtykieta.Text.ToString <> "" Then
            For Each row As GridViewRow In GridViewMagazyn.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                Dim schemat As String = row.Cells(3).Text.ToString
                Dim nr_zamow As String = row.Cells(2).Text.ToString

                If TBEtykieta.Text <> "" And TBEtykieta.Text.Length = 12 Then
                    ''Dim ls_schemat As String = TBEtykieta.Text.Substring(0, 2).ToString
                    ''Dim ls_nr_zamow As String = TBEtykieta.Text.Substring(2, 12).ToString
                    Dim ls_schemat As String = "DAJAR"
                    Dim ls_nr_zamow As String = TBEtykieta.Text.ToString

                    ''If ls_schemat = "DA" Then : ls_schemat = "DAJAR"
                    ''ElseIf ls_schemat = "DO" Then : ls_schemat = "DOMINUS"
                    ''End If

                    If nr_zamow = ls_nr_zamow Then
                        GridViewMagazyn.SelectedIndex = row.RowIndex
                        Dim ob_czytnik As New ZamowieniaInformacje(ls_nr_zamow, ls_schemat)
                        Aktualizacja_Session_Czytnik(ob_czytnik)
                        Exit For
                    End If
                End If

            Next
        End If
    End Sub

End Class