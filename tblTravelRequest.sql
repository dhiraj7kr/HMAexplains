SELECT TOP (1000) [Id]
      ,[journeyType]
      ,[classOfService]
      ,[preferredCarrier]
      ,[departureCity_1]
      ,[arrivalCity_1]
      ,[OneWaydepartureDate_1]
      ,[OneWayarrivalDate_1]
      ,[totalRestrictFare]
      ,[onwardRestrictFare]
      ,[returnRestrictFare]
      ,[directFlight]
      ,[bookingAddress]
      ,[bookingCity]
      ,[bookingState]
      ,[bookingCountry]
      ,[bookingZipcode]
      ,[bookingEmail]
      ,[bookingMobile]
      ,[Status]
      ,[CreatedBy]
      ,[CreatedDate]
      ,[ModifiedBy]
      ,[ModifiedDate]
      ,[PurposeOfTravel]
      ,[PurposeOfTravel2]
      ,[Remarks]
      ,[HMARequestId]
      ,[Travelling]
      ,[TwoWaydepartureDate_11]
      ,[TwoWayarrivalDate_11]
      ,[TicketStatus]
      ,[Comments]
      ,[HMATravelRequestID]
      ,[TotalCost]
      ,[ChildeTravelID]
      ,[ParentTravelID]
      ,[PrimaryAOPGroup]
      ,[GSTNumber]
      ,[IsCovidDeclared]
      ,[Company]
      ,[FromCity]
      ,[ToCity]
      ,[SpecifyTravelPurpose]
  FROM [dbo].[tblTravelRequest]







Here is your data formatted in tabular form with the requested headers from **[dbo].[tblTravelRequest]** (TOP 10 records):

