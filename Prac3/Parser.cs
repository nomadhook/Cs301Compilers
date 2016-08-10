
using System;
using System.IO;
using System.Text;

namespace Parva {

public class Parser {
	public const int _EOF = 0;
	public const int _identifier = 1;
	public const int _number = 2;
	public const int _stringLit = 3;
	public const int _charLit = 4;
	// terminals
	public const int EOF_SYM = 0;
	public const int identifier_Sym = 1;
	public const int number_Sym = 2;
	public const int stringLit_Sym = 3;
	public const int charLit_Sym = 4;
	public const int void_Sym = 5;
	public const int lparen_Sym = 6;
	public const int rparen_Sym = 7;
	public const int lbrace_Sym = 8;
	public const int rbrace_Sym = 9;
	public const int semicolon_Sym = 10;
	public const int continue_Sym = 11;
	public const int break_Sym = 12;
	public const int const_Sym = 13;
	public const int comma_Sym = 14;
	public const int equal_Sym = 15;
	public const int true_Sym = 16;
	public const int false_Sym = 17;
	public const int null_Sym = 18;
	public const int lbrack_Sym = 19;
	public const int rbrack_Sym = 20;
	public const int if_Sym = 21;
	public const int elsif_Sym = 22;
	public const int else_Sym = 23;
	public const int do_Sym = 24;
	public const int while_Sym = 25;
	public const int for_Sym = 26;
	public const int to_Sym = 27;
	public const int downto_Sym = 28;
	public const int return_Sym = 29;
	public const int halt_Sym = 30;
	public const int read_Sym = 31;
	public const int write_Sym = 32;
	public const int plus_Sym = 33;
	public const int minus_Sym = 34;
	public const int new_Sym = 35;
	public const int bang_Sym = 36;
	public const int lbrackrbrack_Sym = 37;
	public const int int_Sym = 38;
	public const int bool_Sym = 39;
	public const int barbar_Sym = 40;
	public const int star_Sym = 41;
	public const int slash_Sym = 42;
	public const int andand_Sym = 43;
	public const int percent_Sym = 44;
	public const int equalequal_Sym = 45;
	public const int bangequal_Sym = 46;
	public const int less_Sym = 47;
	public const int lessequal_Sym = 48;
	public const int greater_Sym = 49;
	public const int greaterequal_Sym = 50;
	public const int starequal_Sym = 51;
	public const int slashequal_Sym = 52;
	public const int plusequal_Sym = 53;
	public const int minusequal_Sym = 54;
	public const int andequal_Sym = 55;
	public const int barequal_Sym = 56;
	public const int percentequal_Sym = 57;
	public const int NOT_SYM = 58;
	// pragmas

	public const int maxT = 58;

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

	static void Parva() {
		Expect(void_Sym);
		Expect(identifier_Sym);
		Expect(lparen_Sym);
		Expect(rparen_Sym);
		Block();
	}

	static void Block() {
		Expect(lbrace_Sym);
		while (StartOf(1)) {
			Statement();
		}
		Expect(rbrace_Sym);
	}

	static void Statement() {
		switch (la.kind) {
		case lbrace_Sym: {
			Block();
			break;
		}
		case semicolon_Sym: {
			Get();
			break;
		}
		case const_Sym: {
			ConstDeclarations();
			break;
		}
		case int_Sym: case bool_Sym: {
			VarDeclarations();
			break;
		}
		case identifier_Sym: {
			Assignment();
			break;
		}
		case if_Sym: {
			IfStatement();
			break;
		}
		case while_Sym: {
			WhileStatement();
			break;
		}
		case do_Sym: {
			DoWhileStatement();
			break;
		}
		case for_Sym: {
			ForStatement();
			break;
		}
		case return_Sym: {
			ReturnStatement();
			break;
		}
		case halt_Sym: {
			HaltStatement();
			break;
		}
		case read_Sym: {
			ReadStatement();
			break;
		}
		case write_Sym: {
			WriteStatement();
			break;
		}
		case continue_Sym: {
			Get();
			break;
		}
		case break_Sym: {
			Get();
			break;
		}
		default: SynErr(59); break;
		}
	}

	static void ConstDeclarations() {
		Expect(const_Sym);
		OneConst();
		while (la.kind == comma_Sym) {
			Get();
			OneConst();
		}
		Expect(semicolon_Sym);
	}

	static void VarDeclarations() {
		Type();
		OneVar();
		while (la.kind == comma_Sym) {
			Get();
			OneVar();
		}
		Expect(semicolon_Sym);
	}

	static void Assignment() {
		Designator();
		while (la.kind == comma_Sym) {
			Get();
			Designator();
		}
		if (la.kind == equal_Sym) {
			Get();
		} else if (StartOf(2)) {
			AssignmentOP();
		} else SynErr(60);
		Expression();
		while (la.kind == comma_Sym) {
			Get();
			Expression();
		}
		Expect(semicolon_Sym);
	}

