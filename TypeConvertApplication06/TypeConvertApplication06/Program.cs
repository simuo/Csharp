using System;

namespace TypeConvertApplication06
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            double t = 3.12334123123;
            // Console.WriteLine(sizeof(double));
            int i;
            
            //强制转换double为int
            i = (int) t;
            Console.WriteLine(i);
            Console.ReadKey();
        }
    }
}