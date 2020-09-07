class ForEachTest
{
    static void Main(string[] args)
    {
        int[] fibarray = new int[] { 0, 1, 1, 2, 3, 5, 8, 13 };
        foreach (int element in fibarray)
        {
            System.Console.WriteLine(element);
        }
        System.Console.WriteLine();


        // 类似 foreach 循环
        for (int i = 0; i < fibarray.Length; i++)
        {
            System.Console.WriteLine(fibarray[i]);
        }
        System.Console.WriteLine();


        // 设置集合中元素的计算器
        int count = 0;
        foreach (int element in fibarray)
        {
            count += 1;
            System.Console.WriteLine("Element #{0}: {1}", count, element);
        }
        System.Console.WriteLine("Number of elements in the array: {0}", count);
    }
}