Imports dajarSWMagazyn_MIA.HashMd5
Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO
Imports dajarSWMagazyn_MIA.pl.com.dhl24
Imports dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop
Imports dajarSWMagazyn_MIA.dhlDEAPI
Imports dajarSWMagazyn_MIA.dhl_parcel_de_package
Imports dajarSWMagazyn_MIA.ups_de_package
Imports System.Net
Imports System.Globalization
Imports System.Reflection
Imports Neodynamic.SDK.Web

Partial Public Class a_labelEdit
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

    Protected Sub page_loaded() Handles Me.LoadComplete
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)

        Try
            BZapiszPaczke.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zapisac paczke " & LPaczkaID.Text.ToString & "\n') == false) return false")
            BAnulujPaczek.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować paczke " & LPaczkaID.Text.ToString & "\n') == false) return false")
            BAnulowaniePaczkomat.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować paczkomat dla paczki " & LPaczkaID.Text.ToString & "\n') == false) return false")

            BDodajZamowienie.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz wprowadzic zamowienie " & LZamowienieId.Text.ToString & "\n') == false) return false")
            BAnulujZamowienie.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować zamowienie " & LZamowienieId.Text.ToString & "\n') == false) return false")

            BWprowadzList_DHL.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno wprowadzic recznie nowy numer list przewozowy DHL\n') == false) return false")

            ''BGenerujEtykiete_DHL.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno utworzyc nowy list przewozowy dhl\n') == false) return false")
            BAnuluj_DHL.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować paczke dhl " & LNr_list_przewozowy.Text.ToString & "\n') == false) return false")

            ''BWprowadzList_GEIS.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno wprowadzic recznie nowy numer list przewozowy geis\n') == false) return false")

            ''BGenerujEtykiete_GEIS.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno utworzyc nowy list przewozowy geis\n') == false) return false")
            BRaportPickup_GEIS.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno utworzyc protokol zdawczo-odbiorczy geis\n') == false) return false")
            BAnuluj_GEIS.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować paczke geis " & LNr_list_przewozowy.Text.ToString & "\n') == false) return false")

            ''BWprowadzList_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno wprowadzic recznie nowy numer list przewozowy INPOST\n') == false) return false")

            BGenerujEtykiete_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno utworzyc nowa oferte przewozowa INPOST\n') == false) return false")
            BGenerujOferte_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno oplacic oferte INPOST\n') == false) return false")
            BAnuluj_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować paczke INPOST " & LNr_list_przewozowy.Text.ToString & "\n') == false) return false")
            BZerowanie_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno wyzerowac ustawienia paczek INPOST " & LNr_list_przewozowy.Text.ToString & "\n') == false) return false")

            Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                conn.Open()
                If Session("shipment_id") Is Nothing Then
                    Dim shipment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' AND SHIPMENT_ID LIKE 'SH%'", conn)
                    If shipment_id <> "" Then
                        Session("shipment_id") = shipment_id
                        LNr_list_przewozowy.Text = Session("shipment_id")
                    Else
                        LNr_list_przewozowy.Text = ""
                    End If
                End If

                sqlexp = "select kod_kontr from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & Session("kod_dyspo") & "')"
                Dim kod_kontr As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                sqlexp = "select kod_odb from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & Session("kod_dyspo") & "')"
                Dim kod_odb As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                sqlexp = "select kod_mie from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & Session("kod_dyspo") & "')"
                Dim kod_mie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                sqlexp = "select kod_kraju,(naz||naz_1),adr,kod_p,miejsc from ht_kontm where kod_kontr = " & kod_kontr & " and kod_odb = '" & kod_odb & "' and kod_mie='" & kod_mie & "' and is_deleted='N'"
                Dim cmd As OracleCommand = New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        ''TB_SR_Country.Text = dr.Item(0).ToString
                        DDL_SR_COUNTRY.SelectedValue = dr.Item(0).ToString
                        TB_Country.Text = dhlDESoapRequest.dhlGetCountryLanguageName(DDL_SR_COUNTRY.SelectedValue.ToString)
                        TB_SR_NAME.Text = dr.Item(1).ToString
                        TB_PC_PERSON_NAME.Text = dr.Item(1).ToString
                        TB_SR_STREET.Text = dr.Item(2).ToString

                        TB_SR_POSTAL_CODE.Text = dr.Item(3).ToString.Replace("-", "")
                        TB_SR_CITY.Text = dr.Item(4).ToString
                    End While

                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()

                Dim f_id As String = DDLFirma.SelectedValue.ToString
                If f_id <> "-1" And f_id = "DHL_DE" Then
                    ''DDL_SR_COUNTRY.SelectedValue = "DE"
                Else
                    ''TB_SR_Country.Text = "PL"
                    DDL_SR_COUNTRY.SelectedValue = "PL"
                End If

                If Session("dhl_opcje") = "1" Then
                    CB_SR_IS_PACKSTATION.Enabled = True
                    CB_SR_IS_POSTFILIALE.Enabled = True
                End If

                sqlexp = "select e_mail,telefon from ht_konto where kod_kontr = " & kod_kontr & ""
                cmd = New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                dr = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        TB_PC_EMAIL_ADD.Text = dr.Item(0).ToString.Trim
                        TB_PC_PHONE_NUM.Text = dr.Item(1).ToString.Trim.Replace(" ", "").Replace("-", "")
                    End While

                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()


                ValidateFormField(DDL_SR_COUNTRY.SelectedValue, "SR_Country", "dp_swm_mia_paczka")
                TB_Country.Text = dhlDESoapRequest.dhlGetCountryLanguageName(DDL_SR_COUNTRY.SelectedValue.ToString)
                ValidateFormField(TB_SR_POST_NUM.Text, "SR_POST_NUM", "dp_swm_mia_paczka")

                ''####2022.07.21 / aktualizacja rodzaju uslugi paczkomatu
                If TB_SR_POST_NUM.Text.ToString <> "" Then
                    If LNr_zamow_o.Text.ToString.StartsWith("SAL") Then
                        DDL_SERVICE_TYPE_INPOST.SelectedValue = "inpost_locker_allegro"
                    Else
                        DDL_SERVICE_TYPE_INPOST.SelectedValue = "inpost_locker_standard"
                    End If
                End If


                ValidateFormField(TB_SR_NAME.Text, "SR_Name", "dp_swm_mia_paczka")
                ValidateFormField(TB_SR_POSTAL_CODE.Text, "SR_POSTAL_CODE", "dp_swm_mia_paczka")
                ValidateFormField(TB_SR_CITY.Text, "SR_CITY", "dp_swm_mia_paczka")

                If CB_SR_IS_PACKSTATION.Checked = True Then
                    TB_SR_STREET.Text = "Packstation"
                ElseIf CB_SR_IS_POSTFILIALE.Checked = True Then
                    TB_SR_STREET.Text = "Postfiliale"
                Else
                    ValidateFormField(TB_SR_STREET.Text, "SR_STREET", "dp_swm_mia_paczka")

                End If

                ValidateFormField(TB_SR_HOUSE_NUM.Text, "SR_HOUSE_NUM", "dp_swm_mia_paczka")
                ValidateFormField(TB_SR_APART_NUM.Text, "SR_APART_NUM", "dp_swm_mia_paczka")
                ValidateFormField(TB_PC_PERSON_NAME.Text, "PC_PERSON_NAME", "dp_swm_mia_paczka")
                ValidateFormField(TB_PC_PHONE_NUM.Text, "PC_PHONE_NUM", "dp_swm_mia_paczka")
                ValidateFormField(TB_PC_EMAIL_ADD.Text, "PC_EMAIL_ADD", "dp_swm_mia_paczka")

                If Session("shipment_date") Is Nothing Then
                    ValidateFormField(TB_ST_SHIPMENT_DATE.Text, "ST_SHIPMENT_DATE", "dp_swm_mia_paczka")
                End If
                ValidateFormField(TB_ST_SHIPMENT_START.Text, "ST_SHIPMENT_START", "dp_swm_mia_paczka")
                ValidateFormField(TB_ST_SHIPMENT_END.Text, "ST_SHIPMENT_END", "dp_swm_mia_paczka")
                ValidateFormField(TB_B_BILL_ACC_NUM.Text, "B_BILL_ACC_NUM", "dp_swm_mia_paczka")

                ValidateFormField(DDL_B_SHIP_PAYMENT_TYPE.SelectedValue, "B_SHIP_PAYMENT_TYPE", "dp_swm_mia_paczka")
                ValidateFormField(DDL_B_PAYMENT_TYPE.Text, "B_PAYMENT_TYPE", "dp_swm_mia_paczka")

                If Session("firma_id") IsNot Nothing Then
                    If Session("firma_id").ToString.Contains("GEIS") Then
                        ValidateFormField(DDL_CONTENT_GEIS.SelectedValue, "CONTENT", "dp_swm_mia_paczka_base")
                        If Session("schemat_dyspo") = "DOMINUS" Then
                            If DDL_SR_COUNTRY.SelectedValue = "PL" Then
                                TB_COMMENT_F_GEIS.Text = "DO " & Session("kod_dyspo")
                            Else
                                TB_COMMENT_F_GEIS.Text = "DO " & Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                            End If
                        Else
                            If DDL_SR_COUNTRY.SelectedValue = "PL" Then
                                TB_COMMENT_F_GEIS.Text = Session("kod_dyspo")
                            Else
                                TB_COMMENT_F_GEIS.Text = Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                            End If
                        End If

                        ValidateFormField(TB_COMMENT_F_GEIS.Text, "COMMENT_F", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_DROP_OFF_GEIS.SelectedValue, "DROP_OFF", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_SERVICE_TYPE_GEIS.SelectedValue, "SERVICE_TYPE", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_LABEL_TYPE_GEIS.SelectedValue, "LABEL_TYPE", "dp_swm_mia_paczka_base")

                    ElseIf Session("firma_id").ToString.Contains("INPOST") Then
                        ValidateFormField(DDL_CONTENT_INPOST.SelectedValue, "CONTENT", "dp_swm_mia_paczka_base")

                        ''####2022.08.04 / aktualizacja pola login allegro w opisie zamowienia dla INPOST

                        sqlexp = "select external_username from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'"
                        Dim external_username As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                        If Session("schemat_dyspo") = "DOMINUS" Then
                            If DDL_SR_COUNTRY.SelectedValue = "PL" Then
                                TB_COMMENT_F_INPOST.Text = "DO " & Session("kod_dyspo") & " " & external_username
                            Else
                                TB_COMMENT_F_INPOST.Text = "DO " & Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                            End If
                        Else
                            If DDL_SR_COUNTRY.SelectedValue = "PL" Then
                                TB_COMMENT_F_INPOST.Text = Session("kod_dyspo") & " " & external_username
                            Else
                                TB_COMMENT_F_INPOST.Text = Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                            End If

                        End If

                        ''##2023.09.14 / wprowadzenie obslugi komentarza dla MSPL 
                        If LNr_zamow_o.Text.Contains("MSPL") Then
                            TB_COMMENT_F_INPOST.Text = Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                        End If


                        ''####2022.08.10 / aktualizacja pol [numer paczkomatu,imie,nazwisko] dla INPOST
                        ''##2023.11.09 / dodanie obslugi paczkopunktu dla INPOST
                        Dim opis_zamow As String = TBOpisZam.Text.ToString
                        If opis_zamow.Contains("PACZKOMAT") = True And TB_SR_POST_NUM.Text.ToString.Length = 0 Then

                            Dim wzorzecReguly As String = "PACZKOMAT\s[A-Z0-9]+"
                            Dim rx As Regex = New Regex(wzorzecReguly)
                            Dim wzorzecString As String = opis_zamow

                            Dim m As Match = rx.Match(wzorzecString)
                            If m.Success = True Then
                                TB_SR_POST_NUM.Text = m.Value.ToString.Replace("PACZKOMAT ", "")
                            End If
                        ElseIf opis_zamow.Contains("PACZKOPUNKT") = True And TB_SR_POST_NUM.Text.ToString.Length = 0 Then

                            Dim wzorzecReguly As String = "PACZKOPUNKT\s[A-Z0-9-]+"
                            Dim rx As Regex = New Regex(wzorzecReguly)
                            Dim wzorzecString As String = opis_zamow

                            Dim m As Match = rx.Match(wzorzecString)
                            If m.Success = True Then
                                TB_SR_POST_NUM.Text = m.Value.ToString.Replace("PACZKOPUNKT ", "")
                            End If

                            ''''DDL_SERVICE_TYPE_INPOST.SelectedValue = "inpost_locker_customer_service_point"
                        End If


                        Dim firstname As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_firstname from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)
                        Dim lastname As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_lastname from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)

                        If TB_SR_FIRSTNAME_INPOST.Text.ToString.Length = 0 Then
                            TB_SR_FIRSTNAME_INPOST.Text = firstname.ToUpper
                        End If

                        If TB_SR_LASTNAME_INPOST.Text.ToString.Length = 0 Then
                            TB_SR_LASTNAME_INPOST.Text = lastname.ToUpper
                        End If

                        ValidateFormField(TB_COMMENT_F_INPOST.Text, "COMMENT_F", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_DROP_OFF_INPOST.SelectedValue, "DROP_OFF", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_SERVICE_TYPE_INPOST.SelectedValue, "SERVICE_TYPE", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_LABEL_TYPE_INPOST.SelectedValue, "LABEL_TYPE", "dp_swm_mia_paczka_base")

                        ''####2022.10.14 / walidacja pola TB_SR_HOUSE_NUM oraz TB_SR_APART_NUM
                        If TB_SR_APART_NUM.Text <> "" And TB_SR_APART_NUM.Text <> "&bnsp;" Then
                            TB_SR_HOUSE_NUM.Text = TB_SR_HOUSE_NUM.Text.ToString & " " & TB_SR_APART_NUM.Text.ToString
                            TB_SR_APART_NUM.Text = ""
                        End If

                        If TB_SR_POSTAL_CODE.Text <> "" And TB_SR_POSTAL_CODE.Text <> "&nbsp;" Then
                            If TB_SR_POSTAL_CODE.Text.Contains("-") = False Then
                                Dim kod_p As String = TB_SR_POSTAL_CODE.Text.ToString
                                If kod_p.Length >= 5 Then
                                    TB_SR_POSTAL_CODE.Text = kod_p.Substring(0, 2).ToString & "-" & kod_p.Substring(2, 3).ToString
                                End If
                            End If
                        End If

                    ElseIf Session("firma_id").ToString.Contains("DHL_DE_API") Then
                        If TB_SR_POSTAL_CODE.Text <> "" And TB_SR_POSTAL_CODE.Text <> "&nbsp;" Then
                            If TB_SR_POSTAL_CODE.Text.Contains("-") = True Then
                                TB_SR_POSTAL_CODE.Text = TB_SR_POSTAL_CODE.Text.ToString.Replace("-", "")
                            End If
                        End If

                        ''If TB_SR_HOUSE_NUM.Text <> "" And TB_SR_HOUSE_NUM.Text <> "&bnsp;" Then
                        ''    Dim numer_domu As String = TB_SR_HOUSE_NUM.Text.ToString
                        ''    Dim numer_lokalu As String = ""
                        ''    Dim walidacja_numer_domu() As String = {"\", "/", "m", "m."}
                        ''    For Each walidacja_id In walidacja_numer_domu
                        ''        If TB_SR_HOUSE_NUM.Text.Contains(walidacja_id) Then
                        ''            Dim walidacja_tab As String() = numer_domu.Split(walidacja_id)
                        ''            If walidacja_tab.Count > 0 Then
                        ''                TB_SR_HOUSE_NUM.Text = walidacja_tab(0).ToString
                        ''                TB_SR_APART_NUM.Text = walidacja_tab(1).ToString
                        ''            End If

                        ''        End If
                        ''    Next
                        ''End If

                        ValidateFormField(DDL_CONTENT_DHL_DE_API.SelectedValue, "CONTENT", "dp_swm_mia_paczka_base")
                        If Session("schemat_dyspo") = "DOMINUS" Then
                            If DDL_SR_COUNTRY.SelectedValue = "PL" Then
                                TB_COMMENT_F_DHL_DE_API.Text = "DO " & Session("kod_dyspo")
                            Else
                                TB_COMMENT_F_DHL_DE_API.Text = "DO " & Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                            End If
                        Else
                            If DDL_SR_COUNTRY.SelectedValue = "PL" Then
                                TB_COMMENT_F_DHL_DE_API.Text = Session("kod_dyspo")
                            Else
                                TB_COMMENT_F_DHL_DE_API.Text = Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                            End If
                        End If

                        ''##2023.09.14 / wprowadzenie obslugi komentarza dla MSPL 
                        If LNr_zamow_o.Text.Contains("MSPL") Then
                            TB_COMMENT_F_DHL_DE_API.Text = Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                        End If

                        ValidateFormField(TB_COMMENT_F_DHL_DE_API.Text, "COMMENT_F", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_DROP_OFF_DHL_DE_API.SelectedValue, "DROP_OFF", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_SERVICE_TYPE_DHL_DE_API.SelectedValue, "SERVICE_TYPE", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_LABEL_TYPE_DHL_DE_API.SelectedValue, "LABEL_TYPE", "dp_swm_mia_paczka_base")
                    ElseIf Session("firma_id").ToString.Contains("UPS_DE_API") Then
                        If TB_SR_POSTAL_CODE.Text <> "" And TB_SR_POSTAL_CODE.Text <> "&nbsp;" Then
                            If TB_SR_POSTAL_CODE.Text.Contains("-") = True Then
                                TB_SR_POSTAL_CODE.Text = TB_SR_POSTAL_CODE.Text.ToString.Replace("-", "")
                            End If
                        End If

                        If TB_SR_HOUSE_NUM.Text <> "" And TB_SR_HOUSE_NUM.Text <> "&bnsp;" Then
                            Dim numer_domu As String = TB_SR_HOUSE_NUM.Text.ToString
                            Dim numer_lokalu As String = ""
                            Dim walidacja_numer_domu() As String = {"\", "/", "m", "m."}
                            For Each walidacja_id In walidacja_numer_domu
                                If TB_SR_HOUSE_NUM.Text.Contains(walidacja_id) Then
                                    Dim walidacja_tab As String() = numer_domu.Split(walidacja_id)
                                    If walidacja_tab.Count > 0 Then
                                        TB_SR_HOUSE_NUM.Text = walidacja_tab(0).ToString
                                        TB_SR_APART_NUM.Text = walidacja_tab(1).ToString
                                    End If

                                End If
                            Next
                        End If

                        ValidateFormField(DDL_CONTENT_UPS_DE_API.SelectedValue, "CONTENT", "dp_swm_mia_paczka_base")
                        If Session("schemat_dyspo") = "DOMINUS" Then
                            If DDL_SR_COUNTRY.SelectedValue = "PL" Then
                                TB_COMMENT_F_UPS_DE_API.Text = "DO " & Session("kod_dyspo")
                            Else
                                TB_COMMENT_F_UPS_DE_API.Text = "DO " & Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                            End If
                        Else
                            If DDL_SR_COUNTRY.SelectedValue = "PL" Then
                                TB_COMMENT_F_UPS_DE_API.Text = Session("kod_dyspo")
                            Else
                                TB_COMMENT_F_UPS_DE_API.Text = Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                            End If
                        End If

                        ''##2023.09.14 / wprowadzenie obslugi komentarza dla MSPL 
                        If LNr_zamow_o.Text.Contains("MSPL") Then
                            TB_COMMENT_F_UPS_DE_API.Text = Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                        End If

                        ValidateFormField(TB_COMMENT_F_UPS_DE_API.Text, "COMMENT_F", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_DROP_OFF_UPS_DE_API.SelectedValue, "DROP_OFF", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_SERVICE_TYPE_UPS_DE_API.SelectedValue, "SERVICE_TYPE", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_LABEL_TYPE_UPS_DE_API.SelectedValue, "LABEL_TYPE", "dp_swm_mia_paczka_base")
                    Else
                        ''##2022.10.17 / walidacja pol kod_p, numer domu, numer telefonu dla DHL
                        If TB_SR_POSTAL_CODE.Text <> "" And TB_SR_POSTAL_CODE.Text <> "&nbsp;" Then
                            If TB_SR_POSTAL_CODE.Text.Contains("-") = True Then
                                TB_SR_POSTAL_CODE.Text = TB_SR_POSTAL_CODE.Text.ToString.Replace("-", "")
                            End If
                        End If

                        If TB_SR_HOUSE_NUM.Text <> "" And TB_SR_HOUSE_NUM.Text <> "&bnsp;" Then
                            Dim numer_domu As String = TB_SR_HOUSE_NUM.Text.ToString
                            Dim numer_lokalu As String = ""
                            Dim walidacja_numer_domu() As String = {"\", "/", "m", "m."}
                            For Each walidacja_id In walidacja_numer_domu
                                If TB_SR_HOUSE_NUM.Text.Contains(walidacja_id) Then
                                    Dim walidacja_tab As String() = numer_domu.Split(walidacja_id)
                                    If walidacja_tab.Count > 0 Then
                                        TB_SR_HOUSE_NUM.Text = walidacja_tab(0).ToString
                                        TB_SR_APART_NUM.Text = walidacja_tab(1).ToString
                                    End If

                                End If
                            Next
                        End If

                        ValidateFormField(DDL_CONTENT.SelectedValue, "CONTENT", "dp_swm_mia_paczka_base")
                        If Session("schemat_dyspo") = "DOMINUS" Then
                            If DDL_SR_COUNTRY.SelectedValue = "PL" Then
                                TB_COMMENT_F.Text = "DO " & Session("kod_dyspo")
                            Else
                                TB_COMMENT_F.Text = "DO " & Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                            End If
                        Else
                            If DDL_SR_COUNTRY.SelectedValue = "PL" Then
                                TB_COMMENT_F.Text = Session("kod_dyspo")
                            Else
                                TB_COMMENT_F.Text = Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                            End If
                        End If

                        ''##2023.09.14 / wprowadzenie obslugi komentarza dla MSPL 
                        If LNr_zamow_o.Text.Contains("MSPL") Then
                            TB_COMMENT_F_DHL_PS.Text = Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                        End If

                        ValidateFormField(TB_COMMENT_F.Text, "COMMENT_F", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_DROP_OFF.SelectedValue, "DROP_OFF", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_SERVICE_TYPE.SelectedValue, "SERVICE_TYPE", "dp_swm_mia_paczka_base")
                        ValidateFormField(DDL_LABEL_TYPE.SelectedValue, "LABEL_TYPE", "dp_swm_mia_paczka_base")
                    End If
                End If

                If Session("firma_id") IsNot Nothing Then
                    If Session("firma_id").ToString.Contains("GEIS") Then
                        ''TB_COMMENT_F_GEIS.Text = Session("schemat_dyspo").ToString.Substring(0, 2) & Session("kod_dyspo").ToString
                        LadujDaneGridView_SS_Service_type_GEIS()
                    ElseIf Session("firma_id").ToString.Contains("INPOST") Then
                        ''TB_COMMENT_F_GEIS.Text = Session("schemat_dyspo").ToString.Substring(0, 2) & Session("kod_dyspo").ToString
                        LadujDaneGridView_SS_Service_type_INPOST()

                    ElseIf Session("firma_id").ToString.Contains("DHL_DE_API") Then
                        LadujDaneGridView_SS_Service_type_DHL_DE_API()
                        TB_B_BILL_ACC_NUM.Text = "33333333330102"
                    ElseIf Session("firma_id").ToString.Contains("UPS_DE_API") Then
                        LadujDaneGridView_SS_Service_type_UPS_DE_API()
                        TB_B_BILL_ACC_NUM.Text = "33333333330102"

                    ElseIf Session("firma_id").ToString.Contains("DHL") Then

                        If Session("schemat_dyspo") = "DAJAR" Then
                            TB_B_BILL_ACC_NUM.Text = "1276909"
                            TB_SS_SERVICE_VALUE.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select zg.wartosc from ht_zog zg where zg.ie$14 = DESQL_GRAF.DF11_2('" & Session("kod_dyspo") & "')", conn)

                            sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_paczka_ss WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' AND SS_SERVICE_TYPE='UBEZP'"
                            Dim ile_usluga_ubezp As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                            If ile_usluga_ubezp = "0" Then
                                sqlexp = "insert into dp_swm_mia_paczka_ss (schemat,shipment_id,ss_service_type,ss_service_value,ss_coll_on_form) values('" & Session("schemat_dyspo") & "','" & Session("shipment_id") & "','UBEZP','500','BANK_TRANSFER')"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            End If
                        ElseIf Session("schemat_dyspo") = "DOMINUS" Then
                            TB_B_BILL_ACC_NUM.Text = "1276909"
                            TB_SS_SERVICE_VALUE.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select zg.wartosc from ht_zog zg where zg.ie$14 = DESQL_GRAF.DF11_2('" & Session("kod_dyspo") & "')", conn)

                            Dim ss_service_value As Double = Double.Parse(TB_SS_SERVICE_VALUE.Text)
                        End If

                        LadujDaneGridView_SS_Service_type()

                    End If
                End If

                Session("contentKomunikat") = Session(session_id)

                EMAIL_ADD_Maskowanie(TB_PC_EMAIL_ADD, CBEmailSzyfrowanie)
                REFRESH_PAGE_CONTENT()
                conn.Close()
            End Using
        Catch ex As Exception
            ''''Console.WriteLine("Me.LoadComplete " & vbNewLine & ex.Message.ToString)
            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
            Session(session_id) += "<br />" & ex.Message.ToString & "<br />"
            Session(session_id) += "</div>"
        End Try
    End Sub

    Public Sub REFRESH_PAGE_CONTENT()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim f_id As String = ""

            If Session("firma_id") IsNot Nothing Then
                f_id = Session("firma_id")
            Else
                Dim shipment_grid As String = Session("shipment_id")
                If shipment_grid.StartsWith("S") Then
                    shipment_grid = "S%" & shipment_grid.Substring(2, shipment_grid.Length - 2)
                End If

                ''f_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct pi.firma_id from dp_swm_mia_paczka_info pi where pi.shipment_id like '" & shipment_grid & "' and pi.schemat='" & Session("schemat_dyspo") & "' and pi.paczka_id like '%/" & shipment_grid.ToString.Split("/")(1).ToString & "'", conn)
                f_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct pi.firma_id from dp_swm_mia_paczka_info pi left join dp_swm_mia_paczka_base ba on pi.shipment_id=ba.shipment_id where pi.shipment_id like '" & shipment_grid & "' and pi.schemat='" & Session("schemat_dyspo") & "' and ba.comment_f like '" & Session("kod_dyspo") & "%'", conn)
            End If

            If f_id <> "" Then
                ''!!!!!!!!!!!!!!sprawdzic wyswietlanie tabel i paneli dla firm kurierskich

                Dim firma_id As List(Of String) = GenerateFirmaTable()
                For Each tobj In firma_id
                    Dim obj_table As Table = DirectCast(FindControl("Table_" & tobj), Table)
                    If obj_table IsNot Nothing Then
                        obj_table.Visible = False
                    End If
                Next

                Dim tabelaObj As Table = DirectCast(FindControl("Table_" & f_id), Table)
                If tabelaObj IsNot Nothing Then
                    tabelaObj.Visible = True
                End If

                ''Dim t_DHL_PS As String() = {"Panel_DHL_PS_PRZESYLKA", "Panel_DHL_PS_GENERATE", "Panel_DHL_PS_SS_SERVICE_TYPE", "Panel_DHL_PS_B_PAYMENT"}
                Dim t_dhl As String() = {"Panel_DHL_PRZESYLKA", "Panel_DHL_GENERATE", "Panel_DHL_SS_SERVICE_TYPE", "Panel_DHL_B_PAYMENT"}
                Dim t_dhl_de_api As String() = {"Panel_DHL_DE_API_PRZESYLKA", "Panel_DHL_DE_API_GENERATE", "Panel_DHL_DE_API_SS_SERVICE_TYPE"}
                Dim t_ups_api As String() = {"Panel_UPS_DE_API_PRZESYLKA", "Panel_UPS_DE_API_GENERATE", "Panel_UPS_DE_API_SS_SERVICE_TYPE", "Panel_UPS_DE_API_B_PAYMENT"}
                Dim t_dhl_ps As String() = {"Panel_DHL_PS_PRZESYLKA", "Panel_DHL_GENERATE", "Panel_DHL_SS_SERVICE_TYPE", "Panel_DHL_B_PAYMENT"}
                Dim t_geis As String() = {"Panel_GEIS_PRZESYLKA", "Panel_GEIS_GENERATE", "Panel_GEIS_SS_SERVICE_TYPE"}
                Dim t_inpost As String() = {"Panel_INPOST_PRZESYLKA", "Panel_INPOST_GENERATE", "Panel_INPOST_SS_SERVICE_TYPE", "Panel_INPOST_B_PAYMENT"}

                If f_id.Contains("DHL_PS") Then
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_de_api, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_geis, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_inpost, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_ups_api, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_ps, Page, True)

                ElseIf f_id.Contains("DHL_DE_API") Then
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_ps, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_geis, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_inpost, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_ups_api, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_de_api, Page, True)

                ElseIf f_id.Contains("DHL") Then
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_ps, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_de_api, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_geis, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_inpost, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_ups_api, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl, Page, True)

                ElseIf f_id.Contains("GEIS") Then
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_ps, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_de_api, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_inpost, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_ups_api, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_geis, Page, True)

                ElseIf f_id.Contains("INPOST") Then
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_ps, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_de_api, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_geis, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_ups_api, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_inpost, Page, True)

                ElseIf f_id.Contains("UPS_DE_API") Then
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_ps, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_dhl_de_api, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_geis, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_inpost, Page, False)
                    dajarSWMagazyn_MIA.MyFunction.SetPanelObjectValue(t_ups_api, Page, True)

                End If

            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Dim przerwijLadowanie As Boolean = False
        Session.Remove(session_id)

        Try
            Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                conn.Open()

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

                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

                BWprowadzList_DHL.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno wprowadzic recznie nowy numer list przewozowy DHL\n') == false) return false")

                ''BGenerujEtykiete_DHL.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno utworzyc nowy list przewozowy DHL\n') == false) return false")
                BAnuluj_DHL.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować paczke DHL " & LNr_list_przewozowy.Text.ToString & "\n') == false) return false")

                BDodaj_SS_Service_type.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz dodac wybrana usluge dodatkowa DHL\n') == false) return false")
                BAnuluj_SS_Service_type.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz usunac wybrana usluge dodatkowa DHL\n') == false) return false")

                BDodaj_SS_Service_type_GEIS.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz dodac wybrana usluge dodatkowa GEIS\n') == false) return false")
                BAnuluj_SS_Service_type_GEIS.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz usunac wybrana usluge dodatkowa GEIS\n') == false) return false")

                BDodaj_SS_Service_type_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz dodac wybrana usluge dodatkowa INPOST\n') == false) return false")
                BAnuluj_SS_Service_type_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz usunac wybrana usluge dodatkowa INPOST\n') == false) return false")

                BZapiszPaczke.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zapisac paczke " & LPaczkaID.Text.ToString & "\n') == false) return false")
                BAnulujPaczek.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować paczke " & LPaczkaID.Text.ToString & "\n') == false) return false")

                BDodajZamowienie.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz wprowadzic zamowienie " & LZamowienieId.Text.ToString & "\n') == false) return false")
                BAnulujZamowienie.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować zamowienie " & LZamowienieId.Text.ToString & "\n') == false) return false")

                ''BWprowadzList_GEIS.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno wprowadzic recznie nowy numer list przewozowy GEIS\n') == false) return false")
                ''BGenerujEtykiete_GEIS.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno utworzyc nowy list przewozowy GEIS\n') == false) return false")
                BAnuluj_GEIS.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować paczke GEIS " & LNr_list_przewozowy.Text.ToString & "\n') == false) return false")
                BRaportPickup_GEIS.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno utworzyc protokol zdawczo-odbiorczy GEIS\n') == false) return false")

                ''BWprowadzList_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno wprowadzic recznie nowy numer list przewozowy INPOST\n') == false) return false")

                BGenerujEtykiete_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno utworzyc nowa oferte przewozowa INPOST\n') == false) return false")
                BGenerujOferte_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno oplacic oferte INPOST\n') == false) return false")
                BAnuluj_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować paczke INPOST " & LNr_list_przewozowy.Text.ToString & "\n') == false) return false")
                BZerowanie_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno wyzerowac ustawienia paczek INPOST " & LNr_list_przewozowy.Text.ToString & "\n') == false) return false")


                ''BZapiszFirme.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz ustawic firme " & DDLFirma.SelectedValue.ToString & " dla wszystkich paczek\n') == false) return false")

                If Not Page.IsPostBack Then
                    If przerwijLadowanie = False Then

                        Session.Remove(session_id)
                        Session.Remove("press_BZapiszEtykiete")

                        LadujDaneGridViewPakowanie()

                        ''##2024.05.27 / sprawdzenie firma_id po odswiezeniu danych paczek
                        For Each gdr As GridViewRow In GridViewPaczki.Rows
                            Dim firma_id As String = gdr.Cells(3).Text.ToString
                            ''If firma_id = "DHL" Or firma_id = "DHL_DE" Then
                            Session("firma_id") = firma_id
                            Exit For
                            ''End If
                        Next

                        LadujDaneGridViewZamowienia()
                        LadujDaneGridViewDyspozycjeInfo()
                        LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                        RefreshDDLFirma(DDLFirma, "")
                        RefreshDDLCountry(DDL_SR_COUNTRY, "")

                        LNr_list_przewozowy.Text = Session("shipment_id")
                        If Session("shipment_date") IsNot Nothing Then
                            TB_ST_SHIPMENT_DATE.Text = Session("shipment_date")
                        Else
                            TB_ST_SHIPMENT_DATE.Text = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("D2") & "-" & DateTime.Now.Day.ToString("D2")
                        End If

                        TB_ST_SHIPMENT_START.Text = "12:00"
                        TB_ST_SHIPMENT_END.Text = "15:00"

                        Session("mag_dyspo") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_zog WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        LMagDyspo.Text = Session("mag_dyspo")
                        LNrZamow.Text = Session("kod_dyspo")
                        LSchemat.Text = Session("schemat_dyspo")

                        sqlexp = "select SR_IS_PACKSTATION from dp_swm_mia_paczka where shipment_id='" & Session("shipment_id") & "'"
                        Dim packstation As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        sqlexp = "select SR_IS_POSTFILIALE from dp_swm_mia_paczka where shipment_id='" & Session("shipment_id") & "'"
                        Dim postfile As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                        If packstation = "1" Then
                            Session("dhl_opcje") = "1"
                            CB_SR_IS_PACKSTATION.Checked = True
                        ElseIf postfile = "1" Then
                            Session("dhl_opcje") = "1"
                            CB_SR_IS_POSTFILIALE.Checked = True
                        End If

                        sqlexp = "select (co_zostawi||co_zostaw2) opis from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & Session("kod_dyspo") & "')"
                        Dim opis_zamowienia As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        TBOpisZam.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                        sqlexp = "select zg.kod_kontr from ht_zog zg where zg.ie$14 = DESQL_GRAF.DF11_2('" & Session("kod_dyspo") & "')"
                        Dim kod_kontr As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                        ValidateDescriptionField(opis_zamowienia)

                        sqlexp = "select zg.nr_zamow_o, zg.data_zam from dp_swm_mia_zog zg where zg.nr_zamow = '" & Session("kod_dyspo") & "' and zg.schemat = '" & Session("schemat_dyspo") & "'"

                        Dim cmd As New OracleCommand(sqlexp, conn)
                        cmd.CommandType = CommandType.Text
                        Dim dr As OracleDataReader = cmd.ExecuteReader()
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

                        If DDL_SR_COUNTRY.SelectedValue = "PL" Then
                            TB_COMMENT_F.Text = Session("kod_dyspo")
                        Else
                            TB_COMMENT_F.Text = Session("kod_dyspo") & " " & LNr_zamow_o.Text.ToString
                        End If

                        If Session("schemat_dyspo") = "DOMINUS" And LNr_zamow_o.Text <> "" Then : panelDigitland.Visible = True
                        Else : panelDigitland.Visible = False
                        End If

                        If Session("firma_id") IsNot Nothing Then
                            If Session("firma_id").ToString.Contains("GEIS") Then
                                TB_COMMENT_F_GEIS.Text = Session("schemat_dyspo").ToString.Substring(0, 2) & " " & Session("kod_dyspo").ToString & " " & LNr_zamow_o.Text.ToString
                                LadujDaneGridView_SS_Service_type_GEIS()
                            ElseIf Session("firma_id").ToString.Contains("INPOST") Then
                                TB_COMMENT_F_INPOST.Text = Session("schemat_dyspo").ToString.Substring(0, 2) & " " & Session("kod_dyspo").ToString & " " & LNr_zamow_o.Text.ToString

                                Dim company As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_company from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)
                                TB_SR_COMPANY_INPOST.Text = company
                                Dim first_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_firstname from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)
                                TB_SR_FIRSTNAME_INPOST.Text = first_name
                                Dim last_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_lastname from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)
                                TB_SR_LASTNAME_INPOST.Text = last_name

                                LadujDaneGridView_SS_Service_type_INPOST()
                            ElseIf Session("firma_id").ToString.Contains("DHL_PS") Then

                                If Session("schemat_dyspo") = "DAJAR" Then
                                    TB_B_BILL_ACC_NUM.Text = "1276909"
                                    TB_SS_SERVICE_VALUE.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select zg.wartosc from ht_zog zg where zg.ie$14 = DESQL_GRAF.DF11_2('" & Session("kod_dyspo") & "')", conn)

                                    sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_paczka_ss WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' AND SS_SERVICE_TYPE='UBEZP'"
                                    Dim ile_usluga_ubezp As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                                    If ile_usluga_ubezp = "0" Then
                                        sqlexp = "insert into dp_swm_mia_paczka_ss (schemat,shipment_id,ss_service_type,ss_service_value,ss_coll_on_form) values('" & Session("schemat_dyspo") & "','" & Session("shipment_id") & "','UBEZP','500','BANK_TRANSFER')"
                                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    End If
                                ElseIf Session("schemat_dyspo") = "DOMINUS" Then
                                    TB_B_BILL_ACC_NUM.Text = "1276909"
                                    TB_SS_SERVICE_VALUE.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select zg.wartosc from ht_zog zg where zg.ie$14 = DESQL_GRAF.DF11_2('" & Session("kod_dyspo") & "')", conn)

                                    Dim ss_service_value As Double = Double.Parse(TB_SS_SERVICE_VALUE.Text)
                                End If


                                LadujDaneGridView_SS_Service_type_POP()
                            ElseIf Session("firma_id").ToString.Contains("DHL_DE_API") Then
                                LadujDaneGridView_SS_Service_type_DHL_DE_API()
                            ElseIf Session("firma_id").ToString.Contains("UPS_DE_API") Then
                                LadujDaneGridView_SS_Service_type_UPS_DE_API()

                            ElseIf Session("firma_id").ToString.Contains("DHL") Then

                                If Session("schemat_dyspo") = "DAJAR" Then
                                    TB_B_BILL_ACC_NUM.Text = "1276909"
                                    TB_SS_SERVICE_VALUE.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select zg.wartosc from ht_zog zg where zg.ie$14 = DESQL_GRAF.DF11_2('" & Session("kod_dyspo") & "')", conn)

                                    sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_paczka_ss WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' AND SS_SERVICE_TYPE='UBEZP'"
                                    Dim ile_usluga_ubezp As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                                    If ile_usluga_ubezp = "0" Then
                                        sqlexp = "insert into dp_swm_mia_paczka_ss (schemat,shipment_id,ss_service_type,ss_service_value,ss_coll_on_form) values('" & Session("schemat_dyspo") & "','" & Session("shipment_id") & "','UBEZP','500','BANK_TRANSFER')"
                                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    End If
                                ElseIf Session("schemat_dyspo") = "DOMINUS" Then
                                    TB_B_BILL_ACC_NUM.Text = "1276909"
                                    TB_SS_SERVICE_VALUE.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select zg.wartosc from ht_zog zg where zg.ie$14 = DESQL_GRAF.DF11_2('" & Session("kod_dyspo") & "')", conn)

                                    Dim ss_service_value As Double = Double.Parse(TB_SS_SERVICE_VALUE.Text)
                                End If


                                LadujDaneGridView_SS_Service_type()

                            End If
                        End If

                    End If
                End If
                conn.Close()
            End Using

            EMAIL_ADD_Maskowanie(TB_PC_EMAIL_ADD, CBEmailSzyfrowanie)
            Session("contentKomunikat") = Session(session_id)

        Catch ex As Exception
            Console.WriteLine("Me.Load " & vbNewLine & ex.Message.ToString)
            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
            Session(session_id) += "<br />" & ex.Message.ToString & "<br />"
            Session(session_id) += "</div>"
        End Try

    End Sub

    Public Sub ValidateDescriptionField(ByVal opis_zamowienia As String)
        Dim rx As Regex
        Dim match As Match

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            sqlexp = "SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
            Dim cmd As New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            Try
                While dr.Read()
                    Dim shippment_id As String = dr.Item(0).ToString
                    For Each ci As CultureInfo In CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                        Dim inforeg As New RegionInfo(ci.LCID)
                        inforeg.TwoLetterISORegionName.ToUpper()
                        Dim kraj_iso As String = inforeg.TwoLetterISORegionName.ToUpper
                        If opis_zamowienia.Contains(" " & kraj_iso & " ") Then
                            Dim kraj_akt As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT SR_COUNTRY FROM dp_swm_mia_PACZKA WHERE SHIPMENT_ID='" & shippment_id & "'", conn)
                            If kraj_akt.Length < 2 Then
                                DDL_SR_COUNTRY.SelectedValue = kraj_iso
                                sqlexp = "update dp_swm_mia_paczka set SR_COUNTRY='" & kraj_iso & "' where shipment_id='" & shippment_id & "'"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            End If
                            Exit For
                        End If
                    Next
                End While
            Catch ex As System.ArgumentOutOfRangeException
                Console.WriteLine(ex)
            End Try
            dr.Close()
            cmd.Dispose()
            conn.Close()

        End Using

        If opis_zamowienia.Contains("#") Then
            Dim opis_tmp() As String = opis_zamowienia.Split("#")
            If opis_tmp.Length > 7 Then
                TB_SR_NAME_TEST.Text = opis_tmp(1).ToString.Trim
                TB_SR_POSTAL_CODE_TEST.Text = opis_tmp(4).Trim
                TB_SR_CITY_TEST.Text = opis_tmp(3).Trim
                TB_SR_STREET_TEST.Text = opis_tmp(2).Trim

                ''##2022.10.18 / walidacja kodu kraju po podstawie pola opisowego [5]
                DDL_SR_COUNTRY.SelectedValue = opis_tmp(5).Trim.ToString

                Dim wzorzecReguly As String = "\d{1,}?[a-z|A-Z|M|m\.\s|\d|/]*"
                ''##2023.02.02 / aktualizacja reguly dotyczaca adresu 
                wzorzecReguly = "(\s)+\d{1,}?[a-z|A-Z|M|m\.\s|\d|\/]*"
                rx = New Regex(wzorzecReguly)
                ''Dim wzorzecString As String = opis_tmp(2).Substring(opis_tmp(2).Length / 2, opis_tmp(2).Length - opis_tmp(2).Length / 2)
                Dim wzorzecString As String = opis_tmp(2).ToString

                match = rx.Match(wzorzecString)
                If match.Success = True Then
                    Dim wzorzecIndeks As Integer = opis_tmp(2).IndexOf(match.Value.ToString)
                    If wzorzecIndeks > 0 Then
                        TB_SR_STREET_TEST.Text = opis_tmp(2).Substring(0, wzorzecIndeks).ToString.Trim
                        TB_SR_HOUSE_NUM_TEST.Text = opis_tmp(2).Substring(wzorzecIndeks, opis_tmp(2).Length - wzorzecIndeks).ToString.Trim
                    End If
                End If

                TB_PC_PERSON_NAME_TEST.Text = TB_SR_NAME_TEST.Text.ToString.Trim
                TB_PC_EMAIL_ADD_TEST.Text = opis_tmp(6).Trim
                TB_PC_PHONE_NUM_TEST.Text = opis_tmp(7).Trim
            End If
        End If

    End Sub

    Public Sub ValidateFormField(ByRef field_object As String, ByVal field As String, ByVal table As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If Session("shipment_id") <> "" And table <> "dp_swm_mia_paczka_base" Then
                sqlexp = "SELECT DISTINCT " & field & " FROM " & table & " WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND NR_ZAMOW='" & Session("kod_dyspo") & "'"
            Else
                If table = "dp_swm_mia_paczka_base" Then
                    If Session("shipment_id") IsNot Nothing Then
                        sqlexp = "SELECT DISTINCT " & field & " FROM " & table & " WHERE COMMENT_F LIKE '%" & Session("kod_dyspo") & "%' AND SHIPMENT_ID LIKE '" & Session("shipment_id") & "'"
                    Else
                        Dim shipment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' AND SHIPMENT_ID LIKE 'SH%'", conn)
                        If shipment_id <> "" Then
                            sqlexp = "SELECT DISTINCT " & field & " FROM " & table & " WHERE COMMENT_F LIKE '%" & Session("kod_dyspo") & "%' AND SHIPMENT_ID LIKE 'SH%'"
                        End If
                    End If

                ElseIf table = "dp_swm_mia_paczka" Then
                    sqlexp = "SELECT DISTINCT " & field & " FROM " & table & " WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' AND SHIPMENT_ID LIKE 'SH%'"
                End If
            End If

            Dim temp As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            If temp <> "" Then field_object = temp
            conn.Close()
        End Using
    End Sub

    Public Function WalidacjaInformacjiEtykieta(ByRef walidacja_message As String) As String
        Dim walidacja_status As String = "1"
        If Session("firma_id") = "UPS_DE_API" Then
            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
            If waga = "" Then waga = "0"
            Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
            Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
            Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString

            If Integer.Parse(szer) + Integer.Parse(wys) + Integer.Parse(dlug) > 299 Then
                walidacja_message = "Suma bokow paczki przekracza 299cm [szer+dlu+wys > 299]!"
                walidacja_status = "0"
            ElseIf Integer.Parse(waga) > 25 Then
                walidacja_message = "Waga paczki przekracza dopuszczalny limit 25kg!"
                walidacja_status = "0"
            ElseIf Integer.Parse(szer) > 99 Or Integer.Parse(dlug) > 99 Or Integer.Parse(wys) > 99 Then
                walidacja_message = "Najdluzszy wymiar paczki przekracza 99cm [max(dlug,wys,szer) > 99]!"
                walidacja_status = "0"
            End If
        End If
        Return walidacja_status
    End Function

    Protected Sub BZapiszEtykiete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BZapiszEtykiete.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session("press_BZapiszEtykiete") = "true"

            If Session("firma_id") = "DHL" Then
                sqlexp = "SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        Dim shippment_id As String = dr.Item(0).ToString
                        Dim tb_form_dhl() As String = {"B_BILL_ACC_NUM", "B_COSTS_CENTER", "ST_SHIPMENT_DATE", "ST_SHIPMENT_START", "ST_SHIPMENT_END", "SR_POST_NUM", "SR_NAME", "SR_POSTAL_CODE", "SR_CITY", "SR_STREET", "SR_HOUSE_NUM", "SR_APART_NUM", "PC_PERSON_NAME", "PC_PHONE_NUM", "PC_EMAIL_ADD"}
                        For Each tb_obj In tb_form_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_dhl() As String = {"B_SHIP_PAYMENT_TYPE", "B_PAYMENT_TYPE", "SR_COUNTRY"}
                        For Each ddl_obj In ddl_form_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj), DropDownList).SelectedValue.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim cb_form_dhl() As String = {"SR_IS_PACKSTATION", "SR_IS_POSTFILIALE"}
                        For Each cb_obj In cb_form_dhl
                            Dim cb_value As Boolean = DirectCast(FindControl("CB_" & cb_obj), CheckBox).Checked
                            If cb_value = True Then
                                sqlexp = "update dp_swm_mia_paczka set " & cb_value & "='1' where shipment_id='" & shippment_id & "'"
                            Else
                                sqlexp = "update dp_swm_mia_paczka set " & cb_value & "='0' where shipment_id='" & shippment_id & "'"
                            End If
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim drop_off_value As String = DirectCast(FindControl("DDL_DROP_OFF"), DropDownList).SelectedValue.ToString
                        Dim czyIstniejeBase As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT COUNT(*) FROM dp_swm_mia_paczka_base WHERE SHIPMENT_ID='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        If czyIstniejeBase = "0" Then
                            sqlexp = "insert into dp_swm_mia_paczka_base(SCHEMAT,SHIPMENT_ID,DROP_OFF) values ('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & drop_off_value & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End If

                        Dim tb_form_base_dhl() As String = {"COMMENT_F"}
                        For Each tb_obj In tb_form_base_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka_base set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_base_dhl() As String = {"CONTENT", "DROP_OFF", "SERVICE_TYPE", "LABEL_TYPE"}
                        For Each ddl_obj In ddl_form_base_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj), DropDownList).SelectedValue.ToString

                            sqlexp = "update dp_swm_mia_paczka_base set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        If DirectCast(FindControl("DDL_SERVICE_TYPE"), DropDownList).SelectedValue.ToString = "EK" Then
                            If Session("shipment_id") IsNot Nothing Then
                                sqlexp = "update dp_swm_mia_paczka_info set firma_id='DHL',pl_type='1' where shipment_id='" & Session("shipment_id") & "' and schemat='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                                dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            End If
                        End If

                    End While
                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()

            ElseIf Session("firma_id") = "DHL_PS" Then

                sqlexp = "SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        Dim shippment_id As String = dr.Item(0).ToString
                        Dim tb_form_dhl() As String = {"B_BILL_ACC_NUM", "B_COSTS_CENTER", "ST_SHIPMENT_DATE", "ST_SHIPMENT_START", "ST_SHIPMENT_END", "SR_POST_NUM", "SR_NAME", "SR_POSTAL_CODE", "SR_CITY", "SR_STREET", "SR_HOUSE_NUM", "SR_APART_NUM", "PC_PERSON_NAME", "PC_PHONE_NUM", "PC_EMAIL_ADD"}
                        For Each tb_obj In tb_form_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_dhl() As String = {"B_SHIP_PAYMENT_TYPE", "B_PAYMENT_TYPE", "SR_COUNTRY"}
                        For Each ddl_obj In ddl_form_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj), DropDownList).SelectedValue.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim cb_form_dhl() As String = {"SR_IS_PACKSTATION", "SR_IS_POSTFILIALE"}
                        For Each cb_obj In cb_form_dhl
                            Dim cb_value As Boolean = DirectCast(FindControl("CB_" & cb_obj), CheckBox).Checked
                            If cb_value = True Then
                                sqlexp = "update dp_swm_mia_paczka set " & cb_value & "='1' where shipment_id='" & shippment_id & "'"
                            Else
                                sqlexp = "update dp_swm_mia_paczka set " & cb_value & "='0' where shipment_id='" & shippment_id & "'"
                            End If
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim drop_off_value As String = DirectCast(FindControl("DDL_DROP_OFF_DHL_PS"), DropDownList).SelectedValue.ToString
                        Dim czyIstniejeBase As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT COUNT(*) FROM dp_swm_mia_paczka_base WHERE SHIPMENT_ID='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        If czyIstniejeBase = "0" Then
                            sqlexp = "insert into dp_swm_mia_paczka_base(SCHEMAT,SHIPMENT_ID,DROP_OFF) values ('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & drop_off_value & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End If

                        Dim tb_form_base_dhl() As String = {"COMMENT_F"}
                        For Each tb_obj In tb_form_base_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj & "_DHL_PS"), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka_base set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_base_dhl() As String = {"CONTENT", "DROP_OFF", "SERVICE_TYPE", "LABEL_TYPE"}
                        For Each ddl_obj In ddl_form_base_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj & "_DHL_PS"), DropDownList).SelectedValue.ToString

                            sqlexp = "update dp_swm_mia_paczka_base set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        If DirectCast(FindControl("DDL_SERVICE_TYPE_DHL_PS"), DropDownList).SelectedValue.ToString = "EK" Then
                            If Session("shipment_id") IsNot Nothing Then
                                sqlexp = "update dp_swm_mia_paczka_info set firma_id='DHL',pl_type='1' where shipment_id='" & Session("shipment_id") & "' and schemat='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                                dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            End If
                        End If

                    End While
                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()

            ElseIf Session("firma_id").ToString.Contains("GEIS") Then
                sqlexp = "SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        Dim shippment_id As String = dr.Item(0).ToString
                        Dim tb_form_dhl() As String = {"ST_SHIPMENT_DATE", "ST_SHIPMENT_START", "ST_SHIPMENT_END", "SR_POST_NUM", "SR_NAME", "SR_POSTAL_CODE", "SR_CITY", "SR_STREET", "SR_HOUSE_NUM", "SR_APART_NUM", "PC_PERSON_NAME", "PC_PHONE_NUM", "PC_EMAIL_ADD"}
                        For Each tb_obj In tb_form_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_dhl() As String = {"SR_COUNTRY"}
                        For Each ddl_obj In ddl_form_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj), DropDownList).SelectedValue.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim drop_off_value As String = DirectCast(FindControl("DDL_DROP_OFF"), DropDownList).SelectedValue.ToString
                        Dim czyIstniejeBase As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT COUNT(*) FROM dp_swm_mia_paczka_base WHERE SHIPMENT_ID='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        If czyIstniejeBase = "0" Then
                            sqlexp = "insert into dp_swm_mia_paczka_base(SCHEMAT,SHIPMENT_ID,DROP_OFF) values ('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & drop_off_value & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End If

                        Dim tb_form_base_dhl() As String = {"COMMENT_F"}
                        For Each tb_obj In tb_form_base_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj & "_GEIS"), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka_base set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_base_dhl() As String = {"CONTENT", "DROP_OFF", "SERVICE_TYPE", "LABEL_TYPE"}
                        For Each ddl_obj In ddl_form_base_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj & "_GEIS"), DropDownList).SelectedValue.ToString

                            sqlexp = "update dp_swm_mia_paczka_base set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next
                    End While
                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()
            ElseIf Session("firma_id") = "DHL_DE" Then
                sqlexp = "SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        Dim shippment_id As String = dr.Item(0).ToString

                        Dim tb_form_dhl() As String = {"B_BILL_ACC_NUM", "B_COSTS_CENTER", "ST_SHIPMENT_DATE", "ST_SHIPMENT_START", "ST_SHIPMENT_END", "SR_POST_NUM", "SR_NAME", "SR_POSTAL_CODE", "SR_CITY", "SR_STREET", "SR_HOUSE_NUM", "SR_APART_NUM", "PC_PERSON_NAME", "PC_PHONE_NUM", "PC_EMAIL_ADD"}
                        For Each tb_obj In tb_form_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj), TextBox).Text.ToString
                            If tb_obj = "PC_PERSON_NAME" And tb_value.Length > 30 Then
                                tb_value = tb_value.Substring(0, 30)
                            End If

                            If tb_obj = "SR_NAME" And tb_value.Length > 30 Then
                                tb_value = tb_value.Substring(0, 30)
                            End If

                            sqlexp = "update dp_swm_mia_paczka set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_dhl() As String = {"B_SHIP_PAYMENT_TYPE", "B_PAYMENT_TYPE", "SR_COUNTRY"}
                        For Each ddl_obj In ddl_form_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj), DropDownList).SelectedValue.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim cb_form_dhl() As String = {"SR_IS_PACKSTATION", "SR_IS_POSTFILIALE"}
                        For Each cb_obj In cb_form_dhl
                            Dim cb_value As Boolean = DirectCast(FindControl("CB_" & cb_obj), CheckBox).Checked
                            If cb_value = True Then
                                sqlexp = "update dp_swm_mia_paczka set " & cb_obj & "='1' where shipment_id='" & shippment_id & "'"
                            Else
                                sqlexp = "update dp_swm_mia_paczka set " & cb_obj & "='0' where shipment_id='" & shippment_id & "'"
                            End If
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim drop_off_value As String = DirectCast(FindControl("DDL_DROP_OFF"), DropDownList).SelectedValue.ToString
                        Dim czyIstniejeBase As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT COUNT(*) FROM dp_swm_mia_paczka_base WHERE SHIPMENT_ID='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        If czyIstniejeBase = "0" Then
                            sqlexp = "insert into dp_swm_mia_paczka_base(SCHEMAT,SHIPMENT_ID,DROP_OFF) values ('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & drop_off_value & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End If

                        Dim tb_form_base_dhl() As String = {"COMMENT_F"}
                        For Each tb_obj In tb_form_base_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka_base set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_base_dhl() As String = {"CONTENT", "DROP_OFF", "SERVICE_TYPE", "LABEL_TYPE"}
                        For Each ddl_obj In ddl_form_base_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj), DropDownList).SelectedValue.ToString

                            sqlexp = "update dp_swm_mia_paczka_base set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next
                    End While
                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()

            ElseIf Session("firma_id") = "DHL_DE_API" Then
                sqlexp = "SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        Dim shippment_id As String = dr.Item(0).ToString

                        Dim tb_form_dhl() As String = {"B_BILL_ACC_NUM", "B_COSTS_CENTER", "ST_SHIPMENT_DATE", "ST_SHIPMENT_START", "ST_SHIPMENT_END", "SR_POST_NUM", "SR_NAME", "SR_POSTAL_CODE", "SR_CITY", "SR_STREET", "SR_HOUSE_NUM", "SR_APART_NUM", "PC_PERSON_NAME", "PC_PHONE_NUM", "PC_EMAIL_ADD"}
                        For Each tb_obj In tb_form_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj), TextBox).Text.ToString
                            If tb_obj = "PC_PERSON_NAME" And tb_value.Length > 50 Then
                                tb_value = tb_value.Substring(0, 50)
                            End If

                            If tb_obj = "SR_NAME" And tb_value.Length > 50 Then
                                tb_value = tb_value.Substring(0, 50)
                            End If

                            sqlexp = "update dp_swm_mia_paczka set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_dhl() As String = {"B_SHIP_PAYMENT_TYPE", "B_PAYMENT_TYPE", "SR_COUNTRY"}
                        For Each ddl_obj In ddl_form_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj), DropDownList).SelectedValue.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim cb_form_dhl() As String = {"SR_IS_PACKSTATION", "SR_IS_POSTFILIALE"}
                        For Each cb_obj In cb_form_dhl
                            Dim cb_value As Boolean = DirectCast(FindControl("CB_" & cb_obj), CheckBox).Checked
                            If cb_value = True Then
                                sqlexp = "update dp_swm_mia_paczka set " & cb_obj & "='1' where shipment_id='" & shippment_id & "'"
                            Else
                                sqlexp = "update dp_swm_mia_paczka set " & cb_obj & "='0' where shipment_id='" & shippment_id & "'"
                            End If
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim drop_off_value As String = DirectCast(FindControl("DDL_DROP_OFF"), DropDownList).SelectedValue.ToString
                        Dim czyIstniejeBase As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT COUNT(*) FROM dp_swm_mia_paczka_base WHERE SHIPMENT_ID='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        If czyIstniejeBase = "0" Then
                            sqlexp = "insert into dp_swm_mia_paczka_base(SCHEMAT,SHIPMENT_ID,DROP_OFF) values ('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & drop_off_value & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End If

                        Dim tb_form_base_dhl() As String = {"COMMENT_F"}
                        For Each tb_obj In tb_form_base_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj & "_DHL_DE_API"), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka_base set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_base_dhl() As String = {"CONTENT", "DROP_OFF", "SERVICE_TYPE", "LABEL_TYPE"}
                        For Each ddl_obj In ddl_form_base_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj & "_DHL_DE_API"), DropDownList).SelectedValue.ToString

                            sqlexp = "update dp_swm_mia_paczka_base set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next
                    End While
                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()
            ElseIf Session("firma_id") = "UPS_DE_API" Then
                sqlexp = "SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        Dim shippment_id As String = dr.Item(0).ToString

                        Dim tb_form_dhl() As String = {"B_BILL_ACC_NUM", "B_COSTS_CENTER", "ST_SHIPMENT_DATE", "ST_SHIPMENT_START", "ST_SHIPMENT_END", "SR_POST_NUM", "SR_NAME", "SR_POSTAL_CODE", "SR_CITY", "SR_STREET", "SR_HOUSE_NUM", "SR_APART_NUM", "PC_PERSON_NAME", "PC_PHONE_NUM", "PC_EMAIL_ADD"}
                        For Each tb_obj In tb_form_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj), TextBox).Text.ToString
                            If tb_obj = "PC_PERSON_NAME" And tb_value.Length > 30 Then
                                tb_value = tb_value.Substring(0, 30)
                            End If

                            If tb_obj = "SR_NAME" And tb_value.Length > 30 Then
                                tb_value = tb_value.Substring(0, 30)
                            End If

                            sqlexp = "update dp_swm_mia_paczka set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_dhl() As String = {"B_SHIP_PAYMENT_TYPE", "B_PAYMENT_TYPE", "SR_COUNTRY"}
                        For Each ddl_obj In ddl_form_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj), DropDownList).SelectedValue.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim cb_form_dhl() As String = {"SR_IS_PACKSTATION", "SR_IS_POSTFILIALE"}
                        For Each cb_obj In cb_form_dhl
                            Dim cb_value As Boolean = DirectCast(FindControl("CB_" & cb_obj), CheckBox).Checked
                            If cb_value = True Then
                                sqlexp = "update dp_swm_mia_paczka set " & cb_obj & "='1' where shipment_id='" & shippment_id & "'"
                            Else
                                sqlexp = "update dp_swm_mia_paczka set " & cb_obj & "='0' where shipment_id='" & shippment_id & "'"
                            End If
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim drop_off_value As String = DirectCast(FindControl("DDL_DROP_OFF"), DropDownList).SelectedValue.ToString
                        Dim czyIstniejeBase As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT COUNT(*) FROM dp_swm_mia_paczka_base WHERE SHIPMENT_ID='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        If czyIstniejeBase = "0" Then
                            sqlexp = "insert into dp_swm_mia_paczka_base(SCHEMAT,SHIPMENT_ID,DROP_OFF) values ('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & drop_off_value & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End If

                        Dim tb_form_base_dhl() As String = {"COMMENT_F"}
                        For Each tb_obj In tb_form_base_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj & "_UPS_DE_API"), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka_base set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_base_dhl() As String = {"CONTENT", "DROP_OFF", "SERVICE_TYPE", "LABEL_TYPE"}
                        For Each ddl_obj In ddl_form_base_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj & "_UPS_DE_API"), DropDownList).SelectedValue.ToString

                            sqlexp = "update dp_swm_mia_paczka_base set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'   and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next
                    End While
                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()
                ''######2022.07.20 // zapisywanie etykiety INPOST
            ElseIf Session("firma_id").ToString.Contains("INPOST") Then
                sqlexp = "SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        Dim shippment_id As String = dr.Item(0).ToString
                        Dim tb_form_dhl() As String = {"ST_SHIPMENT_DATE", "ST_SHIPMENT_START", "ST_SHIPMENT_END", "SR_POST_NUM", "SR_NAME", "SR_POSTAL_CODE", "SR_CITY", "SR_STREET", "SR_HOUSE_NUM", "SR_APART_NUM", "PC_PERSON_NAME", "PC_PHONE_NUM", "PC_EMAIL_ADD"}
                        For Each tb_obj In tb_form_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_dhl() As String = {"SR_COUNTRY"}
                        For Each ddl_obj In ddl_form_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj), DropDownList).SelectedValue.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_inpost() As String = {"B_COSTS_CENTER"}
                        For Each ddl_obj In ddl_form_inpost
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj & "_INPOST"), DropDownList).SelectedValue.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim drop_off_value As String = DirectCast(FindControl("DDL_DROP_OFF"), DropDownList).SelectedValue.ToString
                        Dim czyIstniejeBase As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT COUNT(*) FROM dp_swm_mia_paczka_base WHERE SHIPMENT_ID='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        If czyIstniejeBase = "0" Then
                            sqlexp = "insert into dp_swm_mia_paczka_base(SCHEMAT,SHIPMENT_ID,DROP_OFF) values ('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & drop_off_value & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End If

                        Dim tb_form_base_dhl() As String = {"COMMENT_F"}
                        For Each tb_obj In tb_form_base_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj & "_INPOST"), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka_base set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_base_dhl() As String = {"CONTENT", "DROP_OFF", "SERVICE_TYPE", "LABEL_TYPE"}
                        For Each ddl_obj In ddl_form_base_dhl
                            Dim ddl_value As String = DirectCast(FindControl("DDL_" & ddl_obj & "_INPOST"), DropDownList).SelectedValue.ToString

                            sqlexp = "update dp_swm_mia_paczka_base set " & ddl_obj & "='" & ddl_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next
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

    Protected Sub BWprowadzList_DHL_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWprowadzList_DHL.Click
        ''####2022.08.17 / wprowdzenie dodawania recznie listow DHL
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim shippment_id As String = Session("shipment_id").ToString

            If shippment_id <> "" Then
                Dim dhl_label_id As String = TBListPrzewozowy_DHL.Text.ToString

                If dhl_label_id = "" Then
                    dhl_label_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                End If

                ''##2023.06.22 / wprowadzenie ST_SHIPMENT_DATE dla listow DHL
                sqlexp = "update dp_swm_mia_paczka set shipment_id='" & dhl_label_id & "',st_shipment_date='" & DirectCast(FindControl("TB_ST_SHIPMENT_DATE"), TextBox).Text.ToString & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & dhl_label_id & "',shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                For Each row As GridViewRow In GridViewZamowienia.Rows
                    Dim nr_zam As String = row.Cells(2).Text.ToString
                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), dhl_label_id, nr_zam)
                Next

                ''zmiana numeru listu przewozowego
                Session("shipment_id") = dhl_label_id
                LNr_list_przewozowy.Text = Session("shipment_id")
                LadujDaneGridViewZamowienia()
                LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                TBListPrzewozowy_DHL.Text = ""
            End If
            conn.Close()

        End Using
    End Sub

    Protected Sub BWprowadzList_DHL_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWprowadzList_DHL_DE_API.Click
        ''####2022.08.17 / wprowdzenie dodawania recznie listow DHL
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim shippment_id As String = Session("shipment_id").ToString

            If shippment_id <> "" Then
                Dim dhl_label_id As String = TBListPrzewozowy_DHL_DE_API.Text.ToString

                If dhl_label_id = "" Then
                    dhl_label_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                End If

                ''##2023.06.22 / wprowadzenie ST_SHIPMENT_DATE dla listow DHL
                sqlexp = "update dp_swm_mia_paczka set shipment_id='" & dhl_label_id & "',st_shipment_date='" & DirectCast(FindControl("TB_ST_SHIPMENT_DATE"), TextBox).Text.ToString & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & dhl_label_id & "',shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                For Each row As GridViewRow In GridViewZamowienia.Rows
                    Dim nr_zam As String = row.Cells(2).Text.ToString
                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), dhl_label_id, nr_zam)
                Next

                ''zmiana numeru listu przewozowego
                Session("shipment_id") = dhl_label_id
                LNr_list_przewozowy.Text = Session("shipment_id")
                LadujDaneGridViewZamowienia()
                LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                TBListPrzewozowy_DHL_DE_API.Text = ""
            End If
            conn.Close()

        End Using
    End Sub

    Protected Sub BWprowadzList_UPS_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWprowadzList_UPS_DE_API.Click
        ''####2022.08.17 / wprowdzenie dodawania recznie listow DHL
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim shippment_id As String = Session("shipment_id").ToString

            If shippment_id <> "" Then
                Dim dhl_label_id As String = TBListPrzewozowy_UPS_DE_API.Text.ToString

                If dhl_label_id = "" Then
                    dhl_label_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                End If

                ''##2023.06.22 / wprowadzenie ST_SHIPMENT_DATE dla listow DHL
                sqlexp = "update dp_swm_mia_paczka set shipment_id='" & dhl_label_id & "',st_shipment_date='" & DirectCast(FindControl("TB_ST_SHIPMENT_DATE"), TextBox).Text.ToString & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & dhl_label_id & "',shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                For Each row As GridViewRow In GridViewZamowienia.Rows
                    Dim nr_zam As String = row.Cells(2).Text.ToString
                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), dhl_label_id, nr_zam)
                Next

                ''zmiana numeru listu przewozowego
                Session("shipment_id") = dhl_label_id
                LNr_list_przewozowy.Text = Session("shipment_id")
                LadujDaneGridViewZamowienia()
                LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                TBListPrzewozowy_UPS_DE_API.Text = ""
            End If
            conn.Close()

        End Using
    End Sub

    Protected Sub BWprowadzList_INPOST_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWprowadzList_INPOST.Click
        ''####2022.08.17 / wprowdzenie dodawania recznie listow INPOST
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim shippment_id As String = Session("shipment_id").ToString

            If shippment_id <> "" Then
                Dim inpost_label_id As String = TBListPrzewozowy_INPOST.Text.ToString

                If inpost_label_id = "" Then
                    inpost_label_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                End If

                ''##2023.06.22 / wprowadzenie ST_SHIPMENT_DATE dla listow INPOST
                sqlexp = "update dp_swm_mia_paczka set shipment_id='" & inpost_label_id & "',st_shipment_date='" & DirectCast(FindControl("TB_ST_SHIPMENT_DATE"), TextBox).Text.ToString & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & inpost_label_id & "',shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                For Each row As GridViewRow In GridViewZamowienia.Rows
                    Dim nr_zam As String = row.Cells(2).Text.ToString
                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), inpost_label_id, nr_zam)
                Next

                ''zmiana numeru listu przewozowego
                Session("shipment_id") = inpost_label_id
                LNr_list_przewozowy.Text = Session("shipment_id")
                LadujDaneGridViewZamowienia()
                LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                TBListPrzewozowy_INPOST.Text = ""
            End If
            conn.Close()

        End Using
    End Sub

    Protected Sub BWprowadzList_GEIS_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWprowadzList_GEIS.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim shippment_id As String = Session("shipment_id").ToString

            If shippment_id <> "" Then
                Dim geis_label_id As String = TBListPrzewozowy_GEIS.Text.ToString

                If geis_label_id = "" Then
                    geis_label_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                End If

                ''##2023.06.22 / wprowadzenie ST_SHIPMENT_DATE dla listow GEIS
                sqlexp = "update dp_swm_mia_paczka set shipment_id='" & geis_label_id & "',st_shipment_date='" & DirectCast(FindControl("TB_ST_SHIPMENT_DATE"), TextBox).Text.ToString & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & geis_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & geis_label_id & "',shipment_id='" & geis_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & geis_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                For Each row As GridViewRow In GridViewZamowienia.Rows
                    Dim nr_zam As String = row.Cells(2).Text.ToString
                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), geis_label_id, nr_zam)
                Next

                ''zmiana numeru listu przewozowego
                Session("shipment_id") = geis_label_id
                LNr_list_przewozowy.Text = Session("shipment_id")
                LadujDaneGridViewZamowienia()
                LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                TBListPrzewozowy_GEIS.Text = ""
            End If

            conn.Close()
        End Using
    End Sub

    Protected Sub BGenerujEtykiete_DHL_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BGenerujEtykiete_DHL.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session.Remove("contentKomunikat")
            Dim przerwijGenerowanie As Boolean = False
            For Each gdr As GridViewRow In GridViewPaczki.Rows
                Dim firma_id As String = gdr.Cells(3).Text.ToString
                If firma_id.Contains("DHL") = False Then
                    przerwijGenerowanie = True
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Wprowadz pole firma_id dla wszystkich utworzonych paczek!<br />"
                    Session(session_id) += "</div>"
                    Exit For
                End If
            Next

            sqlexp = "select count(*) from dp_swm_mia_paczka where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and shipment_id not like 'S%'"
            Dim ile_ship_dhl As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            If ile_ship_dhl <> "0" Then
                przerwijGenerowanie = True
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Dla wybranych paczek istnieje juz utworzona etykieta DHL!<br />"
                Session(session_id) += "</div>"
            End If

            If przerwijGenerowanie = False Then
                Dim shippment_id As String = Session("shipment_id").ToString
                ''shippment_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                If Session("firma_id") = "DHL" Then
                    If DirectCast(FindControl("DDL_SERVICE_TYPE"), DropDownList).SelectedValue.ToString = "EK" Then
                        Dim dhl_label_id_LAST As String = ""
                        Dim package_count As Integer = GridViewPaczki.Rows.Count
                        ''dhl_package.dhl_loadPackage(package_count - 1)
                        ''package_count = 0
                        For Each gdr As GridViewRow In GridViewPaczki.Rows
                            ''#############################TWORZENIE NAGLOWKA DLA DHL24############################
                            Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                            dhl_package.dhl_loadRequestInfo(DirectCast(FindControl("DDL_CONTENT"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("TB_COMMENT_F"), TextBox).Text.ToString)
                            dhl_package.dhl_loadShipmentInfo(DirectCast(FindControl("DDL_SERVICE_TYPE"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("DDL_DROP_OFF"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("DDL_LABEL_TYPE"), DropDownList).SelectedValue.ToString)
                            dhl_package.dhl_loadShipmentTime(DirectCast(FindControl("TB_ST_SHIPMENT_DATE"), TextBox).Text.ToString, DirectCast(FindControl("TB_ST_SHIPMENT_START"), TextBox).Text.ToString, DirectCast(FindControl("TB_ST_SHIPMENT_END"), TextBox).Text.ToString)
                            dhl_package.dhl_loadBilling(DirectCast(FindControl("DDL_B_SHIP_PAYMENT_TYPE"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("DDL_B_PAYMENT_TYPE"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("TB_B_BILL_ACC_NUM"), TextBox).Text.ToString)

                            dhl_package.dhl_loadAddressat("DAJAR SP Z O.O.", "75137", "Koszalin", "Rożana", "9", "0")

                            dhl_package.dhl_loadReceiverAddressat(DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_CITY"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_APART_NUM"), TextBox).Text.ToString, DirectCast(FindControl("DDL_SR_COUNTRY"), DropDownList).SelectedValue.ToString)
                            'ZMIANA ROZMIARU TEL. DO 9 ZNAKOW
                            Dim phone_num As String = DirectCast(FindControl("TB_PC_PHONE_NUM"), TextBox).Text.ToString
                            If phone_num.Length > 9 Then
                                phone_num = phone_num.Substring(0, 9)
                            End If

                            dhl_package.dhl_loadPreavisoContact_EK(phone_num, DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_EMAIL_ADD"), TextBox).Text.ToString)

                            ''#############################TWORZENIE DANYCH DLA PACZKI DHL24############################

                            dhl_package.dhl_loadPackage(1)

                            ''#########################LADOWANIE AKTUALNEGO NUMERU SHIPMENT_ID DLA PACZEK DHL24#####################
                            Dim paczka_id As String = gdr.Cells(2).Text.ToString

                            sqlexp = "select shipment_id from dp_swm_mia_paczka_info where paczka_id='" & paczka_id & "' and SCHEMAT='" & Session("schemat_dyspo") & "'"
                            shippment_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                            Dim firma_id As String = gdr.Cells(3).Text.ToString
                            Dim typ As String = gdr.Cells(4).Text.ToString
                            Dim rodzaj As String = gdr.Cells(5).Text.ToString
                            Dim waga As String = gdr.Cells(6).Text.ToString
                            Dim szer As String = gdr.Cells(7).Text.ToString
                            Dim wys As String = gdr.Cells(8).Text.ToString
                            Dim dlug As String = gdr.Cells(9).Text.ToString
                            Dim ile_opak As String = gdr.Cells(10).Text.ToString
                            If firma_id.Contains("DHL") Then
                                If typ = "paczka" Then : typ = "PACKAGE"
                                ElseIf typ = "paleta" Then : typ = "PALLET"
                                ElseIf typ = "koperta" Then : typ = "ENVELOPE"
                                Else : typ = "PACKAGE"
                                End If

                                If rodzaj = "standard" Then : rodzaj = "0"
                                Else : rodzaj = "1"
                                End If

                                dhl_package.dhl_AddPackage(0, typ, ile_opak, waga, szer, wys, dlug, rodzaj)
                                ''package_count += 1
                            End If

                            ''#############################TWORZENIE LISTU PRZEWOZOWEGO DLA PACZKI DHL24############################
                            Dim dhl_label_id As String = dhl_package.dhl_createShipmentJJD(dhl_session)
                            If dhl_label_id <> "" Then
                                ''''dhl_label_id_LAST = "JJD000030059946000000000433"
                                sqlexp = "select count(*) from dp_swm_mia_paczka WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                Dim czy_swm_paczka As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                                If czy_swm_paczka = "0" Then
                                    sqlexp = "insert into dp_swm_mia_paczka select NR_ZAMOW, SCHEMAT, AUTODATA, '" & shippment_id & "' SHIPMENT_ID, B_SHIP_PAYMENT_TYPE, B_BILL_ACC_NUM, B_PAYMENT_TYPE, B_COSTS_CENTER, ST_SHIPMENT_DATE, ST_SHIPMENT_START, ST_SHIPMENT_END, SR_COUNTRY, SR_IS_PACKSTATION, SR_IS_POSTFILIALE, SR_POST_NUM, SR_NAME, SR_POSTAL_CODE, SR_CITY, SR_STREET, SR_HOUSE_NUM, SR_APART_NUM, PC_PERSON_NAME, PC_PHONE_NUM, PC_EMAIL_ADD from dp_swm_mia_paczka where shipment_id='" & dhl_label_id_LAST & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                    dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                End If

                                sqlexp = "update dp_swm_mia_paczka set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                                sqlexp = "select count(*) from dp_swm_mia_paczka_base WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                Dim czy_swm_paczka_base As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                                If czy_swm_paczka_base = "0" Then
                                    sqlexp = "insert into dp_swm_mia_paczka_base select SCHEMAT,'" & shippment_id & "' SHIPMENT_ID,DROP_OFF,SERVICE_TYPE,LABEL_TYPE,CONTENT,COMMENT_F,REFERENCE FROM dp_swm_mia_paczka_base where shipment_id='" & dhl_label_id_LAST & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                End If

                                sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                                sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & dhl_label_id & "',shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                                sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                                For Each row As GridViewRow In GridViewZamowienia.Rows
                                    Dim nr_zam As String = row.Cells(2).Text.ToString
                                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), dhl_label_id, nr_zam)
                                Next

                                ''zmiana numeru listu przewozowego
                                ''Session("shipment_id") = dhl_label_id
                                ''LNr_list_przewozowy.Text = Session("shipment_id")
                                ''LadujDaneGridViewZamowienia()
                                ''LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                                dhl_label_id_LAST = dhl_label_id
                            Else
                                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                                Session(session_id) += "<br />Komunikat DHL : " & dhl_package.exception.ToString & "<br />"
                                Session(session_id) += "</div>"
                            End If



                        Next

                        If dhl_label_id_LAST <> "" Then
                            ''zmiana numeru listu przewozowego
                            Session("shipment_id") = dhl_label_id_LAST
                            LNr_list_przewozowy.Text = Session("shipment_id")
                            LadujDaneGridViewZamowienia()
                            LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                        End If
                    Else
                        Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                        dhl_package.dhl_loadRequestInfo(DirectCast(FindControl("DDL_CONTENT"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("TB_COMMENT_F"), TextBox).Text.ToString)
                        dhl_package.dhl_loadShipmentInfo(DirectCast(FindControl("DDL_SERVICE_TYPE"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("DDL_DROP_OFF"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("DDL_LABEL_TYPE"), DropDownList).SelectedValue.ToString)
                        dhl_package.dhl_loadShipmentTime(DirectCast(FindControl("TB_ST_SHIPMENT_DATE"), TextBox).Text.ToString, DirectCast(FindControl("TB_ST_SHIPMENT_START"), TextBox).Text.ToString, DirectCast(FindControl("TB_ST_SHIPMENT_END"), TextBox).Text.ToString)
                        dhl_package.dhl_loadBilling(DirectCast(FindControl("DDL_B_SHIP_PAYMENT_TYPE"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("DDL_B_PAYMENT_TYPE"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("TB_B_BILL_ACC_NUM"), TextBox).Text.ToString)

                        dhl_package.dhl_loadAddressat("DAJAR SP Z O.O.", "75137", "Koszalin", "Rożana", "9", "0")

                        dhl_package.dhl_loadReceiverAddressat(DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_CITY"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_APART_NUM"), TextBox).Text.ToString, DirectCast(FindControl("DDL_SR_COUNTRY"), DropDownList).SelectedValue.ToString)
                        'ZMIANA ROZMIARU TEL. DO 9 ZNAKOW
                        Dim phone_num As String = DirectCast(FindControl("TB_PC_PHONE_NUM"), TextBox).Text.ToString
                        If phone_num.Length > 9 Then
                            phone_num = phone_num.Substring(0, 9)
                        End If

                        dhl_package.dhl_loadPreavisoContact(phone_num, DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString)

                        If dhl_package.request.shipmentInfo.serviceType <> "EK" Then
                            Dim ss_count As Integer = GridView_SS_Service_type.Rows.Count
                            dhl_package.dhl_loadService(ss_count - 1)
                            ss_count = 0
                            For Each gdr As GridViewRow In GridView_SS_Service_type.Rows
                                Dim ss_service_type As String = gdr.Cells(2).Text.ToString
                                Dim ss_service_value As String = gdr.Cells(3).Text.ToString
                                Dim ss_collect As String = gdr.Cells(4).Text.ToString
                                dhl_package.dhl_AddService(ss_count, ss_service_type, ss_service_value, ss_collect)
                                ss_count += 1
                            Next
                        End If

                        Dim package_count As Integer = GridViewPaczki.Rows.Count
                        dhl_package.dhl_loadPackage(package_count - 1)
                        package_count = 0
                        For Each gdr As GridViewRow In GridViewPaczki.Rows
                            Dim firma_id As String = gdr.Cells(3).Text.ToString
                            Dim typ As String = gdr.Cells(4).Text.ToString
                            Dim rodzaj As String = gdr.Cells(5).Text.ToString
                            Dim waga As String = gdr.Cells(6).Text.ToString
                            Dim szer As String = gdr.Cells(7).Text.ToString
                            Dim wys As String = gdr.Cells(8).Text.ToString
                            Dim dlug As String = gdr.Cells(9).Text.ToString
                            Dim ile_opak As String = gdr.Cells(10).Text.ToString
                            If firma_id.Contains("DHL") Then
                                If typ = "paczka" Then : typ = "PACKAGE"
                                ElseIf typ = "paleta" Then : typ = "PALLET"
                                ElseIf typ = "koperta" Then : typ = "ENVELOPE"
                                Else : typ = "PACKAGE"
                                End If

                                If rodzaj = "standard" Then : rodzaj = "0"
                                Else : rodzaj = "1"
                                End If

                                dhl_package.dhl_AddPackage(package_count, typ, ile_opak, waga, szer, wys, dlug, rodzaj)
                                package_count += 1
                            End If
                        Next

                        Dim dhl_label_id As String = dhl_package.dhl_createShipment(dhl_session)
                        If dhl_label_id <> "" Then
                            sqlexp = "update dp_swm_mia_paczka set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & dhl_label_id & "',shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            For Each row As GridViewRow In GridViewZamowienia.Rows
                                Dim nr_zam As String = row.Cells(2).Text.ToString
                                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), dhl_label_id, nr_zam)
                            Next

                            ''zmiana numeru listu przewozowego
                            Session("shipment_id") = dhl_label_id
                            LNr_list_przewozowy.Text = Session("shipment_id")
                            LadujDaneGridViewZamowienia()
                            LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Komunikat DHL : " & dhl_package.exception.ToString & "<br />"
                            Session(session_id) += "</div>"
                        End If
                    End If
                ElseIf Session("firma_id") = "DHL_PS" Then

                    Dim dhl_parcel_session As ServicePointMethodsService = dhl_parcelshop.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    dhl_parcelshop.dhl_loadRequestInfo(DirectCast(FindControl("DDL_CONTENT_DHL_PS"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("TB_COMMENT_F_DHL_PS"), TextBox).Text.ToString)
                    dhl_parcelshop.dhl_loadShipmentInfo(DirectCast(FindControl("DDL_SERVICE_TYPE_DHL_PS"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("DDL_DROP_OFF_DHL_PS"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("DDL_LABEL_TYPE_DHL_PS"), DropDownList).SelectedValue.ToString)
                    dhl_parcelshop.dhl_loadShipmentTime(DirectCast(FindControl("TB_ST_SHIPMENT_DATE"), TextBox).Text.ToString, DirectCast(FindControl("TB_ST_SHIPMENT_START"), TextBox).Text.ToString, DirectCast(FindControl("TB_ST_SHIPMENT_END"), TextBox).Text.ToString)
                    dhl_parcelshop.dhl_loadBilling(DirectCast(FindControl("DDL_B_SHIP_PAYMENT_TYPE"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("DDL_B_PAYMENT_TYPE"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("TB_B_BILL_ACC_NUM"), TextBox).Text.ToString)

                    dhl_parcelshop.dhl_loadAddressat("DAJAR SP Z O.O.", "75137", "Koszalin", "Rożana", "9", "0")

                    dhl_parcelshop.dhl_loadReceiverAddressat(DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_CITY"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_APART_NUM"), TextBox).Text.ToString, DirectCast(FindControl("DDL_SR_COUNTRY"), DropDownList).SelectedValue.ToString)
                    'ZMIANA ROZMIARU TEL. DO 9 ZNAKOW
                    Dim phone_num As String = DirectCast(FindControl("TB_PC_PHONE_NUM"), TextBox).Text.ToString
                    If phone_num.Length > 9 Then
                        phone_num = phone_num.Substring(0, 9)
                    End If

                    dhl_parcelshop.dhl_loadPreavisoContact(phone_num, DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString)

                    If dhl_parcelshop.request.shipmentData.shipmentInfo.serviceType <> "EK" Then
                        Dim ss_count As Integer = GridView_SS_Service_type.Rows.Count
                        dhl_parcelshop.dhl_loadService(ss_count - 1)
                        ss_count = 0
                        For Each gdr As GridViewRow In GridView_SS_Service_type.Rows
                            Dim ss_service_type As String = gdr.Cells(2).Text.ToString
                            Dim ss_service_value As String = gdr.Cells(3).Text.ToString
                            Dim ss_collect As String = gdr.Cells(4).Text.ToString
                            dhl_parcelshop.dhl_AddService(ss_count, ss_service_type, ss_service_value, ss_collect)
                            ss_count += 1
                        Next
                    End If

                    Dim package_count As Integer = GridViewPaczki.Rows.Count
                    dhl_parcelshop.dhl_loadPackage(package_count - 1)
                    package_count = 0
                    For Each gdr As GridViewRow In GridViewPaczki.Rows
                        Dim firma_id As String = gdr.Cells(3).Text.ToString
                        Dim typ As String = gdr.Cells(4).Text.ToString
                        Dim rodzaj As String = gdr.Cells(5).Text.ToString
                        Dim waga As String = gdr.Cells(6).Text.ToString
                        Dim szer As String = gdr.Cells(7).Text.ToString
                        Dim wys As String = gdr.Cells(8).Text.ToString
                        Dim dlug As String = gdr.Cells(9).Text.ToString
                        Dim ile_opak As String = gdr.Cells(10).Text.ToString
                        If firma_id.Contains("DHL") Then
                            If typ = "paczka" Then : typ = "PACKAGE"
                            ElseIf typ = "paleta" Then : typ = "PALLET"
                            ElseIf typ = "koperta" Then : typ = "ENVELOPE"
                            Else : typ = "PACKAGE"
                            End If

                            If rodzaj = "standard" Then : rodzaj = "0"
                            Else : rodzaj = "1"
                            End If

                            dhl_parcelshop.dhl_AddPackage(package_count, typ, ile_opak, waga, szer, wys, dlug, rodzaj)
                            package_count += 1
                        End If
                    Next


                    dhl_parcelshop.dhl_loadServicePointAccountNumber(DirectCast(FindControl("TB_SR_POST_NUM"), TextBox).Text.ToString)


                    Dim dhlpp() As PointStructure = dhl_parcelshop.dhl_getNearestServicePoint(dhl_parcel_session, dhl_parcelshop.request.shipmentData.ship.receiver.address.city.ToString, dhl_parcelshop.request.shipmentData.ship.receiver.address.postcode.ToString, "1")

                    Dim dhl_label_id As String = dhl_parcelshop.dhl_createShipment(dhl_parcel_session)
                    If dhl_label_id <> "" Then
                        sqlexp = "update dp_swm_mia_paczka set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & dhl_label_id & "',shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        For Each row As GridViewRow In GridViewZamowienia.Rows
                            Dim nr_zam As String = row.Cells(2).Text.ToString
                            result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), dhl_label_id, nr_zam)
                        Next

                        ''zmiana numeru listu przewozowego
                        Session("shipment_id") = dhl_label_id
                        LNr_list_przewozowy.Text = Session("shipment_id")
                        LadujDaneGridViewZamowienia()
                        LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Komunikat DHL POP: " & dhl_parcelshop.exception.ToString & "<br />"
                        Session(session_id) += "</div>"
                    End If

                ElseIf Session("firma_id") = "DHL_DE" Then

                    dhlDESoapRequest.dhl_de_login(Session("schemat_dyspo"), Session("firma_id"))
                    dhlDESoapRequest.StartWebReference()

                    Dim package_count As Integer = 0
                    For Each gdr As GridViewRow In GridViewPaczki.Rows
                        Dim firma_id As String = gdr.Cells(3).Text.ToString
                        If firma_id.Contains("DHL_DE") Then
                            package_count += 1
                        End If
                    Next

                    sqlexp = "select count(*) from dp_swm_mia_paczka where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "'"
                    Dim ile_paczek_obj As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                    For i = 1 To (package_count - ile_paczek_obj)
                        Dim new_shipment_id As String = Session("shipment_id").ToString.Replace("SH", "S" & i)
                        sqlexp = "insert into dp_swm_mia_paczka select NR_ZAMOW, SCHEMAT, AUTODATA, '" & new_shipment_id & "' SHIPMENT_ID, B_SHIP_PAYMENT_TYPE, B_BILL_ACC_NUM, B_PAYMENT_TYPE, B_COSTS_CENTER, ST_SHIPMENT_DATE, ST_SHIPMENT_START, ST_SHIPMENT_END, SR_COUNTRY, SR_IS_PACKSTATION, SR_IS_POSTFILIALE, SR_POST_NUM, SR_NAME, SR_POSTAL_CODE, SR_CITY, SR_STREET, SR_HOUSE_NUM, SR_APART_NUM, PC_PERSON_NAME, PC_PHONE_NUM, PC_EMAIL_ADD from dp_swm_mia_paczka where shipment_id='" & Session("shipment_id") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        sqlexp = "insert into dp_swm_mia_paczka_base select SCHEMAT,'" & new_shipment_id & "' SHIPMENT_ID,DROP_OFF,SERVICE_TYPE,LABEL_TYPE,CONTENT,COMMENT_F,REFERENCE FROM dp_swm_mia_paczka_base where shipment_id='" & Session("shipment_id") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        sqlexp = "insert into dp_swm_mia_paczka_ss select SCHEMAT,'" & new_shipment_id & "' SHIPMENT_ID, SS_SERVICE_TYPE, SS_SERVICE_VALUE, SS_TEXT_INSTR, SS_COLL_ON_FORM from dp_swm_mia_paczka_ss where shipment_id='" & Session("shipment_id") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    Next

                    'aktualizacja numerow shipment_id dla paczka_info
                    Dim package_id As Integer = 0
                    sqlexp = "select shipment_id,paczka_id from dp_swm_mia_paczka_info where shipment_id='" & Session("shipment_id") & "' and schemat='" & Session("schemat_dyspo") & "' order by paczka_id"
                    Dim cmd As New OracleCommand(sqlexp, conn)
                    cmd.CommandType = CommandType.Text
                    Dim dr As OracleDataReader = cmd.ExecuteReader()
                    Try
                        While dr.Read()
                            If package_id > 0 Then
                                Dim paczka_id As String = dr.Item(1).ToString
                                Dim new_shipment_id As String = Session("shipment_id").ToString.Replace("SH", "S" & package_id)
                                sqlexp = "update dp_swm_mia_paczka_info set shipment_id='" & new_shipment_id & "' where paczka_id='" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            End If
                            package_id += 1
                        End While
                    Catch ex As System.ArgumentOutOfRangeException
                        Console.WriteLine(ex)
                    End Try
                    dr.Close()
                    cmd.Dispose()

                    package_id = 0
                    For Each gdr As GridViewRow In GridViewPaczki.Rows
                        Dim shipmentPackage As New List(Of ShipmentItemDDType)
                        Dim paczka_id As String = gdr.Cells(2).Text.ToString
                        Dim firma_id As String = gdr.Cells(3).Text.ToString
                        Dim typ As String = gdr.Cells(4).Text.ToString
                        Dim waga As String = gdr.Cells(6).Text.ToString
                        Dim ile_opak As String = gdr.Cells(10).Text.ToString
                        If firma_id.Contains("DHL_DE") Then
                            Dim new_shipment_id As String = ""
                            If package_id = 0 Then : new_shipment_id = shippment_id
                            Else : new_shipment_id = shippment_id.ToString.Replace("SH", "S" & package_id)
                            End If
                            package_id += 1

                            Dim package As ShipmentItemDDType = dhlDESoapRequest.dhlcreateSoapShipmentItemDDType(waga, "1", "1", "1", "PK")
                            shipmentPackage.Add(package)

                            Dim servicePackage As New List(Of ShipmentServiceDD)
                            ''Dim service As ShipmentServiceDD
                            Dim bankItem As BankType = dhlDESoapRequest.dhlcreateSoapBankDataType("", "", "", "", "", "")
                            Dim dhlshipmentPackage As New dhlDESoapRequest.dhlshipmentItemDDList(DirectCast(FindControl("TB_ST_SHIPMENT_DATE"), TextBox).Text.ToString, DirectCast(FindControl("TB_COMMENT_F"), TextBox).Text.ToString, "", shipmentPackage, servicePackage, bankItem)

                            Dim receiver As New dhlDESoapRequest.dhlDESoapReceiver("", "", "", "", "", "", "", "", "", "", "")
                            Dim receiverWWide As New dhlDESoapRequest.dhlDESoapReceiverWWide("", "", "", "", "", "")

                            sqlexp = "select distinct service_type from dp_swm_mia_paczka_base where shipment_id='" & new_shipment_id & "'"
                            Dim service_type As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                            Dim imie As String = ""
                            Dim nazwisko As String = ""

                            ''ZAWIERA ROZSZERZONY OPIS DODATKOWY DO 50ZNAKOW
                            imie = DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString.Split(" ")(0).ToString
                            If DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString.Split(" ").Length > 1 Then
                                nazwisko = DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString.Split(" ")(1).ToString
                            End If

                            If service_type = "BPI" Then
                                Dim countryName As String = dhlDESoapRequest.dhlGetCountryLanguageName(DDL_SR_COUNTRY.SelectedValue.ToString)
                                receiver = New dhlDESoapRequest.dhlDESoapReceiver(imie, nazwisko, DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_CITY"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, DirectCast(FindControl("DDL_SR_COUNTRY"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("TB_PC_EMAIL_ADD"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PERSON_NAME"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PHONE_NUM"), TextBox).Text.ToString, "")
                                receiverWWide = New dhlDESoapRequest.dhlDESoapReceiverWWide(DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_CITY"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, countryName, DirectCast(FindControl("DDL_SR_COUNTRY"), DropDownList).SelectedValue.ToString)
                            ElseIf service_type = "EPN" Then
                                receiver = New dhlDESoapRequest.dhlDESoapReceiver(imie, nazwisko, DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_CITY"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, DirectCast(FindControl("DDL_SR_COUNTRY"), DropDownList).SelectedValue.ToString, DirectCast(FindControl("TB_PC_EMAIL_ADD"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PERSON_NAME"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PHONE_NUM"), TextBox).Text.ToString, "")
                                receiverWWide = New dhlDESoapRequest.dhlDESoapReceiverWWide("", "", "", "", "", "")
                            End If

                            Dim is_packstation As Boolean = False
                            Dim is_postfile As Boolean = False
                            If CB_SR_IS_PACKSTATION.Checked Then
                                is_packstation = True
                            ElseIf CB_SR_IS_POSTFILIALE.Checked Then
                                is_postfile = True
                            End If

                            Dim ddRequest As CreateShipmentDDRequest = dhlDESoapRequest.createDefaultShipmentDDRequest(DirectCast(FindControl("DDL_SERVICE_TYPE"), DropDownList).SelectedValue.ToString, receiver, receiverWWide, dhlshipmentPackage, is_packstation, is_postfile, DirectCast(FindControl("TB_SR_POST_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, Session("schemat_dyspo"))

                            Try
                                Dim shResponse As dhlDEAPI.CreateShipmentResponse = dhlDESoapRequest.webService.createShipmentDD(dhlDESoapRequest.auth, ddRequest)

                                Dim status As Statusinformation = shResponse.status
                                Dim statusCode As String = status.StatusCode
                                Dim statusMessage As String = status.StatusMessage & vbNewLine

                                If shResponse.CreationState IsNot Nothing Then
                                    Dim msg_length As Integer = shResponse.CreationState(0).StatusMessage.Length - 1
                                    For i = 0 To msg_length
                                        statusMessage &= shResponse.CreationState(0).StatusMessage(i).ToString & vbNewLine
                                    Next
                                End If

                                If statusCode = "0" Then
                                    Dim dhl_label_id As String = shResponse.CreationState(0).ShipmentNumber.Item
                                    Dim labelURL As String = shResponse.CreationState(0).Labelurl.ToString
                                    Dim client As WebClient = New WebClient()
                                    Dim bytes = client.DownloadData(labelURL)

                                    Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/dhl_de/"
                                    dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)

                                    Dim labelDirectLink As String = dir_export & "\" & dhl_label_id.ToString & ".pdf"
                                    File.WriteAllBytes(labelDirectLink, bytes)

                                    Console.WriteLine("CreateShipmentDDRequest:")
                                    Console.WriteLine("Request Status: Code: " + statusCode)
                                    Console.WriteLine("Status-Nachricht: " + statusMessage)
                                    Console.WriteLine("Label URL: " + labelURL)
                                    Console.WriteLine("Shipmentnumber: " + dhl_label_id)

                                    sqlexp = "update dp_swm_mia_paczka set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & new_shipment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & new_shipment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & dhl_label_id & "',shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & new_shipment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' AND PACZKA_ID='" & paczka_id & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & dhl_label_id & "' WHERE shipment_id='" & new_shipment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                                    For Each row As GridViewRow In GridViewZamowienia.Rows
                                        Dim nr_zam As String = row.Cells(2).Text.ToString
                                        result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), dhl_label_id, nr_zam)
                                    Next

                                    ''zmiana numeru listu przewozowego
                                    Session("shipment_id") = dhl_label_id

                                Else
                                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                                    Session(session_id) += "<br />Komunikat DHL : " & statusMessage.ToString & "<br />"
                                    Session(session_id) += "</div>"
                                    Exit For
                                End If
                            Catch ex As Exception
                                Console.WriteLine(ex.Message.ToString)
                                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                                Session(session_id) += "<br />Komunikat DHL : " & ex.Message.ToString & "<br />"
                                Session(session_id) += "</div>"
                                Exit For
                            End Try

                        End If
                    Next

                    LNr_list_przewozowy.Text = Session("shipment_id")
                    LadujDaneGridViewZamowienia()
                    LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BGenerujEtykiete_DHL_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BGenerujEtykiete_DHL_DE_API.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session.Remove("contentKomunikat")
            Dim przerwijGenerowanie As Boolean = False
            For Each gdr As GridViewRow In GridViewPaczki.Rows
                Dim firma_id As String = gdr.Cells(3).Text.ToString
                If firma_id.Contains("DHL") = False Then
                    przerwijGenerowanie = True
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Wprowadz pole firma_id dla wszystkich utworzonych paczek!<br />"
                    Session(session_id) += "</div>"
                    Exit For
                End If
            Next

            sqlexp = "select count(*) from dp_swm_mia_paczka where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and shipment_id not like 'S%'"
            Dim ile_ship_dhl As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            ile_ship_dhl = "0"
            If ile_ship_dhl <> "0" Then
                przerwijGenerowanie = True
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Dla wybranych paczek istnieje juz utworzona etykieta DHL!<br />"
                Session(session_id) += "</div>"
            End If

            If przerwijGenerowanie = False Then
                Dim shippment_id As String = Session("shipment_id").ToString
                ''shippment_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                If Session("firma_id") = "DHL_DE_API" Then
                    If DirectCast(FindControl("DDL_SERVICE_TYPE_DHL_DE_API"), DropDownList).SelectedValue.ToString <> "" Then

                        Dim dhlde_session As New dhl_parcel_de_package.DHL_DE_Session
                        dhl_parcel_de_package.dhl_login(dhlde_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))

                        Dim company As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_company from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)
                        Dim first_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_firstname from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)
                        Dim last_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_lastname from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)

                        dhlde_session.i_shipment.i_receiver = dhl_parcel_de_package.dhl_loadReceiver(DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PERSON_NAME"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_COMPANY_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_FIRSTNAME_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_LASTNAME_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_EMAIL_ADD"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PHONE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_City"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, DirectCast(FindControl("DDL_SR_COUNTRY"), DropDownList).SelectedValue.ToString)
                        dhlde_session.i_shipment.i_receiver.email = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select pc_email_add from dp_swm_mia_paczka where shipment_id='" & shippment_id & "'", conn)

                        If CB_SR_IS_PACKSTATION.Checked = True Then
                            dhlde_session.packStation = DirectCast(FindControl("TB_SR_POST_NUM"), TextBox).Text.ToString
                        End If

                        For Each gdr As GridViewRow In GridView_SS_Service_type_DHL_DE_API.Rows
                            Dim ss_service_type As String = gdr.Cells(2).Text.ToString
                            Dim ss_service_value As String = gdr.Cells(3).Text.ToString
                            If ss_service_type = "insurance" Then
                                dhlde_session.i_shipment.i_insurance = dhl_parcel_de_package.dhl_loadInsurance(ss_service_value.ToString, "PLN")
                            ElseIf ss_service_type = "cod" Then
                                dhlde_session.i_shipment.i_cod = dhl_parcel_de_package.dhl_loadCod(ss_service_value.ToString, "PLN")
                            Else
                                dhlde_session.i_shipment.i_additional_services.Add(ss_service_type)
                            End If
                        Next

                        ''Dim package_count As Integer = GridViewPaczki.Rows.Count
                        ''package_count = 0
                        For Each gdr As GridViewRow In GridViewPaczki.Rows
                            Dim firma_id As String = gdr.Cells(3).Text.ToString
                            Dim typ As String = gdr.Cells(4).Text.ToString
                            Dim rodzaj As String = gdr.Cells(5).Text.ToString
                            Dim waga As String = gdr.Cells(6).Text.ToString
                            Dim szer As String = gdr.Cells(7).Text.ToString
                            Dim wys As String = gdr.Cells(8).Text.ToString
                            Dim dlug As String = gdr.Cells(9).Text.ToString
                            Dim ile_opak As String = gdr.Cells(10).Text.ToString
                            Dim is_non_standard As String = gdr.Cells(11).Text.ToString
                            If is_non_standard = "0" Then : is_non_standard = "false"
                            ElseIf is_non_standard = "1" Then : is_non_standard = "true"
                            End If

                            Dim inpost_paczka_id As String = gdr.RowIndex.ToString
                            dhlde_session.i_shipment.i_parcels.Add(dhl_parcel_de_package.dhl_loadParcel("", inpost_paczka_id, dlug * 10, szer * 10, wys * 10, "mm", waga, "kg", is_non_standard))

                        Next

                        dhlde_session.i_shipment.i_service = DDL_SERVICE_TYPE_DHL_DE_API.SelectedValue.ToString
                        dhlde_session.i_shipment.i_comments = ""
                        dhlde_session.i_shipment.i_mpk = ""
                        dhlde_session.i_shipment.i_reference = TB_COMMENT_F_DHL_DE_API.Text.ToString

                        Dim inpost_custom As New dhl_parcel_de_package.CustomAttributes
                        inpost_custom.target_point = TB_SR_POST_NUM.Text.ToString
                        dhlde_session.i_shipment.i_custom_attributes = inpost_custom

                        ''####2022.08.04 / tworzenie etykiety w trybie ofertowym / mozliwosc pozniejszej edycji oraz usuniecia
                        dhlde_session.i_shipment.i_only_choice_of_offer = "true"
                        Dim dhl_label As dhl_parcel_de_package.ShipmentResponse = dhl_parcel_de_package.REST_POST_CreateShipment(dhlde_session, dhlde_session.i_shipment)
                        ''shipment_id = "11706911"
                        If dhl_label.shipment_id <> "" And dhl_label.shipment_id <> "1111" Then
                            ''Dim inpost_label_id As String = dhl_parcel_de_package.REST_GET_ParcelTrackingNumber(shipment_id)
                            ''If inpost_label_id = "" Then
                            ''    inpost_label_id = shipment_id
                            ''End If

                            sqlexp = "update dp_swm_mia_paczka set shipment_id='" & dhl_label.shipment_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & dhl_label.shipment_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            If dhl_label.tracking_number.Count = GridViewPaczki.Rows.Count Then
                                Dim paczka_pozycja_id As Integer = 0
                                For Each obj_track In dhl_label.tracking_number
                                    Dim tracking_number As String = obj_track.ToString
                                    Dim paczka_id As String = GridViewPaczki.Rows(paczka_pozycja_id).Cells(2).Text.ToString
                                    sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & tracking_number & "',shipment_id='" & dhl_label.shipment_id & "' WHERE shipment_id like '" & shippment_id.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id='" & paczka_id & "'"

                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    paczka_pozycja_id += 1

                                    ''##2024.05.29 / automatycznie drukowanie etykiet dla DHL_DE_API
                                    ''###############################################################
                                    ''WebClientPrint.LicenseOwner = "Dajar Sp. z o.o. - 1 WebApp Lic - 1 WebServer Lic"
                                    ''WebClientPrint.LicenseKey = "273D8F1AB12B42847C2E9FE7717B6756A2CA0026"

                                    ''Application.Lock()
                                    ''Application("session_id_" + Session.SessionID) = Session.SessionID.ToString
                                    ''Application("shipment_id_" + Session.SessionID) = Session("shipment_id").ToString
                                    ''Application("tracking_number_" + Session.SessionID) = tracking_number
                                    ''Application("kod_dyspo_" + Session.SessionID) = Session("kod_dyspo").ToString
                                    ''Application("firma_id_" + Session.SessionID) = Session("firma_id").ToString
                                    ''Application("mylogin_" + Session.SessionID) = Session("mylogin").ToString
                                    ''Application("myhash_" + Session.SessionID) = Session("myhash").ToString
                                    ''Application.UnLock()

                                    ''dajarSWMagazyn_MIA.MyFunction.SetPrintDyspozycja(dajarSWMagazyn_MIA.MyFunction.GetRemoteIp, Session.SessionID.ToString, Session("kod_dyspo").ToString, Session("mylogin").ToString, Session("myhash").ToString, tracking_number)
                                    ''ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "script", "javascript:jsWebClientPrint.print('useDefaultPrinter=' + $('#useDefaultPrinter').attr('checked') + '&printerName=' + $('#installedPrinterName').val() + '&sessionId=" + Session.SessionID + "');", True)
                                Next
                            End If

                            sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & dhl_label.shipment_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            For Each row As GridViewRow In GridViewZamowienia.Rows
                                Dim nr_zam As String = row.Cells(2).Text.ToString
                                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), dhl_label.shipment_id, nr_zam)
                            Next

                            ''zmiana numeru listu przewozowego
                            Session("shipment_id") = dhl_label.shipment_id
                            LNr_list_przewozowy.Text = Session("shipment_id")
                            LadujDaneGridViewZamowienia()
                            LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))

                            ''HLEtykietaPDF_DHL_DE_API.NavigateUrl = "http://10.1.0.64:8084/upload/dhl_de/" & dhl_label.shipment_id & ".pdf"
                            ''HLEtykietaPDF_DHL_DE_API.Text = dhl_label.shipment_id

                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Komunikat DHL_DE_API : " & dhl_parcel_de_package.exception.ToString & "<br />"
                            Session(session_id) += "</div>"
                        End If

                    End If
                End If
            End If
            conn.Close()
        End Using
    End Sub

    ''Protected Sub BImageGIF_TO_PDF_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BImageGIF_TO_PDF.Click
    ''    If Session("tracking_number") IsNot Nothing Then
    ''        Dim tracking_number As String = Session("tracking_number")
    ''        Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/" & Session("firma_id").replace("_API", "") & "/"
    ''        Dim file_in As String = dir_export & "\" & tracking_number.ToString & ".gif"
    ''        Dim file_out As String = dir_export & "\" & tracking_number.ToString & ".pdf"
    ''        dajarSWMagazyn_MIA.MyFunction.IMAGE_CONVERT_GIF_to_PDF(file_in, file_out)
    ''    End If
    ''End Sub

    Protected Sub BGenerujEtykiete_UPS_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BGenerujEtykiete_UPS_DE_API.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session.Remove("contentKomunikat")
            Dim przerwijGenerowanie As Boolean = False
            For Each gdr As GridViewRow In GridViewPaczki.Rows
                Dim firma_id As String = gdr.Cells(3).Text.ToString
                If firma_id.Contains("UPS") = False Then
                    przerwijGenerowanie = True
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Wprowadz pole firma_id dla wszystkich utworzonych paczek!<br />"
                    Session(session_id) += "</div>"
                    Exit For
                End If
            Next

            sqlexp = "select count(*) from dp_swm_mia_paczka where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and shipment_id not like 'S%'"
            Dim ile_ship_dhl As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            ile_ship_dhl = "0"
            If ile_ship_dhl <> "0" Then
                przerwijGenerowanie = True
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Dla wybranych paczek istnieje juz utworzona etykieta UPS!<br />"
                Session(session_id) += "</div>"
            End If

            If przerwijGenerowanie = False Then
                Dim shippment_id As String = Session("shipment_id").ToString
                ''shippment_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                If Session("firma_id") = "UPS_DE_API" Then
                    If DirectCast(FindControl("DDL_SERVICE_TYPE_UPS_DE_API"), DropDownList).SelectedValue.ToString <> "" Then

                        Dim ups_session As New ups_de_package.UPS_DE_Session
                        ups_de_package.ups_login(ups_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))
                        ups_de_package.ups_token(ups_session)
                        Dim company As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_company from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)
                        Dim first_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_firstname from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)
                        Dim last_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_lastname from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)

                        ups_session.i_shipment.i_receiver = ups_de_package.dhl_loadReceiver(DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PERSON_NAME"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_COMPANY_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_FIRSTNAME_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_LASTNAME_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_EMAIL_ADD"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PHONE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_City"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, DirectCast(FindControl("DDL_SR_COUNTRY"), DropDownList).SelectedValue.ToString)
                        ups_session.i_shipment.i_receiver.email = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select pc_email_add from dp_swm_mia_paczka where shipment_id='" & shippment_id & "'", conn)

                        If CB_SR_IS_PACKSTATION.Checked = True Then
                            ups_session.packStation = DirectCast(FindControl("TB_SR_POST_NUM"), TextBox).Text.ToString
                        End If

                        For Each gdr As GridViewRow In GridView_SS_Service_type_UPS_DE_API.Rows
                            Dim ss_service_type As String = gdr.Cells(2).Text.ToString
                            Dim ss_service_value As String = gdr.Cells(3).Text.ToString
                            If ss_service_type = "insurance" Then
                                ups_session.i_shipment.i_insurance = ups_de_package.dhl_loadInsurance(ss_service_value.ToString, "PLN")
                            ElseIf ss_service_type = "cod" Then
                                ups_session.i_shipment.i_cod = ups_de_package.dhl_loadCod(ss_service_value.ToString, "PLN")
                            Else
                                ups_session.i_shipment.i_additional_services.Add(ss_service_type)
                            End If
                        Next

                        ''Dim package_count As Integer = GridViewPaczki.Rows.Count
                        ''package_count = 0
                        For Each gdr As GridViewRow In GridViewPaczki.Rows
                            Dim firma_id As String = gdr.Cells(3).Text.ToString
                            Dim typ As String = gdr.Cells(4).Text.ToString
                            Dim rodzaj As String = gdr.Cells(5).Text.ToString
                            Dim waga As String = gdr.Cells(6).Text.ToString
                            Dim szer As String = gdr.Cells(7).Text.ToString
                            Dim wys As String = gdr.Cells(8).Text.ToString
                            Dim dlug As String = gdr.Cells(9).Text.ToString
                            Dim ile_opak As String = gdr.Cells(10).Text.ToString
                            Dim is_non_standard As String = gdr.Cells(11).Text.ToString
                            If is_non_standard = "0" Then : is_non_standard = "false"
                            ElseIf is_non_standard = "1" Then : is_non_standard = "true"
                            End If

                            ''<asp:ListItem Value = "01" > UPS Letter</asp: ListItem>
                            ''<asp:ListItem Value="02" Selected="True">Moje opakowanie</asp:ListItem>
                            ''<asp:ListItem Value = "03" > UPS Tube</asp: ListItem>
                            ''<asp:ListItem Value="04">UPS PAK</asp:ListItem>
                            ''<asp:ListItem Value = "21" > UPS Express Box</asp: ListItem>
                            ''<asp:ListItem Value="30">UPS Paleta</asp:ListItem>

                            If typ = "UPS Letter" Then : typ = "01"
                            ElseIf typ = "Moje opakowanie" Then : typ = "02"
                            ElseIf typ = "UPS Tube" Then : typ = "03"
                            ElseIf typ = "UPS PAK" Then : typ = "04"
                            ElseIf typ = "UPS Express Box" Then : typ = "21"
                            ElseIf typ = "UPS Paleta" Then : typ = "30"
                            End If

                            Dim inpost_paczka_id As String = gdr.RowIndex.ToString
                            ups_session.i_shipment.i_parcels.Add(ups_de_package.dhl_loadParcel(typ, inpost_paczka_id, dlug, szer, wys, "cm", waga, "kg", is_non_standard))

                        Next

                        ups_session.i_shipment.i_service = DDL_SERVICE_TYPE_UPS_DE_API.SelectedValue.ToString
                        ups_session.i_shipment.i_comments = DDL_CONTENT_UPS_DE_API.SelectedValue.ToString
                        ups_session.i_shipment.i_mpk = ""
                        ups_session.i_shipment.i_reference = TB_COMMENT_F_UPS_DE_API.Text.ToString

                        Dim inpost_custom As New ups_de_package.CustomAttributes
                        inpost_custom.target_point = TB_SR_POST_NUM.Text.ToString
                        ups_session.i_shipment.i_custom_attributes = inpost_custom

                        ''####2022.08.04 / tworzenie etykiety w trybie ofertowym / mozliwosc pozniejszej edycji oraz usuniecia
                        ups_session.i_shipment.i_only_choice_of_offer = "true"
                        ''Dim uuid As String = System.Guid.NewGuid.ToString()
                        Dim ups_label As ups_de_package.ShipmentResponse = ups_de_package.REST_POST_CreateShipment(ups_session, ups_session.i_shipment, "production", CBWalidacjaUlicaUPS.Checked)
                        ''shipment_id = "11706911"
                        If ups_label.shipment_id <> "" And ups_label.shipment_id.Length <> 4 Then
                            ''Dim inpost_label_id As String = dhl_parcel_de_package.REST_GET_ParcelTrackingNumber(shipment_id)
                            ''If inpost_label_id = "" Then
                            ''    inpost_label_id = shipment_id
                            ''End If

                            sqlexp = "update dp_swm_mia_paczka set shipment_id='" & ups_label.shipment_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & ups_label.shipment_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            If ups_label.tracking_number.Count = GridViewPaczki.Rows.Count Then
                                Dim paczka_pozycja_id As Integer = 0
                                For Each obj_track In ups_label.tracking_number
                                    Dim tracking_number As String = obj_track.ToString
                                    Dim paczka_id As String = GridViewPaczki.Rows(paczka_pozycja_id).Cells(2).Text.ToString
                                    sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & tracking_number & "',shipment_id='" & ups_label.shipment_id & "' WHERE shipment_id like '" & shippment_id.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id='" & paczka_id & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    paczka_pozycja_id += 1

                                    ''##2024.05.28 / utworzenie nowych wersji etykiet PDF na podstawie GIF
                                    ''Dim ups_reference As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select comment_f from dp_swm_mia_paczka_base where shipment_id='" & obj_track & "' and comment_f like '%" & Session("kod_dyspo") & "%'", conn)
                                    ''Dim inpost_label As String = ups_de_package.REST_GET_LabelOfShipment(ups_session, obj_track.ToString, ups_reference, "testing")
                                    Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/" & Session("firma_id").replace("_API", "") & "/"
                                    Dim file_in As String = dir_export & "\" & obj_track.ToString & ".gif"
                                    Dim file_out As String = dir_export & "\" & obj_track.ToString & ".pdf"
                                    dajarSWMagazyn_MIA.MyFunction.IMAGE_CONVERT_GIF_to_PDF(file_in, file_out)

                                    ''##2024.05.29 / automatycznie drukowanie etykiet dla UPS_DE_API
                                    ''###############################################################
                                    ''''WebClientPrint.LicenseOwner = "Dajar Sp. z o.o. - 1 WebApp Lic - 1 WebServer Lic"
                                    ''''WebClientPrint.LicenseKey = "273D8F1AB12B42847C2E9FE7717B6756A2CA0026"

                                    ''''Application.Lock()
                                    ''''Application("session_id_" + Session.SessionID) = Session.SessionID.ToString
                                    ''''Application("shipment_id_" + Session.SessionID) = Session("shipment_id").ToString
                                    ''''Application("tracking_number_" + Session.SessionID) = tracking_number
                                    ''''Application("kod_dyspo_" + Session.SessionID) = Session("kod_dyspo").ToString
                                    ''''Application("firma_id_" + Session.SessionID) = Session("firma_id").ToString
                                    ''''Application("mylogin_" + Session.SessionID) = Session("mylogin").ToString
                                    ''''Application("myhash_" + Session.SessionID) = Session("myhash").ToString
                                    ''''Application.UnLock()

                                    ''''dajarSWMagazyn_MIA.MyFunction.SetPrintDyspozycja(dajarSWMagazyn_MIA.MyFunction.GetRemoteIp, Session.SessionID.ToString, Session("kod_dyspo").ToString, Session("mylogin").ToString, Session("myhash").ToString, tracking_number)
                                    ''''ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "script", "javascript:jsWebClientPrint.print('useDefaultPrinter=' + $('#useDefaultPrinter').attr('checked') + '&printerName=' + $('#installedPrinterName').val() + '&sessionId=" + Session.SessionID + "');", True)


                                Next
                            End If

                            sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & ups_label.shipment_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            For Each row As GridViewRow In GridViewZamowienia.Rows
                                Dim nr_zam As String = row.Cells(2).Text.ToString
                                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), ups_label.shipment_id, nr_zam)
                            Next

                            ''zmiana numeru listu przewozowego
                            Session("shipment_id") = ups_label.shipment_id
                            LNr_list_przewozowy.Text = Session("shipment_id")
                            LadujDaneGridViewZamowienia()
                            LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))

                            ''HLEtykietaPDF_UPS_DE_API.NavigateUrl = "http://10.1.0.64:8084/upload/ups_de/" & ups_label.shipment_id & ".gif"
                            ''HLEtykietaPDF_UPS_DE_API.Text = ups_label.shipment_id
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Komunikat UPS_DE_API : " & ups_de_package.exception.ToString & "<br />"
                            Session(session_id) += "</div>"
                        End If

                    End If
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BGenerujEtykiete_GEIS_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BGenerujEtykiete_GEIS.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session.Remove("contentKomunikat")
            Dim przerwijGenerowanie As Boolean = False
            For Each gdr As GridViewRow In GridViewPaczki.Rows
                Dim firma_id As String = gdr.Cells(3).Text.ToString
                If firma_id.Contains("GEIS") = False Then
                    przerwijGenerowanie = True
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Wprowadz pole firma_id dla wszystkich utworzonych paczek!<br />"
                    Session(session_id) += "</div>"
                    Exit For
                End If
            Next

            sqlexp = "select count(*) from dp_swm_mia_paczka where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and shipment_id not like 'S%'"
            Dim ile_ship_dhl As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            If ile_ship_dhl <> "0" Then
                przerwijGenerowanie = True
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Dla wybranych paczek istnieje juz utworzona etykieta GEIS!<br />"
                Session(session_id) += "</div>"
            End If

            If przerwijGenerowanie = False Then
                Dim shippment_id As String = Session("shipment_id").ToString
                ''shippment_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                If Session("firma_id").ToString.Contains("GEIS") Then

                    If DirectCast(FindControl("DDL_SERVICE_TYPE_GEIS"), DropDownList).SelectedValue.ToString <> "" Then
                        Dim geis_session As GService.GServiceClient = geis_package.geis_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))

                        ''GEIS_DAJAR_KRAJ
                        ''GEIS_HIPO
                        ''GEIS_COD_CZECHY
                        ''GEIS_COD_SLOWACJA

                        If Session("firma_id") = "GEIS_HIPO" Then
                            geis_package.geis_loadCoverAddress("Dajar GMBH", "c/o Schenker Deutschland AG TNL Herr Thews, Frau Lenz", "Schwanenfeldstraße 11", "Berlin", "13627", "DE")
                        ElseIf Session("firma_id") = "GEIS_DAJAR_KRAJ" Then
                            geis_package.geis_loadCoverAddress("DAJAR KRAJ", "", "Różana 9", "Koszalin", "75072", "PL")
                            ''ElseIf Session("firma_id") = "GEIS_HIPO" Then
                            geis_package.geis_loadCoverAddress("Magazyn Hipo Dajar", "", "Różana 9", "Koszalin", "75072", "PL")
                        ElseIf Session("firma_id") = "GEIS_COD_CZECHY" Then
                            geis_package.geis_loadCoverAddress("WYSYŁKI COD CZECHY (DAJAR)", "", "Różana 9", "Koszalin", "75072", "PL")
                        ElseIf Session("firma_id") = "GEIS_COD_SLOWACJA" Then
                            geis_package.geis_loadCoverAddress("WYSYŁKI COD SŁOWACJA (DAJAR)", "", "Różana 9", "Koszalin", "75072", "PL")
                        End If

                        geis_package.geis_loadDeliveryAddress(DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_APART_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_City"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, DirectCast(FindControl("DDL_SR_COUNTRY"), DropDownList).SelectedValue.ToString)
                        geis_package.geis_loadDeliveryContact(DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_EMAIL_ADD"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PHONE_NUM"), TextBox).Text.ToString)
                        ''geis_package.geis_loadNote(DirectCast(FindControl("DDL_CONTENT_GEIS"), DropDownList).SelectedValue.ToString)
                        geis_package.geis_loadNoteReference(DirectCast(FindControl("TB_COMMENT_F_GEIS"), TextBox).Text.ToString)
                        ''geis_package.geis_loadNoteDriver("notatka_kierowca")

                        Dim countryISO As String = DDL_SR_COUNTRY.SelectedValue.ToString

                        Dim data_geis As String = DirectCast(FindControl("TB_ST_SHIPMENT_DATE"), TextBox).Text.ToString & "T" & DateTime.Now.ToLongTimeString
                        geis_package.geis_loadPickupDate(data_geis)

                        Dim ss_count As Integer = GridView_SS_Service_type_GEIS.Rows.Count
                        geis_package.geis_loadService(ss_count - 1)
                        ss_count = 0
                        For Each gdr As GridViewRow In GridView_SS_Service_type_GEIS.Rows
                            Dim ss_service_type As String = gdr.Cells(2).Text.ToString
                            Dim ss_service_value As String = gdr.Cells(3).Text.ToString
                            geis_package.geis_AddService(ss_count, ss_service_type, ss_service_value, countryISO)
                            ss_count += 1
                        Next

                        Dim package_count As Integer = GridViewPaczki.Rows.Count
                        geis_package.geis_loadPackage(package_count - 1)
                        package_count = 0
                        For Each gdr As GridViewRow In GridViewPaczki.Rows
                            Dim firma_id As String = gdr.Cells(3).Text.ToString
                            Dim typ As String = gdr.Cells(4).Text.ToString
                            Dim rodzaj As String = gdr.Cells(5).Text.ToString
                            Dim waga As String = gdr.Cells(6).Text.ToString
                            Dim szer As String = gdr.Cells(7).Text.ToString
                            Dim wys As String = gdr.Cells(8).Text.ToString
                            Dim dlug As String = gdr.Cells(9).Text.ToString
                            Dim ile_opak As String = gdr.Cells(10).Text.ToString
                            If firma_id.Contains("GEIS") Then
                                If typ.Contains("[") And typ.Contains("]") Then
                                    Dim typ_t As String = typ.Split("[")(1).ToString.Replace("]", "")
                                    geis_package.geis_AddPackage(package_count, typ_t, ile_opak, waga, szer, wys, dlug, "", "")
                                    package_count += 1
                                End If
                            End If
                        Next

                        Dim geis_label_id As String = geis_package.geis_InsertExport(geis_session)
                        If geis_label_id <> "" Then
                            sqlexp = "update dp_swm_mia_paczka set shipment_id='" & geis_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & geis_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_info set shipment_id='" & geis_label_id & "' WHERE shipment_id like '" & shippment_id.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & geis_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            For Each row As GridViewRow In GridViewZamowienia.Rows
                                Dim nr_zam As String = row.Cells(2).Text.ToString
                                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), geis_label_id, nr_zam)
                            Next

                            ''zmiana numeru listu przewozowego
                            Session("shipment_id") = geis_label_id
                            LNr_list_przewozowy.Text = Session("shipment_id")
                            LadujDaneGridViewZamowienia()
                            LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Komunikat GEIS : " & geis_package.exception.ToString & "<br />"
                            Session(session_id) += "</div>"
                        End If
                    End If

                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BRaportPickup_GEIS_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BRaportPickup_GEIS.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Session.Remove("contentKomunikat")
            If Session("firma_id").ToString.Contains("GEIS") Then
                If Session("shipment_id") <> "" Then
                    Dim data As String = DateTime.Now.ToShortDateString
                    Dim geis_session As GService.GServiceClient = geis_package.geis_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    Dim shipment_list As New List(Of String)
                    sqlexp = "select i.shipment_id from dp_swm_mia_paczka_info i where i.firma_id like '" & Session("firma_id") & "' and i.shipment_id not like 'SH%'"
                    sqlexp = "select i.shipment_id from dp_swm_mia_paczka_info i left join dp_swm_mia_paczka p on p.shipment_id=i.shipment_id where i.firma_id like '" & Session("firma_id") & "' and i.shipment_id not like 'SH%' and p.st_shipment_date like '" & data & "'"

                    Dim cmd As New OracleCommand(sqlexp, conn)
                    cmd.CommandType = CommandType.Text
                    Dim dr As OracleDataReader = cmd.ExecuteReader()
                    Try
                        While dr.Read()
                            shipment_list.Add(dr.Item(0).ToString)
                        End While

                    Catch ex As System.ArgumentOutOfRangeException
                        Console.WriteLine(ex)
                    End Try
                    dr.Close()
                    cmd.Dispose()

                    Dim geis_pickup As String = geis_package.geis_getPickupList(geis_session, Session("firma_id"), data, shipment_list)
                    If geis_pickup.Length <> 4 Then
                        HLRaportPickup_GEIS.NavigateUrl = "http://10.1.0.64:8084/upload/geis/" & geis_pickup
                        HLRaportPickup_GEIS.Text = geis_pickup
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Komunikat GEIS : " & geis_package.exception.ToString & "<br />"
                        Session(session_id) += "</div>"
                    End If

                    ''End If
                End If
            End If

            conn.Close()
        End Using
    End Sub

    Protected Sub BRaportPickup_INPOST_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BRaportPickup_INPOST.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Session.Remove("contentKomunikat")
            If Session("firma_id").ToString.Contains("INPOST") Then
                If Session("shipment_id") <> "" Then
                    Dim inpost_session As New inpost_package.InpostSession
                    inpost_package.inpost_login(inpost_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))

                    Dim raport_date As String = DateTime.Now.Year.ToString("D2") & "-" & DateTime.Now.Month.ToString("D2") & "-" & DateTime.Now.Day.ToString("D2")
                    Dim inpost_label As String = inpost_package.REST_GET_ReportsCod(raport_date, "confirmed")

                    If inpost_label.Length <> 4 Then
                        HLEtykietaPDF_INPOST.NavigateUrl = "http://10.1.0.64:8084/upload/inpost/" & inpost_label
                        HLEtykietaPDF_INPOST.Text = inpost_label
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Komunikat INPOST : " & inpost_package.exception.ToString & "<br />"
                        Session(session_id) += "</div>"
                    End If

                End If
            End If

            conn.Close()
        End Using
    End Sub

    Protected Sub BRaportPickup_DHL_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BRaportPickup_DHL_DE_API.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Session.Remove("contentKomunikat")
            If Session("firma_id").ToString.Contains("DHL_DE_API") Then
                If Session("shipment_id") <> "" Then
                    Dim dhl_session As New dhl_parcel_de_package.DHL_DE_Session
                    dhl_parcel_de_package.dhl_login(dhl_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))

                    Dim raport_date As String = DateTime.Now.Year.ToString("D2") & "-" & DateTime.Now.Month.ToString("D2") & "-" & DateTime.Now.Day.ToString("D2")
                    Dim dhldelist_label As List(Of String) = dhl_parcel_de_package.REST_GET_Manifest(raport_date, dhl_session.billingNumber)

                    If dhldelist_label.Count > 0 Then
                        HLEtykietaPDF_DHL_DE_API.NavigateUrl = dhldelist_label(0).ToString
                        HLEtykietaPDF_DHL_DE_API.Text = dhldelist_label(0).ToString
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Komunikat DHL_DE_API : " & dhl_parcel_de_package.exception.ToString & "<br />"
                        Session(session_id) += "</div>"
                    End If

                End If
            End If

            conn.Close()
        End Using
    End Sub


    Protected Sub BGenerujEtykiete_INPOST_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BGenerujEtykiete_INPOST.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session.Remove("contentKomunikat")
            Dim przerwijGenerowanie As Boolean = False
            For Each gdr As GridViewRow In GridViewPaczki.Rows
                Dim firma_id As String = gdr.Cells(3).Text.ToString
                If firma_id.Contains("INPOST") = False Then
                    przerwijGenerowanie = True
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Wprowadz pole firma_id dla wszystkich utworzonych paczek!<br />"
                    Session(session_id) += "</div>"
                    Exit For
                End If
            Next

            sqlexp = "select count(*) from dp_swm_mia_paczka where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and shipment_id not like 'S%'"
            Dim ile_ship_dhl As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            If ile_ship_dhl <> "0" Then
                przerwijGenerowanie = True
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Dla wybranych paczek istnieje juz utworzona etykieta INPOST!<br />"
                Session(session_id) += "</div>"
            End If

            If przerwijGenerowanie = False Then
                Dim shippment_id As String = Session("shipment_id").ToString
                ''shippment_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                If Session("firma_id").ToString.Contains("INPOST") Then

                    If DirectCast(FindControl("DDL_SERVICE_TYPE_INPOST"), DropDownList).SelectedValue.ToString <> "" Then

                        Dim inpost_session As New inpost_package.InpostSession

                        inpost_package.inpost_login(inpost_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))

                        Dim company As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_company from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)
                        Dim first_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_firstname from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)
                        Dim last_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_lastname from dp_rest_mag_order where increment_id='" & LNr_zamow_o.Text.ToString & "'", conn)

                        inpost_session.i_shipment.i_receiver = inpost_package.inpost_loadReceiver(DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_COMPANY_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_FIRSTNAME_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_LASTNAME_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_EMAIL_ADD"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PHONE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_City"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, DirectCast(FindControl("DDL_SR_COUNTRY"), DropDownList).SelectedValue.ToString)

                        ''Dim ss_count As Integer = GridView_SS_Service_type_INPOST.Rows.Count
                        ''geis_package.geis_loadService(ss_count - 1)
                        ''ss_count = 0
                        For Each gdr As GridViewRow In GridView_SS_Service_type_INPOST.Rows
                            Dim ss_service_type As String = gdr.Cells(2).Text.ToString
                            Dim ss_service_value As String = gdr.Cells(3).Text.ToString
                            If ss_service_type = "insurance" Then
                                inpost_session.i_shipment.i_insurance = inpost_package.inpost_loadInsurance(ss_service_value.ToString, "PLN")
                            ElseIf ss_service_type = "cod" Then
                                inpost_session.i_shipment.i_cod = inpost_package.inpost_loadCod(ss_service_value.ToString, "PLN")
                            Else
                                inpost_session.i_shipment.i_additional_services.Add(ss_service_type)
                            End If
                        Next

                        ''Dim package_count As Integer = GridViewPaczki.Rows.Count
                        ''package_count = 0
                        For Each gdr As GridViewRow In GridViewPaczki.Rows
                            Dim firma_id As String = gdr.Cells(3).Text.ToString
                            Dim typ As String = gdr.Cells(4).Text.ToString
                            Dim rodzaj As String = gdr.Cells(5).Text.ToString
                            Dim waga As String = gdr.Cells(6).Text.ToString
                            Dim szer As String = gdr.Cells(7).Text.ToString
                            Dim wys As String = gdr.Cells(8).Text.ToString
                            Dim dlug As String = gdr.Cells(9).Text.ToString
                            Dim ile_opak As String = gdr.Cells(10).Text.ToString
                            Dim is_non_standard As String = gdr.Cells(11).Text.ToString
                            If is_non_standard = "0" Then : is_non_standard = "false"
                            ElseIf is_non_standard = "1" Then : is_non_standard = "true"
                            End If

                            Dim inpost_paczka_id As String = gdr.RowIndex.ToString

                            If firma_id.Contains("INPOST") Then
                                If rodzaj.Contains("[") And rodzaj.Contains("]") Then
                                    Dim rodzaj_t As String = rodzaj.Split("[")(1).ToString.Replace("]", "")
                                    If rodzaj_t = "A" Then
                                        inpost_session.i_shipment.i_parcels.Add(inpost_package.inpost_loadParcel("small", inpost_paczka_id, dlug * 10, szer * 10, wys * 10, "mm", waga, "kg", is_non_standard))
                                    ElseIf rodzaj_t = "B" Then
                                        inpost_session.i_shipment.i_parcels.Add(inpost_package.inpost_loadParcel("medium", inpost_paczka_id, dlug * 10, szer * 10, wys * 10, "mm", waga, "kg", is_non_standard))
                                    ElseIf rodzaj_t = "C" Then
                                        inpost_session.i_shipment.i_parcels.Add(inpost_package.inpost_loadParcel("large", inpost_paczka_id, dlug * 10, szer * 10, wys * 10, "mm", waga, "kg", is_non_standard))
                                    ElseIf rodzaj_t = "D" Then
                                        inpost_session.i_shipment.i_parcels.Add(inpost_package.inpost_loadParcel("xlarge", inpost_paczka_id, dlug * 10, szer * 10, wys * 10, "mm", waga, "kg", is_non_standard))
                                    ElseIf rodzaj_t = "RECZNIE" Then
                                        inpost_session.i_shipment.i_parcels.Add(inpost_package.inpost_loadParcel("", inpost_paczka_id, dlug * 10, szer * 10, wys * 10, "mm", waga, "kg", is_non_standard))
                                    End If
                                End If
                            End If
                        Next

                        inpost_session.i_shipment.i_service = DDL_SERVICE_TYPE_INPOST.SelectedValue.ToString
                        inpost_session.i_shipment.i_comments = ""
                        inpost_session.i_shipment.i_mpk = DDL_B_COSTS_CENTER_INPOST.SelectedValue.ToString
                        inpost_session.i_shipment.i_reference = TB_COMMENT_F_INPOST.Text.ToString

                        Dim inpost_custom As New inpost_package.CustomAttributes
                        inpost_custom.target_point = TB_SR_POST_NUM.Text.ToString
                        inpost_session.i_shipment.i_custom_attributes = inpost_custom

                        ''####2022.08.04 / tworzenie etykiety w trybie ofertowym / mozliwosc pozniejszej edycji oraz usuniecia
                        inpost_session.i_shipment.i_only_choice_of_offer = "true"
                        Dim shipment_id As String = inpost_package.REST_POST_CreateShipment(inpost_session.i_shipment)
                        ''shipment_id = "11706911"

                        If shipment_id <> "" Then
                            Dim inpost_label_id As String = inpost_package.REST_GET_ParcelTrackingNumber(shipment_id)
                            If inpost_label_id = "" Then
                                inpost_label_id = shipment_id
                            End If

                            sqlexp = "update dp_swm_mia_paczka set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & inpost_label_id & "',shipment_id='" & inpost_label_id & "' WHERE shipment_id like '" & shippment_id.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            For Each row As GridViewRow In GridViewZamowienia.Rows
                                Dim nr_zam As String = row.Cells(2).Text.ToString
                                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), inpost_label_id, nr_zam)
                            Next

                            ''zmiana numeru listu przewozowego
                            Session("shipment_id") = inpost_label_id
                            LNr_list_przewozowy.Text = Session("shipment_id")
                            LadujDaneGridViewZamowienia()
                            LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Komunikat INPOST : " & inpost_package.exception.ToString & "<br />"
                            Session(session_id) += "</div>"
                        End If

                    End If
                End If
            End If

            conn.Close()
        End Using
    End Sub

    Protected Sub BGenerujOferte_INPOST_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BGenerujOferte_INPOST.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session.Remove("contentKomunikat")
            Dim przerwijGenerowanie As Boolean = False
            For Each gdr As GridViewRow In GridViewPaczki.Rows
                Dim firma_id As String = gdr.Cells(3).Text.ToString
                If firma_id.Contains("INPOST") = False Then
                    przerwijGenerowanie = True
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Wprowadz pole firma_id dla wszystkich utworzonych paczek!<br />"
                    Session(session_id) += "</div>"
                    Exit For
                End If
            Next

            sqlexp = "select count(*) from dp_swm_mia_paczka where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and shipment_id not like 'S%'"
            Dim ile_ship_dhl As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            If ile_ship_dhl = "0" Then
                przerwijGenerowanie = True
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Dla wybranych zamowien brakuje utworzonych ofert INPOST!<br />"
                Session(session_id) += "</div>"
            End If

            If przerwijGenerowanie = False Then
                Dim shippment_id As String = Session("shipment_id").ToString
                ''shippment_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                If Session("firma_id").ToString.Contains("INPOST") Then

                    If DirectCast(FindControl("DDL_SERVICE_TYPE_INPOST"), DropDownList).SelectedValue.ToString <> "" Then

                        Dim inpost_session As New inpost_package.InpostSession

                        inpost_package.inpost_login(inpost_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))
                        Dim status As String = inpost_package.REST_GET_StatusOfShipmentId(shippment_id)
                        If status = "offer_selected" Then
                            Dim offer_id As String = inpost_package.REST_GET_OfferIdOfShipmentId(shippment_id)
                            If offer_id <> "" Then
                                Dim shipment_id As String = inpost_package.REST_POST_BuyShipment(shippment_id, offer_id)
                                If shipment_id <> "" Then
                                    Threading.Thread.Sleep(5000)
                                    Dim inpost_label_id As String = inpost_package.REST_GET_ParcelTrackingNumber(shipment_id)
                                    If inpost_label_id = "" Then
                                        inpost_label_id = shipment_id
                                    End If

                                    sqlexp = "update dp_swm_mia_paczka set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & inpost_label_id & "',shipment_id='" & inpost_label_id & "' WHERE shipment_id like '" & shippment_id.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                                    For Each row As GridViewRow In GridViewZamowienia.Rows
                                        Dim nr_zam As String = row.Cells(2).Text.ToString
                                        result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), inpost_label_id, nr_zam)
                                    Next

                                    ''zmiana numeru listu przewozowego
                                    Session("shipment_id") = inpost_label_id
                                    LNr_list_przewozowy.Text = Session("shipment_id")
                                    LadujDaneGridViewZamowienia()
                                    LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                                Else
                                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                                    Session(session_id) += "<br />Komunikat INPOST : " & inpost_package.exception.ToString & "<br />"
                                    Session(session_id) += "</div>"
                                End If

                            End If

                        End If
                    End If
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BPobierzWszystkie_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzWszystkie.Click
        Session.Remove("contentKomunikat")
        If Session("firma_id") = "DHL" Then
            If Session("shipment_id") <> "" Then
                Dim file_dhl As String = "dhl_LP_" & Session("shipment_id") & ".pdf"
                If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
                    HLEtykietaLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_LP_" & Session("shipment_id") & ".pdf"
                    HLEtykietaLP.Text = file_dhl
                    If Session("drukowanie") = True Then
                        Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                        dhl_package.dhl_printLabels(dhl_session, "LP", "pdf", Session("shipment_id"))
                    End If
                Else
                    Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    dhl_package.dhl_getLabels(dhl_session, "LP", "pdf", Session("shipment_id"))
                    HLEtykietaLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_LP_" & Session("shipment_id") & ".pdf"
                    HLEtykietaLP.Text = file_dhl
                End If

                file_dhl = "dhl_BLP_" & Session("shipment_id") & ".pdf"
                If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
                    HLEtykietaBLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_BLP_" & Session("shipment_id") & ".pdf"
                    HLEtykietaBLP.Text = file_dhl
                    If Session("drukowanie") = True Then
                        Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                        dhl_package.dhl_printLabels(dhl_session, "BLP", "pdf", Session("shipment_id"))
                    End If
                Else
                    Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    dhl_package.dhl_getLabels(dhl_session, "BLP", "pdf", Session("shipment_id"))
                    HLEtykietaBLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_BLP_" & Session("shipment_id") & ".pdf"
                    HLEtykietaBLP.Text = file_dhl
                End If

                file_dhl = "dhl_ZBLP_" & Session("shipment_id") & ".zpl"
                If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
                    HLEtykietaZBLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & " dhl_ZBLP_" & Session("shipment_id") & ".zpl"
                    HLEtykietaZBLP.Text = file_dhl
                    If Session("drukowanie") = True Then
                        Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                        dhl_package.dhl_printLabels(dhl_session, "ZBLP", "zpl", Session("shipment_id"))
                    End If
                Else
                    Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    dhl_package.dhl_getLabels(dhl_session, "ZBLP", "zpl", Session("shipment_id"))
                    HLEtykietaZBLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_ZBLP_" & Session("shipment_id") & ".zpl"
                    HLEtykietaZBLP.Text = file_dhl
                End If
            End If
        End If
    End Sub

    Protected Sub BPobierzLP_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzLP.Click
        Session.Remove("contentKomunikat")
        If Session("firma_id") = "DHL" Then
            If Session("shipment_id") <> "" Then
                Dim file_dhl As String = "dhl_LP_" & Session("shipment_id") & ".pdf"
                If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
                    HLEtykietaLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_LP_" & Session("shipment_id") & ".pdf"
                    HLEtykietaLP.Text = file_dhl
                    If Session("drukowanie") = True Then
                        Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                        dhl_package.dhl_printLabels(dhl_session, "LP", "pdf", Session("shipment_id"))
                        Dim pdfFileName As String = dajarSWMagazyn_MIA.MyFunction.networkDirectory & "\" & "dhl_" & "LP" & "_" & Session("shipment_id") & ".pdf"

                    End If
                Else
                    Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    dhl_package.dhl_getLabels(dhl_session, "LP", "pdf", Session("shipment_id"))
                    HLEtykietaLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_LP_" & Session("shipment_id") & ".pdf"
                    HLEtykietaLP.Text = file_dhl
                End If
            End If
        ElseIf Session("firma_id") = "DHL_PS" Then
            If Session("shipment_id") <> "" Then
                Dim file_dhl As String = "dhl_BLP_" & Session("shipment_id") & ".pdf"
                If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
                    HLEtykietaLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_PS_" & Session("shipment_id") & ".pdf"
                    HLEtykietaLP.Text = file_dhl
                    If Session("drukowanie") = True Then
                        Dim dhl_session As ServicePointMethodsService = dhl_parcelshop.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                        dhl_parcelshop.dhl_printLabels(dhl_session, "BLP", "pdf", Session("shipment_id"))
                        Dim pdfFileName As String = dajarSWMagazyn_MIA.MyFunction.networkDirectory & "\" & "dhl_" & "BLP" & "_" & Session("shipment_id") & ".pdf"

                    End If
                Else
                    Dim dhl_session As ServicePointMethodsService = dhl_parcelshop.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    dhl_parcelshop.dhl_getLabels(dhl_session, "BLP", "pdf", Session("shipment_id"))
                    HLEtykietaLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_BLP_" & Session("shipment_id") & ".pdf"
                    HLEtykietaLP.Text = file_dhl
                End If
            End If
        End If
    End Sub

    Protected Sub BPobierzPDF_GEIS_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzPDF_GEIS.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove("contentKomunikat")
        If Session("firma_id").ToString.Contains("GEIS") Then
            If Session("shipment_id") <> "" Then
                Dim geis_session As GService.GServiceClient = geis_package.geis_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                Dim geis_label As String = geis_package.geis_getLabels(geis_session, Session("shipment_id"))
                If geis_label.Length <> 4 Then
                    HLEtykietaPDF_GEIS.NavigateUrl = "http://10.1.0.64:8084/upload/geis/" & geis_label
                    HLEtykietaPDF_GEIS.Text = geis_label
                Else
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Komunikat GEIS : " & geis_package.exception.ToString & "<br />"
                    Session(session_id) += "</div>"
                End If
                ''End If
            End If
        End If
    End Sub

    Protected Sub BPobierzPDF_INPOST_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzPDF_INPOST.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
            Session.Remove("contentKomunikat")
            If Session("firma_id").ToString.Contains("INPOST") Then
                If Session("shipment_id") <> "" Then

                    Dim inpost_session As New inpost_package.InpostSession
                    inpost_package.inpost_login(inpost_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))

                    Dim shippment_id As String = Session("shipment_id").ToString
                    Dim inpost_shipment As String = ""
                    If shippment_id.Length = 24 Then
                        inpost_shipment = inpost_package.REST_GET_ShipmentId(Session("shipment_id"))
                    Else
                        Dim inpost_label_id As String = inpost_package.REST_GET_ParcelTrackingNumber(Session("shipment_id"))
                        If inpost_label_id <> "" And inpost_label_id.Length = 24 Then
                            sqlexp = "update dp_swm_mia_paczka set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_info set shipment_id='" & inpost_label_id & "' WHERE shipment_id like '" & shippment_id.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        End If

                        inpost_shipment = shippment_id
                    End If

                    If inpost_shipment <> "" Then
                        Dim inpost_label As String = inpost_package.REST_GET_LabelOfShipment(Session("shipment_id"), inpost_shipment)
                        If inpost_label.Length <> 4 Then
                            HLEtykietaPDF_INPOST.NavigateUrl = "http://10.1.0.64:8084/upload/inpost/inpost_" & inpost_label & ".pdf"
                            HLEtykietaPDF_INPOST.Text = inpost_label
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Komunikat INPOST : " & inpost_package.exception.ToString & "<br />"
                            Session(session_id) += "</div>"
                        End If
                    End If
                End If
            End If

            conn.Close()
        End Using
    End Sub

    Protected Sub BPobierzPDF_DHL_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzPDF_DHL_DE_API.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
            Session.Remove("contentKomunikat")
            If Session("firma_id").ToString.Contains("DHL_DE_API") Then
                If Session("shipment_id") <> "" Then

                    Dim shippment_id As String = Session("shipment_id").ToString
                    shippment_id = Session("tracking_number").ToString

                    Dim tracking_list As New List(Of String)
                    For Each row As GridViewRow In GridViewPrzesylki_DHL_DE_API.Rows
                        Dim cb As CheckBox = row.FindControl("CBKodSelect")
                        If cb IsNot Nothing And cb.Checked Then
                            Dim tracking_number As String = row.Cells(9).Text.ToString
                            ''tracking_list.Add(tracking_number)
                            Dim dhlde_session As New dhl_parcel_de_package.DHL_DE_Session
                            dhl_parcel_de_package.dhl_login(dhlde_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))

                            If tracking_number <> "" Then
                                Dim inpost_label As String = dhl_parcel_de_package.REST_GET_LabelOfShipment(tracking_number)
                                If inpost_label.Length <> 4 Then
                                    HLEtykietaPDF_DHL_DE_API.NavigateUrl = "http://10.1.0.64:8084/upload/dhl_de/" & inpost_label & ".pdf"
                                    HLEtykietaPDF_DHL_DE_API.Text = inpost_label

                                    Dim link As HyperLink = row.FindControl("CBLink")
                                    If link IsNot Nothing Then
                                        link.NavigateUrl = "http://10.1.0.64:8084/upload/dhl_de/" & tracking_number & ".pdf"
                                    End If
                                Else
                                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                                    Session(session_id) += "<br />Komunikat DHL_DE_API : " & dhl_parcel_de_package.exception.ToString & "<br />"
                                    Session(session_id) += "</div>"
                                End If
                            End If
                        End If
                    Next
                End If
            End If

            conn.Close()
        End Using
    End Sub

    Protected Sub BPobierzPDF_UPS_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzPDF_UPS_DE_API.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
            Session.Remove("contentKomunikat")
            If Session("firma_id").ToString.Contains("UPS_DE_API") Then
                If Session("shipment_id") <> "" Then
                    Dim shippment_id As String = Session("shipment_id").ToString
                    shippment_id = Session("tracking_number").ToString

                    For Each row As GridViewRow In GridViewPrzesylki_UPS_DE_API.Rows
                        Dim cb As CheckBox = row.FindControl("CBKodSelect")
                        If cb IsNot Nothing And cb.Checked Then
                            Dim tracking_number As String = row.Cells(9).Text.ToString
                            Dim ups_session As New ups_de_package.UPS_DE_Session
                            ups_de_package.ups_login(ups_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))
                            ups_de_package.ups_token(ups_session)
                            If tracking_number <> "" Then
                                ''''Dim uuid As String = System.Guid.NewGuid.ToString()
                                Dim ups_reference As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select comment_f from dp_swm_mia_paczka_base where shipment_id='" & shippment_id & "'", conn)
                                Dim inpost_label As String = ups_de_package.REST_GET_LabelOfShipment(ups_session, tracking_number, ups_reference, "testing")
                                If inpost_label.Length <> 4 Then
                                    HLEtykietaPDF_UPS_DE_API.NavigateUrl = "http://10.1.0.64:8084/upload/ups_de/" & inpost_label & ".pdf"
                                    HLEtykietaPDF_UPS_DE_API.Text = inpost_label
                                Else
                                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                                    Session(session_id) += "<br />Komunikat UPS_DE_API : " & ups_de_package.exception.ToString & "<br />"
                                    Session(session_id) += "</div>"
                                End If
                            End If
                        End If
                    Next
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BPobierzPdf_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzPdf.Click
        Session.Remove("contentKomunikat")
        If Session("firma_id") = "DHL" Then
            If Session("shipment_id") <> "" Then
                Dim file_dhl As String = "dhl_BLP_" & Session("shipment_id") & ".pdf"
                If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
                    HLEtykietaBLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_BLP_" & Session("shipment_id") & ".pdf"
                    HLEtykietaBLP.Text = file_dhl
                    If Session("drukowanie") = True Then
                        Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                        dhl_package.dhl_printLabels(dhl_session, "BLP", "pdf", Session("shipment_id"))
                    End If
                Else
                    Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    dhl_package.dhl_getLabels(dhl_session, "BLP", "pdf", Session("shipment_id"))
                    HLEtykietaBLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_BLP_" & Session("shipment_id") & ".pdf"
                    HLEtykietaBLP.Text = file_dhl
                End If
            End If
        End If
    End Sub

    Protected Sub BPobierzZebra_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzZebra.Click
        Session.Remove("contentKomunikat")
        If Session("firma_id") = "DHL" Then
            If Session("shipment_id") <> "" Then
                Dim file_dhl As String = "dhl_ZBLP_" & Session("shipment_id") & ".zpl"
                If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
                    HLEtykietaZBLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_ZBLP_" & Session("shipment_id") & ".zpl"
                    HLEtykietaZBLP.Text = file_dhl
                    If Session("drukowanie") = True Then
                        Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                        dhl_package.dhl_printLabels(dhl_session, "ZBLP", "zpl", Session("shipment_id"))
                    End If
                Else
                    Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    dhl_package.dhl_getLabels(dhl_session, "ZBLP", "zpl", Session("shipment_id"))
                    HLEtykietaZBLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_ZBLP_" & Session("shipment_id") & ".zpl"
                    HLEtykietaZBLP.Text = file_dhl
                End If
            End If
        End If
    End Sub

    Protected Sub BRaportPNP_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BRaportPNP.Click
        Session.Remove("contentKomunikat")
        If Session("firma_id") = "DHL" Then
            Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
            Dim data_raport As String = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("D2") & "-" & DateTime.Now.Day.ToString("D2")
            Dim file_dhl As String = "dhl_pnp_" & Session("schemat_dyspo") & "_" & DateTime.Now.Year & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & ".pdf"
            dhl_package.dhl_getPnp(dhl_session, data_raport, "ALL", file_dhl)

            If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
                HLRaportPNP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & file_dhl
                HLRaportPNP.Text = file_dhl
            End If
        ElseIf Session("firma_id") = "DHL_PS" Then
            Dim dhl_session As ServicePointMethodsService = dhl_parcelshop.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
            Dim data_raport As String = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("D2") & "-" & DateTime.Now.Day.ToString("D2")
            Dim file_dhl As String = "dhl_pnp_" & Session("schemat_dyspo") & "_" & DateTime.Now.Year & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") & ".pdf"
            dhl_parcelshop.dhl_getPnp(dhl_session, data_raport, "ALL", file_dhl)

            If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
                HLRaportPNP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & file_dhl
                HLRaportPNP.Text = file_dhl
            End If
        End If
    End Sub

    Protected Sub BAnulujDHL_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnuluj_DHL.Click
        Session.Remove("contentKomunikat")
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If Session("shipment_id") <> "" Then
                If Session("firma_id") = "DHL" Then
                    Dim dhl_session As DHL24WebapiService = dhl_package.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    Dim dhl_anuluj As String = dhl_package.dhl_deleteShipment(dhl_session, Session("shipment_id"))
                    If dhl_anuluj <> "1" And Session("anulowanie") IsNot Nothing Then
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Komunikat DHL : " & dhl_anuluj.ToString & "<br />"
                        Session(session_id) += "</div>"
                    Else
                        Dim shipment_id_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                        sqlexp = "UPDATE dp_swm_mia_paczka_info SET tracking_number='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "UPDATE dp_swm_mia_paczka_base SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "UPDATE dp_swm_mia_paczka_ss SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "UPDATE dp_swm_mia_paczka SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        Session("shipment_id") = shipment_id_new
                        LNr_list_przewozowy.Text = shipment_id_new

                        HLEtykietaLP.NavigateUrl = ""
                        HLEtykietaLP.Text = ""
                        HLEtykietaBLP.NavigateUrl = ""
                        HLEtykietaBLP.Text = ""
                        HLEtykietaZBLP.NavigateUrl = ""
                        HLEtykietaZBLP.Text = ""

                        LadujDaneGridViewZamowienia()
                        LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                    End If
                ElseIf Session("firma_id") = "DHL_PS" Then
                    Dim dhl_session As ServicePointMethodsService = dhl_parcelshop.dhl_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    Dim dhl_anuluj As String = dhl_parcelshop.dhl_deleteShipment(dhl_session, Session("shipment_id"))
                    If dhl_anuluj <> "1" And Session("anulowanie") IsNot Nothing Then
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Komunikat DHL POP : " & dhl_anuluj.ToString & "<br />"
                        Session(session_id) += "</div>"
                    Else
                        Dim shipment_id_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                        sqlexp = "UPDATE dp_swm_mia_paczka_info SET tracking_number='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "UPDATE dp_swm_mia_paczka_base SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "UPDATE dp_swm_mia_paczka_ss SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "UPDATE dp_swm_mia_paczka SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        Session("shipment_id") = shipment_id_new
                        LNr_list_przewozowy.Text = shipment_id_new

                        HLEtykietaLP.NavigateUrl = ""
                        HLEtykietaLP.Text = ""
                        HLEtykietaBLP.NavigateUrl = ""
                        HLEtykietaBLP.Text = ""
                        HLEtykietaZBLP.NavigateUrl = ""
                        HLEtykietaZBLP.Text = ""

                        LadujDaneGridViewZamowienia()
                        LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                    End If

                ElseIf Session("firma_id") = "DHL_DE" Then
                    dhlDESoapRequest.dhl_de_login(Session("schemat_dyspo"), Session("firma_id"))
                    dhlDESoapRequest.StartWebReference()

                    sqlexp = "select distinct shipment_id from dp_swm_mia_paczka where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "' and shipment_id like 'SH%'"
                    Dim shipment_id_hist As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    shipment_id_hist = shipment_id_hist.Trim

                    Dim ddRequestDelete As dhlDEAPI.DeleteShipmentDDRequest = dhlDESoapRequest.getDeleteShipmentDDRequest(Session("shipment_id"))
                    Dim shResponseDelete As dhlDEAPI.DeleteShipmentResponse = dhlDESoapRequest.webService.deleteShipmentDD(dhlDESoapRequest.auth, ddRequestDelete)
                    Dim status As Statusinformation = shResponseDelete.Status
                    Dim statusCode As String = status.StatusCode
                    Dim statusMessage As String = status.StatusMessage & vbNewLine
                    If statusCode = "0" Then
                        Dim shipment_id_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                        If shipment_id_hist <> "" Then
                            shipment_id_new = shipment_id_hist

                            sqlexp = "UPDATE dp_swm_mia_paczka_info SET tracking_number='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "DELETE FROM dp_swm_mia_paczka_base WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "DELETE FROM dp_swm_mia_paczka_ss WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "DELETE FROM dp_swm_mia_paczka WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Else
                            sqlexp = "UPDATE dp_swm_mia_paczka_info SET tracking_number='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "UPDATE dp_swm_mia_paczka_base SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "UPDATE dp_swm_mia_paczka_ss SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "UPDATE dp_swm_mia_paczka SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End If


                        Session("shipment_id") = shipment_id_new
                        LNr_list_przewozowy.Text = shipment_id_new

                        HLEtykietaLP.NavigateUrl = ""
                        HLEtykietaLP.Text = ""
                        HLEtykietaBLP.NavigateUrl = ""
                        HLEtykietaBLP.Text = ""
                        HLEtykietaZBLP.NavigateUrl = ""
                        HLEtykietaZBLP.Text = ""

                        LadujDaneGridViewPaczka()
                        LadujDaneGridViewZamowienia()
                        LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Komunikat DHL : " & statusMessage.ToString & "<br />"
                        Session(session_id) += "</div>"
                    End If
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnuluj_DHL_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnuluj_DHL_DE_API.Click
        Session.Remove("contentKomunikat")
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If Session("tracking_number") <> "" Then
                If Session("firma_id").ToString.Contains("DHL_DE_API") Then
                    Dim dhlde_session As New dhl_parcel_de_package.DHL_DE_Session
                    dhl_parcel_de_package.dhl_login(dhlde_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))

                    Dim inpost_shipment As String = Session("tracking_number")
                    ''Dim inpost_tracking As String = ""

                    Dim inpost_anuluj As String = dhl_parcel_de_package.REST_DELETE_DeleteShipment(dhlde_session, inpost_shipment)

                    If inpost_anuluj <> "OK" And Session("anulowanie") Is Nothing Then
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Komunikat DHL_DE_API : " & dhl_parcel_de_package.exception.ToString() & "<br />"
                        Session(session_id) += "</div>"

                    Else
                        ''##2024.05.28 / sprawdzenie ile pozostalo paczek do nowej numeracji
                        sqlexp = "select count(*) from dp_swm_mia_paczka_info where shipment_id='" & Session("shipment_id") & "' and tracking_number<>'" & inpost_shipment & "'"
                        Dim ile_paczek_pozostalo As Integer = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        ''#wariant zerowanie tylko tracking_number
                        If ile_paczek_pozostalo > 0 Then
                            sqlexp = "UPDATE dp_swm_mia_paczka_info SET TRACKING_NUMBER='' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        Else
                            ''#wariant nadawanie nowego numeru SH
                            Dim shipment_id_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                            sqlexp = "UPDATE dp_swm_mia_paczka_info SET TRACKING_NUMBER='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            sqlexp = "UPDATE dp_swm_mia_paczka_base SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            sqlexp = "UPDATE dp_swm_mia_paczka_ss SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            sqlexp = "UPDATE dp_swm_mia_paczka SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            Session("shipment_id") = shipment_id_new
                            LNr_list_przewozowy.Text = shipment_id_new
                        End If

                        LadujDaneGridViewZamowienia()
                        LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                    End If

                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnuluj_UPS_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnuluj_UPS_DE_API.Click
        Session.Remove("contentKomunikat")
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If Session("shipment_id") <> "" Then
                If Session("firma_id").ToString.Contains("UPS_DE_API") Then
                    Dim ups_session As New ups_de_package.UPS_DE_Session
                    ups_de_package.ups_login(ups_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))
                    ups_de_package.ups_token(ups_session)
                    Dim inpost_shipment As String = Session("shipment_id")
                    Dim inpost_tracking As String = Session("tracking_number")
                    ''''Dim uuid As String = System.Guid.NewGuid.ToString()
                    ''inpost_shipment = "1Z2220060294314162"
                    ''inpost_tracking = "1Z2220060291994175"
                    Dim inpost_anuluj As String = ups_de_package.REST_DELETE_Shipment(ups_session, inpost_shipment, inpost_tracking, "testing")

                    If inpost_anuluj <> "Success" And Session("anulowanie") Is Nothing Then
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Komunikat UPS_DE_API : " & ups_de_package.exception.ToString() & "<br />"
                        Session(session_id) += "</div>"

                    Else
                        ''##2024.05.28 / sprawdzenie ile pozostalo paczek do nowej numeracji
                        sqlexp = "select count(*) from dp_swm_mia_paczka_info where shipment_id='" & Session("shipment_id") & "' and tracking_number<>'" & inpost_tracking & "'"
                        Dim ile_paczek_pozostalo As Integer = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        ''#wariant zerowanie tylko tracking_number
                        If ile_paczek_pozostalo > 0 Then
                            sqlexp = "UPDATE dp_swm_mia_paczka_info SET TRACKING_NUMBER='' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Else
                            ''#wariant nadawanie nowego numeru SH
                            Dim shipment_id_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                            sqlexp = "UPDATE dp_swm_mia_paczka_info SET tracking_number='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            sqlexp = "UPDATE dp_swm_mia_paczka_base SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            sqlexp = "UPDATE dp_swm_mia_paczka_ss SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            sqlexp = "UPDATE dp_swm_mia_paczka SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            Session("shipment_id") = shipment_id_new
                            LNr_list_przewozowy.Text = shipment_id_new
                        End If

                        ''HLEtykietaPDF_UPS_DE_API.NavigateUrl = ""
                        ''HLEtykietaPDF_UPS_DE_API.Text = ""
                        ''HLEtykietaEPL_UPS_DE_API.NavigateUrl = ""
                        ''HLEtykietaEPL_UPS_DE_API.Text = ""
                        ''HLEtykietaZPL_UPS_DE_API.NavigateUrl = ""
                        ''HLEtykietaZPL_UPS_DE_API.Text = ""

                        LadujDaneGridViewZamowienia()
                        LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                    End If

                End If
            End If
            conn.Close()
        End Using
    End Sub


    Protected Sub BAnulujGEIS_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnuluj_GEIS.Click
        Session.Remove("contentKomunikat")
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If Session("shipment_id") <> "" Then
                If Session("firma_id").ToString.Contains("GEIS") Then
                    Dim geis_session As GService.GServiceClient = geis_package.geis_login("DAJAR", Session("mag_dyspo"), Session("firma_id"))
                    Dim geis_tracking_number As String = Session("tracking_number")
                    Dim geis_anuluj As String = geis_package.geis_deleteShipment(geis_session, geis_tracking_number)

                    ''geis_anuluj = False

                    If geis_anuluj = "False" And Session("anulowanie") Is Nothing Then
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Komunikat GEIS : " & geis_anuluj.ToString & "<br />"
                        Session(session_id) += "</div>"

                    Else
                        ''##2024.05.28 / sprawdzenie ile pozostalo paczek do nowej numeracji
                        sqlexp = "select count(*) from dp_swm_mia_paczka_info where shipment_id='" & Session("shipment_id") & "' and tracking_number<>'" & geis_tracking_number & "'"
                        Dim ile_paczek_pozostalo As Integer = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        ''#wariant zerowanie tylko tracking_number
                        If ile_paczek_pozostalo > 0 Then
                            sqlexp = "UPDATE dp_swm_mia_paczka_info SET TRACKING_NUMBER='' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' AND TRACKING_NUMBER='" & geis_tracking_number & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Else
                            Dim shipment_id_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                            sqlexp = "UPDATE dp_swm_mia_paczka_info SET tracking_number='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            sqlexp = "UPDATE dp_swm_mia_paczka_base SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            sqlexp = "UPDATE dp_swm_mia_paczka_ss SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            sqlexp = "UPDATE dp_swm_mia_paczka SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            Session("shipment_id") = shipment_id_new
                            LNr_list_przewozowy.Text = shipment_id_new
                        End If

                        LadujDaneGridViewZamowienia()
                        LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                    End If

                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnuluj_INPOST_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnuluj_INPOST.Click
        Session.Remove("contentKomunikat")
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If Session("shipment_id") <> "" Then
                If Session("firma_id").ToString.Contains("INPOST") Then

                    Dim inpost_session As New inpost_package.InpostSession

                    inpost_package.inpost_login(inpost_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))
                    Dim inpost_shipment As String = Session("shipment_id")
                    Dim inpost_tracking As String = ""

                    If Session("shipment_id").ToString.Length <> 24 Then
                        inpost_shipment = Session("shipment_id")
                        inpost_tracking = ""
                    Else
                        inpost_shipment = Session("shipment_id")
                        inpost_tracking = inpost_package.REST_GET_ParcelTrackingNumber(Session("shipment_id"))
                    End If

                    Dim inpost_anuluj As String = inpost_package.REST_DELETE_DeleteShipment(inpost_tracking, inpost_shipment)

                    If inpost_anuluj <> "1" And Session("anulowanie") Is Nothing Then
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Komunikat INPOST : " & inpost_package.exception.ToString() & "<br />"
                        Session(session_id) += "</div>"

                    Else
                        Dim shipment_id_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                        sqlexp = "UPDATE dp_swm_mia_paczka_info SET tracking_number='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "UPDATE dp_swm_mia_paczka_base SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "UPDATE dp_swm_mia_paczka_ss SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "UPDATE dp_swm_mia_paczka SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        Session("shipment_id") = shipment_id_new
                        LNr_list_przewozowy.Text = shipment_id_new

                        ''HLEtykietaPDF_INPOST.NavigateUrl = ""
                        ''HLEtykietaPDF_INPOST.Text = ""
                        ''HLEtykietaEPL_INPOST.NavigateUrl = ""
                        ''HLEtykietaEPL_INPOST.Text = ""
                        ''HLEtykietaZPL_INPOST.NavigateUrl = ""
                        ''HLEtykietaZPL_INPOST.Text = ""

                        LadujDaneGridViewZamowienia()
                        LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
                    End If

                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BZerowanie_INPOST_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BZerowanie_INPOST.Click
        Session.Remove("contentKomunikat")
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If Session("shipment_id") <> "" Then
                If Session("firma_id").ToString.Contains("INPOST") Then

                    Dim shipment_id_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                    sqlexp = "UPDATE dp_swm_mia_paczka_info SET tracking_number='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka_base SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka_ss SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    Session("shipment_id") = shipment_id_new
                    LNr_list_przewozowy.Text = shipment_id_new

                    ''HLEtykietaPDF_INPOST.NavigateUrl = ""
                    ''HLEtykietaPDF_INPOST.Text = ""
                    ''HLEtykietaEPL_INPOST.NavigateUrl = ""
                    ''HLEtykietaEPL_INPOST.Text = ""
                    ''HLEtykietaZPL_INPOST.NavigateUrl = ""
                    ''HLEtykietaZPL_INPOST.Text = ""

                    LadujDaneGridViewZamowienia()
                    LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))

                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BZerowanie_DHL_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BZerowanie_DHL_DE_API.Click
        Session.Remove("contentKomunikat")
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If Session("shipment_id") <> "" Then
                If Session("firma_id").ToString.Contains("DHL_DE_API") Then

                    Dim shipment_id_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                    sqlexp = "UPDATE dp_swm_mia_paczka_info SET TRACKING_NUMBER='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka_base SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka_ss SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    Session("shipment_id") = shipment_id_new
                    Session.Remove("tracking_number")
                    LNr_list_przewozowy.Text = shipment_id_new

                    ''HLEtykietaPDF_DHL_DE_API.NavigateUrl = ""
                    ''HLEtykietaPDF_DHL_DE_API.Text = ""
                    ''HLEtykietaEPL_DHL_DE_API.NavigateUrl = ""
                    ''HLEtykietaEPL_DHL_DE_API.Text = ""
                    ''HLEtykietaZPL_DHL_DE_API.NavigateUrl = ""
                    ''HLEtykietaZPL_DHL_DE_API.Text = ""

                    LadujDaneGridViewZamowienia()
                    LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))

                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BZerowanie_UPS_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BZerowanie_UPS_DE_API.Click
        Session.Remove("contentKomunikat")
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If Session("shipment_id") <> "" Then
                If Session("firma_id").ToString.Contains("UPS_DE_API") Then

                    Dim shipment_id_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                    sqlexp = "UPDATE dp_swm_mia_paczka_info SET tracking_number='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka_base SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka_ss SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    Session("shipment_id") = shipment_id_new
                    Session.Remove("tracking_number")

                    LNr_list_przewozowy.Text = shipment_id_new

                    ''HLEtykietaPDF_UPS_DE_API.NavigateUrl = ""
                    ''HLEtykietaPDF_UPS_DE_API.Text = ""
                    ''HLEtykietaEPL_UPS_DE_API.NavigateUrl = ""
                    ''HLEtykietaEPL_UPS_DE_API.Text = ""
                    ''HLEtykietaZPL_UPS_DE_API.NavigateUrl = ""
                    ''HLEtykietaZPL_UPS_DE_API.Text = ""

                    LadujDaneGridViewZamowienia()
                    LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))

                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub CBWymuszenieAnulowania_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBWymuszenieAnulowania.CheckedChanged
        If CBWymuszenieAnulowania.Checked Then
            Session("anulowanie") = True
        Else
            Session("anulowanie") = False
        End If
    End Sub

    Protected Sub CBWymuszenieAnulowania_GEIS_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBWymuszenieAnulowania_GEIS.CheckedChanged
        If CBWymuszenieAnulowania_GEIS.Checked Then
            Session("anulowanie") = True
        Else
            Session("anulowanie") = False
        End If
    End Sub

    Protected Sub BDodaj_SS_Service_type_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BDodaj_SS_Service_type.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If TB_SS_SERVICE_VALUE.Text <> "" Then
                If IsNumeric(TB_SS_SERVICE_VALUE.Text.ToString) Then
                    Dim shippment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                    Dim ss_service_type As String = DirectCast(FindControl("DDL_SS_Service_type"), DropDownList).SelectedValue.ToString
                    Dim ss_service_value As String = TB_SS_SERVICE_VALUE.Text.ToString.Replace(".", ",")
                    Dim ss_collect As String = DirectCast(FindControl("DDL_SS_COLL_ON_FORM"), DropDownList).SelectedValue.ToString

                    sqlexp = "select count(*) from dp_swm_mia_paczka_ss where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "' and ss_coll_on_form='" & ss_collect & "'"
                    Dim czyIstniejeUsluga As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If czyIstniejeUsluga <> "0" Then
                        sqlexp = "update dp_swm_mia_paczka_ss set ss_service_value='" & ss_service_value & "' where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "' and ss_coll_on_form='" & ss_collect & "'"
                    Else
                        sqlexp = "insert into dp_swm_mia_paczka_ss (schemat,shipment_id,ss_service_type,ss_service_value,ss_coll_on_form) values('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & ss_service_type & "','" & ss_service_value & "','" & ss_collect & "')"
                    End If
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    If ss_service_type = "COD" Then
                        sqlexp = "update dp_swm_mia_paczka_ss set ss_service_value='" & ss_service_value & "' where shipment_id='" & shippment_id & "' and ss_service_type='UBEZP'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        If Session("schemat_dyspo") = "DOMINUS" Then
                            sqlexp = "insert into dp_swm_mia_paczka_ss (schemat,shipment_id,ss_service_type,ss_service_value,ss_coll_on_form) values('" & Session("schemat_dyspo") & "','" & shippment_id & "','UBEZP','" & ss_service_value & "','BANK_TRANSFER')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End If
                    End If

                    TB_SS_SERVICE_VALUE.Text = ""
                    LadujDaneGridView_SS_Service_type()
                End If
            Else
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Wypelnij poprawnie pole deklarowana wartosc!<br />"
                Session(session_id) += "</div>"
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnuluj_SS_Service_type_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnuluj_SS_Service_type.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If GridView_SS_Service_type.Rows.Count > 0 Then
                For Each row As GridViewRow In GridView_SS_Service_type.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim ss_service_type As String = row.Cells(2).Text.ToString
                        Dim ss_service_value As String = row.Cells(3).Text.ToString
                        Dim shippment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        sqlexp = "delete from dp_swm_mia_paczka_ss where schemat='" & Session("schemat_dyspo") & "' and shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "' and ss_service_value='" & ss_service_value & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If
                Next
            End If

            LadujDaneGridView_SS_Service_type()
            conn.Close()
        End Using
    End Sub

    Protected Sub BDodaj_SS_Service_type_Click_GEIS(ByVal sender As Object, ByVal e As EventArgs) Handles BDodaj_SS_Service_type_GEIS.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If TB_SS_SERVICE_VALUE_GEIS.Text <> "" Then
                ''If IsNumeric(TB_SS_SERVICE_VALUE_GEIS.Text.ToString) Then
                Dim shippment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                Dim ss_service_type As String = DirectCast(FindControl("DDL_SS_Service_type" & "_GEIS"), DropDownList).SelectedValue.ToString

                Dim ss_service_value As String = TB_SS_SERVICE_VALUE_GEIS.Text.ToString
                If ss_service_type = "COD" Then
                    ss_service_value = TB_SS_SERVICE_VALUE_GEIS.Text.ToString.Replace(".", ",")
                End If

                ''Dim ss_collect As String = DirectCast(FindControl("DDL_SS_COLL_ON_FORM"), DropDownList).SelectedValue.ToString

                sqlexp = "select count(*) from dp_swm_mia_paczka_ss where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "'"
                Dim czyIstniejeUsluga As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If czyIstniejeUsluga <> "0" Then
                    sqlexp = "update dp_swm_mia_paczka_ss set ss_service_value='" & ss_service_value & "' where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "'"
                Else
                    sqlexp = "insert into dp_swm_mia_paczka_ss (schemat,shipment_id,ss_service_type,ss_service_value) values('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & ss_service_type & "','" & ss_service_value & "')"
                End If
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                If ss_service_type = "1002" Then
                    sqlexp = "update dp_swm_mia_paczka_ss set ss_service_value='" & ss_service_value & "' where shipment_id='" & shippment_id & "' and ss_service_type='1005'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    If Session("schemat_dyspo") = "DOMINUS" Then
                        sqlexp = "insert into dp_swm_mia_paczka_ss (schemat,shipment_id,ss_service_type,ss_service_value) values('" & Session("schemat_dyspo") & "','" & shippment_id & "','1005','" & ss_service_value & "')"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If
                End If

                TB_SS_SERVICE_VALUE_GEIS.Text = ""
                LadujDaneGridView_SS_Service_type_GEIS()
                ''End If
            Else
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Wypelnij poprawnie pole deklarowana wartosc!<br />"
                Session(session_id) += "</div>"
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnuluj_SS_Service_type_GEIS_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnuluj_SS_Service_type_GEIS.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If GridView_SS_Service_type_GEIS.Rows.Count > 0 Then
                For Each row As GridViewRow In GridView_SS_Service_type_GEIS.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim ss_service_type As String = row.Cells(2).Text.ToString
                        Dim ss_service_value As String = row.Cells(3).Text.ToString
                        Dim shippment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        sqlexp = "delete from dp_swm_mia_paczka_ss where schemat='" & Session("schemat_dyspo") & "' and shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "' and ss_service_value='" & ss_service_value & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If
                Next
            End If

            LadujDaneGridView_SS_Service_type_GEIS()
            conn.Close()
        End Using
    End Sub

    Protected Sub BDodaj_SS_Service_type_INPOST_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BDodaj_SS_Service_type_INPOST.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If TB_SS_SERVICE_VALUE_INPOST.Text <> "" Then
                ''If IsNumeric(TB_SS_SERVICE_VALUE_GEIS.Text.ToString) Then
                Dim shippment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                Dim ss_service_type As String = DirectCast(FindControl("DDL_SS_Service_type" & "_INPOST"), DropDownList).SelectedValue.ToString

                Dim ss_service_value As String = TB_SS_SERVICE_VALUE_INPOST.Text.ToString
                If ss_service_type = "cod" Or ss_service_type = "insurance" Then
                    ''2022.09.06 / modyfikacja deklarowanych parametrow dla INPOST [bylo "," zostaje "."]
                    ss_service_value = TB_SS_SERVICE_VALUE_INPOST.Text.ToString.Replace(",", ".")
                End If

                ''Dim ss_collect As String = DirectCast(FindControl("DDL_SS_COLL_ON_FORM"), DropDownList).SelectedValue.ToString

                sqlexp = "select count(*) from dp_swm_mia_paczka_ss where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "'"
                Dim czyIstniejeUsluga As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If czyIstniejeUsluga <> "0" Then
                    sqlexp = "update dp_swm_mia_paczka_ss set ss_service_value='" & ss_service_value & "' where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "'"
                Else
                    sqlexp = "insert into dp_swm_mia_paczka_ss (schemat,shipment_id,ss_service_type,ss_service_value) values('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & ss_service_type & "','" & ss_service_value & "')"
                End If
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                ''If ss_service_type = "1002" Then
                ''    sqlexp = "update dp_swm_mia_paczka_ss set ss_service_value='" & ss_service_value & "' where shipment_id='" & shippment_id & "' and ss_service_type='1005'"
                ''    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                ''    If Session("schemat_dyspo") = "DOMINUS" Then
                ''        sqlexp = "insert into dp_swm_mia_paczka_ss (schemat,shipment_id,ss_service_type,ss_service_value) values('" & Session("schemat_dyspo") & "','" & shippment_id & "','1005','" & ss_service_value & "')"
                ''        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                ''    End If
                ''End If

                TB_SS_SERVICE_VALUE_INPOST.Text = ""
                LadujDaneGridView_SS_Service_type_INPOST()
                ''End If
            Else
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Wypelnij poprawnie pole deklarowana wartosc!<br />"
                Session(session_id) += "</div>"
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnuluj_SS_Service_type_INPOST_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnuluj_SS_Service_type_INPOST.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If GridView_SS_Service_type_INPOST.Rows.Count > 0 Then
                For Each row As GridViewRow In GridView_SS_Service_type_INPOST.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim ss_service_type As String = row.Cells(2).Text.ToString
                        Dim ss_service_value As String = row.Cells(3).Text.ToString
                        Dim shippment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        sqlexp = "delete from dp_swm_mia_paczka_ss where schemat='" & Session("schemat_dyspo") & "' and shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "' and ss_service_value='" & ss_service_value & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If
                Next
            End If

            LadujDaneGridView_SS_Service_type_GEIS()
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnuluj_SS_Service_type_UPS_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnuluj_SS_Service_type_UPS_DE_API.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If GridView_SS_Service_type_UPS_DE_API.Rows.Count > 0 Then
                For Each row As GridViewRow In GridView_SS_Service_type_UPS_DE_API.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim ss_service_type As String = row.Cells(2).Text.ToString
                        Dim ss_service_value As String = row.Cells(3).Text.ToString
                        Dim shippment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        sqlexp = "delete from dp_swm_mia_paczka_ss where schemat='" & Session("schemat_dyspo") & "' and shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "' and ss_service_value='" & ss_service_value & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If
                Next
            End If

            LadujDaneGridView_SS_Service_type_UPS_DE_API()
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnuluj_SS_Service_type_DHL_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnuluj_SS_Service_type_DHL_DE_API.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If GridView_SS_Service_type_DHL_DE_API.Rows.Count > 0 Then
                For Each row As GridViewRow In GridView_SS_Service_type_DHL_DE_API.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim ss_service_type As String = row.Cells(2).Text.ToString
                        Dim ss_service_value As String = row.Cells(3).Text.ToString
                        Dim shippment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        sqlexp = "delete from dp_swm_mia_paczka_ss where schemat='" & Session("schemat_dyspo") & "' and shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "' and ss_service_value='" & ss_service_value & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If
                Next
            End If

            LadujDaneGridView_SS_Service_type_DHL_DE_API()
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnulowaniePaczkomat_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnulowaniePaczkomat.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If GridViewPaczki.Rows.Count > 0 Then
                For Each row As GridViewRow In GridViewPaczki.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim shippment_id As String = row.Cells(2).Text.ToString
                        sqlexp = "update dp_swm_mia_paczka_info set pl_type='1' where paczka_id='" & shippment_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If
                Next
            End If

            LadujDaneGridViewPaczka()
            conn.Close()
        End Using
    End Sub

    Protected Sub BKopiujDaneDostawy_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BKopiujDaneDostawy.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session.Remove("press_BZapiszEtykiete")

            Dim tb_form_dhl() As String = {"SR_NAME_TEST", "SR_POSTAL_CODE_TEST", "SR_CITY_TEST", "SR_STREET_TEST", "SR_HOUSE_NUM_TEST", "SR_APART_NUM_TEST", "PC_PERSON_NAME_TEST", "PC_PHONE_NUM_TEST", "PC_EMAIL_ADD_TEST"}
            For Each tb_obj In tb_form_dhl
                Dim value As String = DirectCast(FindControl("TB_" & tb_obj), TextBox).Text.ToString.Trim
                DirectCast(FindControl("TB_" & tb_obj.Replace("_TEST", "")), TextBox).Text = value
                sqlexp = "update dp_swm_mia_paczka set " & tb_obj.Replace("_TEST", "") & " ='" & value & "' where shipment_id = '" & Session("shipment_id") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
            Next
            conn.Close()
        End Using
    End Sub

    Protected Sub BDodajZamowienie_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BDodajZamowienie.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If TBNrZamowienia.Text <> "" And TBSchemat.Text <> "" Then
                Dim shipment_id_glowne As String = LNr_list_przewozowy.Text.ToString
                Dim nr_zamow_glowne As String = LNrZamow.Text.ToString
                Dim schemat_glowne As String = LSchemat.Text.ToString

                Dim nr_zamow_new As String = TBNrZamowienia.Text.ToString
                Dim schemat_new As String = TBSchemat.Text.ToString

                sqlexp = "select count(*) from dp_swm_mia_zog where nr_zamow='" & nr_zamow_new & "' and schemat='" & schemat_new & "'"
                Dim ile_zamowien As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                If ile_zamowien > "0" Then
                    Dim shipment_new As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select shipment_id from dp_swm_mia_paczka where nr_zamow='" & nr_zamow_new & "' and schemat='" & schemat_new & "'", conn)
                    sqlexp = "update dp_swm_mia_paczka set shipment_id='" & shipment_id_glowne & "' where nr_zamow='" & nr_zamow_new & "' and schemat='" & schemat_new & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                    dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & shipment_id_glowne & "',shipment_id='" & shipment_id_glowne & "' where shipment_id='" & shipment_new & "' and schemat='" & schemat_new & "' and paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "'"
                    dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & shipment_id_glowne & "' where shipment_id='" & shipment_new & "' and schemat='" & schemat_new & "'"
                    dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    TBNrZamowienia.Text = ""
                    TBSchemat.Text = ""
                    LadujDaneGridViewZamowienia()
                    LadujDaneGridView_SS_Service_type()
                    LadujDaneGridViewPaczka()
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnulujZamowienie_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnulujZamowienie.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If LZamowienieId.Text <> "" Then
                Dim sel_nr_zamow As String = GridViewZamowienia.SelectedRow.Cells(2).Text.ToString()
                Dim sel_schemat As String = GridViewZamowienia.SelectedRow.Cells(3).Text.ToString()
                Dim shipment_id As String = GridViewZamowienia.SelectedRow.Cells(4).Text.ToString()
                Dim shipment_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                Dim paczka_new As String = dajarSWMagazyn_MIA.MyFunction.GeneratePaczkaId

                sqlexp = "update dp_swm_mia_paczka set shipment_id='" & shipment_new & "' where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "' and st_shipment_date like '" & DateTime.Now.Year.ToString & "-%'"
                dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                sqlexp = "insert into dp_swm_mia_paczka_info (schemat,shipment_id,paczka_id,firma_id,pl_type,pl_weight,pl_width,pl_height,pl_length,pl_quantity,pl_non_std,pl_euro_ret) " _
                & " select '" & sel_schemat & "','" & shipment_new & "','" & paczka_new & "','','1',1,1,1,1,1,'0','0' from dual" _
                & " where ((select count(*) from dp_swm_mia_paczka_info where shipment_id='" & shipment_new & "' and schemat = '" & sel_schemat & "') = 0)"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                LadujDaneGridViewZamowienia()
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub CB_SR_IS_PACKSTATION_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CB_SR_IS_PACKSTATION.CheckedChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If CB_SR_IS_PACKSTATION.Checked = True Then
                TB_SR_STREET.Text = "Packstation"
                sqlexp = "update dp_swm_mia_paczka set SR_STREET='Packstation' where shipment_id = '" & Session("shipment_id") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
            Else
                sqlexp = "update dp_swm_mia_paczka set SR_STREET='' where shipment_id = '" & Session("shipment_id") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                ValidateFormField(TB_SR_STREET.Text, "SR_STREET", "dp_swm_mia_paczka")
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub CB_SR_IS_POSTFILIALE_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CB_SR_IS_POSTFILIALE.CheckedChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If CB_SR_IS_POSTFILIALE.Checked = True Then
                TB_SR_STREET.Text = "Postfiliale"
                sqlexp = "update dp_swm_mia_paczka set SR_STREET='Postfiliale' where shipment_id = '" & Session("shipment_id") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
            Else
                sqlexp = "update dp_swm_mia_paczka set SR_STREET='' where shipment_id = '" & Session("shipment_id") & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                ValidateFormField(TB_SR_STREET.Text, "SR_STREET", "dp_swm_mia_paczka")
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BZapiszFirme_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BZapiszFirme.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("firma_id") IsNot Nothing Then
                Dim f_id As String = DDLFirma.SelectedValue.ToString

                For Each gdr As GridViewRow In GridViewPaczki.Rows
                    Dim firma_id As String = gdr.Cells(3).Text.ToString
                    Dim paczka_id As String = gdr.Cells(2).Text.ToString

                    sqlexp = "update dp_swm_mia_paczka_info set firma_id='" & f_id & "' where paczka_id = '" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    If f_id = "INPOST_ALLEGRO" Then
                        sqlexp = "update dp_swm_mia_paczka_info set pl_type='1' where paczka_id = '" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If

                    If f_id = "DHL_DE_API" Then
                        CBEmailSzyfrowanie.Checked = False
                    End If

                    If f_id = "UPS_DE_API" Then
                        CBEmailSzyfrowanie.Checked = False
                        sqlexp = "update dp_swm_mia_paczka_info set pl_type='02' where paczka_id = '" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        If Session("shipment_id") IsNot Nothing Then
                            sqlexp = "update dp_swm_mia_paczka_base set service_type='11',label_type='GIF' where shipment_id = '" & Session("shipment_id") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End If
                    End If

                    ''If firma_id = "&nbsp;" Or firma_id = "-1" Then
                    ''    gdr.Cells(3).Text = f_id
                    ''    sqlexp = "update dp_swm_mia_paczka_info set firma_id='" & f_id & "' where paczka_id = '" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                    ''    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    ''End If
                    LadujDaneGridViewPaczka()
                    ''##2024.05.17 / dodanie warunku na przelawanie okien dla wybranej firmy przewozowej
                    REFRESH_PAGE_CONTENT()
                Next
            End If
            conn.Close()

        End Using
    End Sub

    Protected Sub BZapiszPaczke_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BZapiszPaczke.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Dim walidacja_message As String = ""
        Dim walidacja_status As String = ""
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("firma_id") IsNot Nothing Then
                If Session("firma_id") = "DHL" Then
                    Dim sel_nr_zamow As String = LNrZamowieniaPaczka.Text
                    Dim sel_schemat As String = LSchematPaczka.Text
                    Dim sel_paczka_id As String = LPaczkaID.Text
                    ''wariant kiedy wprowadzamy nowa paczke 
                    If sel_paczka_id.Contains("PA") = False Then
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)

                        If walidacja_status = "1" And ((IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text) And (DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue <= "2" Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "1")) Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "3") Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = DirectCast(FindControl("DDLRodzaj" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim paleta_zwrot As String = DirectCast(FindControl("DDLPaleta" & Session("firma_id")), DropDownList).SelectedValue.ToString

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                            If szer = "" Then szer = "0"
                            Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                            If wys = "" Then wys = "0"
                            Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString
                            If dlug = "" Then dlug = "0"
                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim paczka_id As String = dajarSWMagazyn_MIA.MyFunction.GeneratePaczkaId
                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "select nr_zamow from dp_swm_mia_pak where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "' and etykieta_id='X'"
                            Dim czyRekordPakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                            If czyRekordPakowanie = "" Then
                                Dim ile_pakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct ile_poz from dp_swm_mia_buf where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "'", conn)
                                sqlexp = "insert into dp_swm_mia_pak (login,hash,nr_zamow,autodata,status,schemat,mag,etykieta_id,ile_poz) values('" & Session("mylogin") & "','" & Session("myhash") & "','" & sel_nr_zamow & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'),'PA','" & sel_schemat & "','" & Session("contentMagazyn") & "','X','" & ile_pakowanie & "')"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PA" & "[" & sel_schemat & "][" & Session("contentMagazyn") & "]", sel_nr_zamow)
                            End If


                            Dim shippment_id As String = Session("shipment_id")

                            ''##2024.05.22 / zmiany dotyczace tworzenia numeru shipment_id / pozostaje 1 SH dla 1 zamowienia
                            If shippment_id Is Nothing Or shippment_id = "" Then
                                shippment_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                            End If

                            sqlexp = "insert into dp_swm_mia_paczka_info (schemat,shipment_id,paczka_id,firma_id,pl_type,pl_weight,pl_width,pl_height,pl_length,pl_quantity,pl_non_std,pl_euro_ret) values('" & sel_schemat & "','" & shippment_id & "','" & paczka_id & "','" & Session("firma_id") & "','" & typ & "'," & waga & "," & szer & "," & wys & "," & dlug & "," & ile_paczek & ",'" & rodzaj & "','" & paleta_zwrot & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            GenerujInformacjeEtykieta(sel_nr_zamow)
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If

                        'sytuacja kiedy modyfikujemy wczesniej wprowadzona paczke
                    Else
                        Dim paczka_id As String = sel_paczka_id
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)
                        If walidacja_status = "1" And ((IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text) And (DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue <= "2" Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "1")) Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "3") Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = DirectCast(FindControl("DDLRodzaj" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim paleta_zwrot As String = DirectCast(FindControl("DDLPaleta" & Session("firma_id")), DropDownList).SelectedValue.ToString

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                            If szer = "" Then szer = "0"
                            Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                            If wys = "" Then wys = "0"
                            Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString
                            If dlug = "" Then dlug = "0"
                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "update dp_swm_mia_paczka_info set firma_id='" & firma_id & "',pl_type='" & typ & "', pl_non_std='" & rodzaj & "', pl_euro_ret='" & paleta_zwrot & "', pl_weight=" & waga & ",pl_width=" & szer & ",pl_height=" & wys & ",pl_length=" & dlug & ",pl_quantity=" & ile_paczek & " where paczka_id = '" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If
                    End If
                ElseIf Session("firma_id").ToString.Contains("INPOST") Then
                    Dim sel_nr_zamow As String = LNrZamowieniaPaczka.Text
                    Dim sel_schemat As String = LSchematPaczka.Text
                    Dim sel_paczka_id As String = LPaczkaID.Text
                    ''wariant kiedy wprowadzamy nowa paczke 
                    If sel_paczka_id.Contains("PA") = False Then
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)

                        If walidacja_status = "1" And ((IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text) And (DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue <= "2" Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "1")) Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "3") Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = DirectCast(FindControl("DDLRodzaj" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim paleta_zwrot As String = DirectCast(FindControl("DDLRodzajStandard" & Session("firma_id")), DropDownList).SelectedValue.ToString

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                            If szer = "" Then szer = "0"
                            Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                            If wys = "" Then wys = "0"
                            Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString
                            If dlug = "" Then dlug = "0"
                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim paczka_id As String = dajarSWMagazyn_MIA.MyFunction.GeneratePaczkaId
                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "select nr_zamow from dp_swm_mia_pak where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "' and etykieta_id='X'"
                            Dim czyRekordPakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                            If czyRekordPakowanie = "" Then
                                Dim ile_pakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct ile_poz from dp_swm_mia_buf where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "'", conn)
                                sqlexp = "insert into dp_swm_mia_pak (login,hash,nr_zamow,autodata,status,schemat,mag,etykieta_id,ile_poz) values('" & Session("mylogin") & "','" & Session("myhash") & "','" & sel_nr_zamow & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'),'PA','" & sel_schemat & "','" & Session("contentMagazyn") & "','X','" & ile_pakowanie & "')"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PA" & "[" & sel_schemat & "][" & Session("contentMagazyn") & "]", sel_nr_zamow)
                            End If


                            ''##2024.05.22 / zmiany dotyczace tworzenia numeru shipment_id / pozostaje 1 SH dla 1 zamowienia
                            Dim shippment_id As String = Session("shipment_id")
                            If shippment_id Is Nothing Or shippment_id = "" Then
                                shippment_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                            End If

                            sqlexp = "insert into dp_swm_mia_paczka_info (schemat,shipment_id,paczka_id,firma_id,pl_type,pl_weight,pl_width,pl_height,pl_length,pl_quantity,pl_non_std,pl_euro_ret) values('" & sel_schemat & "','" & shippment_id & "','" & paczka_id & "','" & Session("firma_id") & "','" & typ & "'," & waga & "," & szer & "," & wys & "," & dlug & "," & ile_paczek & ",'" & rodzaj & "','" & paleta_zwrot & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            GenerujInformacjeEtykieta(sel_nr_zamow)
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If

                        'sytuacja kiedy modyfikujemy wczesniej wprowadzona paczke
                    Else
                        Dim paczka_id As String = sel_paczka_id
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)
                        If walidacja_status = "1" And ((IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text) And (DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue <= "2" Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "1")) Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "3") Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = DirectCast(FindControl("DDLRodzaj" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim paleta_zwrot As String = DirectCast(FindControl("DDLRodzajStandard" & Session("firma_id")), DropDownList).SelectedValue.ToString

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                            If szer = "" Then szer = "0"
                            Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                            If wys = "" Then wys = "0"
                            Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString
                            If dlug = "" Then dlug = "0"
                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "update dp_swm_mia_paczka_info set firma_id='" & firma_id & "',pl_type='" & typ & "', pl_non_std='" & rodzaj & "', pl_euro_ret='" & paleta_zwrot & "', pl_weight=" & waga & ",pl_width=" & szer & ",pl_height=" & wys & ",pl_length=" & dlug & ",pl_quantity=" & ile_paczek & " where paczka_id = '" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If
                    End If
                ElseIf Session("firma_id").ToString.Contains("GEIS") Then
                    Dim sel_nr_zamow As String = LNrZamowieniaPaczka.Text
                    Dim sel_schemat As String = LSchematPaczka.Text
                    Dim sel_paczka_id As String = LPaczkaID.Text
                    ''wariant kiedy wprowadzamy nowa paczke 
                    If sel_paczka_id.Contains("PA") = False Then
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)

                        If walidacja_status = "1" And ((IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text) And (DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue <= "2" Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "1")) Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "3") Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = DirectCast(FindControl("DDLRodzaj" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            ''Dim paleta_zwrot As String = DirectCast(FindControl("DDLPaleta" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim paleta_zwrot As String = "0"

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            waga = waga.Replace(",", ".")
                            Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                            If szer = "" Then szer = "0"
                            szer = szer.Replace(",", ".")
                            Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                            If wys = "" Then wys = "0"
                            wys = wys.Replace(",", ".")
                            Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString
                            If dlug = "" Then dlug = "0"
                            dlug = dlug.Replace(",", ".")
                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim paczka_id As String = dajarSWMagazyn_MIA.MyFunction.GeneratePaczkaId
                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "select nr_zamow from dp_swm_mia_pak where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "' and etykieta_id='X'"
                            Dim czyRekordPakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                            If czyRekordPakowanie = "" Then
                                Dim ile_pakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct ile_poz from dp_swm_mia_buf where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "'", conn)
                                sqlexp = "insert into dp_swm_mia_pak (login,hash,nr_zamow,autodata,status,schemat,mag,etykieta_id,ile_poz) values('" & Session("mylogin") & "','" & Session("myhash") & "','" & sel_nr_zamow & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'),'PA','" & sel_schemat & "','" & Session("contentMagazyn") & "','X','" & ile_pakowanie & "')"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PA" & "[" & sel_schemat & "][" & Session("contentMagazyn") & "]", sel_nr_zamow)
                            End If

                            ''##2024.05.22 / zmiany dotyczace tworzenia numeru shipment_id / pozostaje 1 SH dla 1 zamowienia
                            Dim shippment_id As String = Session("shipment_id")
                            If shippment_id Is Nothing Or shippment_id = "" Then
                                shippment_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                            End If

                            sqlexp = "insert into dp_swm_mia_paczka_info (schemat,shipment_id,paczka_id,firma_id,pl_type,pl_weight,pl_width,pl_height,pl_length,pl_quantity,pl_non_std,pl_euro_ret) values('" & sel_schemat & "','" & shippment_id & "','" & paczka_id & "','" & Session("firma_id") & "','" & typ & "'," & waga & "," & szer & "," & wys & "," & dlug & "," & ile_paczek & ",'" & rodzaj & "','" & paleta_zwrot & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            GenerujInformacjeEtykieta(sel_nr_zamow)
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If

                        'sytuacja kiedy modyfikujemy wczesniej wprowadzona paczke
                    Else
                        Dim paczka_id As String = sel_paczka_id
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)
                        If walidacja_status = "1" And ((IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text) And (DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue <= "2" Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "1")) Or DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue >= "3") Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = DirectCast(FindControl("DDLRodzaj" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            ''Dim paleta_zwrot As String = DirectCast(FindControl("DDLPaleta" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim paleta_zwrot As String = "0"

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            waga = waga.Replace(",", ".")
                            Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                            If szer = "" Then szer = "0"
                            szer = szer.Replace(",", ".")
                            Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                            If wys = "" Then wys = "0"
                            wys = wys.Replace(",", ".")
                            Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString
                            If dlug = "" Then dlug = "0"
                            dlug = dlug.Replace(",", ".")
                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "update dp_swm_mia_paczka_info set firma_id='" & firma_id & "',pl_type='" & typ & "', pl_non_std='" & rodzaj & "', pl_euro_ret='" & paleta_zwrot & "', pl_weight=" & waga & ",pl_width=" & szer & ",pl_height=" & wys & ",pl_length=" & dlug & ",pl_quantity=" & ile_paczek & " where paczka_id = '" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If
                    End If

                ElseIf Session("firma_id") = "DHL_DE" Then
                    Session("dhl_opcje") = "1"
                    '####zapisz paczke DHL_DE
                    Dim sel_nr_zamow As String = LNrZamowieniaPaczka.Text
                    Dim sel_schemat As String = LSchematPaczka.Text
                    Dim sel_paczka_id As String = LPaczkaID.Text
                    ''wariant kiedy wprowadzamy nowa paczke 
                    If sel_paczka_id.Contains("PA") = False Then
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)
                        If walidacja_status = "1" And (IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text)) Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = "0"
                            Dim paleta_zwrot As String = "0"

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            Dim szer As String = "0"
                            Dim wys As String = "0"
                            Dim dlug As String = "0"
                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim paczka_id As String = dajarSWMagazyn_MIA.MyFunction.GeneratePaczkaId
                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "select nr_zamow from dp_swm_mia_pak where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "' and etykieta_id='X'"
                            Dim czyRekordPakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                            If czyRekordPakowanie = "" Then
                                Dim ile_pakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct ile_poz from dp_swm_mia_buf where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "'", conn)
                                sqlexp = "insert into dp_swm_mia_pak (login,hash,nr_zamow,autodata,status,schemat,mag,etykieta_id,ile_poz) values('" & Session("mylogin") & "','" & Session("myhash") & "','" & sel_nr_zamow & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'),'PA','" & sel_schemat & "','" & Session("contentMagazyn") & "','X','" & ile_pakowanie & "')"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PA" & "[" & sel_schemat & "][" & Session("contentMagazyn") & "]", sel_nr_zamow)
                            End If

                            ''##2024.05.22 / zmiany dotyczace tworzenia numeru shipment_id / pozostaje 1 SH dla 1 zamowienia
                            Dim shippment_id As String = Session("shipment_id")
                            If shippment_id Is Nothing Or shippment_id = "" Then
                                shippment_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                            End If

                            sqlexp = "insert into dp_swm_mia_paczka_info (schemat,shipment_id,paczka_id,firma_id,pl_type,pl_weight,pl_width,pl_height,pl_length,pl_quantity,pl_non_std,pl_euro_ret) values('" & sel_schemat & "','" & shippment_id & "','" & paczka_id & "','" & Session("firma_id") & "','" & typ & "'," & waga & "," & szer & "," & wys & "," & dlug & "," & ile_paczek & ",'" & rodzaj & "','" & paleta_zwrot & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            GenerujInformacjeEtykieta(sel_nr_zamow)
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If

                        'sytuacja kiedy modyfikujemy wczesniej wprowadzona paczke
                    Else
                        Dim paczka_id As String = sel_paczka_id
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)
                        If walidacja_status = "1" And IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text) Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = "0"
                            Dim paleta_zwrot As String = "0"

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            Dim szer As String = "0"
                            Dim wys As String = "0"
                            Dim dlug As String = "0"
                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "update dp_swm_mia_paczka_info set firma_id='" & firma_id & "',pl_type='" & typ & "', pl_non_std='" & rodzaj & "', pl_euro_ret='" & paleta_zwrot & "', pl_weight=" & waga & ",pl_width=" & szer & ",pl_height=" & wys & ",pl_length=" & dlug & ",pl_quantity=" & ile_paczek & " where paczka_id = '" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            ''sqlexp = "update dp_swm_mia_paczka set SR_COUNTRY='DE' where shipment_id = '" & Session("shipment_id") & "'"
                            ''result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If
                    End If
                ElseIf Session("firma_id") = "UPS_DE_API" Then
                    Session("dhl_opcje") = "1"
                    '####zapisz paczke DHL_DE
                    Dim sel_nr_zamow As String = LNrZamowieniaPaczka.Text
                    Dim sel_schemat As String = LSchematPaczka.Text
                    Dim sel_paczka_id As String = LPaczkaID.Text
                    ''wariant kiedy wprowadzamy nowa paczke 
                    If sel_paczka_id.Contains("PA") = False Then
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)

                        If walidacja_status = "1" And IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text) Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = "0"
                            Dim paleta_zwrot As String = "0"

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                            Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                            Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString

                            ''##2024.06.17 / walidacja paczki dla UPS_DE_API


                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim paczka_id As String = dajarSWMagazyn_MIA.MyFunction.GeneratePaczkaId
                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "select nr_zamow from dp_swm_mia_pak where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "' and etykieta_id='X'"
                            Dim czyRekordPakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                            If czyRekordPakowanie = "" Then
                                Dim ile_pakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct ile_poz from dp_swm_mia_buf where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "'", conn)
                                sqlexp = "insert into dp_swm_mia_pak (login,hash,nr_zamow,autodata,status,schemat,mag,etykieta_id,ile_poz) values('" & Session("mylogin") & "','" & Session("myhash") & "','" & sel_nr_zamow & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'),'PA','" & sel_schemat & "','" & Session("contentMagazyn") & "','X','" & ile_pakowanie & "')"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PA" & "[" & sel_schemat & "][" & Session("contentMagazyn") & "]", sel_nr_zamow)
                            End If

                            ''##2024.05.22 / zmiany dotyczace tworzenia numeru shipment_id / pozostaje 1 SH dla 1 zamowienia
                            Dim shippment_id As String = Session("shipment_id")
                            If shippment_id Is Nothing Or shippment_id = "" Then
                                shippment_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                            End If

                            sqlexp = "insert into dp_swm_mia_paczka_info (schemat,shipment_id,paczka_id,firma_id,pl_type,pl_weight,pl_width,pl_height,pl_length,pl_quantity,pl_non_std,pl_euro_ret) values('" & sel_schemat & "','" & shippment_id & "','" & paczka_id & "','" & Session("firma_id") & "','" & typ & "'," & waga & "," & szer & "," & wys & "," & dlug & "," & ile_paczek & ",'" & rodzaj & "','" & paleta_zwrot & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            GenerujInformacjeEtykieta(sel_nr_zamow)
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If

                        'sytuacja kiedy modyfikujemy wczesniej wprowadzona paczke
                    Else
                        Dim paczka_id As String = sel_paczka_id
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)
                        If walidacja_status = "1" And IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text) Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = "0"
                            Dim paleta_zwrot As String = "0"

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                            Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                            Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString
                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "update dp_swm_mia_paczka_info set firma_id='" & firma_id & "',pl_type='" & typ & "', pl_non_std='" & rodzaj & "', pl_euro_ret='" & paleta_zwrot & "', pl_weight=" & waga & ",pl_width=" & szer & ",pl_height=" & wys & ",pl_length=" & dlug & ",pl_quantity=" & ile_paczek & " where paczka_id = '" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            ''sqlexp = "update dp_swm_mia_paczka set SR_COUNTRY='DE' where shipment_id = '" & Session("shipment_id") & "'"
                            ''result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If
                    End If
                ElseIf Session("firma_id") = "DHL_DE_API" Then
                    Session("dhl_opcje") = "1"
                    '####zapisz paczke DHL_DE
                    Dim sel_nr_zamow As String = LNrZamowieniaPaczka.Text
                    Dim sel_schemat As String = LSchematPaczka.Text
                    Dim sel_paczka_id As String = LPaczkaID.Text
                    ''wariant kiedy wprowadzamy nowa paczke 
                    If sel_paczka_id.Contains("PA") = False Then
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)

                        If walidacja_status = "1" And IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text) Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = "0"
                            Dim paleta_zwrot As String = "0"

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                            Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                            Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString
                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim paczka_id As String = dajarSWMagazyn_MIA.MyFunction.GeneratePaczkaId
                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "select nr_zamow from dp_swm_mia_pak where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "' and etykieta_id='X'"
                            Dim czyRekordPakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                            If czyRekordPakowanie = "" Then
                                Dim ile_pakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct ile_poz from dp_swm_mia_buf where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "'", conn)
                                sqlexp = "insert into dp_swm_mia_pak (login,hash,nr_zamow,autodata,status,schemat,mag,etykieta_id,ile_poz) values('" & Session("mylogin") & "','" & Session("myhash") & "','" & sel_nr_zamow & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'),'PA','" & sel_schemat & "','" & Session("contentMagazyn") & "','X','" & ile_pakowanie & "')"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PA" & "[" & sel_schemat & "][" & Session("contentMagazyn") & "]", sel_nr_zamow)
                            End If

                            ''##2024.05.22 / zmiany dotyczace tworzenia numeru shipment_id / pozostaje 1 SH dla 1 zamowienia
                            Dim shippment_id As String = Session("shipment_id")
                            If shippment_id Is Nothing Or shippment_id = "" Then
                                shippment_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                            End If

                            sqlexp = "insert into dp_swm_mia_paczka_info (schemat,shipment_id,paczka_id,firma_id,pl_type,pl_weight,pl_width,pl_height,pl_length,pl_quantity,pl_non_std,pl_euro_ret) values('" & sel_schemat & "','" & shippment_id & "','" & paczka_id & "','" & Session("firma_id") & "','" & typ & "'," & waga & "," & szer & "," & wys & "," & dlug & "," & ile_paczek & ",'" & rodzaj & "','" & paleta_zwrot & "')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            GenerujInformacjeEtykieta(sel_nr_zamow)
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If

                        'sytuacja kiedy modyfikujemy wczesniej wprowadzona paczke
                    Else
                        Dim paczka_id As String = sel_paczka_id
                        walidacja_status = WalidacjaInformacjiEtykieta(walidacja_message)
                        If walidacja_status = "1" And IsNumeric(DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text) Then
                            Dim typ As String = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList).SelectedValue.ToString
                            Dim rodzaj As String = "0"
                            Dim paleta_zwrot As String = "0"

                            Dim waga As String = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox).Text.ToString
                            If waga = "" Then waga = "0"
                            Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                            Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                            Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString
                            Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox).Text.ToString

                            Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "update dp_swm_mia_paczka_info set firma_id='" & firma_id & "',pl_type='" & typ & "', pl_non_std='" & rodzaj & "', pl_euro_ret='" & paleta_zwrot & "', pl_weight=" & waga & ",pl_width=" & szer & ",pl_height=" & wys & ",pl_length=" & dlug & ",pl_quantity=" & ile_paczek & " where paczka_id = '" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            ''sqlexp = "update dp_swm_mia_paczka set SR_COUNTRY='DE' where shipment_id = '" & Session("shipment_id") & "'"
                            ''result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            CzyszczenieDanychSzczegolowychEtykieta()
                            LadujDaneGridViewPaczka()
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                            Session(session_id) += walidacja_message.ToString
                            Session(session_id) += "</div>"
                        End If
                    End If
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnulujPaczke_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnulujPaczek.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim sel_nr_zamow As String = GridViewPaczki.SelectedRow.Cells(2).Text.ToString()
            Dim sel_schemat As String = GridViewPaczki.SelectedRow.Cells(3).Text.ToString()
            Dim paczka_id As String = GridViewPaczki.SelectedRow.Cells(4).Text.ToString()

            sel_nr_zamow = LNrZamow.Text.ToString
            sel_schemat = LSchemat.Text.ToString
            paczka_id = LPaczkaID.Text.ToString

            If paczka_id.Contains("PA") Then
                sqlexp = "select count(*) from dp_swm_mia_paczka where shipment_id in(select shipment_id from dp_swm_mia_paczka_info where paczka_id='" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "')"
                Dim ile_paczka_info As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If ile_paczka_info = "0" Then
                    sqlexp = "delete from dp_swm_mia_paczka where shipment_id in (select shipment_id from dp_swm_mia_paczka_info where paczka_id='" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "')"
                    dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                End If

                sqlexp = "delete from dp_swm_mia_paczka_info where paczka_id='" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                sqlexp = "select count(*) from dp_swm_mia_paczka where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "'"
                Dim ilePaczekWZamowieniu As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If ilePaczekWZamowieniu = "0" Then
                    ''warunek usuwanie wpisu z dp_swm_mia_pak jezeli nie ma przypisanej zadnej paczki
                    sqlexp = "delete from dp_swm_mia_pak where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "' and etykieta_id='X'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                End If
                CzyszczenieDanychSzczegolowychEtykieta()
                GenerujInformacjeEtykieta(sel_nr_zamow)
                LadujDaneGridViewPaczka()
            End If
            conn.Close()
        End Using
    End Sub

    Public Sub LadujDaneGridView_SS_Service_type()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            GridView_SS_Service_type.DataSource = Nothing
            GridView_SS_Service_type.DataBind()

            If Session("shipment_id") <> "" Then
                sqlexp = "select ss_service_type,ss_service_value,SS_COLL_ON_FORM from dp_swm_mia_paczka_ss where shipment_id='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
            End If
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridView_SS_Service_type.DataSource = dsPartie.Tables(0)
            GridView_SS_Service_type.DataBind()
            cmd.Dispose()
            conn.Close()
        End Using
    End Sub

    Public Sub LadujDaneGridView_SS_Service_type_POP()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            GridView_SS_Service_type_DHL_PS.DataSource = Nothing
            GridView_SS_Service_type_DHL_PS.DataBind()

            If Session("shipment_id") <> "" Then
                sqlexp = "select ss_service_type,ss_service_value,SS_COLL_ON_FORM from dp_swm_mia_paczka_ss where shipment_id='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
            End If
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridView_SS_Service_type_DHL_PS.DataSource = dsPartie.Tables(0)
            GridView_SS_Service_type_DHL_PS.DataBind()
            cmd.Dispose()
            conn.Close()
        End Using
    End Sub

    Public Sub LadujDaneGridView_SS_Service_type_DHL_DE_API()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            GridView_SS_Service_type_DHL_DE_API.DataSource = Nothing
            GridView_SS_Service_type_DHL_DE_API.DataBind()

            If Session("shipment_id") <> "" Then
                sqlexp = "select ss_service_type,ss_service_value,SS_COLL_ON_FORM from dp_swm_mia_paczka_ss where shipment_id='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
            End If
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridView_SS_Service_type_DHL_DE_API.DataSource = dsPartie.Tables(0)
            GridView_SS_Service_type_DHL_DE_API.DataBind()
            cmd.Dispose()
            conn.Close()
        End Using
    End Sub

    Public Sub LadujDaneGridView_SS_Service_type_UPS_DE_API()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            GridView_SS_Service_type_UPS_DE_API.DataSource = Nothing
            GridView_SS_Service_type_UPS_DE_API.DataBind()

            If Session("shipment_id") <> "" Then
                sqlexp = "select ss_service_type,ss_service_value,SS_COLL_ON_FORM from dp_swm_mia_paczka_ss where shipment_id='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
            End If
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridView_SS_Service_type_UPS_DE_API.DataSource = dsPartie.Tables(0)
            GridView_SS_Service_type_UPS_DE_API.DataBind()
            cmd.Dispose()
            conn.Close()
        End Using
    End Sub


    Public Sub LadujDaneGridView_SS_Service_type_GEIS()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            GridView_SS_Service_type_GEIS.DataSource = Nothing
            GridView_SS_Service_type_GEIS.DataBind()

            If Session("shipment_id") <> "" Then
                sqlexp = "select ss_service_type,ss_service_value from dp_swm_mia_paczka_ss where shipment_id='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
            End If
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridView_SS_Service_type_GEIS.DataSource = dsPartie.Tables(0)
            GridView_SS_Service_type_GEIS.DataBind()
            cmd.Dispose()
            conn.Close()
        End Using
    End Sub

    Public Sub LadujDaneGridView_SS_Service_type_INPOST()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            GridView_SS_Service_type_INPOST.DataSource = Nothing
            GridView_SS_Service_type_INPOST.DataBind()

            If Session("shipment_id") <> "" Then
                sqlexp = "select ss_service_type,ss_service_value from dp_swm_mia_paczka_ss where shipment_id='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
            End If
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridView_SS_Service_type_INPOST.DataSource = dsPartie.Tables(0)
            GridView_SS_Service_type_INPOST.DataBind()
            cmd.Dispose()
            conn.Close()
        End Using
    End Sub

    Public Sub LadujDaneGridViewZamowienia()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            GridViewZamowienia.DataSource = Nothing
            GridViewZamowienia.DataBind()
            GridViewZamowienia.SelectedIndex = -1

            If Session("shipment_id") <> "" Then
                sqlexp = "select nr_zamow,schemat,shipment_id from dp_swm_mia_paczka where nr_zamow='" & Session("kod_dyspo") & "' and shipment_id='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                Dim cmd As New OracleCommand(sqlexp, conn)
                daPartie = New OracleDataAdapter(cmd)
                cb = New OracleCommandBuilder(daPartie)
                dsPartie = New DataSet()
                daPartie.Fill(dsPartie)
                GridViewZamowienia.DataSource = dsPartie.Tables(0)
                GridViewZamowienia.DataBind()
                cmd.Dispose()
            End If
            conn.Close()
        End Using
    End Sub

    Public Sub LadujDaneGridViewPrzesylki(ByVal nr_zamow As String, ByVal schemat As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim obj_grid_view As New GridView
            If Session("firma_id") IsNot Nothing Then
                If Session("firma_id") = "UPS_DE_API" Then
                    obj_grid_view = GridViewPrzesylki_UPS_DE_API
                ElseIf Session("firma_id") = "DHL_DE_API" Then
                    obj_grid_view = GridViewPrzesylki_DHL_DE_API
                ElseIf Session("firma_id") = "INPOST" Then
                    obj_grid_view = GridViewPrzesylki_INPOST
                ElseIf Session("firma_id") = "GEIS" Then
                    obj_grid_view = GridViewPrzesylki_GEIS
                Else
                    obj_grid_view = GridViewPrzesylki
                End If
            Else
                obj_grid_view = GridViewPrzesylki
            End If
            obj_grid_view.DataSource = Nothing
            obj_grid_view.DataBind()
            obj_grid_view.SelectedIndex = -1

            sqlexp = "select db.shipment_id,db.drop_off,db.service_type,db.label_type,db.content,db.comment_f,di.paczka_id,di.tracking_number" _
            & " from dp_swm_mia_paczka_base db, dp_swm_mia_paczka dp, dp_swm_mia_paczka_info di where di.paczka_id like '%/" & DateAndTime.Now.Year.ToString.Substring(2, 2) & "' and db.shipment_id = dp.shipment_id and db.shipment_id = di.shipment_id" _
            & " And dp.nr_zamow='" & nr_zamow & "' and dp.schemat='" & schemat & "' and dp.shipment_id not like 'S%' and db.comment_f like '" & nr_zamow & "%' order by db.shipment_id"
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            obj_grid_view.DataSource = dsPartie.Tables(0)
            obj_grid_view.DataBind()

            For Each row As GridViewRow In obj_grid_view.Rows
                Dim tracking_number As String = row.Cells(10).Text.ToString
                If tracking_number <> "" Then
                    Dim cb_link As HyperLink = row.FindControl("CBLink")
                    If cb_link IsNot Nothing Then
                        If Session("firma_id") = "DHL" Then
                            cb_link.NavigateUrl = "http://10.1.0.64:8084/upload/" & Session("firma_id").ToString.Replace("_API", "") & "/dhl_blp_" & tracking_number & ".pdf"
                            cb_link.Text = "ETYKIETA"
                        ElseIf Session("firma_id") = "INPOST" Then
                            cb_link.NavigateUrl = "http://10.1.0.64:8084/upload/" & Session("firma_id").ToString.Replace("_API", "") & "/inpost_" & tracking_number & ".pdf"
                            cb_link.Text = "ETYKIETA"
                        Else
                            cb_link.NavigateUrl = "http://10.1.0.64:8084/upload/" & Session("firma_id").ToString.Replace("_API", "") & "/" & tracking_number & ".pdf"
                            cb_link.Text = "ETYKIETA"
                        End If
                    End If
                End If
            Next
            cmd.Dispose()
            conn.Close()
        End Using
    End Sub


    Public Sub LadujDaneGridViewPaczka()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            GridViewPaczki.DataSource = Nothing
            GridViewPaczki.DataBind()
            GridViewPaczki.SelectedIndex = -1

            Dim shipment_grid As String = Session("shipment_id")
            Dim st_shipment_date As String = ""
            If shipment_grid.Contains("/") Then
                st_shipment_date = shipment_grid.Substring(shipment_grid.IndexOf("/") + 1, 2)
            End If

            If st_shipment_date = "" Then
                st_shipment_date = DateTime.Now.Year.ToString("D2").Substring(2, 2)
            End If

            ''Dim st_shipment_date As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select substr(st_shipment_date,3,2) from dp_swm_mia_paczka_info where shipment_id='" & shipment_grid & "' and paczka_id like '%/" & DateTime.Now.Year.ToString("D2").Substring(2, 2) & "'", conn)

            ''If shipment_grid.StartsWith("S") Then
            ''    shipment_grid = "S%" & shipment_grid.Substring(2, shipment_grid.Length - 2)
            ''End If

            If st_shipment_date = "" Then
                sqlexp = "select pi.paczka_id,pi.firma_id,case when trim(pi.pl_type)='01' then 'UPS Letter' when trim(pi.pl_type)='02' then 'Moje opakowanie' when trim(pi.pl_type)='03' then 'UPS Tube' when trim(pi.pl_type)='04' then 'UPS PAK' when trim(pi.pl_type)='21' then 'UPS Express Box' when trim(pi.pl_type)='30' then 'UPS Paleta' when trim(pi.pl_type)='1' then 'paczka' when trim(pi.pl_type)='2' then 'paleta' when trim(pi.pl_type)='3' then 'koperta' when trim(pi.pl_type)='4' then 'paczkomatA' when trim(pi.pl_type)='5' then 'paczkomatB' when trim(pi.pl_type)='6' then 'paczkomatC' when trim(pi.pl_type)='7' then 'paczka_poczta' when trim(pi.pl_type)='8' then 'paczka_inpost' when trim(pi.pl_type)='9' then 'paczka_poczta' when trim(pi.pl_type)='10' then 'paczka_cieszyn' when trim(pi.pl_type)='HP' then 'półpaleta [HP]' when trim(pi.pl_type)='EP' then 'bezzwrotna paleta [EP]' when trim(pi.pl_type)='CC' then 'colli [CC]' when trim(pi.pl_type)='FP' then 'europaleta [FP]' when trim(pi.pl_type)='NP' then 'paleta 1.0x2.0 [NP]' when trim(pi.pl_type)='VP' then '1/4 paleta [VP]' when trim(pi.pl_type)='PC' then 'chep [PC]' when trim(pi.pl_type)='DR' then 'przekracza europalete [DR]' when trim(pi.pl_type)='SP' then 'standard paleta [SP]' when trim(pi.pl_type)='PEP' then 'przekracza europalete [PEP]' end typ," _
                & " case when trim(pi.pl_non_std)='0' then 'standard' when trim(pi.pl_non_std)='20' then 'cargo ekspedycja [20]' when trim(pi.pl_non_std)='21' then 'cargo zamówienia [21]' when trim(pi.pl_non_std)='A' then 'rozmiar 8 x 38 x 64 cm [A]' when trim(pi.pl_non_std)='B' then 'rozmiar 19 x 38 x 64 cm [B]' when trim(pi.pl_non_std)='C' then 'rozmiar 41 x 38 x 64 cm [C]' when trim(pi.pl_non_std)='D' then 'rozmiar 50 x 50 x 80 cm [D]' when trim(pi.pl_non_std)='RECZNIE' then 'rozmiar 1 x 1 x 1 cm [RECZNIE]' else 'niestandard' end rodzaj, " _
                & " pi.pl_weight waga, pi.pl_width szer,pi.pl_height wys,pi.pl_length dlu,pi.pl_quantity ile_opak, pi.pl_euro_ret nstd from" _
                & " dp_swm_mia_paczka_info pi" _
                & " where pi.shipment_id like '" & shipment_grid & "' and pi.schemat='" & Session("schemat_dyspo") & "' order by pi.paczka_id asc"
            Else
                sqlexp = "select pi.paczka_id,pi.firma_id,case when trim(pi.pl_type)='01' then 'UPS Letter' when trim(pi.pl_type)='02' then 'Moje opakowanie' when trim(pi.pl_type)='03' then 'UPS Tube' when trim(pi.pl_type)='04' then 'UPS PAK' when trim(pi.pl_type)='21' then 'UPS Express Box' when trim(pi.pl_type)='30' then 'UPS Paleta' when trim(pi.pl_type)='1' then 'paczka' when trim(pi.pl_type)='2' then 'paleta' when trim(pi.pl_type)='3' then 'koperta' when trim(pi.pl_type)='4' then 'paczkomatA' when trim(pi.pl_type)='5' then 'paczkomatB' when trim(pi.pl_type)='6' then 'paczkomatC' when trim(pi.pl_type)='7' then 'paczka_poczta' when trim(pi.pl_type)='8' then 'paczka_inpost' when trim(pi.pl_type)='9' then 'paczka_poczta' when trim(pi.pl_type)='10' then 'paczka_cieszyn' when trim(pi.pl_type)='HP' then 'półpaleta [HP]' when trim(pi.pl_type)='EP' then 'bezzwrotna paleta [EP]' when trim(pi.pl_type)='CC' then 'colli [CC]' when trim(pi.pl_type)='FP' then 'europaleta [FP]' when trim(pi.pl_type)='NP' then 'paleta 1.0x2.0 [NP]' when trim(pi.pl_type)='VP' then '1/4 paleta [VP]' when trim(pi.pl_type)='PC' then 'chep [PC]' when trim(pi.pl_type)='DR' then 'przekracza europalete [DR]' when trim(pi.pl_type)='SP' then 'standard paleta [SP]' when trim(pi.pl_type)='PEP' then 'przekracza europalete [PEP]' end typ," _
                & " case when trim(pi.pl_non_std)='0' then 'standard' when trim(pi.pl_non_std)='20' then 'cargo ekspedycja [20]' when trim(pi.pl_non_std)='21' then 'cargo zamówienia [21]' when trim(pi.pl_non_std)='A' then 'rozmiar 8 x 38 x 64 cm [A]' when trim(pi.pl_non_std)='B' then 'rozmiar 19 x 38 x 64 cm [B]' when trim(pi.pl_non_std)='C' then 'rozmiar 41 x 38 x 64 cm [C]' when trim(pi.pl_non_std)='D' then 'rozmiar 50 x 50 x 80 cm [D]' when trim(pi.pl_non_std)='RECZNIE' then 'rozmiar 1 x 1 x 1 cm [RECZNIE]' else 'niestandard' end rodzaj, " _
                & " pi.pl_weight waga, pi.pl_width szer,pi.pl_height wys,pi.pl_length dlu,pi.pl_quantity ile_opak, pi.pl_euro_ret nstd from" _
                & " dp_swm_mia_paczka_info pi" _
                & " where pi.shipment_id like '" & shipment_grid & "' and pi.schemat='" & Session("schemat_dyspo") & "' and pi.paczka_id like '%/" & st_shipment_date & "' order by pi.paczka_id asc"
            End If

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

    Public Sub GenerujInformacjeEtykieta(ByVal nr_zamow As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim etykietaInput As String = nr_zamow & Date.Now.ToString
            LPaczkaID.Text = getMd5Hash(etykietaInput)
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLFirma(ByRef ddlObiekt As DropDownList, ByVal filtr As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ''If filtr <> "" Then
            ''    sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma where firma_id='" & filtr & "' and (aktywna='X') order by firma_nazwa asc"
            ''Else
            ''    sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma where aktywna='X' order by firma_nazwa asc"
            ''End If

            If filtr <> "" Then
                sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma where firma_id='" & filtr & "' and aktywna='X' order by firma_nazwa asc"
            Else
                sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma where aktywna='X' order by firma_nazwa asc"
            End If

            ddlObiekt.Items.Clear()
            ddlObiekt.Items.Add("")
            Dim cmd As New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            If dr.HasRows Then
                ddlObiekt.DataSource = dr
                ddlObiekt.DataTextField = "FIRMA_NAZWA"
                ddlObiekt.DataValueField = "FIRMA_ID"
                ddlObiekt.DataBind()
            End If
            dr.Close()
            cmd.Dispose()

            ddlObiekt.Items.Add(New System.Web.UI.WebControls.ListItem("WYBIERZ PRZEWOZNIKA", "-1", True))
            ddlObiekt.SelectedValue = "-1"
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLCountry(ByRef ddlObiekt As DropDownList, ByVal filtr As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ''If filtr <> "" Then
            ''    sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma where firma_id='" & filtr & "' and aktywna='X' order by firma_nazwa asc"
            ''Else
            ''    sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma where aktywna='X' order by firma_nazwa asc"
            ''End If

            If filtr <> "" Then
                sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma where firma_id='" & filtr & "' order by firma_nazwa asc"
            Else
                sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma order by firma_nazwa asc"
            End If


            ddlObiekt.Items.Clear()

            Dim itemIso As New List(Of String)
            itemIso.Add("PL")
            For Each ci As CultureInfo In CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                Dim inforeg As New RegionInfo(ci.LCID)
                If itemIso.Contains(inforeg.TwoLetterISORegionName.ToUpper.ToString) = False Then
                    itemIso.Add(inforeg.TwoLetterISORegionName.ToUpper.ToString)
                End If
            Next

            itemIso.Sort()

            For Each i In itemIso
                ddlObiekt.Items.Add(New System.Web.UI.WebControls.ListItem(i, i, True))
            Next
            conn.Close()
        End Using
    End Sub

    Protected Sub DDLFirma_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLFirma.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim f_id As String = DDLFirma.SelectedValue.ToString
            If f_id <> "-1" And f_id = "DHL_DE" Then
                Session("dhl_opcje") = "1"
                ''TB_SR_Country.Text = "DE"
                DDL_SR_COUNTRY.SelectedValue = "DE"
                CB_SR_IS_PACKSTATION.Enabled = True
                CB_SR_IS_POSTFILIALE.Enabled = True
            Else
                Session.Remove("dhl_opcje")
            End If


            ''2021.12.17 //ustawienie zmiennej firma_id
            If f_id <> "-1" Then
                Session("firma_id") = f_id
            End If

            If LPaczkaID.Text.ToString.Contains("PA") Then
                LadujDaneSzczegolowePaczki()
            Else
                ''2012.12.16 // sprawdzenie czy dp_swm_mia_paczka_base.SERVICE_TYPE jest zgodne z wybrana firma_id
                Dim service_type_old As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select SERVICE_TYPE from dp_swm_mia_paczka_base where COMMENT_F LIKE '%" & Session("kod_dyspo") & "%'", conn)
                Dim label_type_old As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select LABEL_TYPE from dp_swm_mia_paczka_base where COMMENT_F LIKE '%" & Session("kod_dyspo") & "%'", conn)

                Dim service_type_new As String = ""
                Dim label_type_new As String = ""

                Dim service_type_correct As Boolean = False
                If Session("firma_id") IsNot Nothing Then
                    Dim ddl_service_type As DropDownList
                    If Session("firma_id").ToString.Contains("GEIS") Then
                        ddl_service_type = DirectCast(FindControl("DDL_SERVICE_TYPE_GEIS"), DropDownList)
                        service_type_new = "21"
                        label_type_new = "1"
                    ElseIf Session("firma_id").ToString.Contains("INPOST") Then
                        ddl_service_type = DirectCast(FindControl("DDL_SERVICE_TYPE_INPOST"), DropDownList)
                        service_type_new = "inpost_courier_standard"
                        label_type_new = "pdf"
                    ElseIf Session("firma_id").ToString.Contains("DHL_DE") Then
                        ddl_service_type = DirectCast(FindControl("DDL_SERVICE_TYPE"), DropDownList)
                        service_type_new = "EPN"
                        label_type_new = "BLP"
                    ElseIf Session("firma_id").ToString.Contains("DHL_PS") Then
                        ddl_service_type = DirectCast(FindControl("DDL_SERVICE_TYPE"), DropDownList)
                        service_type_new = "AH"
                        label_type_new = "BLP"
                    Else
                        ddl_service_type = DirectCast(FindControl("DDL_SERVICE_TYPE"), DropDownList)
                        service_type_new = "AH"
                        label_type_new = "BLP"
                    End If

                    For Each obj_ddl As System.Web.UI.WebControls.ListItem In ddl_service_type.Items
                        If obj_ddl.Value.ToString = service_type_old Then
                            service_type_correct = True
                            Exit For
                        End If
                    Next

                    If service_type_correct = False Then
                        ''service_type = ddl_service_type.Items(0).Value.ToString
                        sqlexp = "update dp_swm_mia_paczka_base set SERVICE_TYPE='" & service_type_new & "',LABEL_TYPE='" & label_type_new & "' where COMMENT_F LIKE '%" & Session("kod_dyspo") & "%'"
                        dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If

                End If

                ''''ManageTableFirmaPrzewozowa()
                ClearGVPaczki()
                GenerujInformacjeEtykieta(Session("kod_dyspo"))
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub DDL_SR_COUNTRY_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDL_SR_COUNTRY.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            sqlexp = "SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
            Dim cmd As New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            Try
                While dr.Read()
                    Dim shippment_id As String = dr.Item(0).ToString
                    Dim countryISO As String = DDL_SR_COUNTRY.SelectedValue.ToString
                    Dim countryName As String = dhlDESoapRequest.dhlGetCountryLanguageName(countryISO)
                    If countryName <> "-1" Then
                        TB_Country.Text = countryName
                        sqlexp = "update dp_swm_mia_paczka set SR_COUNTRY='" & countryISO & "' where shipment_id='" & shippment_id & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If

                End While
            Catch ex As System.ArgumentOutOfRangeException
                Console.WriteLine(ex)
            End Try
            dr.Close()
            cmd.Dispose()

            ''For Each ci As CultureInfo In CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            ''    Dim inforeg As New RegionInfo(ci.LCID)
            ''    If inforeg.TwoLetterISORegionName.ToString = countryISO Then
            ''        TB_Country.Text = inforeg.EnglishName.ToString()
            ''        sqlexp = "update dp_swm_mia_paczka set SR_COUNTRY='" & countryISO & "' where shipment_id='" & shippment_id & "'"
            ''        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
            ''    End If
            ''Next
            conn.Close()
        End Using
    End Sub

    Protected Sub LadujDaneSzczegolowePaczki()
        Dim row As GridViewRow = GridViewPaczki.SelectedRow
        ''ManageTableFirmaPrzewozowa()
        Dim typ As String = row.Cells(4).Text.ToString
        Dim ddlTypObj As DropDownList = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList)
        If Session("firma_id").ToString.Contains("DHL") Then
            If ddlTypObj IsNot Nothing Then
                If typ = "paczka" Then : ddlTypObj.SelectedValue = 1
                ElseIf typ = "paleta" Then : ddlTypObj.SelectedValue = 2
                ElseIf typ = "koperta" Then : ddlTypObj.SelectedValue = 3
                ElseIf typ = "paczkomatA" Then : ddlTypObj.SelectedValue = 4
                ElseIf typ = "paczkomatB" Then : ddlTypObj.SelectedValue = 5
                ElseIf typ = "paczkomatC" Then : ddlTypObj.SelectedValue = 6
                End If
            End If
        End If

        Dim rodzaj As String = row.Cells(5).Text.ToString
        Dim ddlRodzajObj As DropDownList = DirectCast(FindControl("DDLRodzaj" & Session("firma_id")), DropDownList)
        If Session("firma_id").ToString.Contains("DHL") Then
            If ddlRodzajObj IsNot Nothing Then
                If rodzaj = "standard" Then : ddlRodzajObj.SelectedValue = 0
                Else : ddlRodzajObj.SelectedValue = 1
                End If
            End If
        End If

        Dim wagaObj As TextBox = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox)
        If wagaObj IsNot Nothing Then
            wagaObj.Text = row.Cells(6).Text.ToString
        End If

        Dim szerObj As TextBox = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox)
        If szerObj IsNot Nothing Then
            szerObj.Text = row.Cells(7).Text.ToString
        End If

        Dim wysObj As TextBox = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox)
        If wysObj IsNot Nothing Then
            wysObj.Text = row.Cells(8).Text.ToString
        End If

        Dim dluObj As TextBox = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox)
        If dluObj IsNot Nothing Then
            dluObj.Text = row.Cells(9).Text.ToString
        End If

        Dim ilePaczekObj As TextBox = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox)
        If ilePaczekObj IsNot Nothing Then
            ilePaczekObj.Text = row.Cells(10).Text.ToString
        End If

    End Sub

    Protected Sub DDLRodzajINPOST_ALLEGRO_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLRodzajINPOST_ALLEGRO.SelectedIndexChanged
        'Rozmiar A: 8 x 38 x 64 cm
        'Rozmiar B: 19 x 38 x 64 cm
        'Rozmiar C: 41 x 38 x 64 cm
        'Rozmiar D: 50 x 50 x 80 cm
        Dim t_id As String = DDLRodzajINPOST_ALLEGRO.SelectedValue.ToString

        Dim tb As TextBox = Nothing
        If t_id = "A" Then
            tb = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "8"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "38"
            tb = FindControl("TBDlug" & Session("firma_id"))
            tb.Text = "64"
            ''tb = FindControl("TBWaga" & Session("firma_id"))
            ''tb.Text = "1"
        ElseIf t_id = "B" Then
            tb = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "19"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "38"
            tb = FindControl("TBDlug" & Session("firma_id"))
            tb.Text = "64"
            ''tb = FindControl("TBWaga" & Session("firma_id"))
            ''tb.Text = "1"
        ElseIf t_id = "C" Then
            tb = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "41"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "38"
            tb = FindControl("TBDlug" & Session("firma_id"))
            tb.Text = "64"
            ''tb = FindControl("TBWaga" & Session("firma_id"))
            ''tb.Text = "1"
        ElseIf t_id = "D" Then
            tb = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "50"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "50"
            tb = FindControl("TBDlug" & Session("firma_id"))
            tb.Text = "80"
            ''tb = FindControl("TBWaga" & Session("firma_id"))
            ''tb.Text = "1"
        ElseIf t_id = "RECZNIE" Then
            tb = FindControl("TBWys" & Session("firma_id"))
            tb.Text = ""
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = ""
            tb = FindControl("TBDlug" & Session("firma_id"))
            tb.Text = ""
            ''tb = FindControl("TBWaga" & Session("firma_id"))
            ''tb.Text = "1"
        End If
    End Sub

    Protected Sub DDLTypDHL_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLTypDHL.SelectedIndexChanged
        'Standardowa paczka dhl nadawana w Polsce - wymiary max. 140 x 60 x 60cm
        'Standardowa paleta dhl – max. wymiary palety 80 x 120cm., wysokość do 210cm z podstawą
        Dim t_id As String = DDLTypDHL.SelectedValue.ToString

        If t_id = "1" Then
            Dim tb As TextBox = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBDLug" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "1"
        ElseIf t_id = "2" Then
            Dim tb As TextBox = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "210"
            tb = FindControl("TBDLug" & Session("firma_id"))
            tb.Text = "120"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "80"
        ElseIf t_id = "3" Then
            Dim tb As TextBox = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBDLug" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "1"
        End If
    End Sub

    Protected Sub DDLTypDHL_PS_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLTypDHL_PS.SelectedIndexChanged
        'Standardowa paczka dhl nadawana w Polsce - wymiary max. 140 x 60 x 60cm
        'Standardowa paleta dhl – max. wymiary palety 80 x 120cm., wysokość do 210cm z podstawą
        Dim t_id As String = DDLTypDHL_PS.SelectedValue.ToString

        If t_id = "1" Then
            Dim tb As TextBox = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBDLug" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "1"
        ElseIf t_id = "2" Then
            Dim tb As TextBox = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "210"
            tb = FindControl("TBDLug" & Session("firma_id"))
            tb.Text = "120"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "80"
        ElseIf t_id = "3" Then
            Dim tb As TextBox = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBDLug" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "1"
        End If
    End Sub

    Protected Sub DDLTypDHL_DE_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLTypDHL_DE.SelectedIndexChanged
        'Standardowa paczka dhl nadawana poza Polskę - wymiary max. 120 x 60 x 60cm
        'Standardowa paleta dhl – max. wymiary palety 80 x 120cm., wysokość do 210cm z podstawą
        Dim t_id As String = DDLTypDHL_DE.SelectedValue.ToString

        If t_id = "1" Then
            Dim tb As TextBox = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBDLug" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "1"
        ElseIf t_id = "2" Then
            Dim tb As TextBox = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "210"
            tb = FindControl("TBDLug" & Session("firma_id"))
            tb.Text = "120"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "80"
        ElseIf t_id = "3" Then
            Dim tb As TextBox = FindControl("TBWys" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBDLug" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "1"
        End If
    End Sub

    Protected Function GenerateFirmaTable() As List(Of String)
        Dim firma_id As New List(Of String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ''sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma where aktywna = 'X' order by firma_nazwa asc"
            sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma order by firma_nazwa asc"
            Dim cmd As New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            Try
                While dr.Read()
                    If firma_id.Contains(dr.Item(0).ToString) = False Then
                        firma_id.Add(dr.Item(0).ToString)
                    End If
                End While
            Catch ex As System.ArgumentOutOfRangeException
                Console.WriteLine(ex)
            End Try
            dr.Close()
            cmd.Dispose()
            conn.Close()

        End Using
        Return firma_id
    End Function

    Protected Sub ManageTableFirmaPrzewozowa()
        Dim f_id As String = DDLFirma.SelectedValue.ToString
        Dim firma_id As List(Of String) = GenerateFirmaTable()
        For Each tobj In firma_id
            Dim tabelaObj As Table = DirectCast(FindControl("Table" & tobj), Table)
            If tabelaObj IsNot Nothing Then
                tabelaObj.Visible = False
            End If

            Dim panelObj As Panel = DirectCast(FindControl("Panel" & tobj), Panel)
            If panelObj IsNot Nothing Then
                panelObj.Visible = False
            End If
        Next

        Dim temp As Table = DirectCast(FindControl("Table" & f_id), Table)
        If temp IsNot Nothing Then
            temp.Visible = True
            TablePackageButton.Visible = True
            Session("firma_id") = f_id

        Else
            TablePackageButton.Visible = False
            Session.Remove("firma_id")
        End If
    End Sub

    Protected Sub ClearGVPaczki()
        GridViewPaczki.SelectedIndex = -1
        For Each row As GridViewRow In GridViewPaczki.Rows
            Dim cbT As CheckBox = row.FindControl("CBKodSelect")
            cbT.Checked = False
        Next

        ManageTableRowFirmaPrzewozowa()
    End Sub

    Protected Sub ManageTableRowFirmaPrzewozowa()
        Dim firma_id As List(Of String) = GenerateFirmaTable()
        Dim cb As DropDownList = Nothing
        For Each f_id In firma_id
            If f_id = Session("firma_id") Then

                If f_id.Contains("INPOST") Then
                    cb = FindControl("DDLTyp" & f_id)
                    cb.SelectedValue = "1"

                    Dim temp As TextBox = FindControl("TBWys" & f_id)
                    temp.Text = "38"
                    temp = FindControl("TBDLug" & f_id)
                    temp.Text = "64"
                    temp = FindControl("TBSzer" & f_id)
                    temp.Text = "8"
                    temp = FindControl("TBWaga" & f_id)
                    temp.Text = ""

                Else
                    cb = FindControl("DDLRodzaj" & f_id)
                    If cb IsNot Nothing Then
                        If f_id.Contains("GEIS") Then
                            cb.SelectedValue = "21"
                        Else
                            cb.SelectedValue = "0"
                        End If
                    End If

                    cb = FindControl("DDLTyp" & f_id)
                    If cb IsNot Nothing Then
                        If f_id.Contains("DHL") Then
                            cb.SelectedValue = "1"

                            If f_id = "DHL" Then
                                Dim temp As TextBox = FindControl("TBWys" & f_id)
                                temp.Text = "1"
                                temp = FindControl("TBDLug" & f_id)
                                temp.Text = "1"
                                temp = FindControl("TBSzer" & f_id)
                                temp.Text = "1"
                                temp = FindControl("TBWaga" & f_id)
                                temp.Text = ""
                            End If

                        ElseIf f_id.Contains("GEIS_DAJAR_KRAJ") Then
                            cb.SelectedValue = "NP"

                            Dim temp As TextBox = FindControl("TBWys" & f_id)
                            temp.Text = "0,5"
                            temp = FindControl("TBDLug" & f_id)
                            temp.Text = "2,24"
                            temp = FindControl("TBSzer" & f_id)
                            temp.Text = "0,9"
                            temp = FindControl("TBWaga" & f_id)
                            temp.Text = "90"
                        ElseIf f_id.Contains("GEIS_HIPO") Then
                            cb.SelectedValue = "VP"

                            Dim temp As TextBox = FindControl("TBWys" & f_id)
                            temp.Text = ""
                            temp = FindControl("TBDLug" & f_id)
                            temp.Text = ""
                            temp = FindControl("TBSzer" & f_id)
                            temp.Text = ""
                            temp = FindControl("TBWaga" & f_id)
                            temp.Text = ""
                        ElseIf f_id.Contains("GEIS_COD_CZECHY") Then
                            cb.SelectedValue = "VP"

                            Dim temp As TextBox = FindControl("TBWys" & f_id)
                            temp.Text = ""
                            temp = FindControl("TBDLug" & f_id)
                            temp.Text = ""
                            temp = FindControl("TBSzer" & f_id)
                            temp.Text = ""
                            temp = FindControl("TBWaga" & f_id)
                            temp.Text = ""
                        ElseIf f_id.Contains("GEIS_COD_SLOWACJA") Then
                            cb.SelectedValue = "VP"

                            Dim temp As TextBox = FindControl("TBWys" & f_id)
                            temp.Text = ""
                            temp = FindControl("TBDLug" & f_id)
                            temp.Text = ""
                            temp = FindControl("TBSzer" & f_id)
                            temp.Text = ""
                            temp = FindControl("TBWaga" & f_id)
                            temp.Text = ""

                        End If
                    End If

                    cb = FindControl("DDLPaleta" & f_id)
                    If cb IsNot Nothing Then
                        cb.SelectedValue = "0"
                    End If
                End If

                Dim tb As TextBox = FindControl("TBIlePaczek" & f_id)
                If tb IsNot Nothing Then
                    tb.Text = "1"
                End If
            End If
        Next
    End Sub

    Public Sub LadujDaneGridViewPakowanie()
        ''ManageTableFirmaPrzewozowa()
        PanelDodawaniePaczki.Visible = True
        LNrZamowieniaPaczka.Text = Session("kod_dyspo")
        LSchematPaczka.Text = Session("schemat_dyspo")
        GenerujInformacjeEtykieta(Session("kod_dyspo"))
        LadujDaneGridViewPaczka()
    End Sub

    Public Sub CzyszczenieDanychSzczegolowychEtykieta()
        ManageTableRowFirmaPrzewozowa()

        LPaczkaID.Text = ""
        DDLFirma.SelectedValue = "-1"
    End Sub

    Protected Sub GridViewPrzesylki_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewPrzesylki.SelectedIndexChanged
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Dim cb As CheckBox = GridViewPrzesylki.SelectedRow.FindControl("CBKodSelect")
        Dim rowid_paczki As Integer = GridViewPrzesylki.SelectedRow.RowIndex
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
            Session.Remove("shipment_id")
            Session.Remove("gvprzesylki_rowid")
            ''HLEtykietaLP.NavigateUrl = ""
            ''HLEtykietaLP.Text = "etykieta LP"
            ''HLEtykietaBLP.NavigateUrl = ""
            ''HLEtykietaBLP.Text = "etykieta BLP"
            ''HLEtykietaZBLP.NavigateUrl = ""
            ''HLEtykietaZBLP.Text = "etykieta ZBLP"
        Else
            cb.Checked = True
            GridViewPrzesylki.SelectedIndex = rowid_paczki
            Session("shipment_id") = GridViewPrzesylki.SelectedRow.Cells(3).Text.ToString
            Session("tracking_number") = GridViewPrzesylki.SelectedRow.Cells(10).Text.ToString

            Session("gvprzesylki_rowid") = GridViewPrzesylki.SelectedIndex
            LNr_list_przewozowy.Text = Session("tracking_number")

            ''If Session("firma_id") = "DHL" Then
            ''    Dim file_dhl As String = "dhl_LP_" & Session("shipment_id") & ".pdf"
            ''    If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
            ''        HLEtykietaLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_LP_" & Session("shipment_id") & ".pdf"
            ''        HLEtykietaLP.Text = file_dhl
            ''    End If

            ''    file_dhl = "dhl_BLP_" & Session("shipment_id") & ".pdf"
            ''    If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
            ''        HLEtykietaBLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_BLP_" & Session("shipment_id") & ".pdf"
            ''        HLEtykietaBLP.Text = file_dhl
            ''    End If

            ''    file_dhl = "dhl_ZBLP_" & Session("shipment_id") & ".zpl"
            ''    If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl\" & file_dhl) Then
            ''        HLEtykietaZBLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl/" & "dhl_ZBLP_" & Session("shipment_id") & ".zpl"
            ''        HLEtykietaZBLP.Text = file_dhl
            ''    End If
            ''ElseIf Session("firma_id") = "DHL_DE" Then
            ''    Dim file_dhl As String = Session("shipment_id") & ".pdf"
            ''    If File.Exists(dajarSWMagazyn_MIA.MyFunction.networkPath & "\dhl_de\" & file_dhl) Then
            ''        HLEtykietaLP.NavigateUrl = "http://10.1.0.64:8084/upload/dhl_de/" & file_dhl
            ''        HLEtykietaLP.Text = file_dhl
            ''    End If
            ''End If
        End If
    End Sub

    Protected Sub GridViewPrzesylki_UPS_DE_API_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewPrzesylki_UPS_DE_API.SelectedIndexChanged
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Dim cb As CheckBox = GridViewPrzesylki_UPS_DE_API.SelectedRow.FindControl("CBKodSelect")
        Dim rowid_paczki As Integer = GridViewPrzesylki_UPS_DE_API.SelectedRow.RowIndex
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
            Session.Remove("shipment_id")
            Session.Remove("gvprzesylki_rowid")
        Else
            cb.Checked = True
            GridViewPrzesylki_UPS_DE_API.SelectedIndex = rowid_paczki
            Session("shipment_id") = GridViewPrzesylki_UPS_DE_API.SelectedRow.Cells(3).Text.ToString
            Session("tracking_number") = GridViewPrzesylki_UPS_DE_API.SelectedRow.Cells(10).Text.ToString
            Session("gvprzesylki_rowid") = GridViewPrzesylki_UPS_DE_API.SelectedIndex
            LNr_list_przewozowy.Text = Session("tracking_number")

        End If
    End Sub

    Protected Sub GridViewPrzesylki_INPOST_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewPrzesylki_INPOST.SelectedIndexChanged
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Dim cb As CheckBox = GridViewPrzesylki_INPOST.SelectedRow.FindControl("CBKodSelect")
        Dim rowid_paczki As Integer = GridViewPrzesylki_INPOST.SelectedRow.RowIndex
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
            Session.Remove("shipment_id")
            Session.Remove("gvprzesylki_rowid")
        Else
            cb.Checked = True
            GridViewPrzesylki_INPOST.SelectedIndex = rowid_paczki
            Session("shipment_id") = GridViewPrzesylki_INPOST.SelectedRow.Cells(3).Text.ToString
            Session("tracking_number") = GridViewPrzesylki_INPOST.SelectedRow.Cells(10).Text.ToString
            Session("gvprzesylki_rowid") = GridViewPrzesylki_INPOST.SelectedIndex
            LNr_list_przewozowy.Text = Session("tracking_number")
        End If
    End Sub

    Protected Sub GridViewPrzesylki_GEIS_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewPrzesylki_GEIS.SelectedIndexChanged
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Dim cb As CheckBox = GridViewPrzesylki_GEIS.SelectedRow.FindControl("CBKodSelect")
        Dim rowid_paczki As Integer = GridViewPrzesylki_GEIS.SelectedRow.RowIndex
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
            Session.Remove("shipment_id")
            Session.Remove("gvprzesylki_rowid")
        Else
            cb.Checked = True
            GridViewPrzesylki_GEIS.SelectedIndex = rowid_paczki
            Session("shipment_id") = GridViewPrzesylki_GEIS.SelectedRow.Cells(3).Text.ToString
            Session("tracking_number") = GridViewPrzesylki_GEIS.SelectedRow.Cells(10).Text.ToString
            Session("gvprzesylki_rowid") = GridViewPrzesylki_GEIS.SelectedIndex
            LNr_list_przewozowy.Text = Session("tracking_number")

        End If
    End Sub

    Protected Sub GridViewPrzesylki_DHL_DE_API_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewPrzesylki_DHL_DE_API.SelectedIndexChanged
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Dim cb As CheckBox = GridViewPrzesylki_DHL_DE_API.SelectedRow.FindControl("CBKodSelect")
        Dim rowid_paczki As Integer = GridViewPrzesylki_DHL_DE_API.SelectedRow.RowIndex
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
            Session.Remove("shipment_id")
            Session.Remove("gvprzesylki_rowid")
        Else
            cb.Checked = True
            GridViewPrzesylki_DHL_DE_API.SelectedIndex = rowid_paczki
            Session("shipment_id") = GridViewPrzesylki_DHL_DE_API.SelectedRow.Cells(3).Text.ToString
            Session("tracking_number") = GridViewPrzesylki_DHL_DE_API.SelectedRow.Cells(10).Text.ToString
            Session("gvprzesylki_rowid") = GridViewPrzesylki_DHL_DE_API.SelectedIndex
            LNr_list_przewozowy.Text = Session("tracking_number")

        End If
    End Sub

    Protected Sub GridViewPaczki_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewPaczki.SelectedIndexChanged
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Dim cb As CheckBox = GridViewPaczki.SelectedRow.FindControl("CBKodSelect")
        Dim rowid_paczki As Integer = GridViewPaczki.SelectedRow.RowIndex
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
            CzyszczenieDanychSzczegolowychEtykieta()
            ''ManageTableFirmaPrzewozowa()
            GenerujInformacjeEtykieta(Session("kod_dyspo"))
        Else
            ClearGVPaczki()
            cb.Checked = True
            GridViewPaczki.SelectedIndex = rowid_paczki
            Dim row As GridViewRow = GridViewPaczki.SelectedRow
            ''LNrZamowieniaPaczka.Text = row.Cells(2).Text.ToString
            ''LSchematPaczka.Text = row.Cells(3).Text.ToString
            LPaczkaID.Text = row.Cells(2).Text.ToString

            '####ZMIANA DOMYSLNEGO TYPU WYBORU PRZEWOZNIKA
            Dim typ As String = row.Cells(4).Text.ToString
            If typ.Contains("paczkomat") Then : RefreshDDLFirma(DDLFirma, "INPOST")
            Else : RefreshDDLFirma(DDLFirma, "")
            End If

            If row.Cells(3).Text.ToString = "&nbsp;" Then
                DDLFirma.SelectedValue = "-1"
            Else

                If row.Cells(3).Text.ToString = "-1" Then
                    DDLFirma.SelectedValue = "-1"
                    Session("firma_id") = row.Cells(3).Text.ToString
                Else
                    DDLFirma.SelectedValue = row.Cells(3).Text.ToString
                    Session("firma_id") = row.Cells(3).Text.ToString
                    Dim ddlTypObj As DropDownList = DirectCast(FindControl("DDLTyp" & Session("firma_id")), DropDownList)
                    If Session("firma_id") = "DHL_DE" Then
                        If typ = "paczka" Then : ddlTypObj.SelectedValue = 1
                        End If
                    ElseIf Session("firma_id") = "DHL" Then
                        If typ = "paczka" Then : ddlTypObj.SelectedValue = 1
                        ElseIf typ = "paleta" Then : ddlTypObj.SelectedValue = 2
                        ElseIf typ = "koperta" Then : ddlTypObj.SelectedValue = 3
                        ElseIf typ = "paczkomatA" Then : ddlTypObj.SelectedValue = 4
                        ElseIf typ = "paczkomatB" Then : ddlTypObj.SelectedValue = 5
                        ElseIf typ = "paczkomatC" Then : ddlTypObj.SelectedValue = 6
                        End If
                    ElseIf Session("firma_id").ToString.Contains("GEIS") Then
                        If typ.Contains("[HP]") Then : ddlTypObj.SelectedValue = "HP"
                        ElseIf typ.Contains("[HP]") Then : ddlTypObj.SelectedValue = "EP"
                        ElseIf typ.Contains("[CC]") Then : ddlTypObj.SelectedValue = "CC"
                        ElseIf typ.Contains("[FP]") Then : ddlTypObj.SelectedValue = "FP"
                        ElseIf typ.Contains("[NP]") Then : ddlTypObj.SelectedValue = "NP"
                        ElseIf typ.Contains("[VP]") Then : ddlTypObj.SelectedValue = "VP"
                        ElseIf typ.Contains("[PC]") Then : ddlTypObj.SelectedValue = "PC"
                        ElseIf typ.Contains("[PEP]") Then : ddlTypObj.SelectedValue = "PEP"
                        End If
                    ElseIf Session("firma_id").ToString.Contains("UPS_DE_API") Then
                        ''<asp:ListItem Value = "02" > Moje opakowanie</asp: ListItem>
                        ''<asp:ListItem Value="01">UPS Letter</asp:ListItem>
                        ''<asp:ListItem Value = "03" > UPS Tube</asp: ListItem>
                        ''<asp:ListItem Value="04">UPS PAK</asp:ListItem>
                        ''<asp:ListItem Value = "21" > UPS Express Box</asp: ListItem>
                        ''<asp:ListItem Value="30">UPS Paleta</asp:ListItem>

                        If typ.Contains("Moje opakowanie") Then : ddlTypObj.SelectedValue = "02"
                        ElseIf typ.Contains("UPS Letter") Then : ddlTypObj.SelectedValue = "01"
                        ElseIf typ.Contains("UPS Tube") Then : ddlTypObj.SelectedValue = "03"
                        ElseIf typ.Contains("UPS PAK") Then : ddlTypObj.SelectedValue = "04"
                        ElseIf typ.Contains("UPS Express Box") Then : ddlTypObj.SelectedValue = "21"
                        ElseIf typ.Contains("UPS Paleta") Then : ddlTypObj.SelectedValue = "30"
                        End If
                    End If

                    Dim rodzaj As String = row.Cells(5).Text.ToString
                    Dim ddlRodzajObj As DropDownList = DirectCast(FindControl("DDLRodzaj" & Session("firma_id")), DropDownList)
                    If ddlRodzajObj IsNot Nothing Then

                        If Session("firma_id") = "DHL_DE" Then
                            If rodzaj = "standard" Then : ddlRodzajObj.SelectedValue = 0
                            Else : ddlRodzajObj.SelectedValue = 1
                            End If
                        ElseIf Session("firma_id") = "DHL" Then
                            If rodzaj = "standard" Then : ddlRodzajObj.SelectedValue = 0
                            Else : ddlRodzajObj.SelectedValue = 1
                            End If
                        ElseIf Session("firma_id").ToString.Contains("GEIS") Then
                            If rodzaj = "cargo ekspedycja [20]" Then : ddlRodzajObj.SelectedValue = "20"
                            ElseIf rodzaj = "cargo zamówienia [21]" Then : ddlRodzajObj.SelectedValue = "21"
                            End If
                        ElseIf Session("firma_id").ToString = "INPOST_ALLEGRO" Then
                            If rodzaj.Contains("[A]") Then : ddlRodzajObj.SelectedValue = "A"
                            ElseIf rodzaj.Contains("[B]") Then : ddlRodzajObj.SelectedValue = "B"
                            ElseIf rodzaj.Contains("[C]") Then : ddlRodzajObj.SelectedValue = "C"
                            ElseIf rodzaj.Contains("[D]") Then : ddlRodzajObj.SelectedValue = "D"
                            ElseIf rodzaj.Contains("[RECZNIE]") Then : ddlRodzajObj.SelectedValue = "RECZNIE"
                            End If
                        End If
                    End If

                    Dim rodzaj_standard As String = row.Cells(11).Text.ToString
                    Dim ddlRodzajStandardObj As DropDownList = DirectCast(FindControl("DDLRodzajStandard" & Session("firma_id")), DropDownList)
                    If ddlRodzajStandardObj IsNot Nothing Then
                        If Session("firma_id").ToString = "INPOST_ALLEGRO" Then
                            If rodzaj_standard = "0" Then : ddlRodzajStandardObj.SelectedValue = "0"
                            ElseIf rodzaj_standard = "1" Then : ddlRodzajStandardObj.SelectedValue = "1"
                            End If
                        End If
                    End If

                    Dim wagaObj As TextBox = DirectCast(FindControl("TBWaga" & Session("firma_id")), TextBox)
                    If wagaObj IsNot Nothing Then
                        wagaObj.Text = row.Cells(6).Text.ToString
                    End If

                    Dim szerObj As TextBox = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox)
                    If szerObj IsNot Nothing Then
                        szerObj.Text = row.Cells(7).Text.ToString
                    End If

                    Dim wysObj As TextBox = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox)
                    If wysObj IsNot Nothing Then
                        wysObj.Text = row.Cells(8).Text.ToString
                    End If

                    Dim dluObj As TextBox = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox)
                    If dluObj IsNot Nothing Then
                        dluObj.Text = row.Cells(9).Text.ToString
                    End If

                    Dim ilePaczekObj As TextBox = DirectCast(FindControl("TBIlePaczek" & Session("firma_id")), TextBox)
                    If ilePaczekObj IsNot Nothing Then
                        ilePaczekObj.Text = row.Cells(10).Text.ToString
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub GridView_SS_Service_type_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridView_SS_Service_type.SelectedIndexChanged
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Dim cb As CheckBox = GridView_SS_Service_type.SelectedRow.FindControl("CBKodSelect")
        Dim rowid_paczki As Integer = GridView_SS_Service_type.SelectedRow.RowIndex
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
            TB_SS_SERVICE_VALUE.Text = ""
        Else
            cb.Checked = True
            GridView_SS_Service_type.SelectedIndex = rowid_paczki
            Dim row As GridViewRow = GridView_SS_Service_type.SelectedRow

            Dim ss_service_type As String = row.Cells(2).Text.ToString
            Dim ss_text_value As String = row.Cells(3).Text.ToString
            Dim ss_coll_on_form As String = row.Cells(4).Text.ToString

            Dim ddlServiceType As DropDownList = DirectCast(FindControl("DDL_SS_SERVICE_TYPE"), DropDownList)
            ddlServiceType.SelectedValue = ss_service_type

            Dim ddlServiceValue As TextBox = DirectCast(FindControl("TB_SS_SERVICE_VALUE"), TextBox)
            ddlServiceValue.Text = ss_text_value

            Dim ddlCollOnForm As DropDownList = DirectCast(FindControl("DDL_SS_COLL_ON_FORM"), DropDownList)
            ddlCollOnForm.SelectedValue = ss_coll_on_form

        End If
    End Sub

    Protected Sub GridView_SS_Service_type_GEIS_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridView_SS_Service_type_GEIS.SelectedIndexChanged
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Dim cb As CheckBox = GridView_SS_Service_type_GEIS.SelectedRow.FindControl("CBKodSelect")
        Dim rowid_paczki As Integer = GridView_SS_Service_type_GEIS.SelectedRow.RowIndex
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
            TB_SS_SERVICE_VALUE.Text = ""
        Else
            cb.Checked = True
            GridView_SS_Service_type.SelectedIndex = rowid_paczki
            Dim row As GridViewRow = GridView_SS_Service_type_GEIS.SelectedRow

            Dim ss_service_type As String = row.Cells(2).Text.ToString
            Dim ss_text_value As String = row.Cells(3).Text.ToString

            Dim ddlServiceType As DropDownList = DirectCast(FindControl("DDL_SS_SERVICE_TYPE_GEIS"), DropDownList)
            ddlServiceType.SelectedValue = ss_service_type

            Dim ddlServiceValue As TextBox = DirectCast(FindControl("TB_SS_SERVICE_VALUE_GEIS"), TextBox)
            ddlServiceValue.Text = ss_text_value

        End If
    End Sub

    Protected Sub GridViewZamowienia_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewZamowienia.SelectedIndexChanged
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Dim cb As CheckBox = GridViewZamowienia.SelectedRow.FindControl("CBKodSelect")
        Dim rowid_paczki As Integer = GridViewZamowienia.SelectedRow.RowIndex
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
            LZamowienieId.Text = ""
        Else
            cb.Checked = True
            GridViewZamowienia.SelectedIndex = rowid_paczki
            Dim row As GridViewRow = GridViewZamowienia.SelectedRow
            LZamowienieId.Text = row.Cells(2).Text.ToString
        End If
    End Sub

    Public Sub LadujDaneGridViewDyspozycjeInfo()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim lista_zamowien As String = ""
            For Each row As GridViewRow In GridViewZamowienia.Rows
                Dim nr_zam As String = row.Cells(2).Text.ToString
                lista_zamowien &= "'" & nr_zam & "',"
            Next

            If lista_zamowien.Length > 0 Then
                lista_zamowien = lista_zamowien.Substring(0, lista_zamowien.Length - 1)
            Else
                lista_zamowien = "'" & Session("kod_dyspo") & "'"
            End If

            sqlexp = "select rownum as lp,w.nr_zamow, w.schemat, w.nr_zamow_o, w.skrot, desql_japa_nwa.fsql_japa_rnaz(get_index_tow(w.skrot)) nazwa, " _
            & " desql_japa_nwa.fsql_japa_rkod(get_index_tow(w.skrot)) kod_tow, w.ile_poz, w.jm, w.mag, w.status, w.login from (" _
            & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
            & " zg.data_zam, 'M' typ_oper," _
            & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag, dm.login," _
            & " (select distinct zd.jm from ht_zod zd where zd.ie$0 like dm.nr_zamow||'%' and zd.is_deleted = 'N' and zd.skrot = dm.skrot) jm" _
            & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.nr_zamow in(" & lista_zamowien & ") and dm.schemat in('" & Session("schemat_dyspo") & "')" _
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

    ''##2023.01.26 / maskowanie adresow email dla sklepow DE
    Protected Sub EMAIL_ADD_Maskowanie(ByRef obj_field As TextBox, ByVal obj_cbszyfrowanie As CheckBox)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim email_package As String = obj_field.Text.ToString
            If email_package <> "" And obj_cbszyfrowanie.Checked = False Then
                Dim nr_zamow As String = LNrZamow.Text.ToString
                If nr_zamow <> "" Then
                    Dim increment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("Select  increment_id from dp_rest_mag_order where nr_zamow_dt='" & nr_zamow & "'", conn)
                    If increment_id.Contains("DE") And email_package.Contains("@") Then
                        Dim b() As String = email_package.Split("@")
                        Dim email_1 As String = b(0).ToString
                        Dim email_2 As String = b(1).ToString
                        Dim random_id As New Random
                        Dim special_mask As String = "*"

                        For i = 0 To Math.Ceiling(email_1.Length / 3)
                            Dim r_id As Integer = random_id.Next(0, email_1.Length - 1)
                            Dim e_c As Char = email_1(r_id).ToString
                            email_1 = email_1.Replace(e_c, special_mask)
                        Next

                        For i = 0 To Math.Ceiling(email_2.Length / 3)
                            Dim r_id As Integer = random_id.Next(0, email_2.Length - 1)
                            Dim e_c As Char = email_2(r_id).ToString
                            If e_c <> "." Then
                                email_2 = email_2.Replace(e_c, special_mask)
                            End If
                        Next

                        obj_field.Text = email_1.ToString & "@" & email_2.ToString

                        ''Return b(0).ToString & "@allegromail.pl"
                    End If
                End If
            ElseIf email_package <> "" And obj_cbszyfrowanie.Checked = True Then
                obj_field.Text = TB_PC_EMAIL_ADD_TEST.Text
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub TB_PC_EMAIL_ADD_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TB_PC_EMAIL_ADD.TextChanged
        EMAIL_ADD_Maskowanie(TB_PC_EMAIL_ADD, CBEmailSzyfrowanie)
    End Sub

    Protected Sub BPobierzNrDigit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzNrDigit.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If LSchemat.Text = "DOMINUS" And LNr_zamow_o.Text <> "" Then
                sqlexp = "select nr_zamow from ht_zog where nr_zamow_o = '" & LNr_zamow_o.Text & "'"
                sqlexp = "select zzg.fd_nr_z_de from de_dyzzg zzg, de_dyzag zag where zag.fd_nr_zamo like '" & LNr_zamow_o.Text & "' and +zag.fd_nr_zewn = zzg.fd_nr_zewn"
                LNrZamowDigit.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            End If
            conn.Close()
        End Using

    End Sub

    Protected Sub BWyswietlArtykuly_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWyswietlArtykuly.Click
        If GridViewDyspozycje.Visible = False Then
            GridViewDyspozycje.Visible = True
        Else
            GridViewDyspozycje.Visible = False
        End If
    End Sub

    Protected Sub BDrukowanieEtykieta_UPS_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BDrukowanieEtykieta_UPS_DE_API.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim obj_grid_view As New GridView
            If Session("firma_id") IsNot Nothing Then
                If Session("firma_id") = "UPS_DE_API" Then
                    obj_grid_view = GridViewPrzesylki_UPS_DE_API
                ElseIf Session("firma_id") = "DHL_DE_API" Then
                    obj_grid_view = GridViewPrzesylki_DHL_DE_API
                ElseIf Session("firma_id") = "INPOST" Then
                    obj_grid_view = GridViewPrzesylki_INPOST
                ElseIf Session("firma_id") = "GEIS" Then
                    obj_grid_view = GridViewPrzesylki_GEIS
                End If
            Else
                obj_grid_view = GridViewPrzesylki
            End If

            For Each row As GridViewRow In obj_grid_view.Rows
                Dim tracking_number As String = row.Cells(10).Text.ToString
                If tracking_number <> "" Then

                    WebClientPrint.LicenseOwner = "Dajar Sp. z o.o. - 1 WebApp Lic - 1 WebServer Lic"
                    WebClientPrint.LicenseKey = "273D8F1AB12B42847C2E9FE7717B6756A2CA0026"

                    Application.Lock()
                    Application("session_id_" + Session.SessionID) = Session.SessionID.ToString
                    Application("shipment_id_" + Session.SessionID) = Session("shipment_id").ToString
                    Application("tracking_number_" + Session.SessionID) = tracking_number
                    Application("kod_dyspo_" + Session.SessionID) = Session("kod_dyspo").ToString
                    Application("firma_id_" + Session.SessionID) = Session("firma_id").ToString
                    Application("mylogin_" + Session.SessionID) = Session("mylogin").ToString
                    Application("myhash_" + Session.SessionID) = Session("myhash").ToString
                    Application.UnLock()

                    dajarSWMagazyn_MIA.MyFunction.SetPrintDyspozycja(dajarSWMagazyn_MIA.MyFunction.GetRemoteIp, Session.SessionID.ToString, Session("kod_dyspo").ToString, Session("mylogin").ToString, Session("myhash").ToString, tracking_number)
                    ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "script", "javascript:jsWebClientPrint.print('useDefaultPrinter=' + $('#useDefaultPrinter').attr('checked') + '&printerName=' + $('#installedPrinterName').val() + '&sessionId=" + Session.SessionID + "');", True)
                End If
            Next
            conn.Close()

        End Using
    End Sub

    Protected Sub BDrukowanieEtykieta_DHL_DE_API_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BDrukowanieEtykieta_DHL_DE_API.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim obj_grid_view As New GridView
            If Session("firma_id") IsNot Nothing Then
                If Session("firma_id") = "UPS_DE_API" Then
                    obj_grid_view = GridViewPrzesylki_UPS_DE_API
                ElseIf Session("firma_id") = "DHL_DE_API" Then
                    obj_grid_view = GridViewPrzesylki_DHL_DE_API
                ElseIf Session("firma_id") = "INPOST" Then
                    obj_grid_view = GridViewPrzesylki_INPOST
                ElseIf Session("firma_id") = "GEIS" Then
                    obj_grid_view = GridViewPrzesylki_GEIS
                End If
            Else
                obj_grid_view = GridViewPrzesylki
            End If

            For Each row As GridViewRow In obj_grid_view.Rows
                Dim tracking_number As String = row.Cells(10).Text.ToString
                If tracking_number <> "" Then

                    WebClientPrint.LicenseOwner = "Dajar Sp. z o.o. - 1 WebApp Lic - 1 WebServer Lic"
                    WebClientPrint.LicenseKey = "273D8F1AB12B42847C2E9FE7717B6756A2CA0026"

                    Application.Lock()
                    Application("session_id_" + Session.SessionID) = Session.SessionID.ToString
                    Application("shipment_id_" + Session.SessionID) = Session("shipment_id").ToString
                    Application("tracking_number_" + Session.SessionID) = tracking_number
                    Application("kod_dyspo_" + Session.SessionID) = Session("kod_dyspo").ToString
                    Application("firma_id_" + Session.SessionID) = Session("firma_id").ToString
                    Application("mylogin_" + Session.SessionID) = Session("mylogin").ToString
                    Application("myhash_" + Session.SessionID) = Session("myhash").ToString
                    Application.UnLock()

                    dajarSWMagazyn_MIA.MyFunction.SetPrintDyspozycja(dajarSWMagazyn_MIA.MyFunction.GetRemoteIp, Session.SessionID.ToString, Session("kod_dyspo").ToString, Session("mylogin").ToString, Session("myhash").ToString, tracking_number)
                    ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "script", "javascript:jsWebClientPrint.print('useDefaultPrinter=' + $('#useDefaultPrinter').attr('checked') + '&printerName=' + $('#installedPrinterName').val() + '&sessionId=" + Session.SessionID + "');", True)
                End If
            Next
            conn.Close()

        End Using
    End Sub

End Class