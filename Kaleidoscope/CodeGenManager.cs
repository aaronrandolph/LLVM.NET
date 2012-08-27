using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVM;

namespace Kaleidoscope
{
    static unsafe class CodeGenManager
    {
        public static Module Module { get; set; }
        public static IDictionary<string, Value> NamedValues { get; private set; }
        public static TextWriter ErrorOutput { get; set; }

        static CodeGenManager()
        {
            NamedValues = new Dictionary<string, Value>();
            ErrorOutput = Console.Out;
        }
    }
}
