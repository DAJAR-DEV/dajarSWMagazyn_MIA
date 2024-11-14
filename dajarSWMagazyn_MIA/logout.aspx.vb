Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class logout
    Inherits System.Web.UI.Page
    Dim sqlExp As String = ""
    Dim result As String = ""

    Protected Sub DDLOperator_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DDLOperator.SelectedIndexChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim login As String = DDLOperator.SelectedValue.ToString
            sqlexp = "select hash from dp_swm_mia_uzyt where login = '" & login & "'"
            Dim hash As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
            Session("mylogin") = login
            Session("myhash") = hash
            sqlExp = "select trim(TYP_OPER) typ_oper FROM dp_swm_mia_UZYT WHERE LOGIN = '" & login & "'"
            Session("mytyp_oper") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlExp, conn)
            Session("contentHash") = "SESJA MIASTKO : " & hash
            Session("contentOperator") = "STANOWISKO W STREFIE : " & Session("mytyp_oper")
            Response.Redirect("logged.aspx")
            conn.Close()
        End Using
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session("contentKomunikat") = Session(session_id)

        If Not Page.IsPostBack Then
            dajarSWMagazyn_MIA.MyFunction.RefreshDDLOperator(DDLOperator, Session)
        End If

        TBUsername.Text = Session("mylogin")
        DDLTypOperatora.SelectedValue = Session("mytyp_oper")
    End Sub

    Protected Sub Login_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnWyloguj.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If TBUsername.Text.ToString <> "" And TBPassword.Text.ToString <> "" Then
                sqlExp = "SELECT IMIE||' '||NAZWISKO||'@'||LOGIN FROM dp_swm_mia_UZYT WHERE LOGIN = '" & TBUsername.Text & "' AND PASSWORD = '" & TBPassword.Text & "'"
                Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlExp, conn)
                If czy_istnieje <> "" Then
                    Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()

                    Dim mag_id As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN='" & TBUsername.Text & "'", conn)
                    If mag_id = "ALL" Then
                        sqlExp = "UPDATE dp_swm_mia_UZYT SET STATUS='' WHERE LOGIN = '" & Session("mylogin") & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlExp, conn)
                        result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "LOGOUT", Session("mytyp_oper"))
                        Session.Remove(session_id)
                        Session.Abandon()
                        Response.Redirect("index.aspx")
                    End If

                    sqlExp = "UPDATE dp_swm_mia_UZYT SET TYP_OPER='', STATUS='' WHERE LOGIN = '" & Session("mylogin") & "'"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlExp, conn)
                    result = dajarSWMagazyn_MIA.MyFunction.SetOperacjeStatus(Session("mylogin"), Session("myhash"), "LOGOUT", Session("mytyp_oper"))

                    Dim telefon As String = MyFunction.GetStringFromSqlExp("SELECT TELEFON FROM dp_swm_mia_UZYT WHERE LOGIN='" & Session("mylogin") & "'", conn)
                    Dim message As String = MyFunction.DataEvalSmsTime & vbNewLine & "dajarSWMagazyn_MIA " & vbNewLine & "wylogowanie uzytkownika : " & Session("mylogin") & " mag. : " & Session("contentMagazyn") & " typ_oper. : " & Session("mytyp_oper") & " sesja : " & Session("myhash")
                    dajarSWMagazyn_MIA.MyFunction.SetSmsInformationStatus(Session("mylogin"), Session("myhash"), telefon, message)

                    Session.Remove(session_id)
                    Session.Abandon()
                    Response.Redirect("index.aspx")
                Else
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Wprowadzone dane dla uzytkownika " & Session("mylogin") & " są niepoprawne!<br />"
                    Session(session_id) += "</div>"
                End If
            Else
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Wprowadzone dane dla uzytkownika " & Session("mylogin") & " są niepoprawne!<br />"
                Session(session_id) += "</div>"
            End If
            conn.Close()
        End Using
    End Sub
End Class