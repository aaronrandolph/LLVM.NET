using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class BasicBlock
    {
        private readonly LLVMBasicBlockRef* m_handle;

        public BasicBlock(LLVMBasicBlockRef* handle)
        {
            m_handle = handle;
        }

        public LLVMBasicBlockRef* Handle
        {
            get { return m_handle; }
        }
    }
}
