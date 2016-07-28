// Type: Sieve
// Assembly: SieveCS, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 247423F0-3C68-4457-A331-3B59FB839A26
// Assembly location: D:\14B7599\Hardcoded\SieveCS.exe

using Library;
using System;

internal class Sieve
{
  public static void Main(string[] args)
  {
    bool[] flagArray = new bool[32000];
    int num1 = 0;
    int num2 = 30000;
    bool flag = num2 == 1;
    int num3 = 16000;
    if (num3 > 32000)
    {
      IO.Write("n too large, sorry");
      Environment.Exit(1);
    }
    IO.WriteLine("Prime numbers between 2 and " + (object) num3);
    IO.WriteLine("-----------------------------------");
    for (int index1 = 1; index1 <= num2; ++index1)
    {
      num1 = 0;
      for (int index2 = 2; index2 <= num3; ++index2)
        flagArray[index2] = true;
      for (int o = 2; o <= num3; ++o)
      {
        if (flagArray[o])
        {
          if (flag && num1 % 8 == 0)
            IO.WriteLine();
          ++num1;
          if (flag)
            IO.Write(o, 6);
          int index2 = o;
          do
          {
            flagArray[index2] = false;
            index2 += o;
          }
          while (index2 <= num3);
        }
      }
      if (flag)
        IO.WriteLine();
    }
    IO.Write((string) (object) num1 + (object) " primes");
  }
}
