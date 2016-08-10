
using System;
using System.IO;
using System.Text;

namespace EBNFR {

public class Parser {
	public const int _EOF = 0;
	public const int _letter = 1;
	public const int _digit = 2;
	// terminals
	public const int EOF_SYM = 0;
	public const int letter_Sym = 1;
	public const int digit_Sym = 2;
	public const int equal_Sym = 3;
	public const int point_Sym = 4;
	public const int bar_Sym = 5;
	public const int lbrack_Sym = 6;
	public const int rbrack_Sym = 7;
	public const int lparen_Sym = 8;
	public const int rparen_Sym = 9;
	public const int lbrace_Sym = 10;
	public const int rbrace_Sym = 11;
	public const int underscore_Sym = 12;
	public const int squote_Sym = 13;
	public const int dquote_Sym = 14;
	public const int accent_Sym = 15;
	public const int tilde_Sym = 16;
	public const int bang_Sym = 17;
	public const int at_Sym = 18;
	public const int hash_Sym = 19;
	public const int dollar_Sym = 20;
	public const int percent_Sym = 21;
	public const int uparrow_Sym = 22;
	public const int and_Sym = 23;
	public const int star_Sym = 24;
	public const int plus_Sym = 25;
	public const int minus_Sym = 26;
	public const int colon_Sym = 27;
	public const int semicolon_Sym = 28;
	public const int comma_Sym = 29;
	public const int less_Sym = 30;
	public const int greater_Sym = 31;
	public const int slash_Sym = 32;
	public const int query_Sym = 33;
	public const int NOT_SYM = 34;
	// pragmas

	public const int maxT = 34;

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
		if (la.kind == EOF_SYM) {
			Get();
		} else if (la.kind == letter_Sym) {
			Production();
			EBNFR();
		} else SynErr(35);
	}

	static void Production() {
		nonterminal();
		Expect(equal_Sym);
		Expression();
		Expect(point_Sym);
	}

	static void nonterminal() {
		Expect(letter_Sym);
		nonterminalR();
	}

	static void Expression() {
		Term();
		if (la.kind == bar_Sym) {
			Get();
			Expression();
		} else if (StartOf(1)) {
		} else SynErr(36);
	}

	static void Term() {
		Factor();
		if (StartOf(2)) {
			Term();
		} else if (StartOf(3)) {
		} else SynErr(37);
	}

	static void Factor() {
		if (la.kind == letter_Sym) {
			nonterminal();
		} else if (la.kind == squote_Sym || la.kind == dquote_Sym) {
			terminal();
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
		} else SynErr(38);
	}

	static void terminal() {
		if (la.kind == squote_Sym) {
			Get();
			nq1Terminal();
		} else if (la.kind == dquote_Sym) {
			Get();
			nq2Terminal();
		} else SynErr(39);
	}

	static void nonterminalR() {
		if (la.kind == letter_Sym || la.kind == digit_Sym || la.kind == underscore_Sym) {
			if (la.kind == letter_Sym) {
				Get();
			} else if (la.kind == underscore_Sym) {
				Get();
			} else {
				Get();
			}
			nonterminalR();
		} else if (StartOf(4)) {
		} else SynErr(40);
	}

	static void nq1Terminal() {
		noQuote();
		if (la.kind == squote_Sym) {
			Get();
		} else if (StartOf(5)) {
			nq1Terminal();
		} else SynErr(41);
	}

	static void nq2Terminal() {
		noQuote();
		if (la.kind == dquote_Sym) {
			Get();
		} else if (StartOf(5)) {
			nq2Terminal();
		} else SynErr(42);
	}

	static void noQuote() {
		if (la.kind == letter_Sym) {
			Get();
		} else if (la.kind == digit_Sym) {
			Get();
		} else if (StartOf(6)) {
			symbol();
		} else SynErr(43);
	}

