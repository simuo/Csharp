using System;

namespace MyExample
{
    class Program
    {
        static void Main(string[] args)
        {
            CalcuLator c = new CalcuLator();
            int x= c.Add(2, 3);
            Console.WriteLine(x);
        }
        class CalcuLator
        {
            public int Add(int a,int b)
            {
                int result = a + b;
                return result;
            }
        }
    }
}
