using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class BasicBlock : Value
    {
        private readonly string m_name;

        public BasicBlock(string name, LLVMBasicBlockRef* handle)
            : base((LLVMValueRef*) handle)
        {
            m_name = name;  // base.name, calls LLVM API to fetch Value name
        }

        public BasicBlock(string name, Function function)
        {
            Guard.ArgumentNull(name, "name");
            Guard.ArgumentNull(function, "function");

            m_handle = (LLVMValueRef*) Native.AppendBasicBlock(function.Handle, name);
            m_name = name;
        }

        public LLVMBasicBlockRef* BBHandle
        {
            get { return (LLVMBasicBlockRef*) m_handle; }
        }

        public override bool IsNull
        {
            get { return m_handle == null; }
        }

        public override string Name
        {
            get { return m_name; }
        }

        public Value GetTerminator()
        {
            LLVMValueRef* term = Native.GetBasicBlockTerminator(BBHandle);
            if(term == null)
                return Value.Null;

            return new Value(term);
        }

        public void MoveBefore(BasicBlock block)
        {
            Guard.ArgumentNull(block, "block");
            Native.MoveBasicBlockBefore(BBHandle, block.BBHandle);
        }

        public void MoveAfter(BasicBlock block)
        {
            Guard.ArgumentNull(block, "block");
            Native.MoveBasicBlockAfter(BBHandle, block.BBHandle);
        }

        public void Delete()
        {
            if(BBHandle == null)
                throw new InvalidOperationException("BasicBlock is null");

            Native.DeleteBasicBlock(BBHandle);
            m_handle = null;
        }

        public void RemoveFromParent()
        {
            if(BBHandle == null)
                throw new InvalidOperationException("BasicBlock is null");

            Native.RemoveBasicBlockFromParent(BBHandle);
        }

        public Function GetParent()
        {
            if(BBHandle == null)
                throw new InvalidOperationException("BasicBlock is null");

            LLVMValueRef* func = Native.GetBasicBlockParent(BBHandle);
            if(func == null)
                return null;

            return new Function(func);
        }

    }
}
