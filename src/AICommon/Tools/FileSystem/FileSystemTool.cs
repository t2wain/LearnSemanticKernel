using Microsoft.Extensions.FileProviders;
using System.ComponentModel;

namespace AICommon.Plugins.FileSystem
{
    /// <summary>
    /// Provide methods to manage the local directories
    /// and files under a specified root directory. It will
    /// prevent all actions outside of the specified
    /// </summary>
    public class FileSystemTool : IDisposable
    {

        PhysicalFileProvider _baseFP;

        public FileSystemTool(string rootDirectory)
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

        public string GetRelativePath(string relativePath)
        {
            var fullPath = Path.GetFullPath(Path.Combine(FP.Root, relativePath));
            if (Directory.Exists(fullPath))
                fullPath = new DirectoryInfo(fullPath).FullName;
            else if (File.Exists(fullPath))
                fullPath = new FileInfo(fullPath).FullName;
            else return "";

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

        [Description("""
            List directories and their nested subdirectories/files 
            starting at the given relative directory path.
            """)]
        public FSItem[] ListFileSystemItem(string relativeDirectoryPath = "", 
            bool includeDirectory = true, 
            bool includeFile = true,
            bool includeNestedSubdirectories = true)
        {
            if (!FP.GetDirectoryContents(relativeDirectoryPath).Exists)
                return [];

            var relPath = relativeDirectoryPath;
            if (!string.IsNullOrWhiteSpace(relativeDirectoryPath))
            {
                relPath = GetRelativePath(relativeDirectoryPath);
                if (string.IsNullOrWhiteSpace(relPath))
                    return [];
            }

            var root = Path.GetFullPath(Path.Combine(FP.Root, relPath));

            SearchOption searchOption = includeNestedSubdirectories ? 
                SearchOption.AllDirectories : 
                SearchOption.TopDirectoryOnly;

            IEnumerable<FSItem> dirNames = [];
            if (includeDirectory)
            {
                dirNames =
                    Directory.EnumerateDirectories(root, "*", searchOption)
                        .Select(p => new DirectoryInfo(p))
                        .Select(d => new FSItem
                        {
                            Name = d.Name,
                            ParentDirectory = d.Parent?.Name,
                            IsDirectory = true,
                            RelativePath = Path.GetRelativePath(root, d.FullName)
                        });
            }

            IEnumerable<FSItem> fileNames = [];
            if (includeFile)
            {
                fileNames =
                    Directory.EnumerateFiles(root, "*", searchOption)
                        .Select(f => new FileInfo(f))
                        .Select(i => new FSItem
                        {
                            Name = i.Name,
                            ParentDirectory = i.Directory?.Name,
                            IsDirectory = false,
                            RelativePath = Path.GetRelativePath(root, i.FullName)
                        });
            }

            return dirNames.Concat(fileNames)
                .OrderBy(f => f.RelativePath)
                .ToArray();
        }

        #endregion

        #region Read / Write

        [Description("""
            Create files and/or directories. File path has a file extension.
            Directory path don't have a file extension. All parent directories 
            in the item relative path will also be created if not exist.
            Return the relative paths of only successfully created items.
            """)]
        public string[] CreateItems(string[] itemRelativePaths)
        {
            List<string> fileCreated = new();
            foreach (var p in itemRelativePaths)
            {
                if (CreateItem(p))
                    fileCreated.Add(p);
            }
            return fileCreated.ToArray();
        }

        [Description("""
            Create file or directory. All parent directories 
            in the item relative path will also be created 
            if not exist.
            """)]
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

        [Description("""
            Write text content to multiple files. Files will be created 
            if not exist. All parent directories in the file 
            relative path will be created if not exist.)]
            """)]
        public async Task WriteFilesAsync(FileContent[] fileContents)
        {
            foreach (var file in fileContents)
            {
                await WriteAsync(file.FileName, file.TextContent);
            }
        }

        [Description("""
            Write text content to the file. File will be created 
            if not exist. All parent directories in the file 
            relative path will be created if not exist.)]
            """)]
        public async Task WriteAsync(string fileRelativePath, string textConent)
        {
            var cont = IsRelPathExist(fileRelativePath) || CreateItem(fileRelativePath);
            if (cont) 
                await File.WriteAllTextAsync(Path.Combine(FP.Root, fileRelativePath), textConent);
        }

        [Description("Read and return the entire text content of multiple files")]
        public async Task<FileContent[]> ReadFilesAsync(string[] fileRelativePaths)
        {
            List<FileContent> files = new();
            foreach(var fn in fileRelativePaths)
            {
                var fi = FP.GetFileInfo(fn);
                if (!fi.Exists)
                    continue;
                using (var reader = new StreamReader(fi.CreateReadStream(), true)) 
                { 
                    files.Add(new()
                    {
                        FileName = fi.Name,
                        RelativePath = GetRelativePath(fn),
                        TextContent = await reader.ReadToEndAsync()
                    });
                }
            }
            return files.ToArray();
        }

        [Description("Read and return the entire text content of the file")]
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
