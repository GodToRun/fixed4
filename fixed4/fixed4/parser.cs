using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace fixed4
{
    public class AST
    {
        public LexicalToken root = new LexicalToken();
    }
    public class parser
    {
        public static AST parse(LexicalToken[] tokens)
        {
            AST ast = new AST();
            ast.root.tokenWord = LexerWord.Root;
            ast.root.tokenValue = "root";
            int stack = 1;
            List<LexicalToken> rootStacks = new List<LexicalToken>();
            rootStacks.Add(ast.root);
            for (int i = 0; i < tokens.Length; i++)
            {
                LexicalToken token = tokens[i];
                if (token.tokenWord != LexerWord.LBrac && token.tokenWord != LexerWord.RBrac)
                {
                    rootStacks[stack - 1].tokens.Add(token);
                    token.parent = rootStacks[stack - 1];
                }
                if (token.tokenWord == LexerWord.Function)
                {
                    while (tokens[i+1].tokenWord != LexerWord.LParen)
                    {
                        token.tokens.Add(tokens[++i]);
                        tokens[i].parent = token;
                    }
                    //i--;
                    rootStacks[stack - 1] = tokens[i];
                    while (tokens[i].tokenWord != LexerWord.RParen)
                    {
                        tokens[i].tokens.Add(tokens[++i]);
                        if (i > 0)
                            tokens[i].parent = tokens[i - 1];
                    }
                }
                else if (token.tokenWord == LexerWord.LBrac)
                {
                    rootStacks.Add(tokens[i-1]);
                    stack++;
                }
                else if (token.tokenWord == LexerWord.ASM)
                {

                }
         
                else if (token.tokenWord == LexerWord.RBrac)
                {
                    rootStacks.RemoveAt(stack-1);
                    stack--;
                }
                else if (token.tokenWord == LexerWord.If)
                {
                    while (tokens[i].tokenWord != LexerWord.RParen)
                    {
                        token.tokens.Add(tokens[++i]);
                        tokens[i].parent = token;
                    }
                }
                else
                {
                    while (tokens[i].tokenWord != LexerWord.EOL)
                    {
                        tokens[i].tokens.Add(tokens[++i]);
                        if (i > 0)
                            tokens[i].parent = tokens[i - 1];
                    }
                }
            }
            return ast;
        }
    }
}
