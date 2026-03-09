using Microsoft.Extensions.FileProviders;
using Microsoft.SemanticKernel;

namespace AIUtilityLib.Plugins.FileSystem
{
    public class FileSystemPlugin : IDisposable
    {

        PhysicalFileProvider _baseFP;

        public FileSystemPlugin(string rootDirectory)
        {
            if (string.IsNullOrWhiteSpace(rootDirectory) || !Directory.Exists(rootDirectory))
                throw new Exception("Root directory does not exist.");

            this._baseFP = new PhysicalFileProvider(new DirectoryInfo(rootDirectory).FullName);
        }

        public void Dispose()
        {
            _baseFP.Dispose();
            _baseFP = null!;
        }

        #region Utility

        public bool IsRelPathExist(string relativePath) =>
            FP.GetDirectoryContents(relativePath).Exists || FP.GetFileInfo(relativePath).Exists;

        public string? GetRelativePath(string relativePath)
        {
            var fullPath = Path.GetFullPath(Path.Combine(FP.Root, relativePath));
            if (Directory.Exists(fullPath))
                fullPath = new DirectoryInfo(fullPath).FullName;
            else if (File.Exists(fullPath))
                fullPath = new FileInfo(fullPath).FullName;
            else return null;

            if (fullPath.StartsWith(FP.Root, StringComparison.OrdinalIgnoreCase))
                return Path.GetRelativePath(FP.Root, fullPath);
            else return "";
        }

        public string? NormalizeFileRelativePath(string relativePath)
        {
            var fullPath = Path.GetFullPath(Path.Combine(FP.Root, relativePath));
            fullPath = new FileInfo(fullPath).FullName;
            if (fullPath.StartsWith(FP.Root, StringComparison.OrdinalIgnoreCase))
                return Path.GetRelativePath(FP.Root, fullPath);
            else return "";
        }

        #endregion

        #region File / Folder

        public PhysicalFileProvider FP => _baseFP;

        [KernelFunction("get_items")]
        public FSItem[] GetFileSystemContent(string relativeDirectoryPath = "")
        {
            if (!FP.GetDirectoryContents(relativeDirectoryPath).Exists 
                || FP.GetFileInfo(relativeDirectoryPath).Exists)
                return [];

            var root = Path.GetFullPath(Path.Combine(FP.Root, relativeDirectoryPath));

            var dirNames =
                Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories)
                    .Select(p => new DirectoryInfo(p))
                    .Select(d => new FSItem
                    {
                        Name = d.Name,
                        ParentDirectory = d.Parent?.Name,
                        IsDirectory = true,
                        RelativePath = Path.GetRelativePath(root, d.FullName)
                    });

            var fileNames =
                Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
                    .Select(f => new FileInfo(f))
                    .Select(i => new FSItem
                    {
                        Name = i.Name,
                        ParentDirectory = i.Directory?.Name,
                        IsDirectory = false,
                        RelativePath = Path.GetRelativePath(root, i.FullName)
                    });

            return dirNames.Concat(fileNames)
                .OrderBy(f => f.RelativePath)
                .ToArray();
        }

        #endregion

        #region Read / Write

        [KernelFunction("create_item")]
        public bool CreateItem(string itemRelativePath)
        {
            if (IsRelPathExist(itemRelativePath))
                return true;

            var rp = NormalizeFileRelativePath(itemRelativePath);
            if (string.IsNullOrWhiteSpace(rp))
                return false;

            var lstSegment = rp.Split(Path.DirectorySeparatorChar);
            string? subPath = "";
            bool created = false;
            foreach (var seg in lstSegment)
            {
                subPath = Path.Combine(subPath, seg); 
                if (Path.HasExtension(subPath) && !IsRelPathExist(subPath))
                {
                    using var writer = File.CreateText(Path.Combine(FP.Root, subPath));
                    created = true;
                }
                else if (!IsRelPathExist(subPath))
                {
                    Directory.CreateDirectory(Path.Combine(FP.Root, subPath));
                }
                if (!IsRelPathExist(subPath))
                {
                    created = false;
                    break;
                }
            }
            return created;
        }

        [KernelFunction("write_file")]
        public async Task WriteAsync(string fileRelativePath, string content)
        {
            var cont = IsRelPathExist(fileRelativePath) || CreateItem(fileRelativePath);
            if (cont) 
                await File.WriteAllTextAsync(Path.Combine(FP.Root, fileRelativePath), content);
        }

        [KernelFunction("read_file")]
        public async Task<string> ReadAsync(string fileRelativePath) 
        {
            var fi = FP.GetFileInfo(fileRelativePath);
            if (!fi.Exists)
                return "";
            using var reader = new StreamReader(fi.CreateReadStream(), true);
            return await reader.ReadToEndAsync();
        }

        public bool DisableFileOverwrite { get; set; }

        #endregion
    }
}
