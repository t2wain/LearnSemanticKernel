using System.Text.Json;
using SK = AIUtilityLib.Plugins.FileSystem;

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
            var items = Context.FileSystem.ListFileSystemItem();
            Assert.True(items.Length > 0);

            items = Context.FileSystem.ListFileSystemItem("ChatPlugin");
            Assert.True(items.Length > 0);

            string s = JsonSerializer.Serialize(items);
            var d = JsonSerializer.Deserialize<SK.FSItem[]>(s)!;
            Assert.True(d.Length > 0);

            var relPath = @"ChatPlugin\Chat\..\ChatFilter\..\";
            items = Context.FileSystem.ListFileSystemItem(relPath);
            Assert.True(items.Length > 0);

            s = JsonSerializer.Serialize(items);
            d = JsonSerializer.Deserialize<SK.FSItem[]>(s)!;
            Assert.True(d.Length > 0);

        }

        [Fact]
        public void GetFileItemsFail()
        {

            var relPath = @".\..\..\";
            var items = Context.FileSystem.ListFileSystemItem(relPath);
            Assert.True(items.Length == 0);

            var rp = Context.FileSystem.GetRelativePath(relPath);

            items = Context.FileSystem.ListFileSystemItem(@"CalendarPlugin\AssistantShowCalendarEvents\config.json");
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
