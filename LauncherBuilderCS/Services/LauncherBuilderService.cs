using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using LauncherCS;

namespace LauncherBuilderCS.Services
{
    internal sealed class LauncherBuilderService
    {
        private const string BaseExecutableResource = "LauncherBuilderCS.Resources.BaseLauncher.bin.gz.b64";
        public const uint FooterMagic = 0x464C434D; // "MLCF"

        private static readonly byte[] FooterMagicBytes = Encoding.ASCII.GetBytes("MLCF");
        private readonly byte[] _baseExecutable;
        private readonly OptionXmlBuilder _xmlBuilder = new();

        public LauncherBuilderService()
        {
            _baseExecutable = LoadBaseExecutable();
        }

        public void BuildExecutable(string outputPath, LauncherConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException("Output path must not be empty.", nameof(outputPath));
            }

            var directory = Path.GetDirectoryName(Path.GetFullPath(outputPath));
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var xml = _xmlBuilder.Build(configuration);
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var xmlBytes = encoding.GetBytes(xml);
            var compressed = ResourceEncoder.Compress(xmlBytes);
            ResourceEncoder.ApplyXor(compressed);

            using var output = File.Create(outputPath);
            output.Write(_baseExecutable, 0, _baseExecutable.Length);
            output.Write(compressed, 0, compressed.Length);
            output.Write(BitConverter.GetBytes(compressed.Length));
            output.Write(FooterMagicBytes, 0, FooterMagicBytes.Length);
        }

        public void SaveOptionsXml(string path, LauncherConfiguration configuration)
        {
            var xml = _xmlBuilder.Build(configuration);
            var encoding = Encoding.GetEncoding("iso-8859-1");
            File.WriteAllText(path, xml, encoding);
        }

        private static byte[] LoadBaseExecutable()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(BaseExecutableResource);
            if (stream == null)
            {
                throw new InvalidOperationException("Base launcher executable resource not found.");
            }

            using var reader = new StreamReader(stream, Encoding.ASCII);
            var base64 = reader.ReadToEnd().Trim();
            var compressed = Convert.FromBase64String(base64);

            using var compressedStream = new MemoryStream(compressed);
            using var gzip = new GZipStream(compressedStream, CompressionMode.Decompress);
            using var buffer = new MemoryStream();
            gzip.CopyTo(buffer);
            return buffer.ToArray();
        }
    }
}
