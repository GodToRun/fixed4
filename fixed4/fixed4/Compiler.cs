using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fixed4
{
    public enum PLATFORM {
        X86,X86_64,INTEL,AT_T
    }
    public enum VAR_TYPE
    {
        Int, Short, Char, Func
    }
    public struct Variable
    {
        public VAR_TYPE type;
        public bool isArray, isPointer, isStackPlus;
        public int stack, size;
        public Variable(string name, VAR_TYPE type, int stack, int size, bool isArray, bool isPointer, bool isStackPlus)
        {
            this.type = type;
            this.stack = stack;
            this.size = size;
            this.isArray = isArray;
            this.isPointer = isPointer;
            this.isStackPlus = isStackPlus;
            Compiler.variables.Add(name, this);
        }
    }
    public class Compiler
    {
        public static int varstack = 0, argstack = 0, ifstack = 0;
        public static bool isInParameter = false, isInIf = false;
        public static Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        public static int string_index = 0;
        public static List<LexerWord> decKeywords = new List<LexerWord>();
        public static string processString(LexicalToken rootDquote, ref string label)
        {
            label = "fixed4_STR_" + string_index;
            string_index++;
            return label + ":\ndb '" + rootDquote.tokens[0].tokenValue + "',0\n";
        }
        public static string CompileValue(LexicalToken root, ref string code)
        {
            if (root.tokenWord == LexerWord.Number)
                return root.tokenValue;
            else if (root.tokenWord == LexerWord.Ident)
            {
                if (!variables[root.tokenValue].isStackPlus)
                    return "dword [ebp-" + variables[root.tokenValue].stack + "]";
                else
                    return "dword [ebp+" + variables[root.tokenValue].stack + "]";
            }
            else if  (root.tokenWord == LexerWord.DQUOTE)
            {
                string label = "";
                code = code.Insert(13, "\n" + processString(root, ref label));
                return label;
            }
            
            return "";
        }
        public static int sizeofVar(VAR_TYPE type)
        {
            switch (type)
            {
                case VAR_TYPE.Int:
                    return 4;
                case VAR_TYPE.Func:
                    return 4;
                case VAR_TYPE.Short:
                    return 2;
                case VAR_TYPE.Char:
                    return 1;
            }
            return 0;
        }
        public static int call1loop(LexicalToken lParenRoot, ref string code, ref List<string> pushLists)
        {
            int siz = 0;
            if (lParenRoot.tokenWord == LexerWord.COMMA || lParenRoot.tokenWord == LexerWord.LParen)
            {
                string rc = CompileValue(lParenRoot.tokens[0], ref code);
                pushLists.Add("\npush " + rc + "\n");
                siz += 4;
            }
            foreach (LexicalToken token in lParenRoot.tokens)
            {
                siz += call1loop(token, ref code, ref pushLists);
            }
            return siz;
        }

        public static string compileRootLoop(LexicalToken root, ref string code, bool loop)
        {
            if (root.tokenWord == LexerWord.Return)
            {
                if (root.tokens.Count > 1)
                    code += "mov eax, " + CompileValue(root.tokens[0], ref code) + "\n";
                code += "pop ebp\n";
                code += "ret";
            }
            else if (root.tokenWord == LexerWord.Int ||
                root.tokenWord == LexerWord.Short ||
                root.tokenWord == LexerWord.Char ||
                root.tokenWord == LexerWord.Extern ||
                root.tokenWord == LexerWord.Function ||
                root.tokenWord == LexerWord.MulOperator)
            {
                decKeywords.Add(root.tokenWord);
            }
            else if (root.tokenWord == LexerWord.EOL || root.tokenWord == LexerWord.LBrac)
            {
                if (root.tokenWord == LexerWord.LBrac && isInParameter) isInParameter = false;
                decKeywords.Clear();
            }
            else if (root.tokenWord == LexerWord.RParen && isInParameter) isInParameter = false;
            else if (root.tokenWord == LexerWord.Ident && !variables.ContainsKey(root.tokenValue) && decKeywords.Count > 0)
            {
                bool isExtern = false;
                Variable var = new Variable();
                if (isInParameter)
                {
                    var.isStackPlus = true;
                    var.size = sizeofVar(var.type);
                    argstack += var.size;
                    var.stack = argstack + 4;
                }
                foreach (LexerWord keyword in decKeywords)
                {
                    if (keyword == LexerWord.Extern)
                    {
                        isExtern = true;
                        code += "\nextern _" + root.tokenValue + "\n";
                    }
                    else if (keyword == LexerWord.Int)
                    {
                        var.type = VAR_TYPE.Int;
                    }
                    else if (keyword == LexerWord.Short)
                    {
                        var.type = VAR_TYPE.Short;
                    }
                    else if (keyword == LexerWord.Char)
                    {
                        var.type = VAR_TYPE.Char;
                    }
                    else if (keyword == LexerWord.MulOperator)
                    {
                        var.isPointer = true;
                    }
                    else if (keyword == LexerWord.Function)
                    {
                        var.type = VAR_TYPE.Func;
                        varstack = 0;
                        if (!isInParameter)
                        {
                            isInParameter = true;
                            argstack = 0;
                        }
                    }
                }
                if (var.type == VAR_TYPE.Func && !isExtern)
                {
                    code += "\nglobal _" + root.tokenValue + "\n";
                    code += "_" + root.tokenValue + ":\n";
                    code += "push ebp\n";
                    code += "mov ebp, esp\n";
                }
                var.isArray = false;
                var.size = sizeofVar(var.type);
                if (var.isPointer) var.size = 4;
                if (!isInParameter)
                {
                    varstack += var.size;
                    var.stack = varstack;
                }
                if (isExtern && var.type == VAR_TYPE.Func)
                {
                    isInParameter = false;
                }
                variables.Add(root.tokenValue, var);
                decKeywords.Clear();
            }
            else if (root.tokenWord == LexerWord.RBrac)
            {
                if (isInIf)
                {
                    code += "fixed4_If_L"+ifstack+":\n";
                    ifstack++;
                    isInIf = false;
                }
            }
            else if (variables.ContainsKey(root.tokenValue) && decKeywords.Count == 0 && root.tokens.Count > 0 && root.tokens[0].tokenWord == LexerWord.LParen &&
                variables[root.tokenValue].type == VAR_TYPE.Func)
            {
                List<string> str = new List<string>();
                int siz = call1loop(root.tokens[0], ref code, ref str);
                str.Reverse();
                foreach (string str1 in str)
                {
                    code += str1;
                }
                code += "call _" + root.tokenValue + "\n";
                code += "add esp, " + siz + "\n";
            }
            else if (root.tokenWord == LexerWord.If)
            {
                isInIf = true;
                LexicalToken ft = root.tokens[1];
                LexicalToken lt = root.tokens[root.tokens.Count-2];
                LexicalToken? op = null;
                foreach (LexicalToken token in root.tokens)
                {
                    if (token.tokenWord == LexerWord.Equals)
                    {
                        op = token;
                        break;
                    }
                }
                string r1 = CompileValue(ft, ref code);
                string r2 = CompileValue(lt, ref code);
                code += "mov eax, " + r1 + "\n";
                code += "cmp eax, " + r2 + "\n";
                if (op != null && op.tokenWord == LexerWord.Equals)
                {
                    code += "jne fixed4_If_L" + ifstack + "\n";
                }
            }
            else if (root.tokenWord == LexerWord.Equals && root.tokens.Count > 0 && root.parent != null)
            {
                if (root.tokens[0].tokenValue != "&")
                {
                    string cv = CompileValue(root.tokens[0], ref code);
                    code += "mov eax, " + cv + "\n";
                    string cv2 = CompileValue(root.parent, ref code);
                    code += "mov " + cv2 + ", eax\n";
                }
                else
                {
                    code += "lea eax, " + CompileValue(root.tokens[0].tokens[0], ref code).Replace("dword", "") + "\n";
                    code += "mov " + CompileValue(root.parent, ref code) + ", eax\n";
                }
            }
            else if (root.tokenWord == LexerWord.ASM)
            {
                LexicalToken inline = root.tokens[0].tokens[0];
                code += "\n" + inline.tokenValue + "\n";
            }
            if (loop)
            {
                foreach (LexicalToken token in root.tokens)
                {
                    compileRootLoop(token, ref code, true);
                }
            }
            return code;
        }
        public static string compile(PLATFORM platform, AST ast)
        {
            string code = "section .text\n";
            compileRootLoop(ast.root, ref code, true);
            return code;
        }
    }
}
