Imports dajarSWMagazyn_MIA.HashMd5
Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class login
    Inherits System.Web.UI.Page
    Dim sqlExp As String = ""

    Protected Sub DDLOperator_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLOperator.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim login As String = DDLOperator.SelectedValue.ToString

            If login <> "WYBIERZ OPERATORA" Then
                Session.RemoveAll()
                sqlExp = "select hash from dp_swm_mia_uzyt where login = '" & login & "'"
                Dim hash As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlExp, conn)
                Session("mylogin") = login
                Session("myhash") = hash
                sqlExp = "SELECT trim(TYP_OPER) typ_oper FROM dp_swm_mia_UZYT WHERE LOGIN = '" & login & "'"
                Session("mytyp_oper") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlExp, conn)
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
        Dim userAgent As String = HttpContext.Current.Request.UserAgent.ToLower

        Session("userAgent") = HttpContext.Current.Request.UserAgent.ToLower
        If userAgent IsNot Nothing And (userAgent.Contains("ipad") Or userAgent.Contains("iphone")) Then
            Session("myDevice") = "ipad"
        Else
            Session("myDevice") = "standard"
        End If

        Session("contentKomunikat") = Session(session_id)

        If Not Page.IsPostBack Then
            dajarSWMagazyn_MIA.MyFunction.RefreshDDLOperator(DDLOperator, Session)
            RefreshDDLOperatorLogowanie()
            DDLOperator.SelectedValue = "WYBIERZ OPERATORA"
        End If
    End Sub

    Sub Login_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnZaloguj.Click
        If DDLOperatorLogowanie.Text.ToString <> "" And TBPassword.Text <> "" Then
            CheckLogin(DDLOperatorLogowanie.Text.ToString, TBPassword.Text)
        End If
    End Sub

    Sub RefreshDDLOperatorLogowanie()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim sqlExp As String = "select login from dp_swm_mia_uzyt where ((status = 'X' and mag = 'ALL') or (status is null)) and blokada_konta is null order by login"
            DDLOperatorLogowanie.Items.Clear()
            DDLOperatorLogowanie.Items.Add("")
            Dim cmd As New OracleCommand(sqlExp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            If dr.HasRows Then
                DDLOperatorLogowanie.DataSource = dr
                DDLOperatorLogowanie.DataTextField = "LOGIN"
                DDLOperatorLogowanie.DataValueField = "LOGIN"
                DDLOperatorLogowanie.DataBind()
            End If
            dr.Close()
            cmd.Dispose()
            conn.Close()

        End Using
    End Sub

    Sub CheckLogin(ByVal username As String, ByVal password As String)
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
        Dim result As String = ""

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim param As New List(Of OracleParameter)
            param.Add(New OracleParameter("username", username))
            param.Add(New OracleParameter("password", password))

            sqlExp = "SELECT COUNT(*) FROM dp_swm_mia_UZYT WHERE LOGIN = '" & username & "' AND STATUS='X'"
            result = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlExp, conn)
            sqlExp = "SELECT trim(TYP_OPER) typ_oper FROM dp_swm_mia_UZYT WHERE LOGIN = '" & username & "'"
            Dim typ_oper As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlExp, conn)

            If result <> "0" And typ_oper <> "S" Then
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Użytkownik " & username & " jest już zalogowany w systemie!<br />"
                Session(session_id) += "</div>"
            Else
                sqlExp = "SELECT IMIE||' '||NAZWISKO||'@'||LOGIN FROM dp_swm_mia_UZYT WHERE LOGIN = :username AND PASSWORD = :password"
                result = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExpParameter(sqlExp, conn, param)
                If result <> "" Then
                    Session("mylogin") = DDLOperatorLogowanie.Text.ToString
                    Dim hashInput As String = Session("mylogin") & Date.Now.ToString

                    Dim mag_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN='" & username & "'", conn)
                    If mag_id = "ALL" Then
                        Session("myhash") = getMd5Hash(hashInput)
                        Session.Timeout = 480
                        Session("mysuper_user") = True
                        Session("mytyp_oper") = "S"
                        Session("contentOperator") = "STANOWISKO W STREFIE : " & Session("mytyp_oper")
                        Session("contentMagazyn") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN = '" & username & "'", conn)
                        Session("contentHash") = "SESJA MIASTKO : " & Session("myhash")
                        result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "LOGIN", Session("mytyp_oper"))
                        ''##2024.05.21 / wyswietlanie informacji o wersji testowej aplikacji
                        If conn.ConnectionString.Contains("10.1.0.148") Then
                            Session("contentOperator") = Session("contentOperator") & "_____WERSJA_TESTOWA_PROGRAMU_____"
                        End If

                        Response.Redirect("admin.aspx")
                    End If

                    Session("myhash") = getMd5Hash(hashInput)
                    typ_oper = DDLTypOperatora.SelectedValue.ToString
                    Session.Timeout = 480
                    Session("contentHash") = "SESJA MIASTKO : " & Session("myhash")
                    Session("mytyp_oper") = typ_oper

                    Session("contentOperator") = "STANOWISKO W STREFIE : " & Session("mytyp_oper")
                    Session("contentMagazyn") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN = '" & username & "'", conn)

                    ''##2024.05.21 / wyswietlanie informacji o wersji testowej aplikacji
                    If conn.ConnectionString.Contains("10.1.0.148") Then
                        Session("contentOperator") = Session("contentOperator") & "_____WERSJA_TESTOWA_PROGRAMU_____"
                    End If

                    sqlExp = "select blokada_konta from dp_swm_mia_UZYT where LOGIN = '" & Session("mylogin") & "'"
                    Dim blokada_konta As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlExp, conn)

                    If blokada_konta <> "Y" Then
                        sqlExp = "UPDATE dp_swm_mia_UZYT SET HASH = '" & Session("myhash") & "',TYP_OPER='" & typ_oper.ToUpper & "',STATUS='X' WHERE LOGIN = '" & Session("mylogin") & "'"
                        dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlExp, conn)
                        result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "LOGIN", typ_oper)
                        Dim telefon As String = MyFunction.GetStringFromSqlExp("SELECT TELEFON FROM dp_swm_mia_UZYT WHERE LOGIN='" & Session("mylogin") & "'", conn)
                        Dim message As String = MyFunction.DataEvalSmsTime & vbNewLine & "dajarSWMagazyn_MIA " & vbNewLine & "logowanie uzytkownika : " & Session("mylogin") & " mag. : " & Session("contentMagazyn") & " typ_oper. : " & Session("mytyp_oper") & " sesja : " & Session("myhash")
                        ''''dajarSWMagazyn_MIA.MyFunction.SetSmsInformationStatus(Session("mylogin"), Session("myhash"), telefon, message)
                        Response.Redirect("logged.aspx")
                    Else
                        dajarSWMagazyn_MIA.MyFunction.ClearSessionInformation(Session)
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Twoje konto zostało zablokowane. Skontaktuj sie z administratorem systemu dajarSWMagazyn_MIA!<br />"
                        Session(session_id) += "</div>"
                    End If
                End If
            End If
            conn.Close()

        End Using
    End Sub

    Protected Sub DDLOperatorLogowanie_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DDLOperatorLogowanie.SelectedIndexChanged
        If DDLOperatorLogowanie.Text <> "" Then
            Dim login As String = DDLOperatorLogowanie.Text.ToString
            If login.Split("_").Length = 3 Then
                Dim typ_oper As String = login.Split("_")(2).ToString
                If typ_oper = "M" Or typ_oper = "W" Then
                    DDLTypOperatora.SelectedValue = typ_oper
                End If
            End If
        End If
    End Sub
End Class