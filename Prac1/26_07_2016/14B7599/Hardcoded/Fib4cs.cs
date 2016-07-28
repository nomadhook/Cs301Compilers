// Print a table of Fibonacci numbers using (slow) recursive definition
// P.D. Terry,  Rhodes University, 2016

using Library;

class Fib4 {

  static int[] fibmem;

  static int fib(int m) {
  // Compute m-th term in Fibonacci series 0,1,1,2 ...
    if (m == 0) return 0;
    if (m == 1) return 1;
    if (fibmem[m] == 0)
      fibmem[m] = fib(m-1) + fib(m-2);
    return fibmem[m];
  } // fib

  static void Main() {
    fibmem = new int[4000];
    int limit;
    IO.Write("Supply upper limit ");
    limit = IO.ReadInt();
    int i = 0;
    while (i <= limit) {
      fibmem[i] = 0;
      i = i + 1;
    }
    i = limit;
    while (i >= 0) {
      IO.WriteLine(i + "\t" + fib(i));
      i = i - 1;
    } // while
  } // Main

} // Fib4
