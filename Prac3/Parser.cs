
using System;
using System.IO;
using System.Text;

namespace EBNFR {

public class Parser {
	public const int _EOF = 0;
	public const int _terminal = 1;
	// terminals
	public const int EOF_SYM = 0;
	public const int terminal_Sym = 1;
	public const int equal_Sym = 2;
	public const int point_Sym = 3;
	public const int bar_Sym = 4;
	public const int lbrack_Sym = 5;
	public const int rbrack_Sym = 6;
	public const int lparen_Sym = 7;
	public const int rparen_Sym = 8;
	public const int lbrace_Sym = 9;
	public const int rbrace_Sym = 10;
	public const int underscore_Sym = 11;
	public const int d0_Sym = 12;
	public const int d1_Sym = 13;
	public const int d2_Sym = 14;
	public const int d3_Sym = 15;
	public const int d4_Sym = 16;
	public const int d5_Sym = 17;
	public const int d6_Sym = 18;
	public const int d7_Sym = 19;
	public const int d8_Sym = 20;
	public const int d9_Sym = 21;
	public const int A_Sym = 22;
	public const int B_Sym = 23;
	public const int C_Sym = 24;
	public const int D_Sym = 25;
	public const int E_Sym = 26;
	public const int F_Sym = 27;
	public const int G_Sym = 28;
	public const int H_Sym = 29;
	public const int I_Sym = 30;
	public const int J_Sym = 31;
	public const int K_Sym = 32;
	public const int L_Sym = 33;
	public const int M_Sym = 34;
	public const int N_Sym = 35;
	public const int O_Sym = 36;
	public const int P_Sym = 37;
	public const int Q_Sym = 38;
	public const int R_Sym = 39;
	public const int S_Sym = 40;
	public const int T_Sym = 41;
	public const int U_Sym = 42;
	public const int V_Sym = 43;
	public const int W_Sym = 44;
	public const int X_Sym = 45;
	public const int Y_Sym = 46;
	public const int Z_Sym = 47;
	public const int a_Sym = 48;
	public const int b_Sym = 49;
	public const int c_Sym = 50;
	public const int d_Sym = 51;
	public const int e_Sym = 52;
	public const int f_Sym = 53;
	public const int g_Sym = 54;
	public const int h_Sym = 55;
	public const int i_Sym = 56;
	public const int j_Sym = 57;
	public const int k_Sym = 58;
	public const int l_Sym = 59;
	public const int m_Sym = 60;
	public const int n_Sym = 61;
	public const int o_Sym = 62;
	public const int p_Sym = 63;
	public const int q_Sym = 64;
	public const int r_Sym = 65;
	public const int s_Sym = 66;
	public const int t_Sym = 67;
	public const int u_Sym = 68;
	public const int v_Sym = 69;
	public const int w_Sym = 70;
	public const int x_Sym = 71;
	public const int y_Sym = 72;
	public const int z_Sym = 73;
	public const int NOT_SYM = 74;
	// pragmas

	public const int maxT = 74;

	const bool T = true;
	const bool x = false;
	const int minErrDist = 2;

	public static Token token;    // last recognized token   /* pdt */
	public static Token la;       // lookahead token
	static int errDist = minErrDist;

	

	static void SynErr (int n) {
		if (errDist >= minErrDist) Errors.SynErr(la.line, la.col, n);
		errDist = 0;
	}

	public static void SemErr (string msg) {
		if (errDist >= minErrDist) Errors.Error(token.line, token.col, msg); /* pdt */
		errDist = 0;
	}

	public static void SemError (string msg) {
		if (errDist >= minErrDist) Errors.Error(token.line, token.col, msg); /* pdt */
		errDist = 0;
	}

	public static void Warning (string msg) { /* pdt */
		if (errDist >= minErrDist) Errors.Warn(token.line, token.col, msg);
		errDist = 2; //++ 2009/11/04
	}

