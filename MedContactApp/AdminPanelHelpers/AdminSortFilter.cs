using AutoMapper;
using MedContactApp.AdminPanelHelpers;
using MedContactApp.FilterSortHelpers;
using MedContactApp.FilterSortPageHelpers;
using MedContactApp.Helpers;
using MedContactApp.Models;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
namespace MedContactApp.AdminPanelHelpers
{
    public class AdminSortFilter
    {

        internal IQueryable<User> UserSort(IQueryable<User> users, SortState sortOrder, int fSort = 0)
        {
            Expression<Func<User, string?>>? orderFamilyBy = default;
            if (fSort == 1)
            {
                orderFamilyBy = d => d!.Family!.MainUserId.Equals(d.Id) ? ("!" + d.FamilyId.ToString() + "A") :
                                (d.FamilyId != null ? ("!" + d.FamilyId.ToString() + "B") : ("z" + d.FamilyId.ToString() + "z"));
            }

            switch (sortOrder)
            {
                case SortState.EmailDesc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenByDescending(s => s.Email);
                    else users = users.OrderByDescending(s => s.Email);
                    break;
                case SortState.NameAsc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenBy(s => s.Name);
                    else users = users.OrderBy(s => s.Name);
                    break;
                case SortState.NameDesc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenByDescending(s => s.Name);
                    else users = users.OrderByDescending(s => s.Name);
                    break;
                case SortState.SurnameAsc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenBy(s => s.Surname);
                    else users = users.OrderBy(s => s.Surname);
                    break;
                case SortState.SurnameDesc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenByDescending(s => s.Surname);
                    else users = users.OrderByDescending(s => s.Surname);
                    break;
                case SortState.BirtDateAsc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenBy(s => s.BirthDate);
                    else users = users.OrderBy(s => s.BirthDate);
                    break;
                case SortState.BirthDateDesc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenByDescending(s => s.BirthDate);
                    else users = users.OrderByDescending(s => s.BirthDate);
                    break;
                case SortState.LastLoginAsc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenBy(s => s.LastLogin);
                    else users = users.OrderBy(s => s.LastLogin);
                    break;
                case SortState.LastLoginDesc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenByDescending(s => s.LastLogin);
                    else users = users.OrderByDescending(s => s.LastLogin);
                    break;
                case SortState.IsFullBlockedAsc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenBy(s => s.IsFullBlocked);
                    else users = users.OrderBy(s => s.IsFullBlocked);
                    break;
                case SortState.IsFullBlockedDesc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenByDescending(s => s.IsFullBlocked);
                    else users = users.OrderByDescending(s => s.IsFullBlocked);
                    break;
                case SortState.IsFamilyDependentAsc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenBy(s => s.IsDependent);
                    else users = users.OrderBy(s => s.IsDependent);
                    break;
                case SortState.IsFamilyDependentDesc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenByDescending(s => s.IsDependent);
                    else users = users.OrderByDescending(s => s.IsDependent);
                    break;
                case SortState.IsOnlineAsc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenBy(s => s.IsOnline);
                    else users = users.OrderBy(s => s.IsOnline);
                    break;
                case SortState.IsOnlineDesc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenByDescending(s => s.IsOnline);
                    else users = users.OrderByDescending(s => s.IsOnline);
                    break;
                case SortState.GenderAsc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenBy(s => s.Gender);
                    else users = users.OrderBy(s => s.Gender);
                    break;
                case SortState.GenderDesc:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenByDescending(s => s.Gender);
                    else users = users.OrderByDescending(s => s.Gender);
                    break;

                default:
                    if (orderFamilyBy != null) users = users.OrderBy(orderFamilyBy).ThenBy(s => s.Email);
                    else users = users.OrderBy(s => s.Email);
                    break;
            }

            return users;
        }
        internal IQueryable<User> UserFilter(IQueryable<User> users, string email, string name, string surname)
        {
            if (!string.IsNullOrEmpty(email))
            {

                users = users.Where(p => p.Email!.Contains(email));
            }
            if (!string.IsNullOrEmpty(name))
            {

                users = users.Where(p => p.Name!.Contains(name) || 
                                    p.MidName!.Contains(name) || 
                                    p.Surname!.Contains(name));
            }
            if (!string.IsNullOrEmpty(surname))
            {
                
                users = users.Where(p => p.Surname!.Contains(surname));
            }
            return users;
        }

