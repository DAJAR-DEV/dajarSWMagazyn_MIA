Imports dajarSWMagazyn_MIA.HashMd5
Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class change_pass
    Inherits System.Web.UI.Page

    Dim sqlExp As String = ""
    Dim result As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim userAgent As String = HttpContext.Current.Request.UserAgent.ToLower
        Session("userAgent") = HttpContext.Current.Request.UserAgent.ToLower
        If userAgent IsNot Nothing And (userAgent.Contains("ipad") Or userAgent.Contains("iphone")) Then
            Session("myDevice") = "ipad"
        Else
            Session("myDevice") = "standard"
        End If
    End Sub

    Protected Sub BZapiszZmiany_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BZapiszZmiany.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If TBLogin.Text = "" And TBPass1.Text = "" And TBPass2.Text = "" Then
            ElseIf TBPass1.Text = "" Or TBPass2.Text = "" Then
            Else
                If TBPass1.Text <> TBPass2.Text Then
                    Session("contentKomunikat") = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session("contentKomunikat") += "<br />Wprowadzone hasła nie są jednakowe!<br />Zmień ustawienia nowego hasła i spróbuj ponownie.<br />"
                    Session("contentKomunikat") += "</div>"
                Else
                    sqlExp = "SELECT KOD_UZYT FROM DP_MONIT_UZYT WHERE LOGIN = '" & TBLogin.Text.ToLower & "'"
                    Dim wynikSql As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlExp, conn)
                    wynikSql = wynikSql.Trim

                    If wynikSql <> "" Then
                        sqlExp = "UPDATE DP_MONIT_UZYT SET PASSWORD = '" & TBPass1.Text & "' WHERE KOD_UZYT = '" & wynikSql & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlExp, conn)
                        Session.Remove("contentKomunikaty")
                        Response.Redirect("login.aspx")
                    Else
                        Session("contentKomunikat") = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session("contentKomunikat") += "<br />Wprowadzona nazwa użytkownika /" & TBLogin.Text & "/ nie figuruje w systemie!<br /><br />"
                        Session("contentKomunikat") += "</div>"
                    End If
                End If
            End If
            conn.Close()
        End Using
    End Sub
End Class