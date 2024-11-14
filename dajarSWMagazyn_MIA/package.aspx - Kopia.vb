Imports dajarSWMagazyn.HashMd5
Imports System.Data.OracleClient
Imports System.Data

Partial Public Class package
    Inherits System.Web.UI.Page
    Dim result As Boolean = False
    Dim sqlexp As String = ""
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

    Class ObiektZaznaczenie
        Public nr_zamow As String = ""
        Public schemat As String = ""
        Public etykieta As String = ""
        Public ile_poz As String = ""
        Public ile_obcy As String = ""
        Public aktywny As Boolean = True

        Public Sub New(ByVal _nr_zamow As String, ByVal _schemat As String, ByVal _etykieta As String, ByVal _ile_poz As String, ByVal _ile_obcy As String)
            nr_zamow = _nr_zamow
            schemat = _schemat
            etykieta = _etykieta
            ile_poz = _ile_poz
            ile_obcy = _ile_obcy
        End Sub
    End Class

    Protected Sub GridViewPakowanie_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewPakowanie.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewPakowanie.PageIndex = e.NewPageIndex
        GridViewPakowanie.DataBind()
        LadujDaneGridViewPakowanie()
    End Sub

    Protected Sub DDLOperator_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLOperator.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn.MyFunction.connString)
            conn.Open()
            Dim login As String = DDLOperator.SelectedValue.ToString
            sqlexp = "select hash from dp_swm_uzyt where login = '" & login & "'"
            Dim hash As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            Session("login") = login
            Session("hash") = hash
            sqlexp = "SELECT TYP_OPER FROM DP_SWM_UZYT WHERE LOGIN = '" & login & "'"
            Session("typ_oper") = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            Session("contentHash") = "SESJA : " & hash
            Session("contentOperator") = "OPERATOR : " & Session("typ_oper")
            Dim page_ddloperator As String = "logged.aspx"
            Response.Redirect(page_ddloperator)
        End Using
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Dim przerwijLadowanie As Boolean = False

        Dim listaWybranych As List(Of ObiektZaznaczenie) = PobierzGridViewPakowanieZaznaczone()
        If listaWybranych.Count > 1 Then
            BZakonczPakowanie.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zamknąć wybrane dokumenty[kilka zamowien na etykiecie]?\n') == false) return false")
        Else
            BZakonczPakowanie.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zamknąć wybrane dokumenty?\n') == false) return false")
        End If


        If Session("login") = Nothing And Session("hash") = Nothing Then
            Session.Abandon()
            Response.Redirect("index.aspx")
        ElseIf Session("typ_oper") <> "P" Then
            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
            Session(session_id) += "<br />Zaloguj sie na opowiedniego operatora - typ PAKOWANIE<br />"
            Session(session_id) += "</div>"
            przerwijLadowanie = True
            PanelPakowanie.Visible = False
        End If

        Using conn As New OracleConnection(dajarSWMagazyn.MyFunction.connString)
            conn.Open()
            Session("contentMagazyn") = dajarSWMagazyn.MyFunction.GetStringFromSqlExp("SELECT MAG FROM DP_SWM_UZYT WHERE LOGIN = '" & Session("login") & "'", conn)
        End Using
        Session("contentKomunikat") = Session(session_id)

        If Not Page.IsPostBack Then
            dajarSWMagazyn.MyFunction.RefreshDDLOperator(DDLOperator, Session)
            If przerwijLadowanie = False Then
                LadujDaneGridViewPakowanie()
            End If
        End If
    End Sub

    Protected Sub BWyswietlSzczegoly_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWyswietlSzczegoly.Click
        Dim generujGridView As Boolean = False
        Dim lista_zamowien As New List(Of ZamowieniaInformacje)

        If GridViewPakowanie.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewPakowanie.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                If cb IsNot Nothing And cb.Checked Then
                    generujGridView = True
                    Dim schemat As String = row.Cells(3).Text.ToString
                    Dim nr_zamow As String = row.Cells(2).Text.ToString
                    lista_zamowien.Add(New ZamowieniaInformacje(nr_zamow, schemat))
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
            If obj_zam.schemat = "DAJAR" Then
                sqlexp_DAJAR &= " union all" _
                & " select rownum as lp,w.nr_zamow, w.schemat, w.nr_zamow_o, w.skrot, desql_japa_nwa.fsql_japa_rnaz(get_index_tow(w.skrot)) nazwa, " _
                & " desql_japa_nwa.fsql_japa_rkod(get_index_tow(w.skrot)) kod_tow, w.ile_poz, w.jm, w.mag, w.status from (" _
                & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                & " to_char(zg.data_fakt, 'YYYY/MM/DD') ||' '|| zg.godzina data_zam, 'M' typ_oper," _
                & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag," _
                & " (select zd.jm from ht_zod zd where zd.ie$0 like dm.nr_zamow||'%' and zd.is_deleted = 'N' and zd.index_tow = get_index_tow(dm.skrot)) jm" _
                & " from dp_swm_mag dm, ht_zog zg where dm.status in('PA','PE','MG','WN','ZW')" _
                & " and dm.nr_zamow in('" & obj_zam.nr_zamow & "')" _
                & " and zg.ie$14 = desql_graf.df11_2(dm.nr_zamow) order by dm.nr_zamow" _
                & " ) w"
            ElseIf obj_zam.schemat = "DOMINUS" Then
                sqlexp_DAJAR &= " union all" _
                & " select rownum as lp,w.nr_zamow, w.schemat, w.nr_zamow_o, w.skrot, desql_japa_nwa.fsql_japa_rnaz(dominus.get_index_tow(w.skrot)) nazwa, " _
                & " desql_japa_nwa.fsql_japa_rkod(dominus.get_index_tow(w.skrot)) kod_tow, w.ile_poz, w.jm, w.mag, w.status from (" _
                & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                & " to_char(zg.data_fakt, 'YYYY/MM/DD') ||' '|| zg.godzina data_zam, 'M' typ_oper," _
                & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag," _
                & " (select zd.jm from dominus.ht_zod zd where zd.ie$0 like dm.nr_zamow||'%' and zd.is_deleted = 'N' and zd.index_tow = dominus.get_index_tow(dm.skrot)) jm" _
                & " from dp_swm_mag dm, dominus.ht_zog zg where dm.status in('PA','PE','MG','WN','ZW')" _
                & " and dm.nr_zamow in('" & obj_zam.nr_zamow & "')" _
                & " and zg.ie$14 = desql_graf.df11_2(dm.nr_zamow) order by dm.nr_zamow" _
                & " ) w"
            End If
        Next

        If sqlexp_DAJAR <> "" Then
            sqlexp_DAJAR = sqlexp_DAJAR.Substring(10, sqlexp_DAJAR.Length - 10)
            Using conn As New OracleConnection(dajarSWMagazyn.MyFunction.connString)
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
            End Using
        End If

        GridViewIndeksy.DataSource = indeksy_dtable
        GridViewIndeksy.DataBind()

        AktualizujGridViewIndeksyStatusy()

    End Sub

    Public Sub AktualizujGridViewIndeksyStatusy()
        If GridViewIndeksy.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewIndeksy.Rows
                Dim mag As String = row.Cells(9).Text.ToString
                Dim status As String = row.Cells(10).Text.ToString
                If mag <> Session("contentMagazyn") And status = "PA" Then
                    row.BackColor = LPakowaniePartiami.BackColor
                ElseIf mag = 46 And status = "WN" Then
                    row.BackColor = LPakowaniePartiamiZakonczone.BackColor
                End If
            Next
        End If
    End Sub

    Public Sub LadujDaneGridViewPakowanie()
        Using conn As New OracleConnection(dajarSWMagazyn.MyFunction.connString)
            conn.Open()

            GridViewPakowanie.SelectedIndex = -1
            CzyszczenieInformacjiEtykieta()

            If Session("typ_oper") = "P" And Session("contentMagazyn") = "43" Then
                sqlexp = "select w.nr_zamow,w.schemat,w.nr_zamow_o,(w.ile_poz_buf-w.ile_obcy) ile_43,w.ile_obcy ile_46,w.data_zam,w.typ_oper,w.etykieta_id,w.mag from (" _
                & " select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                & " (select count(*) from dp_swm_mag where nr_zamow = dm.nr_zamow and status in ('PA')) ile_poz," _
                & " (select count(*) from dp_swm_buf where nr_zamow = dm.nr_zamow) ile_poz_buf," _
                & " (select count(*) from dp_swm_mag where nr_zamow = dm.nr_zamow and mag <> " & Session("contentMagazyn") & ") ile_obcy," _
                & " to_char(zg.data_fakt, 'YYYY/MM/DD') data_zam, 'P' typ_oper,'' etykieta_id, dm.mag" _
                & " from dp_swm_mag dm, ht_zog zg where dm.status in ('PA') and zg.nr_zamow not in(select distinct nr_zamow from dp_swm_pak where nr_zamow = dm.nr_zamow)" _
                & " and zg.ie$14 = desql_graf.df11_2(dm.nr_zamow) and dm.mag = " & Session("contentMagazyn") _
                & " union all" _
                & " select distinct dp.nr_zamow,dp.schemat,zg.nr_zamow_o," _
                & " (select count(*) from dp_swm_mag where nr_zamow = dp.nr_zamow and status in ('PA')) ile_poz," _
                & " (select count(*) from dp_swm_buf where nr_zamow = dp.nr_zamow) ile_poz_buf," _
                & " (select count(*) from dp_swm_mag where nr_zamow = dp.nr_zamow and mag <> " & Session("contentMagazyn") & ") ile_obcy," _
                & " to_char(zg.data_fakt, 'YYYY/MM/DD') data_zam, 'P' typ_oper,etykieta_id,'" & Session("contentMagazyn") & "'" _
                & " from dp_swm_pak dp, ht_zog zg where dp.login = '" & Session("login") & "' and dp.status in ('PA')" _
                & " and zg.ie$14 = desql_graf.df11_2(dp.nr_zamow)" _
                & " ) w" _
                & " order by w.nr_zamow"
            ElseIf Session("typ_oper") = "P" And Session("contentMagazyn") = "46" Then
                sqlexp = "select w.nr_zamow,w.schemat,w.nr_zamow_o,(w.ile_poz_buf-w.ile_obcy) ile_46,w.ile_obcy ile_43,w.data_zam,w.typ_oper,w.etykieta_id,w.mag from (" _
                & " select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                & " (select count(*) from dp_swm_mag where nr_zamow = dm.nr_zamow and status in ('PA')) ile_poz," _
                & " (select count(*) from dp_swm_buf where nr_zamow = dm.nr_zamow) ile_poz_buf," _
                & " (select count(*) from dp_swm_mag where nr_zamow = dm.nr_zamow and mag <> " & Session("contentMagazyn") & ") ile_obcy," _
                & " to_char(zg.data_fakt, 'YYYY/MM/DD') data_zam, 'P' typ_oper,'' etykieta_id, dm.mag" _
                & " from dp_swm_mag dm, ht_zog zg where dm.status in ('PA') and zg.nr_zamow not in(select distinct nr_zamow from dp_swm_pak where nr_zamow = dm.nr_zamow)" _
                & " and zg.ie$14 = desql_graf.df11_2(dm.nr_zamow) and dm.mag = " & Session("contentMagazyn") _
                & " union all" _
                & " select distinct dp.nr_zamow,dp.schemat,zg.nr_zamow_o," _
                & " (select count(*) from dp_swm_mag where nr_zamow = dp.nr_zamow and status in ('PA')) ile_poz," _
                & " (select count(*) from dp_swm_buf where nr_zamow = dp.nr_zamow) ile_poz_buf," _
                & " (select count(*) from dp_swm_mag where nr_zamow = dp.nr_zamow and mag <> " & Session("contentMagazyn") & ") ile_obcy," _
                & " to_char(zg.data_fakt, 'YYYY/MM/DD') data_zam, 'P' typ_oper,etykieta_id,'" & Session("contentMagazyn") & "'" _
                & " from dp_swm_pak dp, ht_zog zg where dp.login = '" & Session("login") & "' and dp.status in ('PA')" _
                & " and zg.ie$14 = desql_graf.df11_2(dp.nr_zamow)" _
                & " ) w" _
                & " order by w.nr_zamow"
            End If

            GridViewPakowanie.DataSource = Nothing
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridViewPakowanie.DataSource = dsPartie.Tables(0)
            GridViewPakowanie.DataBind()
            cmd.Dispose()

            If dsPartie.Tables(0).Rows.Count = 0 Then
                LIleDokumentow.Text = "brak"
                PanelPakowanieData.Visible = False
            Else
                LIleDokumentow.Text = dsPartie.Tables(0).Rows.Count.ToString
                PanelPakowanieData.Visible = True
            End If
        End Using
    End Sub

    Public Function SprawdzenieIndeksowOczekujacych_ZakonczPakowanie(ByVal lista As List(Of ObiektZaznaczenie)) As Boolean
        Dim zwracanyStatus As Boolean = False
        Using conn As New OracleConnection(dajarSWMagazyn.MyFunction.connString)
            conn.Open()
            For Each elem In lista
                Dim nr_zamow As String = elem.nr_zamow
                Dim schemat As String = elem.schemat
                If Session("contentMagazyn") = 43 Then
                    sqlexp = "select count(*) from dp_swm_mag where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and mag <> " & Session("contentMagazyn")
                    Dim ileNaMagazynieObcym As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If ileNaMagazynieObcym <> "0" Then
                        sqlexp = "select count(*) from dp_swm_pak where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and mag <> " & Session("contentMagazyn") & " and status in('PE')"
                        Dim ileZakonczonychNaMagazynieObcym As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        If ileZakonczonychNaMagazynieObcym = "0" Then
                            zwracanyStatus = False
                        ElseIf ileNaMagazynieObcym <> ileZakonczonychNaMagazynieObcym Then
                            zwracanyStatus = False
                        End If
                    End If
                ElseIf Session("contentMagazyn") = 46 Then
                    sqlexp = "select count(*) from dp_swm_mag where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and mag <> " & Session("contentMagazyn")
                    Dim ileNaMagazynieObcym As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If ileNaMagazynieObcym = "0" Then
                        Return True
                    End If
                End If
            Next
        End Using
        Return zwracanyStatus
    End Function


    Protected Sub BZakonczPakowanie_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BZakonczPakowanie.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn.MyFunction.connString)
            conn.Open()
            'wariant zakonczenia pakowania dla 43
            If Session("contentMagazyn") = 43 Then
                Dim listaWybranych As List(Of ObiektZaznaczenie) = PobierzGridViewPakowanieZaznaczone()
                For Each element In listaWybranych
                    Dim nr_zamow As String = element.nr_zamow
                    Dim schemat As String = element.schemat
                    If element.ile_obcy = 0 Then
                        'sprawdzanie warunkow na poprawnie utworzona etykiete logistyczna
                        If element.etykieta = "" Or element.etykieta = "&nbsp;" Then
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Nie mozna zakonczyc pakowania - co najmniej jedno zamowienie nie posiada przypisanej etykiety!<br />"
                            Session(session_id) += "</div>"
                        Else
                            Dim timestamp As String = dajarSWMagazyn.MyFunction.DataEvalLogTime()
                            sqlexp = "update dp_swm_pak set status = 'PE', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and login = '" & Session("login") & "' and schemat='" & schemat & "'"
                            result = dajarSWMagazyn.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            result = dajarSWMagazyn.MyFunction.SetOperacjeStatus(Session("login"), Session("hash"), "PE", nr_zamow)
                            element.aktywny = False
                        End If
                    ElseIf element.ile_obcy > 0 Then
                        'sprawdzanie statusow wszystkich indeksow dla zamowienia na magazynie obcym (46)
                        sqlexp = "select count(*) from dp_swm_mag where nr_zamow='" & nr_zamow & "' and mag=46 and status <> 'WN'"
                        Dim ileIndeksowOczekuje As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        If ileIndeksowOczekuje <> "0" Then
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Oczekiwanie na skierowanie indeksow z magazynu 46!<br />"
                            Session(session_id) += "</div>"
                        Else
                            'sprawdzanie warunkow na poprawnie utworzona etykiete logistyczna
                            If element.etykieta = "" Or element.etykieta = "&nbsp;" Then
                                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                                Session(session_id) += "<br />Nie mozna zakonczyc pakowania - co najmniej jedno zamowienie nie posiada przypisanej etykiety!<br />"
                                Session(session_id) += "</div>"
                            Else
                                Dim timestamp As String = dajarSWMagazyn.MyFunction.DataEvalLogTime()
                                sqlexp = "update dp_swm_pak set status = 'PE', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and login = '" & Session("login") & "' and schemat='" & schemat & "'"
                                result = dajarSWMagazyn.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                result = dajarSWMagazyn.MyFunction.SetOperacjeStatus(Session("login"), Session("hash"), "PE", nr_zamow)
                                element.aktywny = False
                            End If
                        End If
                    End If
                Next
                'wariant zakonczenia pakowania dla 46
            ElseIf Session("contentMagazyn") = 46 Then
                Dim listaWybranych As List(Of ObiektZaznaczenie) = PobierzGridViewPakowanieZaznaczone()
                Dim przerwijPakowanie As Boolean = False
                Dim etykieta_id_bierzaca As String = ""
                If listaWybranych.Count > 0 Then
                    For Each element In listaWybranych
                        If element.ile_obcy > 0 Then
                            Dim nr_zamow As String = element.nr_zamow
                            Dim schemat As String = element.schemat
                            sqlexp = "update dp_swm_mag set status='WN' where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and mag = " & Session("contentMagazyn")
                            result = dajarSWMagazyn.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            result = dajarSWMagazyn.MyFunction.SetOperacjeStatus(Session("login"), Session("hash"), "WN", nr_zamow)
                            element.aktywny = False
                        End If
                    Next

                    'kontynuacja sprawdzanie pozostalych warunkow etykiety
                    For Each element In listaWybranych
                        If element.aktywny And (element.etykieta = "" Or element.etykieta = "&nbsp;") Then
                            przerwijPakowanie = True
                            Exit For
                        End If
                    Next

                    If przerwijPakowanie = True Then
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Nie mozna zakonczyc pakowania - co najmniej jedno zamowienie nie posiada przypisanej etykiety!<br />"
                        Session(session_id) += "</div>"
                    Else
                        For Each element In listaWybranych
                            If element.aktywny Then
                                Dim timestamp As String = dajarSWMagazyn.MyFunction.DataEvalLogTime()
                                Dim nr_zamow As String = element.nr_zamow
                                Dim schemat As String = element.schemat
                                sqlexp = "update dp_swm_pak set status = 'PE', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and login = '" & Session("login") & "' and schemat='" & schemat & "'"
                                result = dajarSWMagazyn.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                result = dajarSWMagazyn.MyFunction.SetOperacjeStatus(Session("login"), Session("hash"), "PE", nr_zamow)

                                '#####sprawdzenie warunku czy generowac email do biura burek / tylko magazyn 46
                                If Session("contentMagazyn") = 46 Then
                                    sqlexp = "select count(*) from dp_swm_mag where nr_zamow = '" & nr_zamow & "' and mag <> 46"
                                    Dim ileNaMagazynieObcym As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                                    If ileNaMagazynieObcym <> "0" Then
                                        '##########################################################TWORZENIE WIADOMOSCI EMAIL##################################################
                                        ''Dim email_adres As New List(Of String)
                                        ''email_adres = New List(Of String)(New String() {"michal.michalak@dajar.pl"})
                                        ''Dim email_titla As String = "[dajarSWMagazyn] zakonczenie pakowania dla zamowienia " & nr_zamow & " na magazynie 46"
                                        ''sqlexp = "select w.nr_zamow, w.nr_zamow_o, w.schemat, w.data_zam, w.mag, TO_CHAR(sysdate,'YYYY/MM/DD HH24:MM:SS') from (" _
                                        ''& " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                                        ''& " to_char(zg.data_fakt, 'YYYY/MM/DD') data_zam, 'M' typ_oper," _
                                        ''& " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag" _
                                        ''& " from dp_swm_mag dm, ht_zog zg where dm.login = '" & Session("login") & "' and dm.status in('PA')" _
                                        ''& " and dm.nr_zamow = '" & nr_zamow & "'" _
                                        ''& " and zg.ie$14 = desql_graf.df11_2(dm.nr_zamow) order by dm.nr_zamow" _
                                        ''& " ) w group by w.nr_zamow, w.schemat, w.nr_zamow_o, w.status, w.mag, w.data_zam"

                                        ''Dim email_template As String = "zakonczenie_pakowania_46.htm"
                                        ''Dim form_field As New List(Of String)(New String() {"(NR_ZAMOW)", "(NR_ZAMOW_O)", "(SCHEMAT)", "(DATA_ZAM)", "(MAG_ID)", "(DATA_SYSTEM)"})
                                        ''dajarSWMagazyn.MyFunction.PrepareToSendEmailMessage(email_adres, email_titla, email_template, form_field, sqlexp, nr_zamow, schemat, Session("login"), conn, Session)
                                        '##########################################################TWORZENIE WIADOMOSCI EMAIL##################################################

                                    End If
                                End If
                            End If
                        Next
                        LadujDaneGridViewPakowanie()
                    End If
                End If
            End If

            Response.Redirect(Request.RawUrl)

        End Using
    End Sub

    Public Sub GenerujInformacjeEtykieta(ByVal obiekt As ObiektZaznaczenie)
        Using conn As New OracleConnection(dajarSWMagazyn.MyFunction.connString)
            conn.Open()
            Dim etykieta_id As String = ""
            If obiekt.etykieta = "" Then
                sqlexp = "select etykieta_id from DP_SWM_pak where LOGIN = '" & Session("login") & "' and nr_zamow = '" & obiekt.nr_zamow & "'"
                etykieta_id = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            Else
                etykieta_id = obiekt.etykieta
            End If

            If etykieta_id <> "" Then
                sqlexp = "SELECT dlu,wys,szer,waga from dp_swm_etykieta where etykieta_id = '" & etykieta_id & "'"

                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()

                Try
                    While dr.Read()
                        LEtykietaID.Text = etykieta_id
                        TBDlugosc.Text = dr.Item(0).ToString
                        TBWysokosc.Text = dr.Item(1).ToString
                        TBSzerokosc.Text = dr.Item(2).ToString
                        TBWaga.Text = dr.Item(3).ToString
                    End While
                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try

                dr.Close()
                cmd.Dispose()
            Else
                Dim etykietaInput As String = LNrZamowienia.Text & Date.Now.ToString
                CzyszczenieInformacjiEtykieta()
                LEtykietaID.Text = getMd5Hash(etykietaInput)
            End If
        End Using
    End Sub

    Protected Sub GridViewIndeksy_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewPakowanie.SelectedIndexChanged
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Dim cb As CheckBox = GridViewPakowanie.SelectedRow.FindControl("CBKodSelect")
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
            CzyszczenieInformacjiEtykieta()
        Else
            cb.Checked = True
        End If

        AktualizujGridViewPakowanieStatusy()

        Using conn As New OracleConnection(dajarSWMagazyn.MyFunction.connString)
            conn.Open()
            If GridViewPakowanie.Rows.Count > 0 Then
                LNrZamowienia.Text = ""
                LSchemat.Text = ""
                Dim elementEtykieta As New ObiektZaznaczenie("", "", "", "", "")
                Dim listaWybranych As List(Of ObiektZaznaczenie) = PobierzGridViewPakowanieZaznaczone()
                For Each elem In listaWybranych
                    LNrZamowienia.Text &= elem.nr_zamow.ToString & " "
                    LSchemat.Text &= elem.schemat.ToString & " "
                    If elem.etykieta.Contains("ET") Then elementEtykieta = elem
                Next

                GenerujInformacjeEtykieta(elementEtykieta)
                Panel3.Visible = True
            End If
        End Using
    End Sub

    Public Function PobierzGridViewPakowanieZaznaczone() As List(Of ObiektZaznaczenie)
        Dim listaWybranych As New List(Of ObiektZaznaczenie)
        If GridViewPakowanie.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewPakowanie.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                If cb IsNot Nothing And cb.Checked Then
                    Dim obiektZaznaczenie As New ObiektZaznaczenie(row.Cells(2).Text.ToString, row.Cells(3).Text.ToString, row.Cells(9).Text.ToString, row.Cells(5).Text.ToString, row.Cells(6).Text.ToString)
                    listaWybranych.Add(obiektZaznaczenie)
                End If
            Next
        End If
        Return listaWybranych
    End Function

    Protected Sub GridViewPakowanie_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewPakowanie.RowDataBound
        AktualizujGridViewPakowanieStatusy()
    End Sub

    Public Sub AktualizujGridViewPakowanieStatusy()
        If GridViewPakowanie.Rows.Count > 0 Then
            Using conn As New OracleConnection(dajarSWMagazyn.MyFunction.connString)
                conn.Open()
                For Each row As GridViewRow In GridViewPakowanie.Rows
                    Dim nr_zamow As String = row.Cells(2).Text.ToString
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        row.BackColor = GridViewPakowanie.SelectedRowStyle.BackColor
                    Else
                        Dim schemat As String = row.Cells(3).Text.ToString
                        If schemat = "DAJAR" Then : row.BackColor = Ldajar.BackColor
                        ElseIf schemat = "DOMINUS" Then : row.BackColor = Ldominus.BackColor
                        End If

                        Dim etykieta_id As String = row.Cells(9).Text.ToString
                        If etykieta_id <> "" And etykieta_id <> "&nbsp;" Then
                            row.BackColor = LPodjete.BackColor
                        End If

                        '' ''Dim rekordyNaMagazynieInnym As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        '' ''If rekordyNaMagazynieInnym <> "0" Then
                        '' ''    row.BackColor = LPakowaniePartiami.BackColor
                        '' ''End If
                    End If
                Next
            End Using
        End If
    End Sub

    Public Function SprawdzenieIndeksowOczekujacych_ZapiszEtykiete(ByVal lista As List(Of ObiektZaznaczenie)) As Boolean
        Dim zwracanyStatus As Boolean = True
        Using conn As New OracleConnection(dajarSWMagazyn.MyFunction.connString)
            conn.Open()
            For Each elem In lista
                Dim nr_zamow As String = elem.nr_zamow
                Dim schemat As String = elem.schemat
                If Session("contentMagazyn") = 43 Then
                    sqlexp = "select count(*) from dp_swm_mag where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and mag <> " & Session("contentMagazyn")
                    Dim ileNaMagazynieObcym As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If ileNaMagazynieObcym <> "0" Then
                        sqlexp = "select count(*) from dp_swm_pak where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and mag <> " & Session("contentMagazyn") & " and status in('PE')"
                        Dim ileZakonczonychNaMagazynieObcym As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        If ileZakonczonychNaMagazynieObcym = "0" Then
                            zwracanyStatus = False
                        ElseIf ileNaMagazynieObcym <> ileZakonczonychNaMagazynieObcym Then
                            zwracanyStatus = False
                        End If
                    End If
                ElseIf Session("contentMagazyn") = 46 Then
                    sqlexp = "select count(*) from dp_swm_mag where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and mag <> " & Session("contentMagazyn")
                    Dim ileNaMagazynieObcym As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If ileNaMagazynieObcym <> "0" Then
                        zwracanyStatus = False
                    End If
                End If
            Next
        End Using
        Return zwracanyStatus
    End Function

    Protected Sub BZapiszEtykiete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BZapiszEtykiete.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn.MyFunction.connString)
            conn.Open()
            Dim listaWybranych As List(Of ObiektZaznaczenie) = PobierzGridViewPakowanieZaznaczone()
            Dim etykieta_id_bierzaca As String = ""
            If listaWybranych.Count > 0 Then
                '####sprawdzenie warunku czy zamowienie czesciowe(43 i 46) - wszystkie indeksy na 46 musza miec status PE
                Dim czyKontynuowac As Boolean = SprawdzenieIndeksowOczekujacych_ZapiszEtykiete(listaWybranych)
                If czyKontynuowac = False Then
                    If Session("contentMagazyn") = 43 Then
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Dla wybranych zamowien niektore indeksy sa obecnie w trakcie kompletowania na magazynie 46. Poczekaj az zmienia status na spakowane PE [WYSWIETL SZCZEGOLY]!<br />"
                        Session(session_id) += "</div>"
                    ElseIf Session("contentMagazyn") = 46 Then
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Dla wybranych zamowien nie mozna utworzyc etykiety logistycznej! Zakoncz zamowienie / skieruj je na magazyn 43[ZAKONCZ PAKOWANIE]!<br />"
                        Session(session_id) += "</div>"
                    End If
                Else
                    'wybrane chociaz jedno zamowienie ktore zawiera juz przypisana etykiete
                    If LEtykietaID.Text.Contains("ET") Then
                        etykieta_id_bierzaca = LEtykietaID.Text.ToString
                        WalidacjaInformacjiEtykieta()
                        If IsNumeric(TBDlugosc.Text) And IsNumeric(TBSzerokosc.Text) And IsNumeric(TBWysokosc.Text) And IsNumeric(TBWaga.Text) Then
                            Dim dlu As String = TBDlugosc.Text
                            Dim wys As String = TBWysokosc.Text
                            Dim szer As String = TBSzerokosc.Text
                            Dim waga As String = TBWaga.Text

                            'aktualizacja danych do wprowadzonej etykiety
                            sqlexp = "update dp_swm_etykieta set dlu='" & dlu & "', wys='" & wys & "', szer='" & szer & "', waga='" & waga & "' where etykieta_id = '" & etykieta_id_bierzaca & "'"
                            result = dajarSWMagazyn.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            For Each element In listaWybranych
                                Dim nr_zamow As String = element.nr_zamow
                                Dim schemat As String = element.schemat
                                Dim ile_poz As String = element.ile_poz
                                sqlexp = "select nr_zamow from dp_swm_pak where nr_zamow='" & nr_zamow & "' and schemat='" & schemat & "' and etykieta_id='" & etykieta_id_bierzaca & "'"
                                Dim czyRekordPakowanie As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                                If czyRekordPakowanie = "" Then
                                    Dim timestamp As String = dajarSWMagazyn.MyFunction.DataEvalLogTime()
                                    sqlexp = "insert into dp_swm_pak (login,hash,nr_zamow,autodata,status,schemat,mag,etykieta_id,ile_poz) values('" & Session("login") & "','" & Session("hash") & "','" & nr_zamow & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'),'PA','" & schemat & "','" & Session("contentMagazyn") & "','" & etykieta_id_bierzaca & "','" & ile_poz & "')"
                                    result = dajarSWMagazyn.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    result = dajarSWMagazyn.MyFunction.SetOperacjeStatus(Session("login"), Session("hash"), "PA", nr_zamow)
                                Else
                                    sqlexp = "update dp_swm_pak set etykieta_id='" & etykieta_id_bierzaca & "' where nr_zamow='" & nr_zamow & "' and schemat='" & schemat & "' and login='" & Session("login") & "'"
                                    result = dajarSWMagazyn.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                End If
                            Next

                            LadujDaneGridViewPakowanie()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zakonczeniem pakowania!<br />"
                            Session(session_id) += "</div>"
                        End If
                    Else
                        etykieta_id_bierzaca = dajarSWMagazyn.MyFunction.GenerateEtykietaId()
                        'sprawdzenie poprawnego formatu pol na etykiecie
                        WalidacjaInformacjiEtykieta()
                        If IsNumeric(TBDlugosc.Text) And IsNumeric(TBSzerokosc.Text) And IsNumeric(TBWysokosc.Text) And IsNumeric(TBWaga.Text) Then
                            Dim dlu As String = TBDlugosc.Text
                            Dim wys As String = TBWysokosc.Text
                            Dim szer As String = TBSzerokosc.Text
                            Dim waga As String = TBWaga.Text

                            'utworzenie nowej etykiety logistycznej
                            Dim timestamp As String = dajarSWMagazyn.MyFunction.DataEvalLogTime()
                            sqlexp = "insert into dp_swm_etykieta values('" & etykieta_id_bierzaca & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'), '" & dlu & "', '" & wys & "','" & szer & "', '" & waga & "')"
                            result = dajarSWMagazyn.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            For Each element In listaWybranych
                                Dim nr_zamow As String = element.nr_zamow
                                Dim schemat As String = element.schemat
                                Dim ile_poz As String = element.ile_poz
                                sqlexp = "select nr_zamow from dp_swm_pak where nr_zamow='" & nr_zamow & "' and schemat='" & schemat & "' and etykieta_id='" & etykieta_id_bierzaca & "'"
                                Dim czyRekordPakowanie As String = dajarSWMagazyn.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                                If czyRekordPakowanie = "" Then
                                    sqlexp = "insert into dp_swm_pak (login,hash,nr_zamow,autodata,status,schemat,mag,etykieta_id,ile_poz) values('" & Session("login") & "','" & Session("hash") & "','" & nr_zamow & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'),'PA','" & schemat & "','" & Session("contentMagazyn") & "','" & etykieta_id_bierzaca & "','" & ile_poz & "')"
                                    result = dajarSWMagazyn.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    result = dajarSWMagazyn.MyFunction.SetOperacjeStatus(Session("login"), Session("hash"), "PA", nr_zamow)
                                Else
                                    sqlexp = "update dp_swm_pak set etykieta_id='" & etykieta_id_bierzaca & "' nr_zamow='" & nr_zamow & "' and schemat='" & schemat & "' and login='" & Session("login") & "'"
                                    result = dajarSWMagazyn.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                End If
                            Next

                            GridViewIndeksy.DataSource = Nothing
                            GridViewIndeksy.DataBind()
                            Panel3.Visible = False
                            LadujDaneGridViewPakowanie()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zakonczeniem pakowania!<br />"
                            Session(session_id) += "</div>"
                        End If
                    End If
                End If

                Response.Redirect(Request.RawUrl)
            End If
        End Using
    End Sub

    Protected Sub BOdznaczWszystkie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BOdznaczWszystkie.Click
        If GridViewPakowanie.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewPakowanie.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                cb.Checked = False
            Next
        End If

        LNrZamowienia.Text = ""
        LSchemat.Text = ""
        Dim listaWybranych As List(Of ObiektZaznaczenie) = PobierzGridViewPakowanieZaznaczone()
        For Each elem In listaWybranych
            LNrZamowienia.Text &= elem.nr_zamow.ToString & " "
            LSchemat.Text &= elem.schemat.ToString & " "
        Next
        AktualizujGridViewPakowanieStatusy()
    End Sub

    Protected Sub BZaznaczWszystkie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BZaznaczWszystkie.Click
        If GridViewPakowanie.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewPakowanie.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                cb.Checked = True
            Next
        End If

        LNrZamowienia.Text = ""
        LSchemat.Text = ""
        Dim listaWybranych As List(Of ObiektZaznaczenie) = PobierzGridViewPakowanieZaznaczone()
        For Each elem In listaWybranych
            LNrZamowienia.Text &= elem.nr_zamow.ToString & " "
            LSchemat.Text &= elem.schemat.ToString & " "
        Next
        AktualizujGridViewPakowanieStatusy()
    End Sub

    Public Sub CzyszczenieInformacjiEtykieta()
        LNrZamowienia.Text = ""
        LSchemat.Text = ""
        LEtykietaID.Text = ""
        TBDlugosc.Text = ""
        TBWysokosc.Text = ""
        TBSzerokosc.Text = ""
        TBWaga.Text = ""
    End Sub

    Public Sub WalidacjaInformacjiEtykieta()
        TBDlugosc.Text = TBDlugosc.Text.Replace(".", ",")
        TBWysokosc.Text = TBWysokosc.Text.Replace(".", ",")
        TBSzerokosc.Text = TBSzerokosc.Text.Replace(".", ",")
        TBWaga.Text = TBWaga.Text.Replace(".", ",")
    End Sub

End Class