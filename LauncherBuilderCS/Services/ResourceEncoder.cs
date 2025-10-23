using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace LauncherBuilderCS.Services
{
    internal static class ResourceEncoder
    {
        private static readonly byte[] XorCodes = { 0x53, 0x6B, 0x79, 0x54, 0x65, 0x61, 0x6D }; // "SkyTeam"

        public static string CreateImageDataFromFile(string path)
        {
            using var stream = File.OpenRead(path);
            return CreateImageDataFromStream(stream);
        }

        public static string CreateImageDataFromStream(Stream stream)
        {
            using var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            return CreateImageData(buffer.ToArray());
        }

        public static string CreateImageData(ReadOnlySpan<byte> data)
        {
            var compressed = Compress(data);
            return Convert.ToBase64String(compressed);
        }

        public static byte[] Compress(ReadOnlySpan<byte> data)
        {
            using var output = new MemoryStream();
            using (var zlib = new ZLibStream(output, CompressionLevel.SmallestSize, leaveOpen: true))
            {
                zlib.Write(data);
            }

            return output.ToArray();
        }

        public static byte[] CompressString(string value, Encoding encoding)
        {
            var bytes = encoding.GetBytes(value);
            return Compress(bytes);
        }

        public static void ApplyXor(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= XorCodes[i % XorCodes.Length];
            }
        }

        public static ReadOnlySpan<byte> GetXorCodes() => XorCodes;
    }
}
