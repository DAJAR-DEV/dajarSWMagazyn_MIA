Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class a_manageEdit
    Inherits System.Web.UI.Page

    Dim sqlexp As String = ""
    Dim result As Boolean
    Dim daRejestrSklepow As New OracleDataAdapter
    Dim dsRejestrSklepow As New DataSet
    Dim rejestrSklepow As New List(Of String)
    Dim cb As OracleCommandBuilder

    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet

    Protected Sub GridViewDyspozycje_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewDyspozycje.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewDyspozycje.PageIndex = e.NewPageIndex
        GridViewDyspozycje.DataBind()
        LadujDaneGridViewDyspozycjeInfo()
    End Sub

    Protected Sub GridViewDyspozycje_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewDyspozycje.RowDataBound
        ''AktualizujGridViewDyspozycjeStatusy()
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim status As String = e.Row.Cells(10).Text.ToString
            If status = "MG" Then : e.Row.BackColor = LMagazyn.BackColor
            ElseIf status = "WN" Then : e.Row.BackColor = LWznowione.BackColor
            ElseIf status = "ZW" Then : e.Row.BackColor = LWstrzymane.BackColor
            ElseIf status = "PA" Then : e.Row.BackColor = LPakowanie.BackColor
            ElseIf status = "PE" Then : e.Row.BackColor = LSpakowane.BackColor
            ElseIf status = "RP" Then : e.Row.BackColor = LPilne.BackColor
            ElseIf status = "HB" Then : e.Row.BackColor = LBlokadaNaHippo.BackColor
            ElseIf status = "BB" Then : e.Row.BackColor = LBlokadaNaBurek.BackColor
            ElseIf status = "PP" Then : e.Row.BackColor = LPrzeslanoNaDocelowy.BackColor
            ElseIf status = "RW" Then : e.Row.BackColor = LRezygnacjaWlasna.BackColor
            End If
        End If
    End Sub

    Public Sub LadujDaneGridViewPaczka(ByVal nr_zamow As String, ByVal schemat As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            GridViewPaczki.SelectedIndex = -1
            sqlexp = "select pi.paczka_id,pi.firma_id,case when pi.pl_type='1' then 'paczka' when pi.pl_type='2' then 'paleta' when pi.pl_type='3' then 'koperta' when pi.pl_type='4' then 'paczkomatA' when pi.pl_type='5' then 'paczkomatB' when pi.pl_type='6' then 'paczkomatC' when pi.pl_type='7' then 'paczka_poczta' when pi.pl_type='8' then 'paczka_inpost' when pi.pl_type='9' then 'paczka_poczta' when pi.pl_type='10' then 'paczka_cieszyn' when pi.pl_type='HP' then 'półpaleta [HP]' when pi.pl_type='EP' then 'bezzwrotna paleta [EP]' when pi.pl_type='CC' then 'colli [CC]' when pi.pl_type='FP' then 'europaleta [FP]' when pi.pl_type='NP' then 'paleta 1.0x2.0 [NP]' when pi.pl_type='VP' then '1/4 paleta [VP]' when pi.pl_type='PC' then 'chep [PC]' when pi.pl_type='DR' then 'przekracza europalete [DR]' end typ," _
            & " case when pi.pl_non_std='A' then 'paczkomat[A]' when pi.pl_non_std='B' then 'paczkomat[B]' when pi.pl_non_std='C' then 'paczkomat[C]' when pi.pl_non_std='D' then 'paczkomat[D]' when pi.pl_non_std='0' then 'standard' else 'niestandard' end rodzaj,pi.pl_weight waga, pi.pl_width szer,pi.pl_height wys,pi.pl_length dlu,pi.pl_quantity ile_paczek from" _
            & " dp_swm_mia_paczka_info pi" _
            & " where pi.shipment_id=(select shipment_id from dp_swm_mia_paczka where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "') order by pi.paczka_id asc"

            GridViewPaczki.DataSource = Nothing
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridViewPaczki.DataSource = dsPartie.Tables(0)
            GridViewPaczki.DataBind()
            cmd.Dispose()
            conn.Close()
        End Using
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

        BWstrzymajDyspo.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz wstrzymac aktywna dyspozycje " & Session("kod_dyspozycji") & "') == false) return false")
        BWznowDyspo.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz automatycznie wznowic aktywna dyspozycje " & Session("kod_dyspozycji") & "') == false) return false")
        BAnulowanieDyspo.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz anulowac dyspozycje [rezygnacja wlasna]" & Session("kod_dyspozycji") & "') == false) return false")
        BSpakowaneDyspo.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz automatycznie spakowac dyspozycje [status PE]" & Session("kod_dyspozycji") & "') == false) return false")

        If Not Page.IsPostBack Then
            If przerwijLadowanie = False Then

                LadujDaneGridViewDyspozycjeInfo()
                LadujDaneGridViewLosowanieInfo()

                Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                    conn.Open()

                    Session("mag_dyspo") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_zog WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                    LMagDyspo.Text = Session("mag_dyspo")
                    LNrZamow.Text = Session("kod_dyspo")

                    Session("status_dyspo") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT STAT_U1 FROM dp_swm_mia_zog WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                    LStatus.Text = Session("status_dyspo")

                    Session("typ_dyspo") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT TYP_OPER FROM dp_swm_mia_BUF WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                    LStatusDigit.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT STATUS FROM dp_swm_mia_buf WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                    TBInformacjeDodatkowe.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT INFO FROM dp_swm_mia_mag WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                    LWydrukID.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT WYDRUK_ID from dp_swm_mia_wydruk WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                    If Session("status_dyspo") <> "ZW" Then : BWstrzymajDyspo.Enabled = True
                    Else : BWstrzymajDyspo.Enabled = False
                    End If

                    If Session("status_dyspo") = "ZW" Then : BWznowDyspo.Enabled = True
                    Else : BWznowDyspo.Enabled = False
                    End If

                    ''If Session("status_dyspo") <> "PE" Then : BSpakowaneDyspo.Enabled = True
                    ''Else : BSpakowaneDyspo.Enabled = False
                    ''End If


                    If Session("status_dyspo") = "MG" Or Session("status_dyspo") = "HB" Or Session("status_dyspo") = "BB" Or Session("status_dyspo") = "ZW" Or Session("status_dyspo") = "WN" Then
                        DDLOperatorDyspo.Enabled = True
                    Else
                        DDLOperatorDyspo.Enabled = False
                    End If

                    If Session("status_dyspo") = "DR" Or Session("status_dyspo") = "RP" Then
                        DDLOperatorLosowanie.Enabled = True
                    Else
                        DDLOperatorLosowanie.Enabled = False
                    End If

                    If Session("typ_dyspo") = "M" Then
                        sqlexp = "select to_char(dm.autodata,'RRRR/MM/DD HH24:MI:SS') autodata, " _
                    & " to_char(dm.autodata_zak,'RRRR/MM/DD HH24:MI:SS') autodata_zak,'' etykieta_id from dp_swm_mia_mag dm where dm.nr_zamow = '" & Session("kod_dyspo") & "' AND dm.SCHEMAT='" & Session("schemat_dyspo") & "'"
                    ElseIf Session("typ_dyspo") = "P" Then
                        sqlexp = "select to_char(dp.autodata,'RRRR/MM/DD HH24:MI:SS') autodata, " _
                    & " to_char(dp.autodata_zak, 'YYYY/MM/DD HH24:MI:SS') autodata_zak, dp.etykieta_id from dp_swm_mia_pak dp where dp.nr_zamow = '" & Session("kod_dyspo") & "' AND dp.SCHEMAT='" & Session("schemat_dyspo") & "'"
                    End If

                    Dim cmd As New OracleCommand(sqlexp, conn)
                    cmd.CommandType = CommandType.Text
                    Dim dr As OracleDataReader = cmd.ExecuteReader()
                    dr = cmd.ExecuteReader()
                    Try
                        While dr.Read()
                            LDataWprowadzenia.Text = dr.Item(0).ToString
                            LDataZakonczenia.Text = dr.Item(1).ToString
                            LEtykieta.Text = dr.Item(2).ToString
                        End While

                    Catch ex As System.ArgumentOutOfRangeException
                        Console.WriteLine(ex)
                    End Try
                    dr.Close()
                    cmd.Dispose()

                    LEtykieta.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT ETYKIETA_ID FROM dp_swm_mia_pak WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                    If LEtykieta.Text <> "" Then
                        PanelEtykieta.Visible = True
                        LadujDaneGridViewPaczka(Session("kod_dyspo"), Session("schemat_dyspo"))
                    End If

                    LOperatorBiezacy.Text = Session("login_dyspo")
                    LOperatorTypOper.Text = Session("typ_dyspo")
                    LStatusUzytkownik.Text = Session("typ_dyspo")
                    LSchemat.Text = Session("schemat_dyspo")

                    RefreshDDLOperatorDyspoBiezacy(DDLOperatorDyspo, Session)
                    RefreshDDLOperatorLosowanieBiezacy(DDLOperatorLosowanie, Session)
                    conn.Close()
                End Using

                Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                    conn.Open()
                    sqlexp = "select zg.nr_zamow_o, zg.data_zam " _
                & " from dp_swm_mia_zog zg where zg.nr_zamow = '" & Session("kod_dyspo") & "' and zg.schemat = '" & Session("schemat_dyspo") & "'"

                    Dim cmd As New OracleCommand(sqlexp, conn)
                    cmd.CommandType = CommandType.Text
                    Dim dr As OracleDataReader = cmd.ExecuteReader()
                    dr = cmd.ExecuteReader()
                    Try
                        While dr.Read()
                            LNr_zamow_o.Text = dr.Item(0).ToString
                            LDataZam.Text = dr.Item(1).ToString
                        End While

                    Catch ex As System.ArgumentOutOfRangeException
                        Console.WriteLine(ex)
                    End Try
                    dr.Close()
                    cmd.Dispose()
                    conn.Close()
                End Using

                If Session("schemat_dyspo") = "DOMINUS" And LNr_zamow_o.Text <> "" Then : panelDigitland.Visible = True
                Else : panelDigitland.Visible = False
                End If
            End If
        End If
        Session("contentKomunikat") = Session(session_id)

    End Sub

    Protected Sub RefreshDDLOperatorDyspoBiezacy(ByRef ddlObiekt As DropDownList, ByRef sesja_bierzaca As System.Web.SessionState.HttpSessionState)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim typ_oper As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct typ_oper from dp_swm_mia_buf where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "'", conn)
            Dim sqlExp As String = "select login from dp_swm_mia_uzyt where status = 'X' and typ_oper in('" & typ_oper & "') and blokada_konta is null and mag='" & Session("mag_dyspo").ToString & "' order by login"
            ddlObiekt.Items.Clear()
            Dim cmd As New OracleCommand(sqlExp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            If dr.HasRows Then
                ddlObiekt.DataSource = dr
                ddlObiekt.DataTextField = "LOGIN"
                ddlObiekt.DataValueField = "LOGIN"
                ddlObiekt.DataBind()
            End If
            dr.Close()
            cmd.Dispose()

            If sesja_bierzaca("mylogin") IsNot Nothing Then
                ddlObiekt.SelectedValue = sesja_bierzaca("mylogin")
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLOperatorLosowanieBiezacy(ByRef ddlObiekt As DropDownList, ByRef sesja_bierzaca As System.Web.SessionState.HttpSessionState)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim typ_oper As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct typ_oper from dp_swm_mia_buf where dyspozycja='N' and nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "'", conn)
            Dim sqlExp As String = "select login from dp_swm_mia_uzyt where status = 'X' and typ_oper in('" & typ_oper & "') and blokada_konta is null and mag='" & Session("mag_dyspo").ToString & "' order by login"
            ddlObiekt.Items.Clear()
            Dim cmd As New OracleCommand(sqlExp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            If dr.HasRows Then
                ddlObiekt.DataSource = dr
                ddlObiekt.DataTextField = "LOGIN"
                ddlObiekt.DataValueField = "LOGIN"
                ddlObiekt.DataBind()
            End If
            dr.Close()
            cmd.Dispose()

            ''If sesja_bierzaca("mylogin") IsNot Nothing Then
            ''    ddlObiekt.SelectedValue = sesja_bierzaca("mylogin")
            ''End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BZmianaOper_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BZmianaOper.Click
        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("kod_dyspo") IsNot Nothing And Session("kod_dyspo") <> "" Then

                Dim cbOperChecked As Boolean = CBOperLosowanie.Checked
                If cbOperChecked = True Then
                    Dim oper_login As String = DDLOperatorLosowanie.SelectedValue.ToString
                    Dim oper_hash As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select hash from dp_swm_mia_uzyt where login='" & oper_login & "'", conn)
                    Dim losowaniePoprawne As Boolean = False
                    sqlexp = "select skrot,ilosc from dp_swm_mia_buf where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and dyspozycja='N'"
                    Dim cmd As New OracleCommand(sqlexp, conn)
                    cmd.CommandType = CommandType.Text
                    Dim dr As OracleDataReader = cmd.ExecuteReader()
                    dr = cmd.ExecuteReader()
                    Try
                        While dr.Read()
                            Dim skrot As String = dr.Item(0).ToString
                            Dim ilosc As String = dr.Item(1).ToString
                            dajarSWMagazyn_MIA.MyFunction.SetMagazynDyspozycja(oper_login, oper_hash, Session("kod_dyspo"), "MG", Session("schemat_dyspo"), LMagDyspo.Text.ToString, skrot, ilosc)
                            dajarSWMagazyn_MIA.MyFunction.SetBuforAktualizacja(Session("kod_dyspo"), Session("schemat_dyspo"), skrot)
                            losowaniePoprawne = True
                        End While

                    Catch ex As System.ArgumentOutOfRangeException
                        Console.WriteLine(ex)
                    End Try
                    dr.Close()
                    cmd.Dispose()

                    If losowaniePoprawne Then
                        dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(oper_login, oper_hash, "MG" & "[" & Session("schemat_dyspo") & "][" & LMagDyspo.Text.ToString & "]", Session("kod_dyspo"))
                        dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(Session("kod_dyspo"), Session("schemat_dyspo"), "MG")
                        Response.Redirect("a_manageEdit.aspx")
                    End If
                End If

                If Session("status_dyspo") = "MG" Or Session("status_dyspo") = "HB" Or Session("status_dyspo") = "ZW" Or Session("status_dyspo") = "WN" Then

                    Dim loginNew As String = DDLOperatorDyspo.SelectedValue.ToString
                    Dim hashNew As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT HASH FROM dp_swm_mia_uzyt where login = '" & loginNew & "'", conn)
                    Dim info_dod As String = ""
                    If TBInformacjeDodatkowe.Text.Length > 4000 Then
                        info_dod = TBInformacjeDodatkowe.Text.ToString.Substring(0, 4000)
                    Else
                        info_dod = TBInformacjeDodatkowe.Text.ToString
                    End If
                    If Session("typ_dyspo") = "M" Or Session("typ_dyspo") = "O" Or Session("typ_dyspo") = "MO" Or Session("typ_dyspo") = "MP" Or Session("typ_oper") = "ME" Then
                        sqlexp = "UPDATE dp_swm_mia_mag SET LOGIN='" & loginNew & "',HASH='" & hashNew & "',INFO='" & info_dod & "' WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND LOGIN='" & Session("login_dyspo") & "'"
                    End If

                    LOperatorBiezacy.Text = loginNew
                    LOperatorTypOper.Text = LStatusUzytkownik.Text
                    Session("typ_dyspo") = LStatusUzytkownik.Text

                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    Response.Redirect("a_manageEdit.aspx")
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub DDLOperatorDyspo_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DDLOperatorDyspo.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If DDLOperatorDyspo.SelectedValue.ToString <> "" Then
                sqlexp = "select trim(TYP_OPER) typ_oper from dp_swm_mia_uzyt where login = '" & DDLOperatorDyspo.SelectedValue.ToString & "'"
                Dim typ_operNew As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                LStatusUzytkownik.Text = typ_operNew
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub DDLOperatorLosowanie_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DDLOperatorLosowanie.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If DDLOperatorLosowanie.SelectedValue.ToString <> "" Then
                sqlexp = "select trim(TYP_OPER) typ_oper from dp_swm_mia_uzyt where login = '" & DDLOperatorDyspo.SelectedValue.ToString & "'"
                LStatusUzytkownikLosowanie.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BWznowDyspo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWznowDyspo.Click
        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("kod_dyspo") IsNot Nothing And Session("kod_dyspo") <> "" Then
                If Session("status_dyspo") = "ZW" Then
                    sqlexp = "update dp_swm_mia_mag set status='WN' where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and status in('ZW')"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "WN" & "[" & Session("schemat_dyspo") & "][" & Session("mag_dyspo") & "]", Session("kod_dyspo"))
                    dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(Session("kod_dyspo"), Session("schemat_dyspo"), "WN")
                    Session("status_dyspo") = "WN"

                    sqlexp = "update dp_swm_mia_buf set status='DR' where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and status in('ZW')"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    Dim ile_bb As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select count(*) from dp_swm_mia_mag where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and status in('BB')", conn)
                    Dim ile_hb As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select count(*) from dp_swm_mia_mag where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and status in('HB')", conn)
                    If ile_bb <> "0" Then
                        dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(Session("kod_dyspo"), Session("schemat_dyspo"), "BB")
                        Session("status_dyspo") = "BB"
                    ElseIf ile_hb <> "0" Then
                        dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(Session("kod_dyspo"), Session("schemat_dyspo"), "HB")
                        Session("status_dyspo") = "HB"
                    End If

                    Response.Redirect("a_manageEdit.aspx")
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BWstrzymajDyspo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWstrzymajDyspo.Click
        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("kod_dyspo") IsNot Nothing And Session("kod_dyspo") <> "" Then
                If Session("status_dyspo") <> "ZW" Then
                    sqlexp = "update dp_swm_mia_mag set status='ZW' where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and status in('MG','WN')"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "ZW" & "[" & Session("schemat_dyspo") & "][" & Session("mag_dyspo") & "]", Session("kod_dyspo"))
                    dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(Session("kod_dyspo"), Session("schemat_dyspo"), "ZW")

                    sqlexp = "update dp_swm_mia_buf set status='ZW' where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and status in('DR','RP')"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    Session("status_dyspo") = "ZW"
                    Response.Redirect("a_manageEdit.aspx")
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Public Sub LadujDaneGridViewDyspozycjeInfo()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            sqlexp = "select rownum as lp,w.nr_zamow, w.schemat, w.nr_zamow_o, w.skrot, desql_japa_nwa.fsql_japa_rnaz(get_index_tow(w.skrot)) nazwa, " _
                & " desql_japa_nwa.fsql_japa_rkod(get_index_tow(w.skrot)) kod_tow, w.ile_poz, w.jm, w.mag, w.status, w.login from (" _
                & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                & " zg.data_zam, 'M' typ_oper," _
                & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag, dm.login," _
                & " (select distinct zd.jm from ht_zod zd where zd.ie$0 like dm.nr_zamow||'%' and zd.is_deleted = 'N' and zd.skrot = dm.skrot) jm" _
                & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.nr_zamow in('" & Session("kod_dyspo") & "') and dm.schemat in('" & Session("schemat_dyspo") & "')" _
                & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat order by dm.nr_zamow) w"

            GridViewDyspozycje.DataSource = Nothing
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridViewDyspozycje.DataSource = dsPartie.Tables(0)
            GridViewDyspozycje.DataBind()
            cmd.Dispose()
            conn.Close()
        End Using
    End Sub

    Public Sub LadujDaneGridViewLosowanieInfo()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            sqlexp = "select rownum as lp,w.nr_zamow, w.schemat, w.skrot, desql_japa_nwa.fsql_japa_rnaz(get_index_tow(w.skrot)) nazwa, " _
            & " desql_japa_nwa.fsql_japa_rkod(get_index_tow(w.skrot)) kod_tow, w.ilosc, w.jm, w.status,w.stan_700,w.typ_oper from (" _
            & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ilosc, " _
            & " zg.data_zam, to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.stan_700,dm.typ_oper," _
            & " (select distinct zd.jm from ht_zod zd where zd.ie$0 like dm.nr_zamow||'%' and zd.is_deleted = 'N' and zd.skrot = dm.skrot) jm" _
            & " from dp_swm_mia_buf dm, dp_swm_mia_zog zg where dm.nr_zamow in('" & Session("kod_dyspo") & "') and dm.schemat in('" & Session("schemat_dyspo") & "')" _
            & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat and dm.dyspozycja='N' order by dm.nr_zamow) w"

            GridViewLosowanie.DataSource = Nothing
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridViewLosowanie.DataSource = dsPartie.Tables(0)
            GridViewLosowanie.DataBind()
            cmd.Dispose()
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnulowanieDyspo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnulowanieDyspo.Click
        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("kod_dyspo") IsNot Nothing And Session("kod_dyspo") <> "" Then

                dajarSWMagazyn_MIA.MyFunction.ZRELA1(Session("kod_dyspo"), Session("schemat_dyspo"))
                dajarSWMagazyn_MIA.MyFunction.ZRELA2(Session("kod_dyspo"), Session("schemat_dyspo"))

                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "RW" & "[" & Session("schemat_dyspo") & "][" & Session("mag_dyspo") & "]", Session("kod_dyspo"))
                dajarSWMagazyn_MIA.MyFunction.AnulowanieDyspozycjaHT_ZOG(Session("kod_dyspo"), Session("schemat_dyspo"))
                dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(Session("kod_dyspo"), Session("schemat_dyspo"), "RW")

                Dim informacje As String = TBInformacjeDodatkowe.Text.Replace("'", "")
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp("update dp_swm_mia_mag set info='" & informacje & "' where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "'", conn)

                sqlexp = "update dp_swm_mia_mag set status='RW' where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_pak set status='RW' where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_buf set dyspozycja='Y' where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                Session("status_dyspo") = "RW"
                Response.Redirect("a_manageEdit.aspx")
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BPobierzNrDigit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzNrDigit.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If LSchemat.Text = "DOMINUS" And LNr_zamow_o.Text <> "" Then
                LNrZamowDigit.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select nr_zamow from ht_zog where nr_zamow_o = '" & LNr_zamow_o.Text & "'", conn)
            End If
            conn.Close()
        End Using

    End Sub

    Protected Sub BSpakowaneDyspo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BSpakowaneDyspo.Click
        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("kod_dyspo") IsNot Nothing And Session("kod_dyspo") <> "" Then
                Dim nr_zamow As String = Session("kod_dyspo").ToString
                Dim schemat As String = Session("schemat_dyspo").ToString

                sqlexp = "insert into dp_swm_mia_pak (login,hash,nr_zamow,autodata,status,schemat,mag,etykieta_id,ile_poz) " _
                & " select '" & Session("mylogin").ToString & "','" & Session("myhash") & "','" & nr_zamow & "',TO_TIMESTAMP('2015/01/08 11:45', 'RR/MM/DD HH24:MI:SS'),'PE','" & schemat & "','" & Session("mag_dyspo") & "','X','0' from dual" _
                & " where ((select count(*) from dp_swm_mia_pak where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "') = 0)"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                Dim shippment_id As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                Dim paczka_id As String = dajarSWMagazyn_MIA.MyFunction.GeneratePaczkaId

                sqlexp = "insert into dp_swm_mia_paczka (nr_zamow,schemat,shipment_id,autodata) " _
                & " select '" & nr_zamow & "','" & schemat & "','" & shippment_id & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') from dual" _
                & " where ((select count(*) from dp_swm_mia_paczka where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "') = 0)"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                Dim shipment_id_kopia As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select shipment_id from dp_swm_mia_paczka where nr_zamow='" & nr_zamow & "' and schemat='" & schemat & "'", conn)
                If shipment_id_kopia <> "" Then
                    shippment_id = shipment_id_kopia
                End If

                sqlexp = "insert into dp_swm_mia_paczka_info (schemat,shipment_id,paczka_id,firma_id,pl_type,pl_weight,pl_width,pl_height,pl_length,pl_quantity,pl_non_std,pl_euro_ret) " _
                & " select '" & schemat & "','" & shippment_id & "','" & paczka_id & "','','1',1,1,1,1,1,'0','0' from dual" _
                & " where ((select count(*) from dp_swm_mia_paczka_info where shipment_id='" & shippment_id & "' and schemat = '" & schemat & "') = 0)"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                ''AKTUALIZACJA 25.05.2019 // ZMIANA STATUSU BUFORA ZAMOWIEN NA AUTOPAKOWANIU
                sqlexp = "update dp_swm_mia_buf set dyspozycja='Y' where nr_zamow='" & nr_zamow & "' and schemat='" & schemat & "' and dyspozycja='N'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                sqlexp = "update dp_swm_mia_pak set status = 'PE', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and schemat='" & schemat & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_mag set status = 'PE', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and schemat='" & schemat & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PE" & "[" & schemat & "][" & Session("mag_dyspo") & "]", nr_zamow)
                dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(nr_zamow, schemat, "PE")

                Session("status_dyspo") = "PE"
                Response.Redirect("a_manageEdit.aspx")
            End If
            conn.Close()
        End Using

    End Sub

    Protected Sub BGenerujWydruk_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BGenerujWydruk.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim wydruk_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT WYDRUK_ID from dp_swm_mia_wydruk WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

            If wydruk_id <> "" Then
                sqlexp = "delete from dp_swm_mia_wydruk where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and wydruk_id='" & wydruk_id & "'"
                dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
            End If

            LWydrukID.Text = ""
            conn.Close()
        End Using
    End Sub

    Protected Sub BStatusPE_Click(sender As Object, e As EventArgs) Handles BStatusPE.Click
        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("kod_dyspo") IsNot Nothing And Session("kod_dyspo") <> "" Then
                Dim nr_zamow As String = Session("kod_dyspo").ToString
                Dim schemat As String = Session("schemat_dyspo").ToString

                ''AKTUALIZACJA 25.05.2019 // ZMIANA STATUSU BUFORA ZAMOWIEN NA AUTOPAKOWANIU
                sqlexp = "update dp_swm_mia_buf set dyspozycja='Y' where nr_zamow='" & nr_zamow & "' and schemat='" & schemat & "' and dyspozycja='N'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                sqlexp = "update dp_swm_mia_zog set stat_u1 = 'PE', autodata = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and schemat='" & schemat & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_mag set status = 'PE', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and schemat='" & schemat & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_pak set status = 'PE', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and schemat='" & schemat & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PE" & "[" & schemat & "][" & Session("mag_dyspo") & "]", nr_zamow)
                dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(nr_zamow, schemat, "PE")

                Session("status_dyspo") = "PE"
                Response.Redirect("a_manageEdit.aspx")
            End If
            conn.Close()
        End Using


    End Sub
End Class