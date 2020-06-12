﻿using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;
using System.Threading.Tasks;

namespace localization.Localization
{
    [HtmlTargetElement("form", Attributes = CultureAttributeName)]
    public class CultureFormLinkTagHelper : TagHelper
    {
        private const string CultureAttributeName = "cms-culture";
        /// <summary>
        /// The culture to use attribute.
        /// </summary>        
        [HtmlAttributeName(CultureAttributeName)]
        public string Culture { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {                      
            if (string.IsNullOrEmpty(Culture))
            {
                Culture = CultureInfo.CurrentCulture.Name;
            }

            LocalizationUrlResult urlResult = LocalizationTagHelperUtility.GetUrlResult(context, Culture);

            output.Attributes.SetAttribute("action", urlResult.Url);

            return Task.FromResult(0);
        }        
    }
}
