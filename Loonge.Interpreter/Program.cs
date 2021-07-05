using System;

namespace Loonge.interpreter
{
    class Program
    {
        

        static void Main(string[] args)
        {
            var vm = new VirtualMachine();
            vm.Interpret(new []
            {
                0, 5,   // Literal 5
                0, 10,  // Literal 10
                3,      // Add
                5,      // GetTop
                0, 15,
                0, 25,
                2
            });
        }
    }
}
