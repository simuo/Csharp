using System;

namespace Reference13
{ 
    class Program
    {
        public void getValues(out int x,out int y)
        {
            x = Convert.ToInt32(Console.ReadLine());
            y = Convert.ToInt32(Console.ReadLine());
        }
        public static void Main(string[] args)
        {
            Program p =new Program();
            int a, b;
            p.getValues(out a,out b);
            Console.WriteLine(a);
            Console.WriteLine(b);
        }
    }
}