        internal IEnumerable<AppointmentDto> AppointmentFilter(IEnumerable<AppointmentDto> apms, string? speciality, string? name, string? date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                var resTime = DateTime.TryParse(date, out DateTime sDate);
                if (resTime)
                {
                    apms = apms.Where(a => a.StartTime.Equals(sDate) ||
                    (a.DayTimeTable != null && a.DayTimeTable.Date.Equals(sDate)));
                }
            }
            if (!string.IsNullOrEmpty(speciality))
            {
                
                apms = apms.Where(p => p?.DayTimeTable?.DoctorData?.Speciality?.Name != null && 
                                  p.DayTimeTable.DoctorData.Speciality.Name.Contains(speciality, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(name))
            {
                
                apms = apms.Where(p =>
                p?.DayTimeTable?.DoctorData?.User?.Name != null && p.DayTimeTable.DoctorData.User.Name.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                p?.DayTimeTable?.DoctorData?.User?.MidName != null && p.DayTimeTable.DoctorData.User.MidName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                p?.DayTimeTable?.DoctorData?.User?.Surname != null && p.DayTimeTable.DoctorData.User.Surname.Contains(name, StringComparison.OrdinalIgnoreCase));
            }
           
            return apms;
        }
        internal IEnumerable<AppointmentDto> AppointmentCustomerFilter(IEnumerable<AppointmentDto> apms, string? name, string? birthdate)
        {
            if (!string.IsNullOrEmpty(birthdate))
            {
                var resTime = DateTime.TryParse(birthdate, out DateTime bDate);
                if (resTime)
                {   
                    apms = apms.Where(a => a.CustomerData?.User?.BirthDate!= null && a.CustomerData?.User?.BirthDate.Value.Year.Equals(bDate.Year) ==true);
                }
            }

            if (!string.IsNullOrEmpty(name))
            {

                apms = apms.Where(p =>
                p?.CustomerData?.User?.Name != null && p.CustomerData?.User?.Name?.Contains(name, StringComparison.OrdinalIgnoreCase)==true ||
                p?.CustomerData?.User?.MidName != null && p.CustomerData?.User?.MidName.Contains(name, StringComparison.OrdinalIgnoreCase)==true ||
                p?.CustomerData?.User?.Surname != null && p.CustomerData?.User?.Surname.Contains(name, StringComparison.OrdinalIgnoreCase) == true);
            }

            return apms;
        }

        internal IEnumerable<AppointmentDto> AppointmentCustomerSort(IEnumerable<AppointmentDto> apms, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.DateDesc:
                    apms = apms.OrderByDescending(s => s.StartTime);
                    break;
                case SortState.NameAsc:
                    apms = apms.OrderBy(s => s?.CustomerData?.User?.Name);
                    break;
                case SortState.NameDesc:
                    apms = apms.OrderByDescending(s => s?.CustomerData?.User?.Name);
                    break;
                case SortState.BirtDateAsc:
                    apms = apms.OrderBy(s => s?.CustomerData?.User?.BirthDate);
                    break;
                case SortState.BirthDateDesc:
                    apms = apms.OrderByDescending(s => s?.CustomerData?.User?.BirthDate);
                    break;
                case SortState.SurnameAsc:
                    apms = apms.OrderBy(s => s?.CustomerData?.User?.Surname);
                    break;
                case SortState.SurnameDesc:
                    apms = apms.OrderByDescending(s => s?.CustomerData?.User?.Surname);
                    break;
                default:

                    apms = apms.OrderBy(s => s.StartTime);
                    break;
            }

            return apms;
        }
        internal IEnumerable<AppointmentDto> AppointmentSort(IEnumerable<AppointmentDto> apms, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.DateAsc:
                    apms = apms.OrderBy(s => s.StartTime);
                    break;
                case SortState.NameAsc:
                    apms = apms.OrderBy(s => s?.DayTimeTable?.DoctorData?.User?.Name);
                    break;
                case SortState.NameDesc:
                    apms = apms.OrderByDescending(s => s?.DayTimeTable?.DoctorData?.User?.Name);
                    break;
                case SortState.SurnameAsc:
                    apms = apms.OrderBy(s => s?.DayTimeTable?.DoctorData?.User?.Surname);
                    break;
                case SortState.SurnameDesc:
                    apms = apms.OrderByDescending(s => s?.DayTimeTable?.DoctorData?.User?.Surname);
                    break;
                case SortState.SpecialityAsc:
                    apms = apms.OrderBy(s => s?.DayTimeTable?.DoctorData?.Speciality?.Name);
                    break;
                case SortState.SpecialityDesc:
                    apms = apms.OrderByDescending(s => s?.DayTimeTable?.DoctorData?.Speciality?.Name);
                    break;

                default:
                    apms = apms.OrderByDescending(s => s.StartTime);
                    break;
            }

