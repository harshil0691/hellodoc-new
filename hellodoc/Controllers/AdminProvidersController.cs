using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.Shifts;
using hellodoc.Repositories.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using NUnit.Framework.Internal.Execution;
using hellodoc.DbEntity.ViewModels.DashboardLists;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using hellodoc.DbEntity.DataModels;
using Org.BouncyCastle.Tsp;
using Microsoft.IdentityModel.Tokens;

namespace hellodoc.Controllers
{
    public class AdminProvidersController : Controller
    {
        private readonly ILogger<AdminDashController> _logger;
        private readonly IAdminDashRepository _adminDashRepository;
        private readonly IPatientLogin _patientLogin;
        private readonly IRequests _requests;
        private readonly IHostingEnvironment HostingEnviroment;
        private readonly IAuthManager _authManger;
        private readonly IAdminProviders _adminProviders;

        public AdminProvidersController(ILogger<AdminDashController> logger, IAdminDashRepository adminDashRepository, IPatientLogin patientLogin, IRequests requests, IHostingEnvironment hostingEnvironment, IAuthManager authManager, IAdminProviders adminProviders)
        {
            _logger = logger;
            _adminDashRepository = adminDashRepository;
            _patientLogin = patientLogin;
            _requests = requests;
            HostingEnviroment = hostingEnvironment;
            _authManger = authManager;
            _adminProviders = adminProviders;
        }

        public IActionResult GetProvidersView(PartialViewModal partialView)
        {
            var date = DateTime.Now;
            if(partialView.datestring != null)
            {
                date = DateTime.Parse(partialView.datestring);
            }
            var physicianid = HttpContext.Session.GetInt32("physicianid");

            switch (partialView.actionType)
            {
                case "provider":
                    return PartialView("_Providers", _adminProviders.ProvidersTable(partialView.pageNumber,partialView.regionid));
                case "scheduling":
                    _authManger.authaction(HttpContext, "scheduling");
                    DashboardListsModal listsModal = new DashboardListsModal();
                    listsModal.regions = _adminDashRepository.GetRegions("",0);
                    listsModal.role = HttpContext.Session.GetString("loginType") ?? "admin";
                    listsModal.physicianId = physicianid??0;
                    return PartialView("_Scheduling",listsModal);
                case "invoicing":

                    InvoicingModal invoicing = new InvoicingModal();
                    invoicing.currentMonth = partialView.currentMonth;
                    invoicing.currentYear = partialView.currentYear;
                    invoicing.timeSlot = partialView.timeSlot;
                    invoicing.numberOfDays = partialView.numberOfDays;
                    invoicing.loginType = HttpContext.Session.GetString("loginType")??"admin";
                    invoicing.physicians = _adminDashRepository.GetPhysicianList();
                    invoicing.selectedPhysician = partialView.physicianid;

                    if(invoicing.loginType == "admin")
                    {
                        invoicing.invoicings = _adminProviders.GetInvoicings(partialView);
                    }

                    if(invoicing.loginType == "provider")
                    {
                        partialView.physicianid = HttpContext.Session.GetInt32("physiciandashid")??0;
                        invoicing.payrateCounts = _adminProviders.GetDashTimeSheet(partialView);
                    }


                    return PartialView("_Invoicing",invoicing);

                case "finalizetimesheet":


                    partialView.accoutOpen = HttpContext.Session.GetString("loginType")??"user";
                    if (partialView.accoutOpen == "provider")
                    {
                        partialView.physicianid = HttpContext.Session.GetInt32("physiciandashid")??0;
                    }
                    var timelist =  _adminProviders.GetTimesheets(partialView);

                    ViewBag.month = partialView.currentMonth;
                    ViewBag.year = partialView.currentYear;
                    ViewBag.timeslot = partialView.timeSlot;
                    ViewBag.physicianid = partialView.physicianid;

                    return PartialView("_FinalizeTimesheet",timelist);

                case "provideroncall":
                    var modal = _adminProviders.ProvidersOnCallList(partialView.regionid, date);
                    return PartialView("_ProvidersOnCall", modal);

                case "shiftreview":
                    ViewBag.back = partialView.back;
                    var list = _adminProviders.ShiftsDetailsList(partialView.regionid, partialView.pageNumber);
                    return PartialView("_ShiftsForReview", list);
                case "createProvider":
                    return PartialView("_CreateProvider",_adminProviders.GetForCreateProvider());

                default:
                    return PartialView("_default");
            }
        }

