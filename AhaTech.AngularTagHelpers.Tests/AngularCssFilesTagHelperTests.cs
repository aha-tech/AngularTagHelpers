using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace AhaTech.AngularTagHelpers.Tests
{
    public class AngularCssFilesTagHelperTests : TagHelperTests<AngularCssFilesTagHelper>
    {
        public AngularCssFilesTagHelperTests()
        {
            TagHelper = new AngularCssFilesTagHelper(CreateHostingEnvironment(), new MemoryCache(new MemoryDistributedCacheOptions()))
            {
                ViewContext = CreateViewContext("")
            };
        }


        [Fact]
        public void SrcAttributeNotSet_UsesRoot()
        {
            CreateFile("styles.css");

            var output = ProcessTagHelper();

            Assert.Equal("<link rel=\"stylesheet\" href=\"/styles.css\"></link>", output);
        }
        [Fact]
        public void SrcAttributeSet_UsesSrc()
        {
            CreateFile("subDir","styles.css");

            TagHelper.Src = "subDir";

            var output = ProcessTagHelper();

            Assert.Equal("<link rel=\"stylesheet\" href=\"/subDir/styles.css\"></link>", output);
        }
        [Fact]
        public void SrcDirDoesNotExist_NoException()
        {
            // The framework does not throw an exception when the dir does not exist, it just returns empty, so we'll do that too
            TagHelper.Src = "subDir";

            var output = ProcessTagHelper();

            Assert.Equal(string.Empty, output);

        }
        [Fact]
        public void NoFiles_Outputs_Nothing()
        {
            var output = ProcessTagHelper();

            Assert.Equal(string.Empty, output);
        }
        [Fact]
        public void OneCssFile_OutputsLink()
        {
            CreateFile("styles.css");

            var output = ProcessTagHelper();

            Assert.Equal("<link rel=\"stylesheet\" href=\"/styles.css\"></link>", output);
        }
        [Fact]
        public void RequestBaseSet_AppendsRequestBaseToTheLink()
        {
            TagHelper.ViewContext = CreateViewContext("/subDir");

            CreateFile("styles.css");

            var output = ProcessTagHelper();

            Assert.Equal("<link rel=\"stylesheet\" href=\"/subDir/styles.css\"></link>", output);
        }
        [Fact]
        public void StylesheetWithHashAppended_DiscoversTheFile()
        {
            CreateFile("styles.3ff695c00d717f2d2a11.css");

            var output = ProcessTagHelper();

            Assert.Equal("<link rel=\"stylesheet\" href=\"/styles.3ff695c00d717f2d2a11.css\"></link>", output);
        }
        [Fact]
        public void FilesWithoutDots_NoExceptions()
        {
            CreateFile("styles.css");
            CreateFile("other");

            var output = ProcessTagHelper();

            Assert.Equal("<link rel=\"stylesheet\" href=\"/styles.css\"></link>", output);
        }
    }
}
