Imports Oracle.ManagedDataAccess.Client
Imports System.Data

Partial Public Class a_adresyAdd
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

        If Not Page.IsPostBack Then
            If przerwijLadowanie = False Then
                RefreshDDLHala(DDLHala)
                RefreshDDLRzad(DDLRzad)
                RefreshDDLRegal(DDLRegalFrom)
                RefreshDDLRegal(DDLRegalTo)
                RefreshDDLPolka(DDLPolkaFrom)
                RefreshDDLPolka(DDLPolkaTo)
            End If
        End If

        Session("contentKomunikat") = Session(session_id)

    End Sub

    Protected Sub RefreshDDLHala(ByRef ddlObiekt As DropDownList)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()
            ''ddlObiekt.Items.Add("")

            Dim znak As Char = ""
            For i = 65 To 90
                znak = Chr(i)
                ddlObiekt.Items.Add(New ListItem(znak, znak))
            Next
            ddlObiekt.SelectedValue = "A"
            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLRzad(ByRef ddlObiekt As DropDownList)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()
            ''ddlObiekt.Items.Add("")

            Dim znak As String = ""
            For i = 1 To 99
                znak = i.ToString().PadLeft(2, "0")
                ddlObiekt.Items.Add(New ListItem("A" & i.ToString, "A" & i.ToString))
                ddlObiekt.Items.Add(New ListItem(znak, znak))
            Next
            ddlObiekt.SelectedValue = "01"
            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLRegal(ByRef ddlObiekt As DropDownList)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()
            ''ddlObiekt.Items.Add("")

            Dim znak As String = ""
            For i = 1 To 99
                znak = i.ToString().PadLeft(2, "0")
                ddlObiekt.Items.Add(New ListItem(znak, znak))
            Next
            ddlObiekt.SelectedValue = "01"
            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub

    Protected Sub RefreshDDLPolka(ByRef ddlObiekt As DropDownList)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()
            ''ddlObiekt.Items.Add("")

            Dim znak As String = ""
            For i = 0 To 99
                znak = i.ToString().PadLeft(2, "0")
                ddlObiekt.Items.Add(New ListItem(znak, znak))
            Next
            ddlObiekt.SelectedValue = "00"
            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub

    Protected Sub BAdresAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BAdresAdd.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim hala As String = DDLHala.SelectedValue.ToString
            Dim rzad As String = DDLRzad.SelectedValue.ToString
            Dim regalFrom As Integer = Integer.Parse(DDLRegalFrom.SelectedValue.ToString)
            Dim regalTo As Integer = Integer.Parse(DDLRegalTo.SelectedValue.ToString)

            Dim polkaFrom As Integer = DDLPolkaFrom.SelectedValue.ToString
            Dim polkaTo As Integer = DDLPolkaTo.SelectedValue.ToString

            For j = polkaFrom To polkaTo
                Dim polka_str As String = j.ToString().PadLeft(2, "0")
                ''''polka_str = Chr(j).ToString & "0"
                For i = regalFrom To regalTo
                    Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                    Dim regal_str As String = i.ToString().PadLeft(2, "0")
                    ''''regal_str = Chr(i).ToString & "0"
                    Dim adres As String = hala & rzad & regal_str & polka_str
                    sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_adresy WHERE adres='" & adres & "' and hala='" & hala & "' and rzad='" & rzad & "' and regal='" & regal_str & "' and polka='" & polka_str & "'"
                    Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If czy_istnieje = "0" Then
                        sqlexp = "insert into dp_swm_mia_adresy values('" & adres & "','" & hala & "','" & rzad & "','" & regal_str & "','" & polka_str & "',1,'" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Wprowadzy adres zamagazynowania juz widnieje w systemie!<br />"
                        Session(session_id) += "</div>"
                    End If
                Next
            Next

            If Session(session_id) Is Nothing Then
                Session.Remove(session_id)
                Response.Redirect("a_adresy.aspx")
            End If
            conn.Close()
        End Using
    End Sub
End Class