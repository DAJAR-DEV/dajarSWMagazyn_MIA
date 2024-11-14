Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class address_auto_braki
    Inherits System.Web.UI.Page
    Dim sqlexp As String = ""
    Dim result As Boolean = False
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder

    Public Class ObiektMagazynowy
        Public skrot As String = ""
        Public adres As String = ""
        Public strefa As String = ""
        Public is_active As String = ""

        Public Sub New(ByVal _skrot As String, ByVal _adres As String, ByVal _strefa As String, ByVal _is_active As String)
            skrot = _skrot
            adres = _adres
            strefa = _strefa
            is_active = _is_active
        End Sub
    End Class

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

    Protected Sub RefreshDDLHala(ByRef ddlObiekt As DropDownList)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()

            sqlexp = "select distinct hala from dp_swm_mia_adresy order by hala"
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
                sqlexp = "select distinct rzad from dp_swm_mia_adresy where hala='" & hala & "' order by rzad"
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

                ddlObiekt.SelectedValue = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select min(distinct rzad) from dp_swm_mia_adresy where hala='" & hala & "' order by rzad", conn)
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
                sqlexp = "select distinct regal from dp_swm_mia_adresy where hala='" & hala & "' and rzad='" & rzad & "' order by regal"
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

                ddlObiekt.SelectedValue = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select min(distinct regal) from dp_swm_mia_adresy where hala='" & hala & "' and rzad='" & rzad & "' order by rzad", conn)
            Else
                Dim znak As String = ""
                For i = 1 To 99
                    znak = i.ToString().PadLeft(2, "0")
                    ddlObiekt.Items.Add(New ListItem(znak, znak))
                Next
                ddlObiekt.SelectedValue = "01"
            End If

            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub

    Protected Sub GridViewTowary_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewTowary.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewTowary.PageIndex = e.NewPageIndex
        GridViewTowary.DataBind()
        LadujDaneGridViewMagazyn(TBFiltrowanie.Text.ToString)
    End Sub

    Protected Sub GridViewAdresy_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewAdresy.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewAdresy.PageIndex = e.NewPageIndex
        GridViewAdresy.DataBind()
        AktualizujGridViewAdresy()
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Dim przerwijLadowanie As Boolean = False

        BTowarZlozenie.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zlozyc wskazane artykuly?\n') == false) return false")
        BDyspozycjaZakoncz.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zakonczyc automagazynowanie towaru?\n') == false) return false")

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

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session("contentMagazyn") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN = '" & Session("mylogin") & "'", conn)
            conn.Close()
        End Using

        If Not Page.IsPostBack Then
            RefreshDDLHala(DDLHala)
            PWozek.Visible = False

            dajarSWMagazyn_MIA.MyFunction.RefreshDDLOperator(DDLOperator, Session)
            If Session("mytyp_oper") = "W" Then
                PWozek.Visible = True
            End If
            If przerwijLadowanie = False Then
                LadujDaneGridViewMagazyn("")
            End If
        End If

        Session("contentKomunikat") = Session(session_id)
    End Sub

    Public Sub LadujDaneGridViewMagazyn(ByVal filtr As String)
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("mytyp_oper") = "W" Then
                sqlexp = "select count(*) from dp_swm_mia_wozek where login='" & Session("mylogin") & "' and hash='" & Session("myhash") & "' and is_active='1'"
                Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If czy_istnieje <> "0" Then
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Zakoncz wszystkie zadania w zakladce PRZEMAG. WŁASNE/ZŁOŻENIE TOWARU!<br />"
                    Session(session_id) += "</div>"
                    PanelMagazyn.Visible = False
                Else
                    Dim filename As String = "\sql\automag_braki.sql"
                    Dim fileInput As New StreamReader(dajarSWMagazyn_MIA.MyFunction.networkMainPath & filename, System.Text.Encoding.GetEncoding(1250))
                    Dim bufor As String = fileInput.ReadToEnd
                    fileInput.Close()
                    Dim sql_1 As String = bufor

                    If filtr <> "" Then
                        sql_1 &= " and (w3.skrot like '" & filtr & "%')"
                    End If

                    sql_1 &= " order by w3.skrot asc"

                    Dim result = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sql_1, conn)
                    If result.Length > 0 Then
                        sqlexp = sql_1
                        Session("mm_sql") = 1
                        Label10.Text = "ZAKŁADKA AUTOMAGAZYNOWANIE TOWARU WARIANT BRAKI"
                    Else
                        sqlexp = ""
                        Session.Remove("mm_sql")
                        Label10.Text = "ZAKŁADKA AUTOMAGAZYNOWANIE TOWARU"
                    End If
                End If
            End If

            If sqlexp.Length > 1 Then
                Dim cmd_reader As New OracleCommand(sqlexp, conn)
                cmd_reader.CommandType = CommandType.Text
                Dim reader As OracleDataReader = cmd_reader.ExecuteReader()

                If reader.HasRows Then
                    Dim cmd As New OracleCommand(sqlexp, conn)
                    daPartie = New OracleDataAdapter(cmd)
                    cb = New OracleCommandBuilder(daPartie)
                    dsPartie = New DataSet()
                    daPartie.Fill(dsPartie)
                    GridViewTowary.DataSource = dsPartie.Tables(0)
                    GridViewTowary.DataBind()
                    cmd.Dispose()

                    LIleDokumentow.Text = dsPartie.Tables(0).Rows.Count.ToString
                    PanelMagazynData.Visible = True

                Else
                    LIleDokumentow.Text = "brak"
                    PanelMagazynData.Visible = False
                End If
            Else
                LIleDokumentow.Text = "brak"
                PanelMagazynData.Visible = False
            End If

            ''GridViewTowary.DataSource = Nothing
            ''Dim cmd As New OracleCommand(sqlexp, conn)
            ''daPartie = New OracleDataAdapter(cmd)
            ''cb = New OracleCommandBuilder(daPartie)
            ''dsPartie = New DataSet()
            ''daPartie.Fill(dsPartie)
            ''GridViewTowary.DataSource = dsPartie.Tables(0)
            ''GridViewTowary.DataBind()
            ''cmd.Dispose()

            ''If dsPartie.Tables(0).Rows.Count = 0 Then
            ''    LIleDokumentow.Text = "brak"
            ''    PanelMagazynData.Visible = False
            ''Else
            ''    LIleDokumentow.Text = dsPartie.Tables(0).Rows.Count.ToString
            ''    PanelMagazynData.Visible = True
            ''End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BDyspozycjaZakoncz_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BDyspozycjaZakoncz.Click
        ''Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        ''Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
        ''    conn.Open()

        ''    sqlexp = "select skrot from dp_swm_mia_towary_hist where oper='OP_ZL' and login='" & Session("mylogin") & "' and hash='" & Session("myhash") & "' order by skrot"
        ''    Dim cmd As New OracleCommand(sqlexp, conn)
        ''    cmd.CommandType = CommandType.Text
        ''    Dim dr As OracleDataReader = cmd.ExecuteReader()
        ''    dr = cmd.ExecuteReader()
        ''    Try
        ''        While dr.Read()
        ''            Dim skrot As String = dr.Item(0).ToString
        ''            sqlexp = "update dp_swm_mia_wozek set is_active='0' where skrot='" & skrot & "' and login='" & Session("mylogin") & "'"
        ''            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
        ''        End While

        ''    Catch ex As System.ArgumentOutOfRangeException
        ''        Console.WriteLine(ex)
        ''    End Try
        ''    dr.Close()
        ''    cmd.Dispose()

        ''    LadujDaneGridViewMagazyn("")
        ''    conn.Close()
        ''End Using
        Response.Redirect("address.aspx")
    End Sub

    Protected Sub BTowarZlozenie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarZlozenie.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim adres_zlozenia As String = ""
            Dim strefa_zlozenia As String = DDLStrefaMagazyn.SelectedValue.ToString
            If Session("adr_zlozenie") IsNot Nothing Then
                adres_zlozenia = Session("adr_zlozenie")
            Else
                Dim hala As String = DDLHala.SelectedValue.ToString
                Dim rzad As String = DDLRzad.SelectedValue.ToString
                Dim regal As String = DDLRegal.SelectedValue.ToString
                adres_zlozenia = hala & rzad & regal
            End If

            If Session("mm_obiekt") IsNot Nothing Then
                Dim ob_list As New List(Of ObiektMagazynowy)
                ob_list = Session("mm_obiekt")

                For Each ob_mag In ob_list
                    ob_mag.is_active = "1"

                    Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                    sqlexp = "SELECT IS_ACTIVE FROM dp_swm_mia_towary WHERE adres='" & adres_zlozenia & "' and skrot='" & ob_mag.skrot & "' and strefa='" & strefa_zlozenia & "'"
                    Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If czy_istnieje = "" Then
                        ''sqlexp = "update dp_swm_mia_wozek set is_active='0' where adres='" & ob_mag.adres & "' and skrot='" & ob_mag.skrot & "' and strefa='" & ob_mag.strefa & "' and login='" & Session("mylogin") & "'"
                        ''result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "insert into dp_swm_mia_towary values('" & ob_mag.skrot & "','" & adres_zlozenia & "','" & strefa_zlozenia & "','" & ob_mag.is_active & "','" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "insert into dp_swm_mia_wozek_auto values('" & ob_mag.skrot & "','" & adres_zlozenia & "','" & strefa_zlozenia & "','" & ob_mag.is_active & "','" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        dajarSWMagazyn_MIA.MyFunction.SetHistoriaTowary(ob_mag.skrot, adres_zlozenia, strefa_zlozenia, ob_mag.is_active, "OP_AT", Session("mylogin"), Session("myhash"))
                    ElseIf czy_istnieje = "0" Then
                        sqlexp = "update dp_swm_mia_towary set is_active='1',login='" & Session("mylogin") & "',hash='" & Session("myhash") & "',autodata=TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where adres='" & ob_mag.adres & "' and skrot='" & ob_mag.skrot & "' and strefa='" & ob_mag.strefa & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "insert into dp_swm_mia_wozek_auto values('" & ob_mag.skrot & "','" & adres_zlozenia & "','" & strefa_zlozenia & "','" & ob_mag.is_active & "','" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        dajarSWMagazyn_MIA.MyFunction.SetHistoriaTowary(ob_mag.skrot, adres_zlozenia, strefa_zlozenia, ob_mag.is_active, "OP_AT", Session("mylogin"), Session("myhash"))
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Wybrany towar jest juz zlozony pod wskazanym adresem zamagazynowania!<br />"
                        Session(session_id) += "</div>"
                    End If
                Next
            ElseIf Session("mm_skrot") IsNot Nothing Then
                Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                sqlexp = "SELECT IS_ACTIVE FROM dp_swm_mia_towary WHERE adres='" & adres_zlozenia & "' and skrot='" & Session("mm_skrot") & "' and strefa='" & strefa_zlozenia & "'"
                Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If czy_istnieje = "" Then
                    ''sqlexp = "update dp_swm_mia_wozek set is_active='0' where adres='" & ob_mag.adres & "' and skrot='" & ob_mag.skrot & "' and strefa='" & ob_mag.strefa & "' and login='" & Session("mylogin") & "'"
                    ''result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "insert into dp_swm_mia_towary values('" & Session("mm_skrot") & "','" & adres_zlozenia & "','" & strefa_zlozenia & "','" & "1" & "','" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "insert into dp_swm_mia_wozek_auto values('" & Session("mm_skrot") & "','" & adres_zlozenia & "','" & strefa_zlozenia & "','" & "1" & "','" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    dajarSWMagazyn_MIA.MyFunction.SetHistoriaTowary(Session("mm_skrot"), adres_zlozenia, strefa_zlozenia, "1", "OP_AT", Session("mylogin"), Session("myhash"))
                ElseIf czy_istnieje = "0" Then
                    sqlexp = "update dp_swm_mia_towary set is_active='1',login='" & Session("mylogin") & "',hash='" & Session("myhash") & "',autodata=TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where adres='" & adres_zlozenia & "' and skrot='" & Session("mm_skrot") & "' and strefa='" & strefa_zlozenia & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "insert into dp_swm_mia_wozek_auto values('" & Session("mm_skrot") & "','" & adres_zlozenia & "','" & strefa_zlozenia & "','" & "1" & "','" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    dajarSWMagazyn_MIA.MyFunction.SetHistoriaTowary(Session("mm_skrot"), adres_zlozenia, strefa_zlozenia, "1", "OP_AT", Session("mylogin"), Session("myhash"))
                Else
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Wybrany towar jest juz zlozony pod wskazanym adresem zamagazynowania!<br />"
                    Session(session_id) += "</div>"
                End If

            End If

            Dim filter_data As String = TBFiltrowanie.Text
            LadujDaneGridViewMagazyn(filter_data)
            Session.Remove("mm_obiekt")
            Session.Remove("mm_skrot")
            AktualizujGridViewIndeksy()
            conn.Close()
        End Using
    End Sub

    Protected Sub GridViewAdresy_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewAdresy.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim cb As CheckBox = e.Row.FindControl("CBKodSelect")
            If cb IsNot Nothing And cb.Checked Then
                e.Row.BackColor = GridViewTowary.SelectedRowStyle.BackColor
            Else
                Dim strefa As String = e.Row.Cells(7).Text.ToString
                If strefa = "Z" Then : e.Row.BackColor = LTowarZbieranie.BackColor
                ElseIf strefa = "N" Then : e.Row.BackColor = LTowarNiepelna.BackColor
                ElseIf strefa = "P" Then : e.Row.BackColor = LTowarPaleta.BackColor
                Else : e.Row.BackColor = LTowarPusty.BackColor
                End If

                Dim is_active As String = e.Row.Cells(8).Text.ToString
                If is_active = "0" Then : e.Row.BackColor = LTowarPusty.BackColor
                End If
            End If
        End If
    End Sub

    Protected Sub GridViewIndeksy_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewIndeksy.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim cb As CheckBox = e.Row.FindControl("CBKodSelect")
            If cb IsNot Nothing And cb.Checked Then
                e.Row.BackColor = GridViewIndeksy.SelectedRowStyle.BackColor
            Else
                Dim is_active As String = e.Row.Cells(8).Text.ToString
                If is_active = "0" Then : e.Row.BackColor = LTowarPusty.BackColor
                End If
            End If
        End If
    End Sub

    Protected Sub GridViewTowary_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewTowary.SelectedIndexChanged
        ''If GridViewTowary.Rows.Count > 0 Then
        ''    For Each row As GridViewRow In GridViewTowary.Rows
        ''        Dim cBox As CheckBox = row.FindControl("CBKodSelect")
        ''        cBox.Checked = False
        ''    Next
        ''End If

        Session.Remove("mm_skrot")

        Dim cb As CheckBox = GridViewTowary.SelectedRow.FindControl("CBKodSelect")
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False

            ''Dim ob_list_mag As New List(Of ObiektMagazynowy)
            ''ob_list_mag = Session("mm_obiekt")
            ''Dim ob_mag As New ObiektMagazynowy(GridViewTowary.SelectedRow.Cells(2).Text.ToString, GridViewTowary.SelectedRow.Cells(8).Text.ToString, GridViewTowary.SelectedRow.Cells(9).Text.ToString, GridViewTowary.SelectedRow.Cells(10).Text.ToString)
            ''For i = ob_list_mag.Count - 1 To 0 Step -1
            ''    Dim ob_temp As ObiektMagazynowy = ob_list_mag(i)
            ''    If ob_temp.skrot = ob_mag.skrot And ob_temp.adres = ob_mag.adres And ob_temp.strefa = ob_mag.strefa Then
            ''        ob_list_mag.RemoveAt(i)
            ''    End If
            ''Next

            ''Session("mm_obiekt") = ob_list_mag
        Else
            cb.Checked = True
            Dim ob_list_mag As New List(Of ObiektMagazynowy)
            Dim skrot As String = GridViewTowary.SelectedRow.Cells(2).Text.ToString()
            ''Dim mm_skrot As String = ""
            ''For Each row As GridViewRow In GridViewTowary.Rows
            ''    Dim cb_row As CheckBox = row.FindControl("CBKodSelect")
            ''    If cb_row IsNot Nothing And cb_row.Checked Then
            ''        Dim ob_mag As New ObiektMagazynowy(row.Cells(2).Text.ToString, row.Cells(8).Text.ToString, row.Cells(9).Text.ToString, row.Cells(10).Text.ToString)
            ''        ob_list_mag.Add(ob_mag)
            ''        mm_skrot &= row.Cells(2).Text.ToString & ";"

            ''        DDLHala.SelectedValue = ob_mag.adres.Substring(0, 1).ToString
            ''        RefreshDDLRzad(DDLRzad, DDLHala.SelectedValue)
            ''        DDLRzad.SelectedValue = ob_mag.adres.Substring(1, 2).ToString
            ''        RefreshDDLRegal(DDLRegal, DDLHala.SelectedValue, DDLRzad.SelectedValue)
            ''        DDLRegal.SelectedValue = ob_mag.adres.Substring(3, 2).ToString
            ''        DDLStrefaMagazyn.SelectedValue = ob_mag.strefa.ToString
            ''    End If
            ''Next

            ''Session("mm_obiekt") = ob_list_mag
            Session("mm_skrot") = skrot
        End If

        AktualizujGridViewAdresy()
    End Sub

    Protected Sub GridViewAdresy_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewAdresy.SelectedIndexChanged
        ''If GridViewAdresy.Rows.Count > 0 Then
        ''    For Each row As GridViewRow In GridViewTowary.Rows
        ''        Dim cBox As CheckBox = row.FindControl("CBKodSelect")
        ''        cBox.Checked = False
        ''    Next
        ''End If

        ''Session.Remove("mm_obiekt")

        Dim cb As CheckBox = GridViewAdresy.SelectedRow.FindControl("CBKodSelect")
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False

            Dim ob_list_mag As New List(Of ObiektMagazynowy)
            ob_list_mag = Session("mm_obiekt")
            Dim ob_mag As New ObiektMagazynowy(GridViewAdresy.SelectedRow.Cells(2).Text.ToString, GridViewAdresy.SelectedRow.Cells(3).Text.ToString, GridViewAdresy.SelectedRow.Cells(7).Text.ToString, GridViewAdresy.SelectedRow.Cells(8).Text.ToString)
            For i = ob_list_mag.Count - 1 To 0 Step -1
                Dim ob_temp As ObiektMagazynowy = ob_list_mag(i)
                If ob_temp.skrot = ob_mag.skrot And ob_temp.adres = ob_mag.adres And ob_temp.strefa = ob_mag.strefa Then
                    ob_list_mag.RemoveAt(i)
                End If
            Next

            Session("mm_obiekt") = ob_list_mag
        Else
            cb.Checked = True
            Dim ob_list_mag As New List(Of ObiektMagazynowy)
            Dim skrot As String = GridViewAdresy.SelectedRow.Cells(2).Text.ToString()
            Dim mm_skrot As String = ""
            For Each row As GridViewRow In GridViewAdresy.Rows
                Dim cb_row As CheckBox = row.FindControl("CBKodSelect")
                If cb_row IsNot Nothing And cb_row.Checked Then
                    Dim ob_mag As New ObiektMagazynowy(row.Cells(2).Text.ToString, row.Cells(3).Text.ToString, row.Cells(7).Text.ToString, row.Cells(8).Text.ToString)
                    ob_list_mag.Add(ob_mag)
                    mm_skrot &= row.Cells(2).Text.ToString & ";"

                    DDLHala.SelectedValue = ob_mag.adres.Substring(0, 1).ToString
                    RefreshDDLRzad(DDLRzad, DDLHala.SelectedValue)
                    DDLRzad.SelectedValue = ob_mag.adres.Substring(1, 2).ToString
                    RefreshDDLRegal(DDLRegal, DDLHala.SelectedValue, DDLRzad.SelectedValue)
                    DDLRegal.SelectedValue = ob_mag.adres.Substring(3, 2).ToString
                    ''DDLStrefaMagazyn.SelectedValue = ob_mag.strefa.ToString
                    DDLStrefaMagazyn.SelectedValue = "Z"
                End If
            Next

            Session("mm_obiekt") = ob_list_mag
            ''Session("mm_skrot") = skrot
        End If

        ''AktualizujGridViewAdresy()
    End Sub


    Public Sub AktualizujGridViewAdresy()
        If GridViewTowary.Rows.Count > 0 Then
            Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                conn.Open()

                GridViewAdresy.DataSource = Nothing
                Dim indeksy_dtable As New DataTable
                Dim polaDataTable As String() = {"SKROT", "ADRES", "HALA", "RZAD", "REGAL", "STREFA", "IS_ACTIVE", "AUTODATA", "LOGIN"}
                For Each pole In polaDataTable
                    indeksy_dtable.Columns.Add(New DataColumn(pole, Type.GetType("System.String")))
                Next

                If Session("mm_skrot") IsNot Nothing Then
                    Dim sqlexp As String = "select w.skrot,w.adres,w.hala,w.rzad,w.regal,w.strefa,w.is_active,TO_CHAR(w.autodata, 'YYYY/MM/DD HH24:MI:SS') autodata, w.login from (" _
                     & " select skrot, adres,substr(adres,0,1) hala, substr(adres,2,2) rzad, substr(adres,4,2) regal, strefa, is_active, autodata, login from dp_swm_mia_towary where skrot like '" & Session("mm_skrot") & "' and strefa in('Z','N','P')" _
                     & ") w order by w.is_active desc, w.autodata desc"

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
                    GridViewAdresy.DataSource = indeksy_dtable
                End If

                GridViewAdresy.DataBind()
                conn.Close()
            End Using
        End If
    End Sub

    Public Sub AktualizujGridViewIndeksy()
        If GridViewTowary.Rows.Count > 0 Then
            Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                conn.Open()

                GridViewIndeksy.DataSource = Nothing
                Dim indeksy_dtable As New DataTable
                Dim polaDataTable As String() = {"SKROT", "ADRES", "HALA", "RZAD", "REGAL", "STREFA", "IS_ACTIVE", "AUTODATA", "LOGIN"}
                For Each pole In polaDataTable
                    indeksy_dtable.Columns.Add(New DataColumn(pole, Type.GetType("System.String")))
                Next

                If Session("mm_skrot") IsNot Nothing Then
                    Dim sqlexp As String = "select w.skrot,w.adres,w.hala,w.rzad,w.regal,w.strefa,w.is_active,TO_CHAR(w.autodata, 'YYYY/MM/DD HH24:MI:SS') autodata, w.login from (" _
                     & " select skrot, adres,substr(adres,0,1) hala, substr(adres,2,2) rzad, substr(adres,4,2) regal, strefa, is_active, autodata, login from dp_swm_mia_towary where skrot like '" & Session("mm_skrot") & "' and strefa in('N','P')" _
                     & ") w order by w.is_active desc, w.autodata desc"

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
                    GridViewIndeksy.DataSource = indeksy_dtable
                End If

                GridViewIndeksy.DataBind()
                conn.Close()
            End Using
        End If
    End Sub

    Protected Sub TBFiltrowanie_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBFiltrowanie.TextChanged
        Dim filter_data As String = TBFiltrowanie.Text
        If filter_data <> "" Then
            LadujDaneGridViewMagazyn(filter_data)
        Else
            LadujDaneGridViewMagazyn("")
        End If
    End Sub

    Protected Sub BTowarWyszukaj_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarWyszukaj.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If TBFiltrowanie.Text.ToString <> "" Then
                LadujDaneGridViewMagazyn(TBFiltrowanie.Text.ToString)
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub GridViewIndeksy_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewIndeksy.SelectedIndexChanged
        If GridViewIndeksy.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewIndeksy.Rows
                Dim cBox As CheckBox = row.FindControl("CBKodSelect")
                cBox.Checked = False
            Next
        End If

        Session.Remove("adr_zlozenie")

        Dim cb As CheckBox = GridViewIndeksy.SelectedRow.FindControl("CBKodSelect")
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
        Else
            cb.Checked = True
            Session("adr_zlozenie") = GridViewIndeksy.SelectedRow.Cells(3).Text.ToString
        End If
    End Sub
End Class