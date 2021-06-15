using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using TMPFT.Screen;

namespace TMPFT.Screen
{
	[ScenarioMetadata(Name: "Graph View x", Description: "Demos GraphView control")]
	[ScenarioCategory("Controls")]
	class GraphViewChart : Scenarios
    {

		public GraphView graphView;
		private TextView about;

		int currentGraph = 0;
		Action[] graphs;

		public override void Setup()
		{
			Win.Title = this.GetName();
			Win.Y = 1; // menu
			Win.Height = Dim.Fill(1); // status bar
			Top.LayoutSubviews();

			graphs = new Action[] {
				 ()=>SetupPeriodicTableScatterPlot(),    //0
				 ()=>SetupLineGraph(),                   //4
				 ()=>MultiBarGraph()                     //7
			};


			var menu = new MenuBar(new MenuBarItem[] {
				new MenuBarItem ("_File", new MenuItem [] {
					new MenuItem ("Scatter _Plot", "",()=>graphs[currentGraph = 0]()),
					new MenuItem ("_Line Graph","",()=>graphs[currentGraph = 1]()),
					new MenuItem ("_Multi Bar Graph","",()=>graphs[currentGraph = 2]()),
					new MenuItem ("_Quit", "", () => Quit()),
				}),
				new MenuBarItem ("_View", new MenuItem [] {
					new MenuItem ("Zoom _In", "", () => Zoom(0.5f)),
					 new MenuItem ("Zoom _Out", "", () =>  Zoom(2f)),
				}),

				});
			Top.Add(menu);

			graphView = new GraphView()
			{
				X = 1,
				Y = 1,
				Width = Dim.Fill(),
				Height = Dim.Fill(),
			};


			Win.Add(graphView);


			var frameRight = new FrameView("About")
			{
				X = Pos.Right(graphView) + 1,
				Y = 0,
				Width = 25,
				Height = Dim.Fill(5),
			};


			frameRight.Add(about = new TextView()
			{
				Width = Dim.Fill(),
				Height = Dim.Fill()
			});

			Win.Add(frameRight);


			var statusBar = new StatusBar(new StatusItem[] {
				new StatusItem(Key.CtrlMask | Key.Q, "~^Q~ Quit", () => Quit()),
				new StatusItem(Key.CtrlMask | Key.G, "~^G~ Next", ()=>graphs[currentGraph++%graphs.Length]()),
			});

			Top.Add(statusBar);
		}

		private void SetupLineGraph()
		{
			graphView.Reset();

			about.Text = "Price/Position prediction chart";

			var black = Application.Driver.MakeAttribute(graphView.ColorScheme.Normal.Foreground, Color.Black);
			var cyan = Application.Driver.MakeAttribute(Color.BrightCyan, Color.Black);
			var magenta = Application.Driver.MakeAttribute(Color.BrightMagenta, Color.Black);
			var red = Application.Driver.MakeAttribute(Color.BrightRed, Color.Black);

			graphView.GraphColor = black;

			List<PointF> randomPoints = new List<PointF>();

			Random r = new Random();

			for (int i = 0; i < 10; i++)
			{
				randomPoints.Add(new PointF(r.Next(100), r.Next(100)));
			}

			var points = new ScatterSeries()
			{
				Points = randomPoints
			};

			var line = new PathAnnotation()
			{
				LineColor = cyan,
				Points = randomPoints.OrderBy(p => p.X).ToList(),
				BeforeSeries = true,
			};

			graphView.Series.Add(points);
			graphView.Annotations.Add(line);


			randomPoints = new List<PointF>();

			for (int i = 0; i < 10; i++)
			{
				randomPoints.Add(new PointF(r.Next(100), r.Next(100)));
			}


			var points2 = new ScatterSeries()
			{
				Points = randomPoints,
				Fill = new GraphCellToRender('x', red)
			};

			var line2 = new PathAnnotation()
			{
				LineColor = magenta,
				Points = randomPoints.OrderBy(p => p.X).ToList(),
				BeforeSeries = true,
			};

			graphView.Series.Add(points2);
			graphView.Annotations.Add(line2);

			// How much graph space each cell of the console depicts
			graphView.CellSize = new PointF(2, 5);

			// leave space for axis labels
			graphView.MarginBottom = 2;
			graphView.MarginLeft = 3;

			// One axis tick/label per
			graphView.AxisX.Increment = 20;
			graphView.AxisX.ShowLabelsEvery = 1;
			graphView.AxisX.Text = "X →";

			graphView.AxisY.Increment = 20;
			graphView.AxisY.ShowLabelsEvery = 1;
			graphView.AxisY.Text = "↑Y";

			var max = line.Points.Union(line2.Points).OrderByDescending(p => p.Y).First();
			graphView.Annotations.Add(new TextAnnotation() { Text = "(Max)", GraphPosition = new PointF(max.X + (2 * graphView.CellSize.X), max.Y) });

			graphView.SetNeedsDisplay();
		}

