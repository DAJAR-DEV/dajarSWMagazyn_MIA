Imports Oracle.ManagedDataAccess.Client
Imports System.IO
Imports System.Data
Imports Newtonsoft.Json.Linq

Public Class dhl_parcel_de_package
    Public Shared url As String = ""
    Public Shared organization As String = ""
    Public Shared password As String = ""
    Public Shared access_token As String = ""
    Public Shared dhl_api_key As String = ""
    Public Shared exception As String = ""
    Public Shared billingNumber As String = ""

    Public Shared req_url As String = ""
        Public Shared req_url_desc As String = ""
        Public Shared j_string As String = ""
        Public Shared req_return As String = ""

    Public Shared Sub dhl_login(ByRef obj_session As DHL_DE_Session, ByVal schemat As String, ByVal mag_id As String, ByVal firma_id As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Console.WriteLine("dhl_login...")

            ''Dim sqlexp = "SELECT LOGIN,HASLO FROM dp_swm_mia_firma_login WHERE SCHEMAT='" & schemat & "' AND AKTYWNA='X' AND MAG='" & mag_id & "' AND FIRMA_ID='" & firma_id & "'"
            ''Dim sqlexp = "SELECT LOGIN,HASLO FROM dp_swm_mia_firma_login WHERE SCHEMAT='" & schemat & "' AND MAG='" & mag_id & "' AND FIRMA_ID='" & firma_id & "' AND AKTYWNA='X'"
            Dim sqlexp = "SELECT LOGIN,HASLO FROM dp_swm_mia_firma_login WHERE SCHEMAT='" & schemat & "' AND MAG='" & mag_id & "' AND FIRMA_ID='" & firma_id & "' AND AKTYWNA='X'"
            Dim cmd As OracleCommand = New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            Try
                While dr.Read()
                    organization = dr.Item(0).ToString
                    password = dr.Item(1).ToString

                    Dim basic_access As String = dajarSWMagazyn_MIA.MyFunction.CONVERT_STR_TO_BASE64(organization & ":" & password)

                    If organization = "webdajar1" Then
                        url = "https://api-eu.dhl.com/parcel/de/shipping/v2"
                        billingNumber = "62603146560101"
                        dhl_api_key = "7rgfsXFaKqTvbfGrzBXc5Zht6RITMpr9"
                    ElseIf organization = "sandy_sandbox" Then
                        url = "https://api-sandbox.dhl.com/parcel/de/shipping/v2"
                        billingNumber = "33333333330102"
                        dhl_api_key = "7rgfsXFaKqTvbfGrzBXc5Zht6RITMpr9"
                    End If

                    obj_session.dhl_api_key = dhl_api_key
                    obj_session.organization = organization
                    obj_session.password = password
                    obj_session.access_token = basic_access
                    access_token = basic_access
                    obj_session.url = url
                    obj_session.billingNumber = billingNumber

                End While

            Catch ex As System.ArgumentOutOfRangeException
                Console.WriteLine(ex)
            End Try
            dr.Close()
            cmd.Dispose()
            conn.Close()

        End Using
    End Sub

    Public Shared Function dhl_change_billing_number(ByVal obj_session As DHL_DE_Session, ByVal service As String) As String
        Dim new_billingNumber As String = ""
        Dim new_service As String = ""
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Console.WriteLine("dhl_change_billing...")
            If obj_session.billingNumber <> "" And obj_session.billingNumber.Length = 14 And service <> "" Then
                new_service = service.Substring(1, 2).ToString
                new_billingNumber = obj_session.billingNumber.Substring(0, 10) & new_service & obj_session.billingNumber.Substring(12, 2)
                Return new_billingNumber
            End If

            conn.Close()
        End Using
        Return new_billingNumber

    End Function

    Public Class DHL_DE_Session
        Public url As String = ""
        Public organization As String = ""
        Public password As String = ""
        Public access_token As String = ""
        Public dhl_api_key As String = ""
        Public i_shipment As Shipment = New Shipment
        Public billingNumber As String = ""
        Public packStation As String = ""
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
        Public billingNumer As String = ""
    End Class

    Public Class ShipmentResponse
        Public shipment_id As String = ""
        Public tracking_number As New List(Of String)
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
        Public name2 As String = ""
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

    Public Shared Function dhl_loadReceiver(ByVal Name As String, ByVal name2 As String, ByVal company_name As String, ByVal first_name As String, ByVal last_name As String, ByVal email As String, ByVal phone As String, ByVal street As String, ByVal building_number As String, ByVal city As String, ByVal post_code As String, ByVal country_code As String) As Receiver
        Dim obj_receiver As New Receiver
        obj_receiver.name = Name
        obj_receiver.name2 = name2
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

    Public Shared Function dhl_loadParcel(ByVal template As String, ByVal id As String, ByVal length As String, ByVal width As String, ByVal height As String, ByVal unit As String, ByVal weight_amount As String, ByVal weight_unit As String, ByVal is_non_standard As String) As Parcels
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

    Public Shared Function dhl_loadCod(ByVal amount As String, ByVal currency As String) As MoneyData
        Dim obj_service As New MoneyData
        obj_service.amount = amount
        obj_service.currency = currency
        '' i_shipment.i_cod = obj_service
        Return obj_service
    End Function

    Public Shared Function dhl_loadInsurance(ByVal amount As String, ByVal currency As String) As MoneyData
        Dim obj_service As New MoneyData
        obj_service.amount = amount
        obj_service.currency = currency
        ''i_shipment.i_insurance = obj_service
        Return obj_service
    End Function

    Public Shared Function dhl_loadAdditionalServices(ByVal add_service As String) As String
        Dim obj_service As String = add_service
        ''i_shipment.i_additional_services.Add(obj_service)
        Return obj_service
    End Function

    Public Shared Function REST_POST_CreateShipment(ByVal obj_session As DHL_DE_Session, ByVal obj_ship As Shipment) As ShipmentResponse
        Dim shipment_obiekt As New ShipmentResponse
        exception = ""
        Try


            Dim req_temp As String = ""
            req_url = url & "/orders"
            req_url_desc = ""

            ''##counter code ISO 3166-1 alpha-3 [3znaki]
            Dim rest_shipper As String = "'shipper': {
                'name1': 'Dajar GmbH c/o Schenker Deutschland',
    	        'name2': 'AG TNL Herr Thews, Frau Lenz',
                'addressStreet': 'Schwanenfeldstraße 11',
                'postalCode': '13627',
                'city': 'Berlin',
                'country': 'DEU',
                'email': 'shop@dajar.de',
                'phone': '00493034965186'
                },"

            Dim rest_receiver As String = ""
            If obj_session.packStation <> "" Then
                rest_receiver = "'consignee': {
                'name1': '" & obj_ship.i_receiver.name & "',
                'name2': '" & obj_session.packStation & "',
                'email': '" & obj_ship.i_receiver.email & "',
                'phone': '" & obj_ship.i_receiver.phone & "',
                'addressStreet': '" & obj_ship.i_receiver.address_street & " " & obj_ship.i_receiver.address_building_number & "',
                'city': '" & obj_ship.i_receiver.address_city & "',
                'postalCode': '" & obj_ship.i_receiver.address_post_code & "',
                'country': '" & dajarSWMagazyn_MIA.MyFunction.Convert_ISO_2_TO_3(obj_ship.i_receiver.address_country_code) & "',
                'locker': {
		        'name': 'packstation " & obj_ship.i_receiver.address_building_number & "',
		        'lockerID': '" & obj_ship.i_receiver.address_building_number & "',
		        'postNumber': '" & obj_session.packStation & "',
                'city': '" & obj_ship.i_receiver.address_city & "',
                'postalCode': '" & obj_ship.i_receiver.address_post_code & "'
	            }
                },"
            Else
                rest_receiver = "'consignee': {
                'name1': '" & obj_ship.i_receiver.name & "',
                'name2': '" & obj_ship.i_receiver.name2 & "',
                'email': '" & obj_ship.i_receiver.email & "',
                'phone': '" & obj_ship.i_receiver.phone & "',
                'addressStreet': '" & obj_ship.i_receiver.address_street & " " & obj_ship.i_receiver.address_building_number & "',
                'city': '" & obj_ship.i_receiver.address_city & "',
                'postalCode': '" & obj_ship.i_receiver.address_post_code & "',
                'country': '" & dajarSWMagazyn_MIA.MyFunction.Convert_ISO_2_TO_3(obj_ship.i_receiver.address_country_code) & "'
                },"
            End If

            Dim rest_parcel_list As String = ""
            For Each id_parcel In obj_ship.i_parcels
                Dim obj_rest_parcel As String = ""

                ''<asp:ListItem Value="V01PAK">DHL PAKET</asp:ListItem>
                ''<asp:ListItem Value="V53WPAK">DHL PAKET International</asp:ListItem>
                ''<asp:ListItem Value="V54EPAK">DHL Europaket</asp:ListItem>
                ''<asp:ListItem Value="V62WP">Warenpost</asp:ListItem>
                ''<asp:ListItem Value="V66WPI">Warenpost International</asp:ListItem>

                obj_session.billingNumber = dhl_change_billing_number(obj_session, obj_session.i_shipment.i_service)

                ''If obj_session.i_shipment.i_service = "V53WPAK" Then
                ''    obj_session.billingNumber = obj_session.billingNumber.Replace("01", "53")
                ''ElseIf obj_session.i_shipment.i_service = "V54EPAK" Then
                ''    obj_session.billingNumber = obj_session.billingNumber.Replace("01", "54")
                ''ElseIf obj_session.i_shipment.i_service = "V62WP" Then
                ''    obj_session.billingNumber = obj_session.billingNumber.Replace("01", "62")
                ''ElseIf obj_session.i_shipment.i_service = "V66WPI" Then
                ''    obj_session.billingNumber = obj_session.billingNumber.Replace("01", "66")
                ''End If

                obj_rest_parcel &= "{
            'product': '" & obj_session.i_shipment.i_service & "',
            'billingNumber': '" & obj_session.billingNumber & "',
            'refNo': '" & obj_ship.i_reference.ToString & "',
            'costCenter': '" & obj_ship.i_mpk.ToString & "',
            'creationSoftware': 'dajarSWMagazyn_MIA',"

                obj_rest_parcel &= rest_shipper
                obj_rest_parcel &= rest_receiver

                obj_rest_parcel &= "'details': {
            'dim':{'uom':'" & id_parcel.dimensions_unit & "',
                'length': " & id_parcel.dimensions_length & ",
                'width': " & id_parcel.dimensions_width & ",
                'height': " & id_parcel.dimensions_height & "
            },
            'weight': {'uom':'" & id_parcel.weight_unit & "',
                'value': '" & id_parcel.weight_amount & "'
            }
            }"

                obj_rest_parcel &= "},"
                rest_parcel_list += obj_rest_parcel
            Next

            rest_parcel_list = rest_parcel_list.Substring(0, rest_parcel_list.Count - 1)
            Dim rest_parcel As String = "{'shipments': [" & rest_parcel_list & "]}"
            req_temp &= rest_parcel

            ''''If obj_ship.i_cod IsNot Nothing Then
            ''''    If obj_ship.i_cod.amount <> "" Then
            ''''        Dim obj_service As String = "'cod': {
            ''''            'amount': " & obj_ship.i_cod.amount.ToString & ",
            ''''            'currency': '" & obj_ship.i_cod.currency.ToString & "'
            ''''            },"
            ''''        req_temp &= obj_service
            ''''    End If
            ''''End If

            ''''If obj_ship.i_insurance IsNot Nothing Then
            ''''    If obj_ship.i_insurance.amount <> "" Then
            ''''        Dim obj_service As String = "'insurance': {
            ''''        'amount': " & obj_ship.i_insurance.amount.ToString & ",
            ''''        'currency': '" & obj_ship.i_insurance.currency.ToString & "'
            ''''        },"
            ''''        req_temp &= obj_service
            ''''    End If
            ''''End If

            ''''req_temp &= "'service': '" & obj_ship.i_service.ToString & "',"

            ''''Dim rest_custom_attributes As String = ""
            ''''    If obj_ship.i_custom_attributes.target_point <> "" Then
            ''''        Dim obj_service As String = "'custom_attributes': {
            ''''    'target_point': '" & obj_ship.i_custom_attributes.target_point.ToString & "',
            ''''    'sending_method': 'dispatch_order'
            ''''    },"
            ''''        req_temp &= obj_service
            ''''    ElseIf obj_ship.i_service.ToString = "inpost_courier_allegro" Then
            ''''        req_temp &= "'custom_attributes' :  {
            ''''    'sending_method': 'dispatch_order'
            ''''    },"
            ''''    End If

            ''''    Dim rest_service_list As String = ""
            ''''    For Each id_service In obj_ship.i_additional_services
            ''''        rest_service_list &= "'" & id_service & "',"
            ''''    Next

            ''''    If rest_service_list <> "" Then
            ''''        rest_service_list = rest_service_list.Substring(0, rest_service_list.Count - 1)
            ''''        Dim rest_service As String = "'additional_services': [" & rest_service_list & "],"
            ''''        req_temp &= rest_service
            ''''    End If

            Dim stringJson As String = req_temp
            stringJson = stringJson.Replace("'", """")

            ''Dim varses = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CheckoutFormAddParcelTrackingNumber)(stringJson)

            Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/dhl_de/"
            dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)


            j_string = stringJson
            ''jdata = Encoding.UTF8.GetBytes(j_string)
            req_return = DhlParcelDeRestAPI.SendRequest_POST(req_url + req_url_desc, obj_session.access_token, j_string, "Basic ", "application/json", "application/json")
            If req_return.Contains("error") = False And req_return.Contains("Bad Request") = False Then

                ''Dim j_rss As JObject = JObject.Parse(req_return).SelectToken("ShipmentResponse").SelectToken("ShipmentResults")
                ''shipment_obiekt.shipment_id = j_rss.Property("ShipmentIdentificationNumber")
                ''shipment_obiekt.tracking_number = New List(Of String)

                Dim myItems As JArray = DhlParcelDeRestAPI.JSON_SelectArray(req_return, "items")
                Dim t_list As New List(Of String)
                t_list = DhlParcelDeRestAPI.json_array_to_list(myItems)
                For Each t In t_list
                    Dim tracking_number As String = DhlParcelDeRestAPI.JSON_SelectToken(t, "shipmentNo")
                    shipment_obiekt.tracking_number.Add(tracking_number)
                    If shipment_obiekt.shipment_id = "" Then
                        shipment_obiekt.shipment_id = tracking_number
                    End If

                    Dim j_rss As JObject = JObject.Parse(t).SelectToken("label")
                    Dim b64 As String = j_rss.Property("b64")
                    Dim pdfContent As Byte() = Convert.FromBase64String(b64)
                    ''Dim fileStream As New FileStream(dir_stream, FileMode.Create, FileAccess.Write)
                    Dim dir_stream As String = dir_export & "\" & tracking_number.ToString & ".pdf"

                    File.WriteAllBytes(dir_stream, pdfContent)

                Next
            ElseIf req_return.Contains("validationMessages") = True Then
                Dim myItems As JArray = DhlParcelDeRestAPI.JSON_SelectArray(req_return, "items")
                Dim t_list As New List(Of String)
                t_list = UpsRestAPI.json_array_to_list(myItems)
                For Each t In t_list
                    myItems = DhlParcelDeRestAPI.JSON_SelectArray(t, "validationMessages")
                    Dim t_list_message As New List(Of String)
                    t_list_message = UpsRestAPI.json_array_to_list(myItems)

                    For Each tt In t_list_message
                        exception &= tt.ToString
                    Next
                Next

                shipment_obiekt.shipment_id = "1111"
                Return shipment_obiekt
            End If

            Return shipment_obiekt
            ''exception = "[REST_POST_CreateShipment]" & req_return.ToString
            ''    REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "Y")
            ''    myTrackingNumber = j_return.Replace("""", "")
            ''Else
            ''    REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "N")
            ''End If
        Catch ex As Exception
            exception = "[REST_POST_CreateShipment]" & ex.Message.ToString
            shipment_obiekt.shipment_id = "1111"
            Return shipment_obiekt
        End Try
    End Function

    Public Shared Function REST_DELETE_DeleteShipment(ByVal obj_session As DHL_DE_Session, ByVal shipment_id As String) As String
        Try
            Dim req_temp As String = ""
            req_url = url & "/orders?profile=STANDARD_GRUPPENPROFIL&shipment=" & shipment_id
            req_url_desc = ""

            Dim stringJson As String = req_temp
            stringJson = stringJson.Replace("'", """")

            j_string = stringJson
            ''jdata = Encoding.UTF8.GetBytes(j_string)
            req_return = DhlParcelDeRestAPI.SendRequest_DELETE(req_url + req_url_desc, obj_session.access_token, j_string, "Basic ", "application/json", "application/json")
            If req_return.Contains("OK") = True Then
                Dim myItems As JArray = DhlParcelDeRestAPI.JSON_SelectArray(req_return, "items")
                Dim t_list As New List(Of String)
                t_list = DhlParcelDeRestAPI.json_array_to_list(myItems)
                For Each t In t_list
                    shipment_id = DhlParcelDeRestAPI.JSON_SelectToken(t, "shipmentNo")
                Next
                Return "OK"
            End If



            exception = "[REST_DELETE_DeleteShipment]" & req_return.ToString
            Return exception

        Catch ex As Exception
            exception = "[REST_DELETE_DeleteShipment]" & ex.Message.ToString
            Return "1111"
        End Try
    End Function

    Public Shared Function REST_GET_LabelOfShipment(ByVal tracking_number As String) As String
        Try
            Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/dhl_de/"
            dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)
            Dim dir_stream As String = dir_export & "\" & tracking_number.ToString & ".pdf"

            Dim req_temp As String = ""
            req_url = url & "/orders?shipment=" & tracking_number
            req_url_desc = ""

            Dim stringJson As String = req_temp
            stringJson = stringJson.Replace("'", """")

            j_string = stringJson
            ''req_return = DhlParcelDeRestAPI.SavePdfFileSendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/pdf", "application/json", dir_stream)
            req_return = DhlParcelDeRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "Basic ", "application/json", "application/json")
            If req_return.Contains("OK") = True Then
                Dim myItems As JArray = DhlParcelDeRestAPI.JSON_SelectArray(req_return, "items")
                Dim t_list As New List(Of String)
                t_list = DhlParcelDeRestAPI.json_array_to_list(myItems)
                For Each t In t_list
                    Dim label As String = DhlParcelDeRestAPI.JSON_SelectToken(t, "label")
                    If label.Contains("b64") Then
                        Dim b64_t() As String = label.Split(",")

                        Dim b64 As String = b64_t(0).Substring(b64_t(0).IndexOf("b64: ") + 5, b64_t(0).Length - b64_t(0).IndexOf("b64: ") - 5)
                        Dim pdfContent As Byte() = Convert.FromBase64String(b64)
                        ''Dim fileStream As New FileStream(dir_stream, FileMode.Create, FileAccess.Write)
                        File.WriteAllBytes(dir_stream, pdfContent)
                    End If
                Next
                Return tracking_number.ToString
            End If


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

    Public Shared Function REST_GET_MultipleLabelOfShipment(ByVal filename As String, ByVal tracking_list As List(Of String)) As String
        Try
            Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/dhl_de/"
            dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)
            Dim dir_stream As String = dir_export & "\" & filename.ToString & ".pdf"

            Dim req_temp As String = ""
            req_url = url & "/orders?"
            req_url_desc = ""
            For Each tracking_id In tracking_list
                req_url_desc &= "shipment=" & tracking_id & "&"
            Next

            If req_url_desc.Length > 1 Then
                req_url_desc = req_url_desc.Substring(0, req_url_desc.Count - 1)
            End If

            Dim stringJson As String = req_temp
            stringJson = stringJson.Replace(" '", """")

            j_string = stringJson
            ''req_return = DhlParcelDeRestAPI.SavePdfFileSendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/pdf", "application/json", dir_stream)
            req_return = DhlParcelDeRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "Basic ", "application/json", "application/json")
            If req_return.Contains("OK") = True Then
                Dim myItems As JArray = DhlParcelDeRestAPI.JSON_SelectArray(req_return, "items")
                Dim t_list As New List(Of String)
                t_list = DhlParcelDeRestAPI.json_array_to_list(myItems)
                For Each t In t_list
                    Dim shipment_no As String = DhlParcelDeRestAPI.JSON_SelectToken(t, "shipmentNo")
                    Dim label As String = DhlParcelDeRestAPI.JSON_SelectToken(t, "label")
                    If label.Contains("b64") Then
                        Dim b64_t() As String = label.Split(",")
                        Dim b64 As String = b64_t(0).Substring(b64_t(0).IndexOf("b64: ") + 5, b64_t(0).Length - b64_t(0).IndexOf("b64: ") - 5)
                        Dim pdfContent As Byte() = Convert.FromBase64String(b64)
                        ''Dim fileStream As New FileStream(dir_stream, FileMode.Create, FileAccess.Write)
                        dir_stream = dir_export & "\" & shipment_no.ToString & ".pdf"
                        File.WriteAllBytes(dir_stream, pdfContent)
                    End If
                Next
                Return filename.ToString
            End If


            If req_return = "1" Then
                ''Return ("inpost_" & tracking_number.ToString & ".pdf")
                Return filename.ToString
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
        req_return = DhlParcelDeRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/json", "application/json")

        If req_return.Contains("items") = True Then
            Dim j_rss As JObject = JObject.Parse(req_return)
            ''Console.WriteLine(j_rss.SelectToken("services").ToString())
            Dim j_servies As JArray = j_rss.SelectToken("items")
            Dim j_list, t_list As New List(Of String)
            j_list = DhlParcelDeRestAPI.json_array_to_list(j_servies)
            For Each a In j_list
                shipment_id = DhlParcelDeRestAPI.JSON_SelectToken(a.ToString, "id")
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
        req_return = DhlParcelDeRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/json", "application/json")
        If req_return.Contains("items") = True Then
            Dim j_rss As JObject = JObject.Parse(req_return)
            ''Console.WriteLine(j_rss.SelectToken("services").ToString())
            Dim j_servies As JArray = j_rss.SelectToken("items")
            Dim j_list, t_list As New List(Of String)
            j_list = DhlParcelDeRestAPI.json_array_to_list(j_servies)
            For Each a In j_list
                status = DhlParcelDeRestAPI.JSON_SelectToken(a.ToString, "status")
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
        req_return = DhlParcelDeRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/json", "application/json")
        If req_return.Contains("items") = True Then
            Dim j_rss As JObject = JObject.Parse(req_return)
            ''Console.WriteLine(j_rss.SelectToken("services").ToString())
            Dim j_servies As JArray = j_rss.SelectToken("items")
            Dim j_list, t_list As New List(Of String)
            j_list = DhlParcelDeRestAPI.json_array_to_list(j_servies)
            For Each a In j_list
                Dim j_servies_ofe As JArray = DhlParcelDeRestAPI.JSON_SelectArray(a.ToString, "offers")
                Dim j_list_ofe, t_list_ofe As New List(Of String)
                j_list_ofe = DhlParcelDeRestAPI.json_array_to_list(j_servies_ofe)
                For Each a_ofe In j_list_ofe
                    offer_id = DhlParcelDeRestAPI.JSON_SelectToken(a_ofe.ToString, "id")
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
        req_return = DhlParcelDeRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/json", "application/json")
        If req_return.Contains("items") = True Then
            Dim j_rss As JObject = JObject.Parse(req_return)
            ''Console.WriteLine(j_rss.SelectToken("services").ToString())
            Dim j_servies As JArray = j_rss.SelectToken("items")
            Dim j_list, t_list As New List(Of String)
            j_list = DhlParcelDeRestAPI.json_array_to_list(j_servies)
            For Each a In j_list
                tracking_number = DhlParcelDeRestAPI.JSON_SelectToken(a.ToString, "tracking_number")
                Exit For
            Next
        End If

        Return tracking_number
    End Function

    Public Shared Function REST_GET_Manifest(ByVal start_date As String, ByVal billingnumber As String) As List(Of String)
        Try
            Dim manifestlist_dhlde As New List(Of String)
            Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/dhl_de/"
            dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)

            Dim req_temp As String = ""
            req_url = url & "/manifests"
            req_url_desc = "?billingNumber=" & billingnumber & "&date=" & start_date

            Dim stringJson As String = req_temp
            stringJson = stringJson.Replace("'", """")

            j_string = stringJson
            ''req_return = DhlParcelDeRestAPI.SavePdfFileSendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/csv", "application/json", dir_stream)
            req_return = DhlParcelDeRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "Basic ", "application/json", "application/json")
            If req_return.Contains("OK") = True Then
                Dim manifest_id As Integer = 0
                Dim myItems As JArray = DhlParcelDeRestAPI.JSON_SelectArray(req_return, "manifest")
                Dim t_list As New List(Of String)
                t_list = DhlParcelDeRestAPI.json_array_to_list(myItems)
                For Each t In t_list
                    ''Dim label As String = t
                    Dim label_b64 As String = DhlParcelDeRestAPI.JSON_SelectToken(t, "b64")
                    If label_b64 <> "" Then
                        Dim pplist_dhlde As String = "manifest_dhlde_" & dajarSWMagazyn_MIA.MyFunction.DataEval & "_" & manifest_id & ".pdf"
                        manifestlist_dhlde.Add(pplist_dhlde)
                        Dim dir_stream As String = dir_export & "\" & pplist_dhlde
                        Dim pdfContent As Byte() = Convert.FromBase64String(label_b64)
                        ''Dim fileStream As New FileStream(dir_stream, FileMode.Create, FileAccess.Write)
                        File.WriteAllBytes(dir_stream, pdfContent)
                        manifest_id += 1
                    End If
                Next
            End If

            Return manifestlist_dhlde

        Catch ex As Exception
            exception = "[REST_GET_Manifest]" & ex.Message.ToString
            Return New List(Of String)
            Exit Function
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
        req_return = DhlParcelDeRestAPI.SendRequest_DELETE(req_url + req_url_desc, access_token, j_string, "Basic ", "application/json", "application/json")
        If req_return.Contains("items") = True Then
            Dim j_rss As JObject = JObject.Parse(req_return)
            ''Console.WriteLine(j_rss.SelectToken("services").ToString())
            Dim j_servies As JArray = j_rss.SelectToken("items")
            Dim j_list, t_list As New List(Of String)
            j_list = DhlParcelDeRestAPI.json_array_to_list(j_servies)
            For Each a In j_list
                tracking_number = DhlParcelDeRestAPI.JSON_SelectToken(a.ToString, "tracking_number")
                Exit For
            Next
        End If

        Return tracking_number
    End Function

End Class
