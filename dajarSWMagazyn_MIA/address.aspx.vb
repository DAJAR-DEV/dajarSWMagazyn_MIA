Imports Oracle.ManagedDataAccess.Client
Imports System.Data

Partial Public Class address
    Inherits System.Web.UI.Page
    Dim sqlexp As String = ""
    Dim result As Boolean = False
    Dim daPartie As OracleDataAdapter
    Dim dsPartie As DataSet
    Dim cb As OracleCommandBuilder

    Class ZamowieniaInformacje
        Public nr_zamow As String = ""
        Public schemat As String = ""

        Public Sub New(ByVal _nr_zamow As String, ByVal _schemat As String)
            nr_zamow = _nr_zamow
            schemat = _schemat
        End Sub
    End Class

    Protected Sub GridViewMagazyn_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles GridViewMagazyn.PageIndexChanging
        Dim session_id As String = "contentKomunikat_" & dajarSWMagazyn_MIA.MyFunction.GetPageNameFromUrl(Request.Url.AbsoluteUri)
        Session.Remove(session_id)
        GridViewMagazyn.PageIndex = e.NewPageIndex
        GridViewMagazyn.DataBind()
        LadujDaneGridViewMagazyn("")
    End Sub

    Protected Sub CBFiltrowanieBraki_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBFiltrowanieBraki.CheckedChanged
        If CBFiltrowanieBraki.Checked Then
            Session("filter") = " and dt.is_active = 0"
        Else
            Session("filter") = " and 1=1"
        End If

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

        BTowarBrak.Attributes.Add("onclick", "javascrip:if(confirm ('Czy na pewno chcesz zglosic brak wybranych artykulow?\n') == false) return false")

        If Session("mylogin") = Nothing And Session("myhash") = Nothing Then
            Session.Abandon()
            Response.Redirect("index.aspx")
        ElseIf Session("mytyp_oper") <> "M" And Session("mytyp_oper") <> "W" And Session("mytyp_oper") <> "O" And Session("mytyp_oper") <> "MO" And Session("mytyp_oper") <> "MP" And Session("mytyp_oper") <> "ME" Then
            Session(session_id) = "<div style=""text-align:center; font-weight:bold; height:75px; background-color:Red; color:White;"">"
            Session(session_id) += "<br />Zaloguj sie na opowiedniego operatora - typ MAGAZYN/WOZEK/OGROD/MAGAZYN-OGROD/MAGAZYN-PODUSZKA<br />"
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
            Panel_oper_magazyn.Visible = False
            Panel_oper_magazyn_dane.Visible = False
            Panel_oper_wozek.Visible = False
            TRPrzemagazywanieWlasne.Visible = False

            dajarSWMagazyn_MIA.MyFunction.RefreshDDLOperator(DDLOperator, Session)
            If Session("mytyp_oper") = "M" Or Session("mytyp_oper") = "O" Or Session("mytyp_oper") = "MO" Or Session("mytyp_oper") = "MP" Or Session("mytyp_oper") = "ME" Then
                Panel_oper_magazyn.Visible = True
                Panel_oper_magazyn_dane.Visible = True
            ElseIf Session("mytyp_oper") = "W" Then
                Panel_oper_wozek.Visible = True
            End If

            If Session("view_przemagazynowanie_wlasne") = True Then
                TRPrzemagazywanieWlasne.Visible = True
            End If

            If Session("view_automagazynowanie") = True Then
                TRAutomagazynowanie.Visible = True
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

            If Session("mytyp_oper") = "M" Or Session("mytyp_oper") = "O" Or Session("mytyp_oper") = "MO" Or Session("mytyp_oper") = "MP" Or Session("mytyp_oper") = "ME" Then
                If filtr <> "" Then
                    sqlexp = "select dt.skrot,r.nazdop,r.jm,r.kod_tow,r.skt2_0,r.klasa,dt.adres,dt.strefa,dt.is_active,desql_japu_nwa.fsql_japu_wolne_MAG(get_index_tow(dt.skrot),'700') stan_700 from dp_swm_mia_towary dt, ht_rejna r where dt.skrot = r.nazpot and r.is_deleted='N' and dt.strefa in('Z') and (dt.skrot like '" & filtr & "%' or dt.adres like '" & filtr & "%') " & Session("filter") & " order by dt.skrot"
                Else
                    sqlexp = "select dt.skrot,r.nazdop,r.jm,r.kod_tow,r.skt2_0,r.klasa,dt.adres,dt.strefa,dt.is_active,desql_japu_nwa.fsql_japu_wolne_MAG(get_index_tow(dt.skrot),'700') stan_700 from dp_swm_mia_towary dt, ht_rejna r where dt.skrot = r.nazpot and r.is_deleted='N' and dt.strefa in('Z') " & Session("filter") & " order by dt.skrot"
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
                    Panel_oper_magazyn_dane.Visible = False
                Else
                    LIleDokumentow.Text = dsPartie.Tables(0).Rows.Count.ToString
                    Panel_oper_magazyn_dane.Visible = True
                End If
            ElseIf Session("mytyp_oper") = "W" Then
                ''If filtr <> "" Then
                ''    sqlexp = "select dt.skrot,r.nazdop,r.jm,r.kod_tow,r.skt2_0,r.klasa,dt.adres,dt.strefa,dt.oper,dt.is_active,TO_CHAR(dt.autodata, 'YYYY/MM/DD HH24:MI:SS') autodata from dp_swm_mia_towary_hist dt,ht_rejna r where dt.skrot = r.nazpot and r.is_deleted='N' and (dt.skrot like '" & filtr & "%' or dt.adres like '" & filtr & "%' or r.nazdop like '%" & filtr & "%' or dt.oper like '" & filtr & "') " & Session("filter") & " and login='" & Session("mylogin") & "' order by dt.autodata desc, dt.skrot asc"
                ''Else
                ''    sqlexp = "select dt.skrot,r.nazdop,r.jm,r.kod_tow,r.skt2_0,r.klasa,dt.adres,dt.strefa,dt.oper,dt.is_active,TO_CHAR(dt.autodata, 'YYYY/MM/DD HH24:MI:SS') autodata from dp_swm_mia_towary_hist dt,ht_rejna r where dt.skrot = r.nazpot and r.is_deleted='N' " & Session("filter") & " and login='" & Session("mylogin") & "' order by dt.autodata desc, dt.skrot asc"
                ''End If
            End If
            conn.Close()
        End Using
    End Sub

    Protected Sub BTowarBrak_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarBrak.Click
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()

            If GridViewMagazyn.Rows.Count > 0 Then
                For Each row As GridViewRow In GridViewMagazyn.Rows
                    Dim cb As CheckBox = row.FindControl("CBKodSelect")
                    If cb IsNot Nothing And cb.Checked Then
                        Dim skrot As String = row.Cells(2).Text.ToString
                        Dim adres As String = row.Cells(8).Text.ToString
                        Dim strefa As String = row.Cells(9).Text.ToString
                        Dim oper As String = "OP_BR"
                        Dim is_active As String = row.Cells(10).Text.ToString
                        is_active = "0"
                        ''If is_active = "1" Then : is_active = "0"
                        ''Else : is_active = "1"
                        ''End If

                        Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                        sqlexp = "update dp_swm_mia_towary set is_active='" & is_active & "' where skrot='" & skrot & "' and adres='" & adres & "' and strefa='" & strefa & "'"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)

                        sqlexp = "insert into dp_swm_mia_towary_hist values('" & skrot & "','" & adres & "','" & strefa & "','" & is_active & "','" & oper & "','" & Session("mylogin") & "','" & Session("myhash") & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                        result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                    End If
                Next
            End If

            Dim filter_data As String = TBFiltrowanie.Text
            LadujDaneGridViewMagazyn(filter_data)
            conn.Close()
        End Using
    End Sub

    Protected Sub BTowarWymagazynowanie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarWymagazynowanie.Click
        Response.Redirect("address_wymag.aspx")
    End Sub

    Protected Sub BTowarPrzyjecieReczne_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarPrzyjecieReczne.Click
        Response.Redirect("address_reczne.aspx")
    End Sub

    Protected Sub BTowarPrzyjecieMM_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarPrzyjecieMM.Click
        Response.Redirect("address_przyjecie_mm.aspx")
    End Sub

    Protected Sub BTowarPrzemagazywananieWlasne_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarPrzemagazywananieMenu.Click
        Session("view_przemagazynowanie_wlasne") = True
        Response.Redirect("address.aspx")
    End Sub

    Protected Sub BTowarAutomagazynowanieMenu_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarAutomagazynowanieMenu.Click
        Session("view_automagazynowanie") = True
        Response.Redirect("address.aspx")
    End Sub

    Protected Sub BTowarAutomagazynowanie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarAutomagazynowanie.Click
        Response.Redirect("address_auto.aspx")
    End Sub

    Protected Sub BTowarAutomagazynowanieBraki_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarAutomagazynowanieBraki.Click
        Response.Redirect("address_auto_braki.aspx")
    End Sub

    Protected Sub BTowarPobranie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarPobranie.Click
        Response.Redirect("address_pobranie.aspx")
    End Sub

    Protected Sub BTowarZlozenie_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BTowarZlozenie.Click
        Response.Redirect("address_zlozenie.aspx")
    End Sub

    Protected Sub GridViewMagazyn_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridViewMagazyn.RowDataBound
        If Session("mytyp_oper") = "M" Or Session("mytyp_oper") = "O" Or Session("mytyp_oper") = "MO" Or Session("mytyp_oper") = "MP" Or Session("mytyp_oper") = "ME" Then
            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim cb As CheckBox = e.Row.FindControl("CBKodSelect")
                If cb IsNot Nothing And cb.Checked Then
                    e.Row.BackColor = GridViewMagazyn.SelectedRowStyle.BackColor
                Else
                    Dim is_active As String = e.Row.Cells(10).Text.ToString
                    If is_active = "1" Then : e.Row.BackColor = LTowarAktywny.BackColor
                    ElseIf is_active = "0" Then : e.Row.BackColor = LTowarNieaktywny.BackColor
                    End If
                End If
            End If
        ElseIf Session("mytyp_oper") = "W" Then
            If e.Row.RowType = DataControlRowType.DataRow Then
                Dim cb As CheckBox = e.Row.FindControl("CBKodSelect")
                If cb IsNot Nothing And cb.Checked Then
                    e.Row.BackColor = GridViewMagazyn.SelectedRowStyle.BackColor
                Else
                    Dim is_active As String = e.Row.Cells(11).Text.ToString
                    If is_active = "1" Then : e.Row.BackColor = LTowarAktywny.BackColor
                    ElseIf is_active = "0" Then : e.Row.BackColor = LTowarNieaktywny.BackColor
                    End If
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
End Class