	public static bool Successful() { /* pdt */
		return Errors.count == 0;
	}

	public static string LexString() { /* pdt */
		return token.val;
	}

	public static string LookAheadString() { /* pdt */
		return la.val;
	}

	static void Get () {
		for (;;) {
			token = la; /* pdt */
			la = Scanner.Scan();
			if (la.kind <= maxT) { ++errDist; break; }

			la = token; /* pdt */
		}
	}

	static void Expect (int n) {
		if (la.kind==n) Get(); else { SynErr(n); }
	}

	static bool StartOf (int s) {
		return set[s, la.kind];
	}

	static void ExpectWeak (int n, int follow) {
		if (la.kind == n) Get();
		else {
			SynErr(n);
			while (!StartOf(follow)) Get();
		}
	}

	static bool WeakSeparator (int n, int syFol, int repFol) {
		bool[] s = new bool[maxT+1];
		if (la.kind == n) { Get(); return true; }
		else if (StartOf(repFol)) return false;
		else {
			for (int i=0; i <= maxT; i++) {
				s[i] = set[syFol, i] || set[repFol, i] || set[0, i];
			}
			SynErr(n);
			while (!s[la.kind]) Get();
			return StartOf(syFol);
		}
	}

	static void EBNFR() {
		if (StartOf(1)) {
			Production();
			EBNFR();
		} else if (la.kind == EOF_SYM) {
			Get();
		} else SynErr(75);
	}

	static void Production() {
		nonterminal();
		Expect(equal_Sym);
		Expression();
		Expect(point_Sym);
	}

	static void nonterminal() {
		if (StartOf(1)) {
			letter();
		} else if (StartOf(1)) {
			letter();
			nonterminalR();
		} else SynErr(76);
	}

	static void Expression() {
		if (StartOf(2)) {
			Term();
		} else if (StartOf(2)) {
			Term();
			Expect(bar_Sym);
			Expression();
		} else SynErr(77);
	}

	static void Term() {
		if (StartOf(2)) {
			Factor();
		} else if (StartOf(2)) {
			Factor();
			Term();
		} else SynErr(78);
	}

	static void Factor() {
		if (StartOf(1)) {
			nonterminal();
		} else if (la.kind == terminal_Sym) {
			Get();
		} else if (la.kind == lbrack_Sym) {
			Get();
			Expression();
			Expect(rbrack_Sym);
		} else if (la.kind == lparen_Sym) {
			Get();
			Expression();
			Expect(rparen_Sym);
		} else if (la.kind == lbrace_Sym) {
			Get();
			Expression();
			Expect(rbrace_Sym);
		} else SynErr(79);
	}

