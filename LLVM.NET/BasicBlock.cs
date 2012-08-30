using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class BasicBlock : IPointerWrapper
    {
        private readonly LLVMBasicBlockRef* m_handle;
        private readonly string m_name;

        public BasicBlock(LLVMBasicBlockRef* handle, string name)
        {
            if(handle == null)
                throw new ArgumentNullException("handle");

            if(name == null)
                name = Value.GetName((LLVMValueRef*)handle);

            m_handle = handle;
            m_name = name;
        }

        public BasicBlock(string name)
        {
            Guard.ArgumentNull(name, "name");

            m_handle = Native.AppendBasicBlock(null, name);
            m_name = name;
        }

        public BasicBlock(string name, Function function)
        {
            Guard.ArgumentNull(name, "name");

            m_handle = Native.AppendBasicBlock(function != null ? function.Handle : null, name);
            m_name = name;
        }

        public LLVMBasicBlockRef* Handle
        {
            get { return m_handle; }
        }

        public string Name
        {
            get { return m_name; }
        }

        public Function GetParent()
        {
            return new Function(Native.GetBasicBlockParent(m_handle));
        }

        #region IPointerWrapper Members

        IntPtr IPointerWrapper.NativePointer
        {
            get { return (IntPtr)m_handle; }
        }

        #endregion
    }
}
