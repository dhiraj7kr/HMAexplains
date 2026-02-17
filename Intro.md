This is a **Console Job (Background Scheduler Job)** that synchronizes **Travel Ticket Status** between your **Database** and the **HMA Travel Booking API**, updates status, invoices, cancellations, and sends emails.

I’ll explain the **complete flow step-by-step in simple real-world terms**, then explain **each method and logic**.

---

# High Level Purpose (In Simple Words)

This program:

1. Gets all booked travel requests from your database
2. Calls HMA API to check latest ticket status
3. Updates database based on latest status:

   * Booked
   * Cancelled
   * Partially cancelled
   * Invalid
   * Pending
4. Updates invoice details
5. Updates cancelled passenger details
6. Sends confirmation email if booking confirmed
7. Writes logs

This job likely runs via:

* Windows Scheduler OR
* Windows Service OR
* Cron job

---

# REAL WORLD FLOW DIAGRAM

```
Database → Get travel requests
           ↓
For each ticket:
           ↓
Call HMA API → Get latest status
           ↓
Compare DB status vs HMA status
           ↓
Update Database accordingly:
    • Booked → Update status + invoice + send mail
    • Cancelled → Update cancelled passengers + invoice
    • Partially cancelled → Update pax cancellation
    • Invalid → Mark draft
           ↓
Write logs
```

---

# ENTRY POINT

```
static void Main(string[] args)
{
    Console.WriteLine("Started");

    GetData();

    Console.WriteLine("End");
}
```

This starts the process.

Calls:

```
GetData()
```

This is the main engine.

---

# MAIN CORE METHOD: GetData()

This method does:

```
1. Fetch travel requests from DB
2. Loop each ticket
3. Call HMA API
4. Process response
5. Update database
```

---

# STEP 1: Get all booked travel requests

```
var Dt = GetAllBookedTravelRequest().Tables[0];
```

Calls stored procedure:

```
USP_GetAllApprovedSavedTravelRequest_New_ConApp
```

Returns table like:

| Id | HMARequestId | Status | journeyType | Company |
| -- | ------------ | ------ | ----------- | ------- |

These are tickets that need status update.

---

# STEP 2: Loop each ticket

```
for (var row = 0; row < Dt.Rows.Count; row++)
```

Processing each ticket one by one.

---

# STEP 3: Call HMA API

Depending on company:

```
if Company == BFL
   use GetTicketURL

else
   use BFDLGetTicketURL
```

Example:

```
https://hmaapi.com/getticket/TRN1234
```

Code:

```
string Data = httpClient.GetStringAsync(URL).Result;
```

API returns JSON:

Example:

```
{
  statusCode: 200,
  bookingSummary:
  {
     status: "INVOICED",
     travelPlanId: "TRN1234",
     priceDetail: {...},
     destinations: [...]
  }
}
```

---

# STEP 4: Deserialize JSON

```
var response = JsonConvert.DeserializeObject<dynamic>(Data);
```

Now program can access:

```
response["bookingSummary"]["status"]
```

---

# STEP 5: Validate API response

If invalid:

```
statusCode = 404 OR 0 OR bookingSummary null
```

Then:

```
log error
increment InvalidTickets
```

---

# STEP 6: Call UpdateStatus()

```
UpdateStatus(response, Dt, row, Data);
```

This is MOST IMPORTANT METHOD.

Handles everything.

---

# MAIN LOGIC METHOD: UpdateStatus(dynamic response, DataTable Dt, int row, string Data)

This method decides:

```
Booked?
Cancelled?
Partial Cancel?
Invalid?
Pending?
```

---

# CASE 1: INVALID TICKET

```
if statusCode = 404 OR 0
```

Means ticket not found in HMA.

Action:

```
InvalidTickets++

UpdateStatusToDraft()
```

Calls stored procedure:

```
USP_UpdateStatustoDraft
```

Database status becomes:

```
Draft
```

---

# CASE 2: FULLY CANCELLED or INVOICED

Condition:

```
onewaystatus = 9 OR 11
twowaystatus = 9 OR 11
```

Meaning:

```
9 = Cancelled
11 = Invoiced
```

Action:

Loop passengers:

```
InsertUpdateInvoiceData()
```

Stored procedure:

