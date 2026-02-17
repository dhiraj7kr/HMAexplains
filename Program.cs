using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace TravelBooking
{
    class Program
    {
        static string SiteUrl = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["SiteUrl"]);
        static string Username = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["UserName"]);
        static string PassWord = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["PassWord"]);
        static DateTime dt = DateTime.Now;
        static int CancelledTickets = 0;
        static int BookedTickets = 0;
        static int InvalidTickets = 0;
        static string company = "";
        static void Main(string[] args)
        {
            Console.WriteLine("Started");
            // ChangeStatusToExpired();
            GetData();
            Console.WriteLine("End");
            //Console.ReadLine();
        }

        private static void GetData()
        {
            try
            {
                //Get HMA requestID



                var Dt = GetAllBookedTravelRequest().Tables[0];
                //WriteToLogdata(JsonConvert.SerializeObject(Dt));
                //Console.WriteLine("Data Table:"+Dt.Rows);
                //Console.ReadLine();
                //Get updatedstatus table data to check weather mail sent or not
                // var Dtmailsend = GetAllUpdatedStatusRequest().Tables[0];

                //List<UpdatedData> updateds = new List<UpdatedData>();

                //updateds= ConvertDataTable<UpdatedData>(Dtmailsend);

                Exception logTicketCount = new Exception("Total Fetched Tickets Count: " + Dt.Rows.Count);
                WriteToLogs(logTicketCount);
                Console.WriteLine("Total Count: " + Dt.Rows.Count);
                
                //for each ticketId we will get data from HMA site and 
                for (var row = 0; row < Dt.Rows.Count; row++)
                {
                    //Console.WriteLine("LineNo:66");
                    //Console.WriteLine("Count: " + (row + 1));
                    using (var httpClient = new HttpClient())
                    {
                        //Console.WriteLine("LineNo:71");
                        try
                        {
                            if (Convert.ToString(Dt.Rows[row]["Company"]).ToLower() == "bfl")
                            {
                                //Console.WriteLine("LineNo:74");
                                //Making Api call to hma site to get updated hma response
                                string URL = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["GetTicketURL"]) + Convert.ToString(Dt.Rows[row]["HMARequestId"]);
                                //string URL = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["GetTicketURL"]) + "TRN3444";
                                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                                //Console.WriteLine("URL: "+URL);
                                string Data = httpClient.GetStringAsync(new Uri(URL)).Result;
                                //Console.WriteLine("Data :" + Data);
                                //Data = Data.Substring(1);
                                //Data = Data.Substring(Data.Length);
                                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Data);
                                if (Convert.ToString(response["statusCode"]) == "404" || Convert.ToString(response["statusCode"]) == "0" || (response["bookingSummary"] == null || Convert.ToString(response["bookingSummary"]) == "null"))
                                {
                                    Exception ex1 = new Exception("Above error is to get data for ticket Id: " + Convert.ToString(Dt.Rows[row]["HMARequestId"]));
                                    WriteToLogs(ex1);
                                }
                                else
                                {
                                    Console.WriteLine("Ticket Id: " + Dt.Rows[row]["HMARequestId"]);
                                    UpdateStatus(response, Dt, row, Data);
                                }
                               
                            }
                            else
                            {
                                //Making Api call to hma site to get updated hma response
                                string URL = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["BFDLGetTicketURL"]) + Convert.ToString(Dt.Rows[row]["HMARequestId"]);
                                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                                //Console.WriteLine("URL: "+URL);
                                string Data = httpClient.GetStringAsync(new Uri(URL)).Result;
                                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Data);
                                //Added This part of code if condition on 29MAR2025 for handling as good request
                                if (Convert.ToString(response["statusCode"]) == "404" || Convert.ToString(response["statusCode"]) == "0" || (response["bookingSummary"] == null || Convert.ToString(response["bookingSummary"]) == "null"))
                                {
                                    Exception ex1 = new Exception("Above error is to get data for ticket Id: " + Convert.ToString(Dt.Rows[row]["HMARequestId"]));
                                    WriteToLogs(ex1);
                                }
                                else
                                {
                                    Console.WriteLine("Ticket Id: " + Dt.Rows[row]["HMARequestId"]);
                                    UpdateStatus(response, Dt, row, Data);
                                }
                               
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine("LineNo:88");
                            WriteToLogs(ex);
                            Exception ex1 = new Exception("Above error is to get data for ticket Id: " + Convert.ToString(Dt.Rows[row]["HMARequestId"]));
                            WriteToLogs(ex1);
                            //string URL = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["AlternativeGetTicketURL"]) + Convert.ToString(Dt.Rows[row]["HMARequestId"]); //"TRN1403"; 
                            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                            //string Data = httpClient.GetStringAsync(new Uri(URL)).Result;
                            ////Data = Data.Substring(1);
                            ////Data = Data.Substring(Data.Length);
                            //var response = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Data);
                            ////Console.WriteLine("LineNo:94");
                            //UpdateStatus(response, Dt, row, Data);
                        }
                    }
                }

                Exception exceptionCount = new Exception("Booked Ticket Count: " + BookedTickets + " , Cancelled Ticket Count: " + CancelledTickets + " , Failed Booking Count: " + InvalidTickets);
                WriteToLogs(exceptionCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                WriteToLogs(ex);

            }
        }
        //normal update: when ticket is booked or pending.
        public static void UpdateStatus(dynamic response,DataTable Dt,int row, string Data)
        {
            try
            {
                //Console.WriteLine("Response:" + response);
                //Console.WriteLine("row:" + row);
                //Console.WriteLine("Data:"+Data);
                //Console.ReadLine();
                if (Convert.ToString(response["statusCode"]) == "404" || Convert.ToString(response["statusCode"]) == "0")
                {
                    Exception statusNotFountError = new Exception("Ticket Number: " + Dt.Rows[row]["HMARequestId"] + " , " + Convert.ToString(Dt.Rows[row]["Status"]) + " ,Invalid Request Id");
                    WriteToLogs(statusNotFountError);
                    InvalidTickets++;
                    //ModifiedDate
                    if (Convert.ToDateTime(Convert.ToString(Dt.Rows[row]["OneWaydepartureDate_1"])) < DateTime.Now)
                    {//Thipps Commenting
                        //Console.WriteLine("HMA requestID:"+Convert.ToString(Dt.Rows[row]["HMARequestId"])+"Id:"+ Convert.ToString(Dt.Rows[row]["Id"]));
                        //Console.ReadKey();
                     UpdateStatusToDraft(Convert.ToString(Dt.Rows[row]["HMARequestId"]), Convert.ToString(Dt.Rows[row]["Id"]));
                    }
                }


                // newly added for only cancelled and partially cancelled and booked

                // if (Convert.ToString(Dt.Rows[row]["Status"]) == "9" || Convert.ToString(Dt.Rows[row]["Status"]) == "11" || Convert.ToString(Dt.Rows[row]["Status"]) == "15")
                else if (
                    (Convert.ToString(Dt.Rows[row]["onewaystatus"]) == "9" && Convert.ToString(Dt.Rows[row]["journeyType"]) == "ONE_WAY") ||
                   (Convert.ToString(Dt.Rows[row]["onewaystatus"]) == "11" && Convert.ToString(Dt.Rows[row]["journeyType"]) == "ONE_WAY") ||
                   (Convert.ToString(Dt.Rows[row]["onewaystatus"]) == "11" && Convert.ToString(Dt.Rows[row]["Twowaystatus"]) == "11" && Convert.ToString(Dt.Rows[row]["journeyType"]) == "ROUND_TRIP") ||
                   (Convert.ToString(Dt.Rows[row]["Twowaystatus"]) == "9" && Convert.ToString(Dt.Rows[row]["Twowaystatus"]) == "9" && Convert.ToString(Dt.Rows[row]["journeyType"]) == "ROUND_TRIP"))
                {
                    string excMsg = "Ticket Number: " + Convert.ToString(Dt.Rows[row]["HMARequestId"]) + "," + Convert.ToString(Dt.Rows[row]["Status"]) + "," + Convert.ToString(response["bookingSummary"]["status"]);
                    Exception excomplete = new Exception(excMsg);
                    WriteToLogs(excomplete);
                    //Insert Update Cancellation Data
                    // made changes here
                    try
                    {
                        var DestinationsData = response["bookingSummary"]["destinations"];
                        var status = Convert.ToString(response["bookingSummary"]["status"]);
                        if (DestinationsData.Count > 0)
                        {
                            for (int DestCount = 0; DestCount < DestinationsData.Count; DestCount++)
                            {
                                var PassengerData = DestinationsData[DestCount]["paxDetail"];
                                if (PassengerData.Count > 0)
                                {
                                    for (int PaxCount = 0; PaxCount < PassengerData.Count; PaxCount++)
                                    {
                                        string FromCity = DestinationsData[DestCount].segment[0].origin;
                                        string ToCity = "";
                                        var SegmentData = DestinationsData[DestCount]["segment"];
                                        if (SegmentData.Count >= 2)
                                        {
                                            ToCity = DestinationsData[DestCount].segment[SegmentData.Count - 1].destination;
                                        }
                                        else
                                        {
                                            ToCity = DestinationsData[DestCount].segment[0].destination;
                                        }
                                        int InvoiceStatus = 11;
                                        if (Convert.ToString(status).ToLower() == "invoiced")
                                        {
                                            InvoiceStatus = 11;
                                        }
                                        else
                                        {
                                            if (Convert.ToString(status).ToLower() == "cancelled")
                                            {
                                                InvoiceStatus = 9;
                                            }

                                        }
                                        //Console.WriteLine("Line no:184");
                                        //Console.ReadLine();
                                        InsertUpdateInvoiceData(Convert.ToString(Dt.Rows[row]["Id"]), Username, Convert.ToString(PassengerData[PaxCount]["firstName"]), Convert.ToString(PassengerData[PaxCount]["lastName"]), Convert.ToString(PassengerData[PaxCount]["paxPriceDetail"]["totalFare"]),
                                            Convert.ToString(PassengerData[PaxCount]["invoiceNo"]), Convert.ToString(response["bookingSummary"]["createdDate"]), InvoiceStatus, FromCity, ToCity
                                            );
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogs(ex);
                    }
                }
                else
                {
                    //Console.WriteLine("Line no:201");
                    //Console.ReadKey();
                    string excMsg = "Ticket Number: " + Convert.ToString(Dt.Rows[row]["HMARequestId"]) + "," + Convert.ToString(Dt.Rows[row]["Status"]) + "," + Convert.ToString(response["bookingSummary"]["status"]);
                    string DbStatus = Convert.ToString(Dt.Rows[row]["Status"]);
                    Exception excomplete = new Exception(excMsg);
                    Console.WriteLine("LineNO:230");
                    WriteToLogs(excomplete);
                    //Console.WriteLine(excomplete);
                    //Console.WriteLine("inside if call success");
                    var status = Convert.ToString(response["bookingSummary"]["status"]);
                    string HMATravelRequestID = Convert.ToString(response["bookingSummary"]["travelPlanId"]);
                    DateTime OneWaydepartureTime = Convert.ToDateTime(response["bookingSummary"]["destinations"][0]["segment"][0]["departureDate"]);
                    DateTime OneWayarrivalTime = Convert.ToDateTime(response["bookingSummary"]["destinations"][0]["segment"][0]["arrivalDate"]);
                    DateTime? TwoWaydepartureTime = null;
                    DateTime? TwoWayarrivalTime = null;
                    if (Convert.ToString(Dt.Rows[row]["journeyType"]) != "ONE_WAY")
                    {
                        TwoWaydepartureTime = Convert.ToDateTime(response["bookingSummary"]["destinations"][1]["segment"][0]["departureDate"]);
                        TwoWayarrivalTime = Convert.ToDateTime(response["bookingSummary"]["destinations"][1]["segment"][0]["arrivalDate"]);
                    }
                    string totalFare = Convert.ToString(response["bookingSummary"]["priceDetail"]["totalFare"]);

                    // add data to tblupdatetravelrequest table
                    var CancelledData = response["bookingSummary"]["destinations"];
                    int CancellationDataavailabecount = 0;
                    //Console.WriteLine("LineNo:233");
                    //Console.WriteLine("CancelledData :" + CancelledData + "CancellationDataavailabecount :" + CancellationDataavailabecount);
                    //check whether the cancelled items in destinations array of the response
                    if ( Convert.ToString(status).ToLower() != "cancelled")
                    {

                        for (int count = 0; count < CancelledData.Count; count++)
                        {
                            if ((Convert.ToString(CancelledData[count]["status"]).ToLower() == "cancelled")||(Convert.ToString(CancelledData[count]["status"]).ToLower() == "partiallycancelled"))
                            {
                                for (int innerCount = 0; innerCount < CancelledData[count]["paxDetail"].Count; innerCount++)
                                {
                                    CancellationDataavailabecount++;
                                }
                            }
                        }
                        //Console.WriteLine("Cancelled Data:" + CancelledData);
                        //Console.WriteLine("CancellationDataavailabecount:" + CancellationDataavailabecount);
                        //Console.ReadLine();
                    }
                    else
                    {
                    }

                    // if ticket is applied for cancellation and still cancellation in pending from hma site
                    //ie. the response is same as previous then dont update anything
                    //if (Convert.ToString(Dt.Rows[row]["Status"]) == "6" && CancellationDataavailabecount == 0 && (status == "INVOICED" || status == "PENDING_CONFIRMATION"))
                    if ((Convert.ToString(Dt.Rows[row]["onewaystatus"]) == "6" || (Convert.ToString(Dt.Rows[row]["Twowaystatus"]) == "6")) && CancellationDataavailabecount == 0 && (Convert.ToString(status).ToLower() == "invoiced" || Convert.ToString(status).ToLower() == "pending_confirmation" || Convert.ToString(status).ToLower() == "pendingconfirmation"))
                    {

                    }
                    else
                    {
                        //Console.WriteLine("LineNo:257");
                        //if the status is pending in db and we got updated response from hma 
                        // it will go here 
                        //Console.WriteLine("Check Status: "+Convert.ToString(Dt.Rows[row]["Status"]));
                        if (CancellationDataavailabecount == 0)
                        {
                            if (Convert.ToString(Dt.Rows[row]["Status"]) == "14" || Convert.ToString(Dt.Rows[row]["Status"]) == "4" || Convert.ToString(Dt.Rows[row]["Status"]) == "1")
                            {
                                var DestinationsData = response["bookingSummary"]["destinations"];
                                string FromCity = DestinationsData[0].segment[0].origin;
                                string ToCity = "";
                                var SegmentData = DestinationsData[0]["segment"];
                                if (SegmentData.Count >= 2)
                                {
                                    ToCity = DestinationsData[0].segment[SegmentData.Count - 1].destination;
                                }
                                else
                                {
                                    ToCity = DestinationsData[0].segment[0].destination;
                                }
                                UpdateStatus(Data, status, Convert.ToString(Dt.Rows[row]["HMARequestId"]), Convert.ToString(Dt.Rows[row]["journeyType"]), HMATravelRequestID, OneWaydepartureTime, OneWayarrivalTime, TwoWaydepartureTime, TwoWayarrivalTime, totalFare,FromCity,ToCity);

                            }
                        }
                    }
                    //if status is cancelled no need to go for partial cancellation process

                    //for partial cancellation
                    //go thorught each destination in response and check the status and 
                    //get cancelled passenger details

                    //change here to update the cancellation status
                    //Console.WriteLine("LineNo:277");
                    //Console.WriteLine("CancelledData:"+ CancelledData);
                    //Console.ReadLine();
                    if (Convert.ToString(status).ToLower()  != "cancelled")
                    {
                        for (int count = 0; count < CancelledData.Count; count++)
                        {
                            if ((Convert.ToString(CancelledData[count]["status"]).ToLower() == "cancelled")|| (Convert.ToString(CancelledData[count]["status"]).ToLower() == "partiallycancelled"))
                            {

                                for (int innerCount = 0; innerCount < CancelledData[count]["paxDetail"].Count; innerCount++)
                                {
                                    string FromCity = CancelledData[count].segment[0].origin;
                                    string ToCity = "";
                                    var SegmentData = CancelledData[count]["segment"];
                                    if (SegmentData.Count >= 2)
                                    {
                                        ToCity = CancelledData[count].segment[SegmentData.Count - 1].destination;
                                    }

                                    else
                                    {
                                        ToCity = CancelledData[count].segment[0].destination;
                                    }
                                    //cancelled for 

                                    string JourneyType = Convert.ToString(Dt.Rows[row]["journeyType"]);
                                    string oneWayStatus = "";
                                    string twoWayStatus = "";
                                    if (JourneyType == "ONE_WAY")
                                    {

                                        oneWayStatus = "9";
                                    }
                                    if (JourneyType == "ROUND_TRIP")
                                    {
                                        string fromcity = Convert.ToString(Dt.Rows[row]["departureCity_1"]).Split('-')[0].ToString().Trim();
                                        string tocity = Convert.ToString(Dt.Rows[row]["arrivalCity_1"]).Split('-')[0].ToString().Trim();

                                        if (fromcity == FromCity)
                                        {

                                            oneWayStatus = "9";
                                        }
                                        if (tocity == FromCity)
                                        {

                                            twoWayStatus = "9";
                                        }
                                    }


                                    string FirstName = CancelledData[count]["paxDetail"][innerCount]["firstName"];
                                    string LastName = CancelledData[count]["paxDetail"][innerCount]["lastName"];
                                    UpdateCancelledPassangersData(Convert.ToInt32(Dt.Rows[row]["Id"]), FirstName, LastName, Convert.ToString(response["bookingSummary"]["priceDetail"]["totalFare"]), Data, JourneyType, oneWayStatus, twoWayStatus);
                                }
                            }
                        }
                    }
                    else
                    {
                        CancelledTickets++;
                        
                    }
                    //Console.WriteLine("LineNo:341");
                    //no need to change
                    if (DbStatus == "2" && (Convert.ToString(status).ToLower() == "invoiced" || Convert.ToString(status).ToLower() == "pending_confirmation" || Convert.ToString(status).ToLower() == "pendingconfirmation"))
                    {

                        if (Convert.ToDateTime(Convert.ToString(Dt.Rows[row]["OneWaydepartureDate_1"])) >= DateTime.Now)
                        {
                            GetMailTemplateData(SiteUrl, Username, GetPassword(PassWord), EmailTemplate.BookingConfirmationMail, Convert.ToString(Dt.Rows[row]["HMARequestId"]), Convert.ToString(Dt.Rows[row]["Company"]));
                        }
                        //Exception exmail = new Exception("Mail sent to: " + Convert.ToString(Dt.Rows[row]["HMARequestId"]));
                        // WriteToLogs(exmail);
                        BookedTickets++;
                    }
                    //Insert Update Cancellation Data
                    // made changes here
                    // this loop is to update the invoice details for the ticket
                    //Console.WriteLine("Line No:357");
                    //Console.ReadLine();
                    try
                    {
                        var DestinationsData = response["bookingSummary"]["destinations"];
                        if (DestinationsData.Count > 0)
                        {
                            for (int DestCount = 0; DestCount < DestinationsData.Count; DestCount++)
                            {
                                var PassengerData = DestinationsData[DestCount]["paxDetail"];
                                if (PassengerData.Count > 0)
                                {
                                    for (int PaxCount = 0; PaxCount < PassengerData.Count; PaxCount++)
                                    {
                                        string FromCity = DestinationsData[DestCount].segment[0].origin;
                                        string ToCity = "";
                                        var SegmentData = DestinationsData[DestCount]["segment"];
                                        if (SegmentData.Count >= 2)
                                        {
                                            ToCity = DestinationsData[DestCount].segment[SegmentData.Count - 1].destination;
                                        }
                                        else
                                        {
                                            ToCity = DestinationsData[DestCount].segment[0].destination;
                                        }
                                        int InvoiceStatus = 11;
                                        if (Convert.ToString(status).ToLower() == "invoiced")
                                        {
                                            InvoiceStatus = 11;
                                        }
                                        else
                                        {
                                            if (Convert.ToString(status).ToLower() == "cancelled")
                                            {
                                                InvoiceStatus = 9;
                                            }
                                        }
                                        InsertUpdateInvoiceData(Convert.ToString(Dt.Rows[row]["Id"]), Username, Convert.ToString(PassengerData[PaxCount]["firstName"]), Convert.ToString(PassengerData[PaxCount]["lastName"]), Convert.ToString(PassengerData[PaxCount]["paxPriceDetail"]["totalFare"]),
                                            Convert.ToString(PassengerData[PaxCount]["invoiceNo"]), Convert.ToString(response["bookingSummary"]["createdDate"]), InvoiceStatus, FromCity, ToCity
                                            );
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteToLogs(ex);
                    }
                }
            }
            catch(Exception ex)
            {
                Exception statusNotFountError = new Exception("Ticket Number: " + Dt.Rows[row]["HMARequestId"] + " , " + Convert.ToString(Dt.Rows[row]["Status"]) + ", Exception " + ex.Message + " ====" + ex.StackTrace + " ====" + ex.InnerException);
                WriteToLogs(statusNotFountError);
                InvalidTickets++;
            }
        }

        // handles Main logic method to decide the 
        public static void UpdateStatus(string responseCode, string status, string HMATravelID, string journeyType, string HMATravelRequestID, DateTime OneWaydepartureTime,
            DateTime OneWayarrivalTime, DateTime? TwoWaydepartureTime, DateTime? TwoWayarrivalTime, string totalFare,string FromCity, string ToCity)
        {
            //Console.WriteLine("responseCode :"+ responseCode+"Status: "+ status+"HMATravelID:"+ HMATravelID+"JourneyType: "+journeyType+ "HMATravelRequestID :" + HMATravelRequestID+ "OneWaydepartureTime :" + OneWaydepartureTime
            //+ "OneWayarrivalTime :" + OneWayarrivalTime+ "TwoWaydepartureTime :" + TwoWaydepartureTime+ "TwoWayarrivalTime :" + TwoWayarrivalTime+ "totalFare :" + totalFare);
            //Console.ReadLine();
            string connectionString = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            DataSet ds = new DataSet();
            try
            {

                var connection = new SqlConnection(connectionString);
                var command = new SqlCommand();
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "USP_TravelUpdateStatus";
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@HMATravelID", HMATravelID);
                command.Parameters.AddWithValue("@journeyType", journeyType);
                command.Parameters.AddWithValue("@HMATravelRequestID", HMATravelRequestID);
                command.Parameters.AddWithValue("@OneWaydepartureTime", OneWaydepartureTime);
                command.Parameters.AddWithValue("@OneWayarrivalTime", OneWayarrivalTime);
                command.Parameters.AddWithValue("@TwoWaydepartureTime", TwoWaydepartureTime);
                command.Parameters.AddWithValue("@TwoWayarrivalTime", TwoWayarrivalTime);
                command.Parameters.AddWithValue("@responseCode", responseCode);
                command.Parameters.AddWithValue("@IsEmailSent", true);
                command.Parameters.AddWithValue("@totalFare", totalFare);
                command.Parameters.AddWithValue("@FromCity", FromCity);
                command.Parameters.AddWithValue("@ToCity",ToCity);
                //SqlDataAdapter adapter = new SqlDataAdapter(command);
                //adapter.Fill(ds);
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                WriteToLogs(ex);
                Exception ex1 = new Exception("above error is for updating ticket status ticket id: " + HMATravelID + " , " + HMATravelRequestID);
                WriteToLogs(ex1);
            }
        }



        public static void UpdateCancelledPassangersData(int HMAid, string FirstName, string LastName, string totalFare, string responseCode, string JouneyType, string OnewWayStatus, string TwoWayStatus)
        {
            //Console.WriteLine("UpdateCancelledPassangersData:");
            //Console.WriteLine("HMAid: "+ HMAid+ "FirstName :" + FirstName+ "LastName :" + LastName+ "totalFare :"+ totalFare+ "JouneyType :" + JouneyType+ "OnewWayStatus :" + OnewWayStatus+ "TwoWayStatus :" + TwoWayStatus);
            //Console.ReadLine();
            string connectionString = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            DataSet ds = new DataSet();
            try
            {


                var connection = new SqlConnection(connectionString);
                var command = new SqlCommand();
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "USP_TravelUpdateCancelledticketStatus";
                command.Parameters.AddWithValue("@FirstName", FirstName);
                command.Parameters.AddWithValue("@LastName", LastName);
                command.Parameters.AddWithValue("@totalFare", totalFare);
                command.Parameters.AddWithValue("@HMATravelID", HMAid);
                command.Parameters.AddWithValue("@JourneyType", JouneyType);
                command.Parameters.AddWithValue("@OneWayStatus", OnewWayStatus);
                command.Parameters.AddWithValue("@TwoWayStatus", TwoWayStatus);
                //command.Parameters.AddWithValue("@responseCode", responseCode);
                //SqlDataAdapter adapter = new SqlDataAdapter(command);
                //adapter.Fill(ds);
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                WriteToLogs(ex);
                Exception ex1 = new Exception("above error is for updating cancelled pax details ticket id: " + HMAid);
                WriteToLogs(ex1);
            }
        }



        public static DataSet GetAllBookedTravelRequest()
        {
            string connectionString = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            DataSet ds = new DataSet();
            try
            {

                var connection = new SqlConnection(connectionString);
                var command = new SqlCommand();
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "USP_GetAllApprovedSavedTravelRequest_New_ConApp";
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
                connection.Close();
            }
            catch (Exception ex)
            {
                WriteToLogs(ex);
            }
            return ds;
        }


        //invalid ticket
        public static void UpdateStatusToDraft(string HMARequestId, string Id)
        {
            string connectionString = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            DataSet ds = new DataSet();
            try
            {

                var connection = new SqlConnection(connectionString);
                var command = new SqlCommand();
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "USP_UpdateStatustoDraft";

                command.Parameters.AddWithValue("@HMATravelID", HMARequestId);
                command.Parameters.AddWithValue("@Id", Id);
                //SqlDataAdapter adapter = new SqlDataAdapter(command);
                //adapter.Fill(ds);
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                WriteToLogs(ex);
                Exception ex1 = new Exception("above error is for updating cancelled pax details ticket id: " + HMARequestId);
                WriteToLogs(ex1);
            }
        }



        public static DataSet GetAllUpdatedStatusRequest()
        {
            string connectionString = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            DataSet ds = new DataSet();
            try
            {

                var connection = new SqlConnection(connectionString);
                var command = new SqlCommand();
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "USP_GetAllUpdatedStatusRequest";
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
                connection.Close();
            }
            catch (Exception ex)
            {
                WriteToLogs(ex);
            }
            return ds;
        }

        //fully cancelled or invoiced
        public static void InsertUpdateInvoiceData(string HMAId, string CurrentUser, string FirstName, string LastName, string InvoiceAmount, string InvoiceNumber
            , string InvoiceDate, int Status, string FromCity, string ToCity)
        {
            //Console.WriteLine("InsertUpdateInvoiceData");
            //Console.ReadLine();
            string connectionString = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            try
            {

                DataSet ds = new DataSet();

                var connection = new SqlConnection(connectionString);
                var command = new SqlCommand();
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "USP_InsertUpdateInvoiceDetails";

                command.Parameters.AddWithValue("@HMARequestId", HMAId);
                command.Parameters.AddWithValue("@FirstName", FirstName);
                command.Parameters.AddWithValue("@LastName", LastName);
                command.Parameters.AddWithValue("@InvoiceAmount", InvoiceAmount);
                command.Parameters.AddWithValue("@InvoiceNumber", InvoiceNumber);
                command.Parameters.AddWithValue("@InvoiceDate", InvoiceDate);
                command.Parameters.AddWithValue("@Status", Status);
                command.Parameters.AddWithValue("@CreatedBy", CurrentUser);
                command.Parameters.AddWithValue("@ModifiedBy", CurrentUser);
                command.Parameters.AddWithValue("@FromCity", FromCity);
                command.Parameters.AddWithValue("@ToCity", ToCity);
                //SqlDataAdapter adapter = new SqlDataAdapter(command);
                //adapter.Fill(ds);
                command.ExecuteScalar();
                connection.Close();


            }
            catch (Exception ex)
            {
                WriteToLogs(ex);

            }

        }


        static public void WriteToLogs(Exception e)
        {
            try
            {
                string Error1 = e.Message + "," + DateTime.Now + Environment.NewLine;

                var currentDirectory = System.IO.Directory.GetCurrentDirectory();
                //string Path = "C:\\inetpub\\wwwroot\\meshwebapi\\HMA\\log.txt";
                string Path = ConfigurationManager.AppSettings["ExceptionLogPath"] + "log" + dt.Date.ToString("dd-MM-yyyy") + ".txt";
                //var myfile = File.Create(path);


                // File.OpenWrite(Path);
                System.IO.File.AppendAllText(Path, Error1);


                //myfile.Close();

            }
            catch (Exception ex)
            {

            }

        }

        static public void WriteToLogdata(string e)
        {
            try
            {
                string Error1 = e + "," + DateTime.Now + Environment.NewLine;

                var currentDirectory = System.IO.Directory.GetCurrentDirectory();
                //string Path = "C:\\inetpub\\wwwroot\\meshwebapi\\HMA\\log.txt";
                string Path = ConfigurationManager.AppSettings["LogPath"] + "logDatafile" + dt.Date.ToString("dd-MM-yyyy") + ".txt";
                //string Path = "C:\\HMA\\HMA_UAT_Job\\Debug\\log.txt";

                //var myfile = File.Create(path);


                // File.OpenWrite(Path);
                System.IO.File.AppendAllText(Path, Error1);


                //myfile.Close();

            }
            catch (Exception ex)
            {

            }

        }




        public static SecureString GetPassword(string Password)
        {
            SecureString password = new SecureString();
            Array.ForEach(Password.ToArray(), password.AppendChar);
            password.MakeReadOnly();
            return password;
        }



        public static void GetMailTemplateData(string Url, string UserName, SecureString passwrd, string Catagory, string HMAid,string Company)
        {
            try
            {
                using (ClientContext clientcntx = new ClientContext(Url))
                {
                    clientcntx.Credentials = new SharePointOnlineCredentials(UserName, passwrd);
                    Web webpage = clientcntx.Web;
                    clientcntx.Load(webpage);
                    List oList = clientcntx.Web.Lists.GetByTitle("EmailTemplate");
                    CamlQuery camlQuery = new CamlQuery();
                    camlQuery.ViewXml = "<Query>" +
                        "<Where><Eq><FieldRef Name='Category' /><Value Type='Text'>" + Catagory + "</Value></Eq></Where>" +
                                      "</Query>";
                    ListItemCollection collListItem = oList.GetItems(camlQuery);
                    clientcntx.Load(collListItem);
                    clientcntx.ExecuteQuery();
                    string body = "";
                    SendMail sendMail = new SendMail();
                    MailData mailData = new MailData();
                    foreach (var item in collListItem)
                    {
                        if (Convert.ToString(item["Category"]) == "BookingConfirmationMail")
                        {
                            mailData.body = Convert.ToString(item["EmailBody"]);
                            mailData.to = Convert.ToString(item["to"]);
                            mailData.cc = Convert.ToString(item["cc"]);
                            mailData.subject = Convert.ToString(item["EmailSubject"]);
                        }
                    }
                    sendMail.mailData = mailData;
                    sendMail.Category = Catagory;
                    sendMail.ID = HMAid;
                    Mail mail = new Mail();
                    mail.PostSendMail(sendMail,Company);
                    // Console.ReadKey();
                    // Console.WriteLine("success");
                }
            }
            catch (Exception ex)
            {
                WriteToLogs(ex);
                Exception ex1 = new Exception("Mail Send Failed for ticket Id: " + HMAid);
                WriteToLogs(ex1);
            }
        }



        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }



        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }


        private static void ChangeStatusToExpired()
        {
            //Get HMA requestID
            var Dt = GetPendingApprovalTravelRequests().Tables[0];
            Exception logTicketCount = new Exception("Total pending approval status Count: " + Dt.Rows.Count);
            WriteToLogs(logTicketCount);
            Console.WriteLine("Total Count: " + Dt.Rows.Count);

            for (var row = 0; row < Dt.Rows.Count; row++)
            {
                try
                {

                    if (Convert.ToDateTime(Convert.ToDateTime(Dt.Rows[row]["OneWaydepartureDate_1"]).ToString("dd-MM-yyyy")).Date < DateTime.Now.Date)
                    {
                        string excMsg = "Ticket Number: " + Convert.ToString(Dt.Rows[row]["HMARequestId"]) + "," + Convert.ToString(Dt.Rows[row]["Status"]) + "-" + "- Expired";
                        Exception excomplete = new Exception(excMsg);
                        WriteToLogs(excomplete);

                        UpdateStatusToExpired(Convert.ToString(Dt.Rows[row]["HMARequestId"]), Convert.ToString(Dt.Rows[row]["Id"]));
                    }
                }
                catch (Exception ex)
                {
                    Exception statusNotFountError = new Exception("Ticket Number: " + Dt.Rows[row]["HMARequestId"] + " , " + Convert.ToString(Dt.Rows[row]["Status"]) + ", Exception " + ex.Message);
                    WriteToLogs(statusNotFountError);
                }
            }
        }

        public static DataSet GetPendingApprovalTravelRequests()
        {
            string connectionString = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            DataSet ds = new DataSet();
            try
            {
                var connection = new SqlConnection(connectionString);
                var command = new SqlCommand();
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "USP_GetPendingApprovalTravelRequests";
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
                connection.Close();
            }
            catch (Exception ex)
            {
                WriteToLogs(ex);
            }
            return ds;
        }

        public static void UpdateStatusToExpired(string HMARequestId, string Id)
        {
            string connectionString = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["connectionString"]);
            DataSet ds = new DataSet();
            try
            {

                var connection = new SqlConnection(connectionString);
                var command = new SqlCommand();
                connection.Open();
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "USP_UpdateStatustoExpired";
                command.Parameters.AddWithValue("@HMATravelID", HMARequestId);
                command.Parameters.AddWithValue("@Id", Id);
                //SqlDataAdapter adapter = new SqlDataAdapter(command);
                //adapter.Fill(ds);
                command.ExecuteScalar();
                connection.Close();
            }
            catch (Exception ex)
            {
                WriteToLogs(ex);
                Exception ex1 = new Exception("above error is for updating expired date ticket id: " + HMARequestId);
                WriteToLogs(ex1);
            }
        }

    }



}
