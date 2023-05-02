using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainFuzz
{
    static class Compiler
    {
        public static string Compile(string[] lines)
        {
            string prg = "";
            string error = "";
            int ln = 1;
            foreach (string line in lines)
            {
                if(error == "")
                {
                    string[] inst = line.Split(',');
                    int len = inst.Length;
                    switch (inst[0])
                    {
                        //Standard BF Instructions -- Plus a little jazz
                        case "left" or "l":
                            if (len == 1) { prg += "<"; }
                            else if (len == 2) { if (Int16.TryParse(inst[1], out short num)) { for (int i = 0; i < num; i++) { prg += "<"; } } else { error = "Not a Number"; } }
                            else { error = "Invalid Amount of Arguments"; }
                            break;
                        case "right" or "r":
                            if (len == 1) { prg += ">"; }
                            else if (len == 2) { if (Int16.TryParse(inst[1], out short num)) { for (int i = 0; i < num; i++) { prg += ">"; } } else { error = "Not a Number"; } }
                            else { error = "Invalid Amount of Arguments"; }
                            break;
                        case "add" or "a":
                            if (len == 1) { prg += "+"; }
                            else if (len == 2) { if (Int16.TryParse(inst[1], out short num)) { for (int i = 0; i < num; i++) { prg += "+"; } } else { error = "Not a Number"; } }
                            break;
                        case "subtract" or "sub" or "s":
                            if (len == 1) { prg += "-"; }
                            else if (len == 2) { if (Int16.TryParse(inst[1], out short num)) { for (int i = 0; i < num; i++) { prg += "-"; } } else { error = "Not a Number"; } }
                            else { error = "Invalid Amount of Arguments"; }
                            break;
                        case "output" or "out":
                            if (len == 1) { prg += "."; }
                            else { error = "Invalid Amount of Arguments"; }
                            break;
                        case "input" or "in":
                            if (len == 1) { prg += ","; }
                            else { error = "Invalid Amount of Arguments"; }
                            break;
                        case "if":
                            if (len == 1) { prg += "["; }
                            else { error = "Invalid Amount of Arguments"; }
                            break;
                        case "end":
                            if (len == 1) { prg += "]"; }
                            else { error = "Invalid Amount of Arguments"; }
                            break;

                        //BrainFizz Instructions
                        case "set":
                            if (len == 1) { prg += "[-]"; }
                            else if (len == 2) { if (Int16.TryParse(inst[1], out short num)) { prg += "[-]"; for (int i = 0; i < num; i++) { prg += "+"; } } else { error = "Not a Number"; } }
                            else { error = "Invalid Amount of Arguments"; }
                            break;

                        case "":
                            break;

                        default:
                            error = "Invalid Instruction";
                            break;
                    }
                    ln++;
                }
                else { prg = "error"; Console.WriteLine("Error: " + error + ": At Line: " + ln); break; }
            }
            return prg;
        }
    }
}
