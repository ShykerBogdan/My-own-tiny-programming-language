using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Freel.Entities;

namespace Freel.Model
{
    class Lexer

    {
        private static List<string> terminalTokens = new List<string>
        {   "Program", "var","int", "float", "double", "read", "write", "if", "then",
            "for", "do", "or", "and", "not", "{", "}", ";", ",", "=", "(",
            ")","?",":", "+", "-", "*", "/", "[", "]", "<", "<=", ">", ">=", "==", "!=", "idn", "con"};
         
    private static List<char> tokenSeparators = new List<char>
        { '{', '}', ';', ',', '=', '(', ')', ':', '+', '-', '*', '/', '[', ']', '<', '>'};
        private static List<char> whiteSeparators = new List<char>
        { '\n', '\t', ' ', '\r' };

        private List<Token> inputTokens;
        private List<Constant> inputConstants;
        private List<Identifier> inputIdentifiers;
        private List<Error> erorrList;

        private bool varDeclareFlag;       

        private int rowCount;

        public (List<Token> outputTokens, List<Identifier> outputIdentifiers, List<Constant> outoutConstans, List<Error> errors) Run(string programText)
        {
            rowCount = 1;
            varDeclareFlag = false;

            inputTokens = new List<Token>(programText.Split(' ').Length);
            inputConstants = new List<Constant>();
            inputIdentifiers = new List<Identifier>();
            erorrList = new List<Error>();

            bool result = true;

            string lex = string.Empty;        

            for (int i = 0; i < programText.Length; i++)
            {
                if (tokenSeparators.Contains(programText[i]))
                {
                    if ((programText[i] == '-' || programText[i] == '+') &&
                        (programText[i - 1] == 'E' || programText[i - 1] == 'e'))
                    {
                        lex += programText[i];
                        continue;
                    }
                    if (lex != "")
                        result = checkLex(lex);
                    if (!result)
                        break;
                    switch (programText[i])
                    {
                        case '=':
                            if (programText[i + 1] == '=')
                            {
                                checkLex("==");
                                i++;
                            }
                            else
                                checkLex("=");
                            break;

                        case '<':
                            if (programText[i + 1] == '=')
                            {
                                checkLex("<=");
                                i++;
                            }
                            else if (programText[i + 1] == '>')
                            {
                                checkLex("<>");
                                i++;
                            }
                            else
                                checkLex("<");
                            break;

                        case '>':
                            if (programText[i + 1] == '=')
                            {
                                checkLex(">=");
                                i++;
                            }
                            else
                                checkLex(">");
                            break;

                        case '!':
                            if (programText[i + 1] == '=')
                            {
                                checkLex("!=");
                                i++;
                            }
                            else
                                checkLex("!");
                            break;

                        default:
                            result = checkLex(programText[i].ToString());
                            if (!result)
                                break;
                            break;
                    }

                    lex = string.Empty; ;
                }
                else if (whiteSeparators.Contains(programText[i]))
                {
                    if (programText[i] == '\r' )
                        rowCount++;
                    if (lex != "")
                        result = checkLex(lex.ToString());
                    if (!result)
                    break;
                    lex = string.Empty;
                }
                else
                {
                    lex += programText[i];
                }
            }
            
            return (inputTokens, inputIdentifiers, inputConstants, erorrList);
        }

        private bool checkLex(string lex)
        {
            if (terminalTokens.Contains(lex))
            {
                if (lex == "-")
                {
                    Token lastToken = (Token)inputTokens[inputTokens.Count - 1];
                    if ((lastToken.Value == "idn") || (lastToken.Value == "con"))
                        lex = "-B";
                    else lex = "-U";
                }
                inputTokens.Add(new Token(lex, getTokenIndex(lex), rowCount));
                if (!varDeclareFlag && (lex == "strict" || lex == "int" || lex == "double"))
                    varDeclareFlag = true;
                else if ((varDeclareFlag) && (lex == ";"))
                    varDeclareFlag = false;
                return true;
            }

            else if (Regex.IsMatch(lex, "^[0-9]+[.]$|^[0-9]*[.]?[0-9]+$|^[0-9]*[.]?[0-9]*[eE][-+]?[0-9]+$"))
            {
                Constant constant = inputConstants.Find(cons => cons.Value == lex);
                if (constant != null)
                    inputTokens.Add(new Constant(constant.Value, constant.Index, rowCount, constant.ClassIndex));
                else
                {
                    inputConstants.Add(new Constant(lex, terminalTokens.IndexOf("con"), rowCount, inputConstants.Count + 1));
                    inputTokens.Add(inputConstants.Last());
                }
                return true;
            }

            else if (Regex.IsMatch(lex, "^[a-zA-Z][a-zA-Z0-9]*$"))
            {
                if ((varDeclareFlag) || (inputIdentifiers.Count == 0))
                {
                    if (IsContainsIdentifire(lex))
                    {
                        registrateError(1, lex);
                        return false;
                    }
                    inputIdentifiers.Add(new Identifier(lex, terminalTokens.IndexOf("idn"), rowCount, inputIdentifiers.Count + 1));
                    inputTokens.Add(inputIdentifiers.Last());
                    return true;
                }

                if (IsContainsIdentifire(lex) && !IsIdentifireProgrmaName(lex))
                {
                    Identifier ident = inputIdentifiers.First(idn => idn.Value.Equals(lex));
                    inputTokens.Add(new Identifier(ident.Value, ident.Index, rowCount, ident.ClassIndex));
                    return true;

                }
            }
            registrateError(0, lex);
            return false;
        }

        private bool IsContainsIdentifire(string lex)
        {
            try
            {
                Identifier identifier = inputIdentifiers.First(idn => idn.Value.Equals(lex));
                return inputIdentifiers.Contains(identifier);
            }
            catch
            {
                return false;
            }

        }

        private bool IsIdentifireProgrmaName(string lex)
                       => inputIdentifiers.First().Value.Equals(lex);

        private int getTokenIndex(string token)
        {
            if (token == "-U")
                return terminalTokens.IndexOf("-");
            if (token == "-B")
                return terminalTokens.IndexOf("-") + 1;
            for (int i = 0; i < terminalTokens.IndexOf("-") - 1; i++)
            {
                if (terminalTokens[i].Equals(token)) return i + 1;
            }
            for (int i = terminalTokens.IndexOf("-"); i < terminalTokens.Count; i++)
            {
                if (terminalTokens[i].Equals(token)) return i + 2;
            }
            return 0;
        }

        private void registrateError(int code, String lex)
        {
            switch (code)
            {
                case 0:
                    erorrList.Add(new Error(erorrList.Count+1,rowCount, "Wrong input: " + lex));
                    break;
                case 1:
                    erorrList.Add(new Error(erorrList.Count+1, rowCount,"ID was already declared: " + lex ));
                    break;
            }
        }
    }
}
