Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports Oracle.ManagedDataAccess.Client
Imports Image = System.Drawing.Image

Public Class MyFunction
    Shared sqlExp As String = ""
    Public Shared activeComPort As String = "COM8"
    Public Shared simCardNumber As String = "797165884"
    Public Shared networkDirectory As String = "q:\dhl"
    ''Public Shared networkSerwerDirectory As String = "C:\inetpub\wwwroot\dajarSWMagazyn_MIA\upload"
    Public Shared networkPath As String = "\\serwermagazyny.dajar.lokalne\IIS\dajarSWMagazyn_MIA\upload"
    Public Shared networkPathEmailAttahement As String = "\\serwermagazyny.dajar.lokalne\IIS\dajarSWMagazyn_MIA\mailing"

    Public Shared networkMainPath As String = "\\serwermagazyny.dajar.lokalne\IIS\dajarSWMagazyn_MIA\"
    ''Public Shared networkSerwerMainDirectory As String = "C:\inetpub\wwwroot\dajarSWMagazyn_MIA\"

    ' ***********************************
    ' ** TO CHANGE ACTIVE CONNECTION   **
    ' ** SET IsTest TO TRUE OR FALSE   **
    ' ** True - TEST CONNECTION        **
    ' ** False - PRODUCTION CONNECTION **
    ' ***********************************
    
    ' PRODUCTION CONNECTION STRING
    Public Shared ProductionConnection As String = "Data Source=(DESCRIPTION=" _
                                                   + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.1.0.30)(PORT=1521)))" _
                                                   + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=ORCL)));" _
                                                   + "User Id=DAJAR;Password=DAJAR;"
    
    ' TEST CONNECTION STRING
    Public Shared TestConnection As String = "Data Source=(DESCRIPTION=" _
                                             + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.1.0.148)(PORT=1521)))" _
                                             + "(CONNECT_DATA=(SERVER=DEDICATED)(SID=orcl)));" _
                                             + "User Id=DAJAR;Password=DAJAR;"
    
    ' SET CONNECTION STRING
    ' TEST - True
    ' PRODUCTION - False                                         
    Private const IsTest As Boolean = True
    Public Shared ConnString As String = If(IsTest, TestConnection, ProductionConnection)

    Public Shared userEmail As String = "admin.dswm@dajar.pl"
    Public Shared passwordEmail As String = "admin2014@@##"


    Shared Sub CheckUploadDirectory(ByVal dir_IN As String)

        Dim dir_export As String = dir_IN
        If Directory.Exists(dir_export) = False Then
            Directory.CreateDirectory(dir_export)
        End If
    End Sub

    Shared Function DataEvalLogTime() As String
        Return _
            DateTime.Now.Year & "-" & DateTime.Now.Month.ToString("D2") & "-" & DateTime.Now.Day.ToString("D2") & " " &
            DateTime.Now.Hour.ToString("D2") & ":" & DateTime.Now.Minute.ToString("D2") & ":" &
            DateTime.Now.Second.ToString("D2")
    End Function

    Shared Function DataEvalSmsTime() As String
        Return _
            DateTime.Now.Year & "/" & DateTime.Now.Month.ToString("D2") & "/" & DateTime.Now.Day.ToString("D2") & " " &
            DateTime.Now.Hour.ToString("D2") & ":" & DateTime.Now.Minute.ToString("D2") & ":" &
            DateTime.Now.Second.ToString("D2")
    End Function

    Shared Function DataEval() As String
        Return _
            DateTime.Now.Year & DateTime.Now.Month.ToString("D2") & DateTime.Now.Day.ToString("D2") &
            DateTime.Now.Hour.ToString("D2") & DateTime.Now.Minute.ToString("D2") & DateTime.Now.Second.ToString("D2")
    End Function

    Shared Function ExecuteFromSqlExp(ByVal sqlexp As String, ByVal conn As OracleConnection) As Boolean
        Dim cmd As New OracleCommand(sqlexp, conn)
        Try
            cmd.CommandType = CommandType.Text
            cmd.Transaction = conn.BeginTransaction
            cmd.ExecuteNonQuery()
            cmd.Transaction.Commit()

        Catch ex As Exception
            cmd.Transaction.Rollback()
            Return False
        Finally
            cmd.Dispose()
        End Try
        Return True
    End Function

    Shared Function Convert_ISO_2_TO_3(ByVal iso_country As String) As String
        Dim iso_country_return As String = ""
        For Each ci As CultureInfo In CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            Dim inforeg As New RegionInfo(ci.LCID)
            inforeg.TwoLetterISORegionName.ToUpper()
            Dim kraj_iso As String = inforeg.TwoLetterISORegionName.ToUpper
            If kraj_iso = iso_country Then
                iso_country_return = inforeg.ThreeLetterISORegionName.ToString()

                Exit For
            End If
        Next
        Return iso_country_return
    End Function

    Shared Function ExecuteFromSqlExpTransaction(ByVal sqlexpTransaction As List(Of String),
                                                 ByVal conn As OracleConnection) As Boolean
        Dim cmd As New OracleCommand
        Dim trans As OracleTransaction
        cmd = conn.CreateCommand

        trans = conn.BeginTransaction(IsolationLevel.ReadCommitted)
        cmd.Transaction = trans

        Try
            For Each sql_obj In sqlexpTransaction
                cmd.CommandText = sql_obj
                cmd.ExecuteNonQuery()
            Next

            trans.Commit()

        Catch ex As Exception
            trans.Rollback()
            Return False
        Finally
            cmd.Dispose()
        End Try
        Return True
    End Function

    Shared Function ExecuteScalarFromSqlExp(ByVal sqlexp As String, ByVal conn As OracleConnection) As Object
        Dim cmd As New OracleCommand(sqlexp, conn)
        Try
            cmd.CommandType = CommandType.Text
            cmd.Transaction = conn.BeginTransaction
            Dim obj As Object = cmd.ExecuteScalar()
            cmd.Transaction.Commit()
            Return obj

        Catch ex As Exception
            cmd.Transaction.Rollback()
            Return - 1
        Finally
            cmd.Dispose()
        End Try
        Return 0
    End Function

    Shared Function GetStringFromSqlExp(ByVal sqlexp As String, ByVal conn As OracleConnection) As String

        Dim bufor As String = ""
        Dim oracle_cmd As New OracleCommand(sqlexp, conn)
        oracle_cmd.CommandType = CommandType.Text
        Dim oracle_dr As OracleDataReader = oracle_cmd.ExecuteReader()

        Try
            While oracle_dr.Read()
                bufor = oracle_dr.Item(0).ToString
            End While

        Catch ex As ArgumentOutOfRangeException
            Console.WriteLine(ex)
        End Try

        oracle_dr.Close()
        oracle_cmd.Dispose()

        bufor = bufor.Trim

        Return bufor
    End Function

    Shared Function SetBuforAktualizacja(ByVal nr_zamow As String, ByVal schemat As String, ByVal skrot As String) _
        As Boolean
        Dim timestamp As String = DataEvalLogTime()
        Dim sqlexp As String = ""
        Using conn As New OracleConnection(ConnString)
            conn.Open()
            sqlexp = "update dp_swm_mia_buf set dyspozycja = 'Y' where nr_zamow='" & nr_zamow & "' and schemat='" & schemat &
                     "' and skrot='" & skrot & "'"
            ''conn.Close()
            Return ExecuteFromSqlExp(sqlexp, conn)
        End Using
    End Function

    Shared Function SetMagazynDyspozycja(ByVal login As String, ByVal hash As String, ByVal nr_zamow As String,
                                         ByVal status As String, ByVal schemat As String, ByVal mag As String,
                                         ByVal skrot As String, ByVal ilosc As Integer) As Boolean
        Dim timestamp As String = DataEvalLogTime()
        Dim sqlexp As String = ""
        Using conn As New OracleConnection(ConnString)
            conn.Open()
            sqlexp = "insert into dp_swm_mia_mag (login,hash,nr_zamow,autodata,status,schemat,mag,skrot,ile_poz) values('" &
                     login & "','" & hash & "','" & nr_zamow & "',TO_TIMESTAMP('" & timestamp &
                     "', 'RR/MM/DD HH24:MI:SS'),'" & status & "', '" & schemat & "','" & mag & "','" & skrot & "'," &
                     ilosc & ")"

            ''conn.Close()
            Return ExecuteFromSqlExp(sqlexp, conn)
        End Using
    End Function

    Shared Sub ClearSessionInformation(ByRef sesja_bierzaca As HttpSessionState)
        sesja_bierzaca.Remove("mylogin")
        sesja_bierzaca.Remove("myhash")
        sesja_bierzaca.Remove("contentHash")
        sesja_bierzaca.Remove("contentOperator")
        sesja_bierzaca.Remove("contentMagazyn")
        sesja_bierzaca.Remove("contentHash")
        sesja_bierzaca.Remove("contentKomunikat")
        sesja_bierzaca.Remove("mysuper_user")
    End Sub

    Shared Sub RefreshDDLOperator(ByRef ddlObiekt As DropDownList,
                                  ByRef sesja_bierzaca As HttpSessionState)
        Using conn As New OracleConnection(ConnString)
            conn.Open()
            Dim sqlExp As String =
                    "select login from dp_swm_mia_uzyt where status = 'X' and typ_oper in('M','O','P','W','MO','MP','PM','PP','RM','ME') and blokada_konta is null order by login"
            ddlObiekt.Items.Clear()
            ddlObiekt.Items.Add("")
            Dim cmd As New OracleCommand(sqlExp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            If dr.HasRows Then
                ddlObiekt.DataSource = dr
                ddlObiekt.DataTextField = "LOGIN"
                ddlObiekt.DataValueField = "LOGIN"
                ddlObiekt.DataBind()
            End If
            dr.Close()
            cmd.Dispose()

            ddlObiekt.Items.Add(New WebControls.ListItem("WYBIERZ OPERATORA", "WYBIERZ OPERATORA", True))

            If sesja_bierzaca("mylogin") IsNot Nothing Then
                ddlObiekt.SelectedValue = sesja_bierzaca("mylogin")
            Else
                ddlObiekt.SelectedValue = "WYBIERZ OPERATORA"
            End If
            conn.Close()
        End Using
    End Sub

    Shared Function GetStringFromSqlExpParameter(ByVal sqlexp As String, ByVal conn As OracleConnection,
                                                 ByVal param As List(Of OracleParameter)) As String

        Dim bufor As String = ""
        Dim cmd As New OracleCommand(sqlexp, conn)
        cmd.CommandType = CommandType.Text
        For Each p As OracleParameter In param
            cmd.Parameters.Add(New OracleParameter(p.ParameterName, p.Value))
        Next
        Dim dr As OracleDataReader = cmd.ExecuteReader()

        Try
            While dr.Read()
                bufor += dr.Item(0).ToString & vbNewLine
            End While

        Catch ex As ArgumentOutOfRangeException
            Console.WriteLine(ex)
        End Try

        dr.Close()
        cmd.Dispose()

        bufor = bufor.Trim

        Return bufor
    End Function

    Shared Function GeneratePrintId() As String
        Using conn As New OracleConnection(ConnString)
            conn.Open()
            Dim sqlExp As String = "SELECT TO_NUMBER(NVL(MAX(recno)+1,1)) recno from dp_swm_mia_print"
            Dim generateId As String = GetStringFromSqlExp(sqlExp, conn)
            conn.Close()
            Return generateId
        End Using
    End Function

    Shared Function SetPrintDyspozycja(ByVal remote_ip As String, ByVal session_id As String, ByVal nr_zamow As String,
                                       ByVal login As String, ByVal hash As String, ByVal shipment_id As String) _
        As Boolean
        ''RECNO       Not NULL NUMBER(10)    
        ''remote_ip   Not NULL VARCHAR2(100) 
        ''nr_zamow    Not NULL VARCHAR2(12)  
        ''login        Not NULL VARCHAR2(50) 
        ''hash        Not NULL VARCHAR2(100) 
        ''SHIPMENT_ID Not NULL VARCHAR2(50)  
        ''status               Char(1) 

        Dim timestamp As String = DataEvalLogTime()
        Dim sqlexp As String = ""
        Using conn As New OracleConnection(ConnString)
            conn.Open()
            Dim recno As String = GeneratePrintId()
            sqlexp =
                "insert into dp_swm_mia_print (recno,remote_ip,session_id,nr_zamow,login,hash,shipment_id,status,autodata) values('" &
                recno & "','" & remote_ip & "','" & session_id & "','" & nr_zamow & "','" & login & "','" & hash & "','" &
                shipment_id & "','N',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"

            ''conn.Close()
            Return ExecuteFromSqlExp(sqlexp, conn)
        End Using
    End Function

    Shared Function UpdatePrintDyspozycja(ByVal recno As String, ByVal shipment_id As String, ByVal session_id As String,
                                          ByVal status As String) As Boolean

        Dim timestamp As String = DataEvalLogTime()
        Dim sqlexp As String = ""
        Using conn As New OracleConnection(ConnString)
            conn.Open()
            sqlexp = "update dp_swm_mia_print set status='" & status & "' where recno='" & recno & "' and session_id='" &
                     session_id & "'"
            ''conn.Close()
            Return ExecuteFromSqlExp(sqlexp, conn)
        End Using
    End Function


    Shared Function GetRemoteIp() As String
        Dim buf As String = HttpContext.Current.Request.ServerVariables("REMOTE_ADDR")
        Return buf
    End Function

    Shared Function GeneratePaczkaId() As String
        Using conn As New OracleConnection(ConnString)
            conn.Open()
            Dim sqlExp As String =
                    "SELECT 'PA'||LPAD(TO_NUMBER(NVL(SUBSTR(MAX(paczka_id),3,7)+1,1)),7,0)||'/'||TO_CHAR(sysdate,'YY') paczka_id from dp_swm_mia_paczka_info where paczka_id like 'PA%'||TO_CHAR(sysdate,'YY')"
            Dim generateId As String = GetStringFromSqlExp(sqlExp, conn)
            conn.Close()
            Return generateId
        End Using
    End Function

    Shared Function GenerateShipmentId() As String
        Using conn As New OracleConnection(ConnString)
            conn.Open()
            Dim sqlExp As String =
                    "SELECT 'SH'||LPAD(TO_NUMBER(NVL(SUBSTR(MAX(shipment_id),3,7)+1,1)),7,0)||'/'||TO_CHAR(sysdate,'YY') paczka_id from dp_swm_mia_paczka_info where shipment_id like 'SH%'||TO_CHAR(sysdate,'YY')"
            Dim generateId As String = GetStringFromSqlExp(sqlExp, conn)
            conn.Close()
            Return generateId
        End Using
    End Function

    Shared Function UpdateDyspozycjaHT_ZOG(ByVal nr_zamow As String, ByVal schemat As String, ByVal status As String) _
        As Boolean
        Dim timestamp As String = DataEvalLogTime()
        Dim sqlexp As String = ""

        Using conn As New OracleConnection(ConnString)
            conn.Open()
            Dim data_user As String = DateTime.Now.Year & "/" & DateTime.Now.Month.ToString("D2") & "/" &
                                      DateTime.Now.Day.ToString("D2")
            sqlexp = "update dp_swm_mia_zog set stat_u1='" & status & "',stda_u1=to_date('" & data_user &
                     "','RR/MM/DD') where nr_zamow='" & nr_zamow & "' and schemat='" & schemat & "'"
            Dim result As Boolean = ExecuteFromSqlExp(sqlexp, conn)

            Dim recno As String =
                    GetStringFromSqlExp(
                        "SELECT RECNO FROM dp_swm_mia_zog WHERE NR_ZAMOW='" & nr_zamow & "' AND SCHEMAT='" & schemat & "'",
                        conn)
            If schemat = "DAJAR" Or schemat = "DOMINUS" Then
                sqlexp = "update ht_zog set stat_u1='" & status & "',stda_u1=to_date('" & data_user &
                         "','RR/MM/DD') where recno=" & recno & " and nr_zamow='" & nr_zamow & "'"
                ''ElseIf schemat = "DOMINUS" Then
                ''    sqlexp = "update dominus.ht_zog set stat_u1='" & status & "',stda_u1=to_date('" & data_user & "','RR/MM/DD') where recno=" & recno & " and nr_zamow='" & nr_zamow & "'"
            End If

            ''conn.Close()
            Return ExecuteFromSqlExp(sqlexp, conn)
        End Using
    End Function

    Shared Function ZRELA1(ByVal nr_zamow As String, ByVal schemat As String) As String
        Dim is_deleted, ilosc, zreal, ilosc_potw, godzina_po As String
        Dim recno As String
        Dim sqlexp As String = ""
        Dim return_value As String = ""

        Using conn As New OracleConnection(ConnString)
            conn.Open()
            If schemat = "DAJAR" Or schemat = "DOMINUS" Then
                sqlexp = "SELECT RECNO FROM HT_ZOD WHERE IE$0 LIKE '" & nr_zamow & "%'"
                Dim oracle_cmd As New OracleCommand(sqlexp, conn)
                oracle_cmd.CommandType = CommandType.Text
                Dim oracle_dr As OracleDataReader = oracle_cmd.ExecuteReader()

                Try
                    While oracle_dr.Read()
                        recno = oracle_dr.Item(0).ToString
                        is_deleted =
                            GetStringFromSqlExp(
                                "SELECT IS_DELETED FROM HT_ZOD WHERE RECNO=" & recno, conn)
                        ilosc =
                            GetStringFromSqlExp(
                                "SELECT ILOSC FROM HT_ZOD WHERE RECNO=" & recno, conn)
                        zreal =
                            GetStringFromSqlExp(
                                "SELECT ZREAL FROM HT_ZOD WHERE RECNO=" & recno, conn)
                        ilosc_potw =
                            GetStringFromSqlExp(
                                "SELECT ILOSC_POTW FROM HT_ZOD WHERE RECNO=" & recno, conn)
                        godzina_po =
                            GetStringFromSqlExp(
                                "SELECT GODZINA_PO FROM HT_ZOD WHERE RECNO=" & recno, conn)
                        If is_deleted = "Y" Then
                            return_value = "Z"
                        ElseIf (ilosc = zreal) Or (godzina_po = "ZREALIZ.") Then
                            return_value = "X"
                        ElseIf ilosc <> zreal And ilosc_potw = 0 Then
                            return_value = "A"
                        ElseIf ilosc <> zreal And ilosc_potw <> 0 Then
                            return_value = " "
                        End If

                        Dim IE_2 As String = "'" & return_value & "'" &
                                             "||LPAD(INDEX_TOW,10)||LPAD(MAG,3)||TO_CHAR(TERM_DOST,'YYYYMMDD')||NR_ZAMOW"
                        sqlexp = "UPDATE HT_ZOD SET IE$2 = " & IE_2 & " WHERE RECNO=" & recno
                        Dim result As Boolean = ExecuteFromSqlExp(sqlexp, conn)
                    End While
                Catch ex As ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try

                oracle_dr.Close()
                oracle_cmd.Dispose()


                ''ElseIf schemat = "DOMINUS" Then
                ''    sqlexp = "SELECT RECNO FROM DOMINUS.HT_ZOD WHERE IE$0 LIKE '" & nr_zamow & "%'"
                ''    Dim oracle_cmd As New OracleCommand(sqlexp, conn)
                ''    oracle_cmd.CommandType = CommandType.Text
                ''    Dim oracle_dr As OracleDataReader = oracle_cmd.ExecuteReader()

                ''    Try
                ''        While oracle_dr.Read()
                ''            recno = oracle_dr.Item(0).ToString
                ''            is_deleted = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT IS_DELETED FROM DOMINUS.HT_ZOD WHERE RECNO=" & recno, conn)
                ''            ilosc = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT ILOSC FROM DOMINUS.HT_ZOD WHERE RECNO=" & recno, conn)
                ''            zreal = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT ZREAL FROM DOMINUS.HT_ZOD WHERE RECNO=" & recno, conn)
                ''            ilosc_potw = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT ILOSC_POTW FROM DOMINUS.HT_ZOD WHERE RECNO=" & recno, conn)
                ''            godzina_po = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT GODZINA_PO FROM DOMINUS.HT_ZOD WHERE RECNO=" & recno, conn)
                ''            If is_deleted = "Y" Then
                ''                return_value = "Z"
                ''            ElseIf (ilosc = zreal) Or (godzina_po = "ZREALIZ.") Then
                ''                return_value = "X"
                ''            ElseIf ilosc <> zreal And ilosc_potw = 0 Then
                ''                return_value = "A"
                ''            ElseIf ilosc <> zreal And ilosc_potw <> 0 Then
                ''                return_value = " "
                ''            End If

                ''            Dim IE_2 As String = "'" & return_value & "'" & "||LPAD(INDEX_TOW,10)||LPAD(MAG,3)||TO_CHAR(TERM_DOST,'YYYYMMDD')||NR_ZAMOW"
                ''            sqlexp = "UPDATE DOMINUS.HT_ZOD SET IE$2 = " & IE_2 & " WHERE RECNO=" & recno
                ''            Dim result As Boolean = ExecuteFromSqlExp(sqlexp, conn)
                ''        End While
                ''    Catch ex As System.ArgumentOutOfRangeException
                ''        Console.WriteLine(ex)
                ''    End Try

                ''    oracle_dr.Close()
                ''    oracle_cmd.Dispose()
            End If
            conn.Close()

        End Using

        Return " "
    End Function

    Shared Function ZRELA2(ByVal nr_zamow As String, ByVal schemat As String) As String
        Dim ilosc, zreal, godzina_po As String
        Dim recno As String
        Dim sqlexp As String = ""
        Dim return_value As String = ""

        Using conn As New OracleConnection(ConnString)
            conn.Open()
            If schemat = "DAJAR" Or schemat = "DOMINUS" Then
                sqlexp = "SELECT RECNO FROM HT_ZOD WHERE IE$0 LIKE '" & nr_zamow & "%'"
                Dim oracle_cmd As New OracleCommand(sqlexp, conn)
                oracle_cmd.CommandType = CommandType.Text
                Dim oracle_dr As OracleDataReader = oracle_cmd.ExecuteReader()

                Try
                    While oracle_dr.Read()
                        recno = oracle_dr.Item(0).ToString
                        ilosc =
                            GetStringFromSqlExp(
                                "SELECT ILOSC FROM HT_ZOD WHERE RECNO=" & recno, conn)
                        zreal =
                            GetStringFromSqlExp(
                                "SELECT ZREAL FROM HT_ZOD WHERE RECNO=" & recno, conn)
                        godzina_po =
                            GetStringFromSqlExp(
                                "SELECT GODZINA_PO FROM HT_ZOD WHERE RECNO=" & recno, conn)
                        If (ilosc = zreal) Or (godzina_po = "ZREALIZ.") Then
                            return_value = "X"
                        Else
                            return_value = " "
                        End If

                        Dim IE_4 As String = "'" & return_value & "'" &
                                             "||LPAD(MAG,3)||DESQL_GRAF.DF11_2(NR_ZAMOW)||LPAD(INDEX_TOW,10)"
                        sqlexp = "UPDATE HT_ZOD SET IE$4 = " & IE_4 & " WHERE RECNO=" & recno
                        Dim result As Boolean = ExecuteFromSqlExp(sqlexp, conn)
                    End While
                Catch ex As ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try

                oracle_dr.Close()
                oracle_cmd.Dispose()
                ''ElseIf schemat = "DOMINUS" Then
                ''    sqlexp = "SELECT RECNO FROM DOMINUS.HT_ZOD WHERE IE$0 LIKE '" & nr_zamow & "%'"
                ''    Dim oracle_cmd As New OracleCommand(sqlexp, conn)
                ''    oracle_cmd.CommandType = CommandType.Text
                ''    Dim oracle_dr As OracleDataReader = oracle_cmd.ExecuteReader()

                ''    Try
                ''        While oracle_dr.Read()
                ''            recno = oracle_dr.Item(0).ToString
                ''            ilosc = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT ILOSC FROM DOMINUS.HT_ZOD WHERE RECNO=" & recno, conn)
                ''            zreal = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT ZREAL FROM DOMINUS.HT_ZOD WHERE RECNO=" & recno, conn)
                ''            godzina_po = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp("SELECT GODZINA_PO FROM DOMINUS.HT_ZOD WHERE RECNO=" & recno, conn)
                ''            If (ilosc = zreal) Or (godzina_po = "ZREALIZ.") Then
                ''                return_value = "X"
                ''            Else
                ''                return_value = " "
                ''            End If

                ''            Dim IE_4 As String = "'" & return_value & "'" & "||LPAD(MAG,3)||DESQL_GRAF.DF11_2(NR_ZAMOW)||LPAD(INDEX_TOW,10)"
                ''            sqlexp = "UPDATE DOMINUS.HT_ZOD SET IE$4 = " & IE_4 & " WHERE RECNO=" & recno
                ''            Dim result As Boolean = ExecuteFromSqlExp(sqlexp, conn)
                ''        End While
                ''    Catch ex As System.ArgumentOutOfRangeException
                ''        Console.WriteLine(ex)
                ''    End Try

                ''    oracle_dr.Close()
                ''    oracle_cmd.Dispose()
            End If
            conn.Close()

        End Using
        Return " "
    End Function

    Shared Function AnulowanieDyspozycjaHT_ZOG(ByVal nr_zamow As String, ByVal schemat As String) As Boolean
        Dim timestamp As String = DataEvalLogTime()
        Dim sqlexp As String = ""

        Using conn As New OracleConnection(ConnString)
            conn.Open()
            Dim data_user As String = DateTime.Now.Year & "/" & DateTime.Now.Month.ToString("D2") & "/" &
                                      DateTime.Now.Day.ToString("D2")
            sqlexp = "update dp_swm_mia_zog set zrealizowa='X',sposob='W' where nr_zamow='" & nr_zamow & "' and schemat='" &
                     schemat & "'"
            Dim result As Boolean = ExecuteFromSqlExp(sqlexp, conn)

            Dim recno As String =
                    GetStringFromSqlExp(
                        "SELECT RECNO FROM dp_swm_mia_zog WHERE NR_ZAMOW='" & nr_zamow & "' AND SCHEMAT='" & schemat & "'",
                        conn)
            If schemat = "DAJAR" Or schemat = "DOMINUS" Then
                sqlexp = "update ht_zog set" _
                         & " zrealizowa='X'," _
                         & " sposob='W'," _
                         & " IE$0='X'||substr(ie$0,2,length(ie$0))," _
                         & " IE$1='X'||substr(ie$1,2,length(ie$1))," _
                         & " IE$2='X'||substr(ie$2,2,length(ie$2))," _
                         & " IE$3='X'||substr(ie$3,2,length(ie$3))," _
                         & " IE$4='X'||substr(ie$4,2,length(ie$4))," _
                         & " IE$5='X'||substr(ie$5,2,length(ie$5))," _
                         & " IE$6='X'||substr(ie$6,2,length(ie$6))," _
                         & " IE$7=substr(ie$7,1,3)||'X'||substr(ie$7,5,length(ie$7))," _
                         & " IE$8=substr(ie$8,1,3)||'X'||substr(ie$8,5,length(ie$8))," _
                         & " IE$9=substr(ie$9,1,3)||'X'||substr(ie$9,5,length(ie$9))," _
                         & " IE$10=substr(ie$10,1,3)||'X'||substr(ie$10,5,length(ie$10))," _
                         & " IE$11=substr(ie$11,1,3)||'X'||substr(ie$11,5,length(ie$11))," _
                         & " IE$12=substr(ie$12,1,3)||'X'||substr(ie$12,5,length(ie$12))," _
                         & " IE$13=substr(ie$13,1,3)||'X'||substr(ie$13,5,length(ie$13))," _
                         & " IE$15='X'," _
                         & " IE$16=substr(ie$16,1,3)||'X'" _
                         & " where recno=" & recno & " and nr_zamow='" & nr_zamow & "'"

                ''ElseIf schemat = "DOMINUS" Then
                ''    sqlexp = "update dominus.ht_zog set " _
                ''    & " zrealizowa='X'," _
                ''    & " sposob='W'," _
                ''    & " IE$0='X'||substr(ie$0,2,length(ie$0))," _
                ''    & " IE$1='X'||substr(ie$1,2,length(ie$1))," _
                ''    & " IE$2='X'||substr(ie$2,2,length(ie$2))," _
                ''    & " IE$3='X'||substr(ie$3,2,length(ie$3))," _
                ''    & " IE$4='X'||substr(ie$4,2,length(ie$4))," _
                ''    & " IE$5='X'||substr(ie$5,2,length(ie$5))," _
                ''    & " IE$6='X'||substr(ie$6,2,length(ie$6))," _
                ''    & " IE$7=substr(ie$7,1,3)||'X'||substr(ie$7,5,length(ie$7))," _
                ''    & " IE$8=substr(ie$8,1,3)||'X'||substr(ie$8,5,length(ie$8))," _
                ''    & " IE$9=substr(ie$9,1,3)||'X'||substr(ie$9,5,length(ie$9))," _
                ''    & " IE$10=substr(ie$10,1,3)||'X'||substr(ie$10,5,length(ie$10))," _
                ''    & " IE$11=substr(ie$11,1,3)||'X'||substr(ie$11,5,length(ie$11))," _
                ''    & " IE$12=substr(ie$12,1,3)||'X'||substr(ie$12,5,length(ie$12))," _
                ''    & " IE$13=substr(ie$13,1,3)||'X'||substr(ie$13,5,length(ie$13))," _
                ''    & " IE$15='X'," _
                ''    & " IE$16=substr(ie$16,1,3)||'X'" _
                ''    & " where recno=" & recno & " and nr_zamow='" & nr_zamow & "'"
            End If

            result = ExecuteFromSqlExp(sqlexp, conn)

            sqlexp = "select lpad(mag,3,'0') from dp_swm_mia_zog where nr_zamow='" & nr_zamow & "' and schemat='" & schemat &
                     "'"
            Dim magId As String = GetStringFromSqlExp(sqlexp, conn)

            If schemat = "DAJAR" Or schemat = "DOMINUS" Then
                sqlexp = "select sum(ilosc),index_tow from ht_zod where ie$0 like '" & nr_zamow &
                         "%' and is_deleted='N' and index_tow not in(33032,8836) group by index_tow"
                Dim oracle_cmd As New OracleCommand(sqlexp, conn)
                oracle_cmd.CommandType = CommandType.Text
                Dim oracle_dr As OracleDataReader = oracle_cmd.ExecuteReader()

                Try
                    While oracle_dr.Read()
                        Dim ilosc As String = oracle_dr.Item(0).ToString
                        Dim indeks As String = oracle_dr.Item(1).ToString
                        sqlexp = "update ht_rn" & magId & " set zamowienia=zamowienia-" & ilosc & " where ie$0=lpad(" &
                                 indeks & ",10)"
                        result = ExecuteFromSqlExp(sqlexp, conn)
                    End While
                Catch ex As ArgumentOutOfRangeException
                    Console.WriteLine(ex)
                End Try

                oracle_dr.Close()
                oracle_cmd.Dispose()

                sqlexp =
                    "update ht_zod zd set zd.ilosc=0,zd.ilosc_potw=0,ie$2='X'||substr(ie$2,2,length(ie$2)),ie$4='X'||substr(ie$4,2,length(ie$4)) where zd.ie$0 like '" &
                    nr_zamow & "%' and zd.is_deleted='N'"
                result = ExecuteFromSqlExp(sqlexp, conn)

                ''ElseIf schemat = "DOMINUS" Then
                ''    sqlexp = "select sum(ilosc),index_tow from dominus.ht_zod where ie$0 like '" & nr_zamow & "%' and is_deleted='N' and index_tow not in(33032,8836) group by index_tow"
                ''    Dim oracle_cmd As New OracleCommand(sqlexp, conn)
                ''    oracle_cmd.CommandType = CommandType.Text
                ''    Dim oracle_dr As OracleDataReader = oracle_cmd.ExecuteReader()

                ''    Try
                ''        While oracle_dr.Read()
                ''            Dim ilosc As String = oracle_dr.Item(0).ToString
                ''            Dim indeks As String = oracle_dr.Item(1).ToString
                ''            If magId = "043" Then
                ''                sqlexp = "update dominus.ht_rejna set zamowienia=zamowienia-" & ilosc & " where ie$0=lpad(" & indeks & ",10)"
                ''            ElseIf magId = "046" Then
                ''                sqlexp = "update dominus.ht_rn" & magId & " set zamowienia=zamowienia-" & ilosc & " where ie$0=lpad(" & indeks & ",10)"
                ''            End If
                ''            result = ExecuteFromSqlExp(sqlexp, conn)
                ''        End While
                ''    Catch ex As System.ArgumentOutOfRangeException
                ''        Console.WriteLine(ex)
                ''    End Try

                ''    oracle_dr.Close()
                ''    oracle_cmd.Dispose()

                ''    sqlexp = "update dominus.ht_zod zd set zd.ilosc=0,zd.ilosc_potw=0,ie$2='X'||substr(ie$2,2,length(ie$2)),ie$4='X'||substr(ie$4,2,length(ie$4)) where zd.ie$0 like '" & nr_zamow & "%' and zd.is_deleted='N'"
                ''    result = ExecuteFromSqlExp(sqlexp, conn)
            End If
            conn.Close()

            Return True
        End Using
    End Function

    Shared Function SetOperacjeStatus(ByVal login As String, ByVal hash As String, ByVal typ_oper As String, ByVal status As String) As Boolean
        Dim result As Boolean = False
        Dim timestamp As String = DataEvalLogTime()
        Dim sqlexp As String = ""
        Using conn As New OracleConnection(ConnString)
            conn.Open()
            sqlexp = "INSERT INTO dp_swm_mia_oper VALUES('" & login & "','" & hash & "','" & typ_oper & "',TO_TIMESTAMP('" &
                     timestamp & "', 'RR/MM/DD HH24:MI:SS'),'" & status & "')"
            result = ExecuteFromSqlExp(sqlexp, conn)
            conn.Close()
            Return result

        End Using
    End Function

    Shared Function SetHistoriaTowary(ByVal skrot As String, ByVal adres As String, ByVal strefa As String, ByVal is_active As String, ByVal oper As String, ByVal login As String, ByVal hash As String) As Boolean
        Dim result As Boolean = False
        Dim timestamp As String = DataEvalLogTime()
        Using conn As New OracleConnection(ConnString)
            conn.Open()
            Dim sqlExp As String = "insert into dp_swm_mia_towary_hist values('" & skrot & "','" & adres & "','" & strefa &
                                   "','" & is_active & "','" & oper & "','" & login & "','" & hash & "',TO_TIMESTAMP('" &
                                   timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
            result = ExecuteFromSqlExp(sqlExp, conn)
            conn.Close()
            Return result
        End Using
    End Function

    Shared Function SetSmsInformationStatus(ByVal login As String, ByVal hash As String, ByVal telefon As String,
                                            ByVal message As String) As Boolean
        Dim timestamp As String = DataEvalLogTime()
        Dim sqlexp As String = ""
        Using conn As New OracleConnection(ConnString)
            conn.Open()
            Dim hashInput As String = login & telefon & Date.Now.ToString
            Dim id_sms As String = HashMd5.getMd5Hash(hashInput)

            sqlexp = "INSERT INTO dp_swm_mia_SMS_INFO (ID_SMS,LOGIN,HASH,MESSAGE,TELEFON,STATUS,DATA) VALUES('" & id_sms &
                     "','" & login & "','" & hash & "','" & message & "','" & telefon & "','W',TO_TIMESTAMP('" &
                     timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
            conn.Close()
            Return ExecuteFromSqlExp(sqlexp, conn)
        End Using
    End Function

    Shared Function GetPageNameFromUrl(ByVal request_url As String) As String
        Dim result As String = ""
        Dim tmp() As String = request_url.Split("/")
        If tmp(tmp.Length - 1).Contains(".aspx") Then
            result = tmp(tmp.Length - 1).Replace(".aspx", "")
        End If
        Return result
    End Function


    Shared Function ReplacePageUrl(ByVal page_name As String, ByVal request_url As String) As String
        Dim tmp() As String = request_url.Split("/")
        Dim result As String = ""
        For i = 0 To tmp.Length - 2
            result &= tmp(i).ToString & "/"
        Next

        result &= page_name
        Return result
    End Function

    Shared Sub Session_Remove(ByVal variable As List(Of String),
                              ByRef sesja_bierzaca As HttpSessionState)
        For Each sv In variable
            sesja_bierzaca.Remove(sv)
        Next
    End Sub

    Shared Function SprawdzCzyIstniejeZalacznik(ByVal katalog As String, ByVal nazwaPliku As String,
                                                ByVal myDevice As String) As String
        '####sprawdzenie za jakiego urzadzenia jest generowana informacja####
        Dim fileSciezkaDostepu As String = ""

        If myDevice = "ipad" Then _
            : fileSciezkaDostepu = networkPath & "/" & katalog & "/" & nazwaPliku
        ElseIf myDevice = "standard" Then _
            : fileSciezkaDostepu = networkDirectory & "/" & katalog & "/" & nazwaPliku
        End If

        If File.Exists(fileSciezkaDostepu) Then : Return nazwaPliku
        Else : Return ""
        End If
    End Function

    Shared Sub __PrepareToSendEmailMessage(ByVal email_adres As List(Of String), ByVal email_title As String,
                                           ByVal email_template As String, ByVal form_field As List(Of String),
                                           ByVal sqlexp As String, ByVal dokument As String, ByVal schemat As String,
                                           ByVal osoba_generujaca As String, ByVal conn As OracleConnection,
                                           ByVal sesja_bierzaca As HttpSessionState)
        Dim FileAttach As New List(Of String)
        Dim strMessage As String = ""

        '#######tresc wiadomosci start
        Dim _
            fileEmailContent As _
                New StreamReader(networkPathEmailAttahement & "/" & email_template,
                                 Encoding.GetEncoding(1250))
        Dim bufor As String = fileEmailContent.ReadToEnd
        fileEmailContent.Close()

        Dim cmd As New OracleCommand(sqlexp, conn)
        cmd.CommandType = CommandType.Text
        Dim dr As OracleDataReader = cmd.ExecuteReader()

        Try
            While dr.Read()
                For i = 0 To form_field.Count - 1
                    Dim element As String = form_field.Item(i).ToString
                    If dr.IsDBNull(i) = False Then : bufor = bufor.Replace(element, dr.Item(i).ToString)
                    Else : bufor = bufor.Replace(element, "brak")
                    End If
                Next
            End While

        Catch ex As ArgumentOutOfRangeException
            Console.WriteLine(ex)
        End Try

        dr.Close()
        cmd.Dispose()

        If schemat = "DAJAR" Or schemat = "DOMINUS" Then
            sqlexp = "select rownum as lp, w.skrot, get_index_tow(w.skrot) indeks, desql_japa_nwa.fsql_japa_rnaz(get_index_tow(w.skrot)) nazwa, " _
                     & " desql_japa_nwa.fsql_japa_rkod(get_index_tow(w.skrot)) kod_tow, w.ile_poz, w.jm from (" _
                     & " select dm.login,dm.hash,dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
                     & " to_char(zg.data_fakt, 'YYYY/MM/DD') ||' '|| zg.godzina data_zam, 'M' typ_oper," _
                     & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag," _
                     &
                     " (select distinct zd.jm from ht_zod zd where zd.ie$0 like dm.nr_zamow||'%' and zd.is_deleted = 'N' and zd.skrot = dm.skrot) jm" _
                     & " from dp_swm_mia_mag dm, ht_zog zg where dm.login = 'm_brzezinski' and dm.status in('MG')" _
                     & " and dm.schemat = 'DAJAR'" _
                     & " and zg.ie$14 = desql_graf.df11_2(dm.nr_zamow) order by dm.nr_zamow" _
                     & " ) w where w.nr_zamow='" & dokument & "'"
            ''ElseIf schemat = "DOMINUS" Then
            ''    sqlexp = "select rownum as lp, w.skrot, dominus.get_index_tow(w.skrot) indeks, desql_japa_nwa.fsql_japa_rnaz(dominus.get_index_tow(w.skrot)) nazwa, " _
            ''    & " desql_japa_nwa.fsql_japa_rkod(dominus.get_index_tow(w.skrot)) kod_tow, w.ile_poz, w.jm from (" _
            ''    & " select dm.login,dm.hash,dm.nr_zamow,dm.schemat,zg.nr_zamow_o, dm.skrot, dm.ile_poz, " _
            ''    & " to_char(zg.data_fakt, 'YYYY/MM/DD') ||' '|| zg.godzina data_zam, 'M' typ_oper," _
            ''    & " to_char(dm.autodata,'YYYY/MM/DD HH24:MI:SS') autodata, dm.status, dm.mag," _
            ''    & " (select distinct zd.jm from dominus.ht_zod zd where zd.ie$0 like dm.nr_zamow||'%' and zd.is_deleted = 'N' and zd.index_tow = dominus.get_index_tow(dm.skrot)) jm" _
            ''    & " from dp_swm_mia_mag dm, dominus.ht_zog zg where dm.login = 'k_stanczyk' and dm.status in('MG')" _
            ''    & " and dm.schemat = 'DOMINUS'" _
            ''    & " and zg.ie$14 = desql_graf.df11_2(dm.nr_zamow) order by dm.nr_zamow" _
            ''    & " ) w where w.nr_zamow='" & dokument & "'"

        End If
        Dim listaArtykulow As String = GetStringFromSqlExp(sqlexp, conn)
        bufor = bufor.Replace("(LISTA_ARTYKULOW)", listaArtykulow)

        sqlexp = "select TO_CHAR(max(autodata),'YYYY/MM/DD HH24:MM:SS') from dp_swm_mia_mag where nr_zamow='" & dokument &
                 "' and login='" & osoba_generujaca & "' and status in('PA')"
        Dim autodata As String = GetStringFromSqlExp(sqlexp, conn)
        bufor = bufor.Replace("(AUTODATA)", autodata)

        bufor = bufor.Replace("(OPERATOR)", osoba_generujaca)

        bufor = bufor.Replace("ą", "±")
        bufor = bufor.Replace("ś", "¶")
        bufor = bufor.Replace("Ą", "ˇ")
        bufor = bufor.Replace("Ś", "¦")

        strMessage = bufor
        '#######tresc wiadomosci koniec

        '###################################################################DODAWANIE ZALACZNIKA DO WIADOMOSCI################################################################
        ''Dim fileDirectory As String = ""
        ''Dim katalogBierzacy As String = ""

        ''Dim formatPliku As String() = {".pdf", ".jpg"}
        ''For Each f_pliku In formatPliku
        ''    fileDirectory = dajarSWMagazyn_MIA.MyFunction.SprawdzCzyIstniejeZalacznik(katalogBierzacy, dokument.ToString.Replace("/", "_") & "_0" & f_pliku, sesja_bierzaca("myDevice"))
        ''    For kk = 0 To 10
        ''        Dim tmpfileDirectory As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/" & katalogBierzacy & "/" & fileDirectory.Replace("_0" & f_pliku, "_" & kk & f_pliku)
        ''        If File.Exists(tmpfileDirectory) Then : FileAttach.Add(tmpfileDirectory)
        ''        End If
        ''    Next

        ''    fileDirectory = dajarSWMagazyn_MIA.MyFunction.networkPath & "/" & katalogBierzacy & "/" & "nota_" & dokument.ToString.Replace("/", "_") & "_DK" & f_pliku
        ''    If File.Exists(fileDirectory) Then FileAttach.Add(fileDirectory)

        ''    fileDirectory = dajarSWMagazyn_MIA.MyFunction.networkPath & "/" & katalogBierzacy & "/" & "nota_" & dokument.ToString.Replace("/", "_") & f_pliku
        ''    If File.Exists(fileDirectory) Then FileAttach.Add(fileDirectory)
        ''Next
        '###################################################################DODAWANIE ZALACZNIKA DO WIADOMOSCI################################################################
        SendMail.SendEmailMessageMulti(userEmail, email_adres, email_title,
                                                      strMessage, FileAttach, userEmail,
                                                      passwordEmail)
    End Sub

    Shared Function CONVERT_STR_TO_BASE64(ByVal str As String) As String
        Dim base64Decoded As String = str
        Dim data As Byte()
        data = ASCIIEncoding.ASCII.GetBytes(base64Decoded)
        Dim base64Encoded As String = Convert.ToBase64String(data)
        Return base64Encoded
    End Function

    Shared Sub SetPanelObjectValue(ByVal table_object As String(), ByVal page_object As Page,
                                   ByVal state_object As Boolean)
        For Each t_obj In table_object
            Dim t_panel As Panel = DirectCast(page_object.FindControl(t_obj), Panel)
            t_panel.Visible = state_object
        Next
    End Sub

    Shared Sub IMAGE_CONVERT_GIF_to_PDF(ByVal gif_source As String, ByVal pdf_source As String)
        Try
            If File.Exists(gif_source) Then
                ' Tworzenie dokumentu PDF
                Dim document As New Document()
                PdfWriter.GetInstance(document, New FileStream(pdf_source, FileMode.Create))
                document.Open()

                Dim lm As Integer = 75
                Dim rm As Integer = 0
                Dim bm As Integer = 250
                Dim tm As Integer = 0
                ' Załaduj obraz GIF
                Dim gifImage As Image = Image.FromFile(gif_source)
                Dim dimension As New FrameDimension(gifImage.FrameDimensionsList(0))
                Dim frameCount As Integer = gifImage.GetFrameCount(dimension)

                For i As Integer = 0 To frameCount - 1
                    gifImage.SelectActiveFrame(dimension, i)
                    Using stream As New MemoryStream()
                        gifImage.Save(stream, ImageFormat.Png)
                        Dim pngImage As iTextSharp.text.Image = iTextSharp.text.Image.GetInstance(stream.ToArray())
                        ''pngImage.Alignment = ImageAlign.Middle
                        pngImage.RotationDegrees = 270
                        ''pngImage.ScalePercent(56)
                        ''pngImage.SetAbsolutePosition(lm, bm)
                        ''pngImage.ScaleToFit(document.PageSize.Width - (lm + rm), document.PageSize.Height - (bm + tm))

                        pngImage.ScaleToFit(document.PageSize)
                        pngImage.SetAbsolutePosition(Math.Abs(document.PageSize.Width - pngImage.ScaledWidth)/2,
                                                     - Math.Abs(document.PageSize.Height - pngImage.Height))
                        document.Add(pngImage)
                        document.NewPage()
                    End Using
                Next
                document.Close()
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message.ToString)
        End Try
    End Sub
End Class