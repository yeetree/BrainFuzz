namespace BrainFK
{
    public sealed class Program
    {
        // number of cells of the available memory
        static int TAPE_LENGTH = 32767;

        // required variables
        static string prog;
        static short[] tape;
        static int ptrTape;
        static int ptrProg;
        static Stack<int> loopStack;
        static Dictionary<int, int> jmpTable;

        static void Main(string[] args)
        {
            // the interpreter takes one argument: a path to a brainfuck source file
            if (args.Length == 0)
            {
                Console.WriteLine("Please Specify File");
                return;
            }

            // check if the file exists
            var sourceFile = new FileInfo(args[0]);
            if (!sourceFile.Exists)
            {
                Console.WriteLine("File Specified is Not Found");
                return;
            }

            // alright, good to go
            Interpret(sourceFile);
        }

        static void Interpret(FileInfo sourceFile)
        {
            // try to open and read the file using a StreamReader
            try
            {
                using (StreamReader reader = sourceFile.OpenText())
                {
                    prog = reader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                // something went wrong
                // most likely, this would be either a security exception or the
                // file is too big
                Console.WriteLine("Could Not Read File");
                return;
            }

            // initialize variables
            tape = new short[TAPE_LENGTH];
            ptrProg = 0;
            ptrTape = 0;
            loopStack = new Stack<int>();
            jmpTable = new Dictionary<int, int>();

            // start building the jump table:
            // the jump table is a key-value store which matches positions of brackets [ and ]
            // with the positions of their matching counterparts
            // this is also the only place where any error checking is done: all brackets [ and ]
            // must have matching counterparts
            for (int i = 0; i < prog.Length; i++)
            {
                switch (prog[i])
                {
                    case '[':
                        // push its position on the stack
                        loopStack.Push(i);
                        break;
                    case ']':
                        if (loopStack.Count == 0)
                        {
                            // if the stack is empty, then there is no matching counterpart for
                            // this ] bracket
                            // example: program +++[>+++++<-]] would throw this error
                            Console.WriteLine("Unmatched ] at position {0}.", i);
                            return;
                        }
                        else
                        {
                            // if the stack is non-empty, then the top element on the stack is
                            // the position of the matching [ bracket
                            // we can now add both positions to the jump table
                            // note: the +1 is because we actually jump to the position right
                            // after the matching counterpart bracket
                            var openPos = loopStack.Pop();
                            jmpTable.Add(openPos, i + 1);
                            jmpTable.Add(i, openPos + 1);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (loopStack.Count > 0)
            {
                // if the stack is non-empty after running over the entire program, then there
                // are one or more [ brackets with no matching counterparts
                // we only show the first one as an error
                // example: program +++[>+++++[>+++++++<-] would throw this error
                Console.WriteLine("Unmatched [ at position {0}.", loopStack.Pop());
                return;
            }

            // here we actually start running the program
            while (ptrProg < prog.Length)
            {
                switch (prog[ptrProg])
                {
                    case '>':
                        // increment the data pointer by one
                        // if we have reached the end of the tape, we simply wrap around
                        ptrTape++;
                        if (ptrTape == TAPE_LENGTH)
                            ptrTape = 0;
                        ptrProg++;
                        break;
                    case '<':
                        // decrement the data pointer by one
                        // if we have reached the start of the tape, we simply wrap around
                        ptrTape--;
                        if (ptrTape == -1)
                            ptrTape = TAPE_LENGTH - 1;
                        ptrProg++;
                        break;
                    case '+':
                        // increment the value at the data pointer by one
                        tape[ptrTape]++;
                        ptrProg++;
                        break;
                    case '-':
                        // decrement the value at the data pointer by one
                        tape[ptrTape]--;
                        ptrProg++;
                        break;
                    case '.':
                        // write the value at the data pointer to the console
                        // we need to convert it to a char first
                        Console.Write(Convert.ToChar(tape[ptrTape]));
                        ptrProg++;
                        break;
                    case ',':
                        // read the next character from the standard input stream
                        // in windows, hitting the enter key produces a carriage return and a
                        // line feed (CR+LF), but most brainfuck programs are designed to work
                        // with a line feed only, so we simply ignore the carriage return (13)
                        // also, EOF's are handled by doing nothing
                        var rd = (short)Console.Read();
                        if (rd != 13)
                        {
                            if (rd != -1)
                                tape[ptrTape] = rd;
                            ptrProg++;
                        }
                        break;
                    case '[':
                        // if the value at the data pointer is 0, we jump to the command after
                        // the matching ] bracket
                        // otherwise, we simply increment the program pointer
                        if (tape[ptrTape] == 0)
                            ptrProg = jmpTable[ptrProg];
                        else
                            ptrProg++;
                        break;
                    case ']':
                        // if the value at the data pointer is 0, we simply increment the program
                        // pointer
                        // otherwise, we jump to the command after the matching [ bracket
                        if (tape[ptrTape] == 0)
                            ptrProg++;
                        else
                            ptrProg = jmpTable[ptrProg];
                        break;
                    default:
                        // any other characters not matching a brainfuck token are simply ignored
                        ptrProg++;
                        break;
                }
            }
        }
    }
}