﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;

namespace AhaTech.AngularTagHelpers
{
    [HtmlTargetElement("angular-js-files", Attributes = "src")]
    public class AngularJsFilesTagHelper : TagHelper
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMemoryCache _cache;

        public AngularJsFilesTagHelper(IHostingEnvironment hostingEnvironment, IMemoryCache cache)
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

            var files = urlBuilder.BuildUrlList(null, Src + "/*.js", null);

            files = OrderFiles(files).ToList();
            if (!files.Any())
            {
                output.SuppressOutput();
                return;
            }

            output.TagName = "script-block";
            output.Attributes.Clear();

            var extraTags = output.PostContent;

            foreach (var file in files)
            {
                extraTags.AppendFormat("<script src=\"{0}\"></script>", file);
            }
        }

        private IEnumerable<string> OrderFiles(IEnumerable<string> files)
        {
            var dictionary = files.ToDictionary(f =>
            {
                var name = Path.GetFileName(f);
                var index = name.IndexOf(".", StringComparison.OrdinalIgnoreCase);
                index = index > 0 ? index : name.Length;
                return name.Substring(0, index).ToLowerInvariant();
            });

            // ng build
            // ng build --prod
            if (dictionary.TryGetValue("runtime", out var runtime))
            {
                yield return runtime;
            }
            // ng build
            // ng build --prod
            if (dictionary.TryGetValue("polyfills", out var polyfills))
            {
                yield return polyfills;
            }
            // ng build
            if (dictionary.TryGetValue("styles", out var styles))
            {
                yield return styles;
            }
            // ng build
            if (dictionary.TryGetValue("vendor", out var vendor))
            {
                yield return vendor;
            }
            // ng build
            // ng build --prod
            if (dictionary.TryGetValue("main", out var main))
            {
                yield return main;
            }
        }
    }
}