	static void letter() {
		switch (la.kind) {
		case A_Sym: {
			Get();
			break;
		}
		case B_Sym: {
			Get();
			break;
		}
		case C_Sym: {
			Get();
			break;
		}
		case D_Sym: {
			Get();
			break;
		}
		case E_Sym: {
			Get();
			break;
		}
		case F_Sym: {
			Get();
			break;
		}
		case G_Sym: {
			Get();
			break;
		}
		case H_Sym: {
			Get();
			break;
		}
		case I_Sym: {
			Get();
			break;
		}
		case J_Sym: {
			Get();
			break;
		}
		case K_Sym: {
			Get();
			break;
		}
		case L_Sym: {
			Get();
			break;
		}
		case M_Sym: {
			Get();
			break;
		}
		case N_Sym: {
			Get();
			break;
		}
		case O_Sym: {
			Get();
			break;
		}
		case P_Sym: {
			Get();
			break;
		}
		case Q_Sym: {
			Get();
			break;
		}
		case R_Sym: {
			Get();
			break;
		}
		case S_Sym: {
			Get();
			break;
		}
		case T_Sym: {
			Get();
			break;
		}
		case U_Sym: {
			Get();
			break;
		}
		case V_Sym: {
			Get();
			break;
		}
		case W_Sym: {
			Get();
			break;
		}
		case X_Sym: {
			Get();
			break;
		}
		case Y_Sym: {
			Get();
			break;
		}
		case Z_Sym: {
			Get();
			break;
		}
		case a_Sym: {
			Get();
			break;
		}
		case b_Sym: {
			Get();
			break;
		}
		case c_Sym: {
			Get();
			break;
		}
		case d_Sym: {
			Get();
			break;
		}
		case e_Sym: {
			Get();
			break;
		}
		case f_Sym: {
			Get();
			break;
		}
		case g_Sym: {
			Get();
			break;
		}
		case h_Sym: {
			Get();
			break;
		}
		case i_Sym: {
			Get();
			break;
		}
		case j_Sym: {
			Get();
			break;
		}
		case k_Sym: {
			Get();
			break;
		}
		case l_Sym: {
			Get();
			break;
		}
		case m_Sym: {
			Get();
			break;
		}
		case n_Sym: {
			Get();
			break;
		}
		case o_Sym: {
			Get();
			break;
		}
		case p_Sym: {
			Get();
			break;
		}
		case q_Sym: {
			Get();
			break;
		}
		case r_Sym: {
			Get();
			break;
		}
		case s_Sym: {
			Get();
			break;
		}
		case t_Sym: {
			Get();
			break;
		}
		case u_Sym: {
			Get();
			break;
		}
		case v_Sym: {
			Get();
			break;
		}
		case w_Sym: {
			Get();
			break;
		}
		case x_Sym: {
			Get();
			break;
		}
		case y_Sym: {
			Get();
			break;
		}
		case z_Sym: {
			Get();
			break;
		}
		default: SynErr(80); break;
		}
	}

	static void nonterminalR() {
		if (StartOf(3)) {
			if (StartOf(1)) {
				letter();
			} else if (la.kind == underscore_Sym) {
				Get();
			} else {
				digit();
			}
		} else if (StartOf(3)) {
			if (StartOf(1)) {
				letter();
			} else if (la.kind == underscore_Sym) {
				Get();
			} else {
				digit();
			}
			nonterminalR();
		} else SynErr(81);
	}

	static void digit() {
		switch (la.kind) {
		case d0_Sym: {
			Get();
			break;
		}
		case d1_Sym: {
			Get();
			break;
		}
		case d2_Sym: {
			Get();
			break;
		}
		case d3_Sym: {
			Get();
			break;
		}
		case d4_Sym: {
			Get();
			break;
		}
		case d5_Sym: {
			Get();
			break;
		}
		case d6_Sym: {
			Get();
			break;
		}
		case d7_Sym: {
			Get();
			break;
		}
		case d8_Sym: {
			Get();
			break;
		}
		case d9_Sym: {
			Get();
			break;
		}
		default: SynErr(82); break;
		}
	}



	public static void Parse() {
		la = new Token();
		la.val = "";
		Get();
		EBNFR();
		Expect(EOF_SYM);

	}

	static bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x},
		{x,T,x,x, x,T,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x}

	};

} // end Parser

/* pdt - considerable extension from here on */

public class ErrorRec {
	public int line, col, num;
	public string str;
	public ErrorRec next;

	public ErrorRec(int l, int c, string s) {
		line = l; col = c; str = s; next = null;
	}

} // end ErrorRec

public class Errors {

	public static int count = 0;                                     // number of errors detected
	public static int warns = 0;                                     // number of warnings detected
	public static string errMsgFormat = "file {0} : ({1}, {2}) {3}"; // 0=file 1=line, 2=column, 3=text
	static string fileName = "";
	static string listName = "";
	static bool mergeErrors = false;
	static StreamWriter mergedList;

	static ErrorRec first = null, last;
	static bool eof = false;

