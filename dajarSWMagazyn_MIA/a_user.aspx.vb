Imports Oracle.ManagedDataAccess.Client
Imports System.Data
Imports System.IO

Partial Public Class a_user
    Inherits System.Web.UI.Page
    Dim sqlexp As String = ""
    Dim result As Boolean = False
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder

    Protected Sub GridViewUzytkownicy_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewUzytkownicy.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewUzytkownicy.PageIndex = e.NewPageIndex
        GridViewUzytkownicy.DataBind()
        LadujDaneGridViewUzytkownicy()
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

        ''Session.Remove("contentKomunikat")
        If Not Page.IsPostBack Then
            If przerwijLadowanie = False Then
                LadujDaneGridViewUzytkownicy()
            End If
        End If
        Session("contentKomunikat") = Session(session_id)
    End Sub

    Public Sub LadujDaneGridViewUzytkownicy()
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            sqlexp = "SELECT du.LOGIN, du.IMIE, du.NAZWISKO, du.telefon, du.HASH, du.TYP_OPER, " _
            & " (select count(distinct nr_zamow) from dp_swm_mia_mag where login = du.login and status in('MG','WN','ZW','HB')) ile_zam," _
            & " (select count(dm.ile_poz) from dp_swm_mia_mag dm where dm.login = du.login and dm.status in('MG','WN','ZW','HB')) ile_poz," _
            & " (select count(distinct nr_zamow) from dp_swm_mia_pak where login = du.login and status in('PE')) ile_zam_spak," _
            & " (select sum(ile_poz) from dp_swm_mia_pak where login = du.login and status in('PE')) ile_poz_spak," _
            & " du.MAG, du.STATUS, " _
            & " (select max(to_char(autodata, 'YYYY/MM/DD HH24:MI:SS')) from dp_swm_mia_oper where login=du.login and typ_oper='LOGIN') data_log," _
            & " du.BLOKADA_KONTA blokada" _
            & " FROM dp_swm_mia_UZYT du ORDER BY du.blokada_konta desc,du.status asc,du.LOGIN asc"


            sqlexp = "select w.login, w.imie, w.nazwisko, w.telefon, w.hash, w.typ_oper, 
            (select count(distinct nr_zamow) from dp_swm_mia_mag where login = w.login and status in('MG','WN','ZW','HB')) ile_zam,
            (select count(dm.ile_poz) from dp_swm_mia_mag dm where dm.login = w.login and dm.status in('MG','WN','ZW','HB')) ile_poz,
            '0' ile_zam_spak, '0' ile_poz_spak,
            w.mag,w.status,
            (select max(to_char(autodata, 'YYYY/MM/DD HH24:MI:SS')) from dp_swm_mia_oper where login=w.login and typ_oper='LOGIN') data_log,
            w.blokada
            from (
            SELECT du.LOGIN, du.IMIE, du.NAZWISKO, du.telefon, du.HASH, du.TYP_OPER,
            du.MAG, du.STATUS, 
            du.BLOKADA_KONTA blokada
            FROM dp_swm_mia_UZYT du ORDER BY du.blokada_konta desc,du.status asc,du.LOGIN asc
            )w"

            GridViewUzytkownicy.DataSource = Nothing
            Dim cmd As New OracleCommand(sqlexp, conn)
            daPartie = New OracleDataAdapter(cmd)
            cb = New OracleCommandBuilder(daPartie)
            dsPartie = New DataSet()
            daPartie.Fill(dsPartie)
            GridViewUzytkownicy.DataSource = dsPartie.Tables(0)
            GridViewUzytkownicy.DataBind()
            cmd.Dispose()

            If GridViewUzytkownicy.Rows.Count = 0 Then
                LIleDokumentow.Text = "brak rekordow"
            Else
                LIleDokumentow.Text = GridViewUzytkownicy.Rows.Count.ToString
            End If
            conn.Close()

        End Using
    End Sub

    Protected Sub GridViewUzytkownicy_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewUzytkownicy.RowDataBound
        ''AktualizujGridViewDGridViewUzytkownicyStatusy()
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim status As String = e.Row.Cells(12).Text.ToString
            If status = "X" Then
                e.Row.BackColor = LUzytkownikAktywny.BackColor
            End If

            Dim typ_ope As String = e.Row.Cells(6).Text.ToString
            Dim ile_mag As String = e.Row.Cells(7).Text.ToString
            Dim ile_pak As String = e.Row.Cells(8).Text.ToString

            If (typ_ope = "M" Or typ_ope = "O") And ile_mag = "0" Then
                e.Row.BackColor = LUzytkownikWolny.BackColor
            ElseIf typ_ope = "S" Then
                e.Row.BackColor = LAdministrator.BackColor
            End If
        End If

    End Sub

    Protected Sub GridView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridViewUzytkownicy.SelectedIndexChanged
        If GridViewUzytkownicy.Rows.Count > 0 Then
            Session("kod_uzyt") = GridViewUzytkownicy.SelectedRow.Cells(1).Text
            Response.Redirect("a_userEdit.aspx")
        End If
    End Sub

End Class