	static void symbol() {
		switch (la.kind) {
		case accent_Sym: {
			Get();
			break;
		}
		case tilde_Sym: {
			Get();
			break;
		}
		case bang_Sym: {
			Get();
			break;
		}
		case at_Sym: {
			Get();
			break;
		}
		case hash_Sym: {
			Get();
			break;
		}
		case dollar_Sym: {
			Get();
			break;
		}
		case percent_Sym: {
			Get();
			break;
		}
		case uparrow_Sym: {
			Get();
			break;
		}
		case and_Sym: {
			Get();
			break;
		}
		case star_Sym: {
			Get();
			break;
		}
		case lparen_Sym: {
			Get();
			break;
		}
		case rparen_Sym: {
			Get();
			break;
		}
		case underscore_Sym: {
			Get();
			break;
		}
		case plus_Sym: {
			Get();
			break;
		}
		case minus_Sym: {
			Get();
			break;
		}
		case equal_Sym: {
			Get();
			break;
		}
		case lbrack_Sym: {
			Get();
			break;
		}
		case rbrack_Sym: {
			Get();
			break;
		}
		case lbrace_Sym: {
			Get();
			break;
		}
		case rbrace_Sym: {
			Get();
			break;
		}
		case bar_Sym: {
			Get();
			break;
		}
		case colon_Sym: {
			Get();
			break;
		}
		case semicolon_Sym: {
			Get();
			break;
		}
		case comma_Sym: {
			Get();
			break;
		}
		case less_Sym: {
			Get();
			break;
		}
		case point_Sym: {
			Get();
			break;
		}
		case greater_Sym: {
			Get();
			break;
		}
		case slash_Sym: {
			Get();
			break;
		}
		case query_Sym: {
			Get();
			break;
		}
		default: SynErr(44); break;
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
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, T,x,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,x,x, x,x,T,x, T,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, T,T,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,x,T, T,T,T,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x},
		{x,x,x,T, T,T,T,T, T,T,T,T, T,x,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x}

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
			case 1: s = "letter expected"; break;
			case 2: s = "digit expected"; break;
			case 3: s = "\"=\" expected"; break;
			case 4: s = "\".\" expected"; break;
			case 5: s = "\"|\" expected"; break;
			case 6: s = "\"[\" expected"; break;
			case 7: s = "\"]\" expected"; break;
			case 8: s = "\"(\" expected"; break;
			case 9: s = "\")\" expected"; break;
			case 10: s = "\"{\" expected"; break;
			case 11: s = "\"}\" expected"; break;
			case 12: s = "\"_\" expected"; break;
			case 13: s = "\"\'\" expected"; break;
			case 14: s = "\"\"\" expected"; break;
			case 15: s = "\"`\" expected"; break;
			case 16: s = "\"~\" expected"; break;
			case 17: s = "\"!\" expected"; break;
			case 18: s = "\"@\" expected"; break;
			case 19: s = "\"#\" expected"; break;
			case 20: s = "\"$\" expected"; break;
			case 21: s = "\"%\" expected"; break;
			case 22: s = "\"^\" expected"; break;
			case 23: s = "\"&\" expected"; break;
			case 24: s = "\"*\" expected"; break;
			case 25: s = "\"+\" expected"; break;
			case 26: s = "\"-\" expected"; break;
			case 27: s = "\":\" expected"; break;
			case 28: s = "\";\" expected"; break;
			case 29: s = "\",\" expected"; break;
			case 30: s = "\"<\" expected"; break;
			case 31: s = "\">\" expected"; break;
			case 32: s = "\"/\" expected"; break;
			case 33: s = "\"?\" expected"; break;
			case 34: s = "??? expected"; break;
			case 35: s = "invalid EBNFR"; break;
			case 36: s = "invalid Expression"; break;
			case 37: s = "invalid Term"; break;
			case 38: s = "invalid Factor"; break;
			case 39: s = "invalid terminal"; break;
			case 40: s = "invalid nonterminalR"; break;
			case 41: s = "invalid nq1Terminal"; break;
			case 42: s = "invalid nq2Terminal"; break;
			case 43: s = "invalid noQuote"; break;
			case 44: s = "invalid symbol"; break;

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
