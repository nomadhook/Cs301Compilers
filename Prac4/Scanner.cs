
using System;
using System.IO;
using System.Collections;
using System.Text;

namespace BNF {

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
	const int maxT = 7;
	const int noSym = 7;
	// terminals
	const int EOF_SYM = 0;
	const int terminal_Sym = 1;
	const int nonterminal_Sym = 2;
	const int newLine_Sym = 3;
	const int coloncolonequal_Sym = 4;
	const int bar_Sym = 5;
	const int eps_Sym = 6;
	const int NOT_SYM = 7;
	// pragmas

	static short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  5,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  6,  0,  2,  0,  0,  0,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  0,  0,  0,  0,  0,
	  0,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,
	  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  0,  9,  0,  0,  0,
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
		ignore[0] = true; ignore[1] = true; ignore[2] = true; ignore[3] = true; 
		ignore[4] = true; ignore[5] = true; ignore[6] = true; ignore[7] = true; 
		ignore[8] = true; ignore[9] = true; ignore[11] = true; ignore[12] = true; 
		ignore[13] = true; ignore[14] = true; ignore[15] = true; ignore[16] = true; 
		ignore[17] = true; ignore[18] = true; ignore[19] = true; ignore[20] = true; 
		ignore[21] = true; ignore[22] = true; ignore[23] = true; ignore[24] = true; 
		ignore[25] = true; ignore[26] = true; ignore[27] = true; ignore[28] = true; 
		ignore[29] = true; ignore[30] = true; ignore[31] = true; 
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
			case "eps": t.kind = eps_Sym; break;
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
				if ((ch >= 'A' && ch <= 'Z'
				  || ch >= 'a' && ch <= 'z')) { buf.Append(ch); NextCh(); goto case 1; }
				else { t.kind = terminal_Sym; t.val = buf.ToString(); CheckLiteral(); return t; }
			case 2:
				if ((ch >= 'A' && ch <= 'Z'
				  || ch >= 'a' && ch <= 'z')) { buf.Append(ch); NextCh(); goto case 3; }
				else { t.kind = noSym; goto done; }
			case 3:
				if ((ch == ' '
				  || ch >= 'A' && ch <= 'Z'
				  || ch >= 'a' && ch <= 'z')) { buf.Append(ch); NextCh(); goto case 3; }
				else if (ch == '>') { buf.Append(ch); NextCh(); goto case 4; }
				else { t.kind = noSym; goto done; }
			case 4:
				{ t.kind = nonterminal_Sym; goto done; }
			case 5:
				{ t.kind = newLine_Sym; goto done; }
			case 6:
				if (ch == ':') { buf.Append(ch); NextCh(); goto case 7; }
				else { t.kind = noSym; goto done; }
			case 7:
				if (ch == '=') { buf.Append(ch); NextCh(); goto case 8; }
				else { t.kind = noSym; goto done; }
			case 8:
				{ t.kind = coloncolonequal_Sym; goto done; }
			case 9:
				{ t.kind = bar_Sym; goto done; }

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