```
USP_InsertUpdateInvoiceDetails
```

Updates invoice details per passenger.

---

# CASE 3: NORMAL BOOKING UPDATE

When ticket is booked or pending.

Extracts:

```
status
travelPlanId
departure time
arrival time
fare
```

Calls:

```
UpdateStatus(...)
```

Stored procedure:

```
USP_TravelUpdateStatus
```

Updates DB travel request.

---

# CASE 4: PARTIAL CANCELLATION

Example:

2 passengers booked
1 cancelled

Program loops destinations:

```
for each destination
   if cancelled
      loop passengers
         call UpdateCancelledPassangersData()
```

Stored procedure:

```
USP_TravelUpdateCancelledticketStatus
```

Updates cancelled passenger.

---

# CASE 5: SEND EMAIL IF BOOKED

Condition:

```
if DbStatus == 2 AND HMA status = INVOICED
```

Then:

```
GetMailTemplateData()
```

Gets template from SharePoint list:

```
EmailTemplate
```

Sends email using:

```
Mail.PostSendMail()
```

---

# CASE 6: UPDATE INVOICE DATA

Always runs:

```
InsertUpdateInvoiceData()
```

Stored procedure:

```
USP_InsertUpdateInvoiceDetails
```

Updates invoice number, amount, passenger info.

---

# DATABASE STORED PROCEDURES USED

These are critical:

---

## 1. USP_GetAllApprovedSavedTravelRequest_New_ConApp

Fetch travel requests needing update.

---

## 2. USP_TravelUpdateStatus

Updates main travel request status.

Updates:

```
Status
Departure time
Arrival time
Fare
Cities
EmailSent flag
```

---

## 3. USP_TravelUpdateCancelledticketStatus

Updates cancelled passenger.

---

## 4. USP_InsertUpdateInvoiceDetails

Updates invoice details.

---

## 5. USP_UpdateStatustoDraft

Sets ticket to Draft.

---

## 6. USP_UpdateStatustoExpired

Marks expired tickets.

---

# SHAREPOINT USED FOR EMAIL TEMPLATE

Method:

```
GetMailTemplateData()
```

Connects:

```
ClientContext(SiteUrl)
```

Reads list:

```
EmailTemplate
```

Fields:

```
Category
EmailBody
To
CC
Subject
```

Then sends mail.

---

# LOGGING SYSTEM

Logs stored in file:

```
ExceptionLogPath/logdd-MM-yyyy.txt
```

Example log:

```
Ticket Number: TRN1234, Cancelled, 17-02-2026
```

---

# COUNTERS

These track statistics:

```
CancelledTickets
BookedTickets
InvalidTickets
```

Logged at end.

---

# FULL FLOW SUMMARY

```
START
 ↓
Get travel requests from DB
 ↓
For each request:
   ↓
   Call HMA API
   ↓
   If invalid → mark draft
   ↓
   If booked → update DB + invoice + send mail
   ↓
   If cancelled → update cancelled passengers + invoice
   ↓
   If partial cancelled → update cancelled pax
   ↓
   Log everything
 ↓
END
```

---

# WHAT THIS JOB IS CALLED IN INDUSTRY

This is called:

```
Background Synchronization Job
OR
Ticket Status Sync Job
OR
Booking Reconciliation Job
```

---

# WHY THIS JOB EXISTS

Because booking happens in external system:

```
HMA Travel System
```

Your DB must sync with latest status.

This job ensures database always correct.

---

# WHAT SYSTEMS ARE INVOLVED

```
Your Application Database (SQL Server)
        ↑
        ↓
Console Sync Job (This program)
        ↓
        ↑
HMA Travel API
        ↓
        ↑
SharePoint Email Template
        ↓
        ↑
Email Service
```

---

# FINAL SIMPLE REAL LIFE EXAMPLE

User books ticket → status = Pending

HMA confirms booking → status = INVOICED

This job runs →

```
calls HMA API
sees INVOICED
updates DB status to Booked
adds invoice
sends confirmation email
```

---

# IF YOU WANT, I CAN ALSO EXPLAIN

• Each status number meaning (1,2,4,6,9,11,14)
• Database table structure
• Exact execution flow with diagram
• Or optimize this code professionally

Just tell.
