namespace BrainFuzz
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                var sourceFile = new FileInfo(args[0]);
                if (!sourceFile.Exists)
                {
                    Console.WriteLine("File Specified is Not Found");
                    return;
                }
                string[] lines = File.ReadAllLines(args[0]);
                string outf = Compiler.Compile(lines);
                if(outf != "error")
                {
                    Console.WriteLine("Compiled Successfully");
                    File.WriteAllText(args[1], outf);
                }
                else
                {
                    Console.WriteLine("Could not Compile");
                }
            }
            else
            {
                Console.WriteLine("Please Specify File and Output File");
            }
        }
    }
}