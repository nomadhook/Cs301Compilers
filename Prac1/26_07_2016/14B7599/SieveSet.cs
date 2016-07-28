// Sieve of Eratosthenes making use of a set rather than an array
// P.D. Terry,  Rhodes University, 2016

  using Library;

  class SieveSet {

    public static void Main(string [] args) {
      IntSet primeSet = null;
      IO.Write("How many iterations? ");
      int iterations = IO.ReadInt();
      bool display = iterations == 1;
      int limit = IO.ReadInt("Supply largest number to be tested ");
      for (int i = 1; i <= iterations; i++)
        primeSet = Primes(limit);
      if (display) IO.WriteLine(primeSet);
      IO.WriteLine(primeSet.Members() + " primes");
    } // Main

    static IntSet Primes(int max)  {
    // Returns the set of prime numbers smaller than max
      IntSet primeSet = new IntSet();   // the prime numbers
      IntSet crossed  = new IntSet();   // the sieve
      for (int i = 2; i <= max; i++) {  // the passes over the sieve
        if (!crossed.Contains(i)) {
          primeSet.Incl(i);
          int k = i;                    // now cross out multiples of i
          do {
            crossed.Incl(k);
            k += i;
          } while (k <= max && k > 0);
        }
      }
      return primeSet;
    } // Primes

  } // SieveSet
