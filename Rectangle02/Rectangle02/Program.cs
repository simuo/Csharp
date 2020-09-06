﻿using System;

namespace RectangleApplication
{
    class Rectangle
    {
        double length;
        double width;

        public void Acceptdetails()
        {
            length = 10;
            width = 4.2;
        }

        public double GetArea()
        {
            return length * width;
        }

        public void display()
        {
            Console.WriteLine("length:{0}",length);
            Console.WriteLine("width:{0}",width);
            Console.WriteLine("Area=legth*width:{0}={1} x {2}",GetArea(),length,width);
        }
    }

    class ExecuteRectangle
    {
        static void Main(string[] args)
        {
            Rectangle rect=new Rectangle();
            rect.Acceptdetails();
            rect.display();
            Console.ReadLine();
        }
    }
}