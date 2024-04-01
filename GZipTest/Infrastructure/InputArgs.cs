using System.IO.Compression;

namespace GZipTest.Infrastructure
{
    internal class InputArgs
    {
        internal FileInfo SourceFile { get; init; }
        internal FileInfo TargetFile { get; init; }
        internal CompressionMode Mode { get; init; }

        /// <summary>
        /// Входные аргументы
        /// </summary>
        /// <param name="args">compress/decompress [имя исходного файла] [имя результирующего файла]</param>
        internal InputArgs(string[] args)
        {
            if (args.Length == 0 || args.Length > 3)
                throw new ArgumentException("The input parameters are incorrect!");

            // режим
            Mode = args[0].ToLower() switch
            {
                "compress" => CompressionMode.Compress,
                "decompress" => CompressionMode.Decompress,
                _ => throw new System.Exception($"Invalid compression mode: \"{args[0]}\"!")
            };

            // имя исходного файла
            SourceFile = new FileInfo(args[1]);
            if (!SourceFile.Exists)
                throw new System.Exception($"The file \"{args[1]}\" does not exist!");

            // имя результирующего файла
            TargetFile = new FileInfo(args[2]);
            if (string.IsNullOrEmpty(TargetFile.Extension))
                TargetFile = new FileInfo(TargetFile.Name + ".gz");
        }
    }
}
