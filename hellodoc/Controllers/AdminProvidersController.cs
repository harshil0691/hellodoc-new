﻿using Google.Apis.Auth.OAuth2;
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

        [HttpPost]
        public void stopnotification(List<int> idlist,List<int> totallist)
        {
            _adminProviders.StopNotification(idlist, totallist);
        }

        [HttpPost]
        public IActionResult edit_physician(int physicianid)
        {
            var userid1 = HttpContext.Session.GetInt32("Aspid");

            ProviderProfileModal providerProfile = _adminProviders.ProviderProfileData(physicianid).Result;

            return PartialView("_ProviderProfile", providerProfile);
        }

        [HttpPost]
        public IActionResult loadshift(string datestring,string sundaystring , string saturdaystring,string shifttype)
        {
            DateTime date = DateTime.Parse(datestring);
            DateTime sunday = DateTime.Parse(sundaystring);
            DateTime saturday = DateTime.Parse(saturdaystring);


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
                    monthShift.shiftDetailsmodals = _adminProviders.ShiftDetailsmodal(date,sunday,saturday,"month");

                    return PartialView("_MonthWiseShift", monthShift);

                case "week":

                    WeekShiftModal weekShift = new WeekShiftModal();

                    weekShift.Physicians = _adminProviders.physicians();
                    weekShift.shiftDetailsmodals = _adminProviders.ShiftDetailsmodal(date, sunday, saturday, "week");

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
                    dayShift.Physicians = _adminProviders.physicians();
                    dayShift.shiftDetailsmodals = _adminProviders.ShiftDetailsmodal(date, sunday, saturday, "day");

                    return PartialView("_DayWiseShift",dayShift);

                default:
                    return PartialView("_default");
            }
            
        }

        [HttpPost]
        public IActionResult OpenModal(PartialViewModal partialView)
        {
            HttpContext.Session.SetInt32("shiftdetailsid", partialView.shiftdetailsid);
            switch(partialView.actionType)
            {
                case "shiftdetails":
                    ShiftDetailsmodal shift = _adminProviders.GetShift(partialView.shiftdetailsid);
                    return PartialView("_ViewShift",shift);

                case "moreshifts":
                    var list = _adminProviders.ShiftDetailsmodal(partialView.shiftdate,DateTime.Now,DateTime.Now,"month").Where(d => d.Shiftdate.Day == partialView.shiftdate.Day).ToList();
                    ViewBag.TotalShift = list.Count();
                    return PartialView("_MoreShifts",list);

                case "createshift":
                    ShiftDetailsmodal shiftDetailsmodal = new ShiftDetailsmodal();
                    shiftDetailsmodal.regions = _adminDashRepository.GetRegions();
                    shiftDetailsmodal.Shiftdate = DateTime.Now;
                    return PartialView("_CreateShift", shiftDetailsmodal);


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

        public void create_shift(ShiftDetailsmodal shiftDetailsmodal)
        {
            var aspid = HttpContext.Session.GetInt32("userID");
            _adminProviders.CreateShift(shiftDetailsmodal, aspid??1);
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
