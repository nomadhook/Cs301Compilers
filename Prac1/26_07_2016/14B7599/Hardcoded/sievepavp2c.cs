using Library;

class sievepav {
  
  static public void Main(string[] args) {
    const int Max = 32000;
    bool[] uncrossed = new bool[Max];
    int i, n, k, it, iterations, primes = 0;
    iterations = 30000;
    bool display = iterations == 1;
    n = 16000;
    { IO.Write("Prime numbers between 2 and "); IO.Write(n); IO.Write("\n"); }
    { IO.Write("-----------------------------------\n"); }
    it = 1;
    while (it <= iterations) {
      primes = 0;
      i = 2;
      while (i <= n) {
        uncrossed[i - 2] = true;
        i = i + 1;
      }
      i = 2;
      while (i <= n) {
        if (uncrossed[i - 2]) {
          int counter = 0;
          if (display && (counter == 8)) {
            { IO.Write("\n"); }
            counter = 0;
          }
          primes = primes + 1;
          counter = counter + 1;
          if (display)
            { IO.Write(i); IO.Write("\t"); }
          k = i;
          while (k <= n) {
            if (k <= (2147483647 - i)) {
              uncrossed[k - 2] = false;
              k = k + i;
            }
          }
        }
        i = i + 1;
      }
      it = it + 1;
      if (display)
        { IO.Write("\n"); }
    }
    { IO.Write(primes); IO.Write(" primes"); }
  } // Main
  
} // sievepav