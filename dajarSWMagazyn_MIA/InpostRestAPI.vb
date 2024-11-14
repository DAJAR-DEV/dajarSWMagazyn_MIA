Imports System.Net
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Text
Imports Oracle.ManagedDataAccess.Client

Public Class InpostRestAPI

    Public Shared j_return As String = ""
    Public Shared url_desc As String = ""
    Public Shared j_string As String = ""
    Public Shared j_data As Byte()
    Public Shared sqlexp As String = ""

    Class CheckoutFormAddParcelTrackingNumber
        Public carrierId As String = ""
        Public waybill As String = ""
        Public lineItems As JArray
    End Class

    Class MagentoREST
        Public login As String = ""
        Public access_token As String = ""
        Public url As String = ""
        Public store_view As String = ""
        Public client_id As String = ""
        Public client_secret As String = ""
        Public user_code As String = ""
        Public device_code As String = ""
        Public expires_in As String = ""
        Public interval As String = ""
        Public verification_uri As String = ""
        Public verification_uri_complete As String = ""
        Public token As MagentoREST_Token
        Public base64Encoded As String = ""
        Public nr_digit As String = ""
        Public mag As String = ""
    End Class

    Class MagentoREST_Token
        Public access_token As String = ""
        Public token_type As String = ""
        Public refresh_token As String = ""
        Public expires_in As String = ""
        Public scope As String = ""
        Public jti As String = ""
    End Class

    Class DeliveryMethod
        Public id As String = ""
        Public name As String = ""
        Public paymentPolicy As String = ""
    End Class

    Class OrdersForm
        Public billing_address As JObject
        Public coupon_code As String = ""
        Public created_at As String = ""
        Public customer_email As String = ""
        Public customer_note As String = ""
        Public entity_id As String = ""
        Public extension_attributes As JObject
        Public base_grand_total As String = ""
        Public grand_total As String = ""
        Public increment_id As String = ""
        Public items As JArray
        Public base_currency_code As String = ""
        Public order_currency_code As String = ""
        Public payment As JObject
        Public base_shipping As String = ""
        Public base_shipping_incl_tax As String = ""
        Public shipping As String = ""
        Public shipping_incl_tax As String = ""
        Public shipping_description As String = ""
        Public state As String = ""
        Public status As String = ""
        Public store_id As String = ""
        Public base_subtotal As String = ""
        Public base_subtotal_incl_tax As String = ""
        Public subtotal As String = ""
        Public subtotal_incl_tax As String = ""
        Public base_discount_amount As String = ""
        Public base_discount_amount_tax As String = ""
        Public discount_amount As String = ""
        Public updated_at As String = ""
        Public hor_billing_address As String = ""
        Public hor_shipping_address As String = ""
        Public myBillingForm As New InpostRestAPI.BillingAddressForm
        Public return_kod_kontr As String = ""
        Public external_id As String = ""
        Public account_user_name As String = ""
        Public external_username As String = ""
        Public rewardpoints_base_discount As String = ""
        Public rewardpoints_discount As String = ""
    End Class

    Class ItemsForm
        Public item_id As String = ""
        Public name As String = ""
        Public price_incl_tax As String = ""
        Public sku As String = ""
        Public qty_ordered As String = ""
    End Class

    Class BillingAddressForm
        Public city As String = ""
        Public company As String = ""
        Public country_id As String = ""
        Public email As String = ""
        Public firstname As String = ""
        Public lastname As String = ""
        Public postcode As String = ""
        Public street As JArray
        Public telephone As String = ""
    End Class

    Class ShippingAddressForm
        Public city As String = ""
        Public company As String = ""
        Public country_id As String = ""
        Public email As String = ""
        Public firstname As String = ""
        Public lastname As String = ""
        Public postcode As String = ""
        Public street As JArray
        Public telephone As String = ""
    End Class

    Class ShipmentTrackForm
        Public order_id As String = ""
        Public created_at As String = ""
        Public track_number As String = ""
        Public title As String = ""
        Public carrier_code As String = ""
        Public parent_id As String = ""
        Public increment_id As String = ""
    End Class

    Public Shared Sub SetBasicAuthHeader(ByVal request As HttpWebRequest, ByVal auth_type As String, ByVal hash As [String], ByVal accept As String)
        ''request.AllowAutoRedirect = False
        ''request.PreAuthenticate = True
        ''request.KeepAlive = True
        request.Accept = accept
        request.Headers.Add("Authorization", auth_type & hash)
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

    Public Shared Function SendRequest_GET(ByVal uri As String, ByVal hash As String, ByVal jsonSTR As String, ByVal uriData As String, ByVal contentType As String, ByVal accept As String) As String
        Console.WriteLine("SendRequest_GET")
        Console.WriteLine("URL={0} {1}DATA={2} {3}CONTENT_TYPE={4}", uri, vbNewLine, jsonSTR, vbNewLine, contentType)
        Dim req As WebRequest = WebRequest.Create(uri)
        Dim res As String = ""
        SetBasicAuthHeader(req, "Bearer ", hash, accept)
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

        If uriData <> "" Then
            jsonDataBytes = Encoding.UTF8.GetBytes(uriData)

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
        SetBasicAuthHeader(req, "Bearer ", hash, accept)
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

        If uriData <> "" Then
            jsonDataBytes = Encoding.UTF8.GetBytes(uriData)

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

    Public Shared Function REST_GET_ParcelTrackingNumber(ByVal magento_rapi As MagentoREST, ByVal order_id As String) As List(Of ShipmentTrackForm)
        Dim myShipmentTrackList As New List(Of ShipmentTrackForm)

        Dim url As String = magento_rapi.url & "/" & magento_rapi.store_view & "/V1/shipments"
        url_desc = "?searchCriteria[filterGroups][0][filters][0][field]=order_id&searchCriteria[filterGroups][0][filters][0][value]=" & order_id
        j_string = ""
        ''jdata = Nothing
        j_return = SendRequest_GET(url + url_desc, magento_rapi.access_token, j_string, "", "application/json", "application/json")
        If j_return.Contains("tracks") = True Then
            Dim j_rss As JObject = JObject.Parse(j_return)
            ''Console.WriteLine(j_rss.SelectToken("services").ToString())
            Dim j_servies As JArray = j_rss.SelectToken("items")
            Dim j_list, t_list As New List(Of String)
            j_list = json_array_to_list(j_servies)
            For Each a In j_list
                Dim myTracks As JArray = JSON_SelectArray(a, "tracks")
                t_list = json_array_to_list(myTracks)
                For Each t In t_list
                    Dim shipmentTrack As New ShipmentTrackForm
                    shipmentTrack.carrier_code = JSON_SelectToken(t, "carrier_code")
                    shipmentTrack.created_at = JSON_SelectToken(t, "created_at")
                    shipmentTrack.order_id = JSON_SelectToken(t, "order_id")
                    shipmentTrack.title = JSON_SelectToken(t, "title")
                    shipmentTrack.track_number = JSON_SelectToken(t, "track_number")
                    shipmentTrack.parent_id = JSON_SelectToken(t, "parent_id")
                    shipmentTrack.increment_id = JSON_SelectToken(a, "increment_id")
                    myShipmentTrackList.Add(shipmentTrack)
                Next
            Next
        End If
        Return myShipmentTrackList
    End Function

    Public Shared Function REST_POST_ParcelTrackingNumber(ByVal magento_rapi As MagentoREST, ByVal order_id As String, ByVal tracking_nr As String) As String
        Dim myTrackingNumber As String = ""
        Dim url As String = magento_rapi.url & "/" & magento_rapi.store_view & "/V1/order/"
        url_desc = order_id & "/ship"

        Dim trackingNumber As String = tracking_nr
        Dim trackingCode As String = "dhlint"

        Dim temp As String = ""

        If tracking_nr <> "" Then
            temp = "{" _
            & "'notify': false," _
            & "'appendComment': true," _
            & "'comment': {" _
            & "'comment': 'Zamówienie wysłane'," _
            & "'is_visible_on_front': 1" _
            & "}," _
            & "" _
            & "'tracks': [" _
            & "{" _
            & "'track_number': '" & trackingNumber & "'," _
            & "'title': 'DHL'," _
            & "'carrier_code': '" & trackingCode & "'" _
            & "}]}"
        Else
            temp = "{" _
            & "'notify': false," _
            & "'appendComment': true," _
            & "'comment': {" _
            & "'comment': 'Zamówienie wysłane'," _
            & "'is_visible_on_front': 1" _
            & "}}"
        End If

        Dim stringJson As String = temp
        stringJson = stringJson.Replace("'", """")

        Dim varses = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CheckoutFormAddParcelTrackingNumber)(stringJson)

        ''j_string = "client_id=" & magento_rapi.client_id
        j_string = stringJson
        ''jdata = Encoding.UTF8.GetBytes(j_string)
        j_return = SendRequest_POST(url + url_desc, magento_rapi.access_token, j_string, "Bearer ", "application/json", "application/json")
        If j_return.Contains("message") = False Then

            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "Y")
            myTrackingNumber = j_return.Replace("""", "")
        Else
            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "N")
        End If
        Return myTrackingNumber
    End Function

    Public Shared Function REST_POST_DigitlandNumerOrder(ByVal magento_rapi As MagentoREST, ByVal order_id As String, ByVal increment_id As String, ByVal ext_customer_id As String, ByVal ext_order_id As String) As String
        Dim myInvoiceNumber As String = ""
        Dim url As String = magento_rapi.url & "/V1/orders"
        url_desc = ""

        Dim temp As String = "{" _
        & "'entity': {" _
        & "'entity_id': " & order_id & "," _
        & "'increment_id': '" & increment_id & "'," _
        & "'ext_customer_id': '" & ext_customer_id & "'," _
        & "'ext_order_id': '" & ext_order_id & "'" _
        & "}}"

        Dim stringJson As String = temp
        stringJson = stringJson.Replace("'", """")

        Dim varses = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CheckoutFormAddParcelTrackingNumber)(stringJson)

        ''j_string = "client_id=" & magento_rapi.client_id
        j_string = stringJson
        ''jdata = Encoding.UTF8.GetBytes(j_string)
        j_return = SendRequest_POST(url + url_desc, magento_rapi.access_token, j_string, "Bearer ", "application/json", "application/json")
        If j_return.Contains("entity_id") = False Then
            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "Y")
            myInvoiceNumber = j_return.Replace("""", "")
        Else
            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "N")
        End If
        Return myInvoiceNumber
    End Function

    Public Shared Function REST_POST_ProductRepositoryV1(ByVal magento_rapi As MagentoREST, ByVal p_sku As String, ByVal p_qty As String, ByVal p_value As String) As String
        Dim myInvoiceNumber As String = ""
        Dim url As String = magento_rapi.url & "/" & magento_rapi.store_view & "/V1/products"
        url_desc = ""

        Dim stringTemp As String = ""
        If p_value <> "" Then
            stringTemp = "{" _
            & "'product': {" _
            & "'sku': '" & p_sku & "'," _
            & "'extension_attributes': {" _
            & "'stock_item': {" _
            & "'qty': " & p_qty & "," _
            & "'is_in_stock': true" _
            & "}" _
            & "}," _
            & "'custom_attributes': [" _
            & "{" _
            & "'attribute_code': 'availability'," _
            & "'value': '" & p_value & "'" _
            & "}" _
            & "]" _
            & "}," _
            & "'saveOptions': true" _
            & "}"
        Else
            stringTemp = "{" _
            & "'product': {" _
            & "'sku': '" & p_sku & "'," _
            & "'extension_attributes': {" _
            & "'stock_item': {" _
            & "'qty': " & p_qty & "," _
            & "'is_in_stock': true" _
            & "}" _
            & "}" _
            & "}," _
            & "'saveOptions': true" _
            & "}"
        End If

        Dim stringJson As String = stringTemp
        stringJson = stringJson.Replace("'", """")

        Dim varses = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CheckoutFormAddParcelTrackingNumber)(stringJson)

        j_string = stringJson
        ''jdata = Encoding.UTF8.GetBytes(j_string)
        j_return = SendRequest_POST(url + url_desc, magento_rapi.access_token, j_string, "Bearer ", "application/json", "application/json")
        If j_return.Contains("sku") = True Then
            myInvoiceNumber = JSON_SelectToken(j_return, "sku")
            Return "Y"
        ElseIf j_return.Contains("message") = True Then
            myInvoiceNumber = JSON_SelectToken(j_return, "message") & JSON_SelectToken(j_return, "parameters")
            Return "N"
        End If
        Return myInvoiceNumber
    End Function

    Public Shared Function REST_POST_InvoiceOrder(ByVal magento_rapi As MagentoREST, ByVal order_id As String) As String
        Dim myInvoiceNumber As String = ""
        Dim url As String = magento_rapi.url & "/" & magento_rapi.store_view & "/V1/order/"
        url_desc = order_id & "/invoice"

        Dim temp As String = "{" _
        & "'notify': false," _
        & "'appendComment': true," _
        & "'comment': {" _
        & "'comment': 'Zamówienie zrealizowane'," _
        & "'is_visible_on_front': 1" _
        & "}" _
        & "}"

        Dim stringJson As String = temp
        stringJson = stringJson.Replace("'", """")

        Dim varses = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CheckoutFormAddParcelTrackingNumber)(stringJson)

        ''j_string = "client_id=" & magento_rapi.client_id
        j_string = stringJson
        ''jdata = Encoding.UTF8.GetBytes(j_string)
        j_return = SendRequest_POST(url + url_desc, magento_rapi.access_token, j_string, "Bearer ", "application/json", "application/json")
        If j_return.Contains("message") = False Then
            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "Y")
            myInvoiceNumber = j_return.Replace("""", "")
        Else
            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "N")
        End If
        Return myInvoiceNumber
    End Function

    Public Shared Function REST_POST_DigitlandStatusOrder(ByVal magento_rapi As MagentoREST, ByVal entity_id As String, ByVal increment_id As String, ByVal nr_zamow As String, ByVal stat_u1 As String) As String
        Dim myInvoiceNumber As String = ""
        Dim url As String = magento_rapi.url & "/V1/orders"
        url_desc = ""

        Dim temp As String = "{" _
        & "'entity': {" _
        & "'entity_id': " & entity_id & "," _
        & "'ext_order_id': '" & nr_zamow & "'," _
        & "'extension_attributes': {" _
        & "'digit_status': '" & stat_u1 & "'" _
        & "}" _
        & "}" _
        & "}"

        Dim stringJson As String = temp
        stringJson = stringJson.Replace("'", """")

        Dim varses = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CheckoutFormAddParcelTrackingNumber)(stringJson)

        ''j_string = "client_id=" & magento_rapi.client_id
        j_string = stringJson
        ''jdata = Encoding.UTF8.GetBytes(j_string)
        j_return = SendRequest_POST(url + url_desc, magento_rapi.access_token, j_string, "Bearer ", "application/json", "application/json")
        If j_return.Contains(entity_id) = True Then
            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), nr_zamow, stat_u1, "Y")
            myInvoiceNumber = j_return.Replace("""", "")
        Else
            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), nr_zamow, stat_u1, "N")
        End If
        Return myInvoiceNumber
    End Function

    Public Shared Function REST_POST_OrderCancel(ByVal magento_rapi As MagentoREST, ByVal order_id As String) As String
        Dim myInvoiceNumber As String = ""
        Dim url As String = magento_rapi.url & "/V1/orders/"
        url_desc = order_id & "/cancel"

        Dim temp As String = ""

        Dim stringJson As String = temp
        stringJson = stringJson.Replace("'", """")

        Dim varses = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CheckoutFormAddParcelTrackingNumber)(stringJson)

        ''j_string = "client_id=" & magento_rapi.client_id
        j_string = stringJson
        ''jdata = Encoding.UTF8.GetBytes(j_string)
        j_return = SendRequest_POST(url + url_desc, magento_rapi.access_token, j_string, "Bearer ", "application/json", "application/json")
        If j_return.Contains("true") = True Then
            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "Y")
            myInvoiceNumber = j_return.Replace("""", "")
        Else
            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "N")
        End If
        Return myInvoiceNumber
    End Function

    Public Shared Function REST_POST_InvoiceRefund(ByVal magento_rapi As MagentoREST, ByVal order_id As String) As String
        Dim myInvoiceNumber As String = ""
        Dim url As String = magento_rapi.url & "/V1/order/"
        url_desc = order_id & "/refund"

        Dim temp As String = "{" _
        & "'notify': false," _
        & "'appendComment': true," _
        & "'comment': {" _
        & "'comment': 'Zamówienie anulowane'," _
        & "'is_visible_on_front': 1" _
        & "}" _
        & "}"

        Dim stringJson As String = temp
        stringJson = stringJson.Replace("'", """")

        Dim varses = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CheckoutFormAddParcelTrackingNumber)(stringJson)

        ''j_string = "client_id=" & magento_rapi.client_id
        j_string = stringJson
        ''jdata = Encoding.UTF8.GetBytes(j_string)
        j_return = SendRequest_POST(url + url_desc, magento_rapi.access_token, j_string, "Bearer ", "application/json", "application/json")
        If j_return.Contains("message") = False Then
            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "Y")
            myInvoiceNumber = j_return.Replace("""", "")
        Else
            REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "N")
        End If
        Return myInvoiceNumber
    End Function
End Class
