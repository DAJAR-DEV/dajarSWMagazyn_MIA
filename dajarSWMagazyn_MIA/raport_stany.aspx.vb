Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class raport_stany
    Inherits System.Web.UI.Page
    Dim sqlexp As String = ""
    Dim result As Boolean = False
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder

    Protected Sub GridViewRaport_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewRaport.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewRaport.PageIndex = e.NewPageIndex
        GridViewRaport.DataBind()
        LadujDaneGridViewMagazyn("")
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

    Protected Sub page_loaded() Handles Me.LoadComplete
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session("contentKomunikat") = Session(session_id)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Dim przerwijLadowanie As Boolean = False

        ''BUsunTowar.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz usunac wybrany towar z pozycji magazynowej\n') == false) return false")

        If Session("mylogin") = Nothing And Session("myhash") = Nothing Then
            Session.Abandon()
            Response.Redirect("index.aspx")
            ''ElseIf Session("mytyp_oper") <> "M" And Session("mytyp_oper") <> "W" Then
            ''    Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
            ''    Session(session_id) += "<br />Zaloguj sie na opowiedniego operatora - typ MAGAZYN/WOZEK<br />"
            ''    Session(session_id) += "</div>"
            ''    PanelMagazyn.Visible = False
            ''    przerwijLadowanie = True
        Else
            Session.Remove(session_id)
        End If

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session("contentMagazyn") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN = '" & Session("mylogin") & "'", conn)
            conn.Close()
        End Using

        If Not Page.IsPostBack Then
            TBDataFrom.Text = DateTime.Now.Year.ToString & "-" & "01"
            TBDataTo.Text = DateTime.Now.Year.ToString & "-" & DateTime.Now.Month.ToString("D2") & "-31"
            dajarSWMagazyn_MIA.MyFunction.RefreshDDLOperator(DDLOperator, Session)

            If przerwijLadowanie = False Then
                ''LadujDaneGridViewMagazyn("")
            End If
        End If

        Session("contentKomunikat") = Session(session_id)
    End Sub

    Protected Sub TBSkrot_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBSkrot.TextChanged
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If TBSkrot.Text.Length >= 5 Then
                Dim skrot As String = TBSkrot.Text.ToString
                sqlexp = "select indeks,nazdop,kod_tow,skt2_0,desql_japu_nwa.fsql_japu_wolne_MAG(indeks,'700') stan_700 from ht_rejna where nazpot='" & skrot & "' and is_deleted='N'"
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
                    End While

                Catch ex As System.ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try
                dr.Close()
                cmd.Dispose()
                conn.Close()
            End If

        End Using
    End Sub

    Public Sub LadujDaneGridViewMagazyn(ByVal filtr As String)
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            Dim filename As String = "\sql\rap_stany.sql"
            Dim fileInput As New StreamReader(dajarSWMagazyn_MIA.MyFunction.networkMainPath & filename, System.Text.Encoding.GetEncoding(1250))
            Dim bufor As String = fileInput.ReadToEnd
            fileInput.Close()
            Dim sql_1 As String = bufor

            sql_1 = sql_1.Replace("2017-01", TBDataFrom.Text.ToString)
            sql_1 = sql_1.Replace("2017-12", TBDataTo.Text.ToString)
            sql_1 = sql_1.Replace("111434", TBIndeks.Text.ToString)


            sqlexp = sql_1
            Session("mm_sql") = 1

            ''Dim result = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sql_1, conn)
            ''If result.Length > 0 Then
            ''    sqlexp = sql_1
            ''    Session("mm_sql") = 1
            ''Else
            ''    sqlexp = ""
            ''    Session.Remove("mm_sql")
            ''End If

            GridViewRaport.DataSource = Nothing
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridViewRaport.DataSource = dsPartie.Tables(0)
            GridViewRaport.DataBind()
            cmd.Dispose()

            If dsPartie.Tables(0).Rows.Count = 0 Then
                LIleDokumentow.Text = "brak"
            Else
                LIleDokumentow.Text = dsPartie.Tables(0).Rows.Count.ToString
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BGenerujRaport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BGenerujRaport.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If TBSkrot.Text.ToString <> "" And TBIndeks.Text.ToString <> "" Then
                LadujDaneGridViewMagazyn("")
            End If
            conn.Close()
        End Using
    End Sub

    ''Protected Sub GridViewAdresacja_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewAdresacja.RowDataBound
    ''    If e.Row.RowType = DataControlRowType.DataRow Then
    ''        Dim cb As CheckBox = e.Row.FindControl("CBKodSelect")
    ''        If cb IsNot Nothing And cb.Checked Then
    ''            e.Row.BackColor = GridViewAdresacja.SelectedRowStyle.BackColor
    ''        Else
    ''            Dim strefa As String = e.Row.Cells(7).Text.ToString
    ''            If strefa = "Z" Then : e.Row.BackColor = LTowarZbieranie.BackColor
    ''            ElseIf strefa = "N" Then : e.Row.BackColor = LTowarNiepelna.BackColor
    ''            ElseIf strefa = "P" Then : e.Row.BackColor = LTowarPaleta.BackColor
    ''            Else : e.Row.BackColor = LTowarPusty.BackColor
    ''            End If

    ''            Dim is_active As String = e.Row.Cells(8).Text.ToString
    ''            If is_active = "0" Then : e.Row.BackColor = LTowarPusty.BackColor
    ''            End If
    ''        End If
    ''    End If
    ''End Sub
End Class