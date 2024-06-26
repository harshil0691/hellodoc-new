﻿using hellodoc.DbEntity.DataModels;
using hellodoc.DbEntity.ViewModels;
using hellodoc.DbEntity.ViewModels.DashboardLists;
using hellodoc.DbEntity.ViewModels.Shifts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hellodoc.Repositories.Repository.Interface
{
    public interface IAdminProviders
    {
        Physician GetPhysicianAsync(int physicianid);
        DashboardListsModal ProvidersTable(int pageNumber,int regionid);
        void StopNotification(List<int> idlist,List<int> totallist);
        Task<ProviderProfileModal> ProviderProfileData(int physicianid);
        List<ShiftDetailsmodal> ShiftDetailsmodal(DateTime date,DateTime sunday, DateTime saturday,string type,int physicianid,int regionid, string schedulingFor);
        ShiftDetailsmodal GetShift(int shiftdetailsid);
        List<Physician> physicians(int regionid);
        bool UpdateProvider(ProviderProfileModal providerProfile);
        bool DeleteProvider(int providerid);
        void EditShift(int shiftdetailsid,ShiftDetailsmodal shiftDetailsmodal,int aspid);
        void DeleteShift(int shiftdetailsid,int aspid);
        void StatusChangeShift(int shiftdetailsid, int aspid);
        void CreateShift(ShiftDetailsmodal shiftDetailsmodal, int aspid);
        bool CheckShift(int physicianid, TimeOnly starttime, TimeOnly endtime, DateTime shiftdate);
        DashboardListsModal ProvidersOnCallList(int regionid,DateTime date);
        DashboardListsModal ShiftsDetailsList(int regionid,int pageNumber);
        void SelectedShiftOperation(List<int> shiftdetailid, string actionType);
        ProviderProfileModal GetForCreateProvider();
        void CreateProvider(ProviderProfileModal providerProfile);
        List<Timesheet> GetTimesheets(PartialViewModal partialView);
        List<PayrateCountModal> GetDashTimeSheet(PartialViewModal partialView);
        List<Invoicing> GetInvoicings(PartialViewModal partialView);
        Payrate GetPayrate(int physicianid);
        void SavePayrate(int physicianid,int payratetype, int amount,int aspid);

        void SaveTimesheet(List<Timesheet> timesheets,int aspid, int physicianid, int month , int year , int slot);
        void SaveReciept(RecieptModal reciept,string saveType);
        void DeleteReciept(int timesheetid,int aspid);
        void finalizeTimeSheet(int invoicingid,int aspid);
        void approveTimesheet(int invoicingid, int aspid);
    }
}
