using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Terminal.Gui;

namespace TMPFT.Display.Advanced.Views
{
    [ScenarioMetadata(Name: "Performance Metrics", Description: "Performance Data and Statistics")]
    [ScenarioCategory("Statistics")]
    public class MetricDataView : Scenarios
    {
        private FrameView View { get; set; } = new FrameView("Metrics/Statistics")
        {
            X = 0,
            Y = 0,
            Width = Dim.Percent(100),
            Height = Dim.Fill(),
        };
        private TableView tableView { get; set; } = new TableView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        public override void Setup()
        {
			var dt = new DataTable();

			dt.Columns.Add(new DataColumn("Connection (Cl/Pb/Pr)", typeof(string)));
			dt.Columns.Add(new DataColumn("Live Bid/Ask (AVG)", typeof(string)));
			dt.Columns.Add(new DataColumn("$Live Profit", typeof(string)));
			dt.Columns.Add(new DataColumn("$Balance ($/%)", typeof(string)));
			dt.Columns.Add(new DataColumn("$Change ($/%)", typeof(double)));


			tableView.Table = dt;

			Win.Add(tableView);

			SetupScrollBar();
        }
		private void SetupScrollBar()
		{
			var _scrollBar = new ScrollBarView(tableView, true);

			_scrollBar.ChangedPosition += () => {
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

			tableView.DrawContent += (e) => {
				_scrollBar.Size = tableView.Table?.Rows?.Count ?? 0;
				_scrollBar.Position = tableView.RowOffset;
				//	_scrollBar.OtherScrollBarView.Size = _listView.Maxlength - 1;
				//	_scrollBar.OtherScrollBarView.Position = _listView.LeftItem;
				_scrollBar.Refresh();
			};

		}

        private partial class Table
        {

            public static void BuildDefaultTable(TableView tableView)
            {
                var Table = new DataTable();

                Table.Columns.Add(new DataColumn("Connection (Cl/Pb/Pr)", typeof(string)));
                Table.Columns.Add(new DataColumn("Live Bid/Ask (AVG)", typeof(string)));
                Table.Columns.Add(new DataColumn("$Live Profit", typeof(string)));
                Table.Columns.Add(new DataColumn("$Balance ($/%)", typeof(string)));
                Table.Columns.Add(new DataColumn("$Change ($/%)", typeof(string)));
                Table.Columns.Add(new DataColumn("Active (B/S/$)", typeof(string)));
                Table.Columns.Add(new DataColumn("Filled (B/S/$)", typeof(string)));
                Table.Columns.Add(new DataColumn("Pred. NN (Y/X)", typeof(string)));
                Table.Columns.Add(new DataColumn("Pred. ML (LB/UP)", typeof(string)));

                List<object> rowOne = new List<object>(){
                    "1/1/1",
                    "9999999",
                    "45",
                    "4/23%", /*add some negatives to demo styles*/
                    "4/32%",
                    "8/5/92342",
                    "8/3/42342",
                    "45235/353",
                    "23253/92342",
                   };

                List<object> rowTwo = new List<object>(){
                    "Crypt",
                    "LW:{}/UP:{}",
                    "45",
                    "4/23%", /*add some negatives to demo styles*/
                    "4/32%",
                    "8/5/92342",
                    "8/3/42342",
                    "45235/353",
                    "23253/92342",
                   };


                /*                
                 for (int j = 0; j < cols - explicitCols; j++)
                {
                    row.Add("SomeValue" + r.Next(100));
                }
                */

                Table.Rows.Add(rowOne.ToArray());

                tableView.Table = Table;

                Table.Rows[0]["Connection (Cl/Pb/Pr)"] = "cde";
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
