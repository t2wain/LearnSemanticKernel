using System.Management;

namespace TestAI
{
    public class FileSystemTest : IClassFixture<Context>
    {
        Context _context;

        public FileSystemTest(Context context)
        {
            this._context = context;
        }

        Context Context => _context;

        [Fact]
        public void GetFileItems()
        {
            var items = Context.FileSystem.GetFileSystemContent();
            Assert.True(items.Length > 0);

            items = Context.FileSystem.GetFileSystemContent("ChatPlugin");
            Assert.True(items.Length > 0);

            var relPath = @"ChatPlugin\Chat\..\ChatFilter\..\";
            var rp = Context.FileSystem.GetRelativePath(relPath)!;
            items = Context.FileSystem.GetFileSystemContent(rp);
            Assert.True(items.Length > 0);

        }

        [Fact]
        public void GetFileItemsFail()
        {

            var relPath = @".\..\..\";
            var items = Context.FileSystem.GetFileSystemContent(relPath);
            Assert.True(items.Length == 0);

            var rp = Context.FileSystem.GetRelativePath(relPath);

            items = Context.FileSystem.GetFileSystemContent(@"CalendarPlugin\AssistantShowCalendarEvents\config.json");
            Assert.True(items.Length == 0);
        }

        [Fact]
        public void NormalizeRelPath()
        {
            var relPath = @"\\ZZTemp\Folder1\..\Folder2\..\..\ZTemp\Folder1\Test.txt";
            var nRelPath = Context.FileSystem.NormalizeFileRelativePath(relPath);
            Assert.True(string.IsNullOrWhiteSpace(nRelPath));

            relPath = Context.FileSystem.FP.Root + @"ZZTemp\Folder1\..\Folder2\..\..\..\ZTemp\Folder1\Test.txt";
            nRelPath = Context.FileSystem.NormalizeFileRelativePath(relPath);
            Assert.True(string.IsNullOrWhiteSpace(nRelPath));


            relPath = Context.FileSystem.FP.Root + @"ZZTemp\Folder1\..\Folder2\..\ZTemp\Folder1\Test.txt";
            nRelPath = Context.FileSystem.NormalizeFileRelativePath(relPath);
            Assert.False(string.IsNullOrWhiteSpace(nRelPath));

        }

        [Fact]
        public void CreateFile()
        {
            var relPath = @"ZZTemp\Folder1\..\Folder2\..\Folder1\Test.txt";
            var nRelPath = Context.FileSystem.NormalizeFileRelativePath(relPath);
            if (!string.IsNullOrWhiteSpace(nRelPath))
            {
                var res = Context.FileSystem.CreateItem(nRelPath);
                Assert.True(res);
            }
            else Assert.Fail();
        }

        [Fact]
        public async Task WriteFile()
        {
            var relPath = @"ZZTemp\Folder1\..\Folder2\..\Folder1\Test.txt";
            var nRelPath = Context.FileSystem.NormalizeFileRelativePath(relPath);
            if (!string.IsNullOrWhiteSpace(nRelPath))
            {
                string content = "This is a test";
                await Context.FileSystem.WriteAsync(nRelPath, content);
                var text = await Context.FileSystem.ReadAsync(nRelPath);
                Assert.True(string.Compare(content, text) == 0);
            }
            else Assert.Fail();
        }
    }
}
