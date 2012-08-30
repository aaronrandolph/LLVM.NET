using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public static class LLVMUtilities
    {
        public static IntPtr[] MarshallPointerArray<T>(T[] values) where T : IPointerWrapper
        {
            IntPtr[] valuePointers = new IntPtr[values.Length];
            for(int i = 0; i < values.Length; ++i)
            {
                valuePointers[i] = values[i].NativePointer;
            }
            return valuePointers;
        }
    }
}
