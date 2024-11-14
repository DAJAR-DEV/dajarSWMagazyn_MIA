Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.Drawing

Partial Public Class address_reczne
    Inherits System.Web.UI.Page
    Dim sqlexp As String = ""
    Dim result As Boolean = False
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder

    Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
        For i As Integer = 0 To GridViewAdresacja.Rows.Count - 1
            ClientScript.RegisterForEventValidation(GridViewAdresacja.UniqueID, "Select$" & i)
        Next

        MyBase.Render(writer)
    End Sub

    Protected Sub DDLHala_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles DDLHala.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim hala As String = DDLHala.SelectedValue.ToString

            If hala <> "" Then
                RefreshDDLRzad(DDLRzad, hala)
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub DDLRzad_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles DDLRzad.SelectedIndexChanged
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

    Protected Sub DDLRegal_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles DDLRegal.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim hala As String = DDLHala.SelectedValue.ToString
            Dim rzad As String = DDLRzad.SelectedValue.ToString
            Dim regal As String = DDLRegal.SelectedValue.ToString

            If hala <> "" And rzad <> "" Then
                RefreshDDLPolka(DDLPolka, hala, rzad, regal)
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
            ddlObiekt.Items.Insert(0, New ListItem("--", "--"))

            ddlObiekt.SelectedValue = "A"
            ddlObiekt.SelectedValue = "--"
            ''ddlObiekt.Items.Insert(0, New ListItem("--", "--"))
            ddlObiekt.DataBind()
            RefreshDDLRzad(DDLRzad, DDLHala.SelectedValue.ToString)
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLRzad(ByRef ddlObiekt As DropDownList, ByVal hala As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()

            If hala <> "--" Then
                sqlexp = "select distinct rzad from dp_swm_mia_adresy where hala='" & hala &
                         "' and is_active='1' order by rzad"
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
                ddlObiekt.Items.Insert(0, New ListItem("--", "--"))
                ddlObiekt.SelectedValue =
                    dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(
                        "select min(distinct rzad) rzad from dp_swm_mia_adresy where hala='" & hala &
                        "' and is_active='1' order by rzad", conn)
            Else
                Dim znak As String = ""
                For i = 1 To 99
                    znak = i.ToString().PadLeft(2, "0")
                    ddlObiekt.Items.Add(New ListItem("A" & i.ToString, "A" & i.ToString))
                    ddlObiekt.Items.Add(New ListItem(znak, znak))
                Next

                ddlObiekt.Items.Insert(0, New ListItem("--", "--"))
                ddlObiekt.SelectedValue = "01"
                ddlObiekt.SelectedValue = "--"
            End If

            ''RefreshDDLRegal(DDLRegal, DDLHala.SelectedValue.ToString, DDLRzad.SelectedValue.ToString)
            ''##2024.02.01 / modyfikacja aktywnych list adresowych
            RefreshDDLRegal(DDLRegal, hala, DDLRzad.SelectedValue.ToString)
            ''ddlObiekt.Items.Insert(0, New ListItem("--", "--"))
            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLRegal(ByRef ddlObiekt As DropDownList, ByVal hala As String, ByVal rzad As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()

            If hala <> "--" And rzad <> "--" Then
                sqlexp = "select distinct regal from dp_swm_mia_adresy where hala='" & hala & "' and rzad='" & rzad &
                         "' and is_active='1' order by regal"
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
                ddlObiekt.Items.Insert(0, New ListItem("--", "--"))
                ddlObiekt.SelectedValue =
                    dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(
                        "select min(distinct regal) regal from dp_swm_mia_adresy where hala='" & hala & "' and rzad='" &
                        rzad & "' and is_active='1' order by regal", conn)
            Else
                Dim znak As String = ""
                For i = 1 To 99
                    znak = i.ToString().PadLeft(2, "0")
                    ddlObiekt.Items.Add(New ListItem(znak, znak))
                Next
                ddlObiekt.Items.Insert(0, New ListItem("--", "--"))
                ddlObiekt.SelectedValue = "01"
                ddlObiekt.SelectedValue = "--"
            End If

            ''##2024.02.01 / modyfikacja list rozwijanych adresy
            ''RefreshDDLPolka(DDLPolka, DDLHala.SelectedValue.ToString, DDLRzad.SelectedValue.ToString, DDLRegal.SelectedValue.ToString)
            RefreshDDLPolka(DDLPolka, hala, rzad, DDLRegal.SelectedValue.ToString)
            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub


    Protected Sub RefreshDDLPolka(ByRef ddlObiekt As DropDownList, ByVal hala As String, ByVal rzad As String,
                                  ByVal regal As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()

            If hala <> "--" And rzad <> "--" And regal <> "--" Then
                sqlexp = "select distinct polka from dp_swm_mia_adresy where hala='" & hala & "' and rzad='" & rzad &
                         "' and regal='" & regal & "' and is_active='1' order by polka"
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

                ddlObiekt.Items.Insert(0, New ListItem("--", "--"))
                ddlObiekt.SelectedValue =
                    dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(
                        "select min(distinct polka) polka from dp_swm_mia_adresy where hala='" & hala & "' and rzad='" &
                        rzad & "' and regal='" & regal & "' and is_active='1' order by polka", conn)
            Else
                Dim znak As String = ""
                For i = 0 To 99
                    znak = i.ToString().PadLeft(2, "0")
                    ddlObiekt.Items.Add(New ListItem(znak, znak))
                Next
                ddlObiekt.Items.Insert(0, New ListItem("--", "--"))
                ddlObiekt.SelectedValue = "00"
                ddlObiekt.SelectedValue = "--"
            End If

            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub

    Protected Sub GridViewAdresacja_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) _
        Handles GridViewAdresacja.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" &
                                   dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewAdresacja.PageIndex = e.NewPageIndex
        GridViewAdresacja.DataBind()
        LoadHistoriaSkrotu(TBSkrot.Text.ToString)
    End Sub

    Protected Sub DDLOperator_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles DDLOperator.SelectedIndexChanged
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
        Dim session_id As String = "contentKomunikat_" &
                                   dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session("contentKomunikat") = Session(session_id)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim session_id As String = "contentKomunikat_" &
                                   dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Dim przerwijLadowanie As Boolean = False

        BUsunTowar.Attributes.Add("onclick",
                                  "javascrip:if(confirm ('Czy na pewno chcesz usunac wybrany towar z pozycji magazynowej\n') == false) return false")

        If Session("mylogin") = Nothing And Session("myhash") = Nothing Then
            Session.Abandon()
            Response.Redirect("index.aspx")
        ElseIf Session("mytyp_oper") <> "M" And Session("mytyp_oper") <> "W" Then
            Session(session_id) =
                "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
            Session(session_id) += "<br />Zaloguj sie na opowiedniego operatora - typ MAGAZYN/WOZEK<br />"
            Session(session_id) += "</div>"
            PanelMagazyn.Visible = False
            przerwijLadowanie = True
        Else
            Session.Remove(session_id)
        End If

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session("contentMagazyn") =
                dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(
                    "SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN = '" & Session("mylogin") & "'", conn)
            conn.Close()
        End Using


        If Not Page.IsPostBack Then
            RefreshDDLHala(DDLHala)
            ''RefreshDDLRzad(DDLRzad)
            ''RefreshDDLRegal(DDLRegal)
            dajarSWMagazyn_MIA.MyFunction.RefreshDDLOperator(DDLOperator, Session)

            If przerwijLadowanie = False Then
                ''LadujDaneGridViewMagazyn("")
            End If
        End If

        ''If TBSkrot.Text <> "" Then
        ''    LadowanieKartyProduktowej()
        ''End If

        Session("contentKomunikat") = Session(session_id)
    End Sub

    Public Sub LadowanieKartyProduktowej()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            TBIndeks.Text = ""
            TBNazwa.Text = ""
            TBEan.Text = ""
            TBPrefix.Text = ""
            TBStan700.Text = ""

            Dim skrot As String = TBSkrot.Text.ToString
            If skrot.Length >= 5 Then
                If TBSkrot.Text.Length >= 5 And TBSkrot.Text.Length <= 6 Then
                    ''sqlexp = "select indeks,nazdop,kod_tow,skt2_0,DP_SWM.dp_swm_mia_LIP_GET_STAN_INDEKS(indeks) stan,nazpot from ht_rejna where nazpot='" & skrot & "' and is_deleted='N'"
                    sqlexp =
                        "select indeks,nazdop,kod_tow,skt2_0,desql_japu_nwa.fsql_japu_wolne_MAG(indeks,'700') stan_700,nazpot from ht_rejna where nazpot='" &
                        skrot & "' and is_deleted='N'"
                ElseIf TBSkrot.Text.Length >= 7 Then
                    ''sqlexp = "select indeks,nazdop,kod_tow,skt2_0,DP_SWM.dp_swm_mia_LIP_GET_STAN_INDEKS(indeks) stan,nazpot from ht_rejna where kod_tow='" & skrot & "' and is_deleted='N'"
                    sqlexp =
                        "select indeks,nazdop,kod_tow,skt2_0,desql_japu_nwa.fsql_japu_wolne_MAG(indeks,'700') stan_700,nazpot from ht_rejna where kod_tow='" &
                        skrot & "' and is_deleted='N'"
                End If

                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                dr = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        TBIndeks.Text = dr.Item(0).ToString
                        TBNazwa.Text = dr.Item(1).ToString
                        TBEan.Text = dr.Item(2).ToString
                        TBPrefix.Text = dr.Item(3).ToString
                        TBStan700.Text = dr.Item(4).ToString
                        TBSkrot.Text = dr.Item(5).ToString

                        LoadHistoriaSkrotu(TBSkrot.Text.ToString)
                    End While

                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()
            ElseIf TBAdresacja.Text = "" Then
                DDLHala.SelectedValue = "--"
                DDLRzad.SelectedValue = "--"
                DDLRegal.SelectedValue = "--"
                DDLPolka.SelectedValue = "--"
                DDLAdresacjaTowary.DataSource = Nothing
                DDLAdresacjaTowary.DataBind()
                GridViewAdresacja.DataSource = Nothing
                GridViewAdresacja.DataBind()
            End If
            conn.Close()

        End Using
    End Sub

    Protected Sub TBSkrot_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBSkrot.TextChanged
        LadowanieKartyProduktowej()
        If TBSkrot.Text = "" And TBAdresacja.Text <> "" Then
            LadowanieKartyAdresowej()
        End If
    End Sub

    Protected Sub RefreshDDLAdresacjaTowary(ByRef ddlObiekt As DropDownList, ByVal adresacja As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()

            sqlexp = "select distinct w.skrot, w.nazdop from (" _
                     &
                     " select dt.skrot, dt.adres, substr(dt.adres,0,1) hala, substr(dt.adres,2,2) rzad, substr(dt.adres,4,2) regal, dt.strefa, dt.is_active, dt.autodata, dt.login, r.nazdop from dp_swm_mia_towary dt, ht_rejna r where dt.adres like '" &
                     adresacja & "%' and dt.is_active='1' and dt.skrot = r.nazpot and r.is_deleted='N')" _
                     & " w order by w.skrot asc"

            Dim cmd As New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            dr = cmd.ExecuteReader()
            Try
                While dr.Read()
                    ddlObiekt.Items.Add(New ListItem("[" & dr.Item(0).ToString & "] " & dr.Item(1).ToString,
                                                     dr.Item(0).ToString))
                End While

            Catch ex As System.ArgumentOutOfRangeException
                Console.WriteLine(ex)
            End Try
            dr.Close()
            cmd.Dispose()

            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub

    Protected Sub DDLAdresacjaTowary_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles DDLAdresacjaTowary.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim skrot_adres As String = DDLAdresacjaTowary.SelectedValue.ToString

            If skrot_adres <> "" Then
                TBSkrot.Text = skrot_adres
                sqlexp = "select indeks,nazdop,kod_tow,skt2_0 from ht_rejna where nazpot='" & skrot_adres &
                         "' and is_deleted='N'"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                dr = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        TBIndeks.Text = dr.Item(0).ToString
                        TBNazwa.Text = dr.Item(1).ToString
                        TBEan.Text = dr.Item(2).ToString
                        TBPrefix.Text = dr.Item(3).ToString
                    End While

                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()
            End If
            conn.Close()
        End Using
    End Sub

    Public Sub LadowanieKartyAdresowej()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If TBAdresacja.Text.Length = 7 Then
                sqlexp = "select count(*) from dp_swm_mia_adresy where adres like '" & TBAdresacja.Text.ToString &
                         "%' and is_active='1'"
                Dim czyAdresacja As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If czyAdresacja <> "0" Then
                    RefreshDDLAdresacjaTowary(DDLAdresacjaTowary, TBAdresacja.Text.ToString)
                    Dim adresacja As String = TBAdresacja.Text.ToString
                    Dim nr_hala As String = adresacja.Substring(0, 1).ToString
                    RefreshDDLRzad(DDLRzad, nr_hala)
                    DDLHala.SelectedValue = nr_hala
                    If TBAdresacja.Text.Length >= 3 Then
                        Dim nr_rzad As String = adresacja.Substring(1, 2).ToString
                        RefreshDDLRegal(DDLRegal, DDLHala.SelectedValue, nr_rzad)
                        DDLRzad.SelectedValue = nr_rzad
                        If TBAdresacja.Text.Length >= 5 Then
                            Dim nr_regal As String = adresacja.Substring(3, 2).ToString
                            DDLRegal.SelectedValue = nr_regal
                            If TBAdresacja.Text.Length >= 7 Then
                                Dim nr_polka As String = adresacja.Substring(5, 2).ToString
                                RefreshDDLPolka(DDLPolka, DDLHala.SelectedValue, DDLRzad.SelectedValue,
                                                DDLRegal.SelectedValue)
                                DDLPolka.SelectedValue = nr_polka
                            End If
                        End If
                    End If

                    DDLStrefaMagazyn.SelectedValue = "Z"
                End If
                ''##2024.02.01 / ladowanie aktulanej listy wszystkich towarow z adresu
                LoadHistoriaSkrotu_ADRESACJA(TBAdresacja.Text.ToString)
            Else
                DDLHala.SelectedValue = "--"
                DDLRzad.SelectedValue = "--"
                DDLRegal.SelectedValue = "--"
                DDLPolka.SelectedValue = "--"
                DDLAdresacjaTowary.DataSource = Nothing
                DDLAdresacjaTowary.DataBind()
                GridViewAdresacja.DataSource = Nothing
                GridViewAdresacja.DataBind()
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub TBAdresacja_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBAdresacja.TextChanged
        LadowanieKartyAdresowej()
    End Sub

    Protected Sub BTowarWyszukaj_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarWyszukaj.Click
        Dim session_id As String = "contentKomunikat_" &
                                   dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If TBSkrot.Text.ToString <> "" And TBIndeks.Text.ToString <> "" Then
                LoadHistoriaSkrotu(TBSkrot.Text.ToString)
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BDodajTowar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BDodajTowar.Click
        Dim session_id As String = "contentKomunikat_" &
                                   dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If TBSkrot.Text.ToString <> "" And TBIndeks.Text.ToString <> "" Then
                Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                Dim skrot As String = TBSkrot.Text.ToString
                Dim adres As String = DDLHala.SelectedValue.ToString & DDLRzad.SelectedValue.ToString &
                                      DDLRegal.SelectedValue.ToString & DDLPolka.SelectedValue.ToString
                Dim strefa As String = DDLStrefaMagazyn.SelectedValue.ToString

                sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_towary WHERE adres='" & adres & "' and skrot='" & skrot &
                         "' and strefa='" & strefa & "'"
                Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If czy_istnieje = "0" Then
                    sqlexp = "insert into dp_swm_mia_towary values('" & skrot & "','" & adres & "','" & strefa & "',1,'" &
                             Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp &
                             "', 'RR/MM/DD HH24:MI:SS'))"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    dajarSWMagazyn_MIA.MyFunction.SetHistoriaTowary(skrot, adres, strefa, "1", "OP_RE", Session("mylogin"),
                                                                Session("myhash"))
                    LoadHistoriaSkrotu(skrot)
                Else
                    sqlexp = "update dp_swm_mia_towary set is_active='1',login='" & Session("mylogin") & "',hash='" &
                             Session("myhash") & "',autodata=TO_TIMESTAMP('" & timestamp &
                             "', 'RR/MM/DD HH24:MI:SS') where adres='" & adres & "' and skrot='" & skrot &
                             "' and strefa='" & strefa & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    ''Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    ''Session(session_id) += "<br />Wprowadzy skrot jest juz wprowadzony na danym adresie/strefie zamagazynowania!<br />"
                    ''Session(session_id) += "</div>"
                End If

                ''##2024.02.02 / ustawienie domyslnego adresu po dodaniu produktu
                TBAdresacja.Text = adres
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BUsunTowar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BUsunTowar.Click
        Dim session_id As String = "contentKomunikat_" &
                                   dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If TBSkrot.Text.ToString <> "" And TBIndeks.Text.ToString <> "" Then
                If Session("mm_adres") IsNot Nothing Then
                    Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                    Dim skrot As String = TBSkrot.Text.ToString
                    Dim adres As String = Session("mm_adres").ToString
                    Dim strefa As String = Session("mm_strefa").ToString

                    sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_towary WHERE adres='" & adres & "' and skrot='" & skrot &
                             "' and strefa='" & strefa & "'"
                    Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If czy_istnieje = "1" Then
                        sqlexp = "delete from dp_swm_mia_towary where adres='" & adres & "' and skrot='" & skrot &
                                 "' and strefa='" & strefa & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        LoadHistoriaSkrotu(skrot)
                    Else
                        Session(session_id) =
                            "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) +=
                            "<br />Wprowadzy skrot nie jest wprowadzony na danym adresie zamagazynowania!<br />"
                        Session(session_id) += "</div>"
                    End If
                End If
            ElseIf TBAdresacja.Text.Length = 7 Then
                Dim gridviewadresacja_odswiezanie As Boolean = False
                If GridViewAdresacja.Rows.Count > 0 Then
                    For Each row As GridViewRow In GridViewAdresacja.Rows
                        Dim cBox As CheckBox = row.FindControl("CBKodSelect")
                        If cBox.Checked = True Then
                            Dim skrot As String = row.Cells(2).Text.ToString
                            Dim adres As String = row.Cells(3).Text.ToString
                            Dim login As String = row.Cells(10).Text.ToString
                            Dim strefa As String = row.Cells(7).Text.ToString

                            sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_towary WHERE adres='" & adres & "' and skrot='" &
                                     skrot & "' and strefa='" & strefa & "' and login='" & login & "'"
                            Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                            If czy_istnieje = "1" Then
                                sqlexp = "delete from dp_swm_mia_towary where adres='" & adres & "' and skrot='" & skrot &
                                         "' and strefa='" & strefa & "' and login='" & login & "'"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                gridviewadresacja_odswiezanie = True
                                ''LoadHistoriaSkrotu(skrot)
                            End If

                        End If
                    Next

                    If gridviewadresacja_odswiezanie = True Then
                        LoadHistoriaSkrotu_ADRESACJA(TBAdresacja.Text.ToString)
                    End If

                End If

            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub GridViewAdresacja_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) _
        Handles GridViewAdresacja.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then

            Dim rowIndex As Integer = e.Row.RowIndex
            e.Row.Attributes("onclick") = Page.ClientScript.GetPostBackClientHyperlink(GridViewAdresacja, "Select$" &
                                                                                                          rowIndex)
            e.Row.Style("cursor") = "pointer"

            Dim cb As CheckBox = e.Row.FindControl("CBKodSelect")

            If cb IsNot Nothing And cb.Checked Then
                e.Row.BackColor = GridViewAdresacja.SelectedRowStyle.BackColor
            Else
                Dim strefa As String = e.Row.Cells(3).Text.ToString
                If strefa = "Z" Then : e.Row.BackColor = LTowarZbieranie.BackColor
                ElseIf strefa = "N" Then : e.Row.BackColor = LTowarNiepelna.BackColor
                ElseIf strefa = "P" Then : e.Row.BackColor = LTowarPaleta.BackColor
                Else : e.Row.BackColor = LTowarPusty.BackColor
                End If

            End If
        End If
    End Sub

    Public Sub LoadHistoriaSkrotu(ByVal skrot As String)
        If skrot <> "" Then
            GridViewAdresacja.DataSource = Nothing
            Dim indeksy_dtable As New DataTable
            Dim polaDataTable As String() =
                    {"SKROT", "ADRES", "STREFA", "AUTODATA", "LOGIN"}
            For Each pole In polaDataTable
                indeksy_dtable.Columns.Add(New DataColumn(pole, Type.GetType("System.String")))
            Next

            sqlexp = "select w.skrot,w.adres,w.strefa,TO_CHAR(w.autodata, 'YYYY/MM/DD HH24:MI:SS') autodata, w.login from (" _
                     &
                     " select dt.skrot, dt.adres, substr(dt.adres,0,1) hala, substr(dt.adres,2,2) rzad, substr(dt.adres,4,2) regal, dt.strefa, dt.is_active, dt.autodata, dt.login from dp_swm_mia_towary dt where dt.skrot = '" &
                     skrot & "')" _
                     & " w order by w.is_active desc, decode(w.strefa, 'Z', 1, 'N', 2, 'P', 3), w.autodata desc"

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

            GridViewAdresacja.DataSource = indeksy_dtable
            GridViewAdresacja.DataBind()
        Else
            GridViewAdresacja.DataSource = Nothing
            GridViewAdresacja.DataBind()
        End If
    End Sub

    Public Sub LoadHistoriaSkrotu_ADRESACJA(ByVal adres As String)
        If adres <> "" Then
            GridViewAdresacja.DataSource = Nothing
            Dim indeksy_dtable As New DataTable
            Dim polaDataTable As String() =
                    {"SKROT", "ADRES", "STREFA", "AUTODATA", "LOGIN"}
            For Each pole In polaDataTable
                indeksy_dtable.Columns.Add(New DataColumn(pole, Type.GetType("System.String")))
            Next

            sqlexp = "select w.skrot,w.adres,w.strefa,TO_CHAR(w.autodata, 'YYYY/MM/DD HH24:MI:SS') autodata, w.login from (" _
                     &
                     " select dt.skrot, dt.adres, substr(dt.adres,0,1) hala, substr(dt.adres,2,2) rzad, substr(dt.adres,4,2) regal, dt.strefa, dt.is_active, dt.autodata, dt.login from dp_swm_mia_towary dt where dt.adres = '" &
                     adres & "' and dt.is_active='1')" _
                     & " w order by w.is_active desc, decode(w.strefa, 'Z', 1, 'N', 2, 'P', 3), w.autodata desc"

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

            GridViewAdresacja.DataSource = indeksy_dtable
            GridViewAdresacja.DataBind()
        Else
            GridViewAdresacja.DataSource = Nothing
            GridViewAdresacja.DataBind()
        End If
    End Sub

    Protected Sub GridViewAdresacja_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) _
        Handles GridViewAdresacja.RowCommand
        If e.CommandName = "Select" Then
            Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
            GridViewAdresacja.SelectedIndex = rowIndex

            GridViewIndeksy_SelectedIndexChanged(sender, EventArgs.Empty)
        End If
    End Sub

    Protected Sub GridViewIndeksy_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) _
        Handles GridViewAdresacja.SelectedIndexChanged
        If GridViewAdresacja.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewAdresacja.Rows
                Dim cBox As CheckBox = row.FindControl("CBKodSelect")
                cBox.Checked = False
            Next
        End If

        Session.Remove("mm_adres")
        Session.Remove("mm_strefa")

        ''If GridViewAdresacja.SelectedRow.Nothing Then
        Dim cb As CheckBox = GridViewAdresacja.SelectedRow.FindControl("CBKodSelect")
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
        Else
            cb.Checked = True
            Session("mm_adres") = GridViewAdresacja.SelectedRow.Cells(2).Text.ToString
            Session("mm_strefa") = GridViewAdresacja.SelectedRow.Cells(3).Text.ToString
            ''##2024.02.01 / przeladowanie listy zgodnie z zaznczonym sku w historii
            Dim sku As String = GridViewAdresacja.SelectedRow.Cells(1).Text.ToString
            Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                conn.Open()
                sqlexp = "select indeks,nazdop,kod_tow,skt2_0 from ht_rejna where nazpot='" & sku &
                         "' and is_deleted='N'"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        TBIndeks.Text = dr.Item(0).ToString()
                        TBNazwa.Text = dr.Item(1).ToString()
                        TBEan.Text = dr.Item(2).ToString()
                        TBPrefix.Text = dr.Item(3).ToString()
                    End While
                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                conn.Close()
            End Using
            DDLAdresacjaTowary.SelectedValue = sku
            TBSkrot.Text = sku
            Aktualizacja_Adres_Historia(Session("mm_adres"), Session("mm_strefa"))
        End If
        ''End If

        For Each row As GridViewRow in GridViewAdresacja.Rows
            Dim strefa As String = row.Cells(3).Text.ToString
            Select Case strefa
                Case "Z"
                    row.BackColor = LTowarZbieranie.BackColor
                Case "N"
                    row.BackColor = LTowarNiepelna.BackColor
                Case "P"
                    row.BackColor = LTowarPaleta.BackColor
                Case Else
                    row.BackColor = LTowarPusty.BackColor
            End Select
            Dim checkBox As CheckBox = row.FindControl("CBKodSelect")
            If checkBox.Checked Then
                row.BackColor = GridViewAdresacja.SelectedRowStyle.BackColor
            End If
        Next
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