	static void IfStatement() {
		Expect(if_Sym);
		Expect(lparen_Sym);
		Condition();
		Expect(rparen_Sym);
		Statement();
		if (la.kind == elsif_Sym || la.kind == else_Sym) {
			Else();
		}
	}

	static void WhileStatement() {
		Expect(while_Sym);
		Expect(lparen_Sym);
		Condition();
		Expect(rparen_Sym);
		Statement();
	}

	static void DoWhileStatement() {
		Expect(do_Sym);
		Statement();
		Expect(while_Sym);
		Expect(lparen_Sym);
		Condition();
		Expect(rparen_Sym);
	}

	static void ForStatement() {
		Expect(for_Sym);
		Expect(identifier_Sym);
		Expect(equal_Sym);
		Expression();
		if (la.kind == to_Sym) {
			Get();
		} else if (la.kind == downto_Sym) {
			Get();
		} else SynErr(61);
		Expression();
		Statement();
	}

	static void ReturnStatement() {
		Expect(return_Sym);
		Expect(semicolon_Sym);
	}

	static void HaltStatement() {
		Expect(halt_Sym);
		Expect(semicolon_Sym);
	}

	static void ReadStatement() {
		Expect(read_Sym);
		Expect(lparen_Sym);
		ReadElement();
		while (la.kind == comma_Sym) {
			Get();
			ReadElement();
		}
		Expect(rparen_Sym);
		Expect(semicolon_Sym);
	}

	static void WriteStatement() {
		Expect(write_Sym);
		Expect(lparen_Sym);
		WriteElement();
		while (la.kind == comma_Sym) {
			Get();
			WriteElement();
		}
		Expect(rparen_Sym);
		Expect(semicolon_Sym);
	}

	static void OneConst() {
		Expect(identifier_Sym);
		Expect(equal_Sym);
		Constant();
	}

	static void Constant() {
		if (la.kind == number_Sym) {
			Get();
		} else if (la.kind == charLit_Sym) {
			Get();
		} else if (la.kind == true_Sym) {
			Get();
		} else if (la.kind == false_Sym) {
			Get();
		} else if (la.kind == null_Sym) {
			Get();
		} else SynErr(62);
	}

	static void Type() {
		BasicType();
		if (la.kind == lbrackrbrack_Sym) {
			Get();
		}
	}

	static void OneVar() {
		Expect(identifier_Sym);
		if (la.kind == equal_Sym) {
			Get();
			Expression();
		}
	}

	static void Expression() {
		AddExp();
		if (StartOf(3)) {
			RelOp();
			AddExp();
		}
	}

	static void Designator() {
		Expect(identifier_Sym);
		if (la.kind == lbrack_Sym) {
			Get();
			Expression();
			Expect(rbrack_Sym);
		}
	}

	static void AssignmentOP() {
		switch (la.kind) {
		case starequal_Sym: {
			Get();
			break;
		}
		case slashequal_Sym: {
			Get();
			break;
		}
		case plusequal_Sym: {
			Get();
			break;
		}
		case minusequal_Sym: {
			Get();
			break;
		}
		case andequal_Sym: {
			Get();
			break;
		}
		case barequal_Sym: {
			Get();
			break;
		}
		case percentequal_Sym: {
			Get();
			break;
		}
		default: SynErr(63); break;
		}
	}

	static void Condition() {
		Expression();
	}

	static void Else() {
		if (la.kind == elsif_Sym) {
			Get();
			Expect(lparen_Sym);
			Condition();
			Expect(rparen_Sym);
			Statement();
			if (la.kind == elsif_Sym || la.kind == else_Sym) {
				Else();
			}
		} else if (la.kind == else_Sym) {
			Get();
			Statement();
		} else SynErr(64);
	}

	static void ReadElement() {
		if (la.kind == stringLit_Sym) {
			Get();
		} else if (la.kind == identifier_Sym) {
			Designator();
		} else SynErr(65);
	}

	static void WriteElement() {
		if (la.kind == stringLit_Sym) {
			Get();
		} else if (StartOf(4)) {
			Expression();
		} else SynErr(66);
	}

	static void AddExp() {
		if (la.kind == plus_Sym || la.kind == minus_Sym) {
			if (la.kind == plus_Sym) {
				Get();
			} else {
				Get();
			}
		}
		Term();
		while (la.kind == plus_Sym || la.kind == minus_Sym || la.kind == barbar_Sym) {
			AddOp();
			Term();
		}
	}

	static void RelOp() {
		switch (la.kind) {
		case equalequal_Sym: {
			Get();
			break;
		}
		case bangequal_Sym: {
			Get();
			break;
		}
		case less_Sym: {
			Get();
			break;
		}
		case lessequal_Sym: {
			Get();
			break;
		}
		case greater_Sym: {
			Get();
			break;
		}
		case greaterequal_Sym: {
			Get();
			break;
		}
		default: SynErr(67); break;
		}
	}

	static void Term() {
		Factor();
		while (StartOf(5)) {
			MulOp();
			Factor();
		}
	}

