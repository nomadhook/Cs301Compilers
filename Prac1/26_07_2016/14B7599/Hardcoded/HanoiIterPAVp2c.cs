using Library;

class HanoiIterPAV {
  static const int infinite = 70;
  
  static public bool display;
  
  static public void Move(int[] from, int[] []) {
    if (display)
      { IO.Write("Move "); IO.Write(from[2 + from[1]]); IO.Write(" from "); IO.Write(from[0]); IO.Write(" to "); IO.Write(); IO.Write(0); IO.Write("\n"); }
  } // Move
  
  static public void Show(int[] a) {
    { IO.Write(a[0]); IO.Write(a[1]); IO.Write(a[2]); }
    int i = 1;
    while (i <= a[1]) {
      { IO.Write(a[2 + i]); }
      i = i + 1;
    }
    { IO.Write("\n"); }
  } // Show
  
  static public int Hanoi(int[] a, int[] b, int[] c, int n) {
    a[0] = 1;
    a[1] = n;
    a[2] = infinite;
    b[0] = 2;
    b[1] = 0;
    b[2] = infinite;
    c[0] = 3;
    c[1] = 0;
    c[2] = infinite;
    if (n / 2 * 2 == n) {
      b[0] = 3;
      c[0] = 2;
    }
    int i = 1;
    while (i <= n) {
      a[i + 2] = n + 1 - i;
      i = i + 1;
    }
    Move(a, b);
    int count = 1;
    while (a[1] + c[1] > 0) {
      bool atoc = a[2 + a[1]] < c[2 + c[1]];
      if (atoc)
        Move(a, c);
      if ( !atoc)
        Move(c, a);
      Move(b, c);
      count = count + 2;
      int[] safe = a;
      a = b;
      b = c;
      c = safe;
    }
    return count;
  } // Hanoi
  
  static public void Main(string[] args) {
    int[] a = new int[infinite + 3], b = new int[infinite + 3], c = new int[infinite + 3];
    int n = 16;
    int iter = 1;
    display = iter == 1;
    while (iter > 0) {
      { IO.Write(Hanoi(a, b, c, n)); IO.Write(" moves \n"); }
      iter = iter - 1;
    }
  } // Main
  
} // HanoiIterPAV