	static string GetLine() {
		char ch, CR = '\r', LF = '\n';
		int l = 0;
		StringBuilder s = new StringBuilder();
		ch = (char) Buffer.Read();
		while (ch != Buffer.EOF && ch != CR && ch != LF) {
			s.Append(ch); l++; ch = (char) Buffer.Read();
		}
		eof = (l == 0 && ch == Buffer.EOF);
		if (ch == CR) {  // check for MS-DOS
			ch = (char) Buffer.Read();
			if (ch != LF && ch != Buffer.EOF) Buffer.Pos--;
		}
		return s.ToString();
	}

	static void Display (string s, ErrorRec e) {
		mergedList.Write("**** ");
		for (int c = 1; c < e.col; c++)
			if (s[c-1] == '\t') mergedList.Write("\t"); else mergedList.Write(" ");
		mergedList.WriteLine("^ " + e.str);
	}

	public static void Init (string fn, string dir, bool merge) {
		fileName = fn;
		listName = dir + "listing.txt";
		mergeErrors = merge;
		if (mergeErrors)
			try {
				mergedList = new StreamWriter(new FileStream(listName, FileMode.Create));
			} catch (IOException) {
				Errors.Exception("-- could not open " + listName);
			}
	}

	public static void Summarize () {
		if (mergeErrors) {
			mergedList.WriteLine();
			ErrorRec cur = first;
			Buffer.Pos = 0;
			int lnr = 1;
			string s = GetLine();
			while (!eof) {
				mergedList.WriteLine("{0,4} {1}", lnr, s);
				while (cur != null && cur.line == lnr) {
					Display(s, cur); cur = cur.next;
				}
				lnr++; s = GetLine();
			}
			if (cur != null) {
				mergedList.WriteLine("{0,4}", lnr);
				while (cur != null) {
					Display(s, cur); cur = cur.next;
				}
			}
			mergedList.WriteLine();
			mergedList.WriteLine(count + " errors detected");
			if (warns > 0) mergedList.WriteLine(warns + " warnings detected");
			mergedList.Close();
		}
		switch (count) {
			case 0 : Console.WriteLine("Parsed correctly"); break;
			case 1 : Console.WriteLine("1 error detected"); break;
			default: Console.WriteLine(count + " errors detected"); break;
		}
		if (warns > 0) Console.WriteLine(warns + " warnings detected");
		if ((count > 0 || warns > 0) && mergeErrors) Console.WriteLine("see " + listName);
	}

	public static void StoreError (int line, int col, string s) {
		if (mergeErrors) {
			ErrorRec latest = new ErrorRec(line, col, s);
			if (first == null) first = latest; else last.next = latest;
			last = latest;
		} else Console.WriteLine(errMsgFormat, fileName, line, col, s);
	}

