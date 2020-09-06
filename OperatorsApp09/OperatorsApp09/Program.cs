// using System;
//
// namespace OperatorsAppl
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             int a = 21;
//             int b = 10;
//             int c;
//
//             c = a + b;
//             Console.WriteLine("Line 1 - c 的值是 {0}", c);
//             c = a - b;
//             Console.WriteLine("Line 2 - c 的值是 {0}", c);
//             c = a * b;
//             Console.WriteLine("Line 3 - c 的值是 {0}", c);
//             c = a / b;
//             Console.WriteLine("Line 4 - c 的值是 {0}", c);
//             c = a % b;
//             Console.WriteLine("Line 5 - c 的值是 {0}", c);
//
//             // ++a 先进行自增运算再赋值
//             c = ++a;
//             Console.WriteLine("Line 6 - c 的值是 {0}", c);
//
//             // 此时 a 的值为 22
//             // --a 先进行自减运算再赋值
//             c = --a;
//             Console.WriteLine("Line 7 - c 的值是 {0}", c);
//             Console.ReadLine();
//         }
//     }
// }



// using System;
//
// namespace OperatorsAppl
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             int a = 1;
//             int b;
//
//             // a++ 先赋值再进行自增运算
//             b = a++;
//             Console.WriteLine("a = {0}", a);
//             Console.WriteLine("b = {0}", b);
//             Console.ReadLine();
//
//             // ++a 先进行自增运算再赋值
//             a = 1; // 重新初始化 a
//             b = ++a;
//             Console.WriteLine("a = {0}", a);
//             Console.WriteLine("b = {0}", b);
//             Console.ReadLine();
//
//             // a-- 先赋值再进行自减运算
//             a = 1;  // 重新初始化 a
//             b= a--;
//             Console.WriteLine("a = {0}", a);
//             Console.WriteLine("b = {0}", b);
//             Console.ReadLine();
//
//             // --a 先进行自减运算再赋值
//             a = 1;  // 重新初始化 a
//             b= --a;
//             Console.WriteLine("a = {0}", a);
//             Console.WriteLine("b = {0}", b);
//             Console.ReadLine();
//         }
//     }
// }



// using System;
//
// class Program
// {
//     static void Main(string[] args)
//     {
//         int a = 21;
//         int b = 10;
//      
//         if (a == b)
//         {
//             Console.WriteLine("Line 1 - a 等于 b");
//         }
//         else
//         {
//             Console.WriteLine("Line 1 - a 不等于 b");
//         }
//         if (a < b)
//         {
//             Console.WriteLine("Line 2 - a 小于 b");
//         }
//         else
//         {
//             Console.WriteLine("Line 2 - a 不小于 b");
//         }
//         if (a > b)
//         {
//             Console.WriteLine("Line 3 - a 大于 b");
//         }
//         else
//         {
//             Console.WriteLine("Line 3 - a 不大于 b");
//         }
//         /* 改变 a 和 b 的值 */
//         a = 5;
//         b = 20;
//         if (a <= b)
//         {
//             Console.WriteLine("Line 4 - a 小于或等于 b");
//         }
//         if (b >= a)
//         {
//             Console.WriteLine("Line 5 - b 大于或等于 a");
//         }
//     }
// }




// using System;
//
// namespace OperatorsAppl
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             bool a = true;
//             bool b = true;
//            
//             if (a && b)
//             {
//                 Console.WriteLine("Line 1 - 条件为真");
//             }
//             if (a || b)
//             {
//                 Console.WriteLine("Line 2 - 条件为真");
//             }
//             /* 改变 a 和 b 的值 */
//             a = false;
//             b = true;
//             if (a && b)
//             {
//                 Console.WriteLine("Line 3 - 条件为真");
//             }
//             else
//             {
//                 Console.WriteLine("Line 3 - 条件不为真");
//             }
//             if (!(a && b))
//             {
//                 Console.WriteLine("Line 4 - 条件为真");
//             }
//             Console.ReadLine();
//         }
//     }
// }





// using System;
// namespace OperatorsAppl
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             int a = 60;            /* 60 = 0011 1100 */  
//             int b = 13;            /* 13 = 0000 1101 */
//             int c = 0;          
//
//             c = a & b;           /* 12 = 0000 1100 */
//             Console.WriteLine("Line 1 - c 的值是 {0}", c );
//
//             c = a | b;           /* 61 = 0011 1101 */
//             Console.WriteLine("Line 2 - c 的值是 {0}", c);
//
//             c = a ^ b;           /* 49 = 0011 0001 */
//             Console.WriteLine("Line 3 - c 的值是 {0}", c);
//
//             c = ~a;               /*-61 = 1100 0011 */
//             Console.WriteLine("Line 4 - c 的值是 {0}", c);
//
//             c = a << 2;     /* 240 = 1111 0000 */
//             Console.WriteLine("Line 5 - c 的值是 {0}", c);
//
//             c = a >> 2;     /* 15 = 0000 1111 */
//             Console.WriteLine("Line 6 - c 的值是 {0}", c);
//             Console.ReadLine();
//         }
//     }
// }


// using System;
//
// namespace OperatorsAppl
// {
//     class Program
//     {
//         static void Main(string[] args)
//         {
//             int a = 21;
//             int c;
//
//             c = a;
//             Console.WriteLine("Line 1 - =  c 的值 = {0}", c);
//
//             c += a;
//             Console.WriteLine("Line 2 - += c 的值 = {0}", c);
//
//             c -= a;
//             Console.WriteLine("Line 3 - -=  c 的值 = {0}", c);
//
//             c *= a;
//             Console.WriteLine("Line 4 - *=  c 的值 = {0}", c);
//
//             c /= a;
//             Console.WriteLine("Line 5 - /=  c 的值 = {0}", c);
//
//             c = 200;
//             c %= a;
//             Console.WriteLine("Line 6 - %=  c 的值 = {0}", c);
//
//             c <<= 2;
//             Console.WriteLine("Line 7 - <<=  c 的值 = {0}", c);
//
//             c >>= 2;
//             Console.WriteLine("Line 8 - >>=  c 的值 = {0}", c);
//
//             c &= 2;
//             Console.WriteLine("Line 9 - &=  c 的值 = {0}", c);
//
//             c ^= 2;
//             Console.WriteLine("Line 10 - ^=  c 的值 = {0}", c);
//
//             c |= 2;
//             Console.WriteLine("Line 11 - |=  c 的值 = {0}", c);
//             Console.ReadLine();
//         }
//     }
// }


using System;

namespace OperatorsAppl
{
   
    class Program
    {
        static void Main(string[] args)
        {
         
            /* sizeof 运算符的实例 */
            Console.WriteLine("int 的大小是 {0}", sizeof(int));
            Console.WriteLine("short 的大小是 {0}", sizeof(short));
            Console.WriteLine("double 的大小是 {0}", sizeof(double));
         
            /* 三元运算符的实例 */
            int a, b;
            a = 10;
            b = (a == 1) ? 20 : 30;
            Console.WriteLine("b 的值是 {0}", b);

            b = (a == 10) ? 20 : 30;
            Console.WriteLine("b 的值是 {0}", b);
            Console.ReadLine();
        }
    }
}