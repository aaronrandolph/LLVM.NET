using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class TypeRef
    {
        private readonly LLVMTypeRef* m_handle;

        public TypeRef(LLVMTypeRef* handle)
        {
            if(handle == null)
                throw new ArgumentNullException("handle");

            m_handle = handle;
        }

        public LLVMTypeRef* Handle
        {
            get { return m_handle; }
        }

        public static TypeRef CreateBool()
        {
            return new TypeRef(Native.Int1Type());
        }

        public static TypeRef CreateInt8()
        {
            return new TypeRef(Native.Int8Type());
        }

        public static TypeRef CreateInt16()
        {
            return new TypeRef(Native.Int16Type());
        }

        public static TypeRef CreateInt32()
        {
            return new TypeRef(Native.Int32Type());
        }

        public static TypeRef CreateInt64()
        {
            return new TypeRef(Native.Int64Type());
        }

        public static TypeRef CreateFloat()
        {
            return new TypeRef(Native.HalfType());
        }

        public static TypeRef CreateDouble()
        {
            return new TypeRef(Native.DoubleType());
        }
    }
}
