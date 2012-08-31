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
            return MarshallPointerArray<T>(values, values.Length);
        }

        public static IntPtr[] MarshallPointerArray<T>(IEnumerable<T> values, int count) where T : IPointerWrapper
        {
            IntPtr[] valuePointers = new IntPtr[count];
            int i = 0;
            foreach(T pointer in values)
            {
                valuePointers[i++] = pointer.NativePointer;
                if(i == count)
                    break;
            }
            return valuePointers;
        }
    }
}
