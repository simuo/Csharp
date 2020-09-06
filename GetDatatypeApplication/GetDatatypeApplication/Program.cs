﻿using System;

namespace GetDatatypeApplication
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("size of int:{0}",sizeof(int));
            Console.WriteLine("size of byte:{0}",sizeof(byte));
            Console.WriteLine("size of float:{0}",sizeof(float));
            Console.WriteLine("size of double:{0}",sizeof(double));
            Console.WriteLine("size of long:{0}",sizeof(long));
            Console.WriteLine("size of char:{0}",sizeof(char));
            // Console.ReadKey();
        }
    }
}