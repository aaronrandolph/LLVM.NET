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

        public Value BuildFAdd(Value lhs, Value rhs)
        {
            return new Value(Native.BuildFAdd(m_builder, lhs.Handle, rhs.Handle, "addtmp"));
        }

        public Value BuildFSub(Value lhs, Value rhs)
        {
            return new Value(Native.BuildFSub(m_builder, lhs.Handle, rhs.Handle, "subtmp"));
        }

        public Value BuildFMul(Value lhs, Value rhs)
        {
            return new Value(Native.BuildFMul(m_builder, lhs.Handle, rhs.Handle, "multmp"));
        }

        public Value BuildFCmp(Value lhs, LLVMRealPredicate predicate, Value rhs)
        {
            return new Value(Native.BuildFCmp(m_builder, predicate, lhs.Handle, rhs.Handle, "cmptmp"));
        }

        public Value BuildFCmpAndPromote(Value lhs, LLVMRealPredicate predicate, Value rhs, TypeRef promoteType)
        {
            lhs = BuildFCmp(lhs, predicate, rhs);
            return new Value(Native.BuildUIToFP(m_builder, lhs.Handle, promoteType.Handle, "promotetmp")); 
        }

        public Value BuildCondBr(Value ifVal, BasicBlock thenBlock, BasicBlock elseBlock)
        {
            return new Value(Native.BuildCondBr(m_builder, ifVal.Handle, thenBlock.Handle, elseBlock.Handle));
        }

        public Value BuildBr(BasicBlock branchBlock)
        {
            return new Value(Native.BuildBr(m_builder, branchBlock.Handle));
        }

        public Value BuildCall(Function func, Value[] args)
        {
            IntPtr[] argVals = LLVMUtilities.MarshallPointerArray(args);

            return new Value(Native.BuildCall(m_builder, func.Handle, argVals, (uint)args.Length, "calltmp"));
        }

        public Value BuildReturn(Value returnValue)
        {
            return new Value(Native.BuildRet(m_builder, returnValue.Handle));
        }

        public Value BuildPHI(TypeRef type, string name, Value[] incomingValues, BasicBlock[] incomingBlocks)
        {
            if(incomingValues.Length != incomingBlocks.Length)
                throw new InvalidOperationException("Incoming values and blocks must be the same size.");

            IntPtr[] valPointers = LLVMUtilities.MarshallPointerArray(incomingValues);
            IntPtr[] blockPointers = LLVMUtilities.MarshallPointerArray(incomingBlocks);

            LLVMValueRef* phi = Native.BuildPhi(m_builder, type.Handle, name);
            Native.AddIncoming(phi, valPointers, blockPointers, (uint)valPointers.Length);

            return new Value(phi);
        }

        public void SetInsertPoint(BasicBlock bb)
        {
            Native.PositionBuilderAtEnd(m_builder, bb.Handle);
        }

        public BasicBlock GetInsertPoint()
        {
            return new BasicBlock(Native.GetInsertBlock(m_builder), null);
        }
    }
}
