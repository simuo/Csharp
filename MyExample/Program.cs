using System;
using System.Runtime.InteropServices;

namespace MyExample
{
    class Program
    {
        static void Main(string[] args)
        {
            CalcuLator c = new CalcuLator();
            int x = c.Add(2, 3);
            string str = c.Today();
            Console.WriteLine(x);
            Console.WriteLine(str);
            c.PrintSum(100, 200);
            c.PrintXTo1(5);
            c.PrintXTo1WithRecursive(3);
            c.SumFrom1ToX(10);
            int RecursiveResult=c.SumFrom1ToXWithRecursive(10);
            Console.WriteLine("递归求1到X的和为：{0}",RecursiveResult);
            c.Sum1ToXWithFormula(10);
        }
        class CalcuLator
        {
            public int Add(int a, int b)
            {
                int result = a + b;
                return result;
            }
            public string Today()
            {
                int day = DateTime.Now.Day;
                return day.ToString();
            }
            public void PrintSum(int a, int b)
            {
                int ressult = a + b;
                Console.WriteLine(ressult);
            }
            //for循环实现打印从X到1
            public void PrintXTo1(int x)
            {
                for (int i = x; i >=1; i--)
                {
                    Console.WriteLine(i);
                }
            }
            //递归实现打印从X到1
            public void PrintXTo1WithRecursive(int x)
            {
                if (x == 1)
                {
                    Console.WriteLine(1);
                }
                else
                {
                    Console.WriteLine(x);
                    PrintXTo1WithRecursive(x - 1);
                }
            }
            //for循环实现1到100的和
            public void SumFrom1ToX(int x)
            {
                int sum = 0;
                for (int i = 1; i <=x; i++)
                {
                    sum += i;
                }
                Console.WriteLine("1到{1}的和是{0}",sum,x);
            }
            //递归实现求1到100的和
            public int SumFrom1ToXWithRecursive(int x)
            {
                if (x == 1)
                {
                    return 1;
                }
                else
                {
                    int result=  x + SumFrom1ToXWithRecursive(x - 1);
                    return result;
                }
            }
            //利用数学公式计算1到x的和
            public void Sum1ToXWithFormula(int x)
            {
                Console.WriteLine("数学公式求1到X的和为：{0}",(1+x)*x/2);
            }
        }
    }
}
