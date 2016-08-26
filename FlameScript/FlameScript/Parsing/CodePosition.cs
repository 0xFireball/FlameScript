namespace FlameScript.Parsing
{
    public struct CodePosition
    {
        /// <summary>
        /// The line number of the position
        /// </summary>
        public int Line;

        /// <summary>
        /// The column number of the position
        /// </summary>
        public int Column;

        public CodePosition(int line, int column)
        {
            Line = line;
            Column = column;
        }
    }
}