
using System;
using Freel.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freel.Model
{
    public class ParserRecursiveDescent
    {
        private List<Token> lexemes;
        private List<Error> parserErrors;
        private int currentRow;
        private int i;
        private bool errorFlag = false;
        public string Rezult { get; set; }
        private int expressionBreackets = 0;
        public ParserRecursiveDescent()
        {
            parserErrors = new List<Error>();
            Rezult = "";
            currentRow = 1;
            i = 0;
        }

        public List<Error> Process(List<Token> lexemes)
        {
            parserErrors.Clear();
            this.lexemes = lexemes;
            currentRow = 1;
            i = 0;
            Program();
            if (parserErrors.Count == 0)
                Rezult = "Success, 0 errors registrated";
            else
                Rezult = "Build failed, check the errorlist.";
            return parserErrors;
        }

        private bool Program()
        {
            try
            {
                if (CheckLexeme("Program") &&
                    CheckLexeme("idn") &&
                    DeclarationList() &&
                    CheckLexeme("{") &&
                    OperatorsList() &&
                    CheckLexeme("}")
                    )
                {
                    while ((i < lexemes.Count) && lexemes[i].GeneralizedValue.Equals("NL"))
                    {
                        i++;
                    }
                    if (i < lexemes.Count)
                    {
                        Error("Code after programm end");
                        return false;
                    }
                    else
                    {

                        return true;
                    }
                }
                else
                {
                   // currentRow--;
                     Error("Error");
                }
            }
            catch (Exception)
            {
                ErrorOut("Error");
            }
            return false;
        }

        private bool DeclarationList()
        {
            try
            {
                if (Declaration())
                {
                    if (CheckLexeme(";"))
                    {
                        int savedI = i;
                        while (Declaration())
                        {
                            if (!CheckLexeme(";"))
                                return false;
                            else
                            {
                                savedI = i;
                            }
                        }
                        i = savedI;
                        return true;
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErrorOut("Error");
            }
            Error("Error in declaration list");
            return false;
        }

        private bool Declaration()
        {
            try
            {
                if (CheckLexeme("var"))
                    if (CheckLexeme("int") || CheckLexeme("float") || CheckLexeme("double"))
                    {
                        if (IdentifiersList())
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }

            }
            catch (ArgumentOutOfRangeException)
            {
                currentRow--;
                ErrorOut("Error");
            }
            return false;
        }

        private bool IdentifiersList()
        {
            try
            {
                if (CheckLexeme("idn"))
                {
                    int savedI = i;
                    while (CheckLexeme(","))
                    {
                        if (!CheckLexeme("idn"))
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                currentRow--;
                ErrorOut("Error");
            }
            currentRow--;
            Error("Error in declaration");
            return false;
        }

        private bool OperatorsList()
        {
            try
            {
                if (Operator() && CheckLexeme(";"))
                {
                    int savedI = i;
                    while (Operator())
                    {
                        if (!CheckLexeme(";"))
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErrorOut("Error");
            }
            return false;
        }

        private bool Operator()
        {
            try
            {
                int saveID = i;
                if (CheckTernaryOperator())
                {
                    return true;
                }
                else
                {
                    i = saveID;
                    if (CheckRead())
                    {
                        return true;
                    }
                    else
                    {
                        i = saveID;
                        if (CheckWrite())
                        {
                            return true;
                        }
                        else
                        {
                            i = saveID;
                            if (CheckFor())
                            {
                                return true;
                            }
                            else
                            {
                                i = saveID;
                                if (CheckIf())
                                {
                                    return true;
                                }
                                else
                                {
                                    i = saveID;
                                    if (CheckAssignment())
                                    {
                                        return true;
                                    }
                                }


                            }
                        }

                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErrorOut("Error");
            }
            return false;
        }

        private bool CheckAssignment()
        {
            if (CheckLexeme("idn"))
            {
                if (CheckLexeme("=") && Expression())
                    return true;
                else
                {
                   // Error("Wrong code after identifier");
                    return false;
                }
            }
            else
                return false;

        }

        private bool CheckWrite()
        {
            if (CheckLexeme("write"))
            {
                if (CheckLexeme("(") && IdentifiersList() && CheckLexeme(")"))
                    return true;
                else
                {
                    Error("Wrong code after write function");
                    return false;
                }
            }
            else
                return false;
        }

        private bool CheckRead()
        {
            if (CheckLexeme("read"))
            {
                if (CheckLexeme("(") && IdentifiersList() && CheckLexeme(")"))
                    return true;
                else
                {
                    Error("Wrong code after read function");
                    return false;
                }
            }
            else
                return false;
        }

        private bool CheckTernaryOperator()
        {
            return (CheckLexeme("idn") && CheckLexeme("=") && Relation()
                                    && CheckLexeme("?") && Expression() && CheckLexeme(":")
                                    && Expression());

        }

        private bool CheckFor()
        {

            if (CheckLexeme("for"))
            {
                if (CheckLexeme("(") && CheckLexeme("idn") && CheckLexeme("=") && Expression()
                    && CheckLexeme(";") && LogicalExpression()
                    && CheckLexeme(";") && Expression() && CheckLexeme(")")
                    && CheckLexeme("{"))
                {

                    if (OperatorsList())
                    {
                        if (CheckLexeme("}"))
                            return true;
                        else
                        {
                            Error("Error in FOR definition");
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else
                {
                    Error("Errore in FOR definition");
                    return false;
                }

            }
            else
                return false;
        }

        private bool CheckIf()
        {
            if (CheckLexeme("if"))
            {
                if (CheckLexeme("(") && LogicalExpression() && CheckLexeme(")") && CheckLexeme("{"))
                {
                    if (OperatorsList())
                    {
                        if (CheckLexeme("}"))
                            return true;
                        else
                        {
                            Error("Error in IF definition");
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else
                {
                    Error("Error in IF definition");
                    return false;
                }
            }
            else
                return false;
        }

        private bool Expression()
        {
            try
            {
                CheckLexeme("-");
                if (Term())
                {
                    int savedI = i;
                    while (CheckLexeme("+") || (CheckLexeme("-")))
                    {
                        if (!Term())
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErrorOut("Error in expression");
            }
            return false;
        }

        private bool Term()
        {
            try
            {
                if (Mult())
                {
                    int savedI = i;
                    while (CheckLexeme("*") || (CheckLexeme("/")))
                    {
                        if (!Mult())
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErrorOut("Error in expression");
            }
            return false;
        }

        private bool Mult()
        {
            bool flag = false;
            try
            {

                if (CheckLexeme("con") || CheckLexeme("idn"))
                {
                    //if (expressionBreackets == 0 && !CheckLexeme(")"))
                    //    return true;
                    //if (expressionBreackets > 0)
                        return true;
                }
                else
                {
                    if (CheckLexeme("("))
                    {
                       // expressionBreackets++;
                        if (Expression() && CheckLexeme(")"))
                        {
                       //     expressionBreackets--;
                            return true;
                        }
                    }
                }
            }

            catch (ArgumentOutOfRangeException)
            {
                ErrorOut("Error");
            }
            return false;
        }

        private bool RelationSigns()
        {
            try
            {
                if (CheckLexeme("<") || CheckLexeme(">") || CheckLexeme("==")
                        || CheckLexeme("<>") || CheckLexeme("<=") || CheckLexeme(">="))
                    return true;

            }
            catch (ArgumentOutOfRangeException)
            {
                ErrorOut("Error");
            }
          //  Error("Wrong sing in logical expression");
            return false;
        }

        private bool LogicalExpression()
        {
            try
            {
                if (LogicalTerm())
                {
                    int savedI = i;
                    while (CheckLexeme("or"))
                    {
                        if (!LogicalTerm())
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErrorOut("Error");
                return false;
            }
       //     Error("Error in logical expression");
            return false;
        }

        private bool LogicalTerm()
        {
            try
            {
                if (LogicalMult())
                {
                    int savedI = i;
                    while (CheckLexeme("and"))
                    {
                        if (!LogicalMult())
                            return false;
                        else
                        {
                            savedI = i;
                        }
                    }
                    i = savedI;
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                ErrorOut("Error");
                return false;
            }
            return false;
        }

        private bool LogicalMult()
        {
            try
            {
                int savedI = i;
                while (CheckLexeme("not"))
                {
                    savedI = i;
                }
                i = savedI;
                if (Relation())
                {
                    return true;
                }
                i = savedI;
                if (CheckLexeme("[") && LogicalExpression() && CheckLexeme("]"))
                    return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                ErrorOut("Error");
                return false;
            }
            return false;
        }

        private bool Relation()
        {
            try
            {
                if (Expression() && RelationSigns() && Expression())
                    return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                ErrorOut("Error");
            }
            return false;
        }

        private bool CheckLexeme(string str)
        {
            if (Next().Equals(str))
            {
                i++;
                return true;
            }
            return false;

        }

        private string Next()
        {
            currentRow = lexemes[i].Row;
            return lexemes[i].GeneralizedValue;
        }

        private void Error(string str)
        {
            parserErrors.Add(new Error(parserErrors.Count + 1, currentRow, str));
            //if (!errorFlag)
            //{
            //    result = str + Environment.NewLine;
            //    result += "Row " + currentRow + Environment.NewLine;
            //    errorFlag = true;
            //}
        }

        private void ErrorOut(string str)
        {
            parserErrors.Add(new Error(parserErrors.Count + 1, currentRow, str));
            //if (!errorFlag)
            //{
            //    result = str + Environment.NewLine;
            //    result += "Row " + (currentRow + 1) + Environment.NewLine;
            //    errorFlag = true;
            //}
        }
    }
}
