using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FortuoScript.Types;
using System.IO;

namespace FortuoScript
{
    public class FortuoInterpreter
    {
        enum RecordMode
        {
            Wordset, List
        }

        const char commentChar = '%';
        Regex splitRegex;

        Dictionary<string, FTObject> dictionary;
        FTStack stack;
        FTObject a1, a2, a3;

        string word;
        Stack<RecordMode> recordStack;

        public FortuoInterpreter()
        {
            stack = new FTStack();
            dictionary = new Dictionary<string, FTObject>();
            recordStack = new Stack<RecordMode>();
            word = "";

            // "(\\.|[^"\\])*"|([^ ]+)
            splitRegex = new Regex("\"(?:\\\\.|[^\"\\\\])*\"|[^ \t]+");
        }

        public bool Execute(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;

            // remove comments
            int firstCommentIndex = line.IndexOf(commentChar);
            if (firstCommentIndex >= 0)
                line = line.Substring(0, firstCommentIndex);

            if (string.IsNullOrWhiteSpace(line))
                return false;

            // split by white space
            var matches = splitRegex.Matches(line);
            
            foreach (Match m in matches)
            {
                word = m.Value;

                #region Values, Definition, WordSet-Recording
                if (TryPushString())
                    continue;
                if (TryPushInt())
                    continue;
                if (TryPushNameDef())
                    continue;
                if (Record())
                    continue;
                #endregion

                #region Word evaluation switch-statement
                switch (word)
                {
                    #region Reset / Clear
                    case "clear":
                        recordStack.Clear();
                        stack.Clear();
                        dictionary.Clear();
                        break;
                    case "delstack":
                        stack.Clear();
                        break;
                    case "deldict":
                        dictionary.Clear();
                        break;
                    case "ccon":
                        Console.Clear();
                        break;
                    #endregion
                    #region Output
                    case ".":
                        Pop1();
                        Console.Write(a1.Value);
                        break;
                    case "h":
                        CheckType1(FTType.Int);
                        Console.Write("0x{0:x}", (int)a1.Value);
                        break;
                    case "pstack":
                        Console.WriteLine("----------------------------\r\nStack    Count: {0}", stack.Count);
                        for (int i = 0; i < stack.Count; i++)
                        {
                            a1 = stack.ElementAt(i);
                            if (a1.Type == FTType.String)
                                Console.WriteLine("{0} ->\tString:\t\"{1}\"", i, a1.Value);
                            else if (a1.Type == FTType.Int)
                                Console.WriteLine("{0} ->\tInt:\t{1}", i, a1.Value);
                            else if (a1.Type == FTType.Bool)
                                Console.WriteLine("{0} ->\tBool:\t{1}", i, a1.Value);
                            else
                                Console.WriteLine("{0} ->\t{1}", i, a1.Value);
                        }
                        Console.WriteLine("----------------------------");
                        break;
                    case "pdict":
                        Console.WriteLine("----------------------------\r\nDict    Count: {0}", dictionary.Count);
                        for (int i = 0; i < dictionary.Count; i++)
                        {
                            string key = dictionary.Keys.ElementAt(i);
                            a1 = dictionary[key];

                            Console.WriteLine("{0}\t->\t{1}", key, a1.Value);
                        }
                        Console.WriteLine("----------------------------");
                        break;
                    case "cr":
                        Console.WriteLine();
                        break;
                    #endregion
                    #region Integer Manipulation
                    case "+":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Int, (int)a1.Value + (int)a2.Value);
                        break;
                    case "-":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Int, (int)a2.Value - (int)a1.Value);
                        break;
                    case "*":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Int, (int)a1.Value * (int)a2.Value);
                        break;
                    case "/":
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
                            throw new FTStackUnderFlowException("'dup': no items on stack.");
                        stack.Push(stack.Peek());
                        break;
                    case "swap":
                        Pop2();
                        stack.Push(a1);
                        stack.Push(a2);
                        break;
                    case "drop":
                        Pop1();
                        break;
                    #endregion
                    #region Definition
                    case "def":
                        Pop2();
                        if (a2.Type == FTType.NameDef)
                        {
                            string name = a2.Value as string;
                            if (dictionary.ContainsKey(name))
                                dictionary[name] = a1;
                            else
                                dictionary.Add(name, a1);
                        }
                        else
                            throw new FTWrongTypeException("'def': second top most object of stack has to be of type 'NameDef'.");
                        break;
                    case "undef":
                        CheckType1(FTType.NameDef);
                        string val = a1.Value as string;
                        if (dictionary.ContainsKey(val))
                            dictionary.Remove(val);
                        else
                            throw new FTUnexpectedSymbolException("variable '{0}' not defined in dictionary.");
                        break;
                    #endregion
                    #region WordSet
                    case "{":
                        recordStack.Push(RecordMode.Wordset);
                        stack.Push(FTType.StartSet, recordStack.Count);
                        break;
                    case "}":
                        if (recordStack.Count <= 0 || stack.Count <= 0)
                            throw new FTUnexpectedSymbolException("'}': Stack empty.");
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
                        CheckType1(FTType.Int);
                        stack.Push(FTType.Int, -(int)a1.Value);
                        break;
                    case "=":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Bool, (int)a1.Value == (int)a2.Value);
                        break;
                    case ">":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Bool, (int)a2.Value > (int)a1.Value);
                        break;
                    case "<":
                        CheckType2(FTType.Int);
                        stack.Push(FTType.Bool, (int)a2.Value < (int)a1.Value);
                        break;
                    #endregion
                    #region Bool Comparison
                    case "not":
                        CheckType1(FTType.Bool);
                        stack.Push(FTType.Bool, !(bool)a1.Value);
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
                    #region Flow Control
                    case "if":
                        CheckType2(FTType.WordSet, FTType.Bool);
                        if ((bool)a2.Value)
                            ExecuteWordSet((FTWordSet)a1.Value);
                        break;
                    case "ifelse":
                        CheckType3(FTType.WordSet, FTType.WordSet, FTType.Bool);
                        if ((bool)a3.Value)
                            ExecuteWordSet((FTWordSet)a2.Value);
                        else
                            ExecuteWordSet((FTWordSet)a1.Value);
                        break;
                    #endregion
                    #region Files
                    case "exec":
                        CheckType1(FTType.String);
                        ExecuteFile(a1.Value as string);
                        break;
                    #endregion
                    #region Input
                    case "linein":
                        stack.Push(FTType.String, Console.ReadLine());
                        break;
                    #endregion
                    #region String Manipulation
                    case "len":
                        CheckType1(FTType.String);
                        stack.Push(FTType.Int, (a1.Value as string).Length);
                        break;
                    case "concat":
                        CheckType2(FTType.String);
                        stack.Push(FTType.String, (string)a2.Value + (string)a1.Value);
                        break;
                    case "comp":
                        CheckType2(FTType.String);
                        stack.Push(FTType.Bool, ((string)a1.Value).Equals(a2.Value as string));
                        break;
                    case "trim":
                        CheckType1(FTType.String);
                        stack.Push(FTType.String, ((string)a1.Value).Trim());
                        break;
                    case "getchar":
                        CheckType2(FTType.Int, FTType.String);
                        stack.Push(FTType.Int, (int)((string)a2.Value)[(int)a1.Value]);
                        break;
                    case "substr":
                        CheckType3(FTType.Int, FTType.Int, FTType.String);
                        stack.Push(FTType.String, ((string)a3.Value).Substring((int)a2.Value, (int)a1.Value));
                        break;
                    #endregion
                    #region Loops
                    case "repeat":
                        CheckType2(FTType.WordSet, FTType.Int);
                        int num = (int)a2.Value;
                        FTWordSet s = (FTWordSet)a1.Value;
                        for (int i = 0; i < num; i++)
                            ExecuteWordSet(s);
                        break;
                    case "while":
                        CheckType2(FTType.WordSet, FTType.Bool);
                        bool cond = (bool)a2.Value;
                        FTWordSet set = (FTWordSet)a1.Value;
                        while (cond)
                        {
                            ExecuteWordSet(set);
                            CheckType1(FTType.Bool);
                            cond = (bool)a1.Value;
                        }
                        break;
                    #endregion
                    #region Coversion
                    case "tostr":
                        Pop1();
                        if (a1.Type != FTType.Int && a1.Type != FTType.Bool)
                            throw new FTWrongTypeException(string.Format("'tostr': object to convert is of type {0}.\r\ncan only parse types Int and Bool.", a1.Type));
                        stack.Push(FTType.String, a1.Value.ToString().ToLower());
                        break;
                    case "tobool":
                        CheckType1(FTType.Int);
                        stack.Push(FTType.Bool, (int)a1.Value != 0);
                        break;
                    #endregion
                    #region List
                    case "list":
                        stack.Push(FTType.List, new FTList());
                        break;
                    case "count":
                        Peek1();
                        if (a1.Type != FTType.List)
                            throw new FTWrongTypeException("'count' expects top object of stack to be from type FTList.");
                        stack.Push(FTType.Int, ((FTList)a1.Value).Count);
                        break;
                    case "add":
                        Pop2();
                        if (a2.Type != FTType.List)
                            throw new FTWrongTypeException("'add': second top most object of stack has to be of type FTList.");
                        ((FTList)a2.Value).Add(a1);
                        stack.Push(FTType.List, a2.Value);
                        break;
                    case "get":
                        CheckType2(FTType.Int, FTType.List);
                        stack.Push(FTType.List, a2.Value);
                        // TODO: add IndexOutOfBounds check and exception
                        stack.Push(((FTList)a2.Value)[(int)a1.Value]);
                        break;
                    case "set":
                        Pop3();
                        if (a3.Type != FTType.List || a1.Type != FTType.Int)
                            throw new FTWrongTypeException(word);
                        // TODO: add IndexOutOfBounds check and exception
                        ((FTList)a3.Value)[(int)a1.Value] = a2;
                        stack.Push(FTType.List, a3.Value);
                        break;
                    case "remove":
                        CheckType2(FTType.Int, FTType.List);
                        stack.Push(FTType.List, a2.Value);
                        // TODO: add IndexOutOfBounds check and exception
                        ((FTList)a2.Value).RemoveAt((int)a1.Value);
                        break;
                    case "[":
                        recordStack.Push(RecordMode.List);
                        stack.Push(FTType.StartList, recordStack.Count);
                        break;
                    case "]":
                        if (recordStack.Count <= 0 || stack.Count <= 0)
                            throw new FTUnexpectedSymbolException("']': Stack empty.");
                        break;
                    #endregion 
                    case "quit":
                        return true;
                        break;
                    #region default
                    default:
                        if (dictionary.ContainsKey(word))
                        {
                            FTObject v = dictionary[word];
                            if (v.Type == FTType.WordSet)
                                ExecuteWordSet((FTWordSet)v.Value);
                            else
                                stack.Push(v);
                            continue;
                        }

                        throw new FTUnknownSymbolException(string.Format("Unable to parse symbol: '{0}'", word));
                        break;
                    #endregion
                }
                #endregion
            }
            return false;
        }

        private void ExecuteWordSet(FTWordSet set)
        {
            foreach (FTObject o in set)
                Execute(o);
        }

        private void Execute(FTObject obj)
        {
            switch (obj.Type)
            { 
                case FTType.Bool:
                case FTType.Int:
                case FTType.String:
                case FTType.NameDef:
                case FTType.WordSet:
                    stack.Push(obj);
                    break;
                case FTType.Word:
                    Execute((string)obj.Value);
                    break;
            }
        }
        
        public void ExecuteFile(string path)
        {
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                foreach (var l in lines)
                    Execute(l);
            }
            else
                throw new FileNotFoundException(path + " not found.");
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
            if (word.StartsWith("/") && word.Length > 1)
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

        private bool Record()
        {
            if (recordStack.Count > 0)
            {
                switch (word)
                { 
                    case "[":
                        stack.Push(FTType.StartList, recordStack.Count);
                        recordStack.Push(RecordMode.List);
                        break;
                    case "]":
                        FTList list = new FTList();

                        while (true)
                        {
                            Pop1();

                            if (a1.Type == FTType.StartSet)
                                throw new FTUnexpectedSymbolException("Encountered '{' (start of WordSet) when parsing List.");
                            if (a1.Type == FTType.StartList)
                                break;
                            list.Add(a1);
                        }

                        if ((int)a1.Value != recordStack.Count || a1.Type != FTType.StartList)
                            throw new FTWoopsException(word);
                        recordStack.Pop();
                        list.Reverse();
                        stack.Push(FTType.List, list);
                        break;
                    case "{":
                        recordStack.Push(RecordMode.Wordset);
                        stack.Push(FTType.StartSet, recordStack.Count);
                        break;
                    case "}": 
                        FTWordSet cmds = new FTWordSet();

                        while (true)
                        {
                            Pop1();

                            if (a1.Type == FTType.StartList)
                                throw new FTUnexpectedSymbolException("Encountered '[' (start of List) when parsing WordSet.");
                            if (a1.Type == FTType.StartSet)
                                break;

                            cmds.Add(a1);
                        }
                        
                        if ((int)a1.Value != recordStack.Count || a1.Type != FTType.StartSet)
                            throw new FTWoopsException(word);
                        recordStack.Pop();
                        cmds.Reverse();
                        stack.Push(FTType.WordSet, cmds);
                        break;
                    default:
                        if (recordStack.Peek() == RecordMode.List)
                        {
                            if (TryPushInt())
                                return true;
                            else if (TryPushString())
                                return true;
                            else if (TryPushNameDef())
                                return true;
                        }
                        stack.Push(FTType.Word, word);
                        break;
                }
                return true;
            }
            return false;
        }

        private void Peek1()
        { 
            if (stack.Count == 0)
                throw new FTStackUnderFlowException(string.Format("Command '{0}' tried to peek 1 Object.", word));
            a1 = stack.Peek();
        }

        private void Pop1()
        {
            if (stack.Count == 0)
                throw new FTStackUnderFlowException(string.Format("Command '{0}' tried to pop 1 Object.", word));
            a1 = stack.Pop();
        }

        private void Pop2()
        {
            if (stack.Count < 2)
                throw new FTStackUnderFlowException(string.Format("Command '{0}' tried to pop 2 Objects.", word));

            a1 = stack.Pop();
            a2 = stack.Pop();
        }

        private void Pop3()
        {
            if (stack.Count < 3)
                throw new FTStackUnderFlowException(string.Format("Command '{0}' tried to pop 3 Objects.", word));

            a1 = stack.Pop();
            a2 = stack.Pop();
            a3 = stack.Pop();
        }

        private void CheckType1(FTType type)
        {
            Pop1();
            if (a1.Type != type)
                throw new FTWrongTypeException(string.Format("Wrong Argument Types for Command: '{0}'\r\nExpecting {1}", word, type));
        }

        private void CheckType2(FTType type)
        {
            Pop2();
            if (a1.Type != type || a2.Type != type)
                throw new FTWrongTypeException(string.Format("Wrong Argument Types for Command: '{0}'\r\nExpecting {1}, {2}", word, type, type));
        }

        private void CheckType2(FTType type1, FTType type2)
        {
            Pop2();
            if (a1.Type != type1 || a2.Type != type2)
                throw new FTWrongTypeException(string.Format("Wrong Argument Types for Command: '{0}'\r\nExpecting {1}, {2}", word, type1, type2));
        }

        private void CheckType3(FTType type1, FTType type2, FTType type3)
        {
            Pop3();
            if (a3.Type != type3 || a2.Type != type2 || a1.Type != type1)
                throw new FTWrongTypeException(string.Format("Wrong Argument Types for Command: '{0}'\r\nExpecting {1}, {2}, {3}", word, type1, type2, type3));
        }
    }
}