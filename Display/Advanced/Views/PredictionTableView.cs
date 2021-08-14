using System;
using System.Collections.Generic;
using System.Data;
using Terminal.Gui;
using System.Linq;
using System.Globalization;
using TMAPT.Core.Intell;
using TMAPT.Core;

namespace TMAPT.Display.Advanced.Views
{
    [ScenarioMetadata(Name: "Prediction DataView (NN/ML)", Description: "View Prediction Data and Statistics")]
    [ScenarioCategory("Statistics")]
    public class PredictionTableView : Scenario
    {
        public PredictionTableView(CoreLib Core) : base(Core) { }

        TableView tableView;
        private MenuItem miAlwaysShowHeaders;
        private MenuItem miHeaderOverline;
        private MenuItem miHeaderMidline;
        private MenuItem miHeaderUnderline;
        private MenuItem miCellLines;
        private MenuItem miFullRowSelect;
        private MenuItem miExpandLastColumn;

        public override void SetupWindow()
        {
            Win.Title = this.GetName();
            Win.Y = 1; // menu
            Win.Height = Dim.Fill(1); // status bar
            Top.LayoutSubviews();

            this.tableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
            };

            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("Train predict", "", () => TrainPredict()),
                    new MenuItem ("_Quit", "", () => Quit()),
                }),
                new MenuBarItem ("_View", new MenuItem [] {
                    miAlwaysShowHeaders = new MenuItem ("_AlwaysShowHeaders", "", () => ToggleAlwaysShowHeader()){Checked = tableView.Style.AlwaysShowHeaders, CheckType = MenuItemCheckStyle.Checked },
                    miHeaderOverline = new MenuItem ("_HeaderOverLine", "", () => ToggleOverline()){Checked = tableView.Style.ShowHorizontalHeaderOverline, CheckType = MenuItemCheckStyle.Checked },
                    miHeaderMidline = new MenuItem ("_HeaderMidLine", "", () => ToggleHeaderMidline()){Checked = tableView.Style.ShowVerticalHeaderLines, CheckType = MenuItemCheckStyle.Checked },
                    miHeaderUnderline =new MenuItem ("_HeaderUnderLine", "", () => ToggleUnderline()){Checked = tableView.Style.ShowHorizontalHeaderUnderline, CheckType = MenuItemCheckStyle.Checked },
                    miFullRowSelect =new MenuItem ("_FullRowSelect", "", () => ToggleFullRowSelect()){Checked = tableView.FullRowSelect, CheckType = MenuItemCheckStyle.Checked },
                    miCellLines =new MenuItem ("_CellLines", "", () => ToggleCellLines()){Checked = tableView.Style.ShowVerticalCellLines, CheckType = MenuItemCheckStyle.Checked },
                    miExpandLastColumn = new MenuItem ("_ExpandLastColumn", "", () => ToggleExpandLastColumn()){Checked = tableView.Style.ExpandLastColumn, CheckType = MenuItemCheckStyle.Checked },
                    new MenuItem ("_AllLines", "", () => ToggleAllCellLines()),
                    new MenuItem ("_NoLines", "", () => ToggleNoCellLines()),
                    new MenuItem ("_ClearColumnStyles", "", () => ClearColumnStyles()),
                }),
            });
            Top.Add(menu);

            Win.Add(tableView);

            var selectedCellLabel = new Label()
            {
                X = 0,
                Y = Pos.Bottom(tableView),
                Text = "0,0",
                Width = Dim.Fill(),
                TextAlignment = TextAlignment.Right

            };

            Win.Add(selectedCellLabel);

            tableView.SelectedCellChanged += (e) => { selectedCellLabel.Text = $"{tableView.SelectedRow},{tableView.SelectedColumn}"; };
            tableView.CellActivated += EditCurrentCell;
            tableView.KeyPress += TableViewKeyPress;

            SetupScrollBar();

            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.CtrlMask | Key.R, "~^R~ Train", null),
                new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", null),
            });

            Top.Add(statusBar);

            tableView.Table = BuildTable(2, 10000);
        }

        /// <summary>
        /// Generates a new demo <see cref="DataTable"/> with the given number of <paramref name="cols"/> (min 5) and <paramref name="rows"/>
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public DataTable BuildTable(int cols, int rows)
        {
            var dt = new DataTable();

            dt.Columns.Add(new DataColumn("Index", typeof(int)));
            dt.Columns.Add(new DataColumn("Price (NN)", typeof(double)));
            dt.Columns.Add(new DataColumn("Weight (NN)", typeof(double)));
            dt.Columns.Add(new DataColumn("Linear Price", typeof(double)));
            dt.Columns.Add(new DataColumn("Linear Ratio", typeof(double)));
            dt.Columns.Add(new DataColumn("Final nnX", typeof(double)));
            dt.Columns.Add(new DataColumn("Final nnY", typeof(double)));
            dt.Columns.Add(new DataColumn("Final lY", typeof(double)));
            return dt;
        }

        /// <summary>
        /// Builds a simple table in which cell values contents are the index of the cell.  This helps testing that scrolling etc is working correctly and not skipping out any rows/columns when paging
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        private void TrainPredict()
        {
            IntellUI.PredictByNeuralDynamic();

            var rPrices = IntellUI.Prediction.realPrices;
            var nWeights = IntellUI.Prediction.Normalize.nWeights;
            var lPrices = IntellUI.Prediction.linearPrices;
            var qRes = IntellUI.Prediction.queryResponses;

            for (int i = 0; i < lPrices.Length; i++)
            {
                List<object> rowOne = new List<object>()
                {
                        i,
                        rPrices[i],
                        nWeights[i],
                        lPrices[i],
                        qRes[i],
                        IntellUI.Prediction.FinalnnPredictedX,
                        IntellUI.Prediction.FinalnnPredictedY,
                        IntellUI.Prediction.FinalLinearPredictedY
                };

                tableView.Table.Rows.Add(rowOne.ToArray());
            }

           // tableView.Table.Columns["CLinear Ratio)"].ColumnName = Value;

            tableView.Table.AcceptChanges();
        }
        private void SetupScrollBar()
        {
            var _scrollBar = new ScrollBarView(tableView, true);

            _scrollBar.ChangedPosition += () =>
            {
                tableView.RowOffset = _scrollBar.Position;
                if (tableView.RowOffset != _scrollBar.Position)
                {
                    _scrollBar.Position = tableView.RowOffset;
                }
                tableView.SetNeedsDisplay();
            };
            /*
			_scrollBar.OtherScrollBarView.ChangedPosition += () => {
				_listView.LeftItem = _scrollBar.OtherScrollBarView.Position;
				if (_listView.LeftItem != _scrollBar.OtherScrollBarView.Position) {
					_scrollBar.OtherScrollBarView.Position = _listView.LeftItem;
				}
				_listView.SetNeedsDisplay ();
			};*/

            tableView.DrawContent += (e) =>
            {
                _scrollBar.Size = tableView.Table?.Rows?.Count ?? 0;
                _scrollBar.Position = tableView.RowOffset;
                //	_scrollBar.OtherScrollBarView.Size = _listView.Maxlength - 1;
                //	_scrollBar.OtherScrollBarView.Position = _listView.LeftItem;
                _scrollBar.Refresh();
            };

        }
        private void TableViewKeyPress(View.KeyEventEventArgs e)
        {
            if (e.KeyEvent.Key == Key.DeleteChar)
            {

                if (tableView.FullRowSelect)
                {
                    // Delete button deletes all rows when in full row mode
                    foreach (int toRemove in tableView.GetAllSelectedCells().Select(p => p.Y).Distinct().OrderByDescending(i => i))
                        tableView.Table.Rows.RemoveAt(toRemove);
                }
                else
                {

                    // otherwise set all selected cells to null
                    foreach (var pt in tableView.GetAllSelectedCells())
                    {
                        tableView.Table.Rows[pt.Y][pt.X] = DBNull.Value;
                    }
                }

                tableView.Update();
                e.Handled = true;
            }


        }
        private void ClearColumnStyles()
        {
            tableView.Style.ColumnStyles.Clear();
            tableView.Update();
        }
        private void ToggleAlwaysShowHeader()
        {
            miAlwaysShowHeaders.Checked = !miAlwaysShowHeaders.Checked;
            tableView.Style.AlwaysShowHeaders = miAlwaysShowHeaders.Checked;
            tableView.Update();
        }
        private void ToggleOverline()
        {
            miHeaderOverline.Checked = !miHeaderOverline.Checked;
            tableView.Style.ShowHorizontalHeaderOverline = miHeaderOverline.Checked;
            tableView.Update();
        }
        private void ToggleHeaderMidline()
        {
            miHeaderMidline.Checked = !miHeaderMidline.Checked;
            tableView.Style.ShowVerticalHeaderLines = miHeaderMidline.Checked;
            tableView.Update();
        }
        private void ToggleUnderline()
        {
            miHeaderUnderline.Checked = !miHeaderUnderline.Checked;
            tableView.Style.ShowHorizontalHeaderUnderline = miHeaderUnderline.Checked;
            tableView.Update();
        }
        private void ToggleFullRowSelect()
        {
            miFullRowSelect.Checked = !miFullRowSelect.Checked;
            tableView.FullRowSelect = miFullRowSelect.Checked;
            tableView.Update();
        }
        private void ToggleExpandLastColumn()
        {
            miExpandLastColumn.Checked = !miExpandLastColumn.Checked;
            tableView.Style.ExpandLastColumn = miExpandLastColumn.Checked;

            tableView.Update();

        }
        private void ToggleCellLines()
        {
            miCellLines.Checked = !miCellLines.Checked;
            tableView.Style.ShowVerticalCellLines = miCellLines.Checked;
            tableView.Update();
        }
        private void ToggleAllCellLines()
        {
            tableView.Style.ShowHorizontalHeaderOverline = true;
            tableView.Style.ShowVerticalHeaderLines = true;
            tableView.Style.ShowHorizontalHeaderUnderline = true;
            tableView.Style.ShowVerticalCellLines = true;

            miHeaderOverline.Checked = true;
            miHeaderMidline.Checked = true;
            miHeaderUnderline.Checked = true;
            miCellLines.Checked = true;

            tableView.Update();
        }
        private void ToggleNoCellLines()
        {
            tableView.Style.ShowHorizontalHeaderOverline = false;
            tableView.Style.ShowVerticalHeaderLines = false;
            tableView.Style.ShowHorizontalHeaderUnderline = false;
            tableView.Style.ShowVerticalCellLines = false;

            miHeaderOverline.Checked = false;
            miHeaderMidline.Checked = false;
            miHeaderUnderline.Checked = false;
            miCellLines.Checked = false;

            tableView.Update();
        }
        private void CloseExample()
        {
            tableView.Table = null;
        }
        private void Quit()
        {
            Application.RequestStop();
        }
        private void SetDemoTableStyles()
        {
            var alignMid = new TableView.ColumnStyle()
            {
                Alignment = TextAlignment.Centered
            };
            var alignRight = new TableView.ColumnStyle()
            {
                Alignment = TextAlignment.Right
            };

            var dateFormatStyle = new TableView.ColumnStyle()
            {
                Alignment = TextAlignment.Right,
                RepresentationGetter = (v) => v is DateTime d ? d.ToString("yyyy-MM-dd") : v.ToString()
            };

            var negativeRight = new TableView.ColumnStyle()
            {

                Format = "0.##",
                MinWidth = 10,
                AlignmentGetter = (v) => v is double d ?
                                // align negative values right
                                d < 0 ? TextAlignment.Right :
                                // align positive values left
                                TextAlignment.Left :
                                // not a double
                                TextAlignment.Left
            };

            tableView.Style.ColumnStyles.Add(tableView.Table.Columns["DateCol"], dateFormatStyle);
            tableView.Style.ColumnStyles.Add(tableView.Table.Columns["DoubleCol"], negativeRight);
            tableView.Style.ColumnStyles.Add(tableView.Table.Columns["NullsCol"], alignMid);
            tableView.Style.ColumnStyles.Add(tableView.Table.Columns["IntCol"], alignRight);

            tableView.Update();
        }
        private void EditCurrentCell(TableView.CellActivatedEventArgs e)
        {
            if (e.Table == null)
                return;

            var oldValue = e.Table.Rows[e.Row][e.Col].ToString();
            bool okPressed = false;

            var ok = new Button("Ok", is_default: true);
            ok.Clicked += () => { okPressed = true; Application.RequestStop(); };
            var cancel = new Button("Cancel");
            cancel.Clicked += () => { Application.RequestStop(); };
            var d = new Dialog("Enter new value", 60, 20, ok, cancel);

            var lbl = new Label()
            {
                X = 0,
                Y = 1,
                Text = e.Table.Columns[e.Col].ColumnName
            };

            var tf = new TextField()
            {
                Text = oldValue,
                X = 0,
                Y = 2,
                Width = Dim.Fill()
            };

            d.Add(lbl, tf);
            tf.SetFocus();

            Application.Run(d);

            if (okPressed)
            {

                try
                {
                    e.Table.Rows[e.Row][e.Col] = string.IsNullOrWhiteSpace(tf.Text.ToString()) ? DBNull.Value : (object)tf.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery(60, 20, "Failed to set text", ex.Message, "Ok");
                }

                tableView.Update();
            }
        }
        public partial class Table
        { 
            TableView tableView;
        }
    }
}
