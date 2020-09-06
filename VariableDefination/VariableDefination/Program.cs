using System;

namespace VariableDefination
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            short a;
            int b;
            double c;
            
            //初始化
            a = 10;
            b = 20;
            c = a + b;
            Console.WriteLine("a={0}, b={1}, c={2}",a,b,c);

            int input = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("input = {0}",input);
        }
    }
}