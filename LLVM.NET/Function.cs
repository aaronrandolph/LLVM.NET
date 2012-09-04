using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class Function : IPointerWrapper
    {
        private readonly LLVMTypeRef* m_funcType;
        private readonly LLVMValueRef* m_handle;
        private readonly string m_name;
        private readonly TypeRef m_returnType;
        private readonly TypeRef[] m_paramTypes;

        public Function(Module module, string name, TypeRef returnType, TypeRef[] paramTypes)
        {
            m_name = name;
            m_returnType = returnType;
            m_paramTypes = paramTypes;

            IntPtr[] paramArray = LLVMHelper.MarshallPointerArray(paramTypes);

            m_funcType = Native.FunctionType(returnType.Handle, paramArray, (uint)paramArray.Length, 0);
            m_handle = Native.AddFunction(module.Handle, name, m_funcType);
        }

        public Function(LLVMValueRef* handle)
            : this(Value.GetName(handle), handle)
        {
        }

        public Function(string name, LLVMValueRef* handle)
        {
            m_name = name;
            m_handle = handle;
            m_funcType = Native.GetElementType(Native.TypeOf(handle));
            m_returnType = new TypeRef(Native.GetReturnType(m_funcType));

            uint paramCount = Native.CountParamTypes(m_funcType);

            if(paramCount > 0)
            {
                IntPtr[] types = new IntPtr[paramCount];
                Native.GetParamTypes(m_funcType, types);

                m_paramTypes = new TypeRef[paramCount];
                for(int i = 0; i < paramCount; ++i)
                {
                    m_paramTypes[i] = new TypeRef((LLVMTypeRef*)types[i]);
                }
            }
            else
            {
                m_paramTypes = new TypeRef[0];
            }
        }

        public LLVMValueRef* Handle
        {
            get { return m_handle; }
        }

        public LLVMTypeRef* FunctionType
        {
            get { return m_funcType; }
        }

        public int ArgCount
        {
            get { return m_paramTypes.Length; }
        }

        public bool HasBody
        {
            get { return Native.CountBasicBlocks(m_handle) > 0; }
        }

        public Value GetParameter(uint index)
        {
            LLVMValueRef* p = Native.GetParam(m_handle, index);
            if(p == null)
                return Value.Null;

            return new Value(p);
        }

        public bool IsDuplicate()
        {
            return Value.GetName(m_handle) != m_name;
        }

        public void SetLinkage(LLVMLinkage linkage)
        {
            Native.SetLinkage(m_handle, linkage);
        }

        public void Delete()
        {
            Native.DeleteFunction(m_handle);
        }

        public BasicBlock AppendBasicBlock(string name)
        {
            return new BasicBlock(name, this);
        }

        public BasicBlock AppendBasicBlock(BasicBlock block)
        {
            Native.MoveBasicBlockAfter(block.Handle, Native.GetLastBasicBlock(m_handle));
            return block;
        }

        public bool Validate()
        {
            return Native.VerifyFunction(m_handle, LLVMVerifierFailureAction.PrintMessageAction) != 0;
        }

        public void Dump()
        {
            Native.DumpValue(m_handle);
        }

        #region IPointerWrapper Members

        IntPtr IPointerWrapper.NativePointer
        {
            get { return (IntPtr)m_handle; }
        }

        #endregion
    }
}
