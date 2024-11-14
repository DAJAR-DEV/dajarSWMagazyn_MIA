Imports Oracle.ManagedDataAccess.Client
Imports System.IO
Imports System.Data
Imports dajarSWMagazyn_MIA.pl.com.dhl24

Public Class dhl_package

    Public Shared auth As AuthData = New AuthData
    Public Shared request As CreateShipmentRequest = New dajarSWMagazyn_MIA.pl.com.dhl24.CreateShipmentRequest()
    Public Shared exception As String = ""

    Public Shared Function dhl_login(ByVal schemat As String, ByVal mag_id As String, ByVal firma_id As String) As DHL24WebapiService
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Console.WriteLine("DHL24WebapiService...")
            Dim service As DHL24WebapiService = New DHL24WebapiService()
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
        request.content = content
        request.comment = comment
        ''request.comment = request.comment.ToString.Substring(0, 100)
    End Sub

    Public Shared Sub dhl_loadShipmentInfo(ByVal serviceType As String, ByVal dropOffType As String, ByVal labelType As String)
        request.shipmentInfo = New dajarSWMagazyn_MIA.pl.com.dhl24.ShipmentInfo()
        request.shipmentInfo.serviceType = serviceType
        request.shipmentInfo.dropOffType = dropOffType
        request.shipmentInfo.labelType = labelType
    End Sub

    Public Shared Sub dhl_loadShipmentTime(ByVal shipmentDate As String, ByVal shipmentStartHour As String, ByVal shipmentEndHour As String)
        request.shipmentInfo.shipmentTime = New dajarSWMagazyn_MIA.pl.com.dhl24.ShipmentTime()
        request.shipmentInfo.shipmentTime.shipmentDate = shipmentDate
        request.shipmentInfo.shipmentTime.shipmentStartHour = shipmentStartHour
        request.shipmentInfo.shipmentTime.shipmentEndHour = shipmentEndHour
    End Sub

    Public Shared Sub dhl_loadBilling(ByVal shippingPaymentType As String, ByVal paymentType As String, ByVal billingAccountNumber As String)
        request.shipmentInfo.billing = New dajarSWMagazyn_MIA.pl.com.dhl24.Billing()
        request.shipmentInfo.billing.shippingPaymentType = shippingPaymentType
        request.shipmentInfo.billing.paymentType = paymentType
        request.shipmentInfo.billing.billingAccountNumber = billingAccountNumber
    End Sub

    Public Shared Sub dhl_loadAddressat(ByVal name As String, ByVal postalCode As String, ByVal city As String, ByVal street As String, ByVal houseNumber As String, ByVal apartmentNumber As String)
        request.ship = New dajarSWMagazyn_MIA.pl.com.dhl24.Ship()
        request.ship.shipper = New dajarSWMagazyn_MIA.pl.com.dhl24.Addressat()
        request.ship.shipper.address = New dajarSWMagazyn_MIA.pl.com.dhl24.Address()
        request.ship.shipper.address.name = name
        request.ship.shipper.address.postalCode = postalCode
        request.ship.shipper.address.city = city
        request.ship.shipper.address.street = street
        request.ship.shipper.address.houseNumber = houseNumber
        request.ship.shipper.address.apartmentNumber = apartmentNumber
    End Sub

    Public Shared Sub dhl_loadReceiverAddressat(ByVal name As String, ByVal postalCode As String, ByVal city As String, ByVal street As String, ByVal houseNumber As String, ByVal apartmentNumber As String, ByVal country As String)
        request.ship.receiver = New dajarSWMagazyn_MIA.pl.com.dhl24.ReceiverAddressat()
        request.ship.receiver.address = New dajarSWMagazyn_MIA.pl.com.dhl24.ReceiverAddress()
        request.ship.receiver.address.name = name
        request.ship.receiver.address.postalCode = postalCode
        request.ship.receiver.address.city = city
        request.ship.receiver.address.street = street
        request.ship.receiver.address.houseNumber = houseNumber
        request.ship.receiver.address.apartmentNumber = apartmentNumber
        request.ship.receiver.address.country = country
    End Sub

    Public Shared Sub dhl_loadPreavisoContact(ByVal phoneNumber As String, ByVal personName As String)
        request.ship.receiver.preaviso = New dajarSWMagazyn_MIA.pl.com.dhl24.PreavisoContact
        Dim contact As New dajarSWMagazyn_MIA.pl.com.dhl24.PreavisoContact
        contact.phoneNumber = phoneNumber
        contact.personName = personName
        request.ship.receiver.preaviso = contact
        request.ship.receiver.contact = New dajarSWMagazyn_MIA.pl.com.dhl24.PreavisoContact
        request.ship.receiver.contact = contact
    End Sub

    Public Shared Sub dhl_loadPreavisoContact_EK(ByVal phoneNumber As String, ByVal personName As String, ByVal email As String)
        request.ship.receiver.preaviso = New dajarSWMagazyn_MIA.pl.com.dhl24.PreavisoContact
        Dim contact As New dajarSWMagazyn_MIA.pl.com.dhl24.PreavisoContact
        contact.phoneNumber = phoneNumber
        contact.personName = personName
        contact.emailAddress = email
        request.ship.receiver.preaviso = contact
        request.ship.receiver.contact = New dajarSWMagazyn_MIA.pl.com.dhl24.PreavisoContact
        request.ship.receiver.contact = contact
    End Sub

    Public Shared Sub dhl_loadService(ByVal service_count As String)
        request.shipmentInfo.specialServices = New dajarSWMagazyn_MIA.pl.com.dhl24.Service() {New dajarSWMagazyn_MIA.pl.com.dhl24.Service}
        ReDim request.shipmentInfo.specialServices(service_count)
    End Sub

    Public Shared Sub dhl_AddService(ByVal service_id As String, ByVal serviceType As String, ByVal serviceValue As String, ByVal collectOnDeliveryForm As String)
        Dim specialService = New dajarSWMagazyn_MIA.pl.com.dhl24.Service
        specialService.serviceType = serviceType
        specialService.serviceValue = serviceValue
        specialService.collectOnDeliveryForm = collectOnDeliveryForm
        request.shipmentInfo.specialServices(service_id) = specialService
    End Sub

    Public Shared Sub dhl_loadPackage(ByVal package_count As String)
        request.pieceList = New dajarSWMagazyn_MIA.pl.com.dhl24.Package() {New dajarSWMagazyn_MIA.pl.com.dhl24.Package}
        ReDim request.pieceList(package_count)
    End Sub

    Public Shared Sub dhl_AddPackage(ByVal package_id As String, ByVal type As String, ByVal quantity As String, ByVal weight As String, ByVal width As String, ByVal height As String, ByVal length As String, ByVal nonStandard As Boolean)
        Dim package = New dajarSWMagazyn_MIA.pl.com.dhl24.Package
        package.type = type
        package.quantity = quantity
        package.weight = weight
        package.width = width
        package.height = height
        package.length = length
        package.nonStandard = nonStandard
        package.euroReturn = False
        request.pieceList(package_id) = package
    End Sub

    Public Shared Function dhl_createShipment(ByVal service As DHL24WebapiService) As String
        Try
            Dim response As New CreateShipmentResponse
            response = service.createShipment(auth, request)
            Return response.shipmentNotificationNumber.ToString
        Catch ex As Exception
            exception = ex.Message.ToString
            Return ""
        End Try
    End Function

    Public Shared Function dhl_createShipmentJJD(ByVal service As DHL24WebapiService) As String
        Try
            Dim response As New CreateShipmentResponse
            response = service.createShipment(auth, request)
            Return response.packagesTrackingNumbers.ToString
        Catch ex As Exception
            exception = ex.Message.ToString
            Return ""
        End Try
    End Function

    Public Shared Function dhl_deleteShipment(ByVal service As DHL24WebapiService, ByVal shipment_id As String) As String
        Try
            Dim response As New DeleteShipmentResponse
            Dim sh As New DeleteShipmentRequest
            sh.shipmentIdentificationNumber = shipment_id
            response = service.deleteShipment(auth, sh)
            If response.result = False Then
                Return response.error.ToString
            Else
                Return "1"
            End If
        Catch ex As Exception
            exception = ex.Message.ToString
            Return ""
        End Try
    End Function

    Public Shared Sub dhl_getLabels(ByVal service As DHL24WebapiService, ByVal label_type As String, ByVal format As String, ByVal shipment_id As String)
        Dim items = New ItemToPrint() {New ItemToPrint}
        items(0).labelType = label_type
        items(0).shipmentId = shipment_id

        Dim labelResponse() As ItemToPrintResponse = service.getLabels(auth, items)
        Dim pdfContent As Byte() = Convert.FromBase64String(labelResponse(0).labelData)

        Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/dhl/"
        dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)

        Dim pdfFileName As String = dir_export & "/" & "dhl_" & label_type & "_" & labelResponse(0).shipmentId.ToString & "." & format
        pdfFileName = dir_export & "\" & "dhl_" & label_type & "_" & labelResponse(0).shipmentId.ToString & "." & format
        File.WriteAllBytes(pdfFileName, pdfContent)
    End Sub

    Public Shared Sub dhl_getPnp(ByVal service As DHL24WebapiService, ByVal data_raport As String, ByVal format_raport As String, ByVal file_name As String)
        Dim pnpRequest As New PnpRequest
        pnpRequest.authData = auth
        pnpRequest.date = data_raport
        pnpRequest.type = format_raport
        Dim pnpResponse As PnpResponse = service.getPnp(pnpRequest)

        Dim pdfContent As Byte() = Convert.FromBase64String(pnpResponse.fileData)
        Dim dir_export As String = dajarSWMagazyn_MIA.MyFunction.networkPath & "/dhl/"
        dajarSWMagazyn_MIA.MyFunction.CheckUploadDirectory(dir_export)

        Dim pdfFileName As String = dir_export & "/" & file_name
        pdfFileName = dir_export & "\" & file_name
        File.WriteAllBytes(pdfFileName, pdfContent)
    End Sub

    Public Shared Sub dhl_printLabels(ByVal service As DHL24WebapiService, ByVal label_type As String, ByVal format As String, ByVal shipment_id As String)
        Dim items = New ItemToPrint() {New ItemToPrint}
        items(0).labelType = label_type
        items(0).shipmentId = shipment_id

        Dim labelResponse() As ItemToPrintResponse = service.getLabels(auth, items)
        Dim pdfFileName As String = dajarSWMagazyn_MIA.MyFunction.networkDirectory & "\" & "dhl_" & label_type & "_" & labelResponse(0).shipmentId.ToString & "." & format

        If label_type <> "ZBLP" Then
            ''PrintPdfFile(pdfFileName)
            Shell("cmd /C ""C:\Program Files (x86)\Adobe\Reader 11.0\Reader\AcroRd32.exe"" /t """ & pdfFileName & "")
        ElseIf label_type = "ZBLP" Then
            Shell("cmd /C copy " & pdfFileName & " lpt3")
        End If
    End Sub

    Public Shared Sub PrintPdfFile(ByVal filename As String)
        Dim myProcess As Process = New Process
        myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        myProcess.StartInfo.FileName = filename
        myProcess.StartInfo.Verb = "Print"
        ''myProcess.StartInfo.CreateNoWindow = True
        myProcess.StartInfo.UseShellExecute = True
        myProcess.Start()
        myProcess.WaitForInputIdle()

        If myProcess.Responding Then
            myProcess.CloseMainWindow()
        Else
            myProcess.Kill()
        End If
    End Sub
End Class
