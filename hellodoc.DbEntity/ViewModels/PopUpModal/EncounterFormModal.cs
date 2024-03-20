using hellodoc.DbEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.DbEntity.ViewModels.PopUpModal
{
    public partial class EncounterFormModal
    {
        public string patientName { get; set; }
        public string? confirmationnumber { get; set; }
        public int requestid { get; set; }

        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string Location { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public DateOnly? DateOfService { get; set; }
        public long? Phone { get; set; }
        public string? Email { get; set; }
        public string HistoryOfInjury { get; set; }
        public string medicalHistory { get; set; }
        public string Medications { get; set; }
        public string Allergies { get; set; }
        public string Temperature { get; set; }
        public int HeartRate { get; set; }
        public int respiratoryRate { get; set; }
        public string BloodPressure { get; set; }
        public string O2 { get; set; }
        public string Pain { get; set; }
        public string HEENT { get; set; }
        public string CV { get; set; }
        public string Chest { get; set; }
        public string ABD { get; set; }
        public string Extr { get; set; }
        public string Skin { get; set; }
        public string Neuro { get; set; }
        public string Other { get; set; }
        public string Diagnosis { get; set; }
        public string TreatmentPlan { get; set; }
        public string Medicationdispensed { get; set; }
        public string Procedures { get; set; }
        public string FollowUp { get; set; } 



        public List<RequestWiseFile> PatientDocuments { get; set; }
    }
}
