using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
    public unsafe class IRBuilder
    {
        private readonly LLVMBuilderRef* m_builder;

        public IRBuilder()
        {
            m_builder = Native.CreateBuilder();
        }

        public Value CreateFAdd(Value lhs, Value rhs)
        {
            return new Value(Native.BuildFAdd(m_builder, lhs.Handle, rhs.Handle, "addtmp"));
        }

        public Value CreateFSub(Value lhs, Value rhs)
        {
            return new Value(Native.BuildFSub(m_builder, lhs.Handle, rhs.Handle, "subtmp"));
        }

        public Value CreateFMul(Value lhs, Value rhs)
        {
            return new Value(Native.BuildFMul(m_builder, lhs.Handle, rhs.Handle, "multmp"));
        }

        public Value CreateFCmp(Value lhs, LLVMRealPredicate predicate, Value rhs)
        {
            return new Value(Native.BuildFCmp(m_builder, predicate, lhs.Handle, rhs.Handle, "cmptmp"));
        }

        public Value CreateFCmpAndPromote(Value lhs, LLVMRealPredicate predicate, Value rhs, TypeRef promoteType)
        {
            lhs = CreateFCmp(lhs, predicate, rhs);
            return new Value(Native.BuildUIToFP(m_builder, lhs.Handle, promoteType.Handle, "promotetmp")); 
        }

        public Value CreateCall(Function func, Value[] args)
        {
            IntPtr[] argVals = new IntPtr[args.Length];
            for(int i = 0; i < args.Length; ++i)
            {
                argVals[i] = (IntPtr)args[i].Handle;
            }

            return new Value(Native.BuildCall(m_builder, func.Handle, argVals, (uint)args.Length, "calltmp"));
        }

        public Value CreateReturn(Value returnValue)
        {
            return new Value(Native.BuildRet(m_builder, returnValue.Handle));
        }

        public void SetInsertPoint(BasicBlock bb)
        {
            Native.PositionBuilderAtEnd(m_builder, bb.Handle);
        }
    }
}