            return apms;
        }
        internal IQueryable<DoctorData> DoctorDataFilter(IQueryable<DoctorData> dDatas, string email, string name, string surname, string speciality)
        {
            if (!string.IsNullOrEmpty(email))
            {

                dDatas = dDatas.Where(p => p.User!.Email != null && p.User.Email.Contains(email));
            }
            if (!string.IsNullOrEmpty(name))
            {
            
                dDatas = dDatas.Where(p => p.User!.Name != null && p.User.Name.Contains(name)
                                        || p.User!.MidName != null && p.User.MidName.Contains(name)
                                        || p.User!.Surname != null && p.User.Surname.Contains(name));
            }
            if (!string.IsNullOrEmpty(surname))
            {
               
                dDatas = dDatas.Where(p => p.User!.Surname != null && p.User.Surname.Contains(surname));
            }
            if (!string.IsNullOrEmpty(speciality))
            {
                
                dDatas = dDatas.Where(p => p.Speciality!.Name != null && p.Speciality.Name.Contains(speciality));
            }
            return dDatas;
        }
        internal IQueryable<DoctorData> DoctorDataSort(IQueryable<DoctorData> dDatas, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.EmailDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.Email);
                    break;
                case SortState.NameAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.Name);
                    break;
                case SortState.NameDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.Name);
                    break;
                case SortState.SurnameAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.Surname);
                    break;
                case SortState.SurnameDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.Surname);
                    break;
                case SortState.BirtDateAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.BirthDate);
                    break;
                case SortState.BirthDateDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.BirthDate);
                    break;
                case SortState.LastLoginAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.LastLogin);
                    break;
                case SortState.LastLoginDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.LastLogin);
                    break;
                case SortState.IsFullBlockedAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.IsFullBlocked);
                    break;
                case SortState.IsFullBlockedDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.IsFullBlocked);
                    break;
                case SortState.IsBlockedAsc:
                    dDatas = dDatas.OrderBy(s => s.IsBlocked);
                    break;
                case SortState.IsBlockedDesc:
                    dDatas = dDatas.OrderByDescending(s => s.IsBlocked);
                    break;
                case SortState.IsMarkedAsc:
                    dDatas = dDatas.OrderBy(s => s.ForDeletion);
                    break;
                case SortState.IsMarkedDesc:
                    dDatas = dDatas.OrderByDescending(s => s.ForDeletion);
                    break;
                case SortState.IsFamilyDependentAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.IsDependent);
                    break;
                case SortState.IsFamilyDependentDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.IsDependent);
                    break;
                case SortState.IsOnlineAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.IsOnline);
                    break;
                case SortState.IsOnlineDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.IsOnline);
                    break;
                case SortState.GenderAsc:
                    dDatas = dDatas.OrderBy(s => s!.User!.Gender);
                    break;
                case SortState.GenderDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.User!.Gender);
                    break;
                case SortState.SpecialityAsc:
                    dDatas = dDatas.OrderBy(s => s!.Speciality!.Name);
                    break;
                case SortState.SpecialityDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.Speciality!.Name);
                    break;

                default:
                    dDatas = dDatas.OrderBy(s => s!.User!.Email);
                    break;
            }

            return dDatas;
        }
    }
}
