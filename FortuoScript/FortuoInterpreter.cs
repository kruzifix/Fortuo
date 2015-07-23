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

        string word;
        int commandRecordDepth;

        public FortuoInterpreter()
        {
            stack = new FTStack();
            dictionary = new Dictionary<string, FTObject>();
            commandRecordDepth = 0;
            word = "";

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
                word = m.Value;

                // Values, Definition, WordSet-Recording
                if (TryPushString() | TryPushInt() | TryPushNameDef() | EvalRecording())
                    continue;

                FTObject obj;
                #region Word evaluation switch-statement
                switch (word)
                {
                    #region Reset / Clear
                    case "clear":
                        commandRecordDepth = 0;
                        stack.Clear();
                        dictionary.Clear();
                        break;
                    case "delstack":
                        stack.Clear();
                        break;
                    case "deldict":
                        dictionary.Clear();
                        break;
                    #endregion
                    #region Output
                    case ".":
                        if (stack.Count == 0)
                            throw new FTStackUnderFlowException(word);
                        Console.Write(stack.Pop().Value);
                        break;
                    case "pstack":
                        Console.WriteLine("----------------------------\r\nStack    Count: {0}", stack.Count);
                        for (int i = 0; i < stack.Count; i++)
                        {
                            obj = stack.ElementAt(i);
                            if (obj.Type == FTType.String)
                                Console.WriteLine("{0} ->\tString:\t\"{1}\"", i, obj.Value);
                            else if (obj.Type == FTType.Int)
                                Console.WriteLine("{0} ->\tInt:\t{1}", i, obj.Value);
                            else if (obj.Type == FTType.Bool)
                                Console.WriteLine("{0} ->\tBool:\t{1}", i, obj.Value);
                            else
                                Console.WriteLine("{0} ->\t{1}", i, obj.Value);
                        }
                        Console.WriteLine("----------------------------");
                        break;
                    case "pdict":
                        Console.WriteLine("----------------------------\r\nDict    Count: {0}", dictionary.Count);
                        for (int i = 0; i < dictionary.Count; i++)
                        {
                            string key = dictionary.Keys.ElementAt(i);
                            obj = dictionary[key];

                            Console.WriteLine("{0}\t->\t{1}", key, obj.Value);
                        }
                        Console.WriteLine("----------------------------");
                        break;
                    case "cr":
                        Console.WriteLine();
                        break;
                    #endregion
                    #region Integer Manipulation
                    case "add":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Int, (int)a1.Value + (int)a2.Value);
                        break;
                    case "sub":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Int, (int)a2.Value - (int)a1.Value);
                        break;
                    case "mul":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Int, (int)a1.Value * (int)a2.Value);
                        break;
                    case "div":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Int, (int)a2.Value / (int)a1.Value);
                        break;
                    case "mod":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Int, (int)a2.Value % (int)a1.Value);
                        break;
                    #endregion
                    #region Stack Manipulation
                    case "dup":
                        if (stack.Count == 0)
                            throw new FTStackUnderFlowException(word);
                        stack.Push(stack.Peek());
                        break;
                    case "swap":
                        if (stack.Count < 2)
                            throw new FTStackUnderFlowException(word);

                        a1 = stack.Pop();
                        a2 = stack.Pop();
                        stack.Push(a1);
                        stack.Push(a2);
                        break;
                    #endregion
                    #region Definition
                    case "def":
                        if (stack.Count < 2)
                            throw new FTStackUnderFlowException(word);

                        a1 = stack.Pop();
                        a2 = stack.Pop();

                        if (a2.Type == FTType.NameDef)
                        {
                            string name = a2.Value as string;
                            if (dictionary.ContainsKey(name))
                                dictionary[name] = a1;
                            else
                                dictionary.Add(name, a1);
                        }
                        else
                        {
                            stack.Push(a2);
                            stack.Push(a1);
                            throw new FTWrongTypeException(word);
                        }

                        break;
                    #endregion
                    #region WordSet
                    case "{":
                        commandRecordDepth++;
                        stack.Push(FTType.StartSet, commandRecordDepth);
                        break;
                    case "}":
                        if (commandRecordDepth <= 0)
                            throw new FTUnexpectedSymbolException(word);
                        break;
                    #endregion
                    #region Values
                    case "true":
                    case "false":
                        stack.Push(FTType.Bool, bool.Parse(word));
                        break;
                    #endregion
                    #region Integer Comparison
                    case "neg":
                        if (stack.Count == 0)
                            throw new FTStackUnderFlowException(word);

                        obj = stack.Pop();
                        if (obj.Type != FTType.Int)
                            throw new FTWrongTypeException(word);

                        obj.Value = -(int)obj.Value;
                        stack.Push(obj);
                        break;
                    case "eq":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Bool, (int)a1.Value == (int)a2.Value);
                        break;
                    case "gt":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Bool, (int)a2.Value > (int)a1.Value);
                        break;
                    case "lt":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Bool, (int)a2.Value < (int)a1.Value);
                        break;
                    #endregion
                    #region Bool Comparison
                    case "not":
                        if (stack.Count == 0)
                            throw new FTStackUnderFlowException(word);

                        obj = stack.Pop();
                        if (obj.Type != FTType.Bool)
                            throw new FTWrongTypeException(word);

                        obj.Value = !(bool)obj.Value;
                        stack.Push(obj);
                        break;
                    case "and":
                        CheckType2(FTType.Bool);
                        stack.Push(FTType.Bool, (bool)a1.Value && (bool)a2.Value);
                        break;
                    case "or":
                        CheckType2(FTType.Bool);
                        stack.Push(FTType.Bool, (bool)a1.Value || (bool)a2.Value);
                        break;
                    #endregion
                    
                    default:
                        if (dictionary.ContainsKey(word))
                        {
                            obj = dictionary[word];
                            if (obj.Type == FTType.WordSet)
                                Execute(obj);
                            else
                                stack.Push(obj);
                            continue;
                        }

                        throw new FTUnknownSymbolException(word);
                        break;
                }
                #endregion
            }
        }

        private void Execute(FTObject fobj)
        {
            if (fobj.Type == FTType.WordSet)
                Execute(fobj.AsWordSet());
        }

        private bool TryPushString()
        {
            if (word.StartsWith("\"") && word.EndsWith("\""))
            {
                string str = word.Substring(1, word.Length - 2).Replace("\\\"", "\"");
                stack.Push(FTType.String, str);
                return true;
            }
            return false;
        }

        private bool TryPushNameDef()
        {
            if (word.StartsWith("/"))
            {
                string key = word.Substring(1);
                stack.Push(FTType.NameDef, key);
                return true;
            }
            return false;
        }

        private bool TryPushInt()
        {
            int num;
            if (int.TryParse(word, out num))
            {
                // add number to stack
                stack.Push(FTType.Int, num);
                // continue to next cmd
                return true;
            }
            return false;
        }

        private bool EvalRecording()
        {
            if (commandRecordDepth > 0)
            {
                if (word == "}")
                {
                    List<FTObject> cmds = new List<FTObject>();

                    FTObject cmd = stack.Pop();
                    while (cmd.Type != FTType.StartSet)
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

                return true;
            }
            return false;
        }

        private void CheckType2(FTType type)
        {
            if (stack.Count < 2)
                throw new FTStackUnderFlowException(word);

            a1 = stack.Pop();
            a2 = stack.Pop();
            if (a1.Type != type || a2.Type != type)
            {
                stack.Push(a2);
                stack.Push(a1);
                throw new FTWrongTypeException(word);
            }
        }
    }
}