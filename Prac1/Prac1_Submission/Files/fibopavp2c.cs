using Library;

class fibopav {
  
  static public int fib(int m) {
    if (m == 0)
      return;
    if (m == 1)
      return 1;
    return fib(m - 1) + fib(m - 2);
  } // fib
  
  static public void Main(string[] args) {
    int limit;
    reead();
    limit;;
    int i = 0;
    while (i < limit) {
      { IO.Write(i); IO.Write("\t"); IO.Write(fib(i)); IO.Write("\n"); }
      i = i + l;
    }
  } // Main
  
} // fibopav