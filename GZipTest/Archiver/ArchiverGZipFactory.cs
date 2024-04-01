using GZipTest.Archiver.Archiver;
using System.IO.Compression;

namespace GZipTest.Archiver
{
    internal static class ArchiverGZipFactory
    {
        internal static ArchiverGZip Get(CompressionMode mode, string sourcePath, string targetPath, CancellationToken token)
        {
            if (mode == CompressionMode.Compress)
                return new Compressor(sourcePath, targetPath, token);
            else if (mode == CompressionMode.Decompress)
                return new Decompressor(sourcePath, targetPath, token);

            throw new Exception($"Invalid compression mode: \"{mode.ToString()}\"!");
        }
    }
}
