// Sieve of Eratosthenes making use of one set rather than an array of Booleans
// Merely count the primes
// P.D. Terry,  Rhodes University, 2016

  using Library;

  class PrimeCount {

    public static void Main(string [] args) {
      IO.Write("How many iterations? ");
      int iterations = IO.ReadInt();
      int limit = IO.ReadInt("Supply largest number to be tested ");
      int count = 0;
      for (int i = 1; i <= iterations; i++)
        count = NumberOfPrimes(limit);
      IO.WriteLine(count + " primes");
    } // Main

    static int NumberOfPrimes(int max)  {
    // Returns the number of prime numbers smaller than max
      int count = 0;
      IntSet crossed  = new IntSet();   // the sieve
      for (int i = 2; i <= max; i++) {  // the passes over the sieve
        if (!crossed.Contains(i)) {
          count++;
          int k = i;                    // now cross out multiples of i
          do {
            crossed.Incl(k);
            k += i;
          } while (k <= max && k > 0);
        }
      }
      return count;
    } // NumberOfPrimes

  } // PrimeCount
