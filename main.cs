using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Compiler.Forms;
using System.Windows.Forms;

namespace Compiler{
	class Program
   {
       [STAThread]
      static void Main(string[] args)
      {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
      }

        public static bool Compile(string[] args)
        {
            StreamReader istr = new StreamReader(Console.OpenStandardInput(), Console.InputEncoding);
            StreamWriter ostr = new StreamWriter(System.Console.OpenStandardOutput(), Console.OutputEncoding);

            if (args.Count() < 2)
            {
                return false;
            }

            
            int index_in = 1, index_out = 2;

            istr = new StreamReader(args[index_in]);
            if (index_out < args.Count())
            {
                ostr = new StreamWriter(args[index_out]);
            }

            Scaner scaner = null;
            Parser parser = null;
            Fasm.CodeGen codegen = null;
            switch (args[0])
            {
                case "-l":
                    scaner = new Scaner(istr);
                    Token t = null;
                    while (t == null || t.type != Token.Type.EOF)
                    {
                        try
                        {
                            t = scaner.Read();
                            Console.WriteLine(t.ToString());
                        }
                        catch (Scaner.Exception e)
                        {
                            ostr.WriteLine(e.Message);
                        }
                    }
                    break;
                case "-p":
                    parser = new Parser(new Scaner(istr));
                    parser.Parse();
                    parser.PrintTree(ostr);
                    ostr.WriteLine(parser.logger.ToString());
                    break;
                case "-c":
                    parser = new Parser(new Scaner(istr));
                    parser.Parse();
                    if (!parser.logger.isEmpty())
                    {
                        ostr.WriteLine(parser.logger.ToString());
                        break;
                    }
                    codegen = new Fasm.CodeGen(parser.tstack);
                    codegen.Generate(ostr);
                    break;
                case "-cexe":
                    parser = new Parser(new Scaner(istr));
                    parser.Parse();
                    if (!parser.logger.isEmpty())
                    {
                        ostr.WriteLine(parser.logger.ToString());
                        break;
                    }
                    codegen = new Fasm.CodeGen(parser.tstack);
                    codegen.Generate(ostr);
                    ostr.Flush();
                    ostr.Close();
                    if (index_out < args.Count())
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "C:/fasm/fasm.exe",
                            WindowStyle = ProcessWindowStyle.Hidden,
                            Arguments = string.Format("{0} {1}", args[index_out], args[index_out])
                        });
                    }
                    break;
                default:
                    return false;
            }

            istr.Close();
            ostr.Close();
            return true;
        }
   }
}