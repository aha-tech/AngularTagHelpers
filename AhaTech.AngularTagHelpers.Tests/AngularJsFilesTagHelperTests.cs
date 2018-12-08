using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace AhaTech.AngularTagHelpers.Tests
{
    public class AngularJsFilesTagHelperTests : TagHelperTests<AngularJsFilesTagHelper>
    {
        public AngularJsFilesTagHelperTests()
        {
            TagHelper = new AngularJsFilesTagHelper(CreateHostingEnvironment(), new MemoryCache(new MemoryDistributedCacheOptions()))
            {
                ViewContext = CreateViewContext("")
            };
        }

        [Fact]
        public void SrcAttributeNotSet_UsesRoot()
        {
            CreateFile("main.js");

            var output = ProcessTagHelper();

            Assert.Equal("<script-block><script src=\"/main.js\"></script></script-block>", output);
        }
        [Fact]
        public void SrcAttributeSet_UsesSrc()
        {
            TagHelper.Src = "subDir";

            CreateFile("subDir", "main.js");

            var output = ProcessTagHelper();

            Assert.Equal("<script-block><script src=\"/subDir/main.js\"></script></script-block>", output);
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
        public void RequestBaseSet_AppendsRequestBaseToTheLink()
        {
            TagHelper.ViewContext = CreateViewContext("/subDir");

            CreateFile("main.js");

            var output = ProcessTagHelper();

            Assert.Equal("<script-block><script src=\"/subDir/main.js\"></script></script-block>", output);
        }

        [Fact]
        public void DevModeFiles_AppendsInTheCorrectOrder()
        {
            CreateFile("runtime.js");
            CreateFile("polyfills.js");
            CreateFile("styles.js");
            CreateFile("vendor.js");
            CreateFile("main.js");

            var output = ProcessTagHelper();

            Assert.Equal("<script-block><script src=\"/runtime.js\"></script><script src=\"/polyfills.js\"></script><script src=\"/styles.js\"></script><script src=\"/vendor.js\"></script><script src=\"/main.js\"></script></script-block>", output);
        }
        [Fact]
        public void ProdModeFiles_AppendsInTheCorrectOrder()
        {
            CreateFile("runtime.js");
            CreateFile("polyfills.js");
            CreateFile("styles.css");
            CreateFile("main.js");

            var output = ProcessTagHelper();

            Assert.Equal("<script-block><script src=\"/runtime.js\"></script><script src=\"/polyfills.js\"></script><script src=\"/main.js\"></script></script-block>", output);
        }
        [Fact]
        public void FilesWithHashesAppended_DiscoversTheFiles()
        {
            CreateFile("main.ea8051072bd03d2e9d1f.js");

            var output = ProcessTagHelper();

            Assert.Equal("<script-block><script src=\"/main.ea8051072bd03d2e9d1f.js\"></script></script-block>", output);
        }
        [Fact]
        public void FilesWithoutDots_NoExceptions()
        {
            CreateFile("main.js");
            CreateFile("other");

            var output = ProcessTagHelper();

            Assert.Equal("<script-block><script src=\"/main.js\"></script></script-block>", output);
        }
    }
}