﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLVM;

namespace Kaleidoscope.Chapter3
{
    /// ExprAST - Base class for all expression nodes.
    abstract class ExprAST
    {
        public abstract Value CodeGen();
    }

    /// NumberExprAST - Expression class for numeric literals like "1.0".
    class NumberExprAST : ExprAST
    {
        public double Val { get; set; }

        public NumberExprAST(double val) 
        {
            this.Val = val;
        }

        public override Value CodeGen()
        {
            return Value.CreateConstReal(TypeRef.CreateDouble(), this.Val);
        }
    }

    /// VariableExprAST - Expression class for referencing a variable, like "a".
    class VariableExprAST : ExprAST 
    {
        public string Name { get; set; }

        public VariableExprAST(string name)
        {
            this.Name = name;
        }

        public override Value CodeGen()
        {
            Value value = null;

            if(!CodeGenManager.NamedValues.TryGetValue(this.Name, out value))
                CodeGenManager.ErrorOutput.WriteLine("Unknown variable name.");

            return value;
        }
    }

    /// BinaryExprAST - Expression class for a binary operator.
    class BinaryExprAST : ExprAST 
    {
        public char Op { get; set; }
        public ExprAST LHS { get; set; }
        public ExprAST RHS { get; set; }

        public BinaryExprAST(char op, ExprAST lhs, ExprAST rhs) 
        {
            this.Op = op;
            this.LHS = lhs;
            this.RHS = rhs;
        }

        public override Value CodeGen()
        {
            Value l = this.LHS.CodeGen();
            Value r = this.RHS.CodeGen();
            if(l == null || r == null) return null;

            switch(this.Op)
            {
                case '+':
                    return CodeGenManager.Builder.CreateFAdd(l, r);
                case '-':
                    return CodeGenManager.Builder.CreateFSub(l, r);
                case '*':
                    return CodeGenManager.Builder.CreateFMul(l, r);
                case '<':
                    // Convert bool 0/1 to double 0.0 or 1.0
                    return CodeGenManager.Builder.CreateFCmpAndPromote(l, LLVMRealPredicate.RealULT, r, TypeRef.CreateDouble());
            }

            CodeGenManager.ErrorOutput.WriteLine("Unknown binary operator.");
            return null;
        }
    }

    /// CallExprAST - Expression class for function calls.
    class CallExprAST : ExprAST 
    {
        public string Callee { get; set; }
        public List<ExprAST> Args { get; private set; }

        public CallExprAST(string callee, IEnumerable<ExprAST> args)
        {
            this.Callee = callee;
            this.Args = new List<ExprAST>(args);
        }

        public override Value CodeGen()
        {
            // Look up the name in the global module table.
            Function func = CodeGenManager.Module.GetFunction(this.Callee);
            if(func == null)
            {
                CodeGenManager.ErrorOutput.WriteLine("Unknown function referenced.");
                return null;
            }

            // If argument mismatch error.
            if(func.ArgCount != Args.Count)
            {
                CodeGenManager.ErrorOutput.WriteLine("Incorrect # arguments passed.");
                return null;
            }

            List<Value> args = new List<Value>();
            foreach(var arg in this.Args)
            {
                Value val = arg.CodeGen();
                if(val == null)
                    return null;

                args.Add(val);
            }

            return CodeGenManager.Builder.CreateCall(func, args.ToArray());
        }
    }

    /// PrototypeAST - This class represents the "prototype" for a function,
    /// which captures its name, and its argument names (thus implicitly the number
    /// of arguments the function takes).
    class PrototypeAST 
    {
        public string Name { get; set; }
        public List<string> Args { get; private set; }

        public PrototypeAST(string name, IEnumerable<string> args)
        {
            this.Name = name;
            this.Args = new List<string>(args);
        }

        public Function CodeGen()
        {
            List<TypeRef> args = new List<TypeRef>();
            this.Args.ForEach(a => args.Add(TypeRef.CreateDouble()));

            Function func = new Function(CodeGenManager.Module, this.Name,
                                                 TypeRef.CreateDouble(), args.ToArray());
            func.SetLinkage(LLVMLinkage.ExternalLinkage);

            // If F conflicted, there was already something named 'Name'.  If it has a
            // body, don't allow redefinition or reextern.
            if(func.IsDuplicate())
            {
                // Delete the one we just made and get the existing one.
                func.Delete();
                func = CodeGenManager.Module.GetFunction(this.Name);

                // If F already has a body, reject this.
                if(func.HasBody)
                {
                    CodeGenManager.ErrorOutput.WriteLine("redefinition of function.");
                    return null;
                }

                // If F took a different number of args, reject.
                if(func.ArgCount != this.Args.Count)
                {
                    CodeGenManager.ErrorOutput.WriteLine("redefinition of function with different # args.");
                    return null;
                }
            }

            // Set names for all arguments.
            for(int i = 0; i < func.ArgCount; ++i)
            {
                Value val = func.GetParameter((uint)i);
                val.Name = this.Args[i];
                CodeGenManager.NamedValues[this.Args[i]] = val;
            }

            return func;
        }
    }

    /// FunctionAST - This class represents a function definition itself.
    class FunctionAST 
    {
        public PrototypeAST Proto { get; set; }
        public ExprAST Body { get; set; }

        public FunctionAST(PrototypeAST proto, ExprAST body)
        {
            this.Proto = proto;
            this.Body = body;
        }

        public Function CodeGen()
        {
            CodeGenManager.NamedValues.Clear();
            Function func = this.Proto.CodeGen();
            if(func == null)
                return null;

            // Create a new basic block to start insertion into.
            BasicBlock bb = func.AppendBasicBlock("entry");
            CodeGenManager.Builder.SetInsertPoint(bb);
            Value retVal = Body.CodeGen();

            if(retVal != null)
            {
                CodeGenManager.Builder.CreateReturn(retVal);

                // Validate the generated code, checking for consistency.
                func.Validate();

                return func;
            }

            // Error reading body, remove function.
            func.Delete();
            return null;
        }
    };
}