		private void MultiBarGraph()
		{
			graphView.Reset();

			about.Text = "Housing Expenditures by income thirds 1996-2003";

			var black = Application.Driver.MakeAttribute(graphView.ColorScheme.Normal.Foreground, Color.Black);
			var cyan = Application.Driver.MakeAttribute(Color.BrightCyan, Color.Black);
			var magenta = Application.Driver.MakeAttribute(Color.BrightMagenta, Color.Black);
			var red = Application.Driver.MakeAttribute(Color.BrightRed, Color.Black);

			graphView.GraphColor = black;

			var series = new MultiBarSeries(3, 1, 0.25f, new[] { magenta, cyan, red });

			var stiple = Application.Driver.Stipple;

			series.AddBars("'96", stiple, 5900, 9000, 14000);
			series.AddBars("'97", stiple, 6100, 9200, 14800);
			series.AddBars("'98", stiple, 6000, 9300, 14600);
			series.AddBars("'99", stiple, 6100, 9400, 14950);
			series.AddBars("'00", stiple, 6200, 9500, 15200);
			series.AddBars("'01", stiple, 6250, 9900, 16000);
			series.AddBars("'02", stiple, 6600, 11000, 16700);
			series.AddBars("'03", stiple, 7000, 12000, 17000);

			graphView.CellSize = new PointF(0.25f, 1000);
			graphView.Series.Add(series);
			graphView.SetNeedsDisplay();

			graphView.MarginLeft = 3;
			graphView.MarginBottom = 1;

			graphView.AxisY.LabelGetter = (v) => '$' + (v.Value / 1000f).ToString("N0") + 'k';

			// Do not show x axis labels (bars draw their own labels)
			graphView.AxisX.Increment = 0;
			graphView.AxisX.ShowLabelsEvery = 0;
			graphView.AxisX.Minimum = 0;


			graphView.AxisY.Minimum = 0;

			var legend = new LegendAnnotation(new Rect(graphView.Bounds.Width - 20, 0, 20, 5));
			legend.AddEntry(new GraphCellToRender(stiple, series.SubSeries.ElementAt(0).OverrideBarColor), "Lower Third");
			legend.AddEntry(new GraphCellToRender(stiple, series.SubSeries.ElementAt(1).OverrideBarColor), "Middle Third");
			legend.AddEntry(new GraphCellToRender(stiple, series.SubSeries.ElementAt(2).OverrideBarColor), "Upper Third");
			graphView.Annotations.Add(legend);
		}

