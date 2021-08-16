using System;
using System.Collections.Generic;
using System.Data;
using Terminal.Gui;
using TMAPT.Core;
using TMAPT.Core.Simulation;

namespace TMAPT.Display.Advanced.Views
{
    [ScenarioMetadata(Name: "Metrics", Description: "Performance Data and Statistics")]
    [ScenarioCategory("Statistics")]
    public class FullMetricWindow : Scenario
    {
        public FullMetricWindow() { }
        public FullMetricWindow(CoreLib Core) : base(Core) { }
        private FrameView frameView { get; set; } = new FrameView("Metrics/Statistics")
        {
            X = 0,
            Y = 0,
            Width = Dim.Percent(100),
            Height = Dim.Fill(),
        };
        private TableView Table01 { get; set; }
        private TableView Table00 { get; set; }
        private TableView Table10 { get; set; }
        private TableView Table11 { get; set; }

        public override void Setup()
        {

            Table00 = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(75),
                Height = Dim.Percent(60),
            };
            var x = Pos.Right(Table00) + 1;

            Table01 = new TableView()
            {
                X = x,
                Y = 0,
                Width = Dim.Percent(75),
                Height = Dim.Fill(1)
            };

            var y = Pos.Bottom(Table00) + 1;

            Table10 = new TableView()
            {
                X = 0,
                Y = y,
                Width = Dim.Percent(50),
                Height = Dim.Fill(1),
            };

            Table11 = new TableView()
            {
                X = Pos.Right(Table10),
                Y = y,
                Width = Dim.Fill(1),
                Height = Dim.Fill(1),
            };


            //tableView.Style.ShowHorizontalHeaderUnderline = false;
            //tableView.Update();

            DataTable MarketPredictionTable = new DataTable();
            MarketPredictionTable.Columns.Add(new DataColumn("Market/Prediction", typeof(string)));
            Table00.Table = MarketPredictionTable;

            DataTable OrderTable = new DataTable();
            OrderTable.Columns.Add(new DataColumn("Balance/Orders", typeof(string)));
            Table01.Table = OrderTable;

            DataTable SystemData = new DataTable();
            SystemData.Columns.Add(new DataColumn("System/Data", typeof(string)));
            Table10.Table = SystemData;

            DataTable API = new DataTable();
            API.Columns.Add(new DataColumn("COM/Live", typeof(string)));
            Table11.Table = API;
            //Win.Add(frameView);

            Win.Add(Table00);
            Win.Add(Table01);
            Win.Add(Table10);
            Win.Add(Table11);

            addtestData();

            updateBalance();
            updateMarketPredictionT();
            updateSystemTable();
            updateApiLiveTable();

