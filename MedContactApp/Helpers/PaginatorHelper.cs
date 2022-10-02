using System;
using System.Text;
using MedContactDb.Entities;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MedContactApp.Helpers
{
    public static class PaginatorHelper
    {
        public static HtmlString GeneratePagintator(this IHtmlHelper html, int pageCount, string pageRoute)
        {
            var sb = new StringBuilder(@"<Table class=""table table-bordered""><tr>");

            if (pageCount>1)
            {
                for (int i = 1; i <= pageCount; i++)
                {
                    sb.Append(@$"<th><a href=""{pageRoute}/{i - 1}"" class=""text-decoration-none"">Page &nbsp {i} &nbsp</a></th>");
                }
            }
            sb.Append("</tr></Table>");

            return new HtmlString(sb.ToString());
        }
    }
}
