This C# code is a **Console Application** designed to synchronize travel booking data between an external travel portal (referred to as **HMA**) and an internal SQL Server database. It acts as a background "job" that checks for status updates on travel requests (Booked, Cancelled, Invoiced, or Expired) and updates the local system accordingly.

Here is a breakdown of what the code is doing, the database objects it interacts with, and the logic behind it:

---

## 1. Core Functionality: What is it doing?

The application follows a "Fetch-Check-Update" workflow:

1. **Fetch:** It pulls all travel requests from the local database that are currently in a "booked" or "pending" state.
2. **API Integration:** For each request, it makes an HTTP GET call to the HMA travel portal's API to get the latest real-time status.
3. **Synchronization Logic:**
* **Invoicing:** If the ticket is now "Invoiced," it updates the local database with passenger names, amounts, and invoice numbers.
* **Cancellations:** If the ticket (or specific passengers/segments) is cancelled, it triggers a cancellation update in the database.
* **Expirations:** It checks if a travel date has passed without approval and marks those requests as "Expired."
* **Emailing:** If a booking is confirmed, it fetches an email template from **SharePoint Online** and sends a confirmation mail.



---

## 2. Database Interactions (Stored Procedures & Tables)

The code relies heavily on SQL Stored Procedures to abstract the database logic.

| Stored Procedure | Purpose | Why it's used |
| --- | --- | --- |
| `USP_GetAllApprovedSavedTravelRequest_New_ConApp` | **Fetch Data** | To get a list of travel requests that the job needs to check against the external API. |
| `USP_TravelUpdateStatus` | **Update Status** | Used when a ticket's overall status changes (e.g., from Pending to Invoiced). It saves the new status and response JSON. |
| `USP_InsertUpdateInvoiceDetails` | **Accounting** | Specifically used to store financial data (Invoice Number, Amount, Pax Name) once a booking is finalized. |
| `USP_TravelUpdateCancelledticketStatus` | **Cancellation Handling** | Handles "Partial Cancellations" (where one passenger or one leg of a trip is cancelled). |
| `USP_UpdateStatustoDraft` | **Error Recovery** | If an API returns a 404 (Not Found) but the travel date has passed, it moves the request back to "Draft." |
| `USP_GetPendingApprovalTravelRequests` | **Fetch Pending** | Fetches requests that are stuck in "Pending Approval" to check for expiration. |
| `USP_UpdateStatustoExpired` | **Expiry Handling** | Marks requests as "Expired" if the departure date is in the past and they weren't approved. |

---

## 3. Key Components & Logic

### A. The Status Mapping

The code uses specific numeric IDs (likely from a status master table) to handle logic:

* **Status 9:** Cancelled.
* **Status 11:** Invoiced / Booked.
* **Status 6:** Pending Cancellation.
* **Status 4 / 14 / 1:** Various "In-Progress" statuses.

### B. SharePoint Integration (`GetMailTemplateData`)

The app connects to a **SharePoint Online List** called `"EmailTemplate"`.

* **Why?** Instead of hardcoding email text, the business can update the email body/subject in SharePoint, and the code will fetch the latest version to send to the user.

### C. Error Logging (`WriteToLogs`)

The code writes detailed logs to text files (e.g., `log16-02-2026.txt`).

* **Why?** Since this runs as a background task, logs are the only way for developers to see why a specific ticket failed to update (e.g., API timeout or SQL error).

### D. Security (`GetPassword`)

The code uses `SecureString` to handle the SharePoint password. This is a security best practice to prevent the password from being stored in plain text in the computer's memory.

---

