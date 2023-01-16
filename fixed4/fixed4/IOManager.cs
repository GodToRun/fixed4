using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace fixed4
{
    class IOManager
    {
        public static string[] tokenize(string contents)
        {
            bool canTokenize = true;
            List<string> strings = new List<string>();
            string current_string = "";
            char pre_c = '\0';
            foreach (char c in contents)
            {
                if (c == '\0')
                    continue;
                if (canTokenize && (c == '|' || c == '!' || c == '@' || c == '#' || c == '$' ||
                    c == '\n' || c == '\r' || c == ' ' || c == '\t' || c == ' ' || c == '%' ||
                    c == '^' || c == '&' || c == '*' || c == '+' || c == '[' ||
                    c == ']' || c == '{' || c == '}' || c == '(' || c == ')' ||
                    c == '\'' || c == '\\' || c == '-' || c == '/' ||
                    c == ':' || c == ';' || c == '~' || c == '`' || c == ',' ||
                    c == '.' || c == '<' || c == '>' || c == '?' || c == '='))
                {
                    if (current_string != "")
                        strings.Add(current_string);
                    if (pre_c == '\'' || (pre_c != '\'' && c != ' ' && c != '\t' && c != '\n' && c != '\0' && c != '\r'))
                        strings.Add(c.ToString());
                    current_string = "";
                }
                else if (c == '\"')
                {
                    if (current_string.Length > 0)
                        strings.Add(current_string);
                    current_string = "";
                    strings.Add(c + "");
                    canTokenize = !canTokenize;
                }
                else
                {
                    current_string += c;
                }
                pre_c = c;
            }
            if (current_string != "")
            {
                strings.Add(current_string);
            }
            return strings.ToArray();
        }
        public static string readFile(string file)
        {
            return File.ReadAllText(file);
        }
    }
}
