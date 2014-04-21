using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe struct TypeRef : IPointerWrapper
    {
        public static readonly TypeRef Null = new TypeRef();

        private readonly LLVMTypeRef* m_handle;

        private readonly LLVMTypeRef* m_pointedType;
        public LLVMTypeRef* PointedType { get { return m_pointedType; } }

        public TypeRef(LLVMTypeRef* handle, LLVMTypeRef* pointed = null)
        {
            if(handle == null)
                throw new ArgumentNullException("handle");

            m_handle = handle;
            m_pointedType = pointed;
        }

        public LLVMTypeRef* Handle
        {
            get { return m_handle; }
        }

        public LLVMTypeKind TypeKind
        {
            get 
            {
                if(m_handle == null)
                    throw new InvalidOperationException("TypeRef is null");

                return Native.GetTypeKind(m_handle); 
            }
        }

        public bool IsNull
        {
            get { return m_handle == null; }
        }

        public static String Tostring(TypeRef type)
        {
            string ret = type.TypeKind.ToString();
            if (type.TypeKind == LLVMTypeKind.PointerTypeKind)
                ret += " to " + Native.GetTypeKind(type.PointedType).ToString();
            return ret;
        }


        #region Construction of types

        public Value CreateNullValue()
        {
            if(m_handle == null)
                throw new InvalidOperationException("TypeRef is null");

            return new Value(Native.ConstNull(m_handle));
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

        public static TypeRef CreateVoid()
        {
            return new TypeRef(Native.VoidType());
        }

        public static TypeRef CreatePointer(TypeRef target)
        {
            Guard.ArgumentNull(target, "target");
            return new TypeRef(Native.PointerType(target.Handle, 0), target.Handle);
        }

        #endregion


        #region IPointerWrapper Members

        IntPtr IPointerWrapper.NativePointer
        {
            get { return (IntPtr)m_handle; }
        }

        #endregion
    }
}