	static void AddOp() {
		if (la.kind == plus_Sym) {
			Get();
		} else if (la.kind == minus_Sym) {
			Get();
		} else if (la.kind == barbar_Sym) {
			Get();
		} else SynErr(68);
	}

	static void Factor() {
		if (la.kind == identifier_Sym) {
			Designator();
		} else if (StartOf(6)) {
			Constant();
		} else if (la.kind == new_Sym) {
			Get();
			BasicType();
			Expect(lbrack_Sym);
			Expression();
			Expect(rbrack_Sym);
		} else if (la.kind == bang_Sym) {
			Get();
			Factor();
		} else if (la.kind == lparen_Sym) {
			Get();
			Expression();
			Expect(rparen_Sym);
		} else SynErr(69);
	}

	static void MulOp() {
		if (la.kind == star_Sym) {
			Get();
		} else if (la.kind == slash_Sym) {
			Get();
		} else if (la.kind == andand_Sym) {
			Get();
		} else if (la.kind == percent_Sym) {
			Get();
		} else SynErr(70);
	}

	static void BasicType() {
		if (la.kind == int_Sym) {
			Get();
		} else if (la.kind == bool_Sym) {
			Get();
		} else SynErr(71);
	}



	public static void Parse() {
		la = new Token();
		la.val = "";
		Get();
		Parva();
		Expect(EOF_SYM);

	}

	static bool[,] set = {
		{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,x,x, x,x,x,x, T,x,T,T, T,T,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,T,T,T, T,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x},
		{x,T,T,x, T,x,T,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

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
			case 1: s = "identifier expected"; break;
			case 2: s = "number expected"; break;
			case 3: s = "stringLit expected"; break;
			case 4: s = "charLit expected"; break;
			case 5: s = "\"void\" expected"; break;
			case 6: s = "\"(\" expected"; break;
			case 7: s = "\")\" expected"; break;
			case 8: s = "\"{\" expected"; break;
			case 9: s = "\"}\" expected"; break;
			case 10: s = "\";\" expected"; break;
			case 11: s = "\"continue\" expected"; break;
			case 12: s = "\"break\" expected"; break;
			case 13: s = "\"const\" expected"; break;
			case 14: s = "\",\" expected"; break;
			case 15: s = "\"=\" expected"; break;
			case 16: s = "\"true\" expected"; break;
			case 17: s = "\"false\" expected"; break;
			case 18: s = "\"null\" expected"; break;
			case 19: s = "\"[\" expected"; break;
			case 20: s = "\"]\" expected"; break;
			case 21: s = "\"if\" expected"; break;
			case 22: s = "\"elsif\" expected"; break;
			case 23: s = "\"else\" expected"; break;
			case 24: s = "\"do\" expected"; break;
			case 25: s = "\"while\" expected"; break;
			case 26: s = "\"for\" expected"; break;
			case 27: s = "\"to\" expected"; break;
			case 28: s = "\"downto\" expected"; break;
			case 29: s = "\"return\" expected"; break;
			case 30: s = "\"halt\" expected"; break;
			case 31: s = "\"read\" expected"; break;
			case 32: s = "\"write\" expected"; break;
			case 33: s = "\"+\" expected"; break;
			case 34: s = "\"-\" expected"; break;
			case 35: s = "\"new\" expected"; break;
			case 36: s = "\"!\" expected"; break;
			case 37: s = "\"[]\" expected"; break;
			case 38: s = "\"int\" expected"; break;
			case 39: s = "\"bool\" expected"; break;
			case 40: s = "\"||\" expected"; break;
			case 41: s = "\"*\" expected"; break;
			case 42: s = "\"/\" expected"; break;
			case 43: s = "\"&&\" expected"; break;
			case 44: s = "\"%\" expected"; break;
			case 45: s = "\"==\" expected"; break;
			case 46: s = "\"!=\" expected"; break;
			case 47: s = "\"<\" expected"; break;
			case 48: s = "\"<=\" expected"; break;
			case 49: s = "\">\" expected"; break;
			case 50: s = "\">=\" expected"; break;
			case 51: s = "\"*=\" expected"; break;
			case 52: s = "\"/=\" expected"; break;
			case 53: s = "\"+=\" expected"; break;
			case 54: s = "\"-=\" expected"; break;
			case 55: s = "\"&=\" expected"; break;
			case 56: s = "\"|=\" expected"; break;
			case 57: s = "\"%=\" expected"; break;
			case 58: s = "??? expected"; break;
			case 59: s = "invalid Statement"; break;
			case 60: s = "invalid Assignment"; break;
			case 61: s = "invalid ForStatement"; break;
			case 62: s = "invalid Constant"; break;
			case 63: s = "invalid AssignmentOP"; break;
			case 64: s = "invalid Else"; break;
			case 65: s = "invalid ReadElement"; break;
			case 66: s = "invalid WriteElement"; break;
			case 67: s = "invalid RelOp"; break;
			case 68: s = "invalid AddOp"; break;
			case 69: s = "invalid Factor"; break;
			case 70: s = "invalid MulOp"; break;
			case 71: s = "invalid BasicType"; break;

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
