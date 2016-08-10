
using System;
using System.IO;
using System.Collections;
using System.Text;

namespace EBNFR {

public class Token {
	public int kind;    // token kind
	public int pos;     // token position in the source text (starting at 0)
	public int col;     // token column (starting at 0)
	public int line;    // token line (starting at 1)
	public string val;  // token value
	public Token next;  // AW 2003-03-07 Tokens are kept in linked list
}

public class Buffer {
	public const char EOF = (char)256;
	static byte[] buf;
	static int bufLen;
	static int pos;

	public static void Fill (Stream s) {
		bufLen = (int) s.Length;
		buf = new byte[bufLen];
		s.Read(buf, 0, bufLen);
		pos = 0;
	}

	public static int Read () {
		if (pos < bufLen) return buf[pos++];
		else return EOF;                          /* pdt */
	}

	public static int Peek () {
		if (pos < bufLen) return buf[pos];
		else return EOF;                          /* pdt */
	}

	/* AW 2003-03-10 moved this from ParserGen.cs */
	public static string GetString (int beg, int end) {
		StringBuilder s = new StringBuilder(64);
		int oldPos = Buffer.Pos;
		Buffer.Pos = beg;
		while (beg < end) { s.Append((char)Buffer.Read()); beg++; }
		Buffer.Pos = oldPos;
		return s.ToString();
	}

	public static int Pos {
		get { return pos; }
		set {
			if (value < 0) pos = 0;
			else if (value >= bufLen) pos = bufLen;
			else pos = value;
		}
	}

} // end Buffer

public class Scanner {
	const char EOL = '\n';
	const int  eofSym = 0;
	const int charSetSize = 256;
	const int maxT = 34;
	const int noSym = 34;
	// terminals
	const int EOF_SYM = 0;
	const int letter_Sym = 1;
	const int digit_Sym = 2;
	const int equal_Sym = 3;
	const int point_Sym = 4;
	const int bar_Sym = 5;
	const int lbrack_Sym = 6;
	const int rbrack_Sym = 7;
	const int lparen_Sym = 8;
	const int rparen_Sym = 9;
	const int lbrace_Sym = 10;
	const int rbrace_Sym = 11;
	const int underscore_Sym = 12;
	const int squote_Sym = 13;
	const int dquote_Sym = 14;
	const int accent_Sym = 15;
	const int tilde_Sym = 16;
	const int bang_Sym = 17;
	const int at_Sym = 18;
	const int hash_Sym = 19;
	const int dollar_Sym = 20;
	const int percent_Sym = 21;
	const int uparrow_Sym = 22;
	const int and_Sym = 23;
	const int star_Sym = 24;
	const int plus_Sym = 25;
	const int minus_Sym = 26;
	const int colon_Sym = 27;
	const int semicolon_Sym = 28;
	const int comma_Sym = 29;
	const int less_Sym = 30;
	const int greater_Sym = 31;
	const int slash_Sym = 32;
	const int query_Sym = 33;
	const int NOT_SYM = 34;
	// pragmas

	static short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0, 17, 14, 19, 20, 21, 23, 13,  8,  9, 24, 25, 29, 26,  4, 32,
	  2,  2,  2,  2,  2,  2,  2,  2,  2,  2, 27, 28, 30,  3, 31, 33,
	 18,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  6,  0,  7, 22, 12,
	 15,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 10,  5, 11, 16,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  -1};


	static Token t;          // current token
	static char ch;          // current input character
	static int pos;          // column number of current character
	static int line;         // line number of current character
	static int lineStart;    // start position of current line
	static int oldEols;      // EOLs that appeared in a comment;
	static BitArray ignore;  // set of characters to be ignored by the scanner

	static Token tokens;     // the complete input token stream
	static Token pt;         // current peek token

	public static void Init (string fileName) {
		FileStream s = null;
		try {
			s = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			Init(s);
		} catch (IOException) {
			Console.WriteLine("--- Cannot open file {0}", fileName);
			System.Environment.Exit(1);
		} finally {
			if (s != null) s.Close();
		}
	}

