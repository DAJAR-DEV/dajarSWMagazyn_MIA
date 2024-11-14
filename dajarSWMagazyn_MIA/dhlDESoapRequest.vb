Imports System
Imports System.IO
Imports System.Net
Imports System.Text
Imports dajarSWMagazyn_MIA.dhlDEAPI
Imports Oracle.ManagedDataAccess.Client
Imports System.Globalization

Public Class dhlDESoapRequest

    Public Shared webService As SWSServicePortTypeClient
    Public Shared auth As AuthentificationType

    Public Shared CIG_USERNAME As String = ""
    Public Shared CIG_PASSWORD As String = ""
    Public Shared CIG_URL As String = ""

    Private Shared SHIPPER_STREET As String = "Schwanenfeldstraße"
    Private Shared SHIPPER_STREETNR As String = "11"
    Private Shared SHIPPER_CITY As String = "Berlin"
    Private Shared SHIPPER_ZIP As String = "13627"
    Private Shared SHIPPER_COUNTRY_CODE As String = "DE"
    Private Shared SHIPPER_CONTACT_EMAIL As String = "shop@dajar.de"
    Private Shared SHIPPER_CONTACT_NAME As String = "Herr Thews, Frau Lenz"
    Private Shared SHIPPER_CONTACT_PHONE As String = "00493034965186"
    Private Shared SHIPPER_COMPANY_NAME As String = "Dajar GmbH c/o Schenker Deutschland AG TNL"

    '	private static String ENCODING = "UTF8";
    Private Shared MAJOR_RELEASE As String = "1"
    Private Shared MINOR_RELEASE As String = "0"
    Private Shared SDF As String = "yyyy-MM-dd"
    Private Shared DD_PROD_CODE As String = "EPN"
    Private Shared TD_PROD_CODE As String = "WPX"
    Private Shared EKP As String = "6260314656"
    Private Shared PARTNER_ID As String = "01"
    Private Shared SHIPMENT_DESC As String = "Interessanter Artikel"
    Private Shared TD_SHIPMENT_REF As String = "DDU"
    Private Shared TD_VALUE_GOODS As Single = 250
    Private Shared TD_CURRENCY As String = "EUR"
    Private Shared TD_ACC_NUMBER_EXPRESS As String = "144405785"
    'Beispieldaten Für DD-Sendungen aus/nach Deutschland;
    Private Shared RECEIVER_FIRST_NAME As String = "Kai"
    Private Shared RECEIVER_LAST_NAME As String = "Wahn"
    Private Shared RECEIVER_LOCAL_STREET As String = "Marktplatz"
    Private Shared RECEIVER_LOCAL_STREETNR As String = "1"
    Private Shared RECEIVER_LOCAL_CITY As String = "Stuttgart"
    Private Shared RECEIVER_LOCAL_ZIP As String = "70173"
    Private Shared RECEIVER_LOCAL_COUNTRY_CODE As String = "DE"
    Private Shared RECEIVER_P_NUMBER As String = ""

    'Beispieldaten Für TD-Sendungen weltweit;
    Private Shared RECEIVER_WWIDE_STREET As String = "Chung Hsiao East Road."
    Private Shared RECEIVER_WWIDE_STREETNR As String = "55"
    Private Shared RECEIVER_WWIDE_CITY As String = "Taipeh"
    Private Shared RECEIVER_WWIDE_ZIP As String = "100"
    Private Shared RECEIVER_WWIDE_COUNTRY As String = "Taiwan"
    Private Shared RECEIVER_WWIDE_COUNTRY_CODE As String = "TW"

    Private Shared RECEIVER_CONTACT_EMAIL As String = "kai@wahn.de"
    Private Shared RECEIVER_CONTACT_NAME As String = "Kai Wahn"
    Private Shared RECEIVER_CONTACT_PHONE As String = "+886 2 27781-8"
    Private Shared RECEIVER_COMPANY_NAME As String = "Klammer Company"
    Private Shared DUMMY_SHIPMENT_NUMBER As String = "0000000"
    Private Shared EXPORT_REASON As String = "Sale"
    Private Shared SIGNER_TITLE As String = "Director Asia Sales"
    Private Shared INVOICE_NUMBER As String = "200601xx417"
    Private Shared DUMMY_AIRWAY_BILL As String = "0000000000"

    Public Class dhlDESoapReceiver
        Public FIRST_NAME As String = "Kai"
        Public LAST_NAME As String = "Wahn"
        Public LOCAL_STREET As String = "Marktplatz"
        Public LOCAL_STREETNR As String = "1"
        Public LOCAL_CITY As String = "Stuttgart"
        Public LOCAL_ZIP As String = "70173"
        Public LOCAL_COUNTRY_CODE As String = "DE"
        Public CONTACT_EMAIL As String = "kai@wahn.de"
        Public CONTACT_NAME As String = "Kai Wahn"
        Public CONTACT_PHONE As String = "+886 2 27781-8"
        Public COMPANY_NAME As String = "Klammer Company"

        Public Sub New(ByVal _FIRST_NAME As String, ByVal _LAST_NAME As String, ByVal _LOCAL_STREET As String, ByVal _LOCAL_STREETNR As String, ByVal _LOCAL_CITY As String, ByVal _LOCAL_ZIP As String, ByVal _LOCAL_COUNTRY_CODE As String, ByVal _CONTACT_EMAIL As String, ByVal _CONTACT_NAME As String, ByVal _CONTACT_PHONE As String, ByVal _COMPANY_NAME As String)
            FIRST_NAME = _FIRST_NAME
            LAST_NAME = _LAST_NAME
            LOCAL_STREET = _LOCAL_STREET
            LOCAL_STREETNR = _LOCAL_STREETNR
            LOCAL_CITY = _LOCAL_CITY
            LOCAL_ZIP = _LOCAL_ZIP
            LOCAL_COUNTRY_CODE = _LOCAL_COUNTRY_CODE
            CONTACT_EMAIL = _CONTACT_EMAIL
            CONTACT_NAME = _CONTACT_NAME
            CONTACT_PHONE = _CONTACT_PHONE
            COMPANY_NAME = _COMPANY_NAME
        End Sub
    End Class

    Public Class dhlDESoapReceiverWWide
        Public WWIDE_STREET As String = "Chung Hsiao East Road."
        Public WWIDE_STREETNR As String = "55"
        Public WWIDE_CITY As String = "Taipeh"
        Public WWIDE_ZIP As String = "100"
        Public WWIDE_COUNTRY As String = "Taiwan"
        Public WWIDE_COUNTRY_CODE As String = "TW"

        Public Sub New(ByVal _WWIDE_STREET As String, ByVal _WWIDE_STREETNR As String, ByVal _WWIDE_CITY As String, ByVal _WWIDE_ZIP As String, ByVal _WWIDE_COUNTRY As String, ByVal _WWIDE_COUNTRY_CODE As String)
            WWIDE_STREET = _WWIDE_STREET
            WWIDE_STREETNR = _WWIDE_STREETNR
            WWIDE_CITY = _WWIDE_CITY
            WWIDE_ZIP = _WWIDE_ZIP
            WWIDE_COUNTRY = _WWIDE_COUNTRY
            WWIDE_COUNTRY_CODE = _WWIDE_COUNTRY_CODE
        End Sub
    End Class

    Public Shared Sub dhlcreateReceiverDataFromObject(ByVal receiver As dhlDESoapReceiver)
        RECEIVER_FIRST_NAME = receiver.FIRST_NAME.ToString
        RECEIVER_LAST_NAME = receiver.LAST_NAME
        RECEIVER_LOCAL_STREET = receiver.LOCAL_STREET
        RECEIVER_LOCAL_STREETNR = receiver.LOCAL_STREETNR
        RECEIVER_LOCAL_CITY = receiver.LOCAL_CITY
        RECEIVER_LOCAL_ZIP = receiver.LOCAL_ZIP
        RECEIVER_LOCAL_COUNTRY_CODE = receiver.LOCAL_COUNTRY_CODE

        RECEIVER_CONTACT_EMAIL = receiver.CONTACT_EMAIL
        RECEIVER_CONTACT_NAME = receiver.CONTACT_NAME
        RECEIVER_CONTACT_PHONE = receiver.CONTACT_PHONE
    End Sub

    Public Shared Sub dhlcreateReceiverWWideDataFromObject(ByVal receiver As dhlDESoapReceiverWWide)
        RECEIVER_WWIDE_STREET = receiver.WWIDE_STREET
        RECEIVER_WWIDE_STREETNR = receiver.WWIDE_STREETNR
        RECEIVER_WWIDE_CITY = receiver.WWIDE_CITY
        RECEIVER_WWIDE_ZIP = receiver.WWIDE_ZIP
        RECEIVER_WWIDE_COUNTRY = receiver.WWIDE_COUNTRY
        RECEIVER_WWIDE_COUNTRY_CODE = receiver.WWIDE_COUNTRY_CODE
    End Sub

    Public Shared Function createVersion() As dajarSWMagazyn_MIA.dhlDEAPI.Version
        Dim version As New dajarSWMagazyn_MIA.dhlDEAPI.Version()
        version.majorRelease = MAJOR_RELEASE
        version.minorRelease = MINOR_RELEASE
        Return version
    End Function

    Public Shared Function createShipperNativeAddressType() As NativeAddressType
        Dim address As New NativeAddressType()
        address.streetName = SHIPPER_STREET
        address.streetNumber = SHIPPER_STREETNR
        address.city = SHIPPER_CITY
        Dim zip As New ZipType()
        zip.ItemElementName = dajarSWMagazyn_MIA.dhlDEAPI.ItemChoiceType6.germany
        zip.Item = SHIPPER_ZIP
        address.Zip = zip
        Dim origin As New CountryType()
        origin.countryISOCode = SHIPPER_COUNTRY_CODE
        address.Origin = origin

        Return address
    End Function

    Public Shared Function createReceiverNativeAddressType(ByVal worldwide As [Boolean]) As NativeAddressType

        Dim address As New NativeAddressType()
        Dim zip As New ZipType()
        Dim origin As New CountryType()
        If Not worldwide Then
            address.streetName = RECEIVER_LOCAL_STREET
            address.streetNumber = RECEIVER_LOCAL_STREETNR
            address.city = RECEIVER_LOCAL_CITY
            zip.ItemElementName = ItemChoiceType6.germany
            zip.Item = RECEIVER_LOCAL_ZIP
            origin.countryISOCode = RECEIVER_LOCAL_COUNTRY_CODE
        Else
            address.streetName = RECEIVER_WWIDE_STREET
            address.streetNumber = RECEIVER_WWIDE_STREETNR
            address.city = RECEIVER_WWIDE_CITY
            zip.ItemElementName = ItemChoiceType6.other
            zip.Item = RECEIVER_WWIDE_ZIP
            origin.country = RECEIVER_WWIDE_COUNTRY
            origin.countryISOCode = RECEIVER_WWIDE_COUNTRY_CODE
        End If
        address.Zip = zip
        address.Origin = origin
        Return address
    End Function


    Public Shared Function createShipperCommunicationType() As CommunicationType
        Dim communication As New CommunicationType()
        communication.email = SHIPPER_CONTACT_EMAIL
        communication.contactPerson = SHIPPER_CONTACT_NAME
        communication.phone = SHIPPER_CONTACT_PHONE
        Return communication
    End Function


    Public Shared Function createReceiverCommunicationType() As CommunicationType
        Dim communication As New CommunicationType()
        communication.email = RECEIVER_CONTACT_EMAIL
        communication.contactPerson = RECEIVER_CONTACT_NAME
        communication.phone = RECEIVER_CONTACT_PHONE
        Return communication
    End Function

    Public Shared Function createShipperCompany() As NameType
        Dim company As New NameTypeCompany()
        company.name1 = SHIPPER_COMPANY_NAME
        ''company.name2 = SHIPPER_P_NUMBER

        Dim name As New NameType()
        name.Item = company
        Return name
    End Function

    Public Shared Function createReceiverCompany(ByVal isPerson As [Boolean]) As NameType
        Dim name As New NameType()
        If isPerson Then
            Dim person As New NameTypePerson()
            person.firstname = RECEIVER_FIRST_NAME
            person.lastname = RECEIVER_LAST_NAME
            name.Item = person
        Else
            Dim company As New NameTypeCompany()
            company.name1 = RECEIVER_COMPANY_NAME
            company.name2 = RECEIVER_P_NUMBER
            name.Item = company
        End If
        Return name
    End Function


    Public Shared Function createShipmentDetailsDDType(ByVal shipmentItemNb As Integer) As ShipmentDetailsDDType
        Dim today As DateTime = DateTime.Today
        ''today.AddDays(1)

        Dim shipmentDetails As New ShipmentDetailsDDType()
        shipmentDetails.ProductCode = DD_PROD_CODE
        shipmentDetails.ShipmentDate = today.ToString(SDF)
        shipmentDetails.EKP = EKP

        Dim attendance As New ShipmentDetailsDDTypeAttendance()
        attendance.partnerID = PARTNER_ID
        shipmentDetails.Attendance = attendance
        Dim shItemTypeArray As ShipmentItemDDType() = New ShipmentItemDDType(shipmentItemNb - 1) {}
        shipmentDetails.ShipmentItem = shItemTypeArray

        For i As Integer = 0 To shipmentItemNb - 1
            shipmentDetails.ShipmentItem(i) = createDefaultShipmentItemDDType()
        Next
        shipmentDetails.Description = SHIPMENT_DESC

        Return shipmentDetails
    End Function


    Class dhlshipmentItemDDList
        Public shipmentDate As String = ""
        Public customerReference As String = ""
        Public description As String = ""
        Public shipmentItemDDList As New List(Of ShipmentItemDDType)
        Public serviceItemDDList As New List(Of ShipmentServiceDD)
        Public bankItem As BankType

        Public Sub New(ByVal _shipmentDate As String, ByVal _customerReference As String, ByVal _description As String, ByVal _shipmentItemDDList As List(Of ShipmentItemDDType), ByVal _serviceItemDDList As List(Of ShipmentServiceDD), ByVal _bankItem As BankType)
            shipmentDate = _shipmentDate
            customerReference = _customerReference
            description = _description
            shipmentItemDDList = _shipmentItemDDList
            serviceItemDDList = _serviceItemDDList
            bankItem = _bankItem
        End Sub
    End Class

    Class dhlserviceItemDDList
        Public serviceItemDDList As New List(Of ShipmentServiceDD)

        Public Sub New(ByVal _serviceItemDDList As List(Of ShipmentServiceDD))
            serviceItemDDList = _serviceItemDDList
        End Sub
    End Class


    Public Shared Function dhlcreateSoapShipmentServiceDD() As ShipmentServiceDD
        Dim shipmentServices As New ShipmentServiceDD()

        Return shipmentServices
    End Function


    Public Shared Function dhlcreateSoapShipmentDetailsDDType(ByVal dd_prod_code As String, ByVal dhlshipmentItemDDOBJ As dhlshipmentItemDDList) As ShipmentDetailsDDType
        ''Dim today As DateTime = DateTime.Today
        ''today.AddDays(1)

        Dim shipmentDetails As New ShipmentDetailsDDType()
        'typ produktu
        shipmentDetails.ProductCode = dd_prod_code
        shipmentDetails.ShipmentDate = dhlshipmentItemDDOBJ.shipmentDate.ToString
        shipmentDetails.EKP = EKP

        shipmentDetails.CustomerReference = dhlshipmentItemDDOBJ.customerReference.ToString

        Dim attendance As New ShipmentDetailsDDTypeAttendance()
        attendance.partnerID = PARTNER_ID
        shipmentDetails.Attendance = attendance
        shipmentDetails.Description = dhlshipmentItemDDOBJ.description

        If dhlshipmentItemDDOBJ.shipmentItemDDList.Count >= 1 Then
            Dim shItemTypeArray As ShipmentItemDDType() = New ShipmentItemDDType(dhlshipmentItemDDOBJ.shipmentItemDDList.Count - 1) {}
            shipmentDetails.ShipmentItem = shItemTypeArray

            For i As Integer = 0 To dhlshipmentItemDDOBJ.shipmentItemDDList.Count - 1
                shipmentDetails.ShipmentItem(i) = dhlshipmentItemDDOBJ.shipmentItemDDList(i)
            Next
        End If

        If dhlshipmentItemDDOBJ.serviceItemDDList.Count >= 1 Then
            Dim shServiceTypeArray As ShipmentServiceDD() = New ShipmentServiceDD(dhlshipmentItemDDOBJ.serviceItemDDList.Count - 1) {}
            shipmentDetails.Service = shServiceTypeArray

            For i As Integer = 0 To dhlshipmentItemDDOBJ.serviceItemDDList.Count - 1
                shipmentDetails.Service(i) = dhlshipmentItemDDOBJ.serviceItemDDList(i)
            Next
        End If

        If dhlshipmentItemDDOBJ.bankItem.accountNumber <> "" Then
            shipmentDetails.BankData = dhlshipmentItemDDOBJ.bankItem
        End If

        Return shipmentDetails
    End Function


    Public Shared Function createShipmentDetailsTDType(ByVal shipmentItemNb As Integer) As ShipmentDetailsTDType
        Dim today As DateTime = DateTime.Today
        today.AddDays(2)

        Dim shipmentDetails As New ShipmentDetailsTDType()
        shipmentDetails.ProductCode = TD_PROD_CODE
        shipmentDetails.ShipmentDate = today.ToString(SDF)
        Dim acc As New ShipmentDetailsTDTypeAccount()
        acc.accountNumberExpress = TD_ACC_NUMBER_EXPRESS
        shipmentDetails.Account = acc
        shipmentDetails.Dutiable = ShipmentDetailsTDTypeDutiable.Item1
        shipmentDetails.DescriptionOfContent = SHIPMENT_DESC

        Dim shItems As ShipmentItemTDType() = New ShipmentItemTDType(shipmentItemNb - 1) {}

        'ShipmentItems setzen
        For i As Integer = 0 To shipmentItemNb - 1
            shItems(i) = createDefaultShipmentItemTDType()
        Next

        shipmentDetails.ShipmentItem = shItems

        shipmentDetails.ShipmentReference = TD_SHIPMENT_REF
        shipmentDetails.DeclaredValueOfGoodsSpecified = True
        shipmentDetails.DeclaredValueOfGoods = TD_VALUE_GOODS
        shipmentDetails.DeclaredValueOfGoodsCurrency = TD_CURRENCY

        Return shipmentDetails
    End Function


    Public Shared Function createDefaultShipmentItemTDType() As ShipmentItemTDType
        Dim shipmentItem As New ShipmentItemTDType()
        shipmentItem.WeightInKG = [Decimal].Parse("3")
        shipmentItem.LengthInCM = "50"
        shipmentItem.WidthInCM = "30"
        shipmentItem.HeightInCM = "15"
        Return shipmentItem
    End Function


    Public Shared Function createDefaultShipmentItemDDType() As ShipmentItemDDType
        Dim shipmentItem As New ShipmentItemDDType()
        shipmentItem.WeightInKG = [Decimal].Parse("3")
        shipmentItem.LengthInCM = "50"
        shipmentItem.WidthInCM = "30"
        shipmentItem.HeightInCM = "15"
        shipmentItem.PackageType = "PK"
        Return shipmentItem
    End Function

    Public Shared Function createDefaultShipmentServiceDD() As ShipmentServiceDD
        Dim shipmentService As New ShipmentServiceDD
        Dim s1 As New DDServiceGroupOtherTypeCOD
        s1.CODAmount = 2500
        s1.CODCurrency = "EUR"
        shipmentService.Item = s1
        Return shipmentService
    End Function

    Public Shared Function dhlcreateSoapShipmentItemDDType(ByVal weight As String, ByVal length As String, ByVal width As String, ByVal height As String, ByVal packageType As String) As ShipmentItemDDType
        Dim shipmentItem As New ShipmentItemDDType()
        shipmentItem.WeightInKG = [Decimal].Parse(weight)
        shipmentItem.LengthInCM = length
        shipmentItem.WidthInCM = width
        shipmentItem.HeightInCM = height
        shipmentItem.PackageType = packageType
        Return shipmentItem
    End Function

    Public Shared Function dhlcreateSoapBankDataType(ByVal accountOwner As String, ByVal accountNumber As String, ByVal bankCode As String, ByVal bankName As String, ByVal iban As String, ByVal bic As String) As BankType
        Dim bankItem As New BankType
        bankItem.accountOwner = accountOwner
        bankItem.accountNumber = accountNumber
        bankItem.bankCode = bankCode
        bankItem.bankName = bankName
        bankItem.iban = iban
        bankItem.bic = bic
        Return bankItem
    End Function

    Public Shared Function dhlcreateSoapServiceItemDDType(ByVal typ As String, ByVal amount As Double, ByVal currency As String) As ShipmentServiceDD
        ''Dim shipmentOrder As New ShipmentOrderDDTypeShipment()

        '' ''####poprawna sekcja shipmentItem
        ''Dim shipmentItem As New ShipmentItemDDType()
        ''shipmentItem.WeightInKG = [Decimal].Parse("3")
        ''shipmentItem.LengthInCM = "50"
        ''shipmentItem.WidthInCM = "30"
        ''shipmentItem.HeightInCM = "15"
        ''shipmentItem.PackageType = "PK"
        ''Dim shipmentDetails As New ShipmentDetailsDDType()
        ''Dim shItemTypeArray As ShipmentItemDDType() = New ShipmentItemDDType(0) {}
        ''shipmentDetails.ShipmentItem = shItemTypeArray

        ''Dim shipmentItemDDList As New List(Of ShipmentItemDDType)
        ''shipmentItemDDList.Add(shipmentItem)

        ''For i As Integer = 0 To 0
        ''    shipmentDetails.ShipmentItem(i) = shipmentItemDDList(i)
        ''Next

        ''shipmentOrder.ShipmentDetails = shipmentDetails

        '' ''####poprawna sekcja shipmentService
        ''Dim shipmentService As New ShipmentServiceDD
        ''Dim s1 As New DDServiceGroupOtherTypeCOD
        ''s1.CODCurrency = "EUR"
        ''shipmentService.Item = s1
        ''shipmentDetails = New ShipmentDetailsDDType()
        ''Dim seItemTypeArray As ShipmentServiceDD() = New ShipmentServiceDD(0) {}
        ''shipmentDetails.Service = seItemTypeArray

        ''Dim shipmentServiceItemDDList = New List(Of ShipmentServiceDD)
        ''shipmentServiceItemDDList.Add(shipmentService)

        ''For i As Integer = 0 To 0
        ''    shipmentDetails.Service(i) = shipmentServiceItemDDList(i)
        ''Next

        ''shipmentOrder.ShipmentDetails = shipmentDetails

        ''Dim shipmentService As New ShipmentServiceDD
        ''Dim s1 As New DDServiceGroupOtherTypeCOD
        ''s1.CODCurrency = "EUR"
        ''shipmentService.Item = s1

        Dim shipmentService As New ShipmentServiceDD
        If typ = "COD" Then
            Dim service As New DDServiceGroupOtherTypeCOD
            service.CODCurrency = currency
            service.CODAmount = [Decimal].Parse(amount)
            shipmentService.Item = service
        ElseIf typ = "UBEZP" Then
            Dim service As New DDServiceGroupOtherTypeHigherInsurance
            service.InsuranceCurrency = currency
            service.InsuranceAmount = [Decimal].Parse(amount)
            shipmentService.Item = service
        End If

        Return shipmentService
    End Function

    Public Shared Function createDefaultExportDocTDType(ByVal [date] As String) As ExportDocumentTDType
        Dim exportDoc As New ExportDocumentTDType()
        exportDoc.InvoiceType = ExportDocumentTDTypeInvoiceType.commercial
        exportDoc.InvoiceDate = [date]
        exportDoc.InvoiceNumber = INVOICE_NUMBER
        exportDoc.ExportType = ExportDocumentTDTypeExportType.P
        exportDoc.SignerTitle = SIGNER_TITLE
        exportDoc.ExportReason = EXPORT_REASON
        Return exportDoc
    End Function

    Public Shared Function createDefaultShipmentDDRequest(ByVal dd_prod_code As String, ByVal receiverOBJ As dhlDESoapReceiver, ByVal receiverWWideOBJ As dhlDESoapReceiverWWide, ByVal dhlshipmentItemDDOBJ As dhlshipmentItemDDList, ByVal packStation As Boolean, ByVal postfile As Boolean, ByVal p_number As String, ByVal street_number As String, ByVal schemat As String) As CreateShipmentDDRequest
        ''ladowanie danych z obiektu dhlDESoapReceiver
        dhlcreateReceiverDataFromObject(receiverOBJ)
        dhlcreateReceiverWWideDataFromObject(receiverWWideOBJ)

        ' create empty request
        Dim createShipmentDDRequest As New CreateShipmentDDRequest()
        ' set version element

        createShipmentDDRequest.Version = createVersion()
        ' create shipment order object
        Dim shipmentOrderDDType As New ShipmentOrderDDType()

        shipmentOrderDDType.SequenceNumber = "1"

        Dim shipment As New ShipmentOrderDDTypeShipment()
        shipmentOrderDDType.Shipment = shipment
        ''shipment.ShipmentDetails = createShipmentDetailsDDType(1)
        shipment.ShipmentDetails = dhlcreateSoapShipmentDetailsDDType(dd_prod_code, dhlshipmentItemDDOBJ)

        If schemat = "DOMINUS" Then
            Dim notify_1 As New ShipmentNotificationType
            Dim notification As ShipmentNotificationType() = New ShipmentNotificationType(1) {}
            notify_1.RecipientName = "shop"
            notify_1.RecipientEmailAddress = "shop@dominus-agd.pl"
            notification(0) = notify_1

            Dim notify_2 As New ShipmentNotificationType
            notify_2.RecipientName = receiverOBJ.CONTACT_NAME
            notify_2.RecipientEmailAddress = receiverOBJ.CONTACT_EMAIL
            notification(1) = notify_2

            shipment.ShipmentDetails.Notification = notification
        End If

        Dim shipper As New ShipperDDType()
        If postfile Then
            RECEIVER_LOCAL_STREET = "Postfiliale"
            RECEIVER_P_NUMBER = p_number
            RECEIVER_LOCAL_STREETNR = street_number
        ElseIf packStation Then
            RECEIVER_LOCAL_STREET = "Packstation"
            RECEIVER_P_NUMBER = p_number
            RECEIVER_CONTACT_NAME = p_number
            RECEIVER_LOCAL_STREETNR = street_number
        End If

        shipper.Company = createShipperCompany()
        shipper.Address = createShipperNativeAddressType()
        shipper.Communication = createShipperCommunicationType()
        If schemat = "DOMINUS" Then
            shipper.Communication.contactPerson = ""
        End If
        
        shipment.Shipper = shipper

        Dim receiver As New ReceiverDDType()

        receiver.Company = createReceiverCompany(True)
        If dd_prod_code = "EPN" Then
            receiver.Item = createReceiverNativeAddressType(False)
        ElseIf dd_prod_code = "BPI" Then
            receiver.Item = createReceiverNativeAddressType(True)
        End If
        receiver.Communication = createReceiverCommunicationType()

        shipment.Receiver = receiver
        shipmentOrderDDType.LabelResponseType = ShipmentOrderDDTypeLabelResponseType.URL
        Dim shOrder As ShipmentOrderDDType() = New ShipmentOrderDDType(0) {}

        ' Shipment Order zum Request hinzufügen
        shOrder(0) = shipmentOrderDDType
        createShipmentDDRequest.ShipmentOrder = shOrder
        Return createShipmentDDRequest
    End Function


    Public Shared Function createDefaultShipmentTDRequest(ByVal dd_prod_code As String, ByVal receiverWWideOBJ As dhlDESoapReceiverWWide) As CreateShipmentTDRequest
        Dim today As DateTime = DateTime.Today

        ' Leeres Request erstellen
        Dim createShipmentTDRequest As New CreateShipmentTDRequest()
        ' set version element
        createShipmentTDRequest.Version = createVersion()
        ' create shipment order object
        Dim shipmentOrderTDType As New ShipmentOrderTDType()
        shipmentOrderTDType.SequenceNumber = "1"
        Dim shipment As New ShipmentOrderTDTypeShipment()

        shipment.ShipmentDetails = dhlDESoapRequest.createShipmentDetailsTDType(1)
        shipment.ExportDocument = createDefaultExportDocTDType(today.ToString(SDF))
        Dim shipper As New ShipperTDType()

        shipment.Shipper = shipper

        ''ladowanie danych z obiektu dhlDESoapReceiver
        dhlcreateReceiverWWideDataFromObject(receiverWWideOBJ)

        shipper.Company = createShipperCompany()
        shipper.Address = createShipperNativeAddressType()
        shipper.Communication = createShipperCommunicationType()

        Dim receiver As New ReceiverTDType()
        shipment.Receiver = receiver
        receiver.Company = createReceiverCompany(True)
        receiver.Item = createReceiverNativeAddressType(True)
        receiver.Communication = createReceiverCommunicationType()

        shipmentOrderTDType.Shipment = shipment
        shipmentOrderTDType.LabelResponseType = ShipmentOrderTDTypeLabelResponseType.URL

        ' add Shipment Order Object to request
        Dim shOrders As ShipmentOrderTDType() = New ShipmentOrderTDType(0) {}
        shOrders(0) = shipmentOrderTDType
        createShipmentTDRequest.ShipmentOrder = shOrders

        Return createShipmentTDRequest
    End Function

    Public Shared Function dhlGetCountryLanguageName(ByVal countryISO As String) As String
        For Each ci As CultureInfo In CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            Dim inforeg As New RegionInfo(ci.LCID)
            inforeg.TwoLetterISORegionName.ToUpper()
            Dim kraj_iso As String = inforeg.TwoLetterISORegionName.ToUpper
            If kraj_iso = countryISO Then
                Return inforeg.EnglishName.ToString
                Exit For
            End If
        Next
        Return "-1"
    End Function

    Public Shared Function getDefaultLabelDDRequest(ByVal shipmentId As String) As GetLabelDDRequest
        Dim ddRequest As New GetLabelDDRequest()
        ddRequest.Version = createVersion()
        Dim shNumber As New ShipmentNumberType()
        shNumber.ItemElementName = ItemChoiceType7.shipmentNumber
        If shipmentId <> "" Then
            shNumber.Item = shipmentId
        Else
            shNumber.Item = DUMMY_SHIPMENT_NUMBER
        End If

        Dim shNumbers As ShipmentNumberType() = New ShipmentNumberType(0) {}
        shNumbers(0) = shNumber
        ddRequest.ShipmentNumber = shNumbers
        Return ddRequest
    End Function


    Public Shared Function getDeleteShipmentDDRequest(ByVal shipmentId As String) As DeleteShipmentDDRequest
        Dim ddRequest As New DeleteShipmentDDRequest()
        ddRequest.Version = createVersion()
        Dim shNumber As New ShipmentNumberType()
        shNumber.ItemElementName = ItemChoiceType7.shipmentNumber
        If shipmentId <> "" Then
            shNumber.Item = shipmentId
        Else
            shNumber.Item = DUMMY_SHIPMENT_NUMBER
        End If

        Dim shNumbers As ShipmentNumberType() = New ShipmentNumberType(0) {}
        shNumbers(0) = shNumber
        ddRequest.ShipmentNumber = shNumbers
        Return ddRequest
    End Function


    Public Shared Function getDeleteShipmentTDRequest(ByVal shipmentId As String) As DeleteShipmentTDRequest
        Dim tdRequest As New DeleteShipmentTDRequest()
        tdRequest.Version = createVersion()
        Dim shNumber As New ShipmentNumberType()
        shNumber.ItemElementName = ItemChoiceType7.airwayBill
        If shipmentId <> "" Then
            shNumber.Item = shipmentId
        Else
            shNumber.Item = DUMMY_AIRWAY_BILL
        End If

        Dim shNumbers As ShipmentNumberType() = New ShipmentNumberType(0) {}
        shNumbers(0) = shNumber
        tdRequest.ShipmentNumber = shNumbers
        Return tdRequest
    End Function

    Public Shared Sub dhl_de_login(ByVal schemat As String, ByVal firma_id As String)
        Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
            conn.Open()
            Dim sqlexp = "SELECT LOGIN,HASLO FROM dp_swm_mia_firma_login WHERE SCHEMAT='" & schemat & "' AND AKTYWNA='X' AND FIRMA_ID='" & firma_id & "'"
            Dim cmd As OracleCommand = New OracleCommand(sqlexp, conn)
            cmd.CommandType = CommandType.Text
            Dim dr As OracleDataReader = cmd.ExecuteReader()
            Try
                While dr.Read()
                    CIG_USERNAME = dr.Item(0).ToString
                    CIG_PASSWORD = dr.Item(1).ToString
                    If CIG_USERNAME = "6260314656d" Then
                        CIG_URL = "https://cig.dhl.de/services/sandbox/soap"
                    Else
                        CIG_URL = "https://cig.dhl.de/services/production/soap"
                    End If
                End While

            Catch ex As System.ArgumentOutOfRangeException
                Console.WriteLine(ex)
            End Try
            dr.Close()
            cmd.Dispose()
            conn.Close()
        End Using
    End Sub

    Public Shared Sub StartWebReference()
        Console.WriteLine("StartWebReference...")

        Dim userName As String = CIG_USERNAME
        Dim password As String = CIG_PASSWORD
        Dim url As String = CIG_URL

        Try
            dhlDESoapRequest.webService = New SWSServicePortTypeClient("ShipmentServiceSOAP11port0")
            Console.WriteLine("ShipmentServiceSOAP11port0...")

            dhlDESoapRequest.webService.Endpoint.Address = New System.ServiceModel.EndpointAddress(url)
            Dim Binding As System.ServiceModel.BasicHttpBinding = New System.ServiceModel.BasicHttpBinding(System.ServiceModel.BasicHttpSecurityMode.Transport)
            Binding.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Basic
            dhlDESoapRequest.webService.Endpoint.Binding = Binding

            dhlDESoapRequest.webService.ClientCredentials.UserName.UserName = userName
            dhlDESoapRequest.webService.ClientCredentials.UserName.Password = password
            Console.WriteLine("webService.ClientCredentials...{0}/{1}", userName.ToString, password.ToString)

            dhlDESoapRequest.auth = New AuthentificationType
            If CIG_USERNAME = "6260314656d" Then
                dhlDESoapRequest.auth.user = "geschaeftskunden_api"
                dhlDESoapRequest.auth.signature = "Dhl_ep_test1"
                EKP = "5000000000"
            Else
                dhlDESoapRequest.auth.user = "webdajar1"
                dhlDESoapRequest.auth.signature = "qwertyDajar2024!#"
                EKP = "6260314656"
            End If
            dhlDESoapRequest.auth.type = "0"
            Console.WriteLine("AuthentificationType...{0}/{1}", dhlDESoapRequest.auth.user.ToString, dhlDESoapRequest.auth.signature.ToString)
        Catch ex As Exception
            Console.WriteLine(ex.Message.ToString)
        End Try

    End Sub

End Class
