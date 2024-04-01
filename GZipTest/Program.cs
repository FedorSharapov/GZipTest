using GZipTest.Archiver;
using GZipTest.Infrastructure;

namespace GZipTest
{
    internal class Program
    {
        static CancellationTokenSource tokenSource = new();

        static int Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Cancel_KeyPress);
            try
            {
                var inArgs = new InputArgs(args);
                PrintStart(inArgs);

                var archiver = ArchiverGZipFactory.Get(inArgs.Mode, inArgs.SourceFile.FullName, 
                    inArgs.TargetFile.FullName, tokenSource.Token);
                archiver.Run();

                PrintResult(inArgs);
            }
            catch (Exception ex)
            {
                tokenSource.Cancel();
                Console.WriteLine(ex.Message);
                return 1;
            }
            finally
            {
                tokenSource.Dispose();
            }

            return 0;
        }

        static void Cancel_KeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Console.WriteLine("\r\nCanceling the operation...");
            tokenSource.Cancel();
        }
        
        static void PrintStart(InputArgs inputArgs)
        {
            if (inputArgs.Mode == System.IO.Compression.CompressionMode.Compress)
                Console.WriteLine($"Compression started.");
            else
                Console.WriteLine($"Decompression started.");
            Console.WriteLine($"The original file '{inputArgs.SourceFile.Name}' is {inputArgs.SourceFile.Length} bytes.");
        }
        static void PrintResult(InputArgs inputArgs)
        {
            if(inputArgs.Mode == System.IO.Compression.CompressionMode.Compress)
                Console.Write($"The compressed ");
            else
                Console.Write($"The decompressed ");
            Console.WriteLine($"file '{inputArgs.TargetFile.Name}' is {inputArgs.TargetFile.Length} bytes.");
        }
    }
}