	public static void Init (Stream s) {
		Buffer.Fill(s);
		pos = -1; line = 1; lineStart = 0;
		oldEols = 0;
		NextCh();
		ignore = new BitArray(charSetSize+1);
		ignore[' '] = true;  // blanks are always white space
		ignore[9] = true; ignore[10] = true; ignore[11] = true; ignore[12] = true; 
		ignore[13] = true; 
		//--- AW: fill token list
		tokens = new Token();  // first token is a dummy
		Token node = tokens;
		do {
			node.next = NextToken();
			node = node.next;
		} while (node.kind != eofSym);
		node.next = node;
		node.val = "EOF";
		t = pt = tokens;
	}

	static void NextCh() {
		if (oldEols > 0) { ch = EOL; oldEols--; }
		else {
			ch = (char)Buffer.Read(); pos++;
			// replace isolated '\r' by '\n' in order to make
			// eol handling uniform across Windows, Unix and Mac
			if (ch == '\r' && Buffer.Peek() != '\n') ch = EOL;
			if (ch == EOL) { line++; lineStart = pos + 1; }
		}

	}



	static void CheckLiteral() {
		switch (t.val) {
			default: break;
		}
	}

	/* AW Scan() renamed to NextToken() */
	static Token NextToken() {
		while (ignore[ch]) NextCh();

		t = new Token();
		t.pos = pos; t.col = pos - lineStart + 1; t.line = line;
		int state = start[ch];
		StringBuilder buf = new StringBuilder(16);
		buf.Append(ch); NextCh();
		switch (state) {
			case -1: { t.kind = eofSym; goto done; } // NextCh already done /* pdt */
			case 0: { t.kind = noSym; goto done; }   // NextCh already done
			case 1:
				{ t.kind = letter_Sym; goto done; }
			case 2:
				{ t.kind = digit_Sym; goto done; }
			case 3:
				{ t.kind = equal_Sym; goto done; }
			case 4:
				{ t.kind = point_Sym; goto done; }
			case 5:
				{ t.kind = bar_Sym; goto done; }
			case 6:
				{ t.kind = lbrack_Sym; goto done; }
			case 7:
				{ t.kind = rbrack_Sym; goto done; }
			case 8:
				{ t.kind = lparen_Sym; goto done; }
			case 9:
				{ t.kind = rparen_Sym; goto done; }
			case 10:
				{ t.kind = lbrace_Sym; goto done; }
			case 11:
				{ t.kind = rbrace_Sym; goto done; }
			case 12:
				{ t.kind = underscore_Sym; goto done; }
			case 13:
				{ t.kind = squote_Sym; goto done; }
			case 14:
				{ t.kind = dquote_Sym; goto done; }
			case 15:
				{ t.kind = accent_Sym; goto done; }
			case 16:
				{ t.kind = tilde_Sym; goto done; }
			case 17:
				{ t.kind = bang_Sym; goto done; }
			case 18:
				{ t.kind = at_Sym; goto done; }
			case 19:
				{ t.kind = hash_Sym; goto done; }
			case 20:
				{ t.kind = dollar_Sym; goto done; }
			case 21:
				{ t.kind = percent_Sym; goto done; }
			case 22:
				{ t.kind = uparrow_Sym; goto done; }
			case 23:
				{ t.kind = and_Sym; goto done; }
			case 24:
				{ t.kind = star_Sym; goto done; }
			case 25:
				{ t.kind = plus_Sym; goto done; }
			case 26:
				{ t.kind = minus_Sym; goto done; }
			case 27:
				{ t.kind = colon_Sym; goto done; }
			case 28:
				{ t.kind = semicolon_Sym; goto done; }
			case 29:
				{ t.kind = comma_Sym; goto done; }
			case 30:
				{ t.kind = less_Sym; goto done; }
			case 31:
				{ t.kind = greater_Sym; goto done; }
			case 32:
				{ t.kind = slash_Sym; goto done; }
			case 33:
				{ t.kind = query_Sym; goto done; }

		}
		done:
		t.val = buf.ToString();
		return t;
	}

	/* AW 2003-03-07 get the next token, move on and synch peek token with current */
	public static Token Scan () {
		t = pt = t.next;
		return t;
	}

	/* AW 2003-03-07 get the next token, ignore pragmas */
	public static Token Peek () {
		do {                      // skip pragmas while peeking
			pt = pt.next;
		} while (pt.kind > maxT);
		return pt;
	}

	/* AW 2003-03-11 to make sure peek start at current scan position */
	public static void ResetPeek () { pt = t; }

} // end Scanner

} // end namespace
