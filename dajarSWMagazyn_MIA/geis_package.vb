Imports Oracle.ManagedDataAccess.Client
Imports System.IO
Imports System.Data
Imports dajarSWMagazyn_MIA.GService

Public Class geis_package

    Public Shared req_export As GService.RequestOfExportRequestHUsxwOXq = New GService.RequestOfExportRequestHUsxwOXq
    Public Shared g_login As String = ""
    Public Shared g_pass As String = ""
    Public Shared g_lang As String = "5"
    Public Shared exception As String = ""

    Public Shared Function geis_login(ByVal schemat As String, ByVal mag_id As String, ByVal firma_id As String) As GService.GServiceClient
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Console.WriteLine("geis_login...")

            Dim sqlexp = "SELECT LOGIN,HASLO FROM dp_swm_mia_firma_login WHERE SCHEMAT='" & schemat & "' AND AKTYWNA='X' AND MAG='" & mag_id & "' AND FIRMA_ID='" & firma_id & "'"
            ''Dim sqlexp = "SELECT LOGIN,HASLO FROM dp_swm_mia_firma_login WHERE SCHEMAT='" & schemat & "' AND MAG='" & mag_id & "' AND FIRMA_ID='" & firma_id & "'"
            Dim cmd As OracleCommand = New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            Try
                While dr.Read()
                    g_login = dr.Item(0).ToString
                    g_pass = dr.Item(1).ToString
                    g_lang = "5"
                End While

            Catch ex As System.ArgumentOutOfRangeException
                Console.WriteLine(ex)
            End Try
            dr.Close()
            cmd.Dispose()

            req_export.RequestObject = New GService.ExportRequest

            Dim client As New GService.GServiceClient
            ''client.IsHealthy("")
            conn.Close()
            Return client

        End Using
    End Function

    Public Shared Sub geis_loadHeader()
        Dim req_header = New GService.RequestHeader
        req_header.CustomerCode = g_login
        req_header.Password = g_pass
        req_header.Language = g_lang

        req_export.Header = req_header
    End Sub

    Public Shared Sub geis_loadDeliveryAddress(ByVal Name As String, ByVal Street As String, ByVal house_num As String, ByVal apart_num As String, ByVal City As String, ByVal ZipCode As String, ByVal Country As String)
        Dim deliveryAddress As GService.Address = New GService.Address
        deliveryAddress.Name = Name
        deliveryAddress.Street = Street & " " & house_num & " " & apart_num
        deliveryAddress.Street = deliveryAddress.Street.ToString.Trim
        deliveryAddress.City = City
        deliveryAddress.ZipCode = ZipCode
        deliveryAddress.Country = Country

        req_export.RequestObject.DeliveryAddress = deliveryAddress
    End Sub

    Public Shared Sub geis_loadCoverAddress(ByVal Name As String, ByVal Name2 As String, ByVal Street As String, ByVal City As String, ByVal ZipCode As String, ByVal Country As String)
        Dim coverAddress As GService.Address = New GService.Address
        coverAddress.Name = Name
        coverAddress.Name2 = Name2
        coverAddress.Street = Street
        coverAddress.City = City
        coverAddress.ZipCode = ZipCode
        coverAddress.Country = Country
        req_export.RequestObject.CoverAddress = coverAddress
    End Sub


    Public Shared Sub geis_loadDeliveryContact(ByVal FullName As String, ByVal Email As String, ByVal Phone As String)
        Dim deliveryContact As GService.Contact = New GService.Contact
        deliveryContact.FullName = FullName
        deliveryContact.Email = Email
        deliveryContact.Phone = Phone

        ''deliveryContact.FullName = "jan kowalski"
        ''deliveryContact.Email = "jan.kowalski@onet.pl"
        ''deliveryContact.Phone = "123456789"

        req_export.RequestObject.DeliveryContact = deliveryContact
    End Sub

    Public Shared Sub geis_loadNote(ByVal Note As String)
        req_export.RequestObject.Note = Note
    End Sub

    Public Shared Sub geis_loadNoteDriver(ByVal NoteDriver As String)
        req_export.RequestObject.NoteDriver = NoteDriver
    End Sub

    Public Shared Sub geis_loadNoteReference(ByVal Reference As String)
        req_export.RequestObject.Reference = Reference
    End Sub

    Public Shared Sub geis_loadPickupDate(ByVal PickUpDate As String)
        req_export.RequestObject.PickUpDate = PickUpDate
    End Sub

    Public Shared Sub geis_loadPackage(ByVal package_count As String)
        req_export.RequestObject.ExportItems = New GService.ExportItem() {New GService.ExportItem}
        ReDim req_export.RequestObject.ExportItems(package_count)
    End Sub

    Public Shared Sub geis_AddPackage(ByVal package_id As String, ByVal type As String, ByVal quantity As String, ByVal weight As String, ByVal width As String, ByVal height As String, ByVal length As String, ByVal Reference As String, ByVal Description As String)
        Dim package = New GService.ExportItem
        package.Type = type
        package.CountItems = quantity
        package.Weight = weight
        package.Width = width
        package.Height = height
        package.Length = length
        package.Reference = Reference
        package.Description = Description
        req_export.RequestObject.ExportItems(package_id) = package
    End Sub

    Public Shared Sub geis_loadService(ByVal package_count As String)
        req_export.RequestObject.ExportServices = New GService.ExportService() {New GService.ExportService}
        ReDim req_export.RequestObject.ExportServices(package_count)
    End Sub

    Public Shared Function geis_getLabels(ByVal client As GService.GServiceClient, ByVal shipment_id As String) As String
        Try
            Dim req_label As New GService.RequestOfLabelRequestHUsxwOXq
            geis_loadHeader()

            req_label.Header = req_export.Header
            req_label.RequestObject = New GService.LabelRequest
            req_label.RequestObject.DistributionChannel = 2
            req_label.RequestObject.ShipmentNumbers() = New GService.LabelItem(0) {}
            req_label.RequestObject.ShipmentNumbers(0) = New GService.LabelItem
            req_label.RequestObject.ShipmentNumbers(0).ShipmentNumber = shipment_id
            req_label.RequestObject.Format = 5
            req_label.RequestObject.Position = 1

            Dim resp_label As New GService.ResponseOfLabelRequestLabelResponsepMtQ0fwN
            resp_label = client.GetLabel(req_label)
            Console.WriteLine("client.GetLabel...")

            If resp_label.ResponseObject IsNot Nothing Then
                Dim bytes() As Byte = resp_label.ResponseObject.LabelData(0).Data

                Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/geis/"
                dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)

                System.IO.File.WriteAllBytes(dir_export & "\geis_" & req_label.RequestObject.ShipmentNumbers(0).ShipmentNumber.ToString & ".pdf", bytes)
                Return ("geis_" & req_label.RequestObject.ShipmentNumbers(0).ShipmentNumber.ToString & ".pdf")
            End If

            If resp_label.ErrorCode <> "0000" Then
                exception = "[geis_getLabels][" & resp_label.ErrorCode.ToString & "] / " & resp_label.ErrorMessage.ToString
            End If

            Return resp_label.ErrorCode.ToString
        Catch ex As Exception
            exception = "[geis_getLabels]" & ex.Message.ToString
            Return "1111"
        End Try

    End Function

    Public Shared Function geis_getPickupList(ByVal client As GService.GServiceClient, ByVal firma_id As String, ByVal data As String, ByVal shipment_list As List(Of String)) As String
        Try
            Dim req_pickup_list As New GService.RequestOfPickupListRequestHUsxwOXq
            geis_loadHeader()

            req_pickup_list.Header = req_export.Header
            req_pickup_list.RequestObject = New GService.PickupListRequest
            req_pickup_list.RequestObject.DistributionChannel = 2
            req_pickup_list.RequestObject.ExpeditionCreateDate = data

            req_pickup_list.RequestObject.ShipmentsNumbers() = New GService.PickupListShipmItem(shipment_list.Count - 1) {}
            Dim shipmentID As Integer = 0
            For Each ship_id In shipment_list
                req_pickup_list.RequestObject.ShipmentsNumbers(shipmentID) = New GService.PickupListShipmItem
                req_pickup_list.RequestObject.ShipmentsNumbers(shipmentID).ShipmentNumber = ship_id
                shipmentID += 1
            Next

            Dim resp_pickup As New GService.ResponseOfPickupListRequestPickupListResponsepMtQ0fwN
            resp_pickup = client.GetPickupList(req_pickup_list)
            Console.WriteLine("client.GetPickupList...")
            Dim pickup_data As String = req_pickup_list.RequestObject.ExpeditionCreateDate.ToString.Substring(0, 10)

            If resp_pickup.ResponseObject IsNot Nothing Then
                Dim bytes() As Byte = resp_pickup.ResponseObject.PickupListData
                Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/geis/"
                dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)

                System.IO.File.WriteAllBytes(dir_export & "\pplist_" & firma_id & "_" & dajarSWMagazyn_MIA.MyFunction.DataEval & ".pdf", bytes)
                Return ("pplist_" & firma_id & "_" & dajarSWMagazyn_MIA.MyFunction.DataEval & ".pdf")
            End If

            If resp_pickup.ErrorCode <> "0000" Then
                exception = "[geis_getPickupList][" & resp_pickup.ErrorCode.ToString & "] / " & resp_pickup.ErrorMessage.ToString
            End If

            Return resp_pickup.ErrorCode.ToString

        Catch ex As Exception
            exception = "[geis_getPickupList]" & ex.Message.ToString
            Return "1111"
        End Try

    End Function

    Public Shared Function geis_deleteShipment(ByVal client As GService.GServiceClient, ByVal shipment_id As String) As String
        Try
            Dim req_delete As New GService.RequestOfDeleteShipmentRequestHUsxwOXq
            geis_loadHeader()

            req_delete.Header = req_export.Header
            req_delete.RequestObject = New GService.DeleteShipmentRequest
            ''req_delete.RequestObject.DistributionChannel = 2
            req_delete.RequestObject.ShipmentsNumbers() = New GService.DeleteShipmentItem(0) {}
            req_delete.RequestObject.ShipmentsNumbers(0) = New GService.DeleteShipmentItem
            req_delete.RequestObject.ShipmentsNumbers(0).ShipmentNumber = shipment_id

            Dim resp_delete As New GService.ResponseOfDeleteShipmentRequestDeleteShipmentResponsepMtQ0fwN
            resp_delete = client.DeleteShipment(req_delete)
            Console.WriteLine("client.GetLabel...")

            If resp_delete.ResponseObject IsNot Nothing Then
                Return resp_delete.ResponseObject.ShipmentsNumbers(0).IsStorno.ToString
            End If
        Catch ex As Exception
            exception = "[geis_deleteShipment]" & ex.Message.ToString
            Return ""
        End Try

        Return ""
    End Function

    Public Shared Sub geis_AddService(ByVal package_id As String, ByVal type As String, ByVal Parameter_1 As String, ByVal iso_country As String)
        Dim service = New GService.ExportService
        service.Code = type
        service.Parameter_1 = Parameter_1

        If iso_country = "PL" Then
            If service.Code = "POJ" Or service.Code = "COD" Then
                service.Parameter_2 = "PLN"
            End If
        Else
            If service.Code = "POJ" Or service.Code = "COD" Then
                If iso_country = "CZ" Then
                    service.Parameter_2 = "CZK"
                    service.Parameter_4 = "CZ5203000000000117446073"
                ElseIf iso_country = "SK" Then
                    service.Parameter_2 = "EUR"
                    service.Parameter_4 = "SK7775000000000025862643"
                End If
            End If
        End If

        req_export.RequestObject.ExportServices(package_id) = service
    End Sub


    Public Shared Function geis_InsertExport(ByVal client As GService.GServiceClient) As String
        Try
            req_export.RequestObject.DistributionChannel = 2

            ''req_export.RequestObject.PickUpDate = DateTime.Now.ToShortDateString & "T" & DateTime.Now.ToLongTimeString

            geis_loadHeader()

            Dim resp_insexp As New GService.ResponseOfExportRequestExportpMtQ0fwN
            resp_insexp = client.InsertExport(req_export)
            Console.WriteLine("client.InsertExport...")

            If resp_insexp.ResponseObject IsNot Nothing Then
                If resp_insexp.ResponseObject.PackNumber IsNot Nothing Then
                    Return resp_insexp.ResponseObject.PackNumber.ToString
                End If
            End If

            exception = "[" & resp_insexp.ErrorCode.ToString & "] " & resp_insexp.ErrorMessage.ToString

        Catch ex As Exception
            exception = "[geis_InsertExport]" & ex.Message.ToString
            Return ""
        End Try

        Return ""
    End Function
End Class