	public static void SynErr (int line, int col, int n) {
		string s;
		switch (n) {
			case 0: s = "EOF expected"; break;
			case 1: s = "terminal expected"; break;
			case 2: s = "\"=\" expected"; break;
			case 3: s = "\".\" expected"; break;
			case 4: s = "\"|\" expected"; break;
			case 5: s = "\"[\" expected"; break;
			case 6: s = "\"]\" expected"; break;
			case 7: s = "\"(\" expected"; break;
			case 8: s = "\")\" expected"; break;
			case 9: s = "\"{\" expected"; break;
			case 10: s = "\"}\" expected"; break;
			case 11: s = "\"_\" expected"; break;
			case 12: s = "\"0\" expected"; break;
			case 13: s = "\"1\" expected"; break;
			case 14: s = "\"2\" expected"; break;
			case 15: s = "\"3\" expected"; break;
			case 16: s = "\"4\" expected"; break;
			case 17: s = "\"5\" expected"; break;
			case 18: s = "\"6\" expected"; break;
			case 19: s = "\"7\" expected"; break;
			case 20: s = "\"8\" expected"; break;
			case 21: s = "\"9\" expected"; break;
			case 22: s = "\"A\" expected"; break;
			case 23: s = "\"B\" expected"; break;
			case 24: s = "\"C\" expected"; break;
			case 25: s = "\"D\" expected"; break;
			case 26: s = "\"E\" expected"; break;
			case 27: s = "\"F\" expected"; break;
			case 28: s = "\"G\" expected"; break;
			case 29: s = "\"H\" expected"; break;
			case 30: s = "\"I\" expected"; break;
			case 31: s = "\"J\" expected"; break;
			case 32: s = "\"K\" expected"; break;
			case 33: s = "\"L\" expected"; break;
			case 34: s = "\"M\" expected"; break;
			case 35: s = "\"N\" expected"; break;
			case 36: s = "\"O\" expected"; break;
			case 37: s = "\"P\" expected"; break;
			case 38: s = "\"Q\" expected"; break;
			case 39: s = "\"R\" expected"; break;
			case 40: s = "\"S\" expected"; break;
			case 41: s = "\"T\" expected"; break;
			case 42: s = "\"U\" expected"; break;
			case 43: s = "\"V\" expected"; break;
			case 44: s = "\"W\" expected"; break;
			case 45: s = "\"X\" expected"; break;
			case 46: s = "\"Y\" expected"; break;
			case 47: s = "\"Z\" expected"; break;
			case 48: s = "\"a\" expected"; break;
			case 49: s = "\"b\" expected"; break;
			case 50: s = "\"c\" expected"; break;
			case 51: s = "\"d\" expected"; break;
			case 52: s = "\"e\" expected"; break;
			case 53: s = "\"f\" expected"; break;
			case 54: s = "\"g\" expected"; break;
			case 55: s = "\"h\" expected"; break;
			case 56: s = "\"i\" expected"; break;
			case 57: s = "\"j\" expected"; break;
			case 58: s = "\"k\" expected"; break;
			case 59: s = "\"l\" expected"; break;
			case 60: s = "\"m\" expected"; break;
			case 61: s = "\"n\" expected"; break;
			case 62: s = "\"o\" expected"; break;
			case 63: s = "\"p\" expected"; break;
			case 64: s = "\"q\" expected"; break;
			case 65: s = "\"r\" expected"; break;
			case 66: s = "\"s\" expected"; break;
			case 67: s = "\"t\" expected"; break;
			case 68: s = "\"u\" expected"; break;
			case 69: s = "\"v\" expected"; break;
			case 70: s = "\"w\" expected"; break;
			case 71: s = "\"x\" expected"; break;
			case 72: s = "\"y\" expected"; break;
			case 73: s = "\"z\" expected"; break;
			case 74: s = "??? expected"; break;
			case 75: s = "invalid EBNFR"; break;
			case 76: s = "invalid nonterminal"; break;
			case 77: s = "invalid Expression"; break;
			case 78: s = "invalid Term"; break;
			case 79: s = "invalid Factor"; break;
			case 80: s = "invalid letter"; break;
			case 81: s = "invalid nonterminalR"; break;
			case 82: s = "invalid digit"; break;

			default: s = "error " + n; break;
		}
		StoreError(line, col, s);
		count++;
	}

	public static void SemErr (int line, int col, int n) {
		StoreError(line, col, ("error " + n));
		count++;
	}

	public static void Error (int line, int col, string s) {
		StoreError(line, col, s);
		count++;
	}

	public static void Error (string s) {
		if (mergeErrors) mergedList.WriteLine(s); else Console.WriteLine(s);
		count++;
	}

	public static void Warn (int line, int col, string s) {
		StoreError(line, col, s);
		warns++;
	}

	public static void Warn (string s) {
		if (mergeErrors) mergedList.WriteLine(s); else Console.WriteLine(s);
		warns++;
	}

	public static void Exception (string s) {
		Console.WriteLine(s);
		System.Environment.Exit(1);
	}

} // end Errors

} // end namespace
