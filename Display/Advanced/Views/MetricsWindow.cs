using System;
using System.Collections.Generic;
using System.Data;
using Terminal.Gui;

namespace TMAPT.Display.Advanced.Views
{
    [ScenarioMetadata(Name: "Metrics", Description: "Performance Data and Statistics")]
    [ScenarioCategory("Statistics")]
    public class MetricsWindow : Scenario
    {
        private FrameView frameView { get; set; } = new FrameView("Metrics/Statistics")
        {
            X = 0,
            Y = 0,
            Width = Dim.Percent(100),
            Height = Dim.Fill(),
        };
        private TableView Table00 { get; set; }
        private TableView Table01 { get; set; }
        private TableView Table10 { get; set; }
        private TableView Table11 { get; set; }

        public override void Setup()
        {
            Table00 = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Percent(50)
            };

            var x = Pos.Right(Table00) + 1;

            Table01 = new TableView()
            {
                X = x,
                Y = 0,
                Width = Dim.Fill(1),
                Height = Dim.Percent(50),
            };

            var y = Pos.Bottom(Table00) + 1;

            Table10 = new TableView()
            {
                X = 0,
                Y = y,
                Width = Dim.Percent(50),
                Height = Dim.Percent(50),
            };

            Table11 = new TableView()
            {
                X = x,
                Y = y,
                Width = Dim.Fill(1),
                Height = Dim.Percent(50),
            };


            //tableView.Style.ShowHorizontalHeaderUnderline = false;
            //tableView.Update();

            DataTable OrderTable = new DataTable();
            OrderTable.Columns.Add(new DataColumn("Profit/Balance/Orders", typeof(string)));
            Table00.Table = OrderTable;

            DataTable PredictionTable = new DataTable();
            PredictionTable.Columns.Add(new DataColumn("Predictions/Market", typeof(string)));
            Table01.Table = PredictionTable;

            DataTable SystemData = new DataTable();
            SystemData.Columns.Add(new DataColumn("System/Data", typeof(string)));
            Table10.Table = SystemData;

            DataTable API = new DataTable();
            API.Columns.Add(new DataColumn("API/Live", typeof(string)));
            Table11.Table = API;
            //Win.Add(frameView);

            Win.Add(Table00);
            Win.Add(Table01);
            Win.Add(Table10);
            Win.Add(Table11);

            addtestData();

            UpdateBalanceTable();

            SetupScrollBar();
        }
        private void addtestData()
        {
            for (int i = 0; i < 5; i++)
            {
                List<object> row00 = new List<object>(){
                    "0 - Correlation/MH/UP/LB/t-Predicte",
                   };

                Table00.Table.Rows.Add(row00.ToArray());

                List<object> row01 = new List<object>(){
                    "1 - Prediction/Final/Price",
                   };

                Table01.Table.Rows.Add(row01.ToArray());

                List<object> row10 = new List<object>(){
                    "2 - System/Data/Report",
                   };

                Table10.Table.Rows.Add(row10.ToArray());

                List<object> row11 = new List<object>(){
                    "3 - API/Sync/Socket",
                   };

                Table11.Table.Rows.Add(row11.ToArray());
            }
        }
        private void UpdateBalanceTable(string Value = "Order Table Default", int rowIndex = 0)
        {
            // Rows, 1,2,3,4,5
            Table00.Table.Rows[rowIndex]["Profit/Balance/Orders"] = Value;
        }
        private void UpdatePredTable(string Value = "Prediction Table Default", int rowIndex = 0)
        {
            // Rows, 1,2,3,4,5
            Table01.Table.Rows[rowIndex]["Predictions/Market"] = Value;
        }
        private void UpdateSystemTable(string Value = "System Table Default", int rowIndex = 0)
        {
            // Rows, 1,2,3,4,5
            Table00.Table.Rows[rowIndex]["System/Data"] = Value;
        }
        private void UpdateApiLiveTable(string Value = "ApiLive Table Default", int rowIndex = 0)
        {
            // Rows, 1,2,3,4,5
            Table11.Table.Rows[rowIndex]["API/Live"] = Value;
        }

        private void SetupScrollBar()
        {
            var _scrollBar = new ScrollBarView(Table00, true);

            _scrollBar.ChangedPosition += () =>
            {
                Table00.RowOffset = _scrollBar.Position;
                if (Table00.RowOffset != _scrollBar.Position)
                {
                    _scrollBar.Position = Table00.RowOffset;
                }
                Table00.SetNeedsDisplay();
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

            Table00.DrawContent += (e) =>
            {
                _scrollBar.Size = Table00.Table?.Rows?.Count ?? 0;
                _scrollBar.Position = Table00.RowOffset;
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
