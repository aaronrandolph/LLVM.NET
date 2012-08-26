using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kaleidoscope.Chapter3;

namespace Kaleidoscope
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
            LLVM.Native.LinkInJIT();
            //LLVM.Native.InitializeNativeTarget(); // Declared in bindings but not exported from the shared library.
            LLVM.Native.InitializeX86TargetInfo();
            LLVM.Native.InitializeX86Target();
            LLVM.Native.InitializeX86TargetMC();

            Lexer lexer = new Lexer(Console.In);
            Parser parser = new Parser(lexer);
            using(LLVM.Module module = new LLVM.Module("my cool jit"))
            {
                CodeGenManager.Module = module;
                Driver driver = new Driver(parser);

                driver.MainLoop();

                Console.WriteLine("Module:");
                CodeGenManager.Module.Dump();
                Console.ReadLine();
            }
        }
    }
}
