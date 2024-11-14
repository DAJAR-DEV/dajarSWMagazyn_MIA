Imports Oracle.ManagedDataAccess.Client
Imports System.IO
Imports System.Data
Imports Newtonsoft.Json.Linq
Imports dajarSWMagazyn_MIA.UpsRestAPI
Imports System.Web.Util

Public Class ups_de_package
    Public Shared url As String = ""
    Public Shared url_desc As String = ""
    Public Shared organization As String = ""
    Public Shared password As String = ""
    Public Shared access_token As String = ""
    Public Shared exception As String = ""
    Public Shared billingNumber As String = ""
    Public Shared login As String = ""
    Public Shared nr_sap As String = ""

    Public Shared req_url As String = ""
    Public Shared req_url_desc As String = ""
    Public Shared j_string As String = ""
    Public Shared req_return As String = ""
    Public Shared sqlexp As String = ""
    Public Shared j_return As String = ""


    Public Shared Sub ups_login(ByRef obj_session As UPS_DE_Session, ByVal schemat As String, ByVal mag_id As String, ByVal firma_id As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Console.WriteLine("ups_login...")

            Dim sqlexp = "SELECT LOGIN,HASLO,FIRMA_ID,NR_SAP FROM dp_swm_mia_firma_login WHERE SCHEMAT='" & schemat & "' AND MAG='" & mag_id & "' AND FIRMA_ID='" & firma_id & "' AND AKTYWNA='X'"
            Dim cmd As OracleCommand = New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            Try
                While dr.Read()
                    obj_session.organization = dr.Item(0).ToString
                    obj_session.password = dr.Item(1).ToString
                    obj_session.login = dr.Item(2).ToString
                    obj_session.nr_sap = dr.Item(3).ToString
                    Dim basic_access As String = dajarSWMagazyn_MIA.MyFunction.CONVERT_STR_TO_BASE64(obj_session.organization & ":" & obj_session.password)
                    obj_session.base64Encoded = basic_access

                    If conn.ConnectionString.Contains("10.1.0.30") Then
                        url = "https://onlinetools.ups.com"
                    Else
                        url = "https://wwwcie.ups.com"
                    End If
                    obj_session.url = url

                    access_token = ""
                    obj_session.billingNumber = obj_session.nr_sap

                End While

            Catch ex As System.ArgumentOutOfRangeException
                Console.WriteLine(ex)
            End Try
            dr.Close()
            cmd.Dispose()
            conn.Close()

        End Using
    End Sub

    Public Shared Sub ups_token(ByRef obj_session As UPS_DE_Session)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            ups_de_package.LoadTokenHistory(obj_session.login, obj_session, conn)
            If obj_session.token Is Nothing Then
                ups_de_package.REST_POST_CreateToken(obj_session, conn)
            End If

            If ups_de_package.CheckExpireTimeSpanToken(obj_session, conn) = True Then
                ups_de_package.DeactivateToken(obj_session, conn)
                ups_de_package.REST_POST_CreateToken(obj_session, conn)
            End If
            conn.Close()
        End Using

    End Sub

    Public Class UPS_DE_Session
        Public url As String = ""
        Public organization As String = ""
        Public password As String = ""
        Public access_token As String = ""
        Public i_shipment As Shipment = New Shipment
        Public billingNumber As String = ""
        Public packStation As String = ""
        Public login As String = ""
        Public nr_sap As String = ""
        Public token As UPS_DE_Token
        Public base64Encoded As String = ""
    End Class

    Public Class UPS_DE_Token
        Public access_token As String = ""
        Public token_type As String = ""
        Public refresh_token As String = ""
        Public expires_in As String = ""
        Public issued_at As String = ""
        Public scope As String = ""
        Public refresh_count As String = ""
        Public status As String = ""
    End Class

    Public Class ShipmentResponse
        Public shipment_id As String = ""
        Public tracking_number As New List(Of String)
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

    Public Shared Function REST_POST_CreateShipment(ByVal ups_session As UPS_DE_Session, ByVal obj_ship As Shipment, ByVal transactionSrc As String, ByVal walidacjaUlicaUPS As Boolean) As ShipmentResponse
        Dim shipment_obiekt As New ShipmentResponse
        Try
            Dim req_temp As String = ""
            req_url = ups_session.url & "/api/shipments/v2/ship"
            req_url_desc = ""

            Dim header_list As New List(Of UpsRestAPI.HeaderObject)
            Dim transId As String = System.Guid.NewGuid.ToString()
            header_list.Add(New UpsRestAPI.HeaderObject("transId", transId))
            header_list.Add(New UpsRestAPI.HeaderObject("transactionSrc", transactionSrc))

            req_temp &= "ShipmentRequest: {
                Request: {
                SubVersion: '1801',
                RequestOption: 'nonvalidate',
                TransactionReference: {CustomerContext: '" & obj_ship.i_reference & "'}
            },
            "

            req_temp &= "Shipment: {
                Description: '" & obj_ship.i_comments & "',"

            Dim rest_shipper As String = "Shipper: {
              Name: 'Dajar Sp. z o.o.',
              AttentionName: 'Dajar Sp. z o.o.',
              TaxIdentificationNumber: '6692296668',
              Phone: {
                Number: '501051302',
                Extension: ' '
              },
              ShipperNumber: '66338A',
              FaxNumber: '501051302',
              Address: {
                AddressLine: ['Rozana 9'],
                City: 'Koszalin',
                PostalCode: '75220',
                CountryCode: 'PL'
              }
            },"

            rest_shipper &= "ShipperFrom: {
              Name: 'Dajar Sp. z o.o.',
              AttentionName: 'Dajar Sp. z o.o.',
              TaxIdentificationNumber: '6692296668',
              Phone: {
                Number: '501051302',
                Extension: ' '
              },
              ShipperNumber: '66338A',
              FaxNumber: '501051302',
              Address: {
                AddressLine: ['Rozana 9'],
                City: 'Koszalin',
                PostalCode: '75220',
                CountryCode: 'PL'
              }
            },"

            req_temp &= rest_shipper

            Dim rest_receiver As String = ""
            Dim ups_address_line As String = (obj_ship.i_receiver.address_street.ToString & " " & obj_ship.i_receiver.address_building_number.ToString).Trim
            If walidacjaUlicaUPS Then
                Dim ups_address_line_list As New List(Of String)
                If ups_address_line.Length > 35 Then

                    Dim ups_t() As String = ups_address_line.Split(" ")
                    Dim ups_buf As String = ""
                    For Each ups_i In ups_t
                        If (ups_buf.Length + ups_i.Length) <= 35 Then
                            ups_buf &= ups_i & " "
                        Else
                            ups_address_line_list.Add(ups_buf)
                            ups_buf = ups_i & " "
                            Dim b As Boolean = True
                        End If
                    Next

                    ups_address_line_list.Add(ups_buf)
                End If

                If ups_session.packStation <> "" Then
                Else
                    ups_address_line = ""
                    For i As Integer = ups_address_line_list.Count - 1 To 0 Step -1
                        ups_address_line &= "'" & ups_address_line_list(i).ToString & "',"
                    Next
                    ups_address_line = ups_address_line.Substring(0, ups_address_line.Length - 1)
                    ups_address_line = ups_address_line.Trim

                    rest_receiver = "ShipTo: {
                    Name: '" & obj_ship.i_receiver.name & "',
                    AttentionName: '" & obj_ship.i_receiver.name2 & "',
                    Phone: {Number: '" & obj_ship.i_receiver.phone & "'},
                    EmailAddress: '" & obj_ship.i_receiver.email & "',
                    Address: {
                    AddressLine: [" & ups_address_line & "],
                    City: '" & obj_ship.i_receiver.address_city & "',
                    PostalCode: '" & obj_ship.i_receiver.address_post_code & "',
                    CountryCode: '" & obj_ship.i_receiver.address_country_code & "'
                    },
                    Residential: ' '
                    },"

                End If
            Else
                If ups_session.packStation <> "" Then
                Else
                    rest_receiver = "ShipTo: {
                  Name: '" & obj_ship.i_receiver.name & "',
                  AttentionName: '" & obj_ship.i_receiver.name2 & "',
                  Phone: {Number: '" & obj_ship.i_receiver.phone & "'},
                  EmailAddress: '" & obj_ship.i_receiver.email & "',
                  Address: {
                    AddressLine: ['" & ups_address_line & "'],
                    City: '" & obj_ship.i_receiver.address_city & "',
                    PostalCode: '" & obj_ship.i_receiver.address_post_code & "',
                    CountryCode: '" & obj_ship.i_receiver.address_country_code & "'
                  },
                  Residential: ' '
                },"

                End If
            End If

            req_temp &= rest_receiver

            Dim rest_parcel_list As String = ""
            For Each id_parcel In obj_ship.i_parcels
                Dim obj_rest_parcel As String = ""
                If id_parcel.template = "01" Then
                    obj_rest_parcel = "{Description: '',
                  Packaging: {
                    Code: '" & id_parcel.template & "',
                    Description: ''
                  },
                  PackageWeight: {
                    UnitOfMeasurement: {
                      Code: 'KGS',
                      Description: 'KGS'
                    },
                    Weight: '" & id_parcel.weight_amount & "'
                  }}"
                Else
                    obj_rest_parcel = "{Description: '',
                  Packaging: {
                    Code: '" & id_parcel.template & "',
                    Description: ''
                  },
                  Dimensions: {
                    UnitOfMeasurement: {
                      Code: 'CM',
                      Description: 'CM'
                    },
                    Length: '" & id_parcel.dimensions_length & "',
                    Width: '" & id_parcel.dimensions_width & "',
                    Height: '" & id_parcel.dimensions_height & "'
                  },
                  PackageWeight: {
                    UnitOfMeasurement: {
                      Code: 'KGS',
                      Description: 'KGS'
                    },
                    Weight: '" & id_parcel.weight_amount & "'
                  }}"
                End If

                rest_parcel_list += obj_rest_parcel & ","
            Next

            rest_parcel_list = rest_parcel_list.Substring(0, rest_parcel_list.Count - 1)
            Dim rest_parcel As String = "Package: [" & rest_parcel_list & "]"
            req_temp &= rest_parcel & ","

            req_temp &= "PaymentInformation: {
                ShipmentCharge: {
                    Type: '01',
                    BillShipper: {AccountNumber: '" & ups_session.billingNumber & "'}
                    }
                },
                ReferenceNumber: { Value: '" & obj_ship.i_reference & "'},
                Service: {
                    Code: '" & obj_ship.i_service & "',
                    Description: 'UPS Standard'
                    }
                }
            },
              LabelSpecification: {
                LabelImageFormat: {
                    Code: 'GIF',
                    Description: 'GIF'
                },
                HTTPUserAgent: 'Mozilla/4.5'
              }"

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

            Dim stringJson As String = "{" & req_temp & "}"
            stringJson = stringJson.Replace("'", """")

            ''Dim varses = Newtonsoft.Json.JsonConvert.DeserializeObject(Of CheckoutFormAddParcelTrackingNumber)(stringJson)

            j_string = stringJson
            ''jdata = Encoding.UTF8.GetBytes(j_string)
            req_return = UpsRestAPI.SendRequest_POST(req_url + req_url_desc, ups_session.access_token, j_string, "Bearer ", "application/x-www-form-urlencoded", "application/json", header_list)
            If req_return.Contains("ShipmentResponse") = True Then
                Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/ups_de/"
                dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)

                Dim j_rss As JObject = JObject.Parse(req_return).SelectToken("ShipmentResponse").SelectToken("ShipmentResults")
                shipment_obiekt.shipment_id = j_rss.Property("ShipmentIdentificationNumber")
                shipment_obiekt.tracking_number = New List(Of String)
                If j_rss.ToString.Contains("""PackageResults"": [") Then
                    Dim myItems As JArray = JSON_SelectArray(j_rss.ToString, "PackageResults")
                    Dim t_list As New List(Of String)
                    t_list = UpsRestAPI.json_array_to_list(myItems)
                    For Each t In t_list
                        Dim tracking_number As String = UpsRestAPI.JSON_SelectToken(t, "TrackingNumber")
                        j_rss = JObject.Parse(t).SelectToken("ShippingLabel")
                        Dim b64 As String = j_rss.Property("GraphicImage")
                        Dim pdfContent As Byte() = Convert.FromBase64String(b64)
                        ''Dim fileStream As New FileStream(dir_stream, FileMode.Create, FileAccess.Write)
                        Dim dir_stream As String = dir_export & "\" & tracking_number.ToString & ".gif"
                        File.WriteAllBytes(dir_stream, pdfContent)
                        shipment_obiekt.tracking_number.Add(tracking_number)

                    Next
                    ''j_rss = JObject.Parse(req_return).SelectToken("ShipmentResponse").SelectToken("ShipmentResults").SelectToken("PackageResults")
                    ''j_rss = JObject.Parse(req_return).SelectToken("ShipmentResponse").SelectToken("ShipmentResults").SelectToken("PackageResults").SelectToken("ShippingLabel")
                Else
                    j_rss = JObject.Parse(req_return).SelectToken("ShipmentResponse").SelectToken("ShipmentResults").SelectToken("PackageResults")
                    Dim tracking_number As String = UpsRestAPI.JSON_SelectToken(j_rss.ToString, "TrackingNumber")
                    j_rss = JObject.Parse(req_return).SelectToken("ShipmentResponse").SelectToken("ShipmentResults").SelectToken("PackageResults").SelectToken("ShippingLabel")

                    Dim b64 As String = j_rss.Property("GraphicImage")
                    Dim pdfContent As Byte() = Convert.FromBase64String(b64)
                    ''Dim fileStream As New FileStream(dir_stream, FileMode.Create, FileAccess.Write)
                    Dim dir_stream As String = dir_export & "\" & tracking_number.ToString & ".gif"
                    File.WriteAllBytes(dir_stream, pdfContent)
                    shipment_obiekt.tracking_number.Add(tracking_number)
                End If
            ElseIf req_return.Contains("message") = True Then
                Dim j_rss As JObject = JObject.Parse(req_return).SelectToken("response")
                Dim myItems As JArray = JSON_SelectArray(j_rss.ToString, "errors")
                Dim t_list As New List(Of String)
                t_list = UpsRestAPI.json_array_to_list(myItems)
                For Each t In t_list
                    Dim message As String = UpsRestAPI.JSON_SelectToken(t, "message")
                    exception = message
                Next

                shipment_obiekt.shipment_id = "1111"
                Return shipment_obiekt

            End If

            ''    REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "Y")
            ''    myTrackingNumber = j_return.Replace("""", "")
            ''Else
            ''    REST_InsertIntoLogTable(magento_rapi.url, magento_rapi.store_view, (url & url_desc).Replace(magento_rapi.url, "").Replace(magento_rapi.store_view, ""), j_string, j_return, "N")
            ''End If
            Return shipment_obiekt

        Catch ex As Exception
            exception = "[REST_POST_CreateShipment]" & ex.Message.ToString
            shipment_obiekt.shipment_id = "1111"
            Return shipment_obiekt
        End Try
    End Function

    Public Shared Function REST_GET_LabelOfShipment(ByRef ups_session As UPS_DE_Session, ByVal tracking_number As String, ByVal reference As String, ByVal transactionSrc As String) As String
        Try
            Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/ups_de/"
            dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)
            Dim dir_stream As String = dir_export & "\" & tracking_number.ToString & ".pdf"

            Dim req_temp As String = ""
            req_url = ups_session.url & "/api/labels/v2/recovery"
            req_url_desc = ""

            Dim header_list As New List(Of UpsRestAPI.HeaderObject)
            Dim transId As String = System.Guid.NewGuid.ToString()
            header_list.Add(New UpsRestAPI.HeaderObject("transId", transId))
            header_list.Add(New UpsRestAPI.HeaderObject("transactionSrc", transactionSrc))

            req_temp = "{LabelRecoveryRequest: {
              LabelSpecification: {
                HTTPUserAgent: 'Mozilla/4.5',
                LabelImageFormat: {Code: 'PDF'}
              },
              Request: {
                SubVersion: '1801',
                TransactionReference: {CustomerContext: 'zam123'}
              },
              TrackingNumber: '" & tracking_number & "',
					    ReferenceValues: {
						    ReferenceNumber: {Value: '" & reference & "'},
						    ShipperNumber: '" & ups_session.nr_sap & "'
					    },
              Translate: {
                Code: '01',
                DialectCode: 'US',
                LanguageCode: 'eng'
              }
            }
          }"

            Dim stringJson As String = req_temp
            stringJson = stringJson.Replace("'", """")

            j_string = stringJson
            ''req_return = UpsRestAPI.SavePdfFileSendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/pdf", "application/json", dir_stream)
            req_return = UpsRestAPI.SendRequest_POST(req_url + req_url_desc, ups_session.access_token, j_string, "Bearer ", "application/x-www-form-urlencoded", "application/json", header_list)
            If req_return.Contains("LabelRecoveryResponse") = True Then
                Dim j_rss As JObject = JObject.Parse(req_return).SelectToken("LabelRecoveryResponse").SelectToken("LabelResults").SelectToken("LabelImage")
                Dim b64 As String = j_rss.Property("GraphicImage")
                Dim pdfContent As Byte() = Convert.FromBase64String(b64)
                ''Dim fileStream As New FileStream(dir_stream, FileMode.Create, FileAccess.Write)
                File.WriteAllBytes(dir_stream, pdfContent)

                Return tracking_number.ToString
            ElseIf req_return.Contains("message") = True Then
                Dim j_rss As JObject = JObject.Parse(req_return).SelectToken("response")
                Dim myItems As JArray = JSON_SelectArray(j_rss.ToString, "errors")
                Dim t_list As New List(Of String)
                t_list = UpsRestAPI.json_array_to_list(myItems)
                For Each t In t_list
                    Dim message As String = UpsRestAPI.JSON_SelectToken(t, "message")
                    exception = message
                Next

                Return "1111"
            End If
        Catch ex As Exception
            exception = "[REST_GET_LabelOfShipment]" & ex.Message.ToString
            Return "1111"
        End Try

        Return "1111"
    End Function

    Public Shared Sub LoadTokenHistory(ByVal login As String, ByRef ups_session As UPS_DE_Session, ByVal conn As OracleConnection)

        Try
            Console.WriteLine("ladownie danych REST_API / access_token / refresh_token")
            sqlexp = "select access_token,token_type,refresh_token,expires_in,scope,refresh_count,issued_at,status from dp_swm_mia_rest_token where login='" & login & "' and status='approved' order by autodata desc"
            Dim cmd As OracleCommand = New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()

            Try
                While dr.Read()
                    ups_session.token = New UPS_DE_Token
                    ups_session.token.access_token = dr.Item(0).ToString
                    ups_session.access_token = ups_session.token.access_token
                    ups_session.token.token_type = dr.Item(1).ToString
                    ups_session.token.refresh_token = dr.Item(2).ToString
                    ups_session.token.expires_in = dr.Item(3).ToString
                    ups_session.token.scope = dr.Item(4).ToString
                    ups_session.token.refresh_count = dr.Item(5).ToString
                    ups_session.token.issued_at = dr.Item(6).ToString
                    ups_session.token.status = dr.Item(7).ToString

                    Exit While
                End While
            Catch ex As Exception
            End Try

            dr.Close()
            cmd.Dispose()
        Catch ex As Exception
            exception = "[LoadTokenHistory]" & ex.Message.ToString
            Exit Sub
        End Try
    End Sub


    Public Shared Function DeactivateToken(ByRef ups_session As UPS_DE_Session, ByVal conn As OracleConnection) As Boolean
        Try

            If ups_session.token IsNot Nothing Then
                sqlexp = "update dp_swm_mia_rest_token set status='expired' where access_token='" & ups_session.token.access_token & "' and login='" & ups_session.login & "'"
                sqlexp_result = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                Return True
            End If

            Return False
        Catch ex As Exception
            exception = "[DeactivateToken]" & ex.Message.ToString
            Exit Function
        End Try
    End Function

    Public Shared Function CheckExpireTimeSpanToken(ByRef ups_session As UPS_DE_Session, ByVal conn As OracleConnection) As Boolean
        Try

            If ups_session.token IsNot Nothing Then
                Dim time_expire As Int64 = Int64.Parse(ups_session.token.issued_at)
                Dim DateTimeOffset As DateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(time_expire)
                DateTimeOffset = DateTimeOffset.AddSeconds(Int64.Parse(ups_session.token.expires_in))
                Console.WriteLine("DateTimeOffset = " & DateTimeOffset.ToString)
                Console.WriteLine("DateTimeOffset.now() = " & DateTimeOffset.Now.ToString)

                If DateTimeOffset.Ticks < DateTimeOffset.Now.Ticks Then
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            exception = "[CheckExpireTimeSpanToken]" & ex.Message.ToString
            Exit Function
        End Try

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
            ''req_return = UpsRestAPI.SavePdfFileSendRequest_GET(req_url + req_url_desc, access_token, j_string, "", "application/csv", "application/json", dir_stream)
            req_return = UpsRestAPI.SendRequest_GET(req_url + req_url_desc, access_token, j_string, "Basic ", "application/json", "application/json")
            If req_return.Contains("OK") = True Then
                Dim manifest_id As Integer = 0
                Dim myItems As JArray = UpsRestAPI.JSON_SelectArray(req_return, "manifest")
                Dim t_list As New List(Of String)
                t_list = UpsRestAPI.json_array_to_list(myItems)
                For Each t In t_list
                    ''Dim label As String = t
                    Dim label_b64 As String = UpsRestAPI.JSON_SelectToken(t, "b64")
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
                Return manifestlist_dhlde
            End If

        Catch ex As Exception
            exception = "[REST_GET_Manifest]" & ex.Message.ToString
            Return New List(Of String)
            Exit Function
        End Try

        Return New List(Of String)

    End Function

    Public Shared Function REST_DELETE_Shipment(ByRef ups_session As UPS_DE_Session, ByVal shipment_id As String, ByVal trackingnumber As String, ByVal transactionSrc As String) As String
        Try
            Dim tracking_number As String = ""
            Dim req_temp As String = ""
            req_url = ups_session.url & "/api/shipments/v2403/void/cancel/" & shipment_id
            req_url_desc = "?trackingnumber=" & trackingnumber

            Dim header_list As New List(Of UpsRestAPI.HeaderObject)
            Dim transId As String = System.Guid.NewGuid.ToString()
            header_list.Add(New UpsRestAPI.HeaderObject("transId", transId))
            header_list.Add(New UpsRestAPI.HeaderObject("transactionSrc", transactionSrc))

            Dim stringJson As String = req_temp
            stringJson = stringJson.Replace("'", """")

            j_string = stringJson
            req_return = UpsRestAPI.SendRequest_DELETE(req_url + req_url_desc, ups_session.access_token, j_string, "Bearer ", "application/x-www-form-urlencoded", "application/json", header_list)
            If req_return.Contains("VoidShipmentResponse") = True Then
                req_return = req_return.Replace("&#xD;", "")
                If req_return.Contains(trackingnumber) Then
                    Return "Success"
                End If

            ElseIf req_return.Contains("message") = True Then
                Dim j_rss As JObject = JObject.Parse(req_return).SelectToken("response")
                Dim myItems As JArray = JSON_SelectArray(j_rss.ToString, "errors")
                Dim t_list As New List(Of String)
                t_list = UpsRestAPI.json_array_to_list(myItems)
                For Each t In t_list
                    Dim message As String = UpsRestAPI.JSON_SelectToken(t, "message")
                    exception = message
                Next

                Return "1111"

            End If

            Return tracking_number
        Catch ex As Exception
            exception = "[REST_DELETE_Shipment]" & ex.Message.ToString
            Return "1111"
            Exit Function
        End Try

        Return "1111"
    End Function

    Public Shared Function REST_POST_RefreshToken(ByRef ups_session As UPS_DE_Session, ByVal conn As OracleConnection) As Boolean
        Try

            Dim url As String = ups_session.url & "/security/v1/oauth/refresh?grant_type=refresh_token&refresh_token=" & ups_session.token.refresh_token
            j_string = "client_id=" & ups_session.organization
            ''jdata = Encoding.UTF8.GetBytes(j_string)
            j_return = UpsRestAPI.SendRequest_POST(url + url_desc, access_token, j_string, "Basic ", "application/x-www-form-urlencoded", "application/json", New List(Of UpsRestAPI.HeaderObject))

            If j_return.Contains("access_token") = True Then
                ups_session.token.access_token = JSON_SelectToken(j_return, "access_token")
                ups_session.access_token = ups_session.token.access_token
                ups_session.token.token_type = JSON_SelectToken(j_return, "token_type")
                ups_session.token.refresh_token = JSON_SelectToken(j_return, "refresh_token")
                ups_session.token.expires_in = JSON_SelectToken(j_return, "expires_in")
                ups_session.token.scope = JSON_SelectToken(j_return, "scope")
                ups_session.token.refresh_count = JSON_SelectToken(j_return, "refresh_count")
                Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                sqlexp = "INSERT INTO dp_swm_mia_REST_TOKEN VALUES('" & ups_session.login & "','" & ups_session.token.access_token & "','" & ups_session.token.token_type & "','" & ups_session.token.refresh_token & "','" & ups_session.token.expires_in & "','" & ups_session.token.scope & "','" & ups_session.token.refresh_count & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'))"
                Dim result As Boolean = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                Return True
            End If

            Return False
        Catch ex As Exception
            exception = "[REST_POST_RefreshToken]" & ex.Message.ToString
            Exit Function
        End Try
    End Function

    Public Shared Function REST_POST_CreateToken(ByRef ups_session As UPS_DE_Session, ByVal conn As OracleConnection) As Boolean
        Try
            ups_session.token = New UPS_DE_Token

            Dim url As String = ups_session.url & "/security/v1/oauth/token"
            url_desc = ""
            j_string = "grant_type=client_credentials"
            ''jdata = Encoding.UTF8.GetBytes(j_string)
            Dim header_list As New List(Of UpsRestAPI.HeaderObject)
            header_list.Add(New UpsRestAPI.HeaderObject("x-merchant-id", ups_session.nr_sap))
            j_return = UpsRestAPI.SendRequest_POST(url + url_desc, ups_session.base64Encoded, j_string, "Basic ", "application/x-www-form-urlencoded", "application/json", header_list)

            If j_return.Contains("access_token") = True Then
                ups_session.token.access_token = JSON_SelectToken(j_return, "access_token")
                ups_session.access_token = ups_session.token.access_token
                ups_session.token.token_type = JSON_SelectToken(j_return, "token_type")
                ups_session.token.issued_at = JSON_SelectToken(j_return, "issued_at")
                ''ups_rapi.token.refresh_token = JSON_SelectToken(j_return, "refresh_token")
                ups_session.token.expires_in = JSON_SelectToken(j_return, "expires_in")
                ups_session.token.scope = JSON_SelectToken(j_return, "scope")
                ups_session.token.refresh_count = JSON_SelectToken(j_return, "refresh_count")
                ups_session.token.status = JSON_SelectToken(j_return, "status")
                Dim timestamp As String = dajarSWMagazyn_MIA.MyFunction.DataEvalLogTime()
                sqlexp = "INSERT INTO dp_swm_mia_REST_TOKEN VALUES('" & ups_session.login & "','" & ups_session.organization & "','" & ups_session.token.issued_at & "','" & ups_session.token.access_token & "','" & ups_session.token.token_type & "','" & ups_session.token.refresh_token & "','" & ups_session.token.expires_in & "','" & ups_session.token.scope & "','" & ups_session.token.refresh_count & "','" & ups_session.token.status & "',TO_TIMESTAMP('" & timestamp & "', 'RR/MM/DD HH24:MI:SS'),'" & ups_session.url & "')"
                Dim result As Boolean = dajarSWMagazyn_MIA.MyFunction.ExecuteFromSqlExp(sqlexp, conn)
                Return True
            End If

            Return False
        Catch ex As Exception
            exception = "[REST_POST_CreateToken]" & ex.Message.ToString
            Exit Function
        End Try
    End Function

End Class
