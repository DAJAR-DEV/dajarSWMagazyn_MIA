Imports System.Net
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Text
Imports Oracle.ManagedDataAccess.Client

Public Class DhlParcelDeRestAPI

    Public Shared j_return As String = ""
    Public Shared url_desc As String = ""
    Public Shared j_string As String = ""
    Public Shared j_data As Byte()
    Public Shared sqlexp As String = ""

    Public Shared Sub SetBasicAuthHeader(ByVal request As HttpWebRequest, ByVal auth_type As String, ByVal hash As [String], ByVal accept As String)
        ''request.AllowAutoRedirect = False
        ''request.PreAuthenticate = True
        ''request.KeepAlive = True
        request.Accept = accept
        ''request.ContentType = accept
        request.Headers.Add("Authorization", auth_type & hash)
        request.Headers.Add("dhl-api-key", dhl_parcel_de_package.dhl_api_key)
    End Sub

    Public Shared Function JSON_ValidateDateISO_Timestamp(ByVal str As String) As String
        Dim d As DateTime = str
        Dim ret_d As String = d.ToString("yy/MM/dd HH:mm:ss")
        Return ret_d.Replace("-", "/")
    End Function

    Public Shared Function JSON_ValidateToken(ByVal str As String) As String
        Return str.ToString.Replace("""", "").Replace("\", """").Replace("'", "").Trim
    End Function

    Public Shared Function JSON_SelectToken(ByVal j_return As String, ByVal token_name As String) As String
        If j_return <> "" And j_return <> "null" Then
            Dim j_rss As JObject = JObject.Parse(j_return)
            If j_rss.HasValues = True Then
                If j_rss.SelectToken(token_name) IsNot Nothing Then
                    Return j_rss.SelectToken(token_name).ToString().Replace("""", "").Replace("'", "")
                End If
            End If
        End If
        Return ""
    End Function

    Public Shared Function JSON_ParseToken(ByVal j_return As String, ByVal token_name As String) As String
        If j_return <> "" And j_return <> "null" Then
            Dim j_rss As JObject = JObject.Parse(j_return)
            If j_rss.HasValues = True Then
                If j_rss.SelectToken(token_name) IsNot Nothing Then
                    Return j_rss.SelectToken(token_name).ToString()
                End If
            End If
        End If
        Return ""
    End Function

    Public Shared Function JSON_SelectObject(ByVal j_return As String, ByVal token_name As String) As JObject
        Dim j_rss As JObject = JObject.Parse(j_return)
        Return j_rss.SelectToken(token_name)
    End Function

    Public Shared Function JSON_SelectArray(ByVal j_return As String, ByVal token_name As String) As JArray
        Dim j_rss As JObject = JObject.Parse(j_return)
        Return j_rss.SelectToken(token_name)
    End Function

    Public Shared Function REST_InsertIntoLogTable(ByVal url As String, ByVal store_view As String, ByVal oper As String, ByVal request As String, ByVal response As String, ByVal status_rest As String) As Boolean
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()

            request = ValidateTableField(request, 3999, False)
            response = ValidateTableField(response, 3999, False)

            sqlexp = "INSERT INTO DP_REST_MAG_oper VALUES('" & url & "','" & store_view & "','" & oper & "','" & request & "','" & response & "','" & status_rest & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
            Dim result As Boolean = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
            conn.Close()
            Return result
        End Using
    End Function

    Public Shared Function ValidateTableField(ByVal value As String, ByVal size As Integer, ByVal if_upper As Boolean) As String
        value = value.Replace("'", "")
        If value.Length >= size Then value = value.Substring(0, size)
        If if_upper = True Then value = value.ToUpper
        value = value.Trim
        Return value
    End Function

    Public Shared Function json_array_to_list(ByVal array As JArray) As List(Of String)
        Dim j_array As JArray = array
        Dim json_list As New List(Of String)
        For Each t As JToken In j_array
            json_list.Add(t.ToString)
        Next
        Return json_list
    End Function

    Public Shared Function json_element_in_list(ByVal list As List(Of String), ByVal element As String) As Boolean
        For Each a In list
            If a.Contains(element) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Shared Function SendRequest_GET(ByVal uri As String, ByVal hash As String, ByVal jsonSTR As String, ByVal authorization_type As String, ByVal contentType As String, ByVal accept As String) As String
        Console.WriteLine("SendRequest_GET")
        Console.WriteLine("URL={0} {1}DATA={2} {3}CONTENT_TYPE={4}", uri, vbNewLine, jsonSTR, vbNewLine, contentType)

        Dim res As String = ""
        Dim req As WebRequest = WebRequest.Create(uri)
        SetBasicAuthHeader(req, authorization_type, hash, accept)
        req.ContentType = contentType
        req.Method = "GET"


        Dim jsonDataBytes As Byte()
        If jsonSTR <> "" Then
            jsonDataBytes = Encoding.UTF8.GetBytes(jsonSTR)

            If jsonDataBytes IsNot Nothing Then
                req.ContentLength = jsonDataBytes.Length
                Dim stream = req.GetRequestStream()
                stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
                stream.Close()
            End If
        End If

        Try
            Dim response = req.GetResponse().GetResponseStream()
            Dim reader As New StreamReader(response)
            res = reader.ReadToEnd()
            reader.Close()
            response.Close()
        Catch ex As System.Net.WebException
            Dim sResponse As String = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd
            ''Console.WriteLine("RETURN=" & sResponse)
            res = sResponse
        End Try

        Console.WriteLine("RETURN=" & res)
        Return res
    End Function

    Public Shared Function SavePdfFileSendRequest_GET(ByVal uri As String, ByVal hash As String, ByVal jsonSTR As String, ByVal uriData As String, ByVal contentType As String, ByVal accept As String, ByVal filename As String) As String
        Console.WriteLine("SavePdfFileSendRequest_GET")
        Console.WriteLine("URL={0} {1}DATA={2} {3}CONTENT_TYPE={4}", uri, vbNewLine, jsonSTR, vbNewLine, contentType)
        Dim req As WebRequest = WebRequest.Create(uri)
        Dim res As String = ""
        SetBasicAuthHeader(req, "Basic ", hash, accept)
        req.ContentType = contentType
        req.Method = "GET"

        Dim jsonDataBytes As Byte()
        If jsonSTR <> "" Then
            jsonDataBytes = Encoding.UTF8.GetBytes(jsonSTR)

            If jsonDataBytes IsNot Nothing Then
                req.ContentLength = jsonDataBytes.Length
                Dim stream = req.GetRequestStream()
                stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
                stream.Close()
            End If
        End If

        Try
            Dim response = req.GetResponse().GetResponseStream()
            Dim fileStream As New FileStream(filename, FileMode.Create, FileAccess.Write)
            response.CopyTo(fileStream)
            fileStream.Close()
            ''Dim reader As New StreamReader(response)
            ''res = reader.ReadToEnd()
            ''reader.Close()
            response.Close()
            Return "1"
        Catch ex As System.Net.WebException
            Dim sResponse As String = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd
            ''Console.WriteLine("RETURN=" & sResponse)
            res = sResponse
        End Try

        Console.WriteLine("RETURN=" & res)
        Return res
    End Function

    Public Shared Function SendRequest_POST(ByVal uri As String, ByVal hash As String, ByVal jsonSTR As String, ByVal authorization_type As String, ByVal contentType As String, ByVal accept As String) As String
        Console.WriteLine("SendRequest_POST")
        Console.WriteLine("URL={0} {1}DATA={2} {3}CONTENT_TYPE={4}", uri, vbNewLine, jsonSTR, vbNewLine, contentType)

        Dim res As String = ""
        Dim req As WebRequest = WebRequest.Create(uri)
        SetBasicAuthHeader(req, authorization_type, hash, accept)
        req.ContentType = contentType
        req.Method = "POST"

        Dim jsonDataBytes As Byte()

        If jsonSTR <> "" Then
            jsonDataBytes = Encoding.UTF8.GetBytes(jsonSTR)

            If jsonDataBytes IsNot Nothing Then
                req.ContentLength = jsonDataBytes.Length
                Dim stream = req.GetRequestStream()
                stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
                stream.Close()
            End If
        End If

        Try
            Dim response = req.GetResponse().GetResponseStream()
            Dim reader As New StreamReader(response)
            res = reader.ReadToEnd()
            reader.Close()
            response.Close()
        Catch ex As System.Net.WebException
            Dim sResponse As String = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd
            ''Console.WriteLine("RETURN=" & sResponse)
            res = sResponse
        End Try

        ''Dim filein As New StreamWriter(fileout, True, System.Text.Encoding.GetEncoding(65001))
        ''filein.Write(res)
        ''filein.Close()

        Console.WriteLine("RETURN=" & res)
        Return res
    End Function

    Public Shared Function SendRequest_PUT(ByVal uri As String, ByVal hash As String, ByVal jsonSTR As String, ByVal authorization_type As String, ByVal contentType As String, ByVal accept As String) As String
        Console.WriteLine("SendRequest_PUT")
        Console.WriteLine("URL={0} {1}DATA={2} {3}CONTENT_TYPE={4}", uri, vbNewLine, jsonSTR, vbNewLine, contentType)

        Dim res As String = ""
        Dim req As WebRequest = WebRequest.Create(uri)
        SetBasicAuthHeader(req, authorization_type, hash, accept)
        req.ContentType = contentType
        req.Method = "PUT"

        Dim jsonDataBytes As Byte()

        If jsonSTR <> "" Then
            jsonDataBytes = Encoding.UTF8.GetBytes(jsonSTR)

            If jsonDataBytes IsNot Nothing Then
                req.ContentLength = jsonDataBytes.Length
                Dim stream = req.GetRequestStream()
                stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
                stream.Close()
            End If
        End If

        Try
            Dim response = req.GetResponse().GetResponseStream()
            Dim reader As New StreamReader(response)
            res = reader.ReadToEnd()
            reader.Close()
            response.Close()
        Catch ex As System.Net.WebException
            Dim sResponse As String = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd
            ''Console.WriteLine("RETURN=" & sResponse)
            res = sResponse
        End Try

        ''Dim filein As New StreamWriter(fileout, True, System.Text.Encoding.GetEncoding(65001))
        ''filein.Write(res)
        ''filein.Close()

        Console.WriteLine("RETURN=" & res)
        Return res
    End Function

    Public Shared Function SendRequest_DELETE(ByVal uri As String, ByVal hash As String, ByVal jsonSTR As String, ByVal authorization_type As String, ByVal contentType As String, ByVal accept As String) As String
        Console.WriteLine("SendRequest_DELETE")
        Console.WriteLine("URL={0} {1}DATA={2} {3}CONTENT_TYPE={4}", uri, vbNewLine, jsonSTR, vbNewLine, contentType)

        Dim res As String = ""
        Dim req As WebRequest = WebRequest.Create(uri)
        SetBasicAuthHeader(req, authorization_type, hash, accept)
        req.ContentType = contentType
        req.Method = "DELETE"

        Dim jsonDataBytes As Byte()

        If jsonSTR <> "" Then
            jsonDataBytes = Encoding.UTF8.GetBytes(jsonSTR)

            If jsonDataBytes IsNot Nothing Then
                req.ContentLength = jsonDataBytes.Length
                Dim stream = req.GetRequestStream()
                stream.Write(jsonDataBytes, 0, jsonDataBytes.Length)
                stream.Close()
            End If
        End If

        Try
            Dim response = req.GetResponse().GetResponseStream()
            Dim reader As New StreamReader(response)
            res = reader.ReadToEnd()
            reader.Close()
            response.Close()
        Catch ex As System.Net.WebException
            Dim sResponse As String = New StreamReader(ex.Response.GetResponseStream()).ReadToEnd
            ''Console.WriteLine("RETURN=" & sResponse)
            res = sResponse
        End Try

        Console.WriteLine("RETURN=" & res)
        Return res
    End Function

End Class
