Imports dajarSWMagazyn_MIA.HashMd5
Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.Drawing
Imports iText.Kernel.Colors
Imports Neodynamic.SDK.Web

Partial Public Class package
    Inherits System.Web.UI.Page
    Dim result As Boolean = False
    Dim sqlexp As String = ""
    Dim sqlexpTrans As New List(Of String)
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder
    Public _serialport As COM_SerialPort
    Private Shared returnStr As String = ""
    Dim obj_paczka As OBJ_SWM_PACZKA
    Dim obj_paczka_base As OBJ_SWM_PACZKA_BASE
    Public Shared inpost_shipment_id As String = ""
    Public Shared session_kod_dyspo As String = ""
    Public Shared session_mylogin As String = ""
    Public Shared session_myhash As String = ""
    Public Shared session_shipment_id As String = ""

    Class OBJ_SWM_PACZKA
        Public INPOST_TYPE As String = ""
        Public B_SHIP_PAYMENT_TYPE As String = ""
        Public B_BILL_ACC_NUM As String = ""
        Public B_PAYMENT_TYPE As String = ""
        Public B_COSTS_CENTER As String = ""
        Public ST_SHIPMENT_DATE As String = ""
        Public ST_SHIPMENT_START As String = ""
        Public ST_SHIPMENT_END As String = ""
        Public SR_COUNTRY As String = ""
        Public SR_NAME As String = ""
        Public SR_POSTAL_CODE As String = ""
        Public SR_CITY As String = ""
        Public SR_STREET As String = ""
        Public SR_HOUSE_NUM As String = ""
        Public SR_APART_NUM As String = ""
        Public PC_PERSON_NAME As String = ""
        Public PC_PHONE_NUM As String = ""
        Public PC_EMAIL_ADD As String = ""
    End Class

    Class OBJ_SWM_PACZKA_BASE
        Public DROP_OFF As String = ""
        Public SERVICE_TYPE As String = ""
        Public LABEL_TYPE As String = ""
        Public CONTENT As String = ""
        Public COMMENT_F As String = ""
        Public REFERENCE As String = ""
    End Class

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
        Public mag_cel As String = 700
        Public kod_kontr As String = ""

        Public Sub New(ByVal _nr_zamow As String, ByVal _schemat As String, ByVal _kod_kontr As String, ByVal _ile_poz As String, ByVal _etykieta As String)
            nr_zamow = _nr_zamow
            schemat = _schemat
            etykieta = _etykieta
            ile_poz = _ile_poz
            ile_obcy = _ile_poz
            kod_kontr = _kod_kontr
        End Sub
    End Class

    Class ObiektZaznaczeniePaczka
        Public nr_zamow As String = ""
        Public schemat As String = ""
        Public paczka As String = ""

        Public Sub New(ByVal _nr_zamow As String, ByVal _schemat As String, ByVal _paczka As String)
            nr_zamow = _nr_zamow
            schemat = _schemat
            paczka = _paczka
        End Sub
    End Class
    
    Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
        For i As Integer = 0 To GridViewPakowanie.Rows.Count - 1
            ClientScript.RegisterForEventValidation(GridViewPakowanie.UniqueID, "Select$" & i)
        Next
        
        MyBase.Render(writer)
    End Sub

    Protected Sub GridViewPakowanie_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewPakowanie.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewPakowanie.PageIndex = e.NewPageIndex
        GridViewPakowanie.DataBind()
        LadujDaneGridViewPakowanie()
    End Sub

    Protected Sub RefreshDDLFirma(ByRef ddlObiekt As DropDownList)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim sqlExp As String = "select firma_id,firma_nazwa from dp_swm_mia_firma where blokada <> 'X' or blokada is null order by firma_nazwa asc"
            ddlObiekt.Items.Clear()
            ddlObiekt.Items.Add("")
            Dim cmd As New OracleCommand(sqlExp, conn)
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

            ddlObiekt.Items.Add(New ListItem("WYBIERZ PRZEWOZNIKA", "-1", True))
            ddlObiekt.SelectedValue = "-1"
            conn.Close()
        End Using
    End Sub

    Protected Sub ClearGVPaczki()
        GridViewPaczki.SelectedIndex = -1
        For Each row As GridViewRow In GridViewPaczki.Rows
            Dim cbT As CheckBox = row.FindControl("CBKodSelect")
            cbT.Checked = False
        Next
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

    Protected Sub BRefreshPage_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BRefreshPage.Click
        Response.Redirect(Request.RawUrl)
    End Sub

    Protected Sub page_loaded() Handles Me.LoadComplete
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
            ''BDodajPaczke.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zapisac paczke " & LPaczkaID.Text.ToString & "\n') == false) return false")
            BAnulujPaczek.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować paczke " & LPaczkaID.Text.ToString & "\n') == false) return false")
            BZerowanie_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno wyzerowac ustawienia paczek INPOST " & LNr_list_przewozowy.Text.ToString & "\n') == false) return false")

            Session("contentKomunikat") = Session(session_id)

            If Session("mm_sel_index") IsNot Nothing And GridViewPakowanie.Rows.Count > 0 Then
                Dim lista_czytnik As New List(Of ZamowieniaInformacje)
                lista_czytnik = Session("mm_sel_index")

                For Each l In lista_czytnik
                    For Each row As GridViewRow In GridViewPakowanie.Rows
                        Dim r_nr_zamow As String = row.Cells(2).Text.ToString
                        Dim r_schemat As String = row.Cells(3).Text.ToString

                        If l.nr_zamow = r_nr_zamow Then
                            GridViewPakowanie.SelectedIndex = row.RowIndex
                            Dim cb As CheckBox = GridViewPakowanie.Rows(row.RowIndex).FindControl("CBKodSelect")
                            cb.Checked = True
                            DDLRodzajStandard.SelectedValue = "0"
                            PanelDodawaniePaczki.Visible = True
                            LNrZamowienia.Text = l.nr_zamow
                            LSchemat.Text = r_schemat
                            ''LSTAT_U2.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select stat_u2 from dp_swm_mia_zog where nr_zamow='" & l.nr_zamow & "' and schemat='" & l.schemat & "'", conn)

                            ''##2023.01.24 / dodanie sesji i aktualizacja SCHEMATU w oparciu o zaznaczone dane
                            Session("kod_dyspo") = l.nr_zamow
                            Session("schemat_dyspo") = r_schemat
                            LPaczkaID.Text = Session("paczka_dyspo")

                            If Session("shipment_id") Is Nothing Then
                                ''Dim shipment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' AND SHIPMENT_ID LIKE 'SH%'", conn)
                                Dim shipment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT max(SHIPMENT_ID) FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                                If shipment_id <> "" Then
                                    Session("shipment_id") = shipment_id
                                    LNr_list_przewozowy.Text = Session("shipment_id")
                                    HLEtykietaPDF_INPOST.NavigateUrl = "http://10.1.0.64:8084/upload/inpost/inpost_" & Session("shipment_id") & ".pdf"
                                    HLEtykietaPDF_INPOST.Text = Session("shipment_id")
                                    inpost_shipment_id = HLEtykietaPDF_INPOST.Text.ToString
                                Else
                                    LNr_list_przewozowy.Text = ""
                                End If
                            Else
                                ''##2023.01.24 / aktualizacja numeru listu przewozowego dla skanera
                                LNr_list_przewozowy.Text = Session("shipment_id")
                            End If

                            ''##2023.01.24 / wylaczenie zerowania paczka_id dla wczesniej utworzonych paczek
                            ''GenerujInformacjeEtykieta(l.nr_zamow)
                        End If
                    Next
                Next

                WyswietlSzczegolyZamowienia()
                LadujDaneGridViewPaczka()
            End If
            conn.Close()
        End Using
    End Sub

    Public Sub WypelnijPolaPaczka(ByVal obj_nr_zamow As String, ByVal firma_id As String)
        ''Dim rx As Regex
        ''Dim match As Match

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            obj_paczka_base = New OBJ_SWM_PACZKA_BASE
            obj_paczka = New OBJ_SWM_PACZKA

            If firma_id = "DHL" Then
                obj_paczka_base.DROP_OFF = "REGULAR_PICKUP"
                obj_paczka_base.SERVICE_TYPE = "AH"
                obj_paczka_base.LABEL_TYPE = "BLP"
                obj_paczka_base.CONTENT = DirectCast(FindControl("DDL_CONTENT"), DropDownList).SelectedValue.ToString
                obj_paczka_base.COMMENT_F = TB_COMMENT_F_INPOST.Text.ToString

                obj_paczka.B_BILL_ACC_NUM = "1276909"
                obj_paczka.B_PAYMENT_TYPE = "BANK_TRANSFER"
                obj_paczka.B_SHIP_PAYMENT_TYPE = "SHIPPER"
                obj_paczka.ST_SHIPMENT_DATE = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("D2") & "-" & DateTime.Now.Day.ToString("D2")
                obj_paczka.ST_SHIPMENT_START = "12:00"
                obj_paczka.ST_SHIPMENT_END = "15:00"
                obj_paczka.SR_COUNTRY = "XX"
            ElseIf firma_id = "INPOST_ALLEGRO" Then
                obj_paczka_base.DROP_OFF = "REGULAR_PICKUP"
                obj_paczka_base.SERVICE_TYPE = TB_SERVICE_TYPE_INPOST.Text.ToString
                obj_paczka_base.LABEL_TYPE = "pdf"
                obj_paczka_base.CONTENT = DirectCast(FindControl("DDL_CONTENT"), DropDownList).SelectedValue.ToString
                obj_paczka_base.COMMENT_F = TB_COMMENT_F_INPOST.Text.ToString

                obj_paczka.B_BILL_ACC_NUM = "1276909"
                obj_paczka.B_PAYMENT_TYPE = "BANK_TRANSFER"
                obj_paczka.B_SHIP_PAYMENT_TYPE = "SHIPPER"
                obj_paczka.B_COSTS_CENTER = "ALLEGRO"
                obj_paczka.ST_SHIPMENT_DATE = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("D2") & "-" & DateTime.Now.Day.ToString("D2")
                obj_paczka.ST_SHIPMENT_START = "12:00"
                obj_paczka.ST_SHIPMENT_END = "15:00"
                obj_paczka.SR_COUNTRY = "PL"

            End If

            sqlexp = "select (co_zostawi||co_zostaw2) opis from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & obj_nr_zamow & "')"
            Dim opis_zamowienia As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            If opis_zamowienia.Contains("#") Then
                Dim opis_tmp() As String = opis_zamowienia.Split("#")
                If opis_tmp.Length > 7 Then
                    obj_paczka.SR_NAME = opis_tmp(1).ToString.Trim
                    obj_paczka.SR_POSTAL_CODE = opis_tmp(4).Trim
                    obj_paczka.SR_CITY = opis_tmp(3).Trim
                    obj_paczka.SR_STREET = opis_tmp(2).Trim

                    obj_paczka.PC_PERSON_NAME = obj_paczka.SR_NAME.ToString.Trim
                    obj_paczka.PC_EMAIL_ADD = opis_tmp(6).Trim
                    obj_paczka.PC_PHONE_NUM = opis_tmp(7).Trim
                End If
            End If

            If obj_paczka.SR_NAME = "" Then
                ''''MsgBox("Wariant II. pobieranie informacji o kotrahencie z rejestru zamowienie")
                sqlexp = "select kod_kontr from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & obj_nr_zamow & "')"
                Dim kod_kontr As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                sqlexp = "select kod_odb from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & obj_nr_zamow & "')"
                Dim kod_odb As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                sqlexp = "select kod_mie from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & obj_nr_zamow & "')"
                Dim kod_mie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                sqlexp = "select kod_kraju,(naz||naz_1),adr,kod_p,miejsc from ht_kontm where kod_kontr = " & kod_kontr & " and kod_odb = '" & kod_odb & "' and kod_mie='" & kod_mie & "' and is_deleted='N'"
                Dim cmd As OracleCommand = New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        obj_paczka.SR_COUNTRY = dr.Item(0).ToString
                        obj_paczka.SR_NAME = dr.Item(1).ToString
                        obj_paczka.PC_PERSON_NAME = dr.Item(1).ToString
                        obj_paczka.SR_STREET = dr.Item(2).ToString
                        obj_paczka.SR_POSTAL_CODE = dr.Item(3).ToString.Replace("-", "")
                        obj_paczka.SR_CITY = dr.Item(4).ToString
                    End While

                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()

                sqlexp = "select e_mail,telefon from ht_konto where kod_kontr = " & kod_kontr & ""
                cmd = New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                dr = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        obj_paczka.PC_EMAIL_ADD = dr.Item(0).ToString.Trim
                        obj_paczka.PC_PHONE_NUM = dr.Item(1).ToString.Trim.Replace(" ", "").Replace("-", "")
                    End While

                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()
            End If

            If obj_paczka.PC_PHONE_NUM <> "" Then
                obj_paczka.PC_PHONE_NUM = obj_paczka.PC_PHONE_NUM.Replace("+48", "").Replace("+ 48", "").Replace(" ", "").ToString
                ''##2023.02.06 / walidacja numeru telefonu usuwanie 0 z poczatku
                If obj_paczka.PC_PHONE_NUM.StartsWith("0") Then
                    obj_paczka.PC_PHONE_NUM = obj_paczka.PC_PHONE_NUM.Substring(1, obj_paczka.PC_PHONE_NUM.Length - 1)
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Public Sub GenerujInformacjeEtykieta_DAJAR(ByVal obj_nr_zamow As String, ByVal obj_schemat As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim shippment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & obj_nr_zamow & "' AND SCHEMAT='" & obj_schemat & "' AND SHIPMENT_ID LIKE 'SH%'", conn)
            If shippment_id <> "" Then
                ''Session("shipment_id") = shipment_id
                remoteApp.LoadProcessDetails()
                If remoteApp.domainUserName <> "" Then
                    Dim clientIpAdress As String = Request.ServerVariables("REMOTE_ADDR").ToString()
                    clientIpAdress = remoteApp.domainIpAdress
                    clientIpAdress = Request.UserHostAddress.ToString

                    ''Dim clientName As String = Request.UserHostName.ToString

                    sqlexp = "select paczka_id from dp_swm_mia_paczka_info where shipment_id='" & shippment_id & "'"
                    Dim cmd As OracleCommand = New OracleCommand(sqlexp, conn)
                    cmd.CommandType = CommandType.Text
                    Dim dr As OracleDataReader = cmd.ExecuteReader()
                    Try
                        While dr.Read()
                            Dim paczka_id As String = dr.Item(0).ToString
                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "insert into dp_swm_mia_etykieta (shipment_id,paczka_id,nr_zamow,schemat,domain_user,domain_ip,login,hash,label_dajar,autodata) " _
                            & " values('" & shippment_id & "','" & paczka_id & "','" & obj_nr_zamow & "','" & obj_schemat & "','" & remoteApp.domainUserName.ToString & "','" & clientIpAdress & "','" & Session("mylogin") & "','" & Session("myhash") & "','N',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End While

                    Catch ex As System.ArgumentOutOfRangeException
                        Console.WriteLine(ex)
                    End Try
                    dr.Close()
                    cmd.Dispose()

                    WypelnijPolaPaczka(obj_nr_zamow, Session("firma_id"))

                    sqlexp = "update dp_swm_mia_paczka set B_BILL_ACC_NUM='" & obj_paczka.B_BILL_ACC_NUM & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set B_SHIP_PAYMENT_TYPE='" & obj_paczka.B_SHIP_PAYMENT_TYPE & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set B_PAYMENT_TYPE='" & obj_paczka.B_PAYMENT_TYPE & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set ST_SHIPMENT_DATE='" & obj_paczka.ST_SHIPMENT_DATE & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set ST_SHIPMENT_START='" & obj_paczka.ST_SHIPMENT_START & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set ST_SHIPMENT_END='" & obj_paczka.ST_SHIPMENT_END & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set SR_NAME='" & obj_paczka.SR_NAME & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set SR_POSTAL_CODE='" & obj_paczka.SR_POSTAL_CODE & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set SR_CITY='" & obj_paczka.SR_CITY & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set SR_STREET='" & obj_paczka.SR_STREET & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set SR_HOUSE_NUM='" & obj_paczka.SR_HOUSE_NUM & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set SR_APART_NUM='" & obj_paczka.SR_APART_NUM & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set PC_PERSON_NAME='" & obj_paczka.PC_PERSON_NAME & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set PC_PHONE_NUM='" & obj_paczka.PC_PHONE_NUM & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka set PC_EMAIL_ADD='" & obj_paczka.PC_EMAIL_ADD & "' where shipment_id='" & shippment_id & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    Dim czyIstniejeBase As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT COUNT(*) FROM dp_swm_mia_paczka_base WHERE SHIPMENT_ID='" & shippment_id & "' AND SCHEMAT='" & obj_schemat & "'", conn)
                    If czyIstniejeBase = "0" Then
                        sqlexp = "insert into dp_swm_mia_paczka_base(SCHEMAT,SHIPMENT_ID) values ('" & obj_schemat & "','" & shippment_id & "')"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If

                    sqlexp = "update dp_swm_mia_paczka_base set DROP_OFF='" & obj_paczka_base.DROP_OFF & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & obj_schemat & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka_base set SERVICE_TYPE='" & obj_paczka_base.SERVICE_TYPE & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & obj_schemat & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka_base set LABEL_TYPE='" & obj_paczka_base.LABEL_TYPE & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & obj_schemat & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka_base set CONTENT='" & obj_paczka_base.CONTENT & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & obj_schemat & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_paczka_base set COMMENT_F='" & obj_paczka_base.COMMENT_F & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & obj_schemat & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                End If

            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Dim przerwijLadowanie As Boolean = False

        ''BDodajPaczke.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zapisac paczke " & LPaczkaID.Text.ToString & "\n') == false) return false")
        ''BZakonczPakowanie.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zamknąć wybrane zamowienia\n') == false) return false")
        BAnulujPaczek.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno anulować paczke " & LPaczkaID.Text.ToString & "\n') == false) return false")
        BZerowanie_INPOST.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno wyzerowac ustawienia paczek INPOST " & LNr_list_przewozowy.Text.ToString & "\n') == false) return false")

        If Session("mylogin") = Nothing And Session("myhash") = Nothing Then
            Session.Abandon()
            Response.Redirect("index.aspx")
        ElseIf Session("mytyp_oper") <> "P" And Session("mytyp_oper") <> "PP" Then
            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
            Session(session_id) += "<br />Zaloguj sie na opowiedniego operatora - typ PAKOWANIE<br />"
            Session(session_id) += "</div>"
            przerwijLadowanie = True
            PanelPakowanie.Visible = False
        End If

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session("contentMagazyn") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN = '" & Session("mylogin") & "'", conn)
            conn.Close()
        End Using

        ''Session.Remove("serialport_getdata")
        If Not Page.IsPostBack Then
            dajarSWMagazyn_MIA.MyFunction.RefreshDDLOperator(DDLOperator, Session)
            RefreshDDLFirma(DDLFirma, "")
            If przerwijLadowanie = False Then
                LadujDaneGridViewPakowanie()
            End If
            Dim receivedData As String = Request.QueryString("data")
            If Not String.IsNullOrEmpty(receivedData) Then
                For Each row As GridViewRow In GridViewPakowanie.Rows
                    If row.Cells(2).Text = receivedData Then
                        Dim cb As CheckBox = row.FindControl("CBKodSelect")
                        cb.Checked = True

                        GridViewPakowanie.SelectedIndex = row.RowIndex
                        GridViewPakowanie_SelectedIndexChanged(GridViewPakowanie, EventArgs.Empty)
                    End If
                Next
            End If
        End If

        Page.SetFocus(TBEtykieta)
        Session("contentKomunikat") = Session(session_id)
    End Sub

    Protected Sub BPobierzWage_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzWage.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            remoteApp.LoadProcessDetails()
            If remoteApp.domainUserName <> "" Then
                Dim clientIpAdress As String = Request.ServerVariables("REMOTE_ADDR").ToString()
                sqlexp = "select waga from dp_swm_mia_waga where domain_ip='" & clientIpAdress & "' and nr_zamow is null"
                Dim waga As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If waga.Contains("B0") Then
                    waga = waga.Replace("B0", "").Replace(".", ",").Trim
                    TBWagaCOM.Text = waga
                    If IsNumeric(waga) Then
                        TBWaga.Text = Math.Ceiling(Double.Parse(waga))
                    End If
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Private Sub MesgBox(ByVal sMessage As String)
        Dim msg As String
        msg = "<script language='javascript'>"
        msg += "alert('" & sMessage & "');"
        msg += "<" & "/script>"
        Response.Write(msg)
    End Sub

    Protected Sub RefreshDDLFirma(ByRef ddlObiekt As DropDownList, ByVal filtr As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If filtr <> "" Then
                sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma where firma_id='" & filtr & "' order by firma_nazwa asc"
            Else
                sqlexp = "select firma_id,firma_nazwa from dp_swm_mia_firma order by firma_nazwa asc"
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

            If filtr = "" Then
                ddlObiekt.Items.Add(New ListItem("WYBIERZ PRZEWOZNIKA", "-1", True))
                ddlObiekt.SelectedValue = "-1"
            End If
            conn.Close()
        End Using
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
            tb.Text = "1"
            tb = FindControl("TBSzer" & Session("firma_id"))
            tb.Text = "1"
            tb = FindControl("TBDlug" & Session("firma_id"))
            tb.Text = "1"
            ''tb = FindControl("TBWaga" & Session("firma_id"))
            ''tb.Text = "1"
        End If

        ''If obj_paczka.INPOST_TYPE = "paczkomaty" Then
        ''    TBWaga.Text = "1"
        ''End If

    End Sub

    Public Sub WyswietlSzczegolyZamowienia()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()


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

                ''##2022.10.26 // pakowanie paczkomaty
                Dim nr_zamow As String = LNrZamowienia.Text.ToString
                TBOpisZam.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select (co_zostawi||co_zostaw2) opis from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & nr_zamow & "')", conn)

                Dim paczka_smart As String = True
                Dim increment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("Select  increment_id from dp_rest_mag_order where nr_zamow_dt='" & nr_zamow & "'", conn)
                Dim shipping_desc As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("Select shipping_desc from dp_rest_mag_order where nr_zamow_dt='" & nr_zamow & "'", conn)
                Dim shipping_amount As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("Select shipping_amount from dp_rest_mag_order where nr_zamow_dt='" & nr_zamow & "'", conn)

                Dim shipping_address As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("Select shipping_address from dp_rest_mag_order where nr_zamow_dt='" & nr_zamow & "'", conn)

                ''##2022.12.01 / walidacja pozostalych formularzu INPOST
                ''##2023.07.13 / dodanie obslugi paczkomaty empik
                If shipping_desc.Contains("InPost") Or shipping_address.Contains("Paczkomat") Then
                    Session("firma_id") = "INPOST_ALLEGRO"
                    WymiarySzczegoloweINPOST.Visible = True
                    TBOpisZam.Visible = True

                    ''##2023.04.04 / walidacja pola opisowego pod katem pobrania / cod
                    If TBOpisZam.Text.ToUpper.Contains("POBRANIE") = True Or TBOpisZam.Text.ToUpper.Contains("CASHONDELIVERY") = True Then
                        UslugaCodINPOST.Visible = True
                        Dim ss_service_type As String = "cod"
                        Dim ss_service_value As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select wartosc from ht_zog where ie$14 = desql_graf.df11_2('" & nr_zamow & "')", conn)
                        If ss_service_type = "cod" Or ss_service_type = "insurance" Then
                            ss_service_value = ss_service_value.ToString.Replace(",", ".")
                            TB_COD_INPOST.Text = ss_service_value
                        End If
                    Else
                        UslugaCodINPOST.Visible = False
                        TB_COD_INPOST.Text = ""
                    End If

                    If Session("shipment_date") IsNot Nothing Then
                        TB_ST_SHIPMENT_DATE.Text = Session("shipment_date")
                    Else
                        TB_ST_SHIPMENT_DATE.Text = DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("D2") & "-" & DateTime.Now.Day.ToString("D2")
                    End If

                    TB_SR_COUNTRY.Text = "PL"

                    ''##2023.03.13 / wprowdzenie MPK dla DAJARU
                    If increment_id.StartsWith("SPL") Then
                        TB_B_COSTS_CENTER.Text = "DAJAR"
                    Else
                        TB_B_COSTS_CENTER.Text = "ALLEGRO"
                    End If

                    TB_LABEL_TYPE_INPOST.Text = "pdf"
                    ''ustawienie domyslego typu dla INPOST=paczka
                    DDLTyp.SelectedValue = "1"
                    DDLTyp.Enabled = False
                    DDL_CONTENT.Enabled = False
                    DDLRodzajStandard.Enabled = False
                    TBIlePaczek.Enabled = False

                    Dim tbrow As String() = {"TBROW_SR_NAME", "TBROW_SR_COMPANY_INPOST", "TBROW_SR_FIRSTNAME_INPOST", "TBROW_SR_LASTNAME_INPOST", "TBROW_SR_POSTAL_CODE", "TBROW_SR_CITY", "TBROW_SR_STREET", "TBROW_SR_HOUSE_NUM", "TBROW_SR_APART_NUM", "TBROW_PC_PERSON_NAME", "TBROW_PC_PHONE_NUM", "TBROW_PC_EMAIL_ADD", "TBROW_SR_COUNTRY", "TBROW_TB_ST_SHIPMENT_DATE", "TBROW_TB_B_COSTS_CENTER", "TBROW_LABEL_TYPE_INPOST"}
                    ''Dim tbrow As String() = {"TBROW_SR_POST_NUM", "TBROW_SERVICE_TYPE_INPOST", "TBROW_SR_NAME", "TBROW_SR_COMPANY_INPOST", "TBROW_SR_FIRSTNAME_INPOST", "TBROW_SR_LASTNAME_INPOST", "TBROW_SR_POSTAL_CODE", "TBROW_SR_CITY", "TBROW_SR_STREET", "TBROW_SR_HOUSE_NUM", "TBROW_SR_APART_NUM", "TBROW_PC_PERSON_NAME", "TBROW_PC_PHONE_NUM", "TBROW_PC_EMAIL_ADD", "TBROW_SR_COUNTRY", "TBROW_TB_ST_SHIPMENT_DATE", "TBROW_TB_B_COSTS_CENTER", "TBROW_LABEL_TYPE_INPOST"}
                    For Each t In tbrow
                        Dim tr As Panel = FindControl(t)
                        tr.Visible = False
                    Next

                    tbrow = {"TBROW_DDLFirma"}
                    For Each t In tbrow
                        Dim tr As Panel = FindControl(t)
                        tr.Visible = True
                    Next

                    Dim tbcell As String() = {"TBCELL_RodzajINPOST_ALLEGRO"}
                    For Each t In tbcell
                        Dim tc As Panel = FindControl(t)
                        tc.Visible = True
                    Next

                    Dim external_username As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select external_username from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)

                    If increment_id.StartsWith("SPL") Then
                        TB_COMMENT_F_INPOST.Text = nr_zamow & " DAJAR"
                    ElseIf Session("schemat_dyspo") = "DOMINUS" Then
                        TB_COMMENT_F_INPOST.Text = "DO " & nr_zamow & " " & external_username
                    Else
                        TB_COMMENT_F_INPOST.Text = nr_zamow & " " & external_username
                    End If

                    WypelnijPolaPaczka(nr_zamow, Session("firma_id"))

                    If obj_paczka IsNot Nothing Then
                        TB_SR_NAME.Text = obj_paczka.SR_NAME.ToString
                        TB_SR_POSTAL_CODE.Text = obj_paczka.SR_POSTAL_CODE.ToString
                        TB_SR_CITY.Text = obj_paczka.SR_CITY.ToString
                        TB_SR_STREET.Text = obj_paczka.SR_STREET.ToString
                        TB_SR_HOUSE_NUM.Text = obj_paczka.SR_HOUSE_NUM.ToString
                        TB_SR_APART_NUM.Text = obj_paczka.SR_APART_NUM.ToString
                    End If

                    If obj_paczka.SR_HOUSE_NUM = "" Then
                        Dim wzorzecReguly As String = "\d{1,}?[a-z|A-Z|M|m\.\s|\d|/]*"
                        wzorzecReguly = "(\s)+\d{1,}?[a-z|A-Z|M|m\.\s|\d|\/]*"
                        ''##2023.02.02 / aktualizacja reguly dotyczaca adresu 
                        Dim rx As Regex = New Regex(wzorzecReguly)
                        ''Dim wzorzecString As String = opis_tmp(2).Substring(opis_tmp(2).Length / 2, opis_tmp(2).Length - opis_tmp(2).Length / 2)
                        Dim wzorzecString As String = obj_paczka.SR_STREET.ToString

                        Dim m As Match = rx.Match(wzorzecString)
                        If m.Success = True Then
                            Dim wzorzecIndeks As Integer = obj_paczka.SR_STREET.IndexOf(m.Value.ToString)
                            If wzorzecIndeks > 0 Then
                                TB_SR_STREET.Text = obj_paczka.SR_STREET.Substring(0, wzorzecIndeks).ToString.Trim
                                TB_SR_HOUSE_NUM.Text = obj_paczka.SR_STREET.Substring(wzorzecIndeks, obj_paczka.SR_STREET.Length - wzorzecIndeks).ToString.Trim
                            End If
                        End If
                    End If

                    TB_PC_PERSON_NAME.Text = obj_paczka.PC_PERSON_NAME.ToString
                    TB_PC_EMAIL_ADD.Text = obj_paczka.PC_EMAIL_ADD.ToString
                    TB_PC_PHONE_NUM.Text = obj_paczka.PC_PHONE_NUM.ToString


                    Dim firstname As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_firstname from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)
                    Dim lastname As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_lastname from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)

                    If TB_SR_FIRSTNAME_INPOST.Text.ToString.Length = 0 Then
                        TB_SR_FIRSTNAME_INPOST.Text = firstname.ToUpper
                    End If

                    If TB_SR_LASTNAME_INPOST.Text.ToString.Length = 0 Then
                        TB_SR_LASTNAME_INPOST.Text = lastname.ToUpper
                    End If

                    ''####2022.10.14 / walidacja pola TB_SR_HOUSE_NUM oraz TB_SR_APART_NUM
                    If TB_SR_APART_NUM.Text <> "" And TB_SR_APART_NUM.Text <> "&bnsp;" Then
                        TB_SR_HOUSE_NUM.Text = TB_SR_HOUSE_NUM.Text.ToString & " " & TB_SR_APART_NUM.Text.ToString
                        TB_SR_APART_NUM.Text = ""
                    End If

                    If TB_SR_POSTAL_CODE.Text <> "" And TB_SR_POSTAL_CODE.Text <> "&nbsp;" Then
                        If TB_SR_POSTAL_CODE.Text.Contains("-") = False Then
                            Dim kod_p As String = TB_SR_POSTAL_CODE.Text.ToString
                            TB_SR_POSTAL_CODE.Text = kod_p.Substring(0, 2).ToString & "-" & kod_p.Substring(2, 3).ToString
                        End If
                    End If
                Else
                    Dim tbrow As String() = {"TBROW_DDLFirma", "TBROW_SR_NAME", "TBROW_SR_COMPANY_INPOST", "TBROW_SR_FIRSTNAME_INPOST", "TBROW_SR_LASTNAME_INPOST", "TBROW_SR_POSTAL_CODE", "TBROW_SR_CITY", "TBROW_SR_STREET", "TBROW_SR_HOUSE_NUM", "TBROW_SR_APART_NUM", "TBROW_PC_PERSON_NAME", "TBROW_PC_PHONE_NUM", "TBROW_PC_EMAIL_ADD", "TBROW_SR_COUNTRY", "TBROW_TB_ST_SHIPMENT_DATE", "TBROW_TB_B_COSTS_CENTER", "TBROW_LABEL_TYPE_INPOST"}
                    ''Dim tbrow As String() = {"TBROW_DDLFirma", "TBROW_SR_POST_NUM", "TBROW_SERVICE_TYPE_INPOST", "TBROW_SR_NAME", "TBROW_SR_COMPANY_INPOST", "TBROW_SR_FIRSTNAME_INPOST", "TBROW_SR_LASTNAME_INPOST", "TBROW_SR_POSTAL_CODE", "TBROW_SR_CITY", "TBROW_SR_STREET", "TBROW_SR_HOUSE_NUM", "TBROW_SR_APART_NUM", "TBROW_PC_PERSON_NAME", "TBROW_PC_PHONE_NUM", "TBROW_PC_EMAIL_ADD", "TBROW_SR_COUNTRY", "TBROW_TB_ST_SHIPMENT_DATE", "TBROW_TB_B_COSTS_CENTER", "TBROW_LABEL_TYPE_INPOST"}
                    For Each t In tbrow
                        Dim tr As Panel = FindControl(t)
                        tr.Visible = False
                    Next

                    Dim tbcell As String() = {"TBCELL_RodzajINPOST_ALLEGRO"}
                    For Each t In tbcell
                        Dim tc As Panel = FindControl(t)
                        tc.Visible = False
                    Next

                    WymiarySzczegoloweINPOST.Visible = False
                    TBOpisZam.Visible = False

                    If TBOpisZam.Text.ToUpper.Contains("POBRANIE") = True Or TBOpisZam.Text.ToUpper.Contains("CASHONDELIVERY") = True Then
                        UslugaCodINPOST.Visible = False
                    End If

                End If

                If shipping_address.Contains("PaczkoPunkt") Then
                    ''<asp:ListItem Value = "inpost_locker_standard" > Przesyłka paczkomatowa - standardowa</asp: ListItem>
                    ''<asp:ListItem Value="inpost_locker_allegro">Przesyłka paczkomatowa - Allegro Paczkomaty InPost.</asp:ListItem>
                    paczka_smart = False
                    TB_SERVICE_TYPE_INPOST.Text = "inpost_locker_standard"

                    If increment_id.StartsWith("SPL") Then
                        TB_SERVICE_TYPE_INPOST.Text = "inpost_locker_standard"
                    ElseIf increment_id.StartsWith("SAL") Then
                        TB_SERVICE_TYPE_INPOST.Text = "inpost_locker_allegro"
                    End If

                    Session("firma_id") = "INPOST_ALLEGRO"
                    ''RefreshDDLFirma(DDLFirma, Session("firma_id"))
                    DDLFirma.SelectedValue = Session("firma_id")
                    obj_paczka.INPOST_TYPE = "paczkomaty"

                    Dim tbrow As String() = {"TBROW_SR_NAME", "TBROW_SR_COMPANY_INPOST", "TBROW_SR_FIRSTNAME_INPOST", "TBROW_SR_LASTNAME_INPOST", "TBROW_SR_POSTAL_CODE", "TBROW_SR_CITY", "TBROW_SR_STREET", "TBROW_SR_HOUSE_NUM", "TBROW_SR_APART_NUM", "TBROW_PC_PERSON_NAME", "TBROW_PC_PHONE_NUM", "TBROW_PC_EMAIL_ADD"}
                    ''Dim tbrow As String() = {"TBROW_SR_POST_NUM", "TBROW_SERVICE_TYPE_INPOST", "TBROW_SR_NAME", "TBROW_SR_COMPANY_INPOST", "TBROW_SR_FIRSTNAME_INPOST", "TBROW_SR_LASTNAME_INPOST", "TBROW_SR_POSTAL_CODE", "TBROW_SR_CITY", "TBROW_SR_STREET", "TBROW_SR_HOUSE_NUM", "TBROW_SR_APART_NUM", "TBROW_PC_PERSON_NAME", "TBROW_PC_PHONE_NUM", "TBROW_PC_EMAIL_ADD"}
                    For Each t In tbrow
                        Dim tr As Panel = FindControl(t)
                        tr.Visible = False
                    Next

                    tbrow = {"TBROW_DDLFirma"}
                    For Each t In tbrow
                        Dim tr As Panel = FindControl(t)
                        tr.Visible = True
                    Next

                    Dim tbcell As String() = {"TBCELL_RodzajINPOST_ALLEGRO"}
                    For Each t In tbcell
                        Dim tc As Panel = FindControl(t)
                        tc.Visible = True
                    Next

                    If DDLRodzajINPOST_ALLEGRO.SelectedValue = "A" Then
                        DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text = "8"
                        DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text = "38"
                        DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text = "64"
                        DirectCast(FindControl("TBWaga"), TextBox).Text = "1"
                    End If

                    Dim wzorzecReguly As String = "PACZKOPUNKT\s?[A-Z0-9-]+"
                    Dim rx As Regex = New Regex(wzorzecReguly)
                    Dim wzorzecString As String = TBOpisZam.Text

                    Dim m As Match = rx.Match(wzorzecString)
                    If m.Success = True Then
                        TB_SR_POST_NUM.Text = m.Value.ToString.Replace("PACZKOPUNKT ", "").Replace("PACZKOPUNKT", "").ToString
                    End If

                    TB_SR_COMPANY_INPOST.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_company from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)
                    TB_SR_FIRSTNAME_INPOST.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_firstname from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)
                    TB_SR_LASTNAME_INPOST.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_lastname from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)

                ElseIf shipping_desc.Contains("Paczkomaty") Then
                    ''<asp:ListItem Value = "inpost_locker_standard" > Przesyłka paczkomatowa - standardowa</asp: ListItem>
                    ''<asp:ListItem Value="inpost_locker_allegro">Przesyłka paczkomatowa - Allegro Paczkomaty InPost.</asp:ListItem>
                    TB_SERVICE_TYPE_INPOST.Text = "inpost_locker_standard"

                    If increment_id.StartsWith("SPL") Then
                        TB_SERVICE_TYPE_INPOST.Text = "inpost_locker_standard"
                    ElseIf increment_id.StartsWith("SAL") Then
                        TB_SERVICE_TYPE_INPOST.Text = "inpost_locker_allegro"
                    End If

                    If shipping_amount <> "0" Then
                        paczka_smart = False
                        ''TB_SERVICE_TYPE_INPOST.Text = "inpost_locker_standard"
                    End If

                    Session("firma_id") = "INPOST_ALLEGRO"
                    ''RefreshDDLFirma(DDLFirma, Session("firma_id"))
                    DDLFirma.SelectedValue = Session("firma_id")
                    obj_paczka.INPOST_TYPE = "paczkomaty"

                    Dim tbrow As String() = {"TBROW_SR_NAME", "TBROW_SR_COMPANY_INPOST", "TBROW_SR_FIRSTNAME_INPOST", "TBROW_SR_LASTNAME_INPOST", "TBROW_SR_POSTAL_CODE", "TBROW_SR_CITY", "TBROW_SR_STREET", "TBROW_SR_HOUSE_NUM", "TBROW_SR_APART_NUM", "TBROW_PC_PERSON_NAME", "TBROW_PC_PHONE_NUM", "TBROW_PC_EMAIL_ADD"}
                    ''Dim tbrow As String() = {"TBROW_SR_POST_NUM", "TBROW_SERVICE_TYPE_INPOST", "TBROW_SR_NAME", "TBROW_SR_COMPANY_INPOST", "TBROW_SR_FIRSTNAME_INPOST", "TBROW_SR_LASTNAME_INPOST", "TBROW_SR_POSTAL_CODE", "TBROW_SR_CITY", "TBROW_SR_STREET", "TBROW_SR_HOUSE_NUM", "TBROW_SR_APART_NUM", "TBROW_PC_PERSON_NAME", "TBROW_PC_PHONE_NUM", "TBROW_PC_EMAIL_ADD"}
                    For Each t In tbrow
                        Dim tr As Panel = FindControl(t)
                        tr.Visible = False
                    Next

                    tbrow = {"TBROW_DDLFirma"}
                    For Each t In tbrow
                        Dim tr As Panel = FindControl(t)
                        tr.Visible = True
                    Next

                    Dim tbcell As String() = {"TBCELL_RodzajINPOST_ALLEGRO"}
                    For Each t In tbcell
                        Dim tc As Panel = FindControl(t)
                        tc.Visible = True
                    Next

                    If DDLRodzajINPOST_ALLEGRO.SelectedValue = "A" Then
                        DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text = "8"
                        DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text = "38"
                        DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text = "64"
                        DirectCast(FindControl("TBWaga"), TextBox).Text = "1"
                    End If

                    Dim wzorzecReguly As String = "PACZKOMAT\s?[A-Z0-9]+"
                    Dim rx As Regex = New Regex(wzorzecReguly)
                    Dim wzorzecString As String = TBOpisZam.Text

                    Dim m As Match = rx.Match(wzorzecString)
                    If m.Success = True Then
                        TB_SR_POST_NUM.Text = m.Value.ToString.Replace("PACZKOMAT ", "").Replace("PACZKOMAT", "").ToString
                    End If

                    TB_SR_COMPANY_INPOST.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_company from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)
                    TB_SR_FIRSTNAME_INPOST.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_firstname from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)
                    TB_SR_LASTNAME_INPOST.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_lastname from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)

                    ''##2024.06.27 / blokada edycji pol dlug/wys/szer dla paczkomatow dla operatora P
                    DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).ReadOnly = True
                    DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).ReadOnly = True
                    DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).ReadOnly = True

                    ''##2023.07.13 / dodanie obslugi paczkomaty empik
                ElseIf shipping_address.Contains("Paczkomat") Then
                    ''<asp:ListItem Value = "inpost_locker_standard" > Przesyłka paczkomatowa - standardowa</asp: ListItem>
                    ''<asp:ListItem Value="inpost_locker_allegro">Przesyłka paczkomatowa - Allegro Paczkomaty InPost.</asp:ListItem>
                    paczka_smart = False
                    TB_SERVICE_TYPE_INPOST.Text = "inpost_locker_standard"

                    Session("firma_id") = "INPOST_ALLEGRO"
                    ''RefreshDDLFirma(DDLFirma, Session("firma_id"))
                    DDLFirma.SelectedValue = Session("firma_id")
                    obj_paczka.INPOST_TYPE = "paczkomaty"

                    Dim tbrow As String() = {"TBROW_SR_NAME", "TBROW_SR_COMPANY_INPOST", "TBROW_SR_FIRSTNAME_INPOST", "TBROW_SR_LASTNAME_INPOST", "TBROW_SR_POSTAL_CODE", "TBROW_SR_CITY", "TBROW_SR_STREET", "TBROW_SR_HOUSE_NUM", "TBROW_SR_APART_NUM", "TBROW_PC_PERSON_NAME", "TBROW_PC_PHONE_NUM", "TBROW_PC_EMAIL_ADD"}
                    ''Dim tbrow As String() = {"TBROW_SR_POST_NUM", "TBROW_SERVICE_TYPE_INPOST", "TBROW_SR_NAME", "TBROW_SR_COMPANY_INPOST", "TBROW_SR_FIRSTNAME_INPOST", "TBROW_SR_LASTNAME_INPOST", "TBROW_SR_POSTAL_CODE", "TBROW_SR_CITY", "TBROW_SR_STREET", "TBROW_SR_HOUSE_NUM", "TBROW_SR_APART_NUM", "TBROW_PC_PERSON_NAME", "TBROW_PC_PHONE_NUM", "TBROW_PC_EMAIL_ADD"}
                    For Each t In tbrow
                        Dim tr As Panel = FindControl(t)
                        tr.Visible = False
                    Next

                    tbrow = {"TBROW_DDLFirma"}
                    For Each t In tbrow
                        Dim tr As Panel = FindControl(t)
                        tr.Visible = True
                    Next

                    Dim tbcell As String() = {"TBCELL_RodzajINPOST_ALLEGRO"}
                    For Each t In tbcell
                        Dim tc As Panel = FindControl(t)
                        tc.Visible = True
                    Next

                    If DDLRodzajINPOST_ALLEGRO.SelectedValue = "A" Then
                        DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text = "8"
                        DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text = "38"
                        DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text = "64"
                        DirectCast(FindControl("TBWaga"), TextBox).Text = "1"
                    End If

                    Dim wzorzecReguly As String = "PACZKOMAT\s?[A-Z0-9]+"
                    Dim rx As Regex = New Regex(wzorzecReguly)
                    Dim wzorzecString As String = TBOpisZam.Text

                    Dim m As Match = rx.Match(wzorzecString)
                    If m.Success = True Then
                        TB_SR_POST_NUM.Text = m.Value.ToString.Replace("PACZKOMAT ", "").Replace("PACZKOMAT", "").ToString
                    End If

                    TB_SR_COMPANY_INPOST.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_company from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)
                    TB_SR_FIRSTNAME_INPOST.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_firstname from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)
                    TB_SR_LASTNAME_INPOST.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_lastname from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)
                    ''##2023.11.09 / dodanie obslugi paczkopunkt dla INPOST

                    ''##2024.06.27 / blokada edycji pol dlug/wys/szer dla paczkomatow dla operatora P
                    DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).ReadOnly = True
                    DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).ReadOnly = True
                    DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).ReadOnly = True

                ElseIf shipping_desc.Contains("Kurier24 InPost") Then
                    ''<asp:ListItem Value = "inpost_courier_allegro" > Przesyłka kurierska - Allegro Kurier24 InPost.</asp: ListItem>
                    ''<asp:ListItem Value="inpost_courier_standard"> Przesyłka kurierska standardowa</asp:ListItem>
                    TB_SERVICE_TYPE_INPOST.Text = "inpost_courier_allegro"
                    obj_paczka.INPOST_TYPE = "kurier"
                    DDLRodzajINPOST_ALLEGRO.SelectedValue = "RECZNIE"
                    DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text = ""
                    DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text = ""
                    DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text = ""
                    DirectCast(FindControl("TBWaga"), TextBox).Text = "1"


                    If shipping_amount <> "0" Then
                        paczka_smart = False
                        TB_SERVICE_TYPE_INPOST.Text = "inpost_courier_standard"
                    End If
                    Session("firma_id") = "INPOST_ALLEGRO"
                    ''RefreshDDLFirma(DDLFirma, Session("firma_id"))
                    DDLFirma.SelectedValue = Session("firma_id")
                End If

            Else
                GridViewIndeksy.DataSource = Nothing
                GridViewIndeksy.DataBind()
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BPobierzPDF_INPOST_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BPobierzPDF_INPOST.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim przerwij_tworzenie_inpost As Boolean = False

            Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
            Session.Remove("contentKomunikat")
            If Session("firma_id").ToString.Contains("INPOST") Then
                If Session("shipment_id") <> "" Then
                    Dim shippment_id As String = Session("shipment_id").ToString
                    Dim inpost_session As New inpost_package.InpostSession
                    Dim nr_zamow As String = Session("kod_dyspo").ToString
                    Dim nr_zamow_klient As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select nr_zamow_o from ht_zog where ie$14=desql_graf.df11_2('" & nr_zamow & "')", conn)

                    ''#############################################################################################################################################
                    ''##########################################################I.TWORZENIE OFERTY#################################################################
                    ''#############################################################################################################################################
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

                    If przerwijGenerowanie = False Then
                        ''Dim nr_zamow As String = Session("kod_dyspo").ToString
                        ''Dim nr_zamow_klient As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select nr_zamow_o from ht_zog where ie$14=desql_graf.df11_2('" & nr_zamow & "')", conn)

                        Session("mag_dyspo") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_zog WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                        If Session("firma_id").ToString.Contains("INPOST") Then

                            Dim ss_service_cod As Boolean = False
                            ''##2023.01.16 / tworzenie dodatkowej uslugi COD dla kurier inpost pobranie
                            If TBOpisZam.Text.ToUpper.Contains("POBRANIE") = True Or TBOpisZam.Text.ToUpper.Contains("CASHONDELIVERY") = True Then
                                Dim ss_service_type As String = "cod"
                                Dim ss_service_value As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select wartosc from ht_zog where ie$14 = desql_graf.df11_2('" & nr_zamow & "')", conn)
                                If ss_service_type = "cod" Or ss_service_type = "insurance" Then
                                    ss_service_value = ss_service_value.ToString.Replace(",", ".")
                                End If

                                sqlexp = "select count(*) from dp_swm_mia_paczka_ss where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "'"
                                Dim czyIstniejeUsluga As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                                If czyIstniejeUsluga <> "0" Then
                                    sqlexp = "update dp_swm_mia_paczka_ss set ss_service_value='" & ss_service_value & "' where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "'"
                                Else
                                    sqlexp = "insert into dp_swm_mia_paczka_ss (schemat,shipment_id,ss_service_type,ss_service_value) values('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & ss_service_type & "','" & ss_service_value & "')"
                                End If
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                                ss_service_cod = True
                            End If

                            If DirectCast(FindControl("TB_SERVICE_TYPE_INPOST"), TextBox).Text.ToString <> "" Then

                                inpost_session = New inpost_package.InpostSession

                                inpost_package.inpost_login(inpost_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))

                                Dim company As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_company from dp_rest_mag_order where increment_id='" & nr_zamow_klient.ToString & "'", conn)
                                Dim first_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_firstname from dp_rest_mag_order where increment_id='" & nr_zamow_klient.ToString & "'", conn)
                                Dim last_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_lastname from dp_rest_mag_order where increment_id='" & nr_zamow_klient.ToString & "'", conn)

                                inpost_session.i_shipment.i_receiver = inpost_package.inpost_loadReceiver(DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_COMPANY_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_FIRSTNAME_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_LASTNAME_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_EMAIL_ADD"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PHONE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_City"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, "PL")

                                If ss_service_cod = True Then
                                    Dim ss_service_type As String = "cod"
                                    Dim ss_service_value As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select ss_service_value from dp_swm_mia_paczka_ss where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "' ", conn)
                                    inpost_session.i_shipment.i_cod = inpost_package.inpost_loadCod(ss_service_value, "PLN")
                                    inpost_session.i_shipment.i_insurance = inpost_package.inpost_loadInsurance(ss_service_value.ToString, "PLN")
                                End If

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

                                inpost_session.i_shipment.i_service = TB_SERVICE_TYPE_INPOST.Text.ToString
                                inpost_session.i_shipment.i_comments = ""
                                inpost_session.i_shipment.i_mpk = TB_B_COSTS_CENTER.Text.ToString
                                inpost_session.i_shipment.i_reference = TB_COMMENT_F_INPOST.Text.ToString

                                Dim inpost_custom As New inpost_package.CustomAttributes
                                inpost_custom.target_point = TB_SR_POST_NUM.Text.ToString
                                ''inpost_custom.target_point = ""
                                inpost_session.i_shipment.i_custom_attributes = inpost_custom

                                ''####2022.08.04 / tworzenie etykiety w trybie ofertowym / mozliwosc pozniejszej edycji oraz usuniecia
                                inpost_session.i_shipment.i_only_choice_of_offer = "true"
                                Dim shipment_id As String = inpost_package.REST_POST_CreateShipment(inpost_session.i_shipment)

                                If shipment_id <> "" Then
                                    Dim inpost_label_id As String = inpost_package.REST_GET_ParcelTrackingNumber(shipment_id)
                                    If inpost_label_id = "" Then
                                        inpost_label_id = shipment_id
                                    End If

                                    sqlexp = "update dp_swm_mia_paczka set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '%" & Session("kod_dyspo") & "%'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & inpost_label_id & "',shipment_id='" & inpost_label_id & "' WHERE shipment_id like '" & shippment_id.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), inpost_label_id, nr_zamow)

                                    ''zmiana numeru listu przewozowego
                                    Session("shipment_id") = inpost_label_id
                                    LNr_list_przewozowy.Text = Session("shipment_id")
                                Else
                                    przerwij_tworzenie_inpost = True
                                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                                    Session(session_id) += "<br />Komunikat INPOST : " & inpost_package.exception.ToString & "<br />"
                                    Session(session_id) += "</div>"
                                End If

                            End If
                        End If
                    End If

                    Threading.Thread.Sleep(2000)

                    ''#############################################################################################################################################
                    ''##########################################################II.OPLACENIE OFERTY#################################################################
                    ''#############################################################################################################################################
                    ''Dim shippment_id As String = Session("shipment_id").ToString
                    ''Dim nr_zamow As String = Session("kod_dyspo").ToString
                    ''Dim nr_zamow_klient As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select nr_zamow_o from ht_zog where ie$14=desql_graf.df11_2('" & nr_zamow & "')", conn)
                    ''shippment_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                    If Session("firma_id").ToString.Contains("INPOST") And przerwij_tworzenie_inpost = False Then

                        If DirectCast(FindControl("TB_SERVICE_TYPE_INPOST"), TextBox).Text.ToString <> "" Then
                            inpost_session = New inpost_package.InpostSession

                            inpost_package.inpost_login(inpost_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))
                            shippment_id = Session("shipment_id").ToString

                            Dim status As String = inpost_package.REST_GET_StatusOfShipmentId(shippment_id)
                            If status = "offer_selected" Then
                                Dim offer_id As String = inpost_package.REST_GET_OfferIdOfShipmentId(shippment_id)
                                If offer_id <> "" Then
                                    Dim shipment_id As String = inpost_package.REST_POST_BuyShipment(shippment_id, offer_id)
                                    If shipment_id <> "" Then
                                        Threading.Thread.Sleep(1000)
                                        Dim inpost_label_id As String = inpost_package.REST_GET_ParcelTrackingNumber(shipment_id)
                                        If inpost_label_id = "" Then
                                            inpost_label_id = shipment_id
                                        End If

                                        sqlexp = "update dp_swm_mia_paczka set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                        sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '" & Session("kod_dyspo") & "%'"
                                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                        sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & inpost_label_id & "',shipment_id='" & inpost_label_id & "' WHERE shipment_id like '" & shippment_id.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                        sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                                        result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), inpost_label_id, nr_zamow)

                                        ''zmiana numeru listu przewozowego
                                        Session("shipment_id") = inpost_label_id
                                        LNr_list_przewozowy.Text = Session("shipment_id")
                                    Else
                                        przerwij_tworzenie_inpost = True
                                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                                        Session(session_id) += "<br />Komunikat INPOST : " & inpost_package.exception.ToString & "<br />"
                                        Session(session_id) += "</div>"
                                    End If

                                End If

                            End If
                        End If
                    End If

                    ''#############################################################################################################################################
                    ''##########################################################III.GENEROWANIE ETYKIETY PDF#################################################################
                    ''#############################################################################################################################################
                    ''Threading.Thread.Sleep(2000)

                    If Session("firma_id").ToString.Contains("INPOST") And przerwij_tworzenie_inpost = False Then

                        inpost_session = New inpost_package.InpostSession
                        inpost_package.inpost_login(inpost_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))

                        shippment_id = Session("shipment_id").ToString
                        Dim inpost_shipment As String = ""
                        If shippment_id.Length = 24 Then
                            inpost_shipment = inpost_package.REST_GET_ShipmentId(Session("shipment_id"))
                        Else
                            Dim inpost_label_id As String = inpost_package.REST_GET_ParcelTrackingNumber(Session("shipment_id"))
                            If inpost_label_id <> "" And inpost_label_id.Length = 24 Then
                                sqlexp = "update dp_swm_mia_paczka set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '" & Session("kod_dyspo") & "%'"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & inpost_label_id & "',shipment_id='" & inpost_label_id & "' WHERE shipment_id like '" & shippment_id.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
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
                                inpost_shipment_id = HLEtykietaPDF_INPOST.Text.ToString

                                WebClientPrint.LicenseOwner = "Dajar Sp. z o.o. - 1 WebApp Lic - 1 WebServer Lic"
                                WebClientPrint.LicenseKey = "273D8F1AB12B42847C2E9FE7717B6756A2CA0026"

                                Application.Lock()
                                Application("session_id_" + Session.SessionID) = Session.SessionID.ToString
                                Application("shipment_id_" + Session.SessionID) = Session("shipment_id").ToString
                                Application("tracking_number_" + Session.SessionID) = Session("shipment_id").ToString
                                Application("kod_dyspo_" + Session.SessionID) = Session("kod_dyspo").ToString
                                Application("firma_id_" + Session.SessionID) = Session("firma_id").ToString
                                Application("mylogin_" + Session.SessionID) = Session("mylogin").ToString
                                Application("myhash_" + Session.SessionID) = Session("myhash").ToString
                                Application.UnLock()

                                dajarSWMagazyn_MIA.MyFunction.SetPrintDyspozycja(dajarSWMagazyn_MIA.MyFunction.GetRemoteIp, Session.SessionID.ToString, Session("kod_dyspo").ToString, Session("mylogin").ToString, Session("myhash").ToString, Session("shipment_id").ToString)
                                ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "script", "javascript:jsWebClientPrint.print('useDefaultPrinter=' + $('#useDefaultPrinter').attr('checked') + '&printerName=' + $('#installedPrinterName').val() + '&sessionId=" + Session.SessionID + "');", True)
                                ''ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "script", "javascript:jsWebClientPrint.print('useDefaultPrinter=' + $('#useDefaultPrinter').attr('checked') + '&printerName=' + $('#installedPrinterName').val());", True)
                            Else
                                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                                Session(session_id) += "<br />Komunikat INPOST : " & inpost_package.exception.ToString & "<br />"
                                Session(session_id) += "</div>"
                            End If
                        End If
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
                ''przerwijGenerowanie = True
                ''Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                ''Session(session_id) += "<br />Dla wybranych paczek istnieje juz utworzona etykieta INPOST!<br />"
                ''Session(session_id) += "</div>"

                ''##2023.01.18 / tworzenie nowego identyfikatora dla paczek INPOST
                ''Dim shipment_new As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                ''sqlexp = "update dp_swm_mia_paczka set shipment_id='" & shipment_new & "' WHERE shipment_id='" & Session("shipment_id").ToString & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                ''result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                ''sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & shipment_new & "' WHERE shipment_id='" & Session("shipment_id").ToString & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                ''result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                ''sqlexp = "update dp_swm_mia_paczka_info set shipment_id='" & shipment_new & "' WHERE shipment_id like '" & Session("shipment_id").ToString.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                ''result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                ''sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & shipment_new & "' WHERE shipment_id='" & Session("shipment_id").ToString & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                ''result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
            End If

            If przerwijGenerowanie = False Then
                Dim shippment_id As String = Session("shipment_id").ToString
                Dim nr_zamow As String = Session("kod_dyspo").ToString
                Dim nr_zamow_klient As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select nr_zamow_o from ht_zog where ie$14=desql_graf.df11_2('" & nr_zamow & "')", conn)

                Session("mag_dyspo") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_zog WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                ''shippment_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                If Session("firma_id").ToString.Contains("INPOST") Then

                    Dim ss_service_cod As Boolean = False
                    ''##2023.01.16 / tworzenie dodatkowej uslugi COD dla kurier inpost pobranie
                    If TBOpisZam.Text.ToUpper.Contains("POBRANIE") = True Then
                        Dim ss_service_type As String = "cod"
                        Dim ss_service_value As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select wartosc from ht_zog where ie$14 = desql_graf.df11_2('" & nr_zamow & "')", conn)
                        If ss_service_type = "cod" Or ss_service_type = "insurance" Then
                            ss_service_value = ss_service_value.ToString.Replace(",", ".")
                        End If

                        sqlexp = "select count(*) from dp_swm_mia_paczka_ss where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "'"
                        Dim czyIstniejeUsluga As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        If czyIstniejeUsluga <> "0" Then
                            sqlexp = "update dp_swm_mia_paczka_ss set ss_service_value='" & ss_service_value & "' where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "'"
                        Else
                            sqlexp = "insert into dp_swm_mia_paczka_ss (schemat,shipment_id,ss_service_type,ss_service_value) values('" & Session("schemat_dyspo") & "','" & shippment_id & "','" & ss_service_type & "','" & ss_service_value & "')"
                        End If
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        ss_service_cod = True
                    End If


                    If DirectCast(FindControl("TB_SERVICE_TYPE_INPOST"), TextBox).Text.ToString <> "" Then

                        Dim inpost_session As New inpost_package.InpostSession

                        inpost_package.inpost_login(inpost_session, Session("schemat_dyspo"), Session("mag_dyspo"), Session("firma_id"))

                        Dim company As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_company from dp_rest_mag_order where increment_id='" & nr_zamow_klient.ToString & "'", conn)
                        Dim first_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_firstname from dp_rest_mag_order where increment_id='" & nr_zamow_klient.ToString & "'", conn)
                        Dim last_name As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select customer_lastname from dp_rest_mag_order where increment_id='" & nr_zamow_klient.ToString & "'", conn)

                        inpost_session.i_shipment.i_receiver = inpost_package.inpost_loadReceiver(DirectCast(FindControl("TB_SR_Name"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_COMPANY_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_FIRSTNAME_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_LASTNAME_INPOST"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_EMAIL_ADD"), TextBox).Text.ToString, DirectCast(FindControl("TB_PC_PHONE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_STREET"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_HOUSE_NUM"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_City"), TextBox).Text.ToString, DirectCast(FindControl("TB_SR_POSTAL_CODE"), TextBox).Text.ToString, "PL")

                        If ss_service_cod = True Then
                            Dim ss_service_type As String = "cod"
                            Dim ss_service_value As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select ss_service_value from dp_swm_mia_paczka_ss where shipment_id='" & shippment_id & "' and ss_service_type='" & ss_service_type & "' ", conn)
                            inpost_session.i_shipment.i_cod = inpost_package.inpost_loadCod(ss_service_value, "PLN")
                            inpost_session.i_shipment.i_insurance = inpost_package.inpost_loadInsurance(ss_service_value.ToString, "PLN")
                        End If

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
                                        inpost_session.i_shipment.i_parcels.Add(inpost_package.inpost_loadParcel("package", inpost_paczka_id, dlug * 10, szer * 10, wys * 10, "mm", waga, "kg", is_non_standard))
                                    End If
                                End If
                            End If
                        Next

                        inpost_session.i_shipment.i_service = TB_SERVICE_TYPE_INPOST.Text.ToString
                        inpost_session.i_shipment.i_comments = ""
                        inpost_session.i_shipment.i_mpk = TB_B_COSTS_CENTER.Text.ToString
                        inpost_session.i_shipment.i_reference = TB_COMMENT_F_INPOST.Text.ToString

                        Dim inpost_custom As New inpost_package.CustomAttributes
                        inpost_custom.target_point = TB_SR_POST_NUM.Text.ToString
                        ''inpost_custom.target_point = ""
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

                            sqlexp = "update dp_swm_mia_paczka set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'  and comment_f like '" & Session("kod_dyspo") & "%'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & inpost_label_id & "',shipment_id='" & inpost_label_id & "' WHERE shipment_id like '" & shippment_id.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            ''##2022.12.08 / wylaczenie iteracji po liscie zamowien
                            ''For Each row As GridViewRow In GridViewZamowienia.Rows
                            ''    Dim nr_zam As String = row.Cells(2).Text.ToString
                            ''    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), inpost_label_id, nr_zam)
                            ''Next

                            result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), inpost_label_id, nr_zamow)

                            ''zmiana numeru listu przewozowego
                            Session("shipment_id") = inpost_label_id
                            LNr_list_przewozowy.Text = Session("shipment_id")
                            ''LadujDaneGridViewZamowienia()
                            ''LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))



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
                ''przerwijGenerowanie = True
                ''Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                ''Session(session_id) += "<br />Dla wybranych zamowien brakuje utworzonych ofert INPOST!<br />"
                ''Session(session_id) += "</div>"
            End If

            If przerwijGenerowanie = False Then
                Dim shippment_id As String = Session("shipment_id").ToString
                Dim nr_zamow As String = Session("kod_dyspo").ToString
                Dim nr_zamow_klient As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select nr_zamow_o from ht_zog where ie$14=desql_graf.df11_2('" & nr_zamow & "')", conn)
                ''shippment_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                If Session("firma_id").ToString.Contains("INPOST") Then

                    If DirectCast(FindControl("TB_SERVICE_TYPE_INPOST"), TextBox).Text.ToString <> "" Then
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

                                    sqlexp = "update dp_swm_mia_paczka set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_base set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '" & Session("kod_dyspo") & "%'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_info set tracking_number='" & inpost_label_id & "',shipment_id='" & inpost_label_id & "' WHERE shipment_id like '" & shippment_id.Replace("SH", "S%") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                    sqlexp = "update dp_swm_mia_paczka_ss set shipment_id='" & inpost_label_id & "' WHERE shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                                    ''##2022.12.08 / wylaczenie iteracji po liscie zamowien
                                    ''For Each row As GridViewRow In GridViewZamowienia.Rows
                                    ''    Dim nr_zam As String = row.Cells(2).Text.ToString
                                    ''    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), inpost_label_id, nr_zam)
                                    ''Next
                                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), inpost_label_id, nr_zamow)


                                    ''zmiana numeru listu przewozowego
                                    Session("shipment_id") = inpost_label_id
                                    LNr_list_przewozowy.Text = Session("shipment_id")
                                    ''LadujDaneGridViewZamowienia()
                                    ''LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))
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

    '    Protected Sub BWyswietlSzczegoly_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWyswietlSzczegoly.Click
    '        WyswietlSzczegolyZamowienia()
    '    End Sub

    Public Sub AktualizujGridViewIndeksy(ByVal lista_zamowien As List(Of ZamowieniaInformacje))
        GridViewIndeksy.DataSource = Nothing
        Dim indeksy_dtable As New DataTable
        Dim polaDataTable As String() = {"LP", "NR_ZAMOW", "NR_ZAMOW_O", "SKROT", "NAZWA", "KOD_TOW", "ILE_POZ", "JM"}
        For Each pole In polaDataTable
            indeksy_dtable.Columns.Add(New DataColumn(pole, Type.GetType("System.String")))
        Next

        Dim sqlexp_DAJAR As String = ""
        Dim sqlexp_DOMINUS As String = ""

        For Each obj_zam In lista_zamowien
            If obj_zam.schemat = "DAJAR" Or obj_zam.schemat = "DOMINUS" Then
                sqlexp_DAJAR &= " union all" _
                & " select rownum as lp,w.nr_zamow, w.nr_zamow_o, w.skrot, desql_japa_nwa.fsql_japa_rnaz(get_index_tow(w.skrot)) nazwa, " _
                & " desql_japa_nwa.fsql_japa_rkod(get_index_tow(w.skrot)) kod_tow, w.ile_poz, w.jm, w.mag, w.status from (" _
                & " select dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                & " zg.data_zam, 'M' typ_oper," _
                & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag," _
                & " (select distinct zd.jm from ht_zod zd where zd.ie$0 like dm.nr_zamow||'%' and zd.is_deleted = 'N' and zd.skrot = dm.skrot) jm" _
                & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.status in('PA','PE','MG','WN','ZW','PP')" _
                & " and dm.nr_zamow in('" & obj_zam.nr_zamow & "') and dm.schemat='" & obj_zam.schemat & "'" _
                & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat order by dm.nr_zamow desc" _
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

        AktualizujGridViewIndeksyStatusy()

    End Sub

    Public Sub AktualizujGridViewIndeksyStatusy()
        ''    If GridViewIndeksy.Rows.Count > 0 Then
        ''        For Each row As GridViewRow In GridViewIndeksy.Rows
        ''            Dim mag As String = row.Cells(9).Text.ToString
        ''            Dim status As String = row.Cells(10).Text.ToString
        ''            If mag <> Session("contentMagazynCel") And (status = "PA" Or status = "MG") Then
        ''                row.BackColor = LPakowanieNaObcym.BackColor
        ''            ElseIf mag <> Session("contentMagazynCel") And status = "PP" Then
        ''                row.BackColor = LPakowanieWyslanoMagazyn.BackColor
        ''            End If
        ''        Next
        ''    End If
    End Sub

    Public Sub LadujDaneGridViewPakowanie()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            GridViewPakowanie.SelectedIndex = -1
            ''##2023.01.24 / wylaczenie czyszczenie informacji o etykiecie 
            ''CzyszczenieInformacjiEtykieta()

            If Session("mytyp_oper") = "P" Then
                sqlexp = "select w.nr_zamow,w.schemat,w.nr_zamow_o,w.data_dr,w.data_mg,w.data_pak,w.stat_u1 from (" _
                & " select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                & " (select count(*) from dp_swm_mia_mag where nr_zamow = dm.nr_zamow and schemat = dm.schemat and status in ('PA') and mag = 700) ile_poz," _
                & " (select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_mia_oper do where do.status = dm.nr_zamow and do.typ_oper like '%['||dm.schemat||']%' and do.typ_oper like 'MG%') data_mg," _
                & " (select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_mia_oper do where do.status = dm.nr_zamow and do.typ_oper like '%['||dm.schemat||']%' and do.typ_oper like 'PA%') data_pak," _
                & " (select min(to_char(db.autodata,'YYYY/MM/DD')) from dp_swm_mia_buf db where db.status in('DR','RP') and db.nr_zamow=dm.nr_zamow) data_dr," _
                & " zg.autodata data_mag, 'P' typ_oper,'' etykieta_id, dm.mag,zg.stat_u1,case when zg.schemat='DAJAR' then (select kod_kontr from ht_zog where recno=zg.recno) else (select kod_kontr from ht_zog where recno=zg.recno) end kontr" _
                & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.status in ('PA') and zg.nr_zamow not in(select distinct nr_zamow from dp_swm_mia_pak where nr_zamow = dm.nr_zamow and schemat = dm.schemat)" _
                & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat and dm.mag = 700" _
                & " union all" _
                & " select distinct dp.nr_zamow,dp.schemat,zg.nr_zamow_o," _
                & " (select count(*) from dp_swm_mia_mag where nr_zamow = dp.nr_zamow and status in ('PA') and mag=700) ile_poz," _
                & " (select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_mia_oper do where do.status = dp.nr_zamow and do.typ_oper like '%['||dp.schemat||']%' and do.typ_oper like 'MG%') data_mg," _
                & " (select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_mia_oper do where do.status = dp.nr_zamow and do.typ_oper like '%['||dp.schemat||']%' and do.typ_oper like 'PA%') data_pak," _
                & " (select min(to_char(db.autodata,'YYYY/MM/DD')) from dp_swm_mia_buf db where db.status in('DR','RP') and db.nr_zamow=dp.nr_zamow) data_dr," _
                & " zg.autodata data_mag, 'P' typ_oper,(select max(dt.shipment_id) from dp_swm_mia_paczka dt where dt.nr_zamow=dp.nr_zamow and dt.schemat=dp.schemat) etykieta_id,'700',zg.stat_u1,case when zg.schemat='DAJAR' then (select kod_kontr from ht_zog where recno=zg.recno) else (select kod_kontr from ht_zog where recno=zg.recno) end kontr" _
                & " from dp_swm_mia_pak dp, dp_swm_mia_zog zg where dp.login = '" & Session("mylogin") & "' and dp.status in ('PA')" _
                & " and zg.nr_zamow = dp.nr_zamow and zg.schemat = dp.schemat" _
                & " ) w left join dp_rest_mag_order o on w.nr_zamow=o.nr_zamow_dt where w.stat_u1 not in('ZW') and (o.shipping_desc not like '%Paczkomaty%' or o.shipping_address not like '%Paczkomat%' or o.shipping_desc is null or o.shipping_address not like '%PaczkoPunkt%')" _
                & " order by w.nr_zamow desc"
            ElseIf Session("mytyp_oper") = "PP" Then
                ''If Session("mylogin") = "a_paczkomat_P" Then
                sqlexp = "select w.nr_zamow,w.schemat,w.nr_zamow_o,w.kontr,w.ile_poz ile_700,w.data_dr,w.data_mg,w.data_pak,w.etykieta_id,w.mag,w.stat_u1 from (" _
                & " select distinct dm.nr_zamow,dm.schemat,zg.nr_zamow_o," _
                & " (select count(*) from dp_swm_mia_mag where nr_zamow = dm.nr_zamow and schemat = dm.schemat and status in ('PA') and mag = 700) ile_poz," _
                & " (select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_mia_oper do where do.status = dm.nr_zamow and do.typ_oper like '%['||dm.schemat||']%' and do.typ_oper like 'MG%') data_mg," _
                & " (select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_mia_oper do where do.status = dm.nr_zamow and do.typ_oper like '%['||dm.schemat||']%' and do.typ_oper like 'PA%') data_pak," _
                & " (select min(to_char(db.autodata,'YYYY/MM/DD')) from dp_swm_mia_buf db where db.status in('DR','RP') and db.nr_zamow=dm.nr_zamow) data_dr," _
                & " zg.autodata data_mag, 'P' typ_oper,'' etykieta_id, dm.mag,zg.stat_u1,case when zg.schemat='DAJAR' then (select kod_kontr from ht_zog where recno=zg.recno) else (select kod_kontr from ht_zog where recno=zg.recno) end kontr" _
                & " from dp_swm_mia_mag dm, dp_swm_mia_zog zg where dm.status in ('PA') and zg.nr_zamow not in(select distinct nr_zamow from dp_swm_mia_pak where nr_zamow = dm.nr_zamow and schemat = dm.schemat)" _
                & " and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat and dm.mag = 700" _
                & " union all" _
                & " select distinct dp.nr_zamow,dp.schemat,zg.nr_zamow_o," _
                & " (select count(*) from dp_swm_mia_mag where nr_zamow = dp.nr_zamow and status in ('PA') and mag=700) ile_poz," _
                & " (select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_mia_oper do where do.status = dp.nr_zamow and do.typ_oper like '%['||dp.schemat||']%' and do.typ_oper like 'MG%') data_mg," _
                & " (select min(to_char(do.autodata,'YYYY/MM/DD')) from dp_swm_mia_oper do where do.status = dp.nr_zamow and do.typ_oper like '%['||dp.schemat||']%' and do.typ_oper like 'PA%') data_pak," _
                & " (select min(to_char(db.autodata,'YYYY/MM/DD')) from dp_swm_mia_buf db where db.status in('DR','RP') and db.nr_zamow=dp.nr_zamow) data_dr," _
                & " zg.autodata data_mag, 'P' typ_oper,(select max(dt.shipment_id) from dp_swm_mia_paczka dt where dt.nr_zamow=dp.nr_zamow and dt.schemat=dp.schemat) etykieta_id,'700',zg.stat_u1,case when zg.schemat='DAJAR' then (select kod_kontr from ht_zog where recno=zg.recno) else (select kod_kontr from ht_zog where recno=zg.recno) end kontr" _
                & " from dp_swm_mia_pak dp, dp_swm_mia_zog zg where dp.login = '" & Session("mylogin") & "' and dp.status in ('PA')" _
                & " and zg.nr_zamow = dp.nr_zamow and zg.schemat = dp.schemat" _
                & " ) w left join dp_rest_mag_order o on w.nr_zamow=o.nr_zamow_dt where w.stat_u1 not in('ZW') and (o.shipping_desc like '%Paczkomaty%' or o.shipping_address like '%Paczkomat%' or o.shipping_address like '%PaczkoPunkt%')" _
                & " order by w.nr_zamow desc"
            End If

            ''                & " ) w left join dp_rest_mag_order o on w.nr_zamow=o.nr_zamow_dt where w.stat_u1 not in('ZW') and o.shipping_desc like '%Paczkomaty%' and o.payment_method <> 'cashondelivery'" _


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

            conn.Close()
        End Using
    End Sub

    Protected Function GetIp() As String
        Dim buf As String = "[REMOTE_ADDR]=" & System.Web.HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
        Return buf
    End Function

    Protected Sub BDrukowaniePDF_INPOST_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BDrukowaniePDF_INPOST.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If HLEtykietaPDF_INPOST.Text <> "" Then
                ''LocalIP.Text = GetIp()

                WebClientPrint.LicenseOwner = "Dajar Sp. z o.o. - 1 WebApp Lic - 1 WebServer Lic"
                WebClientPrint.LicenseKey = "273D8F1AB12B42847C2E9FE7717B6756A2CA0026"

                Application.Lock()
                Application("session_id_" + Session.SessionID) = Session.SessionID.ToString
                Application("shipment_id_" + Session.SessionID) = Session("shipment_id").ToString
                Application("tracking_number_" + Session.SessionID) = Session("shipment_id").ToString
                Application("kod_dyspo_" + Session.SessionID) = Session("kod_dyspo").ToString
                Application("firma_id_" + Session.SessionID) = Session("firma_id").ToString
                Application("mylogin_" + Session.SessionID) = Session("mylogin").ToString
                Application("myhash_" + Session.SessionID) = Session("myhash").ToString
                Application.UnLock()

                dajarSWMagazyn_MIA.MyFunction.SetPrintDyspozycja(dajarSWMagazyn_MIA.MyFunction.GetRemoteIp, Session.SessionID.ToString, Session("kod_dyspo").ToString, Session("mylogin").ToString, Session("myhash").ToString, Session("shipment_id").ToString)
                ScriptManager.RegisterStartupScript(Me.Page, Me.GetType(), "script", "javascript:jsWebClientPrint.print('useDefaultPrinter=' + $('#useDefaultPrinter').attr('checked') + '&printerName=' + $('#installedPrinterName').val() + '&sessionId=" + Session.SessionID + "');", True)
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
                    sqlexp = "UPDATE dp_swm_mia_paczka_info SET tracking_number='',SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka_base SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' and comment_f like '%" & Session("kod_dyspo") & "%'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka_ss SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    sqlexp = "UPDATE dp_swm_mia_paczka SET SHIPMENT_ID='" & shipment_id_new & "' WHERE SHIPMENT_ID='" & Session("shipment_id") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    Session("shipment_id") = shipment_id_new
                    LNr_list_przewozowy.Text = shipment_id_new

                    HLEtykietaPDF_INPOST.NavigateUrl = ""
                    HLEtykietaPDF_INPOST.Text = ""
                    inpost_shipment_id = HLEtykietaPDF_INPOST.Text.ToString

                    ''LadujDaneGridViewZamowienia()
                    ''LadujDaneGridViewPrzesylki(Session("kod_dyspo"), Session("schemat_dyspo"))

                End If
            End If
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

            If shipment_grid IsNot Nothing Then
                If shipment_grid.StartsWith("S") Then
                    shipment_grid = "S%" & shipment_grid.Substring(2, shipment_grid.Length - 2)
                End If

                Dim st_shipment_date As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select substr(st_shipment_date,3,2) from dp_swm_mia_paczka where shipment_id='" & shipment_grid & "'", conn)

                If st_shipment_date = "" Then
                    sqlexp = "select pi.paczka_id,pi.firma_id,case when trim(pi.pl_type)='1' then 'paczka' when trim(pi.pl_type)='2' then 'paleta' when trim(pi.pl_type)='3' then 'koperta' when trim(pi.pl_type)='4' then 'paczkomatA' when trim(pi.pl_type)='5' then 'paczkomatB' when trim(pi.pl_type)='6' then 'paczkomatC' when trim(pi.pl_type)='7' then 'paczka_poczta' when trim(pi.pl_type)='9' then 'paczka_zagranica' when trim(pi.pl_type)='10' then 'paczka_cieszyn' when trim(pi.pl_type)='HP' then 'półpaleta [HP]' when trim(pi.pl_type)='EP' then 'bezzwrotna paleta [EP]' when trim(pi.pl_type)='CC' then 'colli [CC]' when trim(pi.pl_type)='FP' then 'europaleta [FP]' when trim(pi.pl_type)='NP' then 'paleta 1.0x2.0 [NP]' when trim(pi.pl_type)='VP' then '1/4 paleta [VP]' when trim(pi.pl_type)='PC' then 'chep [PC]' when trim(pi.pl_type)='DR' then 'przekracza europalete [DR]' end typ," _
                & " case when trim(pi.pl_non_std)='0' then 'standard' when trim(pi.pl_non_std)='20' then 'cargo ekspedycja [20]' when trim(pi.pl_non_std)='21' then 'cargo zamówienia [21]' when trim(pi.pl_non_std)='A' then 'rozmiar 8 x 38 x 64 cm [A]' when trim(pi.pl_non_std)='B' then 'rozmiar 19 x 38 x 64 cm [B]' when trim(pi.pl_non_std)='C' then 'rozmiar 41 x 38 x 64 cm [C]' when trim(pi.pl_non_std)='D' then 'rozmiar 50 x 50 x 80 cm [D]' when trim(pi.pl_non_std)='RECZNIE' then 'rozmiar 1 x 1 x 1 cm [RECZNIE]' else 'niestandard' end rodzaj, " _
                & " pi.pl_weight waga, pi.pl_width szer,pi.pl_height wys,pi.pl_length dlu,pi.pl_quantity ile_opak, pi.pl_euro_ret nstd from" _
                & " dp_swm_mia_paczka_info pi" _
                & " where pi.shipment_id like '" & shipment_grid & "' and pi.schemat='" & Session("schemat_dyspo") & "' order by pi.paczka_id asc"
                Else
                    sqlexp = "select pi.paczka_id,pi.firma_id,case when trim(pi.pl_type)='1' then 'paczka' when trim(pi.pl_type)='2' then 'paleta' when trim(pi.pl_type)='3' then 'koperta' when trim(pi.pl_type)='4' then 'paczkomatA' when trim(pi.pl_type)='5' then 'paczkomatB' when trim(pi.pl_type)='6' then 'paczkomatC' when trim(pi.pl_type)='7' then 'paczka_poczta' when trim(pi.pl_type)='9' then 'paczka_zagranica' when trim(pi.pl_type)='10' then 'paczka_cieszyn' when trim(pi.pl_type)='HP' then 'półpaleta [HP]' when trim(pi.pl_type)='EP' then 'bezzwrotna paleta [EP]' when trim(pi.pl_type)='CC' then 'colli [CC]' when trim(pi.pl_type)='FP' then 'europaleta [FP]' when trim(pi.pl_type)='NP' then 'paleta 1.0x2.0 [NP]' when trim(pi.pl_type)='VP' then '1/4 paleta [VP]' when trim(pi.pl_type)='PC' then 'chep [PC]' when trim(pi.pl_type)='DR' then 'przekracza europalete [DR]' end typ," _
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
            End If
            conn.Close()
        End Using

    End Sub

    Public Sub AktualizujDanePaczka()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim lista_shipment As New List(Of String)
            If GridViewPaczki.Rows.Count > 0 Then
                For Each row As GridViewRow In GridViewPaczki.Rows
                    Dim paczka_id As String = row.Cells(2).Text.ToString
                    Dim shipment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT SHIPMENT_ID FROM dp_swm_mia_paczka_info WHERE PACZKA_ID='" & paczka_id & "'", conn)
                    If lista_shipment.Contains(shipment_id) = False And shipment_id <> "" Then
                        lista_shipment.Add(shipment_id)
                    End If
                Next
            End If

            Dim lista_zamowien As New List(Of ZamowieniaInformacje)
            If GridViewPakowanie.Rows.Count > 0 Then
                For Each row As GridViewRow In GridViewPakowanie.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim schemat As String = row.Cells(3).Text.ToString
                        Dim nr_zamow As String = row.Cells(2).Text.ToString
                        lista_zamowien.Add(New ZamowieniaInformacje(nr_zamow, schemat))
                    End If
                Next
            End If

            For Each sh_id In lista_shipment
                For Each zam In lista_zamowien
                    Dim shippment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & zam.nr_zamow & "' AND SCHEMAT='" & zam.schemat & "' AND SHIPMENT_ID='" & sh_id & "'", conn)
                    If shippment_id = "" Then
                        'utworzenie nowej etykiety logistycznej
                        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                        sqlexp = "insert into dp_swm_mia_paczka (nr_zamow,schemat,shipment_id,autodata) values('" & zam.nr_zamow & "','" & zam.schemat & "','" & shippment_id & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If
                Next
            Next
            conn.Close()
        End Using
    End Sub

    Protected Sub BAnulujPaczke_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BAnulujPaczek.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim sel_nr_zamow As String = LNrZamowienia.Text.ToString
            Dim sel_schemat As String = LSchemat.Text.ToString
            Dim paczka_id As String = LPaczkaID.Text.ToString

            If paczka_id.Contains("PA") Then
                ''''sqlexp = "select count(*) from dp_swm_mia_paczka where shipment_id in(select shipment_id from dp_swm_mia_paczka_info where paczka_id='" & paczka_id & "')"
                ''''Dim ile_paczka_info As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                ''''If ile_paczka_info = "0" Then
                ''''    sqlexp = "delete from dp_swm_mia_paczka where shipment_id in (select shipment_id from dp_swm_mia_paczka_info where paczka_id='" & paczka_id & "')"
                ''''    dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                ''''End If

                ''##2023.01.25 / modyfikacja warunku na usuwanie paczek[zawsze glowki dp_swm_mia_paczka] / w warunku gdy mamy wiecej niz 1 dp_swm_mia_paczka
                sqlexp = "select count(*) from dp_swm_mia_paczka where nr_zamow='" & sel_nr_zamow & "'"
                Dim ile_paczka_info As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If ile_paczka_info > "1" Then
                    sqlexp = "delete from dp_swm_mia_paczka where shipment_id in (select shipment_id from dp_swm_mia_paczka_info where paczka_id='" & paczka_id & "')"
                    dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                End If

                sqlexp = "delete from dp_swm_mia_paczka_info where paczka_id='" & paczka_id & "'"
                dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                sqlexp = "select count(*) from dp_swm_mia_paczka_info where shipment_id in(select shipment_id from dp_swm_mia_paczka where nr_zamow='" & sel_nr_zamow & "')"
                Dim ile_paczek As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If ile_paczek = "0" Then
                    sqlexp = "delete from dp_swm_mia_paczka where shipment_id in (select shipment_id from dp_swm_mia_paczka where nr_zamow='" & sel_nr_zamow & "')"
                    dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                End If

                sqlexp = "delete from dp_swm_mia_paczka_base where paczka_id='" & paczka_id & "'"
                dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                sqlexp = "delete from dp_swm_mia_paczka_ss where paczka_id='" & paczka_id & "'"
                dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                sqlexp = "select count(*) from dp_swm_mia_paczka where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "'"
                Dim ilePaczekWZamowieniu As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If ilePaczekWZamowieniu = "0" Then
                    ''warunek usuwanie wpisu z dp_swm_mia_pak jezeli nie ma przypisanej zadnej paczki
                    sqlexp = "delete from dp_swm_mia_pak where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "' and etykieta_id='X'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                End If

                ''##2023.01.24 / aktualizacja pozostalego shipment_id 
                Dim shipment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT max(SHIPMENT_ID) FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                Session("shipment_id") = shipment_id
                LNr_list_przewozowy.Text = Session("shipment_id")

                CzyszczenieDanychSzczegolowychEtykieta()
                GenerujInformacjeEtykieta(sel_nr_zamow)
                ''##2023.01.25 / wylaczenie dodawania nowego rekordu dla pakowania dp_swm_mia_paczka
                ''''AktualizujDanePaczka()
                LadujDaneGridViewPaczka()
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BZakonczPakowanie_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BZakonczPakowanie.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("kod_dyspo") IsNot Nothing And Session("schemat_dyspo") IsNot Nothing Then
                ''''Session("etykieta_dyspo") = True
                If Session("etykieta_dyspo") IsNot Nothing And Session("etykieta_dyspo") = True Then
                    Dim nr_zamow As String = Session("kod_dyspo").ToString
                    Dim schemat As String = Session("schemat_dyspo").ToString
                    Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                    sqlexp = "update dp_swm_mia_pak set status = 'PE', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and login = '" & Session("mylogin") & "' and schemat='" & schemat & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    sqlexp = "update dp_swm_mia_mag set status = 'PE', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and schemat='" & schemat & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PE" & "[" & schemat & "][" & Session("contentMagazyn") & "]", nr_zamow)
                    dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(nr_zamow, schemat, "PE")
                    Session("etykieta_dyspo") = False

                    GenerujInformacjeEtykieta_DAJAR(nr_zamow, schemat)
                    CzyszczenieInformacjiEtykieta()

                    Session.Remove("kod_dyspo")
                    Session.Remove("schemat_dyspo")
                    Session.Remove("contentMagazynCel")
                    Session.Remove("firma_id")
                    Session.Remove("shipment_id")
                    Session.Remove("mm_sel_index")
                Else
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Nie mozna zakonczyc pakowania - co najmniej jedno wybrane zamowienie nie posiada przypisanej etykiety!<br />"
                    Session(session_id) += "</div>"
                End If
            End If

            Dim listaWybranych As List(Of ObiektZaznaczenie) = PobierzGridViewListaZamowienZaznaczone()
            For Each element In listaWybranych
                Dim nr_zamow As String = element.nr_zamow
                Dim schemat As String = element.schemat
                Dim mag_cel As String = element.mag_cel

                'domykanie zamowien / odblokowywanie zadan
                If Session("contentMagazyn") <> mag_cel Then
                    sqlexp = "update dp_swm_mia_mag set status='PP' where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and mag = " & Session("contentMagazyn")
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PP" & "[" & schemat & "][" & Session("contentMagazyn") & "]", nr_zamow)

                    If Session("contentMagazyn") = 700 Then
                        '#######SMS DLA ZADAN ODBLOKOWANYCH DLA OPERATOROW########
                        Dim operator_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct login from dp_swm_mia_mag where nr_zamow='" & nr_zamow & "' and mag=" & mag_cel & " and status in('HB')", conn)
                        Dim operator_hash As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct hash from dp_swm_mia_mag where nr_zamow='" & nr_zamow & "' and mag=" & mag_cel & " and status in('HB')", conn)
                        Dim telefon As String = MyFunction.GetStringFromSqlExp("SELECT TELEFON FROM dp_swm_mia_UZYT WHERE LOGIN='" & operator_id & "'", conn)
                        Dim message As String = MyFunction.DataEvalSmsTime & vbNewLine & "dajarSWMagazyn_MIA " & vbNewLine & "wznowione zad. dla op. " & operator_id & " / mag. " & mag_cel & " / stat.zad. HB" & vbNewLine
                        If schemat = "DAJAR" Then message &= "zam. dajar : " & nr_zamow.ToString & vbNewLine
                        If schemat = "DOMINUS" Then message &= "zam. dominus : " & nr_zamow.ToString & vbNewLine
                        dajarSWMagazyn_MIA.MyFunction.SetSmsInformationStatus(operator_id, operator_hash, telefon, message)

                        sqlexp = "update dp_swm_mia_mag set status='MG' where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and mag = " & mag_cel & " and status in('HB')"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "HB/MG" & "[" & schemat & "][" & mag_cel & "]", nr_zamow)
                    ElseIf Session("contentMagazyn") = 46 Then
                        '#######SMS DLA ZADAN ODBLOKOWANYCH DLA OPERATOROW########
                        Dim operator_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct login from dp_swm_mia_mag where nr_zamow='" & nr_zamow & "' and mag=" & mag_cel & " and status in('BB')", conn)
                        Dim operator_hash As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct hash from dp_swm_mia_mag where nr_zamow='" & nr_zamow & "' and mag=" & mag_cel & " and status in('BB')", conn)
                        Dim telefon As String = MyFunction.GetStringFromSqlExp("SELECT TELEFON FROM dp_swm_mia_UZYT WHERE LOGIN='" & operator_id & "'", conn)
                        Dim message As String = MyFunction.DataEvalSmsTime & vbNewLine & "dajarSWMagazyn_MIA " & vbNewLine & "wznowione zad. dla op. " & operator_id & " / mag. " & mag_cel & " / stat.zad. BB" & vbNewLine
                        If schemat = "DAJAR" Then message &= "zam. dajar : " & nr_zamow.ToString & vbNewLine
                        If schemat = "DOMINUS" Then message &= "zam. dominus : " & nr_zamow.ToString & vbNewLine
                        dajarSWMagazyn_MIA.MyFunction.SetSmsInformationStatus(operator_id, operator_hash, telefon, message)

                        sqlexp = "update dp_swm_mia_mag set status='MG' where nr_zamow = '" & nr_zamow & "' and schemat = '" & schemat & "' and mag = " & mag_cel & " and status in('BB')"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "BB/MG" & "[" & schemat & "][" & mag_cel & "]", nr_zamow)
                    End If

                    dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(nr_zamow, schemat, "MG")
                    element.aktywny = False

                    'zamykanie na tym samym magazynie / sprawdzenie przypisanej paczki
                Else
                    If (element.etykieta = "" Or element.etykieta = "&nbsp;") Then
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Nie mozna zakonczyc pakowania - co najmniej jedno wybrane zamowienie nie posiada przypisanej etykiety!<br />"
                        Session(session_id) += "</div>"
                    Else
                        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                        sqlexp = "update dp_swm_mia_pak set status = 'PE', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and login = '" & Session("mylogin") & "' and schemat='" & schemat & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        sqlexp = "update dp_swm_mia_mag set status = 'PE', autodata_zak = TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS') where nr_zamow = '" & nr_zamow & "' and schemat='" & schemat & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PE" & "[" & schemat & "][" & Session("contentMagazyn") & "]", nr_zamow)
                        dajarSWMagazyn_MIA.MyFunction.UpdateDyspozycjaHT_ZOG(nr_zamow, schemat, "PE")
                        element.aktywny = False

                        GenerujInformacjeEtykieta_DAJAR(nr_zamow, schemat)
                        CzyszczenieInformacjiEtykieta()

                        Session.Remove("kod_dyspo")
                        Session.Remove("schemat_dyspo")
                        Session.Remove("contentMagazynCel")
                        Session.Remove("firma_id")
                        Session.Remove("shipment_id")
                        Session.Remove("mm_sel_index")
                        ''GENEROWANIE AUTOMATYCZNYCH LISTOW PRZEWOZOWYCH DHL / STAT_U2
                        ''If LSTAT_U2.Text = "A1" Or LSTAT_U2.Text = "A2" Then
                        ''End If
                    End If
                End If
            Next

            If Session(session_id) Is Nothing Then
                Response.Redirect(Request.RawUrl)
            End If
            conn.Close()
        End Using
    End Sub

    Public Sub GenerujInformacjeEtykieta(ByVal nr_zamow As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim etykietaInput As String = nr_zamow & Date.Now.ToString
            LPaczkaID.Text = getMd5Hash(etykietaInput)
            Session("paczka_dyspo") = LPaczkaID.Text.ToString
            conn.Close()
        End Using
    End Sub

    Public Function PobierzGridViewListaZamowienZaznaczone() As List(Of ObiektZaznaczenie)
        Dim listaWybranych As New List(Of ObiektZaznaczenie)
        If GridViewPakowanie.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewPakowanie.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                If cb IsNot Nothing And cb.Checked Then
                    ''        Public Sub New(ByVal _nr_zamow As String, ByVal _schemat As String, ByVal _kod_kontr As String, ByVal _ile_poz As String, ByVal _etykieta As String)
                    Dim obiektZaznaczenie As New ObiektZaznaczenie(row.Cells(2).Text.ToString, row.Cells(3).Text.ToString, row.Cells(5).Text.ToString, row.Cells(6).Text.ToString, row.Cells(10).Text.ToString)
                    listaWybranych.Add(obiektZaznaczenie)
                End If
            Next
        End If
        Return listaWybranych
    End Function

    Public Function PobierzGridViewListaPaczekZaznaczone() As List(Of ObiektZaznaczeniePaczka)
        Dim listaWybranych As New List(Of ObiektZaznaczeniePaczka)
        If GridViewPaczki.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewPaczki.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                If cb IsNot Nothing And cb.Checked Then
                    Dim obiektZaznaczenie As New ObiektZaznaczeniePaczka(row.Cells(2).Text.ToString, row.Cells(3).Text.ToString, row.Cells(4).Text.ToString)
                    listaWybranych.Add(obiektZaznaczenie)
                End If
            Next
        End If
        Return listaWybranych
    End Function


    Protected Sub GridViewPakowanie_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewPakowanie.RowDataBound
        ''AktualizujGridViewPakowanieStatusy()
        If e.Row.RowType = DataControlRowType.DataRow Then

            Dim rowIndex As Integer = e.Row.RowIndex
            e.Row.Attributes("onclick") = Page.ClientScript.GetPostBackClientHyperlink(GridViewPakowanie, "Select$" & rowIndex)
            e.Row.Style("cursor") = "pointer"


            Dim nr_zamow As String = e.Row.Cells(2).Text.ToString
            Dim cb As CheckBox = e.Row.FindControl("CBKodSelect")
            If cb IsNot Nothing And cb.Checked Then
                e.Row.BackColor = GridViewPakowanie.SelectedRowStyle.BackColor
            Else
                Dim schemat As String = e.Row.Cells(3).Text.ToString
                If schemat = "DAJAR" Then : e.Row.BackColor = Ldajar.BackColor
                ElseIf schemat = "DOMINUS" Then : e.Row.BackColor = Ldominus.BackColor
                End If

                '                Dim etykieta_id As String = e.Row.Cells(10).Text.ToString
                '                If etykieta_id <> "" And etykieta_id <> "&nbsp;" Then
                '                    e.Row.BackColor = LPodjete.BackColor
                '                End If
            End If

            Dim ile_poz As String = e.Row.Cells(6).Text.ToString
            ''Dim ile_obcy As String = e.Row.Cells(7).Text.ToString
            Dim ile_obcy As String = ile_poz
            ''Dim mag_cel As String = e.Row.Cells(13).Text.ToString
            ''####2022.10.11 / ustawienie mag_cel=43
            Dim mag_cel As String = "700"

            ''magazyn bierzacy = magazyn docelowy
            ''If Session("contentMagazyn") = mag_cel Then
            ''    If ile_obcy > 0 Then
            ''        e.Row.Cells(7).BackColor = LPakowanieWyslanoMagazyn.BackColor
            ''    End If
            ''    ''roznica na magaznie bierzacym/docelowym
            ''Else
            ''    If ile_poz > 0 Then
            ''        e.Row.Cells(6).BackColor = LPakowanieNaObcym.BackColor
            ''    End If
            ''End If
        End If
    End Sub

    Protected Sub GridViewPakowanie_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles GridViewPakowanie.RowCommand
        If e.CommandName = "Select" Then
            Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
            GridViewPakowanie.SelectedIndex = rowIndex
            For Each row As GridViewRow In GridViewPakowanie.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                cb.Checked = False
            Next
            GridViewPakowanie_SelectedIndexChanged(sender, EventArgs.Empty)
        End If
    End Sub

    Public Function WyszukajNumerEtykiety(ByVal lista As List(Of ObiektZaznaczenie)) As String
        Dim etykieta_id_bierzaca As String = ""
        For Each el In lista
            If el.etykieta.Contains("ET") Then
                etykieta_id_bierzaca = el.etykieta
                Exit For
            End If
        Next

        Return etykieta_id_bierzaca
    End Function

    '    Protected Sub BOdznaczWszystkie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BOdznaczWszystkie.Click
    '        If GridViewPakowanie.Rows.Count > 0 Then
    '            For Each row As GridViewRow In GridViewPakowanie.Rows
    '                Dim cb As CheckBox = row.FindControl("CBKodSelect")
    '                cb.Checked = False
    '            Next
    '        End If
    '
    '        CzyszczenieInformacjiEtykieta()
    '        Response.Redirect(Request.RawUrl)
    '    End Sub

    '    Protected Sub BZaznaczWszystkie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BZaznaczWszystkie.Click
    '        If GridViewPakowanie.Rows.Count > 0 Then
    '            For Each row As GridViewRow In GridViewPakowanie.Rows
    '                Dim cb As CheckBox = row.FindControl("CBKodSelect")
    '                cb.Checked = True
    '            Next
    '        End If
    '
    '        LNrZamowienia.Text = ""
    '        LSchemat.Text = ""
    '        ''LSTAT_U2.Text = ""
    '        Dim listaWybranych As List(Of ObiektZaznaczenie) = PobierzGridViewListaZamowienZaznaczone()
    '        For Each elem In listaWybranych
    '            LNrZamowienia.Text = elem.nr_zamow.ToString
    '            LSchemat.Text = elem.schemat.ToString
    '        Next
    '    End Sub

    Public Sub CzyszczenieInformacjiEtykieta()
        LNrZamowienia.Text = ""
        LSchemat.Text = ""
        LPaczkaID.Text = ""
        GridViewPaczki.DataSource = Nothing
        TBWaga.Text = "1"
        TBWysINPOST_ALLEGRO.Text = "1"
        TBSzerINPOST_ALLEGRO.Text = "1"
        TBDlugINPOST_ALLEGRO.Text = "1"
        DDLRodzajINPOST_ALLEGRO.SelectedValue = "A"
        ''LSTAT_U2.Text = ""
    End Sub

    Public Sub CzyszczenieDanychSzczegolowychEtykieta()
        LPaczkaID.Text = ""
        TBWaga.Text = "1"
        ''LSTAT_U2.Text = ""
    End Sub

    Public Sub WalidacjaInformacjiEtykieta()
        If Session("firma_id") = "STD" Then
        ElseIf Session("firma_id") = "DHL" Then
        Else
        End If
        ''TBWaga.Text = TBWaga.Text.Replace(".", ",")
    End Sub

    Protected Sub BDodajPaczke_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BDodajPaczke.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim weryfikacja_kontr As Boolean = True
            Dim kontrahent_hist As String = ""
            Dim weryfikacja_schemat As Boolean = True
            Dim schemat_hist As String = ""
            Dim weryfikacja_shipment As Boolean = True
            Dim shipment_hist As String = ""
            Dim lista_zamowien As New List(Of ZamowieniaInformacje)

            If Session("kod_dyspo") IsNot Nothing And Session("schemat_dyspo") IsNot Nothing Then
                lista_zamowien.Add(New ZamowieniaInformacje(Session("kod_dyspo").ToString, Session("schemat_dyspo").ToString))
            End If

            If GridViewPakowanie.Rows.Count > 0 Then
                For Each row As GridViewRow In GridViewPakowanie.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim nr_zamow As String = row.Cells(2).Text.ToString
                        Dim schemat As String = row.Cells(3).Text.ToString
                        Dim kontr As String = row.Cells(5).Text.ToString
                        If kontr <> kontrahent_hist And kontrahent_hist <> "" Then : weryfikacja_kontr = False
                        ElseIf kontrahent_hist = "" Then : kontrahent_hist = kontr
                        End If

                        If schemat <> schemat_hist And schemat_hist <> "" Then : weryfikacja_schemat = False
                        ElseIf schemat_hist = "" Then : schemat_hist = schemat
                        End If

                        Dim shipment As String = row.Cells(10).Text.ToString
                        If shipment <> shipment_hist And shipment_hist <> "" Then : weryfikacja_shipment = False
                        ElseIf shipment_hist = "" Then : shipment_hist = shipment
                        End If

                        lista_zamowien.Add(New ZamowieniaInformacje(nr_zamow, schemat))
                    End If
                Next
            End If

            If weryfikacja_kontr = False Then
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Wybrane zamowienia sa wprowadzone na roznych kontrahentow!<br />"
                Session(session_id) += "</div>"
            ElseIf weryfikacja_schemat = False Then
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Wybrane zamowienia sa wprowadzone na roznych schematach!<br />"
                Session(session_id) += "</div>"
            ElseIf weryfikacja_shipment = False Then
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Wybrane zamowienia maja przypisane rozne etykiety pakowania!<br />"
                Session(session_id) += "</div>"
            ElseIf Session("firma_id") Is Nothing Or Session("firma_id") = "" Then
                Dim sel_paczka_id As String = LPaczkaID.Text
                ''wariant kiedy wprowadzamy nowa paczke 
                If sel_paczka_id.Contains("PA") = False Then
                    WalidacjaInformacjiEtykieta()
                    If (IsNumeric(DirectCast(FindControl("TBWaga"), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBIlePaczek"), TextBox).Text) And DDLTyp.SelectedValue <= "2") Or DDLTyp.SelectedValue > "2" Then
                        Dim typ As String = DirectCast(FindControl("DDLTyp"), DropDownList).SelectedValue.ToString
                        Dim rodzaj As String = DirectCast(FindControl("DDLRodzajStandard"), DropDownList).SelectedValue.ToString
                        Dim paleta_zwrot As String = "0"

                        Dim waga As String = DirectCast(FindControl("TBWaga"), TextBox).Text.ToString
                        If waga = "" Then waga = "0"
                        Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek"), TextBox).Text.ToString
                        Dim firma_id As String = ""

                        Dim paczka_id As String = dajarSWMagazyn_MIA.MyFunction.GeneratePaczkaId
                        Dim shippment_id As String = ""
                        Dim shippment_id_pakiet As String = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                        For Each zamowienie In lista_zamowien
                            Dim sel_nr_zamow As String = zamowienie.nr_zamow.ToString
                            Dim sel_schemat As String = zamowienie.schemat.ToString

                            remoteApp.LoadProcessDetails()
                            If remoteApp.domainUserName <> "" Then
                                sqlexp = "update dp_swm_mia_waga set hash='" & Session("myhash") & "' where domain_user='" & remoteApp.domainUserName & "' and domain_ip='" & remoteApp.domainIpAdress & "' and nr_zamow is null"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                            End If

                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "select nr_zamow from dp_swm_mia_pak where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "' and etykieta_id='X'"
                            Dim czyRekordPakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                            If czyRekordPakowanie = "" Then
                                Dim ile_pakowanie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select distinct ile_poz from dp_swm_mia_buf where nr_zamow='" & sel_nr_zamow & "' and schemat='" & sel_schemat & "'", conn)
                                sqlexp = "insert into dp_swm_mia_pak (login,hash,nr_zamow,autodata,status,schemat,mag,etykieta_id,ile_poz) values('" & Session("mylogin") & "','" & Session("myhash") & "','" & sel_nr_zamow & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'),'PA','" & sel_schemat & "','" & Session("contentMagazyn") & "','X','" & ile_pakowanie & "')"
                                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                                ''wylaczenie opcji ustawiania statusu PA dla operatorow tworzacych paczki
                                ''result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "PA" & "[" & sel_schemat & "][" & Session("contentMagazyn") & "]", sel_nr_zamow)
                                For Each row As GridViewRow In GridViewPakowanie.Rows
                                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                                    If cb IsNot Nothing And cb.Checked Then
                                        row.Cells(10).Text = "X"
                                    End If
                                Next
                                Session("etykieta_dyspo") = True
                            Else
                                Session("etykieta_dyspo") = True
                            End If

                            shippment_id = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & sel_nr_zamow & "' AND SCHEMAT='" & sel_schemat & "'", conn)
                            If shippment_id = "" Then
                                shippment_id = shippment_id_pakiet
                                'utworzenie nowej etykiety logistycznej
                                sqlexp = "insert into dp_swm_mia_paczka (nr_zamow,schemat,shipment_id,autodata) values('" & sel_nr_zamow & "','" & sel_schemat & "','" & shippment_id & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                                sqlexpTrans.Add(sqlexp)
                                ''result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                                ''GridViewPakowanie.SelectedRow.Cells(10).Text = shippment_id
                                For Each row As GridViewRow In GridViewPakowanie.Rows
                                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                                    If cb IsNot Nothing And cb.Checked Then
                                        row.Cells(10).Text = shippment_id
                                    End If
                                Next

                            End If
                        Next

                        Session("firma_id") = ""
                        Session("shipment_id") = shippment_id

                        sqlexp = "insert into dp_swm_mia_paczka_info (schemat,shipment_id,paczka_id,firma_id,pl_type,pl_weight,pl_width,pl_height,pl_length,pl_quantity,pl_non_std,pl_euro_ret) values('" & Session("schemat_dyspo").ToString & "','" & shippment_id & "','" & paczka_id & "','" & Session("firma_id") & "','" & typ & "'," & waga & ",1,1,1," & ile_paczek & ",'" & rodzaj & "','" & paleta_zwrot & "')"
                        sqlexpTrans.Add(sqlexp)
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExpTransaction(sqlexpTrans, conn)
                        ''result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                        Session(session_id) += "</div>"
                    End If

                    'sytuacja kiedy modyfikujemy wczesniej wprowadzona paczke
                Else
                    Dim paczka_id As String = sel_paczka_id
                    WalidacjaInformacjiEtykieta()
                    If (IsNumeric(DirectCast(FindControl("TBWaga"), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBIlePaczek"), TextBox).Text) And DDLTyp.SelectedValue <= "2") Or DDLTyp.SelectedValue > "2" Then
                        Dim typ As String = DirectCast(FindControl("DDLTyp"), DropDownList).SelectedValue.ToString
                        Dim rodzaj As String = DirectCast(FindControl("DDLRodzajStandard"), DropDownList).SelectedValue.ToString
                        Dim paleta_zwrot As String = "0"

                        Dim waga As String = DirectCast(FindControl("TBWaga"), TextBox).Text.ToString
                        If waga = "" Then waga = "0"
                        Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek"), TextBox).Text.ToString
                        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                        sqlexp = "update dp_swm_mia_paczka_info set pl_type='" & typ & "', pl_non_std='" & rodzaj & "', pl_weight=" & waga & ",pl_quantity=" & ile_paczek & " where paczka_id = '" & paczka_id & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                        Session(session_id) += "</div>"
                    End If
                End If

                If Session(session_id) Is Nothing Then
                    Dim kiedy_ten_warunek As Boolean = True
                    ''cccccccccccccccccccc

                    GenerujInformacjeEtykieta("")
                    ''##2023.01.20 / wylaczenie czyszczenie wagi i numeru paczki_id
                    ''CzyszczenieDanychSzczegolowychEtykieta()
                    ''AktualizujDanePaczka()
                    LadujDaneGridViewPaczka()
                End If
            ElseIf Session("firma_id").ToString.Contains("INPOST") Then
                Dim sel_nr_zamow As String = LNrZamowienia.Text
                Dim sel_schemat As String = LSchemat.Text
                Dim sel_paczka_id As String = LPaczkaID.Text
                ''wariant kiedy wprowadzamy nowa paczke 
                If sel_paczka_id.Contains("PA") = False Then
                    WalidacjaInformacjiEtykieta()

                    If (IsNumeric(DirectCast(FindControl("TBWaga"), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text) And (DirectCast(FindControl("DDLTyp"), DropDownList).SelectedValue = "1")) Then
                        Dim typ As String = DirectCast(FindControl("DDLTyp"), DropDownList).SelectedValue.ToString
                        Dim rodzaj As String = DirectCast(FindControl("DDLRodzaj" & Session("firma_id")), DropDownList).SelectedValue.ToString
                        Dim paleta_zwrot As String = DirectCast(FindControl("DDLRodzajStandard"), DropDownList).SelectedValue.ToString

                        Dim waga As String = DirectCast(FindControl("TBWaga"), TextBox).Text.ToString
                        If waga = "" Then waga = "0"
                        Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                        If szer = "" Then szer = "0"
                        Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                        If wys = "" Then wys = "0"
                        Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString
                        If dlug = "" Then dlug = "0"
                        Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek"), TextBox).Text.ToString

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

                            ''##2023.01.20 / oznaczanie spakowanych zamowien paczkomaty inpost
                            For Each row As GridViewRow In GridViewPakowanie.Rows
                                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                                If cb IsNot Nothing And cb.Checked Then
                                    row.Cells(10).Text = "X"
                                End If
                            Next
                        End If

                        Dim shippment_id As String = Session("shipment_id")
                        sqlexp = "select count(*) from dp_swm_mia_paczka where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "'"
                        Dim ile_paczek_obj As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        If ile_paczek_obj >= 1 Then
                            shippment_id = "S" & ile_paczek_obj & shippment_id.Substring(2, shippment_id.Length - 2)
                        Else
                            shippment_id = dajarSWMagazyn_MIA.MyFunction.GenerateShipmentId
                        End If

                        'utworzenie nowej etykiety logistycznej
                        sqlexp = "insert into dp_swm_mia_paczka (nr_zamow,schemat,shipment_id,autodata) values('" & sel_nr_zamow & "','" & sel_schemat & "','" & shippment_id & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        ''##2023.01.20 / oznaczanie spakowanych zamowien paczkomaty inpost
                        For Each row As GridViewRow In GridViewPakowanie.Rows
                            Dim cb As CheckBox = row.FindControl("CBKodSelect")
                            If cb IsNot Nothing And cb.Checked Then
                                row.Cells(10).Text = shippment_id
                            End If
                        Next

                        sqlexp = "insert into dp_swm_mia_paczka_info (schemat,shipment_id,paczka_id,firma_id,pl_type,pl_weight,pl_width,pl_height,pl_length,pl_quantity,pl_non_std,pl_euro_ret) values('" & sel_schemat & "','" & shippment_id & "','" & paczka_id & "','" & Session("firma_id") & "','" & typ & "'," & waga & "," & szer & "," & wys & "," & dlug & "," & ile_paczek & ",'" & rodzaj & "','" & paleta_zwrot & "')"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        Session("shipment_id") = shippment_id
                        LPaczkaID.Text = paczka_id
                        CzyszczenieDanychSzczegolowychEtykieta()
                        GenerujInformacjeEtykieta(sel_nr_zamow)
                        LadujDaneGridViewPaczka()
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                        Session(session_id) += "</div>"
                    End If

                    'sytuacja kiedy modyfikujemy wczesniej wprowadzona paczke
                Else
                    Dim paczka_id As String = sel_paczka_id
                    WalidacjaInformacjiEtykieta()
                    ''If (IsNumeric(DirectCast(FindControl("TBWaga"), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text) And (DirectCast(FindControl("DDLTyp"), DropDownList).SelectedValue = "1")) Then
                    If (IsNumeric(DirectCast(FindControl("TBWaga"), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text) And IsNumeric(DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text) And (DirectCast(FindControl("DDLTyp"), DropDownList).SelectedValue = "1")) Then
                        Dim typ As String = DirectCast(FindControl("DDLTyp"), DropDownList).SelectedValue.ToString
                        Dim rodzaj As String = DirectCast(FindControl("DDLRodzaj" & Session("firma_id")), DropDownList).SelectedValue.ToString
                        Dim paleta_zwrot As String = DirectCast(FindControl("DDLRodzajStandard"), DropDownList).SelectedValue.ToString

                        Dim waga As String = DirectCast(FindControl("TBWaga"), TextBox).Text.ToString
                        If waga = "" Then waga = "0"
                        Dim szer As String = DirectCast(FindControl("TBSzer" & Session("firma_id")), TextBox).Text.ToString
                        If szer = "" Then szer = "0"
                        Dim wys As String = DirectCast(FindControl("TBWys" & Session("firma_id")), TextBox).Text.ToString
                        If wys = "" Then wys = "0"
                        Dim dlug As String = DirectCast(FindControl("TBDlug" & Session("firma_id")), TextBox).Text.ToString
                        If dlug = "" Then dlug = "0"
                        Dim ile_paczek As String = DirectCast(FindControl("TBIlePaczek"), TextBox).Text.ToString

                        Dim firma_id As String = DirectCast(FindControl("DDLFirma"), DropDownList).SelectedValue.ToString

                        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                        sqlexp = "update dp_swm_mia_paczka_info set firma_id='" & firma_id & "',pl_type='" & typ & "', pl_non_std='" & rodzaj & "', pl_euro_ret='" & paleta_zwrot & "', pl_weight=" & waga & ",pl_width=" & szer & ",pl_height=" & wys & ",pl_length=" & dlug & ",pl_quantity=" & ile_paczek & " where paczka_id = '" & paczka_id & "' and schemat='" & Session("schemat_dyspo") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        ''CzyszczenieDanychSzczegolowychEtykieta()
                        LadujDaneGridViewPaczka()
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Wypelnij poprawne dane logistyczne przed zapisaniem paczki do zamówienia!<br />"
                        Session(session_id) += "</div>"
                    End If
                End If

                ''##2023.01.15 / zapisywanie danych podstawowych paczki tylko dla INPOST
                sqlexp = "SELECT DISTINCT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                Dim cmd As New OracleCommand(sqlexp, conn)
                cmd.CommandType = CommandType.Text
                Dim dr As OracleDataReader = cmd.ExecuteReader()
                Try
                    While dr.Read()
                        Dim shippment_id As String = dr.Item(0).ToString
                        Dim tb_form_dhl() As String = {"B_COSTS_CENTER", "SR_COUNTRY", "ST_SHIPMENT_DATE", "SR_POST_NUM", "SR_NAME", "SR_POSTAL_CODE", "SR_CITY", "SR_STREET", "SR_HOUSE_NUM", "SR_APART_NUM", "PC_PERSON_NAME", "PC_PHONE_NUM", "PC_EMAIL_ADD"}
                        For Each tb_obj In tb_form_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        ''Dim drop_off_value As String = DirectCast(FindControl("DDL_DROP_OFF"), DropDownList).SelectedValue.ToString
                        Dim czyIstniejeBase As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT COUNT(*) FROM dp_swm_mia_paczka_base WHERE SHIPMENT_ID='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)
                        If czyIstniejeBase = "0" Then
                            sqlexp = "insert into dp_swm_mia_paczka_base(SCHEMAT,SHIPMENT_ID,DROP_OFF) values ('" & Session("schemat_dyspo") & "','" & shippment_id & "','REGULAR_PICKUP')"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        End If

                        Dim tb_form_base_dhl() As String = {"COMMENT_F", "SERVICE_TYPE", "LABEL_TYPE"}
                        For Each tb_obj In tb_form_base_dhl
                            Dim tb_value As String = DirectCast(FindControl("TB_" & tb_obj & "_INPOST"), TextBox).Text.ToString
                            sqlexp = "update dp_swm_mia_paczka_base set " & tb_obj & "='" & tb_value & "' where shipment_id='" & shippment_id & "' AND SCHEMAT='" & Session("schemat_dyspo") & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                        Next

                        Dim ddl_form_base_dhl() As String = {"CONTENT"}
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
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub GridViewPakowanie_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewPakowanie.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim session_id As String = "contentKomunikat_" &
                                       dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
            Session.Remove(session_id)

            Dim cb As CheckBox = GridViewPakowanie.SelectedRow.FindControl("CBKodSelect")
            '            If cb IsNot Nothing And cb.Checked Then
            '                cb.Checked = False
            '                CzyszczenieInformacjiEtykieta()
            '                Session.Remove("kod_dyspo")
            '                Session.Remove("schemat_dyspo")
            '                Session.Remove("contentMagazynCel")
            '                Session.Remove("firma_id")
            '                Session.Remove("shipment_id")
            '                ''##2023.01.15 / zerowanie ustawien dla listow przewozowych INPOST
            '                TB_SR_FIRSTNAME_INPOST.Text = ""
            '                TB_SR_LASTNAME_INPOST.Text = ""
            '                TB_SR_POST_NUM.Text = ""
            '                LNr_list_przewozowy.Text = ""
            '                HLEtykietaPDF_INPOST.Text = ""
            '                inpost_shipment_id = HLEtykietaPDF_INPOST.Text.ToString
            '                TBWaga.Text = "1"
            '                TBDlugINPOST_ALLEGRO.Text = "1"
            '                TBSzerINPOST_ALLEGRO.Text = "1"
            '                TBWysINPOST_ALLEGRO.Text = "1"
            '                PanelDodawaniePaczki.Visible = False
            '                WyswietlSzczegolyZamowienia()
            '                GridViewPaczki.DataSource = Nothing
            '            Else
            cb.Checked = True
            Session("kod_dyspo") = GridViewPakowanie.SelectedRow.Cells(2).Text.ToString()
            Session("schemat_dyspo") = GridViewPakowanie.SelectedRow.Cells(3).Text.ToString()

            Session("contentMagazynCel") = "700"
            ''Session("contentMagazynCel") = GridViewPakowanie.SelectedRow.Cells(13).Text.ToString()

            If Session("contentMagazyn") = Session("contentMagazynCel") Then
                PanelDodawaniePaczki.Visible = True
                LNrZamowienia.Text = Session("kod_dyspo")
                LSchemat.Text = Session("schemat_dyspo")

                If LSchemat.Text = "DOMINUS" Then
                    DDLRodzajStandard.SelectedValue = "0"
                ElseIf LSchemat.Text = "DAJAR" Then
                    DDLRodzajStandard.SelectedValue = "0"
                End If

                ''LSTAT_U2.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select stat_u2 from dp_swm_mia_zog where nr_zamow='" & Session("kod_dyspo") & "' and schemat='" & Session("schemat_dyspo") & "'", conn)

                GenerujInformacjeKomentarz(Session("kod_dyspo"), Session("schemat_dyspo"))
                GenerujInformacjeEtykieta(Session("kod_dyspo"))

                If Session("shipment_id") Is Nothing Then
                    ''Dim shipment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT SHIPMENT_ID FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") & "' AND SCHEMAT='" & Session("schemat_dyspo") & "' AND SHIPMENT_ID LIKE 'SH%'", conn)
                    ''##2023.01.15 / pobieranie wszystkich numerow shipment/lacznie z ofertami INPOST
                    Dim shipment_id As String =
                            dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(
                                "SELECT max(SHIPMENT_ID) FROM dp_swm_mia_paczka WHERE NR_ZAMOW='" & Session("kod_dyspo") &
                                "' AND SCHEMAT='" & Session("schemat_dyspo") & "'", conn)

                    If shipment_id <> "" Then
                        Session("shipment_id") = shipment_id
                        LNr_list_przewozowy.Text = Session("shipment_id")
                        If shipment_id.StartsWith("S") = False Then
                            HLEtykietaPDF_INPOST.NavigateUrl = "http://10.1.0.64:8084/upload/inpost/inpost_" &
                                                               Session("shipment_id") & ".pdf"
                            HLEtykietaPDF_INPOST.Text = Session("shipment_id")
                            inpost_shipment_id = HLEtykietaPDF_INPOST.Text.ToString
                        End If
                    Else
                        LNr_list_przewozowy.Text = ""
                    End If
                End If

                ''##2023.01.24 / wylaczenie sekcja z uwagi na page_loaded
                LadujDaneGridViewPaczka()
            Else
                PanelDodawaniePaczki.Visible = False
            End If

            ''##2023.01.24 / wylaczenie sekcja z uwagi na page_loaded
            WyswietlSzczegolyZamowienia()

            If GridViewPaczki.DataSource IsNot Nothing Then
                If GridViewPaczki.Rows.Count > 0 Then
                    Dim paczka_id As String = GridViewPaczki.Rows(0).Cells(2).Text.ToString
                    Dim rodzaj As String = GridViewPaczki.Rows(0).Cells(5).Text.ToString
                    Dim waga As String = GridViewPaczki.Rows(0).Cells(6).Text.ToString
                    Dim szer As String = GridViewPaczki.Rows(0).Cells(7).Text.ToString
                    Dim wys As String = GridViewPaczki.Rows(0).Cells(8).Text.ToString
                    Dim dlu As String = GridViewPaczki.Rows(0).Cells(9).Text.ToString
                    Dim non_standard As String = GridViewPaczki.Rows(0).Cells(11).Text.ToString
                    ''##2023.01.23 / wylaczenie podstawienia ID paczki dla juz utworzonych paczek
                    ''''LPaczkaID.Text = paczka_id
                    If rodzaj.Contains("[") Then
                        Dim rodzaj_t As String = rodzaj.Split("[")(1).ToString.Replace("]", "")
                        If rodzaj_t <> "" Then
                            DDLRodzajINPOST_ALLEGRO.SelectedValue = rodzaj_t
                            TBWaga.Text = waga
                            TBSzerINPOST_ALLEGRO.Text = szer
                            TBWysINPOST_ALLEGRO.Text = wys
                            TBDlugINPOST_ALLEGRO.Text = dlu
                            DDLRodzajStandard.SelectedValue = non_standard
                        End If
                    End If

                End If
            End If
            conn.Close()
        End Using

    End Sub

    Public Sub GenerujInformacjeKomentarz(ByVal obj_nr_zamow As String, ByVal obj_schemat As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            sqlexp = "select kod_kontr from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & obj_nr_zamow & "')"
            Dim kod_kontr As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            sqlexp = "select kod_odb from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & obj_nr_zamow & "')"
            Dim kod_odb As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            sqlexp = "select kod_mie from ht_zog where ie$14 = DESQL_GRAF.DF11_2('" & obj_nr_zamow & "')"
            Dim kod_mie As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

            sqlexp = "select kod_kraju from ht_kontm where kod_kontr = " & kod_kontr & " and kod_odb = '" & kod_odb & "' and kod_mie='" & kod_mie & "' and is_deleted='N'"
            TB_SR_COUNTRY.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

            sqlexp = "select nr_zamow_o from dp_swm_mia_zog where nr_zamow='" & obj_nr_zamow & "' and schemat='" & obj_schemat & "'"
            Dim increment_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            ''Dim increment_id As String = obj_nr_zamow_o
            Dim external_username As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select external_username from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)

            If Session("firma_id") IsNot Nothing Then
                If Session("firma_id") = "INPOST_ALLEGRO" Then
                    ''Dim shipping_desc As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select shipping_desc from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)
                    ''Dim shipping_amount As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select increment_id from dp_rest_mag_order where increment_id='" & increment_id & "'", conn)

                    ''##2023.03.17 / modyfikacja opisu dla paczkomatow
                    ''##2023.09.29 / modyfikacja opisu dla MSPL
                    If increment_id.StartsWith("MSPL") Then
                        TB_COMMENT_F_INPOST.Text = obj_nr_zamow & " " & increment_id
                    ElseIf increment_id.StartsWith("SPL") Then
                        TB_COMMENT_F_INPOST.Text = obj_nr_zamow & " DAJAR"
                    ElseIf Session("schemat_dyspo") = "DOMINUS" Then
                        TB_COMMENT_F_INPOST.Text = "DO " & obj_nr_zamow & " " & external_username
                    Else
                        TB_COMMENT_F_INPOST.Text = obj_nr_zamow & " " & external_username
                    End If

                End If
            Else
                If increment_id.StartsWith("MSPL") Then
                    TB_COMMENT_F_INPOST.Text = obj_nr_zamow & " " & increment_id
                ElseIf increment_id.StartsWith("SPL") Then
                    TB_COMMENT_F_INPOST.Text = obj_nr_zamow & " DAJAR"
                ElseIf Session("schemat_dyspo") = "DOMINUS" Then
                    TB_COMMENT_F_INPOST.Text = "DO " & obj_nr_zamow & " " & external_username
                Else
                    TB_COMMENT_F_INPOST.Text = obj_nr_zamow & " " & external_username
                End If

            End If
            conn.Close()
        End Using
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
            Session.Remove("paczka_dyspo")
            GenerujInformacjeEtykieta(Session("kod_dyspo"))
        Else
            ClearGVPaczki()
            cb.Checked = True
            GridViewPaczki.SelectedIndex = rowid_paczki
            Dim row As GridViewRow = GridViewPaczki.SelectedRow
            ''LNrZamowieniaPaczka.Text = row.Cells(2).Text.ToString
            ''LSchematPaczka.Text = row.Cells(3).Text.ToString

            LPaczkaID.Text = row.Cells(2).Text.ToString
            Session("paczka_dyspo") = LPaczkaID.Text

            '####ZMIANA DOMYSLNEGO TYPU WYBORU PRZEWOZNIKA
            Dim typ_przewoznika As String = row.Cells(4).Text.ToString
            If typ_przewoznika.Contains("paczkomat") Or typ_przewoznika.Contains("paczka_inpost") Then
                Dim ddlRodzajObj As DropDownList = DirectCast(FindControl("DDLRodzaj" & Session("firma_id")), DropDownList)
                If ddlRodzajObj IsNot Nothing Then
                    If Session("firma_id").ToString = "INPOST_ALLEGRO" Then
                        If typ_przewoznika = "paczkomatA" Or typ_przewoznika = "paczka_inpost" Then : ddlRodzajObj.SelectedValue = "A"
                        ElseIf typ_przewoznika = "paczkomatB" Then : ddlRodzajObj.SelectedValue = "B"
                        ElseIf typ_przewoznika = "paczkomatC" Then : ddlRodzajObj.SelectedValue = "C"
                        End If
                    End If
                End If

                If row.Cells(3).Text.ToString = "&nbsp;" Then
                    row.Cells(3).Text = "INPOST_ALLEGRO"
                End If
                ''DDLFirma.SelectedValue = "-1"

                Dim wagaObj As TextBox = DirectCast(FindControl("TBWaga"), TextBox)
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

                Dim ilePaczekObj As TextBox = DirectCast(FindControl("TBIlePaczek"), TextBox)
                If ilePaczekObj IsNot Nothing Then
                    ilePaczekObj.Text = row.Cells(10).Text.ToString
                End If

            Else
                ''wariant archuiwalny obslugiwania zamowienia pakowania zwyklych
                ClearGVPaczki()
                cb.Checked = True
                GridViewPaczki.SelectedIndex = rowid_paczki
                row = GridViewPaczki.SelectedRow
                LPaczkaID.Text = row.Cells(2).Text.ToString
                Session("paczka_dyspo") = LPaczkaID.Text

                Dim typ As String = row.Cells(4).Text.ToString
                Dim ddlTypObj As DropDownList = DirectCast(FindControl("DDLTyp"), DropDownList)
                If typ = "paczka" Then : ddlTypObj.SelectedValue = 1
                ElseIf typ = "paleta" Then : ddlTypObj.SelectedValue = 2
                ElseIf typ = "koperta" Then : ddlTypObj.SelectedValue = 3
                    ''ElseIf typ = "paczkomatA" Then : ddlTypObj.SelectedValue = 4
                    ''ElseIf typ = "paczkomatB" Then : ddlTypObj.SelectedValue = 5
                    ''ElseIf typ = "paczkomatC" Then : ddlTypObj.SelectedValue = 6
                ElseIf typ = "paczka_poczta" Then : ddlTypObj.SelectedValue = 7
                ElseIf typ = "paczka_inpost" Then : ddlTypObj.SelectedValue = 8
                ElseIf typ = "paczka_zagranica" Then : ddlTypObj.SelectedValue = 9
                ElseIf typ = "paczka_cieszyn" Then : ddlTypObj.SelectedValue = 10

                End If

                Dim rodzaj As String = row.Cells(5).Text.ToString
                Dim ddlRodzajObj As DropDownList = DirectCast(FindControl("DDLRodzajStandard"), DropDownList)
                If rodzaj = "standard" Then : ddlRodzajObj.SelectedValue = 0
                Else : ddlRodzajObj.SelectedValue = 1
                End If

                Dim wagaObj As TextBox = DirectCast(FindControl("TBWaga"), TextBox)
                wagaObj.Text = row.Cells(6).Text.ToString

                Dim ilePaczekObj As TextBox = DirectCast(FindControl("TBIlePaczek"), TextBox)
                ilePaczekObj.Text = row.Cells(10).Text.ToString
            End If
        End If
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
                ''##2023.01.24 / dodawanie zamowienia przez czytnik za 1 razem
                list_czytnik.Add(ob_czytnik)
                GenerujInformacjeEtykieta(ob_czytnik.nr_zamow)
            End If
        Else
            list_czytnik.Add(ob_czytnik)
        End If

        Session("mm_sel_index") = list_czytnik
        Return "1"
    End Function

    Protected Sub TBEtykieta_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBEtykieta.TextChanged
        If GridViewPakowanie.Rows.Count > 0 And TBEtykieta.Text.ToString <> "" Then
            DDLRodzajStandard.SelectedValue = "0"
            For Each row As GridViewRow In GridViewPakowanie.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                Dim nr_zamow As String = row.Cells(2).Text.ToString
                Dim schemat As String = row.Cells(3).Text.ToString

                If TBEtykieta.Text <> "" And TBEtykieta.Text.Length = 12 Then
                    ''##2023.01.24 / zerownie ustawienia zmiennej SCHEMAT w przypadku CZYTNIKA
                    Dim ls_schemat As String = ""
                    Dim ls_nr_zamow As String = TBEtykieta.Text.ToString

                    If nr_zamow = ls_nr_zamow Then
                        GridViewPakowanie.SelectedIndex = row.RowIndex
                        Dim ob_czytnik As New ZamowieniaInformacje(ls_nr_zamow, ls_schemat)
                        Aktualizacja_Session_Czytnik(ob_czytnik)
                        ''cb.Checked = True

                        Exit For
                    End If
                End If
            Next
        End If
    End Sub

End Class