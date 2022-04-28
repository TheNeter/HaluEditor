using System.Text;

namespace ngprojects.HaluEditor.Document
{
    public class HaluDocument
    {
        private GapBuffer<HaluRow> _rows = new GapBuffer<HaluRow>();

        //TODO handle all newline characters https://en.wikipedia.org/wiki/Newline#Unicode

        public HaluDocument()
        {
        }

        public int Count
        {
            get
            {
                return Rows.Count;
            }
        }

        public string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int x = 0; x < Count; x++)
                {
                    for (int i = 0; i < this[x].Count; i++)
                    {
                        sb.Append(this[x][i]);
                    }
                }
                return sb.ToString();
            }
            set
            {
                SetText(value);
            }
        }

        internal GapBuffer<HaluRow> Rows
        {
            get
            {
                if (_rows.Count == 0)
                {
                    AddRow();
                }
                return _rows;
            }
            set => _rows = value;
        }

        public HaluRow this[int index]
        {
            get
            {
                if (index >= 0 && index < Rows.Count)
                {
                    return Rows[index];
                }
                return null;
            }
        }

        public HaluRow AddRow()
        {
            HaluRow haluRow = new HaluRow();
            _rows.Add(haluRow);
            return haluRow;
        }

        public bool AddRowToDocumentIfNecessary()
        {
            if (this[Count - 1].Count != this[Count - 1].End)
            {
                AddRow();
                return true;
            }
            return false;
        }

        public HaluRow InsertRow(int Position)
        {
            HaluRow haluRow = new HaluRow();
            Rows.Insert(Position, haluRow);
            return haluRow;
        }

        public void RemoveRow(int Position)
        {
            Rows.RemoveAt(Position);
        }

        internal void Clear()
        {
            Rows = new GapBuffer<HaluRow>();
            AddRow();
        }

        internal void MergeRows(int from, int to)
        {
            if (from != 0 && from != Count)
            {
                Rows[to].RemoveLineEnding();
                Rows[to].Letters.InsertRange(Rows[to].Letters.Count, Rows[from].Letters);
                RemoveRow(from);
            }
        }

        internal void SplitRow(int row, int cursorInRow)
        {
            InsertRow(row + 1);
            Rows[row + 1].Letters.InsertRange(0, Rows[row].Letters.GetRange(cursorInRow, Rows[row].Letters.Count - cursorInRow));
            Rows[row].Letters.RemoveRange(cursorInRow, Rows[row + 1].Letters.Count);
        }

        private void SetText(string value)
        {
            Clear();
            int top = 0;

            if (!string.IsNullOrEmpty(value))
            {
                for (int i = 0; i < value.Length; i++)
                {
                    int index = value.IndexOf('\r', i);
                    if (index >= 0)
                    {
                        if (value[index].Equals('\r') && index + 1 < value.Length && value[index + 1].Equals('\n'))
                        {
                            var segment = value.Substring(i, index + 2 - i);
                            this[top].Letters.AddRange(segment.ToCharArray());
                            AddRow();
                            top++;
                            i = index + 1;
                        }
                        else
                        {
                            var segment = value.Substring(i, index + 1 - i);
                            this[top].Letters.AddRange(segment.ToCharArray());
                            AddRow();
                            top++;
                            i = index;
                        }
                    }
                    else
                    {
                        var segment = value.Substring(i);
                        this[top].Letters.AddRange(segment.ToCharArray());
                        break;
                    }
                }
            }

            AddRowToDocumentIfNecessary();
        }
    }
}