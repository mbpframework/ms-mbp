using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nitrogen.Core.VirtualFileSystem;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Nitrogen.VirtualFileSystem.Test
{
    public class VirtualFileSystem_Test
    {
        private readonly INgFileService _ngFileService;

        public VirtualFileSystem_Test()
        {
            var services = new ServiceCollection();

            //var hostingEnvironment = Mock.Of<IWebHostEnvironment>();

            ILoggerFactory loggerFactory = LoggerFactory.Create(c =>
            {
                c.AddConsole();
            });

            var virtualFileSystemOptions = new VirtualFileSystemModuleOptions_Test()
            {
                Mimes = new Dictionary<string, string> { { ".ngapp", "application/x-msdownload" }, { ".image", "removed" } }
            };

            Options.Create<VirtualFileSystemModuleOptions_Test>(virtualFileSystemOptions);

            //services.AddSingleton(hostingEnvironment);
            services.AddSingleton(loggerFactory.CreateLogger<NgFileService>());
            services.AddScoped(typeof(INgFileService), typeof(NgFileService));

            var provider = services.BuildServiceProvider();
            _ngFileService = provider.GetService<INgFileService>();
        }

        [Fact]
        public async void NgFileService_CreateFile_Test()
        {
            _ngFileService.CreateFile(AppContext.BaseDirectory + "test.txt");
            File.Exists(AppContext.BaseDirectory + "test.txt").ShouldBeTrue();

            await _ngFileService.CreateFileAsync(AppContext.BaseDirectory + "testasync.txt");
            File.Exists(AppContext.BaseDirectory + "testasync.txt").ShouldBeTrue();
        }

        [Fact]
        public async void NgFileService_DeleteFile_Test()
        {
            _ngFileService.CreateFile(AppContext.BaseDirectory + "test.txt");
            _ngFileService.DeleteFile(AppContext.BaseDirectory + "test.txt");
            File.Exists(AppContext.BaseDirectory + "test.txt").ShouldBeFalse();

            _ngFileService.CreateFile(AppContext.BaseDirectory + "testasync.txt");
            await _ngFileService.DeleteFileAsync(AppContext.BaseDirectory + "testasync.txt");
            File.Exists(AppContext.BaseDirectory + "testasync.txt").ShouldBeFalse();
        }

        [Fact]
        public void NgFileService_GetFileMd5_Test()
        {
            _ngFileService.CreateFile(AppContext.BaseDirectory + "test.txt");

            using (var sw = File.AppendText(AppContext.BaseDirectory + "test.txt"))
            {
                sw.Write("abcdefg");
            }
            _ngFileService.GetFileMd5(AppContext.BaseDirectory + "test.txt").ShouldNotBeEmpty();
            _ngFileService.DeleteFile(AppContext.BaseDirectory + "test.txt");
        }

        [Fact]
        public async void NgFileService_GetFileMd5Async_Test()
        {
            _ngFileService.CreateFile(AppContext.BaseDirectory + "test.txt");

            using (var sw = File.AppendText(AppContext.BaseDirectory + "test.txt"))
            {
                sw.Write("abcdefg");
            }
           (await _ngFileService.GetFileMd5Async(AppContext.BaseDirectory + "test.txt")).ShouldNotBeEmpty();
            _ngFileService.DeleteFile(AppContext.BaseDirectory + "test.txt");
        }

        [Fact]
        public void NgFileService_GetEncoding_Test()
        {
            _ngFileService.CreateFile(AppContext.BaseDirectory + "test.txt");

            using (var sw = File.AppendText(AppContext.BaseDirectory + "test.txt"))
            {
                sw.Write("abcdefg");
            }

            _ngFileService.GetEncoding(AppContext.BaseDirectory + "test.txt").BodyName.ShouldBe("utf-8");

            _ngFileService.DeleteFile(AppContext.BaseDirectory + "test.txt");
        }

        [Fact]
        public void NgFileService_GetEncoding_Fs_Test()
        {
            _ngFileService.CreateFile(AppContext.BaseDirectory + "test.txt");

            using (var sw = File.AppendText(AppContext.BaseDirectory + "test.txt"))
            {
                sw.Write("abcdefg");
            }

            using (var fs = File.OpenRead(AppContext.BaseDirectory + "test.txt"))
            {
                _ngFileService.GetEncoding(fs).BodyName.ShouldBe("utf-8");
            }

            _ngFileService.DeleteFile(AppContext.BaseDirectory + "test.txt");
        }

        [Fact]
        public async void NgFileService_GetEncodingAsync_Test()
        {
            _ngFileService.CreateFile(AppContext.BaseDirectory + "test.txt");

            using (var sw = File.AppendText(AppContext.BaseDirectory + "test.txt"))
            {
                sw.Write("abcdefg");
            }

          (await _ngFileService.GetEncodingAsync(AppContext.BaseDirectory + "test.txt")).BodyName.ShouldBe("utf-8");

            _ngFileService.DeleteFile(AppContext.BaseDirectory + "test.txt");
        }

        [Fact]
        public async void NgFileService_GetEncodingAsync_Fs_Test()
        {
            _ngFileService.CreateFile(AppContext.BaseDirectory + "test.txt");

            using (var sw = File.AppendText(AppContext.BaseDirectory + "test.txt"))
            {
                sw.Write("abcdefg");
            }

            using (var fs = File.OpenRead(AppContext.BaseDirectory + "test.txt"))
            {
                (await _ngFileService.GetEncodingAsync(fs)).BodyName.ShouldBe("utf-8");
            }

            _ngFileService.DeleteFile(AppContext.BaseDirectory + "test.txt");
        }

        [Fact]
        public void NgFileService_CreateDirectory_Test()
        {
            _ngFileService.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "test"));
            Directory.Exists(Path.Combine(AppContext.BaseDirectory, "test")).ShouldBeTrue();
            _ngFileService.Delete(Path.Combine(AppContext.BaseDirectory, "test"));
        }

        [Fact]
        public async void NgFileService_CreateDirectoryAsync_Test()
        {
            await _ngFileService.CreateDirectoryAsync(Path.Combine(AppContext.BaseDirectory, "test"));
            Directory.Exists(Path.Combine(AppContext.BaseDirectory, "test")).ShouldBeTrue();
            await _ngFileService.DeleteAsync(Path.Combine(AppContext.BaseDirectory, "test"));
            Directory.Exists(Path.Combine(AppContext.BaseDirectory, "test")).ShouldBeFalse();
        }

        [Fact]
        public void NgFileService_Copy_Test()
        {
            // 准备文件夹
            _ngFileService.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "SourceFile_Test"));
            _ngFileService.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "DestinationFile_Test"));

            // 准备文件
            _ngFileService.CreateFile(Path.Combine(Path.Combine(AppContext.BaseDirectory, "SourceFile_Test"), "a.txt"));

            // 执行
            _ngFileService.Copy(Path.Combine(AppContext.BaseDirectory, "SourceFile_Test"), Path.Combine(AppContext.BaseDirectory, "DestinationFile_Test"));

            // 判断
            File.Exists(Path.Combine(Path.Combine(AppContext.BaseDirectory, "DestinationFile_Test"), "a.txt")).ShouldBeTrue();

            _ngFileService.Delete(Path.Combine(AppContext.BaseDirectory, "SourceFile_Test"));
            _ngFileService.Delete(Path.Combine(AppContext.BaseDirectory, "DestinationFile_Test"));
        }

        [Fact]
        public async void NgFileService_CopyAsync_Test()
        {
            // 准备文件夹
            _ngFileService.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "SourceFile_Test"));
            _ngFileService.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "DestinationFile_Test"));

            // 准备文件
            await _ngFileService.CreateFileAsync(Path.Combine(Path.Combine(AppContext.BaseDirectory, "SourceFile_Test"), "a.txt"));

            // 执行
            await _ngFileService.CopyAsync(Path.Combine(AppContext.BaseDirectory, "SourceFile_Test"), Path.Combine(AppContext.BaseDirectory, "DestinationFile_Test"));

            // 判断
            File.Exists(Path.Combine(Path.Combine(AppContext.BaseDirectory, "DestinationFile_Test"), "a.txt")).ShouldBeTrue();

            await _ngFileService.DeleteAsync(Path.Combine(AppContext.BaseDirectory, "SourceFile_Test"));
            await _ngFileService.DeleteAsync(Path.Combine(AppContext.BaseDirectory, "DestinationFile_Test"));
        }
    }

    public class VirtualFileSystemModuleOptions_Test
    {
        public Dictionary<string, string> Mimes { get; set; }
    }
}
