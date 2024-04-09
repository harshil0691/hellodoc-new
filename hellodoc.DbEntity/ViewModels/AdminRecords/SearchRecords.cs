using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.AdminRecords
{
    public partial class SearchRecords
    {
        public int RequestId { get; set; }
        public int RequestClientId { get; set; }
        public string? Patientname { get; set; }
        public string? Requestor { get; set; }
        public string? DateOfService { get; set; }
        public string? ClosedCaseDate { get; set; }
        public string? Email { get; set; }
        public long? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public long? Zip { get; set; }
        public string? RequestStatus { get; set; }
        public string? Physician { get; set; }
        public string? PhysicianNote { get; set; }
        public string? CancelledByProviderNote { get; set; }
        public string? AdminNote { get; set; }
        public string? PatientNote { get; set; }

    }
}
