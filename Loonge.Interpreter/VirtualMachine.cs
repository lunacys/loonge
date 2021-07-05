using System;
using System.Diagnostics;

namespace Loonge.interpreter
{
    public enum Instruction : int
    {
        Literal = 0,
        WriteNum = 1,
        WriteStr = 2,
        Add = 3,
        GetRegister = 4,
        GetTop
    }

    public static class VmOperations
    {
        public static void WriteSomethingWithNumber(int message, int number)
        {
            Console.WriteLine($"Message: {message}. Number: {number}");
        }

        public static void WriteSomethingWithString(int message, int str)
        {
            Console.WriteLine($"Message: {message}. String: {str}");
        }

        public static int Add(int a, int b)
        {
            return a + b;
        }
    }

    public class VirtualMachine
    {
        public static readonly int MaxStack = 128;
        private int _stackSize = 0;
        private int[] _stack;

        private int _register = 0xF1;

        public VirtualMachine()
        {
            _stack = new int[MaxStack];
        }

        public void Interpret(int[] bytecode)
        {
            for (var i = 0; i < bytecode.Length; i++)
            {
                var c = bytecode[i];

                if ((Instruction) c == Instruction.WriteNum)
                {
                    var str = Pop();
                    var str2 = Pop();

                    VmOperations.WriteSomethingWithNumber(str, str2);
                }
                else if ((Instruction) c == Instruction.WriteStr)
                {
                    var str = Pop();
                    var str2 = Pop();

                    VmOperations.WriteSomethingWithString(str, str2);
                }
                else if ((Instruction) c == Instruction.Add)
                {
                    var str = Pop();
                    var str2 = Pop();
                    Push(VmOperations.Add(str, str2));
                }
                else if ((Instruction) c == Instruction.Literal)
                {
                    var value = bytecode[++i];
                    Push(value);
                }
                else if ((Instruction) c == Instruction.GetRegister)
                {
                    Push(_register);
                }
                else if ((Instruction) c == Instruction.GetTop)
                {
                    Console.WriteLine(Pop());
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void Push(int value)
        {
            Debug.Assert(_stackSize < MaxStack);
            _stack[_stackSize++] = value;
        }

        private int Pop()
        {
            Debug.Assert(_stackSize > 0);
            return _stack[--_stackSize];
        }

        private int Peek()
        {
            Debug.Assert(_stackSize > 0);
            return _stack[_stackSize];
        }
    }
}