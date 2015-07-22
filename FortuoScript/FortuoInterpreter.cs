using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FortuoScript.Types;

namespace FortuoScript
{
    public class FortuoInterpreter
    {
        const char commentChar = '%';
        Regex commentRegex, splitRegex;

        Dictionary<string, FTObject> dictionary;
        FTStack stack;
        FTObject a1, a2;

        int commandRecordDepth;

        public FortuoInterpreter()
        {
            stack = new FTStack();
            dictionary = new Dictionary<string, FTObject>();
            commandRecordDepth = 0;

            commentRegex = new Regex(commentChar + ".*");
            // "(\\.|[^"\\])*"|([^ ]+)
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
                    stack.Push(FTType.String, str);
                    continue;
                }

                if (word.StartsWith("/"))
                {
                    string key = word.Substring(1);
                    stack.Push(FTType.NameDef, key);
                    continue;
                }

                int num;
                if (int.TryParse(word, out num))
                { 
                    // add number to stack
                    stack.Push(FTType.Int, num);
                    // continue to next cmd
                    continue;
                }

                if (commandRecordDepth > 0)
                {
                    if (word == "}")
                    {
                        List<FTObject> cmds = new List<FTObject>();

                        FTObject cmd = stack.Pop();
                        while (cmd.Type != FTType.StartCommandBlock)
                        {
                            cmds.Add(cmd);

                            cmd = stack.Pop();
                        }
                        int startCommandDepth = (int)cmd.Value;

                        if (startCommandDepth != commandRecordDepth)
                        {
                            // Woops!
                            throw new FTWoopsException(word);
                        }
                        commandRecordDepth--;
                        cmds.Reverse();
                        stack.Push(FTType.WordSet, cmds);
                    }
                    else
                        stack.Push(FTType.Word, word);

                    continue;
                }

                // not a number
                switch (word)
                {
                    case ".":
                        if (stack.Count == 0)
                            throw new FTStackUnderFlowException(word);
                        Console.Write(stack.Pop().Value);
                        break;
                    case "{":
                        commandRecordDepth++;
                        stack.Push(new FTObject(FTType.StartCommandBlock, commandRecordDepth));
                        break;
                    case "}":
                        if (commandRecordDepth <= 0)
                            throw new FTUnexpectedSymbolException(word);

                        break;
                    case "true":
                    case "false":
                        stack.Push(FTType.Bool, bool.Parse(word));
                        break;
                    case "cr":
                        Console.WriteLine();
                        break;
                    case "add":
                        CheckType2(word, FTType.Int);
                        stack.Push(FTType.Int, (int)a1.Value + (int)a2.Value);
                        break;
                    case "sub":
                        CheckType2(word, FTType.Int);
                        stack.Push(FTType.Int, (int)a2.Value - (int)a1.Value);
                        break;
                    case "mul":
                        CheckType2(word, FTType.Int);
                        stack.Push(FTType.Int, (int)a1.Value * (int)a2.Value);
                        break;
                    case "div":
                        CheckType2(word, FTType.Int);
                        stack.Push(FTType.Int, (int)a2.Value / (int)a1.Value);
                        break;
                    case "mod":
                        CheckType2(word, FTType.Int);
                        stack.Push(FTType.Int, (int)a2.Value % (int)a1.Value);
                        break;
                    case "clear":
                        stack.Clear();
                        dictionary.Clear();
                        break;
                    case "pstack":
                        Console.WriteLine("----------------------------\r\nStack    Count: {0}", stack.Count);
                        for (int i = 0; i < stack.Count; i++)
                        {
                            FTObject elem = stack.ElementAt(i);
                            if (elem.Type == FTType.String)
                                Console.WriteLine("{0} ->\tString:\t\"{1}\"", i, elem.Value);
                            else if (elem.Type == FTType.Int)
                                Console.WriteLine("{0} ->\tInt:\t{1}", i, elem.Value);
                            else if (elem.Type == FTType.Bool)
                                Console.WriteLine("{0} ->\tBool:\t{1}", i, elem.Value);
                            else
                                Console.WriteLine("{0} ->\t{1}", i, elem.Value);
                        }
                        Console.WriteLine("----------------------------");
                        break;
                    case "dup":
                        if (stack.Count == 0)
                            throw new FTStackEmptyException(word);
                        stack.Push(stack.Peek());
                        break;
                    case "exch":
                        if (stack.Count < 2)
                            throw new FTStackUnderFlowException(word);

                        FTObject b1 = stack.Pop();
                        FTObject b2 = stack.Pop();
                        stack.Push(b1);
                        stack.Push(b2);
                        break;
                    case "def":
                        if (stack.Count < 2)
                            throw new FTStackUnderFlowException(word);

                        FTObject v1 = stack.Pop();
                        FTObject v2 = stack.Pop();

                        if (v2.Type == FTType.NameDef)
                        {
                            string name =v2.Value as string;
                            if (dictionary.ContainsKey(name))
                                dictionary[name] = v1;
                            else
                                dictionary.Add(name, v1);
                        }
                        else
                        {                            
                            stack.Push(v2);
                            stack.Push(v1);
                            throw new FTInconsistentTypesException(word);
                        }

                        break;
                    case "pdict":
                        Console.WriteLine("----------------------------\r\nDict    Count: {0}", dictionary.Count);
                        for (int i = 0; i < dictionary.Count; i++)
                        {
                            string key = dictionary.Keys.ElementAt(i);
                            FTObject obj = dictionary[key];

                            Console.WriteLine("{0}\t->\t{1}", key, obj.Value);
                        }
                        Console.WriteLine("----------------------------");
                        break;
                    default:

                        if (dictionary.ContainsKey(word))
                        {
                            FTObject obj = dictionary[word];
                            if (obj.Type == FTType.WordSet)
                                Execute(obj);
                            else
                                stack.Push(obj);
                            continue;
                        }

                        throw new FTUnknownSymbolException(word);
                        break;
                }
            }
        }

        private void Execute(FTObject fobj)
        {
            if (fobj.Type == FTType.WordSet)
                Execute(fobj.AsWordSet());
        }

        private void CheckType2(string c, FTType type)
        {
            if (stack.Count < 2)
                throw new FTStackUnderFlowException(c);

            a1 = stack.Pop();
            a2 = stack.Pop();
            if (a1.Type != type || a2.Type != type)
            {
                stack.Push(a2);
                stack.Push(a1);
                throw new FTInconsistentTypesException(c);
            }
        }
    }
}