        [HttpPost]
        public IActionResult SaveTimesheet(List<Timesheet> timesheets,int physicianid, int month, int year, int slot)
        {
            var aspid = HttpContext.Session.GetInt32("Aspid");
            //_adminProviders.SaveTimesheet(timesheets,aspid??1,physicianid,month,year,slot);

            if(HttpContext.Session.GetString("loginType") == "provider")
            {
                return RedirectToAction("dashboard","ProviderDashboard");
            }
            return RedirectToAction("admin_dash","AdminDash");
        }

        [HttpPost]
        public void SaveReciept(RecieptModal reciept)
        {
            _adminProviders.SaveReciept(reciept,"save");
        }

        [HttpPost]
        public void DeleteReciept(int timesheetid)
        {
            _adminProviders.DeleteReciept(timesheetid,HttpContext.Session.GetInt32("Aspid")??1);
        }

        public IActionResult finalizeTimeSheet(int invoicingId)
        {
            _adminProviders.finalizeTimeSheet(invoicingId, HttpContext.Session.GetInt32("Aspid") ?? 1);
            return RedirectToAction("admin_dash","AdminDash");
        }

        public IActionResult approveTimeSheet(int invoicingId)
        {
            _adminProviders.approveTimesheet(invoicingId, HttpContext.Session.GetInt32("Aspid") ?? 1);
            return RedirectToAction("admin_dash", "AdminDash");
        }

        [HttpPost]
        public IActionResult CreateProvider(ProviderProfileModal providerProfile)
        {
            try
            {
                _adminProviders.CreateProvider(providerProfile);
                TempData["success"] = "Provider Is Created";
            }
            catch
            {
                TempData["Error"] = "Internal Error Provider Is Not Created";
            }
            return RedirectToAction("admin_dash", "AdminDash");
        }

        [HttpPost]
        public void stopnotification(List<int> idlist,List<int> totallist)
        {
            _adminProviders.StopNotification(idlist.Distinct().ToList(), totallist.Distinct().ToList());
        }

        
        public IActionResult edit_physician(int physicianid)
        {
            var userid1 = HttpContext.Session.GetInt32("Aspid");
            ViewBag.loginType = HttpContext.Session.GetString("loginType");
            ProviderProfileModal providerProfile = _adminProviders.ProviderProfileData(physicianid).Result;
            return PartialView("_ProviderProfile", providerProfile);
        }

        public IActionResult Payrate(int physicianid)
        {
            ViewBag.loginType = HttpContext.Session.GetString("loginType");
            var payrate  = _adminProviders.GetPayrate(physicianid);
            return PartialView("_PayRate",payrate);
        }

        [HttpPost]
        public void SavePayrate(int physicianid,int payratetype,int amount)
        {
            _adminProviders.SavePayrate(physicianid,payratetype,amount,HttpContext.Session.GetInt32("Aspid")??1);
        }


        public IActionResult delete_physician(int physicianid)
        {
            var delete = _adminProviders.DeleteProvider(physicianid);
            if (delete)
            {
                TempData["success"] = "Physican Deleted Successfully";
                return RedirectToAction("admin_dash","AdminDash");
            }
            else
            {
                TempData["error"] = "Internal Error Physician Not Deleted";
                return RedirectToAction("admin_dash", "AdminDash");
            }
        }

        public void UpdateProvider(ProviderProfileModal providerProfile)
        {
            if(providerProfile.selectedRegion != null)
            {
                List<int> list = JsonConvert.DeserializeObject<List<int>>(providerProfile.selectedRegion);
                providerProfile.regionList = list;
            }

            providerProfile.aspid = HttpContext.Session.GetInt32("Aspid")??0;
            _adminProviders.UpdateProvider(providerProfile);
        }

