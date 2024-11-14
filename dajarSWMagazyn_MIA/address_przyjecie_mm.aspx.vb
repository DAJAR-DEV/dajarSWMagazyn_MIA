Imports Oracle.ManagedDataAccess.Client
Imports System.Data

Partial Public Class address_przyjecie_mm
    Inherits System.Web.UI.Page
    Dim sqlexp As String = ""
    Dim result As Boolean = False
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder

    Protected Sub GridViewDokumentMM_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewDokumentMM.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewDokumentMM.PageIndex = e.NewPageIndex
        GridViewDokumentMM.DataBind()

        LoadSzczegolyDokumentuMM()
    End Sub

    Protected Sub DDLHala_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLHala.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim hala As String = DDLHala.SelectedValue.ToString

            If hala <> "" Then
                RefreshDDLRzad(DDLRzad, hala)
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub DDLRzad_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRzad.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim hala As String = DDLHala.SelectedValue.ToString
            Dim rzad As String = DDLRzad.SelectedValue.ToString

            If hala <> "" And rzad <> "" Then
                RefreshDDLRegal(DDLRegal, hala, rzad)
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub DDLRegal_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRegal.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim hala As String = DDLHala.SelectedValue.ToString
            Dim rzad As String = DDLRzad.SelectedValue.ToString
            Dim regal As String = DDLRegal.SelectedValue.ToString

            If hala <> "" And rzad <> "" Then
                RefreshDDLPolka(DDLRegal, hala, rzad, regal)
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLHala(ByRef ddlObiekt As DropDownList)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()

            sqlexp = "select distinct hala from dp_swm_mia_adresy where is_active='1' order by hala"
            Dim cmd As New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            dr = cmd.ExecuteReader()
            Try
                While dr.Read()
                    ddlObiekt.Items.Add(New ListItem(dr.Item(0).ToString, dr.Item(0).ToString))
                End While

            Catch ex As System.ArgumentOutOfRangeException
                Console.WriteLine(ex)
            End Try
            dr.Close()
            cmd.Dispose()

            ddlObiekt.SelectedValue = "A"
            ddlObiekt.DataBind()
            RefreshDDLRzad(DDLRzad, DDLHala.SelectedValue.ToString)
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLRzad(ByRef ddlObiekt As DropDownList, ByVal hala As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()

            If hala <> "" Then
                sqlexp = "select distinct rzad from dp_swm_mia_adresy where hala='" & hala & "' and is_active='1' order by rzad"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                dr = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        ddlObiekt.Items.Add(New ListItem(dr.Item(0).ToString, dr.Item(0).ToString))
                    End While

                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()

                ddlObiekt.SelectedValue = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select min(distinct rzad) from dp_swm_mia_adresy where hala='" & hala & "' and is_active='1' order by rzad", conn)
            Else
                Dim znak As String = ""
                For i = 1 To 99
                    znak = i.ToString().PadLeft(2, "0")
                    ddlObiekt.Items.Add(New ListItem("A" & i.ToString, "A" & i.ToString))
                    ddlObiekt.Items.Add(New ListItem(znak, znak))
                Next

                ddlObiekt.SelectedValue = "01"
            End If

            RefreshDDLRegal(DDLRegal, DDLHala.SelectedValue.ToString, DDLRzad.SelectedValue.ToString)
            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLRegal(ByRef ddlObiekt As DropDownList, ByVal hala As String, ByVal rzad As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()

            If hala <> "" And rzad <> "" Then
                sqlexp = "select distinct regal from dp_swm_mia_adresy where hala='" & hala & "' and rzad='" & rzad & "' and is_active='1' order by regal"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                dr = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        ddlObiekt.Items.Add(New ListItem(dr.Item(0).ToString, dr.Item(0).ToString))
                    End While

                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()

                ddlObiekt.SelectedValue = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select min(distinct regal) from dp_swm_mia_adresy where hala='" & hala & "' and rzad='" & rzad & "' and is_active='1' order by rzad", conn)
            Else
                Dim znak As String = ""
                For i = 1 To 99
                    znak = i.ToString().PadLeft(2, "0")
                    ddlObiekt.Items.Add(New ListItem(znak, znak))
                Next
                ddlObiekt.SelectedValue = "01"
            End If

            RefreshDDLPolka(DDLPolka, DDLHala.SelectedValue.ToString, DDLRzad.SelectedValue.ToString, DDLRegal.SelectedValue.ToString)
            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLPolka(ByRef ddlObiekt As DropDownList, ByVal hala As String, ByVal rzad As String, ByVal regal As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()

            If hala <> "" And rzad <> "" Then
                sqlexp = "select distinct polka from dp_swm_mia_adresy where hala='" & hala & "' and rzad='" & rzad & "' and regal='" & regal & "' and is_active='1' order by regal"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                dr = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        ddlObiekt.Items.Add(New ListItem(dr.Item(0).ToString, dr.Item(0).ToString))
                    End While

                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()

                ddlObiekt.SelectedValue = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select min(distinct polka) from dp_swm_mia_adresy where hala='" & hala & "' and rzad='" & rzad & "' and regal='" & regal & "' and is_active='1' order by rzad", conn)
            Else
                Dim znak As String = ""
                For i = 0 To 99
                    znak = i.ToString().PadLeft(2, "0")
                    ddlObiekt.Items.Add(New ListItem(znak, znak))
                Next
                ddlObiekt.SelectedValue = "00"
            End If

            ddlObiekt.DataBind()
            conn.Close()
        End Using
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
                sqlexp = "SELECT TYP_OPER FROM dp_swm_mia_UZYT WHERE LOGIN = '" & login & "'"
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

    Protected Sub RefreshDDLDokumentMM(ByRef ddlObiekt As DropDownList, ByRef sesja_bierzaca As System.Web.SessionState.HttpSessionState)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim sqlExp As String = ""
            If Session("filtr_mm") = "1" Then
                sqlExp = "select distinct dm.dok_mm from dp_swm_mia_dok_mm dm, dp_swm_mia_towary_mm tm where dm.dok_mm = tm.dok_mm and dm.status in(1) and tm.login='" & Session("mylogin") & "' order by dm.dok_mm asc"
            ElseIf Session("filtr_mm") = "2" Then
                sqlExp = "select distinct dm.dok_mm from dp_swm_mia_dok_mm dm, dp_swm_mia_towary_mm tm where dm.dok_mm = tm.dok_mm and dm.status in(2) and tm.login='" & Session("mylogin") & "' order by dm.dok_mm asc"
            Else
                sqlExp = "select distinct dok_mm from dp_swm_mia_dok_mm where status in(0) order by dok_mm asc"
            End If

            ddlObiekt.Items.Clear()
            Dim cmd As New OracleCommand(sqlExp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            If dr.HasRows Then
                ddlObiekt.DataSource = dr
                ddlObiekt.DataTextField = "DOK_MM"
                ddlObiekt.DataValueField = "DOK_MM"
                ddlObiekt.DataBind()
            End If
            dr.Close()
            cmd.Dispose()

            ddlObiekt.Items.Add(New ListItem("WYBIERZ", "WYBIERZ", True))

            If sesja_bierzaca("dok_mm") IsNot Nothing Then
                ddlObiekt.SelectedValue = sesja_bierzaca("dok_mm")
            Else
                ddlObiekt.SelectedValue = "WYBIERZ"
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub page_loaded() Handles Me.LoadComplete
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session("contentKomunikat") = Session(session_id)
    End Sub

    Protected Sub CBPokazZakonczoneMM_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBPokazZakonczoneMM.CheckedChanged
        If CBPokazZakonczoneMM.Checked Then
            CBPokazArchiwumMM.Checked = False
            Session("filtr_mm") = "2"
        Else
            Session.Remove("filtr_mm")
        End If

        Session.Remove("dok_mm")
        RefreshDDLDokumentMM(DDLDokumentMM, Session)
    End Sub

    Protected Sub CBPokazArchiwumMM_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBPokazArchiwumMM.CheckedChanged
        If CBPokazArchiwumMM.Checked Then
            CBPokazZakonczoneMM.Checked = False
            Session("filtr_mm") = "1"
        Else
            Session.Remove("filtr_mm")
        End If

        Session.Remove("dok_mm")
        RefreshDDLDokumentMM(DDLDokumentMM, Session)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
            Dim przerwijLadowanie As Boolean = False

            BDokumentMMZakoncz.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zakonczyc wprowadzanie wybranego dokumentu MM?\n') == false) return false")
            BDokumentMMReczneZakoncz.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz recznie zakonczyc wybrany dokumentu MM?\n') == false) return false")

            If Session("mylogin") = Nothing And Session("myhash") = Nothing Then
                Session.Abandon()
                Response.Redirect("index.aspx")
            ElseIf Session("mytyp_oper") <> "M" And Session("mytyp_oper") <> "W" Then
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Zaloguj sie na opowiedniego operatora - typ MAGAZYN/WOZEK<br />"
                Session(session_id) += "</div>"
                PanelMagazyn.Visible = False
                przerwijLadowanie = True
            Else
                Session.Remove(session_id)
            End If

            Session("contentMagazyn") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN = '" & Session("mylogin") & "'", conn)

            If Not Page.IsPostBack Then
                sqlexp = "INSERT INTO dp_swm_mia_DOK_MM (dok_mm,status,autodata) (" _
                & "select mt.dok_mm,'0',CURRENT_TIMESTAMP from ht_mmtr mt where mt.ie$0 like '700  44%' and mt.is_deleted='N' and mt.dok_mm not in(select distinct dok_mm from dp_swm_mia_dok_mm))"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                If CBPokazArchiwumMM.Checked Then : Session("filtr_mm") = "1"
                ElseIf CBPokazZakonczoneMM.Checked Then : Session("filtr_mm") = "2"
                Else : Session.Remove("filtr_mm")
                End If

                RefreshDDLHala(DDLHala)
                ''RefreshDDLRzad(DDLRzad)
                ''RefreshDDLRegal(DDLRegal)
                RefreshDDLDokumentMM(DDLDokumentMM, Session)
                dajarSWMagazyn_MIA.MyFunction.RefreshDDLOperator(DDLOperator, Session)

                If Session("dok_mm") IsNot Nothing Then
                    PanelMagazynData.Visible = True
                Else
                    PanelMagazynData.Visible = False
                End If

                If przerwijLadowanie = False Then
                    ''LadujDaneGridViewMagazyn("")
                End If
            End If

            Session("contentKomunikat") = Session(session_id)
            conn.Close()
        End Using
    End Sub

    Protected Sub BDokumentMMAnuluj_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BDokumentMMAnuluj.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("dok_mm") IsNot Nothing And Session("mm_skrot") IsNot Nothing Then
                Dim dok_mm As String = Session("dok_mm")
                Dim split_str() As String = {";"}
                Dim t_skrot() As String = Session("mm_skrot").ToString.Split(split_str, StringSplitOptions.RemoveEmptyEntries)

                Dim mm_adres As String = GridViewDokumentMM.SelectedRow.Cells(11).Text.ToString
                Dim mm_strefa As String = GridViewDokumentMM.SelectedRow.Cells(12).Text.ToString

                For Each skrot In t_skrot
                    For Each row As GridViewRow In GridViewDokumentMM.Rows
                        If row.Cells(3).Text = skrot Then
                            mm_adres = row.Cells(10).Text.ToString
                            mm_strefa = row.Cells(11).Text.ToString
                            Exit For
                        End If
                    Next

                    Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                    sqlexp = "delete from dp_swm_mia_towary_mm where dok_mm='" & dok_mm & "' and skrot='" & skrot & "' and adres='" & mm_adres & "' and login='" & Session("mylogin") & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "delete from dp_swm_mia_towary where skrot='" & skrot & "' and adres='" & mm_adres & "' and login='" & Session("mylogin") & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                Next

                LoadSzczegolyDokumentuMM()

                ''If mm_strefa <> "Z" And mm_strefa <> "N" And mm_strefa <> "P" Then
                ''    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                ''    Session(session_id) += "<br />Dla wybranego dokumentu MM nie zostały przyporzadkowane wszystkie adresy zamagazynowania.<br />"
                ''    Session(session_id) += "</div>"
                ''Else
                ''    Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                ''    sqlexp = "delete from dp_swm_mia_towary_mm where dok_mm='" & dok_mm & "' and skrot='" & mm_skrot & "' and adres='" & mm_adres & "' and login='" & Session("mylogin") & "'"
                ''    ''sqlexp = "update dp_swm_mia_towary_mm set adres='',strefa='',autodata=TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where dok_mm='" & dok_mm & "' and skrot='" & mm_skrot & "' and adres='" & mm_adres & "' and login='" & Session("mylogin") & "'"
                ''    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                ''    LoadSzczegolyDokumentuMM()
                ''End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BDokumentMMZapisz_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BDokumentMMZapisz.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("dok_mm") IsNot Nothing And Session("mm_skrot") IsNot Nothing Then
                Dim dok_mm As String = Session("dok_mm")
                Dim mm_adres As String = ""
                Dim mm_strefa As String = DDLStrefaMagazyn.SelectedValue.ToString

                If Session("mm_adres") IsNot Nothing Then
                    mm_adres = Session("mm_adres")
                Else
                    Dim hala As String = DDLHala.SelectedValue.ToString
                    Dim rzad As String = DDLRzad.SelectedValue.ToString
                    Dim regal As String = DDLRegal.SelectedValue.ToString
                    Dim polka As String = DDLPolka.SelectedValue.ToString
                    mm_adres = hala & rzad & regal & polka
                End If

                Dim split_str() As String = {";"}
                Dim t_skrot() As String = Session("mm_skrot").ToString.Split(split_str, StringSplitOptions.RemoveEmptyEntries)

                For Each skrot In t_skrot
                    Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()

                    sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_towary WHERE adres='" & mm_adres & "' and skrot='" & skrot & "' and strefa='" & mm_strefa & "'"
                    Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If czy_istnieje = "0" Then
                        sqlexp = "insert into dp_swm_mia_towary values('" & skrot & "','" & mm_adres & "','" & mm_strefa & "','1','" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    Else
                        sqlexp = "update dp_swm_mia_towary set is_active='1',login='" & Session("mylogin") & "', hash='" & Session("myhash") & "' where skrot='" & skrot & "' and adres='" & mm_adres & "' and strefa='" & mm_strefa & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If

                    sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_towary_mm WHERE adres='" & mm_adres & "' and skrot='" & skrot & "' and strefa='" & mm_strefa & "' and dok_mm='" & dok_mm & "'"
                    czy_istnieje = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If czy_istnieje = "0" Then
                        sqlexp = "insert into dp_swm_mia_towary_mm values('" & dok_mm & "','" & skrot & "','" & mm_adres & "','" & mm_strefa & "','1','" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    Else
                        sqlexp = "update dp_swm_mia_towary_mm set login='" & Session("mylogin") & "', hash='" & Session("myhash") & "',adres='" & mm_adres & "' where skrot='" & skrot & "' and adres='" & mm_adres & "' and strefa='" & mm_strefa & "' and dok_mm='" & dok_mm & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If

                    sqlexp = "update dp_swm_mia_dok_mm set status='1' where dok_mm='" & dok_mm & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    dajarSWMagazyn_MIA.MyFunction.SetHistoriaTowary(skrot, mm_adres, mm_strefa, "1", "OP_MM", Session("mylogin"), Session("myhash"))
                Next

                LoadSzczegolyDokumentuMM()
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub DDLDokumentMM_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLDokumentMM.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim dok_mm As String = DDLDokumentMM.SelectedValue.ToString

            If dok_mm.Length = "12" Then
                TBDokumentMM.Text = dok_mm
            End If
            conn.Close()
        End Using
    End Sub

    Public Sub LoadSzczegolyDokumentuMM()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If TBDokumentMM.Text.Length = "12" Then
                Session("dok_mm") = TBDokumentMM.Text.ToString

                sqlexp = "select w.*, dt.adres, dt.strefa, dt.is_active from (" _
                & " select mm.dok_mm, mm.skrot, mm.nazwa, r.kod_tow, r.skt2_0, r.klasa, mm.jm, sum(mm.ilosc) s_ilosc" _
                & " from ht_mm mm, ht_rejna r " _
                & " where mm.ie$0 = DESQL_GRAF.DF11_2('" & Session("dok_mm") & "') and lpad(mm.indeks,10) = r.ie$0 and r.is_deleted='N' and r.kod_tow <> ' '" _
                & " group by mm.dok_mm,mm.skrot,mm.indeks,mm.nazwa,r.kod_tow,r.skt2_0,r.klasa,mm.jm" _
                & " order by mm.dok_mm) w left join dp_swm_mia_towary_mm dt on w.skrot = dt.skrot and w.dok_mm = dt.dok_mm order by w.skrot"

                GridViewDokumentMM.DataSource = Nothing
                Dim cmd As New OracleCommand(sqlexp, conn)
                daPartie = New OracleDataAdapter(cmd)
                cb = New OracleCommandBuilder(daPartie)
                dsPartie = New DataSet()
                daPartie.Fill(dsPartie)
                GridViewDokumentMM.DataSource = dsPartie.Tables(0)
                GridViewDokumentMM.DataBind()
                cmd.Dispose()

                If dsPartie.Tables(0).Rows.Count = 0 Then
                    LIleDokumentow.Text = "brak"
                    PanelMagazynData.Visible = False
                Else
                    LIleDokumentow.Text = dsPartie.Tables(0).Rows.Count.ToString
                    PanelMagazynData.Visible = True
                End If

                If GridViewDokumentMM.Rows.Count > 0 Then
                    For Each row As GridViewRow In GridViewDokumentMM.Rows
                        Dim cBox As CheckBox = row.FindControl("CBKodSelect")
                        cBox.Checked = False
                    Next
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BDokumentMMWczytaj_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BDokumentMMWczytaj.Click
        LoadSzczegolyDokumentuMM()
    End Sub

    Protected Sub BDokumentMMReczneZakoncz_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BDokumentMMReczneZakoncz.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("dok_mm") IsNot Nothing Then
                Dim zakoncz_mm As Boolean = True
                For Each row As GridViewRow In GridViewDokumentMM.Rows
                    Dim strefa As String = row.Cells(11).Text.ToString
                    If strefa <> "Z" And strefa <> "N" And strefa <> "P" Then
                        zakoncz_mm = False
                        Exit For
                    End If
                Next

                sqlexp = "update dp_swm_mia_dok_mm set status='2' where dok_mm='" & Session("dok_mm").ToString & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                Session.Remove("dok_mm")
                RefreshDDLDokumentMM(DDLDokumentMM, Session)
                Response.Redirect("address_przyjecie_mm.aspx")
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BDokumentMMZakoncz_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BDokumentMMZakoncz.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("dok_mm") IsNot Nothing Then
                Dim zakoncz_mm As Boolean = True
                For Each row As GridViewRow In GridViewDokumentMM.Rows
                    Dim strefa As String = row.Cells(11).Text.ToString
                    If strefa <> "Z" And strefa <> "N" And strefa <> "P" Then
                        zakoncz_mm = False
                        Exit For
                    End If
                Next

                If zakoncz_mm = False Then
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Dla wybranego dokumentu MM nie zostały przyporzadkowane wszystkie adresy zamagazynowania.<br />"
                    Session(session_id) += "</div>"
                Else
                    sqlexp = "update dp_swm_mia_dok_mm set status='2' where dok_mm='" & Session("dok_mm").ToString & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    Session.Remove("dok_mm")
                    RefreshDDLDokumentMM(DDLDokumentMM, Session)
                    Response.Redirect("address_przyjecie_mm.aspx")
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub GridViewDokumentMM_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewDokumentMM.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim cb As CheckBox = e.Row.FindControl("CBKodSelect")
            If cb IsNot Nothing And cb.Checked Then
                e.Row.BackColor = GridViewDokumentMM.SelectedRowStyle.BackColor
            Else
                Dim strefa As String = e.Row.Cells(11).Text.ToString
                If strefa = "Z" Then : e.Row.BackColor = LTowarZbieranie.BackColor
                ElseIf strefa = "N" Then : e.Row.BackColor = LTowarNiepelna.BackColor
                ElseIf strefa = "P" Then : e.Row.BackColor = LTowarPaleta.BackColor
                Else : e.Row.BackColor = LTowarPusty.BackColor
                End If
            End If
        End If
    End Sub

    Protected Sub GridViewDokumentMM_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewDokumentMM.SelectedIndexChanged
        ''If GridViewDokumentMM.Rows.Count > 0 Then
        ''    For Each row As GridViewRow In GridViewDokumentMM.Rows
        ''        Dim cBox As CheckBox = row.FindControl("CBKodSelect")
        ''        cBox.Checked = False
        ''    Next
        ''End If

        Session.Remove("mm_skrot")
        Session.Remove("mm_adres")
        Session.Remove("mm_strefa")

        Dim cb As CheckBox = GridViewDokumentMM.SelectedRow.FindControl("CBKodSelect")
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
        Else
            cb.Checked = True
            Dim skrot As String = GridViewDokumentMM.SelectedRow.Cells(3).Text.ToString()
            Dim mm_skrot As String = ""
            For Each row As GridViewRow In GridViewDokumentMM.Rows
                Dim cb_row As CheckBox = row.FindControl("CBKodSelect")
                If cb_row IsNot Nothing And cb_row.Checked Then
                    mm_skrot &= row.Cells(3).Text.ToString & ";"
                End If
            Next

            Session("mm_skrot") = mm_skrot
            LoadHistoriaSkrotuMM(Session("mm_skrot"))
        End If


    End Sub

    Public Sub LoadHistoriaSkrotuMM(ByVal mm_skrot As String)
        If GridViewDokumentMM.Rows.Count > 0 Then
            GridViewAdresacja.DataSource = Nothing
            Dim indeksy_dtable As New DataTable
            Dim polaDataTable As String() = {"SKROT", "ADRES", "HALA", "RZAD", "REGAL", "STREFA"}
            For Each pole In polaDataTable
                indeksy_dtable.Columns.Add(New DataColumn(pole, Type.GetType("System.String")))
            Next

            Dim split_str() As String = {";"}
            Dim t_skrot() As String = mm_skrot.Split(split_str, StringSplitOptions.RemoveEmptyEntries)

            For Each skrot In t_skrot
                sqlexp = "select distinct w1.* from (" _
                & " select dt.skrot, dt.adres, substr(dt.adres,0,1) hala, substr(dt.adres,2,2) rzad, substr(dt.adres,4,2) regal, dt.strefa, dt.is_active from dp_swm_mia_towary dt where dt.skrot = '" & skrot & "')" _
                & " w1 order by w1.skrot asc, decode(w1.strefa, 'Z', 1, 'N', 2, 'P', 3)"

                Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                    conn.Open()
                    Dim cmd As New OracleCommand(sqlexp, conn)
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
            Next

            GridViewAdresacja.DataSource = indeksy_dtable
            GridViewAdresacja.DataBind()
        End If
    End Sub

    Protected Sub GridViewAdresacja_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewAdresacja.SelectedIndexChanged
        If GridViewAdresacja.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewAdresacja.Rows
                Dim cBox As CheckBox = row.FindControl("CBKodSelect")
                cBox.Checked = False
            Next
        End If

        Session.Remove("mm_adres")
        Session.Remove("mm_strefa")

        Dim cb As CheckBox = GridViewAdresacja.SelectedRow.FindControl("CBKodSelect")
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
        Else
            cb.Checked = True
            Session("mm_adres") = GridViewAdresacja.SelectedRow.Cells(3).Text.ToString
            Session("mm_strefa") = GridViewAdresacja.SelectedRow.Cells(7).Text.ToString

            Aktualizacja_Adres_Historia(Session("mm_adres"), Session("mm_strefa"))
        End If
    End Sub

    Protected Sub Aktualizacja_Adres_Historia(ByVal adres As String, ByVal strefa As String)
        DDLHala.SelectedValue = adres.Substring(0, 1).ToString
        RefreshDDLRzad(DDLRzad, DDLHala.SelectedValue)
        DDLRzad.SelectedValue = adres.Substring(1, 2).ToString
        RefreshDDLRegal(DDLRegal, DDLHala.SelectedValue, DDLRzad.SelectedValue)
        DDLRegal.SelectedValue = adres.Substring(3, 2).ToString
        RefreshDDLPolka(DDLPolka, DDLHala.SelectedValue, DDLRzad.SelectedValue, DDLRegal.SelectedValue.ToString)
        DDLPolka.SelectedValue = adres.Substring(5, 2).ToString
        DDLStrefaMagazyn.SelectedValue = strefa.ToString
    End Sub

End Class