| Id | journeyType | classOfService | preferredCarrier | departureCity_1                                                | arrivalCity_1                       | OneWaydepartureDate_1   | OneWayarrivalDate_1     | totalRestrictFare | onwardRestrictFare | returnRestrictFare | directFlight | bookingAddress                                                                               | bookingCity | bookingState | bookingCountry | bookingZipcode | bookingEmail                                                                    | bookingMobile | Status | CreatedBy | CreatedDate             | ModifiedBy | ModifiedDate            | PurposeOfTravel        | PurposeOfTravel2 | Remarks | HMARequestId | Travelling | TwoWaydepartureDate_11 | TwoWayarrivalDate_11 | TicketStatus | Comments | HMATravelRequestID | TotalCost | ChildeTravelID | ParentTravelID | PrimaryAOPGroup | GSTNumber | IsCovidDeclared | Company | FromCity | ToCity | SpecifyTravelPurpose |
| -- | ----------- | -------------- | ---------------- | -------------------------------------------------------------- | ----------------------------------- | ----------------------- | ----------------------- | ----------------- | ------------------ | ------------------ | ------------ | -------------------------------------------------------------------------------------------- | ----------- | ------------ | -------------- | -------------- | ------------------------------------------------------------------------------- | ------------- | ------ | --------- | ----------------------- | ---------- | ----------------------- | ---------------------- | ---------------- | ------- | ------------ | ---------- | ---------------------- | -------------------- | ------------ | -------- | ------------------ | --------- | -------------- | -------------- | --------------- | --------- | --------------- | ------- | -------- | ------ | -------------------- |
| 1  | ONE_WAY     | Economy        | NULL             | Mumbai - Chhatrapati Shivaji International - India - BOM - BOM | Pune - Lohegaon - India - PNQ - PNQ | 2019-06-20 06:43:32.540 | NULL                    | 0.00              | 0.00               | 0.00               |              | Tech Park, Near ICICI Bank, Sainik Nagar, Clover Park, Viman Nagar, Pune, Maharashtra 411014 | Pune        | Maharastra   | India          | 411014         | [acuvate1@Bajajfina.onmicrosoft.com](mailto:acuvate1@Bajajfina.onmicrosoft.com) | 9999999999    | 8      | 99999     | 2019-04-23 11:54:42.693 | 99999      | 2019-04-23 12:03:45.833 | Corporate Office Visit | Vendor Meeting   | Yesy    | TR1          | 1          | NULL                   | NULL                 | INVOICED     | NULL     | T4253552537        | 6389      | NULL           | NULL           | NULL            | NULL      | NULL            | NULL    | NULL     | NULL   | NULL                 |
| 2  | ONE_WAY     | Economy        | NULL             | Mumbai - Chhatrapati Shivaji International - India - BOM - BOM | Pune - Lohegaon - India - PNQ - PNQ | 2019-04-30 22:30:00.000 | 2019-05-01 00:20:00.000 | 0.00              | 0.00               | 0.00               |              | Tech Park, Near ICICI Bank, Sainik Nagar, Clover Park, Viman Nagar, Pune, Maharashtra 411014 | Pune        | Maharastra   | India          | 411014         | [acuvate1@Bajajfina.onmicrosoft.com](mailto:acuvate1@Bajajfina.onmicrosoft.com) | 9999999999    | 11     | 99999     | 2019-04-23 12:17:11.280 | 99999      | 2019-10-31 10:18:30.487 | Corporate Office Visit | Training         | Test    | TR2          | 1          | NULL                   | NULL                 | INVOICED     | NULL     | T4260290997        | 6389      | NULL           | NULL           | NULL            | NULL      | NULL            | NULL    | NULL     | NULL   | NULL                 |
| 3  | ONE_WAY     | Economy        | NULL             | Mumbai - Chhatrapati Shivaji International - India - BOM - BOM | Pune - Lohegaon - India - PNQ - PNQ | 2019-04-30 22:30:00.000 | 2019-05-01 00:20:00.000 | 0.00              | 0.00               | 0.00               |              | Tech Park, Near ICICI Bank, Sainik Nagar, Clover Park, Viman Nagar, Pune, Maharashtra 411014 | Pune        | Maharastra   | India          | 411014         | [acuvate1@Bajajfina.onmicrosoft.com](mailto:acuvate1@Bajajfina.onmicrosoft.com) | 9999999999    | 11     | 99999     | 2019-04-23 15:26:27.287 | 99999      | 2019-10-31 10:18:31.783 | Corporate Office Visit |                  | Test    | TR3          | 3          | NULL                   | NULL                 | INVOICED     | NULL     | T4260411431        | 6389      | NULL           | NULL           | NULL            | NULL      | NULL            | NULL    | NULL     | NULL   | NULL                 |
| 4  | ONE_WAY     | Economy        | NULL             | Mumbai - Chhatrapati Shivaji International - India - BOM - BOM | Pune - Lohegaon - India - PNQ - PNQ | 2019-04-19 04:40:00.000 | 2019-04-19 06:15:00.000 | 0.00              | 0.00               | 0.00               |              | Tech Park, Near ICICI Bank, Sainik Nagar, Clover Park, Viman Nagar, Pune, Maharashtra 411014 | Pune        | Maharastra   | India          | 411014         | [acuvate1@Bajajfina.onmicrosoft.com](mailto:acuvate1@Bajajfina.onmicrosoft.com) | 9999999999    | 11     | 99999     | 2019-04-23 15:35:29.977 | 99999      | 2019-10-31 10:18:32.600 | Corporate Office Visit |                  | Test    | TR4          | 1          | NULL                   | NULL                 | INVOICED     | NULL     | T4253573850        | 4210      | NULL           | NULL           | NULL            | NULL      | NULL            | NULL    | NULL     | NULL   | NULL                 |
| 5  | ONE_WAY     | Economy        | NULL             | Mumbai - Chhatrapati Shivaji International - India - BOM - BOM | Pune - Lohegaon - India - PNQ - PNQ | 2019-04-18 20:30:00.000 | 2019-04-18 22:40:00.000 | 0.00              | 0.00               | 0.00               |              | Tech Park, Near ICICI Bank, Sainik Nagar, Clover Park, Viman Nagar, Pune, Maharashtra 411014 | Pune        | Maharastra   | India          | 411014         | [acuvate1@Bajajfina.onmicrosoft.com](mailto:acuvate1@Bajajfina.onmicrosoft.com) | 9999999999    | 11     | 99999     | 2019-04-23 15:36:33.433 | 99999      | 2019-10-31 10:18:33.417 | Corporate Office Visit |                  | Test    | TR5          | 1          | NULL                   | NULL                 | INVOICED     | NULL     | T4253594546        | 5414      | NULL           | NULL           | NULL            | NULL      | NULL            | NULL    | NULL     | NULL   | NULL                 |
| 6  | ONE_WAY     | Economy        | NULL             | Mumbai - Chhatrapati Shivaji International - India - BOM - BOM | Pune - Lohegaon - India - PNQ - PNQ | 2019-04-30 00:00:00.000 | NULL                    | 0.00              | 0.00               | 0.00               |              | Tech Park, Near ICICI Bank, Sainik Nagar, Clover Park, Viman Nagar, Pune, Maharashtra 411014 | Pune        | Maharastra   | India          | 411014         | [acuvate1@Bajajfina.onmicrosoft.com](mailto:acuvate1@Bajajfina.onmicrosoft.com) | 9999999999    | 1      | 99999     | 2019-04-23 15:45:10.097 | 99999      | 2019-04-23 17:00:46.787 | Corporate Office Visit |                  | Test    | TTR6         | 1          | NULL                   | NULL                 | NULL         | NULL     | NULL               | NULL      | NULL           | NULL           | NULL            | NULL      |                 |         |          |        |                      |
| 7  | ONE_WAY     | Economy        | NULL             | Mumbai - Chhatrapati Shivaji International - India - BOM - BOM | Pune - Lohegaon - India - PNQ - PNQ | 2019-05-01 00:00:00.000 | NULL                    | 0.00              | 0.00               | 0.00               |              | Tech Park, Near ICICI Bank, Sainik Nagar, Clover Park, Viman Nagar, Pune, Maharashtra 411014 | Pune        | Maharastra   | India          | 411014         | [acuvate1@Bajajfina.onmicrosoft.com](mailto:acuvate1@Bajajfina.onmicrosoft.com) | 9999999999    | 1      | 99999     | 2019-04-23 17:06:13.770 | 99999      | 2019-04-23 17:06:48.367 | Corporate Office Visit |                  | Test    | TTR7         | 1          | NULL                   | NULL                 | NULL         | NULL     | NULL               | NULL      | NULL           | NULL           | NULL            | NULL      |                 |         |          |        |                      |
| 8  | ONE_WAY     | Economy        | NULL             | Mumbai - Chhatrapati Shivaji International - India - BOM - BOM | Pune - Lohegaon - India - PNQ - PNQ | 2019-05-01 00:00:00.000 | NULL                    | 0.00              | 0.00               | 0.00               |              | Tech Park, Near ICICI Bank, Sainik Nagar, Clover Park, Viman Nagar, Pune, Maharashtra 411014 | Pune        | Maharastra   | India          | 411014         | [acuvate1@Bajajfina.onmicrosoft.com](mailto:acuvate1@Bajajfina.onmicrosoft.com) | 9999999999    | 1      | 99999     | 2019-04-23 17:10:14.577 | 99999      | 2019-04-23 17:11:05.040 | Corporate Office Visit |                  | Test    | TTR8         | 1          | NULL                   | NULL                 | NULL         | NULL     | NULL               | NULL      | NULL           | NULL           | NULL            | NULL      |                 |         |          |        |                      |
| 9  | ONE_WAY     | Economy        | NULL             | Mumbai - Chhatrapati Shivaji International - India - BOM - BOM | Pune - Lohegaon - India - PNQ - PNQ | 2019-04-30 00:00:00.000 | NULL                    | 0.00              | 0.00               | 0.00               |              | Tech Park, Near ICICI Bank, Sainik Nagar, Clover Park, Viman Nagar, Pune, Maharashtra 411014 | Pune        | Maharastra   | India          | 411014         | [acuvate1@Bajajfina.onmicrosoft.com](mailto:acuvate1@Bajajfina.onmicrosoft.com) | 9999999999    | 2      | 99999     | 2019-04-23 17:15:14.230 | 99999      | 2019-04-23 17:15:14.230 | Corporate Office Visit |                  | Test    | TTR9         | 1          | NULL                   | NULL                 | NULL         | NULL     | NULL               | NULL      | NULL           | NULL           | NULL            | NULL      |                 |         |          |        |                      |
| 10 | ONE_WAY     | Economy        | NULL             | Mumbai - Chhatrapati Shivaji International - India - BOM - BOM | Pune - Lohegaon - India - PNQ - PNQ | 2019-04-30 00:00:00.000 | NULL                    | 0.00              | 0.00               | 0.00               |              | Tech Park, Near ICICI Bank, Sainik Nagar, Clover Park, Viman Nagar, Pune, Maharashtra 411014 | Pune        | Maharastra   | India          | 411014         | [acuvate1@Bajajfina.onmicrosoft.com](mailto:acuvate1@Bajajfina.onmicrosoft.com) | 9999999999    | 2      | 99999     | 2019-04-23 17:16:04.917 | 99999      | 2019-04-23 17:16:04.917 | Corporate Office Visit |                  | Test    | TTR10        | 1          | NULL                   | NULL                 | NULL         | NULL     | NULL               | NULL      | NULL           | NULL           | NULL            | NULL      |                 |         |          |        |                      |


