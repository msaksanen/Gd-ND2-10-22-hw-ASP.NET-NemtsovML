using AutoMapper;
using MedContactCore;
using MedContactCore.Abstractions;
using MedContactCore.DataTransferObjects;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MedContactApp.Helpers
{
    public  class PatchMaker<T> where T : class
    {
        public List<PatchModel> Make (T newObj, T source)
        {
            var patchList = new List<PatchModel>();
            Type myType = typeof(T);
            var propList = myType?.GetProperties();
            if (propList != null)
            {
                foreach (var prop in propList)
                {
                    var propName = myType?.GetProperty($"{prop.Name}");
                    var propValueModel = propName?.GetValue(newObj);
                    var propValueSource = propName?.GetValue(source);
                    if (propValueSource?.GetHashCode() != propValueModel?.GetHashCode() && propValueModel != null)
                    {
                        PatchModel patchModel = new() { PropertyName = prop.Name, PropertyValue = propValueModel };
                        patchList.Add(patchModel);
                    }
                } 
            }
            return patchList;
        }
    }
}
