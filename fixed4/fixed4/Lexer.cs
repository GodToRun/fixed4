using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fixed4
{
    public enum LexerWord
    {
        Root,
        Whitespace,
        Return,
        Function,
        Number,
        Ident,
        Int, Short, Char,
        FixedKeyword,
        AddOperator, SubOperator, MulOperator, DivOperator,
        Equals,
        If,
        Else,
        For,
        While,
        LParen,
        RParen,
        LBrac,
        RBrac,
        LSB,
        RSB,
        EOL,
        DQUOTE,
        QUOTE,
        COMMA,
        ASM,
        Extern,
        Unknown
    }
    public class LexicalToken
    {
        public LexerWord tokenWord;
        public string tokenValue = "";
        public List<LexicalToken> tokens = new List<LexicalToken>();
        public LexicalToken? parent;
    }
    public class Lexer
    {
        public static LexicalToken[] LexicalAnalyze(string[] tokens)
        {
            LexicalToken[] lexerWords = new LexicalToken[tokens.Length];
            int i = 0;
            foreach (string t in tokens)
            {
                lexerWords[i] = new LexicalToken();
                if (t == "void")
                    lexerWords[i].tokenWord = LexerWord.Function;
                else if (t == "{")
                    lexerWords[i].tokenWord = LexerWord.LBrac;
                else if (t == "}")
                    lexerWords[i].tokenWord = LexerWord.RBrac;
                else if (t == "(")
                    lexerWords[i].tokenWord = LexerWord.LParen;
                else if (t == ")")
                    lexerWords[i].tokenWord = LexerWord.RParen;
                else if (t == "+")
                    lexerWords[i].tokenWord = LexerWord.AddOperator;
                else if (t == "-")
                    lexerWords[i].tokenWord = LexerWord.SubOperator;
                else if (t == "if")
                    lexerWords[i].tokenWord = LexerWord.If;
                else if (int.TryParse(t, out _))
                    lexerWords[i].tokenWord = LexerWord.Number;
                else if (t == "int")
                    lexerWords[i].tokenWord = LexerWord.Int;
                else if (t == "short")
                    lexerWords[i].tokenWord = LexerWord.Short;
                else if (t == "char")
                    lexerWords[i].tokenWord = LexerWord.Char;
                else if (t == "return")
                    lexerWords[i].tokenWord = LexerWord.Return;
                else if (t == "extern")
                    lexerWords[i].tokenWord = LexerWord.Extern;
                else if (t == "=")
                    lexerWords[i].tokenWord = LexerWord.Equals;
                else if (t == "*")
                    lexerWords[i].tokenWord = LexerWord.MulOperator;
                else if (t == "/")
                    lexerWords[i].tokenWord = LexerWord.DivOperator;
                else if (t == "asm")
                    lexerWords[i].tokenWord = LexerWord.ASM;
                else if (t == ",")
                    lexerWords[i].tokenWord = LexerWord.COMMA;
                else if (t == "\"")
                    lexerWords[i].tokenWord = LexerWord.DQUOTE;
                else if (t == "\'")
                    lexerWords[i].tokenWord = LexerWord.QUOTE;
                else if (t == ";")
                    lexerWords[i].tokenWord = LexerWord.EOL;
                else
                    lexerWords[i].tokenWord = LexerWord.Ident;
                lexerWords[i].tokenValue = tokens[i];
                i++;
            }
            return lexerWords;
        }
    }
}
