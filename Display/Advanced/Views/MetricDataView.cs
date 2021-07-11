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

			int explicitCols = 6;
			dt.Columns.Add(new DataColumn("Connection (Cl/Pb/Pr)", typeof(string)));
			dt.Columns.Add(new DataColumn("Live Bid/Ask (AVG)", typeof(string)));
			dt.Columns.Add(new DataColumn("$Live Profit", typeof(string)));
			dt.Columns.Add(new DataColumn("$Balance ($/%)", typeof(string)));
			dt.Columns.Add(new DataColumn("$Change ($/%)", typeof(double)));
			dt.Columns.Add(new DataColumn("Active (B/S/$)", typeof(string)));
			dt.Columns.Add(new DataColumn("Filled (B/S/$)", typeof(string)));
			dt.Columns.Add(new DataColumn("Orders (A/F/$)", typeof(string)));


			for (int i = 0; i < 5 - explicitCols; i++)
			{
				dt.Columns.Add("Column" + (i + explicitCols));
			}

			var r = new Random(100);

			for (int i = 0; i < 99; i++)
			{

				List<object> row = new List<object>(){
					"Some long t",
					new DateTime(2000+i,12,25),
					r.Next(i),
					(r.NextDouble()*i)-0.5 /*add some negatives to demo styles*/,
					DBNull.Value,
					"Les Mise rables"
				};

				for (int j = 0; j < 5 - explicitCols; j++)
				{
					row.Add("SomeValue" + r.Next(100));
				}

				dt.Rows.Add(row.ToArray());
			}

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
			};*/

			tableView.DrawContent += (e) => {
				_scrollBar.Size = tableView.Table?.Rows?.Count ?? 0;
				_scrollBar.Position = tableView.RowOffset;
				//	_scrollBar.OtherScrollBarView.Size = _listView.Maxlength - 1;
				//	_scrollBar.OtherScrollBarView.Position = _listView.LeftItem;
				_scrollBar.Refresh();
			};

		}

	}
}
