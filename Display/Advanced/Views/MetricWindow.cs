using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Terminal.Gui;

namespace TMPFT.Display.Advanced.Views
{
    [ScenarioMetadata(Name: "Metrics", Description: "Performance Data and Statistics")]
    [ScenarioCategory("Statistics")]
    public class MetricWindow : Scenarios
    {
        private FrameView frameView { get; set; } = new FrameView("Metrics/Statistics")
        {
            X = 0,
            Y = 0,
            Width = Dim.Percent(100),
            Height = Dim.Fill(),
        };
        private TableView tableView { get; set; } 
        private TableView tableView2 { get; set; } 
        public override void Setup()
        {
            tableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Percent(50)
            };

            var y = Pos.Bottom(tableView) + 1;

            tableView2 = new TableView()
            {
                X = 0,
                Y = y,
                Width = Dim.Fill(),
                Height = Dim.Percent(50),
            };


            //tableView.Style.ShowHorizontalHeaderUnderline = false;
            //tableView.Update();

            DataTable Table1 = new DataTable();

            Table1.Columns.Add(new DataColumn("Conn (Cl/Pb/Pr)", typeof(string)));
            Table1.Columns.Add(new DataColumn("Live Bid/Ask (AVG)", typeof(string)));
            Table1.Columns.Add(new DataColumn("$Live Profit", typeof(string)));
            Table1.Columns.Add(new DataColumn("$Balance ($/%)", typeof(string)));
            Table1.Columns.Add(new DataColumn("$Change ($/%)", typeof(string)));

            tableView.Table = Table1;

            addData();


            DataTable Table2 = new DataTable();

            Table2.Columns.Add(new DataColumn("Conn (Cl/Pb/Pr)", typeof(string)));
            Table2.Columns.Add(new DataColumn("Live Bid/Ask (AVG)", typeof(string)));
            Table2.Columns.Add(new DataColumn("$Live Profit", typeof(string)));
            Table2.Columns.Add(new DataColumn("$Balance ($/%)", typeof(string)));
            Table2.Columns.Add(new DataColumn("$Change ($/%)", typeof(string)));

            tableView2.Table = Table2;

            //Win.Add(frameView);

            Win.Add(tableView);
            Win.Add(tableView2);

            SetupScrollBar();
        }

        private void addData()
        {

            List<object> rowOne = new List<object>(){
                    "1/1/1",
                    "9999999.00",
                    "45$/-5%",
                    "44$/-23%", /*add some negatives to demo styles*/
                    "2.2/32%"
                   };

            tableView.Table.Rows.Add(rowOne.ToArray());
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
			};
			*/

            tableView.DrawContent += (e) =>
            {
                _scrollBar.Size = tableView.Table?.Rows?.Count ?? 0;
                _scrollBar.Position = tableView.RowOffset;
                //	_scrollBar.OtherScrollBarView.Size = _listView.Maxlength - 1;
                //	_scrollBar.OtherScrollBarView.Position = _listView.LeftItem;
                _scrollBar.Refresh();
            };

        }

        private partial class Table
        {
            public static DataTable UpdateDataTable(DataTable Table)
            {
                Table.Rows[0]["Connection (Cl/Pb/Pr)"] = "cde";

                return Table;

            }
            private static void SetTableViewStyle(TableView tableView)
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

                tableView.Style.ColumnStyles.Add(tableView.Table.Columns["AVG"], dateFormatStyle);
                tableView.Style.ColumnStyles.Add(tableView.Table.Columns["CC"], negativeRight);
                tableView.Style.ColumnStyles.Add(tableView.Table.Columns["Pred. UP"], alignMid);
                tableView.Style.ColumnStyles.Add(tableView.Table.Columns["Pred. LB"], alignRight);

                tableView.Update();
            }
        }
    }
}