		private void SetupPeriodicTableScatterPlot()
		{
			graphView.Reset();

			about.Text = "This graph shows the atomic weight of each element in the periodic table.\nStarting with Hydrogen (atomic Number 1 with a weight of 1.007)";

			//AtomicNumber and AtomicMass of all elements in the periodic table
			graphView.Series.Add(
				new ScatterSeries()
				{
					Points = new List<PointF>{
						new PointF(1,1.007f),new PointF(2,4.002f),new PointF(3,6.941f),new PointF(4,9.012f),new PointF(5,10.811f),new PointF(6,12.011f),
						new PointF(7,14.007f),new PointF(8,15.999f),new PointF(9,18.998f),new PointF(10,20.18f),new PointF(11,22.99f),new PointF(12,24.305f),
						new PointF(13,26.982f),new PointF(14,28.086f),new PointF(15,30.974f),new PointF(16,32.065f),new PointF(17,35.453f),new PointF(18,39.948f),
						new PointF(19,39.098f),new PointF(20,40.078f),new PointF(21,44.956f),new PointF(22,47.867f),new PointF(23,50.942f),new PointF(24,51.996f),
						new PointF(25,54.938f),new PointF(26,55.845f),new PointF(27,58.933f),new PointF(28,58.693f),new PointF(29,63.546f),new PointF(30,65.38f),
						new PointF(31,69.723f),new PointF(32,72.64f),new PointF(33,74.922f),new PointF(34,78.96f),new PointF(35,79.904f),new PointF(36,83.798f),
						new PointF(37,85.468f),new PointF(38,87.62f),new PointF(39,88.906f),new PointF(40,91.224f),new PointF(41,92.906f),new PointF(42,95.96f),
						new PointF(43,98f),new PointF(44,101.07f),new PointF(45,102.906f),new PointF(46,106.42f),new PointF(47,107.868f),new PointF(48,112.411f),
						new PointF(49,114.818f),new PointF(50,118.71f),new PointF(51,121.76f),new PointF(52,127.6f),new PointF(53,126.904f),new PointF(54,131.293f),
						new PointF(55,132.905f),new PointF(56,137.327f),new PointF(57,138.905f),new PointF(58,140.116f),new PointF(59,140.908f),new PointF(60,144.242f),
						new PointF(61,145),new PointF(62,150.36f),new PointF(63,151.964f),new PointF(64,157.25f),new PointF(65,158.925f),new PointF(66,162.5f),
						new PointF(67,164.93f),new PointF(68,167.259f),new PointF(69,168.934f),new PointF(70,173.054f),new PointF(71,174.967f),new PointF(72,178.49f),
						new PointF(73,180.948f),new PointF(74,183.84f),new PointF(75,186.207f),new PointF(76,190.23f),new PointF(77,192.217f),new PointF(78,195.084f),
						new PointF(79,196.967f),new PointF(80,200.59f),new PointF(81,204.383f),new PointF(82,207.2f),new PointF(83,208.98f),new PointF(84,210),
						new PointF(85,210),new PointF(86,222),new PointF(87,223),new PointF(88,226),new PointF(89,227),new PointF(90,232.038f),new PointF(91,231.036f),
						new PointF(92,238.029f),new PointF(93,237),new PointF(94,244),new PointF(95,243),new PointF(96,247),new PointF(97,247),new PointF(98,251),
						new PointF(99,252),new PointF(100,257),new PointF(101,258),new PointF(102,259),new PointF(103,262),new PointF(104,261),new PointF(105,262),
						new PointF(106,266),new PointF(107,264),new PointF(108,267),new PointF(109,268),new PointF(113,284),new PointF(114,289),new PointF(115,288),
						new PointF(116,292),new PointF(117,295),new PointF(118,294)
			}
				});

			// How much graph space each cell of the console depicts
			graphView.CellSize = new PointF(1, 5);

			// leave space for axis labels
			graphView.MarginBottom = 2;
			graphView.MarginLeft = 3;

			// One axis tick/label per 5 atomic numbers
			graphView.AxisX.Increment = 5;
			graphView.AxisX.ShowLabelsEvery = 1;
			graphView.AxisX.Text = "Atomic Number";
			graphView.AxisX.Minimum = 0;

			// One label every 5 atomic weight
			graphView.AxisY.Increment = 5;
			graphView.AxisY.ShowLabelsEvery = 1;
			graphView.AxisY.Minimum = 0;

			graphView.SetNeedsDisplay();
		}

		private void Zoom(float factor)
		{
			graphView.CellSize = new PointF(
				graphView.CellSize.X * factor,
				graphView.CellSize.Y * factor
			);

			graphView.AxisX.Increment *= factor;
			graphView.AxisY.Increment *= factor;

			graphView.SetNeedsDisplay();
		}

		private void Quit()
		{
			Application.RequestStop();
		}
	}
}
