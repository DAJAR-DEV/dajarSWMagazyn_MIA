Imports Oracle.ManagedDataAccess.Client
Imports System.IO
Imports System.Data
Imports dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop

Public Class dhl_parcelshop

    Public Shared auth As AuthdataStructure = New AuthdataStructure
    Public Shared request As CreateShipmentStructure = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.CreateShipmentStructure
    Public Shared exception As String = ""

    Public Shared Function dhl_login(ByVal schemat As String, ByVal mag_id As String, ByVal firma_id As String) As ServicePointMethodsService
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Console.WriteLine("ServicePointMethodsService...")
            Dim service As ServicePointMethodsService = New ServicePointMethodsService()
            ''Dim sqlexp = "SELECT LOGIN,HASLO FROM dp_swm_mia_firma_login WHERE SCHEMAT='" & schemat & "' AND AKTYWNA='X' AND MAG='" & mag_id & "' AND FIRMA_ID='" & firma_id & "'"
            Dim sqlexp = "SELECT LOGIN,HASLO FROM dp_swm_mia_firma_login WHERE SCHEMAT='" & schemat & "' AND AKTYWNA='X' AND MAG='" & mag_id & "' AND FIRMA_ID='" & firma_id & "'"
            Dim cmd As OracleCommand = New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            Try
                While dr.Read()
                    auth.username = dr.Item(0).ToString
                    auth.password = dr.Item(1).ToString
                End While

            Catch ex As System.ArgumentOutOfRangeException
                Console.WriteLine(ex)
            End Try
            dr.Close()
            cmd.Dispose()

            Console.WriteLine("AuthData...{0}/{1}", auth.username.ToString, auth.password.ToString)
            conn.Close()
            Return service
        End Using
    End Function

    Public Shared Sub dhl_loadRequestInfo(ByVal content As String, ByVal comment As String)
        request.shipmentData = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ShipmentStructure
        request.shipmentData.content = content
        request.shipmentData.comment = comment
        ''request.comment = request.comment.ToString.Substring(0, 100)
    End Sub

    Public Shared Sub dhl_loadShipmentInfo(ByVal serviceType As String, ByVal dropOffType As String, ByVal labelType As String)
        request.shipmentData.shipmentInfo = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ShipmentInfoStructure
        request.shipmentData.shipmentInfo.serviceType = serviceType
        request.shipmentData.shipmentInfo.dropOffType = dropOffType
        request.shipmentData.shipmentInfo.labelType = labelType
    End Sub

    Public Shared Sub dhl_loadShipmentTime(ByVal shipmentDate As String, ByVal shipmentStartHour As String, ByVal shipmentEndHour As String)
        request.shipmentData.shipmentInfo.shipmentDate = shipmentDate
        request.shipmentData.shipmentInfo.shipmentStartHour = shipmentStartHour
        request.shipmentData.shipmentInfo.shipmentEndHour = shipmentEndHour
    End Sub

    Public Shared Sub dhl_loadBilling(ByVal shippingPaymentType As String, ByVal paymentType As String, ByVal billingAccountNumber As String)
        request.shipmentData.shipmentInfo.billing = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.BillingStructure
        request.shipmentData.shipmentInfo.billing.shippingPaymentType = shippingPaymentType
        request.shipmentData.shipmentInfo.billing.paymentType = paymentType
        request.shipmentData.shipmentInfo.billing.billingAccountNumber = billingAccountNumber
    End Sub

    Public Shared Sub dhl_loadAddressat(ByVal name As String, ByVal postalCode As String, ByVal city As String, ByVal street As String, ByVal houseNumber As String, ByVal apartmentNumber As String)
        request.shipmentData.ship = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ShipStructure
        request.shipmentData.ship.shipper = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.FullAddressDataStructure
        request.shipmentData.ship.shipper.address = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.AddressStructure
        request.shipmentData.ship.shipper.address.name = name
        request.shipmentData.ship.shipper.address.postcode = postalCode
        request.shipmentData.ship.shipper.address.city = city
        request.shipmentData.ship.shipper.address.street = street
        request.shipmentData.ship.shipper.address.houseNumber = houseNumber
        request.shipmentData.ship.shipper.address.apartmentNumber = apartmentNumber
    End Sub

    Public Shared Sub dhl_loadServicePointAccountNumber(ByVal dhl_service_point As String)
        request.shipmentData.ship.servicePointAccountNumber = dhl_service_point
    End Sub


    Public Shared Sub dhl_loadReceiverAddressat(ByVal name As String, ByVal postalCode As String, ByVal city As String, ByVal street As String, ByVal houseNumber As String, ByVal apartmentNumber As String, ByVal country As String)
        request.shipmentData.ship.receiver = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ReceiverDataStructure
        request.shipmentData.ship.receiver.address = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ReceiverAddressStructure
        request.shipmentData.ship.receiver.address.name = name
        request.shipmentData.ship.receiver.address.postcode = postalCode
        request.shipmentData.ship.receiver.address.city = city
        request.shipmentData.ship.receiver.address.street = street
        request.shipmentData.ship.receiver.address.houseNumber = houseNumber
        request.shipmentData.ship.receiver.address.apartmentNumber = apartmentNumber
        ''request.shipmentData.ship.receiver.address.countr = country
    End Sub

    Public Shared Sub dhl_loadPreavisoContact(ByVal phoneNumber As String, ByVal personName As String)

        Dim obj_contact As New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ContactStructure
        obj_contact.personName = personName
        obj_contact.phoneNumber = phoneNumber
        request.shipmentData.ship.receiver.contact = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ContactStructure
        request.shipmentData.ship.receiver.contact = obj_contact

        Dim obj_preaviso As New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.PreavisoStructure
        obj_preaviso.personName = personName
        obj_preaviso.phoneNumber = phoneNumber
        request.shipmentData.ship.receiver.preaviso = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.PreavisoStructure
        request.shipmentData.ship.receiver.preaviso = obj_preaviso
    End Sub

    Public Shared Sub dhl_loadPreavisoContact_EK(ByVal phoneNumber As String, ByVal personName As String, ByVal email As String)
        Dim obj_contact As New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ContactStructure
        obj_contact.personName = personName
        obj_contact.phoneNumber = phoneNumber
        obj_contact.emailAddress = email
        request.shipmentData.ship.receiver.contact = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ContactStructure
        request.shipmentData.ship.receiver.contact = obj_contact

        Dim obj_preaviso As New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.PreavisoStructure
        obj_preaviso.personName = personName
        obj_preaviso.phoneNumber = phoneNumber
        obj_preaviso.emailAddress = email
        request.shipmentData.ship.receiver.preaviso = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.PreavisoStructure
        request.shipmentData.ship.receiver.preaviso = obj_preaviso
    End Sub

    Public Shared Sub dhl_loadService(ByVal service_count As String)
        request.shipmentData.shipmentInfo.specialServices = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ServiceStructure() {New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ServiceStructure}
        ReDim request.shipmentData.shipmentInfo.specialServices(service_count)
    End Sub

    Public Shared Sub dhl_AddService(ByVal service_id As String, ByVal serviceType As String, ByVal serviceValue As String, ByVal collectOnDeliveryForm As String)
        Dim specialService = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.ServiceStructure
        specialService.serviceType = serviceType
        specialService.serviceValue = serviceValue
        specialService.collectOnDeliveryForm = collectOnDeliveryForm
        request.shipmentData.shipmentInfo.specialServices(service_id) = specialService
    End Sub

    Public Shared Sub dhl_loadPackage(ByVal package_count As String)
        request.shipmentData.pieceList = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.PieceStructure() {New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.PieceStructure}
        ReDim request.shipmentData.pieceList(package_count)
    End Sub

    Public Shared Sub dhl_AddPackage(ByVal package_id As String, ByVal type As String, ByVal quantity As String, ByVal weight As String, ByVal width As String, ByVal height As String, ByVal length As String, ByVal nonStandard As Boolean)
        Dim package = New dajarSWMagazyn_MIA.pl.com.dhl24_parcelshop.PieceStructure
        package.type = type
        package.quantity = quantity
        package.weight = weight
        package.width = width
        package.height = height
        package.lenght = length
        package.nonStandard = nonStandard
        ''package.euroReturn = False
        request.shipmentData.pieceList(package_id) = package
    End Sub

    Public Shared Function dhl_createShipment(ByVal service As ServicePointMethodsService) As String
        Try
            Dim response As New CreateShipmentResponseStructure
            request.authData = auth
            response = service.createShipment(request)
            Return response.shipmentNumber.ToString
        Catch ex As Exception
            exception = ex.Message.ToString
            Return ""
        End Try
    End Function

    Public Shared Function dhl_getNearestServicePoint(ByVal service As ServicePointMethodsService, ByVal city As String, ByVal postcode As String, ByVal radius As String) As PointStructure()
        Dim dhlpopRequest As New GetNearestServicepointsStructure
        dhlpopRequest.authData = auth
        dhlpopRequest.city = city
        dhlpopRequest.postcode = postcode
        dhlpopRequest.radius = radius

        Dim pnpResponse As GetNearestServicepointsResponseStructure = service.getNearestServicepoints(dhlpopRequest)
        Return pnpResponse.points
    End Function



    Public Shared Function dhl_createShipmentJJD(ByVal service As ServicePointMethodsService) As String
        Try
            Dim response As New CreateShipmentResponseStructure
            response = service.createShipment(request)
            Return response.shipmentNumber.ToString
        Catch ex As Exception
            exception = ex.Message.ToString
            Return ""
        End Try
    End Function

    Public Shared Function dhl_deleteShipment(ByVal service As ServicePointMethodsService, ByVal shipment_id As String) As String
        Try
            Dim response As New DeleteShipmentResponseStructure
            Dim sh As New DeleteShipmentStructure
            sh.shipment = shipment_id
            sh.authData = auth
            response = service.deleteShipment(sh)
            If response.status = "OK" Then
                Return response.ToString
            Else
                Return "1"
            End If
        Catch ex As Exception
            exception = ex.Message.ToString
            Return ""
        End Try
    End Function

    Public Shared Sub dhl_getLabels(ByVal service As ServicePointMethodsService, ByVal label_type As String, ByVal format As String, ByVal shipment_id As String)
        Dim items = New GetLabelStructure() {New GetLabelStructure}
        items(0).type = label_type
        items(0).shipment = shipment_id
        items(0).authData = auth

        Dim labelResponse As LabelStructure = service.getLabel(items(0))

        Dim pdfContent As Byte() = Convert.FromBase64String(labelResponse.labelContent)

        Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/dhl/"
        dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)

        Dim pdfFileName As String = dir_export & "/" & "dhl_" & label_type & "_" & labelResponse.labelName.ToString
        pdfFileName = dir_export & "\" & "dhl_" & label_type & "_" & labelResponse.labelName.ToString
        File.WriteAllBytes(pdfFileName, pdfContent)
    End Sub

    Public Shared Sub dhl_getPnp(ByVal service As ServicePointMethodsService, ByVal data_raport As String, ByVal format_raport As String, ByVal file_name As String)
        Dim pnpRequest As New GetPnpStructure
        pnpRequest.authData = auth
        pnpRequest.shipmentDate = data_raport
        ''pnpRequest.type = format_raport
        Dim pnpResponse As LabelStructure = service.getPnp(pnpRequest)

        Dim pdfContent As Byte() = Convert.FromBase64String(pnpResponse.labelContent)

        Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/dhl/"
        dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)

        Dim pdfFileName As String = dir_export & "/" & file_name
        pdfFileName = dir_export & "\" & file_name
        File.WriteAllBytes(pdfFileName, pdfContent)
    End Sub

    Public Shared Sub dhl_printLabels(ByVal service As ServicePointMethodsService, ByVal label_type As String, ByVal format As String, ByVal shipment_id As String)
        ''Dim items = New ItemToPrint() {New ItemToPrint}
        ''items(0).labelType = label_type
        ''items(0).shipmentId = shipment_id

        ''Dim labelResponse() As ItemToPrintResponse = service.getLabels(auth, items)
        ''Dim pdfFileName As String = dajarSWMagazyn_MIA.MyFunction.networkDirectory & "\" & "dhl_" & label_type & "_" & labelResponse(0).shipmentId.ToString & "." & format

        ''If label_type <> "ZBLP" Then
        ''    ''PrintPdfFile(pdfFileName)
        ''    Shell("cmd /C ""C:\Program Files (x86)\Adobe\Reader 11.0\Reader\AcroRd32.exe"" /t """ & pdfFileName & "")
        ''ElseIf label_type = "ZBLP" Then
        ''    Shell("cmd /C copy " & pdfFileName & " lpt3")
        ''End If
    End Sub

    Public Shared Sub PrintPdfFile(ByVal filename As String)
        ''Dim myProcess As Process = New Process
        ''myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        ''myProcess.StartInfo.FileName = filename
        ''myProcess.StartInfo.Verb = "Print"
        ''''myProcess.StartInfo.CreateNoWindow = True
        ''myProcess.StartInfo.UseShellExecute = True
        ''myProcess.Start()
        ''myProcess.WaitForInputIdle()

        ''If myProcess.Responding Then
        ''    myProcess.CloseMainWindow()
        ''Else
        ''    myProcess.Kill()
        ''End If
    End Sub
End Class
