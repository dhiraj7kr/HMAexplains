Here is a clear mapping of **which stored procedure uses which table**, based on your explanation.

---

## 1️⃣ `USP_GetAllApprovedSavedTravelRequest_New_ConApp`

**Purpose:** Fetcher / Queue Manager

| Table Used         | How It Is Used                                                                                                                                |
| ------------------ | --------------------------------------------------------------------------------------------------------------------------------------------- |
| `tblTravelRequest` | Reads travel requests with Status 1, 4, or 14 created within last 180 days. Returns request data including `onewaystatus` and `twowaystatus`. |

👉 This procedure is **read-only** and only pulls data from `tblTravelRequest`.

---

## 2️⃣ `USP_TravelUpdateStatus`

**Purpose:** Global State Updater

| Table Used                     | How It Is Used                                                                                |
| ------------------------------ | --------------------------------------------------------------------------------------------- |
| `tblTravelRequest`             | Updates main request status (e.g., 11 = Invoiced, 9 = Cancelled), cost, cities, travel dates. |
| `tblPassengers`                | Updates all related passengers to match header status (e.g., 11 or 9).                        |
| `tblUpdatedTavelRequestStatus` | Inserts audit history record every time status changes.                                       |

👉 This procedure updates **header + passengers + audit trail**.

---

## 3️⃣ `USP_InsertUpdateInvoiceDetails`

**Purpose:** Financial Record Keeper

| Table Used          | How It Is Used                                                                              |
| ------------------- | ------------------------------------------------------------------------------------------- |
| `tblInvoiceDetails` | Inserts new invoice record or updates existing one based on `InvoiceNumber`.                |
| `tblPassengers`     | Reads `Paxstatus` to determine whether amount goes to `InvoiceAmount` or `CancelledAmount`. |

👉 This procedure manages **financial records** and syncs with passenger status.

---

## 4️⃣ `USP_TravelUpdateCancelledticketStatus`

**Purpose:** Partial Cancellation Handler

| Table Used                      | How It Is Used                                                                                                              |
| ------------------------------- | --------------------------------------------------------------------------------------------------------------------------- |
| `tblPassengers`                 | Updates OneWay / TwoWay leg status, handles partial cancellations, sets final passenger status to 9 if both legs cancelled. |
| `tblTravelRequest` *(possibly)* | May update overall request status if all passengers/legs are cancelled (depends on implementation).                         |

👉 This procedure mainly works at the **passenger level**, handling split-leg logic.

---

# 🔎 Final Summary Matrix

| Stored Procedure                                | tblTravelRequest   | tblPassengers | tblInvoiceDetails | tblUpdatedTavelRequestStatus |
| ----------------------------------------------- | ------------------ | ------------- | ----------------- | ---------------------------- |
| USP_GetAllApprovedSavedTravelRequest_New_ConApp | ✅ Read             | ❌             | ❌                 | ❌                            |
| USP_TravelUpdateStatus                          | ✅ Update           | ✅ Update      | ❌                 | ✅ Insert                     |
| USP_InsertUpdateInvoiceDetails                  | ❌                  | ✅ Read        | ✅ Insert/Update   | ❌                            |
| USP_TravelUpdateCancelledticketStatus           | ⚠️ Possible Update | ✅ Update      | ❌                 | ❌                            |

---


