
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
	const int maxT = 74;
	const int noSym = 74;
	// terminals
	const int EOF_SYM = 0;
	const int terminal_Sym = 1;
	const int equal_Sym = 2;
	const int point_Sym = 3;
	const int bar_Sym = 4;
	const int lbrack_Sym = 5;
	const int rbrack_Sym = 6;
	const int lparen_Sym = 7;
	const int rparen_Sym = 8;
	const int lbrace_Sym = 9;
	const int rbrace_Sym = 10;
	const int underscore_Sym = 11;
	const int d0_Sym = 12;
	const int d1_Sym = 13;
	const int d2_Sym = 14;
	const int d3_Sym = 15;
	const int d4_Sym = 16;
	const int d5_Sym = 17;
	const int d6_Sym = 18;
	const int d7_Sym = 19;
	const int d8_Sym = 20;
	const int d9_Sym = 21;
	const int A_Sym = 22;
	const int B_Sym = 23;
	const int C_Sym = 24;
	const int D_Sym = 25;
	const int E_Sym = 26;
	const int F_Sym = 27;
	const int G_Sym = 28;
	const int H_Sym = 29;
	const int I_Sym = 30;
	const int J_Sym = 31;
	const int K_Sym = 32;
	const int L_Sym = 33;
	const int M_Sym = 34;
	const int N_Sym = 35;
	const int O_Sym = 36;
	const int P_Sym = 37;
	const int Q_Sym = 38;
	const int R_Sym = 39;
	const int S_Sym = 40;
	const int T_Sym = 41;
	const int U_Sym = 42;
	const int V_Sym = 43;
	const int W_Sym = 44;
	const int X_Sym = 45;
	const int Y_Sym = 46;
	const int Z_Sym = 47;
	const int a_Sym = 48;
	const int b_Sym = 49;
	const int c_Sym = 50;
	const int d_Sym = 51;
	const int e_Sym = 52;
	const int f_Sym = 53;
	const int g_Sym = 54;
	const int h_Sym = 55;
	const int i_Sym = 56;
	const int j_Sym = 57;
	const int k_Sym = 58;
	const int l_Sym = 59;
	const int m_Sym = 60;
	const int n_Sym = 61;
	const int o_Sym = 62;
	const int p_Sym = 63;
	const int q_Sym = 64;
	const int r_Sym = 65;
	const int s_Sym = 66;
	const int t_Sym = 67;
	const int u_Sym = 68;
	const int v_Sym = 69;
	const int w_Sym = 70;
	const int x_Sym = 71;
	const int y_Sym = 72;
	const int z_Sym = 73;
	const int NOT_SYM = 74;
	// pragmas

	static short[] start = {
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,
	  0,  0,  3,  0,  0,  0,  0,  1, 11, 12,  0,  0,  0,  0,  7,  0,
	 16, 17, 18, 19, 20, 21, 22, 23, 24, 25,  0,  0,  0,  6,  0,  0,
	  0, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
	 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51,  9,  0, 10,  0, 15,
	  0, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66,
	 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 13,  8, 14,  0,  0,
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


	static bool Comment0() {
		int level = 1, line0 = line, lineStart0 = lineStart;
		NextCh();
		if (ch == '*') {
			NextCh();
			for(;;) {
				if (ch == '*') {
					NextCh();
					if (ch == ')') {
						level--;
						if (level == 0) { oldEols = line - line0; NextCh(); return true; }
						NextCh();
					}
				} else if (ch == '(') {
					NextCh();
					if (ch == '*') {
						level++; NextCh();
					}
				} else if (ch == Buffer.EOF) return false;
				else NextCh();
			}
		} else {
			if (ch == EOL) { line--; lineStart = lineStart0; }
			pos = pos - 2; Buffer.Pos = pos+1; NextCh();
		}
		return false;
	}


	static void CheckLiteral() {
		switch (t.val) {
			default: break;
		}
	}

	/* AW Scan() renamed to NextToken() */
	static Token NextToken() {
		while (ignore[ch]) NextCh();
		if (ch == '(' && Comment0()) return NextToken();
		t = new Token();
		t.pos = pos; t.col = pos - lineStart + 1; t.line = line;
		int state = start[ch];
		StringBuilder buf = new StringBuilder(16);
		buf.Append(ch); NextCh();
		switch (state) {
			case -1: { t.kind = eofSym; goto done; } // NextCh already done /* pdt */
			case 0: { t.kind = noSym; goto done; }   // NextCh already done
			case 1:
				if (!(ch == 39) && ch != Buffer.EOF) { buf.Append(ch); NextCh(); goto case 2; }
				else { t.kind = noSym; goto done; }
			case 2:
				if (!(ch == 39) && ch != Buffer.EOF) { buf.Append(ch); NextCh(); goto case 2; }
				else if (ch == 39) { buf.Append(ch); NextCh(); goto case 5; }
				else { t.kind = noSym; goto done; }
			case 3:
				if (!(ch == '"') && ch != Buffer.EOF) { buf.Append(ch); NextCh(); goto case 4; }
				else { t.kind = noSym; goto done; }
			case 4:
				if (!(ch == '"') && ch != Buffer.EOF) { buf.Append(ch); NextCh(); goto case 4; }
				else if (ch == '"') { buf.Append(ch); NextCh(); goto case 5; }
				else { t.kind = noSym; goto done; }
			case 5:
				{ t.kind = terminal_Sym; goto done; }
			case 6:
				{ t.kind = equal_Sym; goto done; }
			case 7:
				{ t.kind = point_Sym; goto done; }
			case 8:
				{ t.kind = bar_Sym; goto done; }
			case 9:
				{ t.kind = lbrack_Sym; goto done; }
			case 10:
				{ t.kind = rbrack_Sym; goto done; }
			case 11:
				{ t.kind = lparen_Sym; goto done; }
			case 12:
				{ t.kind = rparen_Sym; goto done; }
			case 13:
				{ t.kind = lbrace_Sym; goto done; }
			case 14:
				{ t.kind = rbrace_Sym; goto done; }
			case 15:
				{ t.kind = underscore_Sym; goto done; }
			case 16:
				{ t.kind = d0_Sym; goto done; }
			case 17:
				{ t.kind = d1_Sym; goto done; }
			case 18:
				{ t.kind = d2_Sym; goto done; }
			case 19:
				{ t.kind = d3_Sym; goto done; }
			case 20:
				{ t.kind = d4_Sym; goto done; }
			case 21:
				{ t.kind = d5_Sym; goto done; }
			case 22:
				{ t.kind = d6_Sym; goto done; }
			case 23:
				{ t.kind = d7_Sym; goto done; }
			case 24:
				{ t.kind = d8_Sym; goto done; }
			case 25:
				{ t.kind = d9_Sym; goto done; }
			case 26:
				{ t.kind = A_Sym; goto done; }
			case 27:
				{ t.kind = B_Sym; goto done; }
			case 28:
				{ t.kind = C_Sym; goto done; }
			case 29:
				{ t.kind = D_Sym; goto done; }
			case 30:
				{ t.kind = E_Sym; goto done; }
			case 31:
				{ t.kind = F_Sym; goto done; }
			case 32:
				{ t.kind = G_Sym; goto done; }
			case 33:
				{ t.kind = H_Sym; goto done; }
			case 34:
				{ t.kind = I_Sym; goto done; }
			case 35:
				{ t.kind = J_Sym; goto done; }
			case 36:
				{ t.kind = K_Sym; goto done; }
			case 37:
				{ t.kind = L_Sym; goto done; }
			case 38:
				{ t.kind = M_Sym; goto done; }
			case 39:
				{ t.kind = N_Sym; goto done; }
			case 40:
				{ t.kind = O_Sym; goto done; }
			case 41:
				{ t.kind = P_Sym; goto done; }
			case 42:
				{ t.kind = Q_Sym; goto done; }
			case 43:
				{ t.kind = R_Sym; goto done; }
			case 44:
				{ t.kind = S_Sym; goto done; }
			case 45:
				{ t.kind = T_Sym; goto done; }
			case 46:
				{ t.kind = U_Sym; goto done; }
			case 47:
				{ t.kind = V_Sym; goto done; }
			case 48:
				{ t.kind = W_Sym; goto done; }
			case 49:
				{ t.kind = X_Sym; goto done; }
			case 50:
				{ t.kind = Y_Sym; goto done; }
			case 51:
				{ t.kind = Z_Sym; goto done; }
			case 52:
				{ t.kind = a_Sym; goto done; }
			case 53:
				{ t.kind = b_Sym; goto done; }
			case 54:
				{ t.kind = c_Sym; goto done; }
			case 55:
				{ t.kind = d_Sym; goto done; }
			case 56:
				{ t.kind = e_Sym; goto done; }
			case 57:
				{ t.kind = f_Sym; goto done; }
			case 58:
				{ t.kind = g_Sym; goto done; }
			case 59:
				{ t.kind = h_Sym; goto done; }
			case 60:
				{ t.kind = i_Sym; goto done; }
			case 61:
				{ t.kind = j_Sym; goto done; }
			case 62:
				{ t.kind = k_Sym; goto done; }
			case 63:
				{ t.kind = l_Sym; goto done; }
			case 64:
				{ t.kind = m_Sym; goto done; }
			case 65:
				{ t.kind = n_Sym; goto done; }
			case 66:
				{ t.kind = o_Sym; goto done; }
			case 67:
				{ t.kind = p_Sym; goto done; }
			case 68:
				{ t.kind = q_Sym; goto done; }
			case 69:
				{ t.kind = r_Sym; goto done; }
			case 70:
				{ t.kind = s_Sym; goto done; }
			case 71:
				{ t.kind = t_Sym; goto done; }
			case 72:
				{ t.kind = u_Sym; goto done; }
			case 73:
				{ t.kind = v_Sym; goto done; }
			case 74:
				{ t.kind = w_Sym; goto done; }
			case 75:
				{ t.kind = x_Sym; goto done; }
			case 76:
				{ t.kind = y_Sym; goto done; }
			case 77:
				{ t.kind = z_Sym; goto done; }

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
