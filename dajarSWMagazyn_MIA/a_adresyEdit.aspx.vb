Imports Oracle.ManagedDataAccess.Client
Imports System.Data

Partial Public Class a_adresyEdit
    Inherits System.Web.UI.Page

    Private da As New OracleDataAdapter
    Private ds As New DataSet
    Private cb As OracleCommandBuilder
    Private daUpdate As New OracleDataAdapter
    Private dsUpdate As New DataSet
    Private cbUpdate As OracleCommandBuilder
    Dim sqlexp As String = ""
    Dim result As Boolean = False

    Protected Sub GridViewTowary_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewTowary.PageIndexChanging
        GridViewTowary.PageIndex = e.NewPageIndex
        GridViewTowary.DataBind()
        AktualizujGridViewTowary()
    End Sub

    Protected Sub GridViewTowary_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewTowary.RowDataBound
        ''AktualizujGridViewDGridViewUzytkownicyStatusy()
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim strefa As String = e.Row.Cells(3).Text.ToString
            If strefa = "Z" Then
                e.Row.BackColor = LZbieranie.BackColor
            ElseIf strefa = "P" Then
                e.Row.BackColor = LPaleta.BackColor
            ElseIf strefa = "N" Then
                e.Row.BackColor = LNiePaleta.BackColor
            End If
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            BUsunTowar.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz usunac wybrany towar z pozycji magazynowej\n') == false) return false")
            BAktywacjaAdres.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz aktywowac/deaktywowac wybrany adres magazynowy\n') == false) return false")

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
                    RefreshDDLRegal(DDLRegal)
                    RefreshDDLPolka(DDLPolka)
                    LISActive.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select is_active from dp_swm_mia_adresy where adres='" & Session("kod_adres") & "'", conn)
                    AktualizujGridViewTowary()
                End If
            End If

            Session("contentKomunikat") = Session(session_id)

            conn.Close()
        End Using
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
            ddlObiekt.SelectedValue = Session("kod_adres").ToString.Substring(0, 1)
            ddlObiekt.DataBind()
        End Using
    End Sub

    Protected Sub RefreshDDLRzad(ByRef ddlObiekt As DropDownList)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ddlObiekt.Items.Clear()
            ''ddlObiekt.Items.Add("")

            Dim znak As String = ""
            For i = 1 To 99
                ddlObiekt.Items.Add(New ListItem("A" & i.ToString, "A" & i.ToString))
                znak = i.ToString().PadLeft(2, "0")
                ddlObiekt.Items.Add(New ListItem(znak, znak))
            Next
            ddlObiekt.SelectedValue = Session("kod_adres").ToString.Substring(1, 2)
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
            ddlObiekt.SelectedValue = Session("kod_adres").ToString.Substring(3, 2)
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
            ddlObiekt.SelectedValue = Session("kod_adres").ToString.Substring(5, 2)
            ddlObiekt.DataBind()
            conn.Close()
        End Using
    End Sub


    Public Sub AktualizujGridViewTowary()
        GridViewTowary.DataSource = Nothing
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            sqlexp = "select dt.skrot,dt.adres,dt.strefa,desql_japu_nwa.fsql_japu_wolne_MAG(get_index_tow(dt.skrot),'700') stan_700,dt.is_active,dt.login,dt.hash,dt.autodata from dp_swm_mia_towary dt where dt.adres like '" & Session("kod_adres") & "' and dt.is_active='1' order by dt.skrot asc"
            Dim cmd As New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text

            da = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(da)
            ds = New DataSet()
            da.Fill(ds)
            GridViewTowary.DataSource = ds.Tables(0)
            GridViewTowary.DataBind()
            conn.Close()

        End Using
    End Sub

    Protected Sub BOdznaczWszystkie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BOdznaczWszystkie.Click
        If GridViewTowary.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewTowary.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                cb.Checked = False
            Next
        End If
    End Sub

    Protected Sub BZaznaczWszystkie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BZaznaczWszystkie.Click
        If GridViewTowary.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewTowary.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                cb.Checked = True
            Next
        End If
    End Sub

    ''Protected Sub BAktywacjaTowary_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BAktywacjaTowary.Click
    ''    Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
    ''        conn.Open()

    ''        If GridViewTowary.Rows.Count > 0 Then
    ''            For Each row As GridViewRow In GridViewTowary.Rows
    ''                Dim cb As CheckBox = row.FindControl("CBKodSelect")
    ''                If cb IsNot Nothing And cb.Checked Then
    ''                    Dim skrot As String = row.Cells(1).Text.ToString
    ''                    Dim adres As String = row.Cells(2).Text.ToString
    ''                    Dim is_active As String = row.Cells(4).Text.ToString
    ''                    If is_active = "1" Then : is_active = "0"
    ''                    ElseIf is_active = "0" Then : is_active = "1"
    ''                    End If

    ''                    sqlexp = "update dp_swm_mia_towary set is_active='" & is_active & "',login='" & Session("mylogin") & "',hash='" & Session("myhash") & "' where adres='" & adres & "' and skrot='" & skrot & "'"
    ''                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
    ''                End If
    ''            Next
    ''        End If

    ''        AktualizujGridViewTowary()
    ''        conn.Close()
    ''    End Using
    ''End Sub

    Protected Sub BDodajTowar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BDodajTowar.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If TBSkrot.Text.ToString <> "" And TBIndeks.Text.ToString <> "" Then
                Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                Dim skrot As String = TBSkrot.Text.ToString
                Dim adres As String = Session("kod_adres")
                Dim strefa As String = DDLStrefaMagazyn.SelectedValue.ToString

                sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_towary WHERE adres='" & adres & "' and skrot='" & skrot & "' and strefa='" & strefa & "'"
                Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                If czy_istnieje = "0" Then
                    sqlexp = "insert into dp_swm_mia_towary values('" & skrot & "','" & adres & "','" & strefa & "',1,'" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                    TBSkrot.Text = ""
                    TBEan.Text = ""
                    TBIndeks.Text = ""
                    TBNazwa.Text = ""
                    TBPrefix.Text = ""
                    DDLStrefaMagazyn.SelectedValue = "Z"
                Else

                    sqlexp = "SELECT IS_ACTIVE FROM dp_swm_mia_towary WHERE adres='" & adres & "' and skrot='" & skrot & "' and strefa='" & strefa & "'"
                    Dim is_active As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If is_active = "0" Then
                        sqlexp = "update dp_swm_mia_towary set is_active='1',login='" & Session("mylogin") & "',hash='" & Session("myhash") & "' where adres='" & adres & "' and skrot='" & skrot & "' and strefa='" & strefa & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    Else
                        Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                        Session(session_id) += "<br />Wprowadzy skrot jest juz wprowadzony na danym adresie/strefie zamagazynowania!<br />"
                        Session(session_id) += "</div>"
                    End If
                End If

            End If


            AktualizujGridViewTowary()
            conn.Close()
        End Using
    End Sub

    Protected Sub BUsunTowar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BUsunTowar.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If GridViewTowary.Rows.Count > 0 Then
                For Each row As GridViewRow In GridViewTowary.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim skrot As String = row.Cells(1).Text.ToString
                        Dim adres As String = row.Cells(2).Text.ToString
                        Dim strefa As String = row.Cells(3).Text.ToString
                        Dim is_active As String = row.Cells(4).Text.ToString

                        sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_towary WHERE adres='" & adres & "' and skrot='" & skrot & "' and strefa='" & strefa & "'"
                        Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        If czy_istnieje <> "0" Then
                            sqlexp = "update dp_swm_mia_towary set is_active='0',login='" & Session("mylogin") & "',hash='" & Session("myhash") & "' where adres='" & adres & "' and skrot='" & skrot & "' and strefa='" & strefa & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            TBSkrot.Text = ""
                            TBEan.Text = ""
                            TBIndeks.Text = ""
                            TBNazwa.Text = ""
                            TBPrefix.Text = ""
                            DDLStrefaMagazyn.SelectedValue = "Z"
                        Else
                            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
                            Session(session_id) += "<br />Wprowadzy skrot nie jest wprowadzony na danym adresie zamagazynowania!<br />"
                            Session(session_id) += "</div>"
                        End If
                    End If
                Next

                AktualizujGridViewTowary()
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BAktywacjaAdres_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BAktywacjaAdres.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim is_active As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("select is_active from dp_swm_mia_adresy where adres='" & Session("kod_adres") & "'", conn)
            If is_active = "0" Then
                is_active = "1"
            Else
                is_active = "0"
            End If

            sqlexp = "update dp_swm_mia_adresy set is_active='" & is_active & "' where adres = '" & Session("kod_adres") & "'"
            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

            TBSkrot.Text = ""
            TBEan.Text = ""
            TBIndeks.Text = ""
            TBNazwa.Text = ""
            TBPrefix.Text = ""
            DDLStrefaMagazyn.SelectedValue = "Z"
            LISActive.Text = is_active

            AktualizujGridViewTowary()
            conn.Close()
        End Using
    End Sub

    Protected Sub TBSkrot_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBSkrot.TextChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If TBSkrot.Text.Length >= 5 Then
                Dim skrot As String = TBSkrot.Text.ToString
                sqlexp = "select indeks,nazdop,kod_tow,skt2_0 from ht_rejna where nazpot='" & skrot & "' and is_deleted='N'"
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
End Class