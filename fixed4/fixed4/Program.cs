using System;
using System.IO;

namespace fixed4
{
    class Program
    {
        static string filename = "";
        static void printAST(LexicalToken root, string memory)
        {
            Console.WriteLine(memory + " - " + root.tokenValue + " : " + root.tokenWord);
            foreach (LexicalToken token in root.tokens)
                printAST(token, root.tokenValue);
        }
        static void Main(string[] args)
        {
            var list = args.ToList();
            list.Add("file:D:/CosmosDevKit/Sources/fixed4/test/fixed4.fx4");
            string outputFolder = "";
            foreach (string str in list)
            {
                if (str.StartsWith("output:"))
                {
                    outputFolder = str.Replace("output:", "");
                }
                else if (str.StartsWith("file:"))
                {
                    filename = str.Replace("file:", "");
                    outputFolder = Path.GetDirectoryName(filename);
                }
            }
            string contents = IOManager.readFile(filename);
            string[] tokens = IOManager.tokenize(contents);
            LexicalToken[] words = Lexer.LexicalAnalyze(tokens);
            AST ast = parser.parse(words);
            string code = Compiler.compile(PLATFORM.INTEL, ast);
            Console.WriteLine("Final Code: " + code);
            printAST(ast.root, "Root");
            foreach (KeyValuePair<string, Variable> kvp in Compiler.variables)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value.type);
            }
            /*if (outputFolder.Length > 0)
                Directory.SetCurrentDirectory(outputFolder);*/
            //preprocessor.preprocess(ref tokens);
            //string code = parser.parse(tokens);
            //File.WriteAllText(Path.GetFileName(filename).Replace(Path.GetExtension(Path.GetFileName(filename)), ".asm"), code);
        }
    }
}
