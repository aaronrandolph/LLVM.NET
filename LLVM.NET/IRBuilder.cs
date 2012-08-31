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
            return BuildFAdd(lhs, rhs, "addtmp");
        }

        public Value BuildFAdd(Value lhs, Value rhs, string varName)
        {
            return new Value(Native.BuildFAdd(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildFSub(Value lhs, Value rhs)
        {
            return BuildFSub(lhs, rhs, "subtmp");
        }

        public Value BuildFSub(Value lhs, Value rhs, string varName)
        {
            return new Value(Native.BuildFSub(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildFMul(Value lhs, Value rhs)
        {
            return BuildFMul(lhs, rhs, "multmp");
        }

        public Value BuildFMul(Value lhs, Value rhs, string varName)
        {
            return new Value(Native.BuildFMul(m_builder, lhs.Handle, rhs.Handle, varName));
        }

        public Value BuildFCmp(Value lhs, LLVMRealPredicate predicate, Value rhs)
        {
            return BuildFCmp(lhs, predicate, rhs, "cmptmp");
        }

        public Value BuildFCmp(Value lhs, LLVMRealPredicate predicate, Value rhs, string varName)
        {
            return new Value(Native.BuildFCmp(m_builder, predicate, lhs.Handle, rhs.Handle, varName));
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
            return BuildCall(func, args, "calltmp");
        }

        public Value BuildCall(Function func, Value[] args, string varName)
        {
            IntPtr[] argVals = LLVMUtilities.MarshallPointerArray(args);

            return new Value(Native.BuildCall(m_builder, func.Handle, argVals, (uint)args.Length, varName));
        }

        public Value BuildReturn(Value returnValue)
        {
            return new Value(Native.BuildRet(m_builder, returnValue.Handle));
        }

        public Value BuildPhi(TypeRef type, string name, PhiIncoming incoming)
        {
            IntPtr[] valPointers = incoming.GetValuePointers();
            IntPtr[] blockPointers = incoming.GetBlockPointers();

            LLVMValueRef* phi = Native.BuildPhi(m_builder, type.Handle, name);
            Native.AddIncoming(phi, valPointers, blockPointers, (uint)valPointers.Length);

            return new Value(phi);
        }

        public void AddPhiIncoming(Value phiNode, Value value, BasicBlock block)
        {
            IntPtr[] valPointers = new IntPtr[] { (IntPtr)value.Handle };
            IntPtr[] blockPointers = new IntPtr[] { (IntPtr)block.Handle };

            Native.AddIncoming(phiNode.Handle, valPointers, blockPointers, 1);
        }

        public void SetInsertPoint(BasicBlock bb)
        {
            Native.PositionBuilderAtEnd(m_builder, bb.Handle);
        }

        public BasicBlock GetInsertPoint()
        {
            return new BasicBlock(null, Native.GetInsertBlock(m_builder));
        }
    }
}