        [HttpPost]
        public IActionResult loadshift(string datestring,string sundaystring , string saturdaystring,string shifttype,int regionid)
        {
            DateTime date = DateTime.Parse(datestring);
            DateTime sunday = DateTime.Parse(sundaystring);
            DateTime saturday = DateTime.Parse(saturdaystring);

            var physicianid = HttpContext.Session.GetInt32("physiciandashid");

            switch (shifttype)
            {
                case "month":
                    MonthShiftModal monthShift = new MonthShiftModal();

                    var totalDays = DateTime.DaysInMonth(date.Year, date.Month);
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var startDayIndex = (int)firstDayOfMonth.DayOfWeek;

                    var dayceiling = (int)Math.Ceiling((float)(totalDays + startDayIndex) / 7);

                    monthShift.daysLoop = (int)dayceiling * 7;
                    monthShift.daysInMonth = totalDays ;
                    monthShift.firstDayOfMonth = firstDayOfMonth;
                    monthShift.startDayIndex = startDayIndex;
                    monthShift.shiftDetailsmodals = _adminProviders.ShiftDetailsmodal(date,sunday,saturday,"month",physicianid??0,regionid, HttpContext.Session.GetString("loginType") ?? "admin");

                    return PartialView("_MonthWiseShift", monthShift);

                case "week":

                    WeekShiftModal weekShift = new WeekShiftModal();

                    weekShift.Physicians = _adminProviders.physicians(regionid);
                    weekShift.shiftDetailsmodals = _adminProviders.ShiftDetailsmodal(date, sunday, saturday, "week",physicianid??0,regionid, HttpContext.Session.GetString("loginType") ?? "admin");

                    List<int> dlist = new List<int>();

                    for (var i = 0; i < 7; i++)
                    {
                        var date12 = sunday.AddDays(i);
                        dlist.Add(date12.Day);
                    }

                    weekShift.datelist = dlist.ToList();
                    weekShift.dayNames = new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

                    return PartialView("_WeekWiseShift", weekShift);

                case "day":

                    DayShiftModal dayShift = new DayShiftModal();
                    dayShift.Physicians = _adminProviders.physicians(regionid);
                    dayShift.shiftDetailsmodals = _adminProviders.ShiftDetailsmodal(date, sunday, saturday, "day", physicianid ?? 0, regionid, HttpContext.Session.GetString("loginType") ?? "admin");

                    return PartialView("_DayWiseShift",dayShift);

                default:
                    return PartialView("_default");
            }
            
        }

        [HttpPost]
        public IActionResult OpenModal(PartialViewModal partialView)
        {
            HttpContext.Session.SetInt32("shiftdetailsid", partialView.shiftdetailsid);
            ViewBag.loginType = HttpContext.Session.GetString("loginType");
            var physicianid = HttpContext.Session.GetInt32("physicianid");
            switch(partialView.actionType)
            {
                case "shiftdetails":
                    ShiftDetailsmodal shift = _adminProviders.GetShift(partialView.shiftdetailsid);
                    return PartialView("_ViewShift",shift);

                case "moreshifts":
                    var list = _adminProviders.ShiftDetailsmodal(partialView.shiftdate,DateTime.Now,DateTime.Now,"month",physicianid??0,partialView.regionid, HttpContext.Session.GetString("loginType") ?? "admin").Where(d => d.Shiftdate.Day == partialView.shiftdate.Day).ToList();
                    ViewBag.TotalShift = list.Count();
                    return PartialView("_MoreShifts",list);

                case "createshift":
                    ShiftDetailsmodal shiftDetailsmodal = new ShiftDetailsmodal();
                    shiftDetailsmodal.regions = _adminDashRepository.GetRegions(HttpContext.Session.GetString("loginType")??"admin",HttpContext.Session.GetInt32("physiciandashid")??0);
                    shiftDetailsmodal.physics = _adminDashRepository.GetPhysicianList();
                    shiftDetailsmodal.Physicianid = HttpContext.Session.GetInt32("physiciandashid")??0;
                    shiftDetailsmodal.LoginType = HttpContext.Session.GetString("loginType") ?? "admin";
                    shiftDetailsmodal.Shiftdate = DateTime.Now;
                    return PartialView("_CreateShift", shiftDetailsmodal);

                case "signature":
                    return PartialView("_SignatureModal");

                default:
                    return PartialView("_default");
            }
        }

        public void edit_shift(ShiftDetailsmodal shift)
        {
            var aspid = HttpContext.Session.GetInt32("userID");
            var shiftdetailsid = HttpContext.Session.GetInt32("shiftdetailsid");
            _adminProviders.EditShift(shiftdetailsid??1, shift,aspid??1);
        }

        public void return_shift()
        {
            var aspid = HttpContext.Session.GetInt32("userID");
            var shiftdetailsid = HttpContext.Session.GetInt32("shiftdetailsid");
            _adminProviders.StatusChangeShift(shiftdetailsid ?? 1,aspid??1);
        }
        public void delete_shift()
        {
            var aspid = HttpContext.Session.GetInt32("userID");
            var shiftdetailsid = HttpContext.Session.GetInt32("shiftdetailsid");
            _adminProviders.DeleteShift(shiftdetailsid ?? 1, aspid ?? 1);
        }

        [HttpPost]
        public void create_shift(ShiftDetailsmodal shiftDetailsmodal)
        {
            var aspid = HttpContext.Session.GetInt32("userID");
            int physician = HttpContext.Session.GetInt32("physiciandashid") ??0;
            
            if(physician != 0)
            {
                shiftDetailsmodal.Physicianid = physician;
            }
            _adminProviders.CreateShift(shiftDetailsmodal, aspid ?? 1);

            //return RedirectToAction("admin_dash", "AdminDash");
        }

        public bool checkShiftAvailability(int physicianid, TimeOnly starttime, TimeOnly endtime, DateTime shiftdate)
        {
            var result  = _adminProviders.CheckShift(physicianid,starttime, endtime, shiftdate);
            return result;
        }


