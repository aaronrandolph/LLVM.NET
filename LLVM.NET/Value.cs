using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class Value
    {
        private readonly LLVMValueRef* m_handle;

        public Value(LLVMValueRef* handle)
        {
            if(handle == null)
                throw new ArgumentNullException("handle");

            m_handle = handle;
        }

        public LLVMValueRef* Handle
        {
            get { return m_handle; }
        }

        public string Name
        {
            get
            {
                return GetName(m_handle);
            }
            set
            {
                Native.SetValueName(m_handle, value);
            }
        }

        public static string GetName(LLVMValueRef* value)
        {
            IntPtr name = Native.GetValueName(value);
            return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(name);
        }

        public static Value CreateConstInt(TypeRef type, long value)
        {
            return new Value(Native.ConstInt(type.Handle, (ulong)value, 1));
        }

        public static Value CreateConstUInt(TypeRef type, ulong value)
        {
            return new Value(Native.ConstInt(type.Handle, value, 0));
        }

        public static Value CreateConstReal(TypeRef type, double value)
        {
            return new Value(Native.ConstReal(type.Handle, value));
        }
    }
}
