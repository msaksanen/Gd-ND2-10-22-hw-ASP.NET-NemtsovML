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
        public static HtmlString GeneratePaginator(this IHtmlHelper html, int pageCount, int currentPage, string pageRoute, string processOptions="")
        {
            var sb = new StringBuilder(@"<nav aria-label=""Paginator""> <ul class=""pagination justify-content-center""  style=""line-height:initial; margin-block: auto;"">");
            if (currentPage>1) 
                sb.Append($@" <li class=""page-item""><a class=""page-link"" href=""{pageRoute}{currentPage-1}{processOptions}"">Previous</a></li>");
            else
                sb.Append($@" <li class=""page-item disabled""><a class=""page-link"" href=""{pageRoute}{currentPage}{processOptions}"">Previous</a></li>");

            if (pageCount>1)  
            {
                if (currentPage == pageCount && pageCount>2) ////shows 3 page links when last page is active
                {
                    sb.Append($@" <li class=""page-item""><a class=""page-link"" href=""{pageRoute}{pageCount - 2}{processOptions}"">&nbsp {pageCount - 2} &nbsp</a></li>");
                }
                for (int i = 1; i <= pageCount; i++)
                {
                    if (currentPage == 1 && i == 3)  //shows 3 page links when current page is 1 
                    {
                        sb.Append($@" <li class=""page-item""><a class=""page-link"" href=""{pageRoute}{i}{processOptions}"">&nbsp {i} &nbsp</a></li>");
                    }

                    if (i == currentPage) // active page link
                        sb.Append($@" <li class=""page-item active"" aria-current=""page""><a class=""page-link"" href=""{pageRoute}/{i}{processOptions}"">&nbsp {i} &nbsp</a></li>");
                   else if(i == currentPage+1 || i==currentPage-1) //shows links to next & previous pages
                    {
                        sb.Append($@" <li class=""page-item""><a class=""page-link"" href=""{pageRoute}{i}{processOptions} "">&nbsp {i} &nbsp</a></li>");
                   }
                }
            }

            if (currentPage + 1 <= pageCount)
                sb.Append($@" <li class=""page-item""><a class=""page-link"" href=""{pageRoute}{currentPage+1}{processOptions}"">Next</a></li>");
            else
                sb.Append($@" <li class=""page-item disabled""><a class=""page-link"" href=""{pageRoute}{currentPage}{processOptions}"">Next</a></li>");

            sb.Append("</ul></nav>");

            return new HtmlString(sb.ToString());
        }
    }
}
