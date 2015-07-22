using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FortuoScript
{
    public class FortuoInterpreter
    {
        const char commentChar = '%';
        Regex commentRegex, splitRegex;

        Dictionary<string, object> dictionary;
        Stack<object> stack;
        object a1, a2;

        public FortuoInterpreter()
        {
            stack = new Stack<object>();
            dictionary = new Dictionary<string, object>();

            commentRegex = new Regex(commentChar + ".*");
            // \\"(\\.|[^"\\])*\\"|([^ ]+)
            splitRegex = new Regex("\"(?:\\\\.|[^\"\\\\])*\"|[^ ]+");
        }

        public void Execute(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return;

            // remove comments
            int firstCommentIndex = line.IndexOf(commentChar);
            if (firstCommentIndex >= 0)
                line = line.Substring(0, firstCommentIndex);

            if (string.IsNullOrWhiteSpace(line))
                return;

            // split by white space
            var matches = splitRegex.Matches(line);
            
            foreach (Match m in matches)
            {
                string word = m.Value;

                if (word.StartsWith("\"") && word.EndsWith("\""))
                {
                    string str = word.Substring(1, word.Length - 2).Replace("\\\"", "\"");
                    stack.Push(str);
                    continue;
                }

                if (word.StartsWith("/"))
                {
                    string key = word.Substring(1);
                    stack.Push(new FTNameDef(key));
                    continue;
                }

                int num;
                if (int.TryParse(word, out num))
                { 
                    // add number to stack
                    stack.Push(num);
                    // continue to next cmd
                    continue;
                }
                
                // not a number
                switch (word)
                {
                    case ".":
                        if (stack.Count == 0)
                            throw new FIStackUnderFlowException(word);
                        Console.Write(stack.Pop());
                        break;
                    case "cr":
                        Console.WriteLine();
                        break;
                    case "add":
                        CheckType2(word, typeof(int));
                        stack.Push((int)a1 + (int)a2);
                        break;
                    case "sub":
                        CheckType2(word, typeof(int));
                        stack.Push((int)a2 - (int)a1);
                        break;
                    case "mul":
                        CheckType2(word, typeof(int));
                        stack.Push((int)a1 * (int)a2);
                        break;
                    case "div":
                        CheckType2(word, typeof(int));
                        stack.Push((int)a2 / (int)a1);
                        break;
                    case "mod":
                        CheckType2(word, typeof(int));
                        stack.Push((int)a2 % (int)a1);
                        break;
                    case "clear":
                        stack.Clear();
                        break;
                    case "pstack":
                        Console.WriteLine("----------------------------\r\nStack    Count: {0}", stack.Count);
                        for (int i = 0; i < stack.Count; i++)
                        {
                            object elem = stack.ElementAt(i);
                            if (elem.GetType() == typeof(string))
                                Console.WriteLine("{0} ->\tString:\t\"{1}\"", i, elem);
                            else if (elem.GetType() == typeof(int))
                                Console.WriteLine("{0} ->\tInt:\t{1}", i, elem);
                            else
                                Console.WriteLine("{0} ->\t{1}", i, elem);
                        }
                        Console.WriteLine("----------------------------");
                        break;
                    case "dup":
                        if (stack.Count == 0)
                            throw new FIStackEmptyException(word);
                        stack.Push(stack.Peek());
                        break;
                    case "exch":
                        if (stack.Count < 2)
                            throw new FIStackUnderFlowException(word);

                        object b1 = stack.Pop();
                        object b2 = stack.Pop();
                        stack.Push(b1);
                        stack.Push(b2);
                        break;
                    case "def":
                        if (stack.Count < 2)
                            throw new FIStackUnderFlowException(word);

                        object v1 = stack.Pop();
                        object v2 = stack.Pop();

                        if (!v2.GetType().Equals(typeof(FTNameDef)))
                        {
                            stack.Push(v2);
                            stack.Push(v1);
                            throw new FIInconsistentTypesException(word);
                        }
                        string name = ((FTNameDef)v2).Name;
                        if (dictionary.ContainsKey(name))
                            dictionary[name] = v1;
                        else
                            dictionary.Add(name, v1);

                        break;
                    case "pdict":
                        Console.WriteLine("----------------------------\r\nDict    Count: {0}", dictionary.Count);
                        for (int i = 0; i < dictionary.Count; i++)
                        {
                            string key = dictionary.Keys.ElementAt(i);
                            object obj = dictionary[key];

                            Console.WriteLine("{0}\t->\t{1}", key, obj);
                        }
                        Console.WriteLine("----------------------------");
                        break;
                    default:

                        if (dictionary.ContainsKey(word))
                        {
                            stack.Push(dictionary[word]);
                            continue;
                        }

                        throw new FIUnknownSymbolException(word);
                        break;
                }
            }
        }

        private void CheckType2(string c, Type type)
        {
            if (stack.Count < 2)
                throw new FIStackUnderFlowException(c);

            a1 = stack.Pop();
            a2 = stack.Pop();
            if (!a1.GetType().Equals(type) || !a2.GetType().Equals(type))
            {
                stack.Push(a2);
                stack.Push(a1);
                throw new FIInconsistentTypesException(c);
            }
        }
    }
}