using MedContactApp.FilterSortHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using MedContactDb.Entities;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace MedContactApp.FilterSortPageHelpers
{
    public class SortHeaderTagHelper : TagHelper
    {
        public SortState Property { get; set; } 
        public SortState Current { get; set; }  
        public string? Action { get; set; }
        public bool Up { get; set; }   

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = null!;

        IUrlHelperFactory urlHelperFactory;
        public SortHeaderTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        [HtmlAttributeName(DictionaryAttributePrefix = "opt-url-")]
        public Dictionary<string, object> OptUrlValues { get; set; } = new();
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            output.TagName = "a";
            //string? url = urlHelper.Action(Action, new { sortOrder = Property});
            //output.Attributes.SetAttribute("href", url);


            OptUrlValues.Add("sortOrder", Property);
            output.Attributes.SetAttribute("href" ,urlHelper.Action(Action, OptUrlValues));

            if (Current == Property)
            {
                TagBuilder tag = new TagBuilder("i");
                if (Up == true)   
                    tag.AddCssClass("bi bi-chevron-double-up");
                else   
                    tag.AddCssClass("bi bi-chevron-double-down");

                output.PreContent.AppendHtml(tag);
            }
        }
    }
}
