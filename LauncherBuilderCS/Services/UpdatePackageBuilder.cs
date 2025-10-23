using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using LauncherBuilderCS.Controls;

namespace LauncherBuilderCS.Services
{
    internal sealed class UpdatePackageBuilder
    {
        public void Build(string inputDirectory, string outputDirectory, IProgress<UpdateProgress>? progress = null)
        {
            if (string.IsNullOrWhiteSpace(inputDirectory) || !Directory.Exists(inputDirectory))
            {
                throw new DirectoryNotFoundException($"Input directory '{inputDirectory}' was not found.");
            }

            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                throw new ArgumentException("Output directory must be provided.", nameof(outputDirectory));
            }

            Directory.CreateDirectory(outputDirectory);

            var files = Directory.GetFiles(inputDirectory, "*", SearchOption.AllDirectories);
            var entries = new StringBuilder();

            using var md5 = MD5.Create();
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                var relative = Path.GetRelativePath(inputDirectory, file);
                var normalized = relative.Replace('/', '\\');
                var lower = normalized.ToLowerInvariant();

                var data = File.ReadAllBytes(file);
                var base64 = Convert.ToBase64String(data);
                var base64Bytes = Encoding.ASCII.GetBytes(base64);

                var relativeOutput = lower.Replace('\\', Path.DirectorySeparatorChar);
                var outputFilePath = Path.Combine(outputDirectory, "udata", relativeOutput) + ".pak";
                var outputFolder = Path.GetDirectoryName(outputFilePath);
                if (!string.IsNullOrEmpty(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                using (var stream = File.Create(outputFilePath))
                using (var zlib = new ZLibStream(stream, CompressionLevel.SmallestSize, leaveOpen: true))
                {
                    zlib.Write(base64Bytes, 0, base64Bytes.Length);
                }

                var hashBytes = md5.ComputeHash(data);
                var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLowerInvariant();

                entries.Append(Quote(lower)).Append(',').Append(Quote(hash)).Append("\r\n");

                progress?.Report(new UpdateProgress(i + 1, files.Length, normalized));
            }

            var updateBytes = Encoding.ASCII.GetBytes(entries.ToString());
            var compressedUpdate = ResourceEncoder.Compress(updateBytes);
            ResourceEncoder.ApplyXor(compressedUpdate);

            var updateConfigPath = Path.Combine(outputDirectory, "update.cfg");
            File.WriteAllBytes(updateConfigPath, compressedUpdate);
        }

        private static string Quote(string value) => "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}
