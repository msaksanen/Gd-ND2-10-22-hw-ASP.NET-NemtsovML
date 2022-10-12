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
        public static HtmlString GeneratePagintator(this IHtmlHelper html, int pageCount, int currentPage, string pageRoute)
        {
            var sb = new StringBuilder(@"<nav aria-label=""Paginator""> <ul class=""pagination"">");
            if (currentPage>0)
                sb.Append($@" <li class=""page-item""><a class=""page-link"" href=""{pageRoute}/{currentPage-1}"">Previous</a></li>");
            else
                sb.Append($@" <li class=""page-item disabled""><a class=""page-link"" href=""{pageRoute}/{currentPage}"">Previous</a></li>");

            if (pageCount>1)
            {
                for (int i = 1; i <= pageCount; i++)
                {
                    if (i == currentPage + 1)
                        sb.Append($@" <li class=""page-item active"" aria-current=""page""><a class=""page-link"" href=""{pageRoute}/{i - 1}"">Page &nbsp {i} &nbsp</a></li>");
                    else
                        sb.Append($@" <li class=""page-item""><a class=""page-link"" href=""{pageRoute}/{i - 1}"">Page &nbsp {i} &nbsp</a></li>");
                }
            }

            if (currentPage + 1 < pageCount)
                sb.Append($@" <li class=""page-item""><a class=""page-link"" href=""{pageRoute}/{currentPage+1}"">Next</a></li>");
            else
                sb.Append($@" <li class=""page-item disabled""><a class=""page-link"" href=""{pageRoute}/{currentPage}"">Next</a></li>");

            sb.Append("</ul></nav>");

            return new HtmlString(sb.ToString());
        }
    }
}
