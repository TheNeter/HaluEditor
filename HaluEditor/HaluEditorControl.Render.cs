using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ngprojects.HaluEditor.Lexer;

namespace ngprojects.HaluEditor
{
    public partial class HaluEditorControl : Canvas
    {
        private const double rectToTextMargin = 2;
        private readonly DpiScale _dpi;
        private FormattedText _defaultchar = Utils.FormatText("X", 20);
        private double _fontSize;
        public int FirstRenderedColumn { get; private set; } = 0;
        public int FirstRenderedRow { get; private set; } = 0;
        public double FontHeightFixed { get; private set; }

        public double FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                if (value > 0)
                {
                    _fontSize = value;
                    if (caret != null)
                    {
                        caret.Height = _fontSize;
                    }
                    _defaultchar = Utils.FormatText("X", _fontSize, dpi: _dpi);
                    FontHeightFixed = _defaultchar.Height;
                    FontWidthFixed = _defaultchar.Width;
                }
            }
        }

        public double FontWidthFixed { get; private set; }

        public int LastRenderedColumn { get; private set; } = 0;
        public int LastRenderedRow { get; private set; } = 0;
        public bool RenderBreakOnLineHeight { get; private set; }

        public void CalculateCaretPosition(Point AbsoluteMousePos)
        {
            double absolutY = Config.TextOffset.Y;
            double absolutHeight = Config.TextOffset.Y;
            int Row = 0;
            for (int actRow = FirstRenderedRow; actRow < _Document.Count; actRow++)
            {
                absolutHeight += _defaultchar.Height;
                if (AbsoluteMousePos.Y >= absolutY)
                {
                    Row = actRow;
                    if (AbsoluteMousePos.Y <= absolutHeight)
                        break;
                }
                absolutY = absolutHeight;
            }
            double absolutX = Config.TextOffset.X;
            double absolutWidth = Config.TextOffset.X;
            int X = 0;
            for (int actLetter = 0; actLetter < _Document[Row].Count; actLetter++)
            {
                absolutWidth += _defaultchar.Width;
                if (AbsoluteMousePos.X >= absolutX)
                {
                    if (actLetter > _Document[Row].End - 1)
                        X = _Document[Row].End;
                    else if (_Document[Row].End == _Document[Row].Count && AbsoluteMousePos.X > absolutWidth)
                        X = _Document[Row].Count;
                    else
                        X = actLetter;
                    if (AbsoluteMousePos.X <= absolutWidth)
                        break;
                }
                absolutX = absolutWidth;
            }
            caret.Top = Row;
            caret.Left = X;
        }

        public void Render(DrawingContext dc)
        {
            double heightOffs = Config.TextOffset.Y;
            double heights = Config.TextOffset.Y;
            double yOffs = Config.TextOffset.Y;
            StringBuilder sbLineNr = new StringBuilder();
            StringBuilder sbText = new StringBuilder();
            int padding = Math.Max(Convert.ToInt32(Math.Ceiling(Math.Log10(_Document.Count))), 2);
            Config.TextOffset = new Point((padding * FontWidthFixed + 1.5 * FontWidthFixed) + rectToTextMargin, Config.TextOffset.Y);
            dc.DrawRectangle(Config.Configuration.LineNumberColor, new Pen(), new Rect(0, 0, Config.TextOffset.X - rectToTextMargin, ActualHeight));

            FirstRenderedRow = ScrollOffsetY;
            FirstRenderedColumn = ScrollOffsetX;
            LastRenderedRow = 0;
            LastRenderedColumn = 0;
            RenderBreakOnLineHeight = false;
            for (int actRow = FirstRenderedRow; actRow < _Document.Count; actRow++)
            {
                if ((heightOffs + _defaultchar.Height) > ActualHeight)
                {
                    RenderBreakOnLineHeight = true;
                }
                if (heightOffs > ActualHeight)
                {
                    break;
                }
                LastRenderedRow = actRow;
                if (!Input.IsSelecting && caret.Top == actRow)
                {
                    dc.DrawRectangle(Config.Configuration.RowHighlightColor,
                        new Pen(Config.Configuration.RowHighlightOutlineColor, Config.Configuration.RowHighlightOutlineWidth),
                        new Rect(
                            Config.TextOffset.X,
                            heightOffs,
                            ActualWidth - Config.Configuration.RowHighlightOutlineWidth - Config.TextOffset.X,
                            _defaultchar.Height)
                        );
                }

                var rowHeight = _defaultchar.Height;
                heights += rowHeight;

                double letterXOffs = Config.TextOffset.X;
                var selectionStartDrawX = 0.0;
                var selectionEndDrawX = 0.0;
                var cachedRowEnd = _Document[actRow].End;
                bool rowHasSelection = false;
                for (int actLetter = ScrollOffsetX; actLetter <= _Document[actRow].Count; actLetter++)
                {
                    string internalRep = GetInternalStringRepresentation(_Document[actRow][actLetter], actLetter);
                    sbText.Append(internalRep);
                    if (letterXOffs > ActualWidth)
                    {
                        break;
                    }
                    if (_Document[actRow][actLetter] == '\0')
                    {
                        caret.DrawCursor(actLetter, actRow, cachedRowEnd, letterXOffs, heightOffs, 0);
                        break;
                    }
                    if (Input.IsSelecting || Input.HasSelection)
                    {
                        bool selectCurrentLetter = false;
                        if (actRow == Input.SelectionFrom?.Y && actRow == Input.SelectionTo?.Y)
                        {
                            if (actLetter >= Input.SelectionFrom?.X && actLetter <= Input.SelectionTo?.X)
                            {
                                selectCurrentLetter = true;
                            }
                        }
                        else if (actRow == Input.SelectionFrom?.Y && actLetter >= Input.SelectionFrom?.X)
                        {
                            selectCurrentLetter = true;
                        }
                        else if (actRow == Input.SelectionTo?.Y && actLetter <= Input.SelectionTo?.X)
                        {
                            selectCurrentLetter = true;
                        }
                        else if (actRow > Input.SelectionFrom?.Y && actRow < Input.SelectionTo?.Y)
                        {
                            selectCurrentLetter = true;
                        }
                        if (selectCurrentLetter)
                        {
                            rowHasSelection = true;
                            if (selectionStartDrawX == 0.0)
                            {
                                selectionStartDrawX = letterXOffs;
                            }
                            selectionEndDrawX += _defaultchar.Width * internalRep.Length;
                        }
                    }
                    if (LastRenderedColumn < actLetter)
                    {
                        LastRenderedColumn = actLetter;
                    }
                    if (_lexerErrorAsPositionPoint is Point lexError)
                    {
                        if (actRow - FirstRenderedRow == lexError.Y && actLetter - FirstRenderedColumn == lexError.X)
                        {
                            dc.DrawRectangle(Brushes.Red, new Pen(), new Rect(letterXOffs, heightOffs, FontWidthFixed, FontHeightFixed));
                            _lexerErrorAsPositionPointRendered = _lexerErrorAsPositionPoint;
                        }
                    }
                    caret.DrawCursor(actLetter, actRow, cachedRowEnd, letterXOffs, heightOffs, _defaultchar.Width);
                    letterXOffs += _defaultchar.Width * internalRep.Length;
                }
                if (rowHasSelection)
                {
                    dc.DrawRectangle(Config.Configuration.RowHighlightColor,
                                           new Pen(Config.Configuration.SelectionOutlineColor, Config.Configuration.SelectionOutlineWidth),
                                           new Rect(
                                               selectionStartDrawX,
                                               yOffs,
                                               selectionEndDrawX,
                                               _defaultchar.Height)
                                           );
                }

                sbText.AppendLine();

                sbLineNr.AppendLine((actRow + 1).ToString().PadLeft(padding));

                yOffs += rowHeight;
                heightOffs += rowHeight;
            }
            FormattedText linenumber = Utils.FormatText(sbLineNr.ToString(), FontSize, Config.Configuration.LineNumberTextColor, dpi: _dpi);
            dc.DrawText(linenumber, new Point(FontWidthFixed / 2, Config.TextOffset.Y));
            FormattedText displayedText = Utils.FormatText(sbText.ToString(), FontSize, Config.Configuration.TextColor, dpi: _dpi);

            //highlight
            try
            {
                foreach (var c in Lexer.Tokenize(displayedText.Text.Replace("\0", " "), false, IsDebugging))
                {
                    if (HightlightMap.TryGetValue(c.TokenType, out var x))
                    {
                        var brush = x.Invoke(c.TokenText);
                        displayedText.SetForegroundBrush(brush, c.Position.Index, c.Position.Length);
                    }
                }
                _lexerErrorAsPositionPoint = null;
            }
            catch (UnrecognizedTokenException te)
            {
                _lexerErrorAsPositionPoint = new Point(te.Position.Column, te.Position.Line);
                if (!_lexerErrorAsPositionPoint.Equals(_lexerErrorAsPositionPointRendered))
                {
                    InvalidateVisual();
                }
            }
            catch (Exception)
            {
                //TODO logging
            }

            dc.DrawText(displayedText, new Point(Config.TextOffset.X, Config.TextOffset.Y));
        }

        private string GetInternalStringRepresentation(char value, int charpos, bool showChar = false)
        {
            string retVal;
            switch (value)
            {
                case ' ':
                    retVal = (showChar) ? "\u2022" : " ";
                    break;

                case '\t':
                    retVal = "".PadLeft(4 - (charpos % 4));
                    break;

                case '\r':
                    retVal = (showChar) ? "\u00B6" : " ";
                    break;

                case '\n':
                    retVal = (showChar) ? "\u00AC" : " ";
                    break;

                case '\v':
                    retVal = (showChar) ? " VT" : " ";
                    break;

                case '\f':
                    //Seitenvorschub!
                    retVal = (showChar) ? " FF" : " ";
                    break;

                case '\u0085':
                case '\u0015':
                    retVal = (showChar) ? " NL" : " ";
                    break;

                case '\u2028':
                    retVal = (showChar) ? " LS" : " ";
                    break;

                case '\u2029':
                    retVal = (showChar) ? " PS" : " ";
                    break;

                default:
                    retVal = value.ToString();
                    break;
            }

            return retVal;
        }
    }
}