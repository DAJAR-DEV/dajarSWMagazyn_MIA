Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class a_userEdit
    Inherits System.Web.UI.Page

    Dim sqlexp As String = ""
    Dim result As Boolean
    Dim daRejestrSklepow As New OracleDataAdapter
    Dim dsRejestrSklepow As New DataSet
    Dim rejestrSklepow As New List(Of String)
    Dim cb As OracleCommandBuilder

    Protected Sub GridViewPrzydzieloneSklepy_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewDyspozycje.PageIndexChanging
        GridViewDyspozycje.PageIndex = e.NewPageIndex
        GridViewDyspozycje.DataBind()

        AktualizujGridViewUzytkownicyEdit(DDLListaZamowien.SelectedValue)
    End Sub

    Public Sub AktualizujGridViewUzytkownicyEdit(ByVal filtr As String)
        GridViewDyspozycje.DataSource = Nothing
        rejestrSklepow = New List(Of String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If filtr <> "" And filtr.Length = 12 Then
                sqlexp = "select rownum as lp,z.* from (" _
                & " select distinct dm.nr_zamow, dm.schemat, zg.nr_zamow_o, 'M' oper, dm.mag, dm.status, dm.skrot, dm.ile_poz, to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.autodata_zak, dm.hash from dp_swm_mia_mag dm, dp_swm_mia_zog zg " _
                & " where dm.login = '" & Session("kod_uzyt") & "' and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat and dm.autodata_zak is null" _
                & " union all " _
                & " select distinct dp.nr_zamow, dp.schemat, zg.nr_zamow_o, 'P' oper, dp.mag, dp.status, '' skrot, dp.ile_poz, to_char(dp.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dp.autodata_zak, dp.hash from dp_swm_mia_pak dp, dp_swm_mia_zog zg " _
                & " where dp.login = '" & Session("kod_uzyt") & "' and zg.nr_zamow = dp.nr_zamow and zg.schemat = dp.schemat and dp.autodata_zak is null" _
                & " ) z where z.nr_zamow = '" & filtr & "' order by z.autodata desc"
            Else
                sqlexp = "select rownum as lp,z.* from (" _
                & " select distinct dm.nr_zamow, dm.schemat, zg.nr_zamow_o, 'M' oper, dm.mag, dm.status, dm.skrot, dm.ile_poz, to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.autodata_zak, dm.hash from dp_swm_mia_mag dm, dp_swm_mia_zog zg " _
                & " where dm.login = '" & Session("kod_uzyt") & "' and zg.nr_zamow = dm.nr_zamow and zg.schemat = dm.schemat and dm.autodata_zak is null" _
                & " union all " _
                & " select distinct dp.nr_zamow, dp.schemat, zg.nr_zamow_o, 'P' oper, dp.mag, dp.status, '' skrot, dp.ile_poz, to_char(dp.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dp.autodata_zak, dp.hash from dp_swm_mia_pak dp, dp_swm_mia_zog zg " _
                & " where dp.login = '" & Session("kod_uzyt") & "' and zg.nr_zamow = dp.nr_zamow and zg.schemat = dp.schemat and dp.autodata_zak is null" _
                & " ) z order by z.autodata desc"
            End If

            Dim cmd As New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text

            daRejestrSklepow = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daRejestrSklepow)
            dsRejestrSklepow = New DataSet()
            daRejestrSklepow.Fill(dsRejestrSklepow)
            GridViewDyspozycje.DataSource = dsRejestrSklepow.Tables(0)
            GridViewDyspozycje.DataBind()
            conn.Close()

        End Using
    End Sub

    Public Sub LadujDDLListaZamowien()
        DDLListaZamowien.Items.Clear()
        DDLListaZamowien.Items.Add(New ListItem("Wszystkie", "Wszystkie", True))
        Dim listaZamowien As New List(Of String)

        For Each row In dsRejestrSklepow.Tables(0).Rows
            Dim nr_zamow As String = row(1).ToString
            If listaZamowien.Contains(nr_zamow) = False Then
                listaZamowien.Add(nr_zamow)
            End If
        Next

        listaZamowien.Sort()
        If listaZamowien.Count > 0 Then
            For Each nr_zam In listaZamowien
                DDLListaZamowien.Items.Add(New ListItem(nr_zam, nr_zam))
            Next
        End If
        DDLListaZamowien.DataBind()
    End Sub

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

        BUsunKonto.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz usunac konto uzytkownika " & Session("kod_uzyt_nazwa") & "') == false) return false")
        BWyslijSms.Attributes.Add("onclick", "javascrip:if(confirm ('Potwierdz wyslanie wiadomosci SMS do operatora " & Session("kod_uzyt").ToString & "') == false) return false")

        If Not Page.IsPostBack Then
            If przerwijLadowanie = False Then

                AktualizujGridViewUzytkownicyEdit("")
                LadujDDLListaZamowien()

                Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                    conn.Open()
                    sqlexp = "select dpu.email,dpu.telefon,dpu.hash,dpu.typ_oper,dpu.mag,dpu.blokada_konta from dp_swm_mia_uzyt dpu where dpu.login = '" & Session("kod_uzyt") & "'"
                    Dim cmd As New OracleCommand(sqlexp, conn)
                    cmd.CommandType = CommandType.Text
                    Dim dr As OracleDataReader = cmd.ExecuteReader()

                    Try
                        While dr.Read()
                            TBEmail.Text = dr.Item(0).ToString
                            TBTelefon.Text = dr.Item(1).ToString
                            LHash.Text = dr.Item(2).ToString
                            LStatusUzytkownik.Text = dr.Item(3).ToString
                            TBMagazyn.Text = dr.Item(4).ToString

                            If IsDBNull(dr.Item(5).ToString) = False Then
                                If dr.Item(5).ToString = "X" Then
                                    CBBlokada.Checked = True
                                Else
                                    CBBlokada.Checked = False
                                End If
                            End If

                        End While

                    Catch ex As System.ArgumentOutOfRangeException
                        Console.WriteLine(ex)
                    End Try

                    dr.Close()
                    cmd.Dispose()

                    sqlexp = "select TO_CHAR(w.autodata, 'YYYY/MM/DD HH24:MI:SS') autodata from (" _
                    & " select autodata from dp_swm_mia_oper " _
                    & " where login = '" & Session("kod_uzyt") & "' and typ_oper IN('LOGIN')" _
                    & " order by autodata desc) w" _
                    & " where rownum = 1"
                    LDataLogowania.Text = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    LOperatorLogin.Text = Session("kod_uzyt")
                    conn.Close()
                End Using
            End If
        End If

        Dim pageUrl As String = dajarSWMagazyn_MIA.MyFunction.ReplacePageUrl("UzytkownicyEdit.aspx", Request.Url.AbsoluteUri)
        Session("contentMenuSzybkie") = "<a href=" & pageUrl & " style=""color: #FFFFFF; font-weight: bold"">" & Session("kod_uzyt_nazwa") & "</a>"
        Session("contentKomunikat") = Session(session_id)

    End Sub


    Protected Sub BZatwierdzZmiany_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BZatwierdzZmiany.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If TBEmail.Text <> "" And TBTelefon.Text <> "" And TBMagazyn.Text <> "" Then
                sqlexp = "UPDATE dp_swm_mia_UZYT SET EMAIL='" & TBEmail.Text & "',TELEFON='" & TBTelefon.Text & "',MAG='" & TBMagazyn.Text & "' WHERE login = '" & Session("kod_uzyt").ToString & "'"
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                If CBBlokada.Checked = True Then
                    sqlexp = "UPDATE dp_swm_mia_UZYT SET BLOKADA_KONTA='X' WHERE login = '" & Session("kod_uzyt").ToString & "'"
                ElseIf CBBlokada.Checked = False Then
                    sqlexp = "UPDATE dp_swm_mia_UZYT SET BLOKADA_KONTA=NULL WHERE login = '" & Session("kod_uzyt").ToString & "'"
                End If
                result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                AktualizujGridViewUzytkownicyEdit(DDLListaZamowien.SelectedValue)
            End If
            conn.Close()
        End Using
    End Sub

    ''Protected Sub BUsunKonto_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BUsunKonto.Click
    ''    ''Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
    ''    ''    conn.Open()
    ''    ''    sqlexp = "DELETE FROM dp_swm_mia_UZYT WHERE LOGIN = '" & Session("kod_uzyt") & "'"
    ''    ''    result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
    ''    ''    Response.Redirect("a_user.aspx")
    ''    ''    conn.Close()
    ''    ''End Using
    ''End Sub

    Protected Sub BWyloguj_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWyloguj.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            sqlexp = "update dp_swm_mia_uzyt set hash = '',status = '',typ_oper='' where login = '" & Session("kod_uzyt").ToString & "'"
            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
            Response.Redirect("a_user.aspx")
            conn.Close()
        End Using
    End Sub

    Protected Sub DDLListaZamowien_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles DDLListaZamowien.SelectedIndexChanged
        Dim nr_zamow As String = DDLListaZamowien.SelectedValue.ToString
        If nr_zamow.Length = 12 Then
            AktualizujGridViewUzytkownicyEdit(nr_zamow)
        Else
            AktualizujGridViewUzytkownicyEdit("")
        End If

    End Sub

    Protected Sub BWyslijSms_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BWyslijSms.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If TBKomunikatSms.Text <> "" And TBKomunikatSms.Text <> "&nbsp;" And TBTelefon.Text <> "" Then
                Dim telefon As String = TBTelefon.Text.ToString
                Dim komunikat As String = TBKomunikatSms.Text.ToString.Replace("!", "").Replace("@", "").Replace("#", "").Replace("$", "").Replace("%", "").Replace("^", "").Replace("&", "").Replace("&", "").Replace("*", "").Replace("(", "").Replace(")", "").Replace(":", "").Replace(";", "").Replace("[", "").Replace("]", "").Replace("<", "").Replace(">", "").Replace("'", "")
                Dim message As String = MyFunction.DataEvalSmsTime & vbNewLine & "dajarSWMagazyn_MIA " & vbNewLine & "komunikat : " & TBKomunikatSms.Text.ToString
                Dim login As String = LOperatorLogin.Text.ToString
                Dim hash As String = LHash.Text.ToString
                dajarSWMagazyn_MIA.MyFunction.SetSmsInformationStatus(login, hash, telefon, message)

                TBKomunikatSms.Text = ""
            End If
            conn.Close()
        End Using
    End Sub

End Class