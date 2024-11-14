Imports Oracle.ManagedDataAccess.Client
Imports System.Data

Partial Public Class userAdd
    Inherits System.Web.UI.Page
    Private da As New OracleDataAdapter
    Private ds As New DataSet
    Private cb As OracleCommandBuilder
    Private daUpdate As New OracleDataAdapter
    Private dsUpdate As New DataSet
    Private cbUpdate As OracleCommandBuilder
    Dim sqlexp As String = ""
    Dim result As Boolean = False

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

        ''Session.Remove("contentKomunikat")

        If Not Page.IsPostBack Then
        End If

        Session("contentKomunikat") = Session(session_id)

    End Sub

    Protected Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BUserAdd.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
            Dim czyBlokadaKonta As String = CBBlokadaKonta.Checked

            Dim insertOperator As Boolean = True
            If TBLogin.Text = "" Or TBHaslo.Text = "" Then
                insertOperator = False
            ElseIf TBMagazyn.Text <> "700" And TBMagazyn.Text <> "46" Then
                insertOperator = False
            ElseIf TBtyp_oper.Text <> "M" And TBtyp_oper.Text <> "O" And TBtyp_oper.Text <> "P" Then
                insertOperator = False
            ElseIf TBTelefon.Text = "" Or TBTelefon.Text.Length <> 9 Then
                insertOperator = False
            End If

            If insertOperator = True Then
                Dim login As String = TBLogin.Text
                sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_UZYT WHERE LOGIN = '" & login & "'"
                Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)

                If czy_istnieje = "0" Then
                    If CBBlokadaKonta.Checked Then : czyBlokadaKonta = "X"
                    Else : czyBlokadaKonta = ""
                    End If

                    sqlexp = "INSERT INTO dp_swm_mia_UZYT (LOGIN,IMIE,NAZWISKO,EMAIL,TELEFON,PASSWORD,BLOKADA_KONTA,TYP_OPER,MAG) " _
                    + "VALUES('" & login & "','" & TBImie.Text & "','" & TBNazwisko.Text & "','" & TBAdresEmail.Text & "','" & TBTelefon.Text & "','" & TBHaslo.Text & "','" & czyBlokadaKonta & "','" & TBtyp_oper.Text & "','" & TBMagazyn.Text & "')"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    Session.Remove(session_id)
                    Response.Redirect("a_user.aspx")
                Else
                    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                    Session(session_id) += "<br />Wprowadzy operator juz widnieje w systemie!<br />"
                    Session(session_id) += "</div>"
                End If
            Else
                Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                Session(session_id) += "<br />Wprowadz poprawnie wszystkie wymagane informacje do utworzenie konta!<br />"
                Session(session_id) += "</div>"
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub TBNazwisko_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TBNazwisko.TextChanged
        If TBImie.Text <> "" And TBNazwisko.Text <> "" Then
            TBLogin.Text = TBImie.Text.Substring(0, 1).ToLower & "_" & TBNazwisko.Text.ToLower
            TBAdresEmail.Text = TBImie.Text.ToLower & "." & TBNazwisko.Text.ToLower & "@dajar.pl"
            TBTelefon.Focus()
        End If
    End Sub

End Class