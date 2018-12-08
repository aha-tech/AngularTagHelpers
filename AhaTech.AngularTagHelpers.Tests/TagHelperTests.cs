using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Internal;
using Microsoft.Extensions.Primitives;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AhaTech.AngularTagHelpers.Tests
{
    public abstract class TagHelperTests<TTagHelper> where TTagHelper : TagHelper
    {
        private readonly TagHelperAttributeList _inputTagAttributes;
        protected TTagHelper TagHelper;
        private TestFileProvider _fileProvider;

        protected TagHelperTests()
        {
            _inputTagAttributes = new TagHelperAttributeList();
        }
        protected ViewContext CreateViewContext(string requestPathBase) => new ViewContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    PathBase = new PathString(requestPathBase)
                }
            }
        };
        protected void CreateFile(string fileName) => _fileProvider.AddFile(fileName);

        protected void CreateFile(string dirName, string fileName) => _fileProvider.AddFile(dirName, fileName);

        protected IHostingEnvironment CreateHostingEnvironment()
        {
            _fileProvider = new TestFileProvider();
            return new TestHostingEnvironment
            {
                WebRootFileProvider = _fileProvider
            };
        }

        protected string ProcessTagHelper()
        {
            var ctx = new TagHelperContext(_inputTagAttributes, new Dictionary<object, object>(), "uniqueId");
            var output = new TagHelperOutput("angular-css-files", new TagHelperAttributeList(), (result, encoder) => Task.FromResult(new DefaultTagHelperContent().SetContent(string.Empty)));
            TagHelper.Process(ctx, output);

            var writer = new StringWriter();

            output.WriteTo(writer, HtmlEncoder.Default);

            return writer.ToString();
        }
    }

    /// <summary>
    /// A very hacky mock implementation of an in-memory file provider
    /// </summary>
    internal class TestFileProvider : IFileProvider
    {
        private readonly List<string> _directContents = new List<string>();
        private readonly List<string> _subDirContents = new List<string>();
        private string _subDir;

        public void AddFile(string fileName)
        {
            _directContents.Add(fileName);
        }
        public void AddFile(string dirName, string fileName)
        {
            if (_subDir != null) throw new InvalidOperationException();
            _subDir = dirName;
            _subDirContents.Add(fileName);
        }
        public IFileInfo GetFileInfo(string subpath)
        {
            throw new NotImplementedException();
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (subpath == "")
            {
                return new DirectoryContentsWithSubdir(_directContents, _subDir, _subDirContents);
            }
            else if (subpath == _subDir)
            {
                return new DirectoryContents(_subDirContents);
            }

            throw new InvalidOperationException();
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }
        internal class DirectoryContentsWithSubdir : IDirectoryContents
        {
            private readonly List<IFileInfo> _directContents;

            public DirectoryContentsWithSubdir(List<string> directContents, string subDir, List<string> subDirContents)
            {
                _directContents = new List<IFileInfo>(directContents.Select(f => new FileInfo(f)))
            {
                new DirectoryInfo(subDir)
            };
            }

            public IEnumerator<IFileInfo> GetEnumerator()
            {
                return _directContents.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool Exists { get; }
        }

        internal class DirectoryContents : IDirectoryContents
        {
            private readonly List<string> _subDirContents;

            public DirectoryContents(List<string> subDirContents)
            {
                _subDirContents = subDirContents;
            }

            public IEnumerator<IFileInfo> GetEnumerator()
            {
                return _subDirContents.Select(f => new FileInfo(f)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public bool Exists => true;
        }

        internal class FileInfo : IFileInfo
        {
            public FileInfo(string fileName)
            {
                Name = fileName;
            }

            public Stream CreateReadStream()
            {
                throw new NotImplementedException();
            }

            public bool Exists => true;
            public long Length => 0;
            public string PhysicalPath => Name;
            public string Name { get; }
            public DateTimeOffset LastModified => DateTimeOffset.Now;
            public bool IsDirectory => false;
        }

        internal class DirectoryInfo : IFileInfo
        {
            public DirectoryInfo(string fileName)
            {
                Name = fileName;
            }

            public Stream CreateReadStream()
            {
                throw new NotImplementedException();
            }

            public bool Exists => true;
            public long Length => 0;
            public string PhysicalPath => Name;
            public string Name { get; }
            public DateTimeOffset LastModified => DateTimeOffset.Now;
            public bool IsDirectory => true;
        }
    }
}