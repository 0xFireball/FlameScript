using System.Globalization;

namespace FlameScript.Lexing
{
    public static class CharClassifier
    {
        public static CharType GetCharType(this char c)
        {
            //First the small sets
            switch (c)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '%':
                case '&':
                case '|':
                case '=':
                    return CharType.Operator;

                case '(':
                case '[':
                case '{':
                    return CharType.OpenBrace;

                case ')':
                case ']':
                case '}':
                    return CharType.CloseBrace;

                case '"':
                    return CharType.StringDelimiter;

                case ',':
                    return CharType.ArgSeperator;

                case ';':
                    return CharType.StatementSeperator;

                case '\r': //\r and \n have UnicodeCategory.Control, not LineSeperator...
                case '\n':
                case '\t':
                    return CharType.NewLine;

                case '.':
                    return CharType.MemberAccess;
            }

            //then the categories
            switch (char.GetUnicodeCategory(c))
            {
                case UnicodeCategory.DecimalDigitNumber:
                    return CharType.Numeric;

                case UnicodeCategory.LineSeparator: //just in case... (see above)
                    return CharType.NewLine;

                case UnicodeCategory.ParagraphSeparator:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.UppercaseLetter:
                    return CharType.Alpha;

                case UnicodeCategory.SpaceSeparator:
                    return CharType.LineSpace;
            }

            return CharType.Unknown; //something really odd, we could probably allow it as a CharType.Alpha, when its not a Control-Char.
        }
    }
}