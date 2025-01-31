using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

class Program
{
    static void Main()
    {
        Vector128<int> v1 = Vector128.Create(1, 2, 3, 4);
        Vector128<int> v2 = Vector128.Create(5, 6, 7, 8);

        Vector128<int> r = Ssse3.UnpackHigh(v1, v2);

        int[] bb = new int[4];
        r.CopyTo(bb);
        Console.WriteLine(string.Join(", ",bb));
    }

   
}