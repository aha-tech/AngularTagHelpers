using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace AhaTech.AngularTagHelpers
{
    [HtmlTargetElement("angular-css-files", Attributes = "src")]
    public class AngularCssFilesTagHelper : TagHelper
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMemoryCache _cache;

        public AngularCssFilesTagHelper(IHostingEnvironment hostingEnvironment, IMemoryCache cache)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
        }

        [HtmlAttributeName("src")]
        public string Src { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var urlBuilder = new GlobbingUrlBuilder(_hostingEnvironment.WebRootFileProvider, _cache,
                ViewContext.HttpContext.Request.PathBase);

            var files = urlBuilder.BuildUrlList(null, Src + "/*.css", "");

            if (files.Count > 1)
            {
                throw new ArgumentException($"Expected only one CSS file, found {files.Count}: {string.Join(", ", files)}");
            }

            if (!files.Any())
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "link";
            output.Attributes.Clear();
            output.Attributes.Add("rel", "stylesheet");
            output.Attributes.Add("href", files.First());

        }
    }
}