            SetupScrollBar();
        }
        private int rows { get; set; } = 10;
        private void addtestData()
        {
            for (int i = 0; i < rows; i++)
            {
                // Market/Prediction - Lastprice/Slope/StdY/Correlation/MaxMinJumpRate/MaxMinValue/BearBull/%MarketRates"
                List<object> row00 = new List<object>(){
                    $"",
                   };

                Table00.Table.Rows.Add(row00.ToArray());

                // Balance/orders - PriceDev $Performance/$VAL/$CH - $Invest/Active(B/S)/Filled(B/S)
                List<object> row01 = new List<object>(){
                    $"",
                   };

                Table01.Table.Rows.Add(row01.ToArray());

                List<object> row10 = new List<object>(){
                    $"",
                   };

                Table10.Table.Rows.Add(row10.ToArray());

                List<object> row11 = new List<object>(){
                    $"",
                   };

                Table11.Table.Rows.Add(row11.ToArray());
            }
        }
        private void updateMarketPredictionT()
        {
            var lastPrice = Core.Simulator.getLastCoin.getBaseValueRounded;
            var maxPrice = Core.Simulator.getLastCoin.getMaxValue;
            var minPrice = Core.Simulator.getLastCoin.getMinValue;
            var upperBound = Core.Simulator.IntellUI.Predictive.upperBound;
            var lowerBound = Core.Simulator.IntellUI.Predictive.lowerBound;
            var jumpRateUp = 0;
            var jumpRateDw = 0;
            var stdY = 0;
            var stdX = 0;
            var slope = 0;
            var corr = 0;

            var m3 = 0;
            var m5 = 0;
            var m10 = 0;
            var m15 = 0;
            var m30 = 0;
            var m60 = 0;
            var m90 = 0;
            var m180 = 0;
            var m360 = 0;

            // Rows, 1,2,3,4,5
            Table00.Table.Rows[0]["Market/Prediction"] = $"Bid/Ask: {lastPrice} | Max/Min: {maxPrice}/{minPrice}";
            Table00.Table.Rows[1]["Market/Prediction"] = $"U/L Pred: {upperBound}/{lowerBound} | U/L Rate: {jumpRateUp}/{jumpRateDw}";
            Table00.Table.Rows[2]["Market/Prediction"] = $"Std Y/X: {stdY}/{stdX} | Slope: {slope} | Corr: {corr}";
            Table00.Table.Rows[3]["Market/Prediction"] = $"Rates: 3: {m3}, 5: {m5}, 10: {m10}, 15: {m15}, 30: {m30}, 60: {m60}, 90: {m90}, 180: {m180}, 360: {m360} ";
        }
        private void updateBalance()
        {
            // Rows, 1,2,3,4,5
            Table01.Table.Rows[0]["Balance/Orders"] = "";

        }
        private void updateSystemTable()
        {
            // Rows, 1,2,3,4,5
            Table10.Table.Rows[0]["System/Data"] = $"Sync Public : --";
            Table10.Table.Rows[1]["System/Data"] = $"Sync Private: --";
        }
        private void updateApiLiveTable()
        {
            // Rows, 1,2,3,4,5
            Table11.Table.Rows[0]["COM/Live"] = $"Public state:  -- | Reconn : -- ";
            Table11.Table.Rows[1]["COM/Live"] = $"Private state: -- | Reconn : -- ";
        }

        private void SetupScrollBar()
        {
            var _scrollBar = new ScrollBarView(Table01, true);

            _scrollBar.ChangedPosition += () =>
            {
                Table01.RowOffset = _scrollBar.Position;
                if (Table01.RowOffset != _scrollBar.Position)
                {
                    _scrollBar.Position = Table01.RowOffset;
                }
                Table01.SetNeedsDisplay();
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

            Table01.DrawContent += (e) =>
            {
                _scrollBar.Size = Table01.Table?.Rows?.Count ?? 0;
                _scrollBar.Position = Table01.RowOffset;
                //	_scrollBar.OtherScrollBarView.Size = _listView.Maxlength - 1;
                //	_scrollBar.OtherScrollBarView.Position = _listView.LeftItem;
                _scrollBar.Refresh();
            };

        }

        private partial class Table
        {
            private void UpdateMarketPredictionTable(int index = 0, string Value = "Prediction Table Default")
            {
                // Rows, 1,2,3,4,5
                //Table00.Table.Rows[index]["Market/Prediction"] = Value;
            }
            private void UpdateBalanceTable(int index = 0, string Value = "Order Table Default")
            {
                // Rows, 1,2,3,4,5
                //Table01.Table.Rows[index]["Balance/Orders"] = Value;
            }
            private void UpdateSystemTable(int index = 0, string Value = "System Table Default")
            {
                // Rows, 1,2,3,4,5
                //Table01.Table.Rows[index]["System/Data"] = Value;
            }
            private void UpdateApiLiveTable(int index = 0, string Value = "ApiLive Table Default")
            {
                // Rows, 1,2,3,4,5
                //Table11.Table.Rows[index]["API/Live"] = Value;
            }
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
