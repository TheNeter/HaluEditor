using System.Diagnostics;
using System.Text;

namespace ngprojects.HaluEditor.Document
{
    [DebuggerDisplay("Count = {Count}; End = {End}")]
    public class HaluRow
    {
        public HaluRow()
        {
            Letters = new GapBuffer<char>();
        }

        public int Count
        {
            get
            {
                return Letters.Count;
            }
        }

        /// <summary>
        /// </summary>
        public int End
        {
            get
            {
                int lcCount = Count;
                if (lcCount == 0)
                {
                    return 0;
                }
                else if (lcCount == 1 && "\r\n".Contains(this[lcCount - 1]))
                {
                    return 0;
                }
                else if (lcCount > 1)
                {
                    string end = "" + this[lcCount - 2] + this[lcCount - 1];
                    if (end.Equals("\r\n"))
                    {
                        return lcCount - 2;
                    }
                    else if (end.Contains("\r") || end.Contains("\n"))
                    {
                        return lcCount - 1;
                    }
                }
                return lcCount;
            }
        }

        public string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var x in Letters)
                {
                    sb.Append(x);
                }
                return sb.ToString();
            }
        }

        internal GapBuffer<char> Letters { get; set; }

        public char this[int index]
        {
            get
            {
                if (IsInRange(index))
                {
                    return Letters[index];
                }
                return '\0';
            }
        }

        public char AddLetter(char Letter)
        {
            return InsertLetter(Count, Letter);
        }

        public char InsertLetter(int Position, char letter)
        {
            if (IsInRangeInclEnd(Position))
            {
                Letters.Insert(Position, letter);
                return letter;
            }
            return '\0';
        }

        public char RemoveLetter(int Position)
        {
            char retVal = '\0';
            if (IsInRange(Position))
            {
                retVal = Letters[Position];
                Letters.RemoveAt(Position);
            }
            return retVal;
        }

        internal void RemoveLineEnding()
        {
            while (Count != 0 && Count != End)
            {
                RemoveLetter(Count - 1);
            }
        }

        private bool IsInRange(int index)
        {
            return index >= 0 && index < Count;
        }

        private bool IsInRangeInclEnd(int index)
        {
            return index >= 0 && index <= Count;
        }
    }
}