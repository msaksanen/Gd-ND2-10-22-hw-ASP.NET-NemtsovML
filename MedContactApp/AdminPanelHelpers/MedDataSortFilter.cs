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
    public class MedDataSortFilter
    {
        internal IQueryable<MedData> MedFilter(IQueryable<MedData> data, string type, string speciality, 
            string name, string date, string depart, string text)
        {
            if (!string.IsNullOrEmpty(date))
            {
                var resTime = DateTime.TryParse(date, out DateTime sDate);
                if (resTime)
                {
                    data = data.Where(d => d.InputDate.Date.Equals(sDate.Date));
                }
            }
            if (!string.IsNullOrEmpty(type))
            {

                data = data.Where(p => p.Type != null && p.Type.Contains(type));
            }
            if (!string.IsNullOrEmpty(depart))
            {

                data = data.Where(p => p.Department!= null && p.Department.Contains(depart));
            }
            if (!string.IsNullOrEmpty(text))
            {
                data = data.Where(p => (p.TextData != null && p.TextData.Contains(text)) ||
                                       (p.ShortSummary != null && p.ShortSummary.Contains(text)));
            }
            if (!string.IsNullOrEmpty(speciality))
            {

                data = data.Where(p => p.DoctorData!= null && p.DoctorData.Speciality!= null && p.DoctorData.Speciality.Name!= null &&
                                       p.DoctorData.Speciality.Name.Contains(speciality));
            }
            if (!string.IsNullOrEmpty(name))
            {

                data = data.Where(p => p.DoctorData != null && p.DoctorData.User!=null && 
                                       (p.DoctorData.User.Name!.Contains(name) ||
                                        p.DoctorData.User.MidName!.Contains(name) ||
                                        p.DoctorData.User.Surname!.Contains(name)));
            }
            return data;
        }
        internal IQueryable<MedData> MedDataSort(IQueryable<MedData> dDatas, SortState sortOrder)
        {
            switch (sortOrder)
            {
                case SortState.DateAsc:
                    dDatas = dDatas.OrderBy(s => s.InputDate);
                    break;
                case SortState.DepartAsc:
                    dDatas = dDatas.OrderBy(s => s.Department);
                    break;
                case SortState.DepartDesc:
                    dDatas = dDatas.OrderByDescending(s => s.Department);
                    break;
                case SortState.TypeAsc:
                    dDatas = dDatas.OrderBy(s => s.Type);
                    break;
                case SortState.TypeDesc:
                    dDatas = dDatas.OrderByDescending(s => s.Type);
                    break;
                case SortState.NameAsc:
                    dDatas = dDatas.OrderBy(s => s!.DoctorData!.User!.Name);
                    break;
                case SortState.NameDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.DoctorData!.User!.Name);
                    break;
                case SortState.SurnameAsc:
                    dDatas = dDatas.OrderBy(s => s!.DoctorData!.User!.Surname);
                    break;
                case SortState.SurnameDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.DoctorData!.User!.Surname);
                    break;    
                case SortState.SpecialityAsc:
                    dDatas = dDatas.OrderBy(s => s!.DoctorData!.Speciality!.Name);
                    break;
                case SortState.SpecialityDesc:
                    dDatas = dDatas.OrderByDescending(s => s!.DoctorData!.Speciality!.Name);
                    break;
                case SortState.TextAsc:
                    dDatas = dDatas.OrderBy(s => s.ShortSummary);
                    break;
                case SortState.TextDesc:
                    dDatas = dDatas.OrderByDescending(s => s.ShortSummary);
                    break;
               

                default:
                    dDatas = dDatas.OrderByDescending(s => s.InputDate);
                    break;
            }

            return dDatas;
        }
    }
}

