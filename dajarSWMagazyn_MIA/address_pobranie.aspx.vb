Imports Oracle.ManagedDataAccess.Client
Imports System.Data

Partial Public Class address_pobranie
    Inherits System.Web.UI.Page
    Dim sqlexp As String = ""
    Dim result As Boolean = False
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder

    Protected Sub GridViewMagazyn_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewMagazyn.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewMagazyn.PageIndex = e.NewPageIndex
        GridViewMagazyn.DataBind()
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

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Dim przerwijLadowanie As Boolean = False

        BTowarPobranie.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz pobrac wskazane artykuly?\n') == false) return false")

        If Session("mylogin") = Nothing And Session("myhash") = Nothing Then
            Session.Abandon()
            Response.Redirect("index.aspx")
        ElseIf Session("mytyp_oper") <> "M" And Session("mytyp_oper") <> "W" Then
            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
            Session(session_id) += "<br />Zaloguj sie na opowiedniego operatora - typ MAGAZYN/WOZEK<br />"
            Session(session_id) += "</div>"
            PanelMagazyn.Visible = False
            przerwijLadowanie = True
        Else
            Session.Remove(session_id)
        End If

        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Session("contentMagazyn") = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT MAG FROM dp_swm_mia_UZYT WHERE LOGIN = '" & Session("mylogin") & "'", conn)
            conn.Close()
        End Using

        If Not Page.IsPostBack Then
            PWozek.Visible = False

            dajarSWMagazyn_MIA.MyFunction.RefreshDDLOperator(DDLOperator, Session)
            If Session("mytyp_oper") = "W" Then
                PWozek.Visible = True
            End If
            If przerwijLadowanie = False Then
                LadujDaneGridViewMagazyn("")
            End If
        End If

        Session("contentKomunikat") = Session(session_id)
    End Sub

    Public Sub LadujDaneGridViewMagazyn(ByVal filtr As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If Session("mytyp_oper") = "W" Then
                If filtr <> "" Then
                    sqlexp = "select dt.skrot,r.nazdop,r.jm,r.kod_tow,r.skt2_0,r.klasa,dt.adres,dt.strefa,dt.is_active from dp_swm_mia_towary dt, ht_rejna r where dt.skrot = r.nazpot and r.is_deleted='N' and dt.strefa in('N','P','Z') and (dt.skrot like '" & filtr & "%' or dt.adres like '" & filtr & "%') and dt.is_active=1 order by decode(dt.strefa, 'N', 1, 'P', 2, 'Z', 3), dt.skrot"
                Else
                    sqlexp = "select dt.skrot,r.nazdop,r.jm,r.kod_tow,r.skt2_0,r.klasa,dt.adres,dt.strefa,dt.is_active from dp_swm_mia_towary dt, ht_rejna r where dt.skrot = r.nazpot and r.is_deleted='N' and dt.strefa in('N','P','Z') and dt.is_active=1 order by decode(dt.strefa, 'N', 1, 'P', 2, 'Z', 3), dt.skrot"
                End If

                GridViewMagazyn.DataSource = Nothing
                Dim cmd As New OracleCommand(sqlexp, conn)
                daPartie = New OracleDataAdapter(cmd)
                cb = New OracleCommandBuilder(daPartie)
                dsPartie = New DataSet()
                daPartie.Fill(dsPartie)
                GridViewMagazyn.DataSource = dsPartie.Tables(0)
                GridViewMagazyn.DataBind()
                cmd.Dispose()

                If dsPartie.Tables(0).Rows.Count = 0 Then
                    LIleDokumentow.Text = "brak"
                    PanelMagazynData.Visible = False
                Else
                    LIleDokumentow.Text = dsPartie.Tables(0).Rows.Count.ToString
                    PanelMagazynData.Visible = True
                End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BOdznaczWszystkie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BOdznaczWszystkie.Click
        If GridViewMagazyn.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewMagazyn.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                cb.Checked = False
            Next
        End If
        ''AktualizujGridViewMagazynStatusy()
    End Sub

    Protected Sub BZaznaczWszystkie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BZaznaczWszystkie.Click
        If GridViewMagazyn.Rows.Count > 0 Then
            For Each row As GridViewRow In GridViewMagazyn.Rows
                Dim cb As CheckBox = row.FindControl("CBKodSelect")
                cb.Checked = True
            Next
        End If
    End Sub

    Protected Sub BTowarPobranie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarPobranie.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If GridViewMagazyn.Rows.Count > 0 Then
                For Each row As GridViewRow In GridViewMagazyn.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim skrot As String = row.Cells(2).Text.ToString
                        Dim adres As String = row.Cells(8).Text.ToString
                        Dim strefa As String = row.Cells(9).Text.ToString
                        Dim is_active As String = row.Cells(10).Text.ToString

                        sqlexp = "SELECT COUNT(*) FROM dp_swm_mia_wozek WHERE adres='" & adres & "' and skrot='" & skrot & "' and strefa='" & strefa & "' and login='" & Session("mylogin") & "' and is_active='1'"
                        Dim czy_istnieje As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                        If czy_istnieje = "0" Then
                            sqlexp = "update dp_swm_mia_towary set is_active='0' where adres='" & adres & "' and skrot='" & skrot & "' and strefa='" & strefa & "'"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                            sqlexp = "insert into dp_swm_mia_wozek values('" & skrot & "','" & adres & "','" & strefa & "','" & is_active & "','" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                            result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                            dajarSWMagazyn_MIA.MyFunction.SetHistoriaTowary(skrot, adres, strefa, is_active, "OP_PO", Session("mylogin"), Session("myhash"))
                        End If
                    End If
                Next
            End If

            Dim filter_data As String = TBFiltrowanie.Text
            LadujDaneGridViewMagazyn(filter_data)
            conn.Close()
        End Using
    End Sub

    Protected Sub GridViewMagazyn_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewMagazyn.RowDataBound
        ''AktualizujGridViewMagazynStatusy()
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim cb As CheckBox = e.Row.FindControl("CBKodSelect")
            If cb IsNot Nothing And cb.Checked Then
                e.Row.BackColor = GridViewMagazyn.SelectedRowStyle.BackColor
            Else
                Dim strefa As String = e.Row.Cells(9).Text.ToString
                If strefa = "Z" Then : e.Row.BackColor = LTowarZbieranie.BackColor
                ElseIf strefa = "N" Then : e.Row.BackColor = LTowarNiepelna.BackColor
                ElseIf strefa = "P" Then : e.Row.BackColor = LTowarPaleta.BackColor
                End If
            End If
        End If
    End Sub

    Protected Sub GridViewMagazyn_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles GridViewMagazyn.SelectedIndexChanged
        Dim cb As CheckBox = GridViewMagazyn.SelectedRow.FindControl("CBKodSelect")
        If cb IsNot Nothing And cb.Checked Then
            cb.Checked = False
        Else
            cb.Checked = True
        End If
    End Sub

    Protected Sub TBFiltrowanie_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TBFiltrowanie.TextChanged
        Dim filter_data As String = TBFiltrowanie.Text
        If filter_data <> "" Then
            LadujDaneGridViewMagazyn(filter_data)
        Else
            LadujDaneGridViewMagazyn("")
        End If
    End Sub

    Protected Sub BTowarWyszukaj_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarWyszukaj.Click
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            If TBFiltrowanie.Text.ToString <> "" Then
                LadujDaneGridViewMagazyn(TBFiltrowanie.Text.ToString)
            End If
            conn.Close()
        End Using
    End Sub
End Class