// Type: Fibo
// Assembly: FiboCS, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 14C1AB78-CA79-4D05-9EBB-54908F86C4F3
// Assembly location: D:\14B7599\Hardcoded\fibocs.exe

using Library;

internal class Fibo
{
  private static int fib(int m)
  {
    if (m == 0)
      return 0;
    if (m == 1)
      return 1;
    else
      return Fibo.fib(m - 1) + Fibo.fib(m - 2);
  }

  public static void Main(string[] args)
  {
    IO.Write("Supply upper limit ");
    int num = IO.ReadInt();
    for (int index = 0; index < num; ++index)
    {
      IO.Write(index, 3);
      IO.WriteLine(Fibo.fib(index), 10);
    }
  }
}
