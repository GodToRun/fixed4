using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace fixed4
{
    class preprocessor
    {
        static Dictionary<string, string> defines = new Dictionary<string, string>();
        public static string stdFolder = "C:\\Program Files\\dword\\";
        static string getAfterToken(string[] tokens, int i, int indicator)
        {
            if (i + indicator >= 0 && i + indicator < tokens.Length)
                return tokens[i + indicator];
            return null;
        }
        public static void preprocess(ref string[] tokens)
        {
            List<string> list = tokens.ToList();
            List<string> newList = new List<string>();
            int len = tokens.Length;
            for (int i = 0; i < len; i++)
            {
                string token = list[i];
                if (token == "#")
                {
                    token = list[++i];
                    if (token == "include")
                    {
                        string str = "";
                        if (list[i + 1] == "\"")
                            str = System.IO.File.ReadAllText(list[i + 2]);
                        else
                            str = System.IO.File.ReadAllText(stdFolder + list[i+2] + ".dw");
                        string[] incTokens = IOManager.tokenize(str);
                        newList.AddRange(incTokens);
                        i += 3;
                    }
                    else if (token == "define")
                    {
                        defines.Add(getAfterToken(tokens, i, 1), getAfterToken(tokens, i, 2));
                        i += 2;
                    }
                }
                else if (defines.ContainsKey(token))
                {
                    newList.Add(defines[token]);
                }
                else
                {
                    newList.Add(token);
                }
            }
            tokens = newList.ToArray();
        }
    }
}