        public IActionResult MDsXShiftReview(string actionType,int regionid,int pageNumber,string datestring)
        {
            DateTime date = DateTime.Parse(datestring);

            switch (actionType)
            {
                case "provideroncall":
                    var modal = _adminProviders.ProvidersOnCallList(regionid,date);
                    return PartialView("_ProvidersOnCall",modal);

                case "shiftreview":
                    var list = _adminProviders.ShiftsDetailsList(regionid, pageNumber);
                    return PartialView("_ShiftsForReview",list);

                default:
                    return PartialView("_default");
            }
            
        }

        public void SelectedShiftOperation(List<int> shiftdetailsid,string actionType)
        {
            _adminProviders.SelectedShiftOperation(shiftdetailsid,actionType);
        }

        //public void setreminder()
        //{
        //    string jsonFile = "hellodoc-419105-daaec0fa1c1d.json";
        //    string calendarId = @"pdhaduk300@gmail.com";

        //    string[] Scopes = { CalendarService.Scope.Calendar };

        //    ServiceAccountCredential credential;

        //    using (var stream =
        //        new FileStream(jsonFile, FileMode.Open, FileAccess.Read))
        //    {
        //        var confg = Google.Apis.Json.NewtonsoftJsonSerializer.Instance.Deserialize<JsonCredentialParameters>(stream);
        //        credential = new ServiceAccountCredential(
        //           new ServiceAccountCredential.Initializer("9f5f134d5043b32735fda161fe74fcf6fb8415d15772a81de3a145244877e9ea@group.calendar.google.com")
        //           {
        //               Scopes = Scopes
        //           }.FromPrivateKey(confg.PrivateKey));
        //    }

        //    var service = new CalendarService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = "Calendar API Sample",
        //    });

        //    var calendar = service.Calendars.Get(calendarId).Execute();
        //    Console.WriteLine("Calendar Name :");
        //    Console.WriteLine(calendar.Summary);

        //    Console.WriteLine("click for more .. ");
        //    Console.Read();


        //    // Define parameters of request.
        //    EventsResource.ListRequest listRequest = service.Events.List(calendarId);
        //    listRequest.TimeMin = DateTime.Now;
        //    listRequest.ShowDeleted = false;
        //    listRequest.SingleEvents = true;
        //    listRequest.MaxResults = 10;
        //    listRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        //    // List events.
        //    Events events = listRequest.Execute();
        //    Console.WriteLine("Upcoming events:");
        //    if (events.Items != null && events.Items.Count > 0)
        //    {
        //        foreach (var eventItem in events.Items)
        //        {
        //            string when = eventItem.Start.DateTime.ToString();
        //            if (String.IsNullOrEmpty(when))
        //            {
        //                when = eventItem.Start.Date;
        //            }
        //            Console.WriteLine("{0} ({1})", eventItem.Summary, when);
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("No upcoming events found.");
        //    }
        //    Console.WriteLine("click for more .. ");
        //    Console.Read();

        //    //var myevent = DB.Find(x => x.Id == "eventid" + 1);

        //    var InsertRequest = service.Events.Insert(myevent, calendarId);

        //    try
        //    {
        //        InsertRequest.Execute();
        //    }
        //    catch (Exception)
        //    {
        //        try
        //        {
        //            service.Events.Update(myevent, calendarId, myevent.Id).Execute();
        //            Console.WriteLine("Insert/Update new Event ");
        //            Console.Read();

        //        }
        //        catch (Exception)
        //        {
        //            Console.WriteLine("can't Insert/Update new Event ");

        //        }
        //    }
        //}


        //static List<Event> DB =
        //     new List<Event>() {
        //        new Event(){
        //            Id = "eventid" + 1,
        //            Summary = "Google I/O 2015",
        //            Location = "800 Howard St., San Francisco, CA 94103",
        //            Description = "A chance to hear more about Google's developer products.",
        //            Start = new EventDateTime()
        //            {
        //                DateTime = new DateTime(2019, 01, 13, 15, 30, 0),
        //                TimeZone = "America/Los_Angeles",
        //            },
        //            End = new EventDateTime()
        //            {
        //                DateTime = new DateTime(2019, 01, 14, 15, 30, 0),
        //                TimeZone = "America/Los_Angeles",
        //            },
        //             Recurrence = new List<string> { "RRULE:FREQ=DAILY;COUNT=2" },
        //            Attendees = new List<EventAttendee>
        //            {
        //                new EventAttendee() { Email = "lpage@example.com"},
        //                new EventAttendee() { Email = "sbrin@example.com"}
        //            }
        //        }
        //     };

    }
}
