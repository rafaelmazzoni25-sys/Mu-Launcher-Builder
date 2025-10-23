using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;

namespace LauncherCS
{
    public static class ResourceDecoder
    {
        private static readonly byte[] XorCodes = { 0x53, 0x6B, 0x79, 0x54, 0x65, 0x61, 0x6D }; // "SkyTeam"

        public static string BuildUrl(string updateUrl, string fileName)
        {
            var normalized = fileName.Replace('\\', '/');
            return updateUrl + normalized.ToLowerInvariant();
        }

        public static void EncryptDecrypt(Stream stream)
        {
            if (!stream.CanSeek || !stream.CanRead || !stream.CanWrite)
            {
                throw new InvalidOperationException("Stream must be seekable and writable.");
            }

            long originalPosition = stream.Position;
            stream.Position = 0;
            var buffer = new byte[1];
            long index = 0;
            while (stream.Read(buffer, 0, 1) == 1)
            {
                buffer[0] ^= XorCodes[index % XorCodes.Length];
                stream.Position--;
                stream.Write(buffer, 0, 1);
                index++;
            }

            stream.Position = originalPosition;
        }

        public static MemoryStream UnpackToMemory(Stream stream)
        {
            stream.Position = 0;
            var output = new MemoryStream();
            try
            {
                using var zlib = new ZLibStream(stream, CompressionMode.Decompress, leaveOpen: true);
                zlib.CopyTo(output);
            }
            catch (InvalidDataException)
            {
                stream.Position = 0;
                stream.CopyTo(output);
            }
            output.Position = 0;
            return output;
        }

        public static Bitmap LoadBitmap(string imageData)
        {
            if (string.Equals(imageData, "EMPTY", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Image data marked as EMPTY.", nameof(imageData));
            }

            var raw = Convert.FromBase64String(imageData);
            using var encoded = new MemoryStream(raw);
            using var unpacked = UnpackToMemory(encoded);
            using var temp = new Bitmap(unpacked);
            return new Bitmap(temp);
        }

        public static Region CreateRegion(Bitmap bitmap)
        {
            var path = new GraphicsPath();
            var transparentColor = bitmap.GetPixel(0, 0);
            for (int y = 0; y < bitmap.Height; y++)
            {
                int start = -1;
                for (int x = 0; x < bitmap.Width; x++)
                {
                    bool isTransparent = bitmap.GetPixel(x, y).ToArgb() == transparentColor.ToArgb();
                    if (!isTransparent)
                    {
                        if (start == -1)
                        {
                            start = x;
                        }
                    }
                    else if (start != -1)
                    {
                        path.AddRectangle(new System.Drawing.Rectangle(start, y, x - start, 1));
                        start = -1;
                    }
                }

                if (start != -1)
                {
                    path.AddRectangle(new System.Drawing.Rectangle(start, y, bitmap.Width - start, 1));
                }
            }

            return new Region(path);
        }

    }
}
