Imports Oracle.ManagedDataAccess.Client
Imports System.IO
Imports System.Data
Imports Newtonsoft.Json.Linq

Public Class inpost_package

    Public Shared url As String = ""
    Public Shared organization As String = ""
    Public Shared access_token As String = ""
    Public Shared exception As String = ""
    ''Public Shared i_shipment As Shipment

    Public Shared req_url As String = ""
    Public Shared req_url_desc As String = ""
    Public Shared j_string As String = ""
    Public Shared req_return As String = ""

    Public Shared Sub inpost_login(ByRef obj_session As InpostSession, ByVal schemat As String, ByVal mag_id As String, ByVal firma_id As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Console.WriteLine("inpost_login...")

            ''Dim sqlexp = "SELECT LOGIN,HASLO FROM dp_swm_mia_firma_login WHERE SCHEMAT='" & schemat & "' AND AKTYWNA='X' AND MAG='" & mag_id & "' AND FIRMA_ID='" & firma_id & "'"
            Dim sqlexp = "SELECT LOGIN,HASLO FROM dp_swm_mia_firma_login WHERE SCHEMAT='" & schemat & "' AND MAG='" & mag_id & "' AND FIRMA_ID='" & firma_id & "' AND AKTYWNA='X'"
            Dim cmd As OracleCommand = New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            Try
                While dr.Read()
                    organization = dr.Item(0).ToString
                    access_token = dr.Item(1).ToString

                    If organization = "5289956" Then
                        url = "https://sandbox-api-shipx-pl.easypack24.net"
                    ElseIf organization = "44376" Then
                        url = "https://api-shipx-pl.easypack24.net"
                    End If

                    obj_session.organization = organization
                    obj_session.access_token = access_token
                    obj_session.url = url

                End While

            Catch ex As System.ArgumentOutOfRangeException
                Console.WriteLine(ex)
            End Try
            dr.Close()
            cmd.Dispose()
            conn.Close()

        End Using
    End Sub

    Public Class InpostSession
        Public url As String = ""
        Public organization As String = ""
        Public access_token As String = ""
        Public i_shipment As Shipment = New Shipment
    End Class


    Public Class Shipment
        Public i_receiver As Receiver
        Public i_parcels As New List(Of Parcels)
        Public i_custom_attributes As CustomAttributes
        Public i_cod As MoneyData
        Public i_insurance As MoneyData
        Public i_is_return As Boolean = False
        Public i_service As String = ""
        Public i_reference As String = ""
        Public i_external_customer_id As String = ""
        Public i_additional_services As New List(Of String)
        Public i_mpk As String = ""
        Public i_comments As String = ""
        Public i_only_choice_of_offer As String = "false"
    End Class

        Public Class MoneyData
            Public amount As String = ""
            Public currency As String = "PLN"
        End Class

        Public Class CustomAttributes
        Public target_point As String = ""
        Public sending_method As String = ""
            Public dropoff_point As String = ""
            Public allegro_transaction_id As String = ""
            Public allegro_user_id As String = ""
            Public dispatch_order_id As Integer = 0
        End Class

        Public Class Receiver
            Public name As String = ""
            Public company_name As String = ""
            Public first_name As String = ""
            Public last_name As String = ""
            Public email As String = ""
            Public phone As String = ""
            Public address_street As String = ""
            Public address_building_number As String = ""
            Public address_city As String = ""
            Public address_post_code As String = ""
            Public address_country_code As String = ""
        End Class

        Public Class Parcels
            Public id As String = ""
        Public template As String = ""
        Public dimensions_length As String = ""
        Public dimensions_width As String = ""
            Public dimensions_height As String = ""
            Public dimensions_unit As String = ""
            Public weight_amount As String = ""
            Public weight_unit As String = ""
            Public is_non_standard As String = "false"
        End Class

        Public Shared Function inpost_loadReceiver(ByVal Name As String, ByVal company_name As String, ByVal first_name As String, ByVal last_name As String, ByVal email As String, ByVal phone As String, ByVal street As String, ByVal building_number As String, ByVal city As String, ByVal post_code As String, ByVal country_code As String) As Receiver
            Dim obj_receiver As New Receiver
            obj_receiver.name = Name
            obj_receiver.company_name = company_name
            obj_receiver.first_name = first_name
            obj_receiver.last_name = last_name
            obj_receiver.email = email
            obj_receiver.phone = phone
            obj_receiver.address_street = street
            obj_receiver.address_building_number = building_number
            obj_receiver.address_city = city
            obj_receiver.address_post_code = post_code
            obj_receiver.address_country_code = country_code

        ''i_shipment.i_receiver = obj_receiver
        Return obj_receiver
        End Function

    Public Shared Function inpost_loadParcel(ByVal template As String, ByVal id As String, ByVal length As String, ByVal width As String, ByVal height As String, ByVal unit As String, ByVal weight_amount As String, ByVal weight_unit As String, ByVal is_non_standard As String) As Parcels
        Dim obj_parcel As New Parcels
        obj_parcel.id = id
        obj_parcel.template = template
        obj_parcel.dimensions_length = length
        obj_parcel.dimensions_width = width
        obj_parcel.dimensions_height = height
        obj_parcel.dimensions_unit = unit
        obj_parcel.weight_amount = weight_amount
        obj_parcel.weight_unit = weight_unit
        obj_parcel.is_non_standard = is_non_standard

        ''i_shipment.i_parcels.Add(obj_parcel)
        Return obj_parcel
    End Function

    Public Shared Function inpost_loadCod(ByVal amount As String, ByVal currency As String) As MoneyData
        Dim obj_service As New MoneyData
        obj_service.amount = amount
        obj_service.currency = currency
        '' i_shipment.i_cod = obj_service
        Return obj_service
    End Function

    Public Shared Function inpost_loadInsurance(ByVal amount As String, ByVal currency As String) As MoneyData
        Dim obj_service As New MoneyData
        obj_service.amount = amount
        obj_service.currency = currency
        ''i_shipment.i_insurance = obj_service
        Return obj_service
    End Function

    Public Shared Function inpost_loadAdditionalServices(ByVal add_service As String) As String
        Dim obj_service As String = add_service
        ''i_shipment.i_additional_services.Add(obj_service)
        Return obj_service
    End Function

    Public Shared Function REST_POST_CreateShipment(ByVal obj_ship As Shipment) As String
        Dim shipment_id As String = ""
        Dim req_temp As String = ""
        req_url = url & "/v1/organizations/" & organization & "/shipments"
        req_url_desc = ""

        Dim rest_receiver As String = ""
        If obj_ship.i_receiver.company_name <> "" Then
            rest_receiver = "{" _
            & "'receiver': {
            'name': '" & obj_ship.i_receiver.name & "',
            'company_name': '" & obj_ship.i_receiver.company_name & "',
            'first_name': '" & obj_ship.i_receiver.first_name & "',
            'last_name': '" & obj_ship.i_receiver.last_name & "', 
            'email': '" & obj_ship.i_receiver.email & "',
            'phone': '" & obj_ship.i_receiver.phone & "',
            'address': {
            'street': '" & obj_ship.i_receiver.address_street & "',
            'building_number': '" & obj_ship.i_receiver.address_building_number & "',
            'city': '" & obj_ship.i_receiver.address_city & "',
            'post_code': '" & obj_ship.i_receiver.address_post_code & "',
            'country_code': '" & obj_ship.i_receiver.address_country_code & "'
            }
            },"
        Else
            rest_receiver = "{" _
            & "'receiver': {
            'name': '" & obj_ship.i_receiver.name & "',
            'first_name': '" & obj_ship.i_receiver.first_name & "',
            'last_name': '" & obj_ship.i_receiver.last_name & "', 
            'email': '" & obj_ship.i_receiver.email & "',
            'phone': '" & obj_ship.i_receiver.phone & "',
            'address': {
            'street': '" & obj_ship.i_receiver.address_street & "',
            'building_number': '" & obj_ship.i_receiver.address_building_number & "',
            'city': '" & obj_ship.i_receiver.address_city & "',
            'post_code': '" & obj_ship.i_receiver.address_post_code & "',
            'country_code': '" & obj_ship.i_receiver.address_country_code & "'
            }
            },"
        End If

        req_temp &= rest_receiver

        Dim rest_parcel_list As String = ""
        For Each id_parcel In obj_ship.i_parcels
            Dim obj_rest_parcel As String = ""

            If id_parcel.id <> "" Then
                obj_rest_parcel = " {
                'id': '" & id_parcel.id & "',
                'template': '" & id_parcel.template & "',
                'dimensions': {
                'length': '" & id_parcel.dimensions_length & "',
                'width': '" & id_parcel.dimensions_width & "',
                'height': '" & id_parcel.dimensions_height & "',
                'unit': '" & id_parcel.dimensions_unit & "'
                },
                'weight': {
                'amount': '" & id_parcel.weight_amount & "',
                'unit': '" & id_parcel.weight_unit & "'
                },
                'is_non_standard': " & id_parcel.is_non_standard & "
                },"
            Else
                obj_rest_parcel = " {
                'template': '" & id_parcel.template & "',
                'dimensions': {
                'length': '" & id_parcel.dimensions_length & "',
                'width': '" & id_parcel.dimensions_width & "',
                'height': '" & id_parcel.dimensions_height & "',
                'unit': '" & id_parcel.dimensions_unit & "'
                },
                'weight': {
                'amount': '" & id_parcel.weight_amount & "',
                'unit': '" & id_parcel.weight_unit & "'
                },
                'is_non_standard': " & id_parcel.is_non_standard & "
                },"

            End If

            rest_parcel_list += obj_rest_parcel
        Next

        rest_parcel_list = rest_parcel_list.Substring(0, rest_parcel_list.Count - 1)
        Dim rest_parcel As String = "'parcels': [" & rest_parcel_list & "],"
        req_temp &= rest_parcel

        If obj_ship.i_cod IsNot Nothing Then
            If obj_ship.i_cod.amount <> "" Then
                Dim obj_service As String = "'cod': {
                'amount': " & obj_ship.i_cod.amount.ToString & ",
                'currency': '" & obj_ship.i_cod.currency.ToString & "'
                },"
                req_temp &= obj_service
            End If
        End If

        If obj_ship.i_insurance IsNot Nothing Then
            If obj_ship.i_insurance.amount <> "" Then
                Dim obj_service As String = "'insurance': {
                'amount': " & obj_ship.i_insurance.amount.ToString & ",
                'currency': '" & obj_ship.i_insurance.currency.ToString & "'
                },"
                req_temp &= obj_service
            End If
        End If

        req_temp &= "'service': '" & obj_ship.i_service.ToString & "',"

        Dim rest_custom_attributes As String = ""
        If obj_ship.i_custom_attributes.target_point <> "" Then
            Dim obj_service As String = "'custom_attributes': {
            'target_point': '" & obj_ship.i_custom_attributes.target_point.ToString & "',
            'sending_method': 'dispatch_order'
            },"
            req_temp &= obj_service
        ElseIf obj_ship.i_service.ToString = "inpost_courier_allegro" Then
            req_temp &= "'custom_attributes' :  {
            'sending_method': 'dispatch_order'
            },"
        End If

        Dim rest_service_list As String = ""
        For Each id_service In obj_ship.i_additional_services
            rest_service_list &= "'" & id_service & "',"
        Next

        If rest_service_list <> "" Then
            rest_service_list = rest_service_list.Substring(0, rest_service_list.Count - 1)
            Dim rest_service As String = "'additional_services': [" & rest_service_list & "],"
            req_temp &= rest_service
        End If

        req_temp &= "'reference': '" & obj_ship.i_reference.ToString & "',
        'comments': '" & obj_ship.i_comments.ToString & "',
        'external_customer_id': '" & obj_ship.i_external_customer_id.ToString & "',
        'mpk': '" & obj_ship.i_mpk.ToString & "',
        'only_choice_of_offer': " & obj_ship.i_only_choice_of_offer & ""

        req_temp &= "}"

        Dim stringJson As String = req_temp
        stringJson = stringJson.Replace("'", """")

        ''Dim varses = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CheckoutFormAddParcelTrackingNumber)(stringJson)

        j_string = stringJson
        ''jdata = Encoding.UTF8.GetBytes(j_string)
        req_return = InpostRestAPI.SendRequest_POST(req_url + req_url_desc, access_token, j_string, "Bearer ", "application/json", "application/json")
        If req_return.Contains("error") = False Then
            shipment_id = InpostRestAPI.JSON_SelectToken(req_return, "id")

        End If

        exception = "[REST_POST_CreateShipment]" & req_return.ToString
        ''    REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "Y")
        ''    myTrackingNumber = j_return.Replace("""", "")
        ''Else
        ''    REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "N")
        ''End If
        Return shipment_id
    End Function

    Public Shared Function REST_POST_BuyShipment(ByVal shipment_id As String, ByVal offer_id As String) As String
        ''Dim shipment_id As String = ""
        Dim req_temp As String = ""
        req_url = url & "/v1/shipments/" & shipment_id & "/buy"
        req_url_desc = ""

        req_temp = "{"
        req_temp &= "'offer_id': '" & offer_id.ToString & "'"
        req_temp &= "}"

        Dim stringJson As String = req_temp
        stringJson = stringJson.Replace("'", """")

        ''Dim varses = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CheckoutFormAddParcelTrackingNumber)(stringJson)

        j_string = stringJson
        ''jdata = Encoding.UTF8.GetBytes(j_string)
        req_return = InpostRestAPI.SendRequest_POST(req_url + req_url_desc, access_token, j_string, "Bearer ", "application/json", "application/json")
        If req_return.Contains("error") = False Then
            shipment_id = InpostRestAPI.JSON_SelectToken(req_return, "id")
        End If

        exception = "[REST_POST_BuyShipment]" & req_return.ToString
        ''    REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "Y")
        ''    myTrackingNumber = j_return.Replace("""", "")
        ''Else
        ''    REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "N")
        ''End If
        Return shipment_id
    End Function


    Public Shared Function REST_DELETE_DeleteShipment(ByVal tracking_number As String, ByVal shipment_id As String) As String
        Try
            Dim req_temp As String = ""
            req_url = url & "/v1/shipments/" & shipment_id
            req_url_desc = ""


            Dim stringJson As String = req_temp
            stringJson = stringJson.Replace("'", """")

            j_string = stringJson
            ''jdata = Encoding.UTF8.GetBytes(j_string)
            req_return = InpostRestAPI.SendRequest_DELETE(req_url + req_url_desc, access_token, j_string, "Bearer ", "application/json", "application/json")
            If req_return.Contains("error") = False Then
                shipment_id = InpostRestAPI.JSON_SelectToken(req_return, "id")
                Return "1"
            End If

            exception = "[REST_DELETE_DeleteShipment]" & req_return.ToString
            Return exception

        Catch ex As Exception
            exception = "[REST_DELETE_DeleteShipment]" & ex.Message.ToString
            Return "1111"
        End Try
    End Function

    Public Shared Function REST_GET_LabelOfShipment(ByVal tracking_number As String, ByVal shipment_id As String) As String

        Try
            Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/inpost/"
            dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)
            Dim dir_stream As String = dir_export & "\inpost_" & tracking_number.ToString & ".pdf"

            Dim req_temp As String = ""
            req_url = url & "/v1/shipments/" & shipment_id & "/label"
            req_url_desc = "?format=Pdf&type=A6"

            Dim stringJson As String = req_temp
            stringJson = stringJson.Replace("'", """")

            j_string = stringJson
            req_return = InpostRestAPI.SavePdfFileSendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/pdf", "application/json", dir_stream)

            If req_return = "1" Then
                ''Return ("inpost_" & tracking_number.ToString & ".pdf")
                Return tracking_number.ToString
            Else
                Return req_return
            End If
        Catch ex As Exception
            exception = "[REST_GET_LabelOfShipment]" & ex.Message.ToString
            Return "1111"
        End Try
    End Function

    Public Shared Function REST_GET_ShipmentId(ByVal tracking_number As String) As String
        Dim shipment_id As String = ""
        Dim req_temp As String = ""
        req_url = url & "/v1/organizations/" & organization & "/shipments"
        req_url_desc = "?tracking_number=" & tracking_number

        Dim stringJson As String = req_temp
        stringJson = stringJson.Replace("'", """")

        j_string = stringJson
        req_return = InpostRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/json", "application/json")

        If req_return.Contains("items") = True Then
            Dim j_rss As JObject = JObject.Parse(req_return)
            ''Console.WriteLine(j_rss.SelectToken("services").ToString())
            Dim j_servies As JArray = j_rss.SelectToken("items")
            Dim j_list, t_list As New List(Of String)
            j_list = InpostRestAPI.json_array_to_list(j_servies)
            For Each a In j_list
                shipment_id = InpostRestAPI.JSON_SelectToken(a.ToString, "id")
                Exit For
            Next
        End If

        Return shipment_id
    End Function

    Public Shared Function REST_GET_StatusOfShipmentId(ByVal shipment_id As String) As String
        Dim status As String = ""
        Dim req_temp As String = ""
        req_url = url & "/v1/organizations/" & organization & "/shipments"
        req_url_desc = "?id=" & shipment_id

        Dim stringJson As String = req_temp
        stringJson = stringJson.Replace("'", """")

        j_string = stringJson
        req_return = InpostRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/json", "application/json")
        If req_return.Contains("items") = True Then
            Dim j_rss As JObject = JObject.Parse(req_return)
            ''Console.WriteLine(j_rss.SelectToken("services").ToString())
            Dim j_servies As JArray = j_rss.SelectToken("items")
            Dim j_list, t_list As New List(Of String)
            j_list = InpostRestAPI.json_array_to_list(j_servies)
            For Each a In j_list
                status = InpostRestAPI.JSON_SelectToken(a.ToString, "status")
                Exit For
            Next
        End If

        Return status
    End Function

    Public Shared Function REST_GET_OfferIdOfShipmentId(ByVal shipment_id As String) As String
        Dim offer_id As String = ""
        Dim req_temp As String = ""
        req_url = url & "/v1/organizations/" & organization & "/shipments"
        req_url_desc = "?id=" & shipment_id

        Dim stringJson As String = req_temp
        stringJson = stringJson.Replace("'", """")

        j_string = stringJson
        req_return = InpostRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/json", "application/json")
        If req_return.Contains("items") = True Then
            Dim j_rss As JObject = JObject.Parse(req_return)
            ''Console.WriteLine(j_rss.SelectToken("services").ToString())
            Dim j_servies As JArray = j_rss.SelectToken("items")
            Dim j_list, t_list As New List(Of String)
            j_list = InpostRestAPI.json_array_to_list(j_servies)
            For Each a In j_list
                Dim j_servies_ofe As JArray = InpostRestAPI.JSON_SelectArray(a.ToString, "offers")
                Dim j_list_ofe, t_list_ofe As New List(Of String)
                j_list_ofe = InpostRestAPI.json_array_to_list(j_servies_ofe)
                For Each a_ofe In j_list_ofe
                    offer_id = InpostRestAPI.JSON_SelectToken(a_ofe.ToString, "id")
                    Exit For
                Next
            Next
        End If

        Return offer_id
    End Function


    Public Shared Function REST_GET_ParcelTrackingNumber(ByVal shipment_id As String) As String
        Dim tracking_number As String = ""
        Dim req_temp As String = ""
        req_url = url & "/v1/organizations/" & organization & "/shipments"
        req_url_desc = "?id=" & shipment_id

        Dim stringJson As String = req_temp
        stringJson = stringJson.Replace("'", """")

        j_string = stringJson
        req_return = InpostRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/json", "application/json")
        If req_return.Contains("items") = True Then
            Dim j_rss As JObject = JObject.Parse(req_return)
            ''Console.WriteLine(j_rss.SelectToken("services").ToString())
            Dim j_servies As JArray = j_rss.SelectToken("items")
            Dim j_list, t_list As New List(Of String)
            j_list = InpostRestAPI.json_array_to_list(j_servies)
            For Each a In j_list
                tracking_number = InpostRestAPI.JSON_SelectToken(a.ToString, "tracking_number")
                Exit For
            Next
        End If

        Return tracking_number
    End Function

    Public Shared Function REST_GET_ReportsCod(ByVal start_date As String, ByVal status As String) As String
        Try
            Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/inpost/"
            dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)
            Dim pplist_inpost As String = "pplist_inpost_" & dajarSWMagazyn_MIA.MyFunction.DataEval & ".json"
            Dim dir_stream As String = dir_export & "\" & pplist_inpost

            Dim req_temp As String = ""
            Dim end_date As String = start_date
            req_url = url & "/v1/organizations/" & organization & "/shipments"
            req_url_desc = "?created_at=" & start_date & "&status=" & status & "&format=json"

            Dim stringJson As String = req_temp
            stringJson = stringJson.Replace("'", """")

            j_string = stringJson
            req_return = InpostRestAPI.SavePdfFileSendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/csv", "application/json", dir_stream)

            If req_return = "1" Then
                Return (pplist_inpost)
            Else
                Return req_return
            End If
        Catch ex As Exception
            exception = "[REST_GET_LabelOfShipment]" & ex.Message.ToString
            Return "1111"
        End Try

    End Function

    Public Shared Function REST_DELETE_ParcelTrackingNumber(ByVal shipment_id As String) As String
        Dim tracking_number As String = ""
        Dim req_temp As String = ""
        req_url = url & "/v1/shipments/" & shipment_id
        req_url_desc = ""

        Dim stringJson As String = req_temp
        stringJson = stringJson.Replace("'", """")

        j_string = stringJson
        req_return = InpostRestAPI.SendRequest_DELETE(req_url + req_url_desc, access_token, j_string, "", "application/json", "application/json")
        If req_return.Contains("items") = True Then
            Dim j_rss As JObject = JObject.Parse(req_return)
            ''Console.WriteLine(j_rss.SelectToken("services").ToString())
            Dim j_servies As JArray = j_rss.SelectToken("items")
            Dim j_list, t_list As New List(Of String)
            j_list = InpostRestAPI.json_array_to_list(j_servies)
            For Each a In j_list
                tracking_number = InpostRestAPI.JSON_SelectToken(a.ToString, "tracking_number")
                Exit For
            Next
        End If

        Return tracking_number
    End Function
End Class
