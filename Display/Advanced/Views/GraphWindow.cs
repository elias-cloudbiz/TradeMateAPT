using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using System.Data;
using System.Globalization;
using NStack;
using System.Threading.Tasks;
using Attribute = Terminal.Gui.Attribute;
using TMAPT.Module;
using TMAPT.Core.Models;
using TMAPT.Core.Events;
using TMAPT.Core.Exchanges;
using TMAPT.Core;

namespace TMAPT.Display
{
    [ScenarioMetadata(Name: "Graph Window", Description: "Main Window Live Graph and Orders")]
    [ScenarioCategory("Main")]
    class GraphWindow : Scenario
    {
        public GraphView graphView { get; set; } = new GraphView()
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        public GraphWindow(CoreLib Core) : base(Core) {
            
        }
        private Graph graph = new Graph();
        private static FrameView FrameTop { get; set; } = new FrameView("Data")
        {
                X = 1,
                Y = 2,
                Width = Dim.Fill(1),
                Height = Dim.Sized(4),
        };
        private FrameView frameLeft { get; set; } 
        private FrameView frameRight { get; set; } 
        private int SelectedItemIndex { get; set; }
        private TableView tableView { get; set; } 
        Action[] graphs;
        private void CreateStatusBar()
        {
            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.CtrlMask | Key.R, "~^R~ Test 1", () => updateMainWindow()),
                new StatusItem(Key.CtrlMask | Key.S, "~^S~ Test 2",  () => updateTableCell(0, "! D3 !"))
            });
            statusBar.ColorScheme = Colors.TopLevel;
            Top.Add(statusBar);
        }
        void createMenuBar()
        {
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("Scatter _Plot", "",()=>Graph.SetupPeriodicTableScatterPlot(graphView)),
                    new MenuItem ("_Line Graph","",()=>Graph.setupLiveGraph(graphView)),
                    new MenuItem ("_Multi Bar Graph","",()=>Graph.SetupPeriodicTableScatterPlot(graphView)),
                    new MenuItem ("_Quit", "", () => quitWindow()),
                }),
                new MenuBarItem ("_View", new MenuItem [] {
                    new MenuItem ("Zoom _In", "", () => Misc.Zoom(graphView, 0.5f)),
                     new MenuItem ("Zoom _Out", "", () =>  Misc.Zoom(graphView, 2f)),
                }),

                });
            Top.Add(menu);

        }
        public override void Setup()
        {
            Win.Title = this.GetName();
            Win.Y = 1; // menu
            Win.Height = Dim.Fill(1); // status bar
            Top.LayoutSubviews();

            graphs = new Action[] {
                 () => Graph.SetupPeriodicTableScatterPlot(graphView),    //0
				 () => Graph.setupLiveGraph(graphView),                   //4
				 () => Graph.MultiBarGraph(graphView)                     //7
			};

            createMenuBar();

            frameLeft = new FrameView("Live")
            {
                X = 0,
                Y = 5,
                Width = Dim.Percent(75),
                Height = Dim.Fill(),
            };

            frameRight = new FrameView("Orders")
            {
                X = Pos.Right(frameLeft) + 1,
                Y = 5,
                Width = Dim.Percent(25),
                Height = Dim.Fill(),
            };
            tableView = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Sized(5),
            };
            // Create default table
            Table.BuildDefaultTable(tableView);
            Win.Add(tableView);

            // Create Frame for Graph
            /// Add Graph
            //Win.Add(FrameLeft);
            frameLeft.Add(graphView);

            // Create orders frame
            Win.Add(frameRight);

            var labelHL = new Label($"Syncinc in {" X "} sec.") { X = 0, Y = 0, Width = Dim.Fill(), Height = 1, TextAlignment = TextAlignment.Centered, /**ColorScheme = Colors.ColorSchemes["Base"]**/ };
            frameRight.Add(labelHL);

            // Create ListView For orders
            ListView _listView = new ListView()
            {
                X = 1,
                Y = 2,
                Height = Dim.Fill(),
                Width = Dim.Fill(5),
                //ColorScheme = Colors.TopLevel,
                AllowsMarking = false,
                AllowsMultipleSelection = false
            };
            List<Type> Orders = Scenario.GetDerivedClasses<Scenario>().OrderBy(t => Scenario.ScenarioMetadata.GetName(t)).ToList();

            var createButton = new Button($"Create")
            {
                //X = Pos.Right (prev) + 2,
                X = 0,
                Y = Pos.Y(_listView) + 1, //+Orders.Count,
            };

            createButton.Clicked += () =>
            {
                // Create Clicked
                createOrder();
            };

            var cancelButton = new Button("Cancel all")
            {
                X = Pos.X(createButton) + 10,
                //TODO: Change to use Pos.AnchorEnd()
                Y = Pos.Y(_listView) + 1,// + Orders.Count,
                IsDefault = false,
            };

            createButton.Clicked += () =>
            {
                // Cancel Clicked
            };

            _listView.SetSource(Orders);

            _listView.OpenSelectedItem += (e) =>
            {
                SelectedItemIndex = _listView.SelectedItem;

                var cancel = "Proceed";
                var ignore = "Ignore";

                var q = MessageBox.Query(0, 0, "Cancel order request", $"Are you sure you to cancel order {SelectedItemIndex} ?", cancel, ignore);
            };

            frameRight.Add(_listView);

            frameRight.Add(createButton);
            frameRight.Add(cancelButton);

            CreateStatusBar();

            //Event.Methods.Public.PublicMethodComplete += (sender, e) => UpdateMainWindow();
            //_Graph.SetupGraph();        
        }
        public void updateMainWindow()
        {
            string ConnectionState = $"0/{Parameters.API.PublicConnection}/{Parameters.API.PrivateConnection}";
            string LivePrice = $"{Core.Simulator.getLastCoin.getBaseValueRounded}";
            string Profit = $"{Parameters.Wallet.Profit}/{Parameters.Wallet.BalanceChangeValue}";

            updateTableCell(0, ConnectionState);
            updateTableCell(1, LivePrice);
            updateTableCell(2, Profit);
            updateTableCell(3, graphView.ScreenToGraphSpace(2, 2).Y.ToString());

            Graph.UpdateLiveGraph(graphView);
        }
        private void updateTableCell(int ColNr = 0, string Value = "Default", int RowIndex = 0)
        {
            switch (ColNr)
            {
                case 0:
                    tableView.Table.Rows[RowIndex]["Connection (Cl/Pb/Pr)"] = Value;
                    break;
                case 1:
                    tableView.Table.Rows[RowIndex]["Live Bid/Ask"] = Value;
                    break;
                case 2:
                    tableView.Table.Rows[RowIndex]["Live Profit"] = Value;
                    break;
                case 3:
                    tableView.Table.Rows[RowIndex]["Balance ($/%)"] = Value;
                    break;
                case 4:
                    tableView.Table.Rows[RowIndex]["Change ($/%)"] = Value;
                    break;
                case 5:
                    tableView.Table.Rows[RowIndex]["Active (B/S/$)"] = Value;
                    break;
                case 6:
                    tableView.Table.Rows[RowIndex]["Filled (B/S/$)"] = Value;
                    break;
                case 7:
                    tableView.Table.Rows[RowIndex]["Pred. NN (Y/X)"] = Value;
                    break;
                case 8:
                    tableView.Table.Rows[RowIndex]["Pred. ML (LB/UP)"] = Value;
                    break;
                default:
                    break;
            }
        }
        private void createOrder()
        {
            var buttons = new List<Button>();

            // This tests dynamically adding buttons; ensuring the dialog resizes if needed and 
            // the buttons are laid out correctly
            var dialog = new Dialog($"Create new order", 50, 10,
                buttons.ToArray());


            var tPrice = new TextField("Price")
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill(1),
                Height = Dim.Fill(1)
            };

            dialog.Add(tPrice);

            var tAmount = new TextField("Amount")
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill(1),
                Height = Dim.Fill(1)
            };

            dialog.Add(tAmount);

            var CreateBtn = new Button("Create")
            {
                X = Pos.Center(),
                Y = Pos.Center()
            };

            dialog.Add(CreateBtn);

            CreateBtn.Clicked += () =>
            {
                Application.RequestStop();
            };

            //MessageBox.Query(0, 0, " Box", "MSG EDIT",  defaultButton, btns.ToArray());

            Application.Run(dialog);

            //MessageBox.ErrorQuery(0, 0, "Error Box", "MSG EDIT", defaultButton, btns.ToArray());
        }
        private void cancelOrder() { }
        private void quitWindow()
        {
            Application.RequestStop();
        }

        private partial class Graph
        {


            //
            static Queue<PointF> PointPriceData = new Queue<PointF>();
            static List<PointF> PointMakeData = new List<PointF>();
            static List<PointF> PointTakeData = new List<PointF>();
            
            //
            // 3. Series of Take
            static ScatterSeries TakeSeriesGraph { get; set; }
            static ScatterSeries MakeSeriesGraph { get; set; }
            static PathAnnotation PricePathAnnotation { get; set; }

            public static void SetupGraph(GraphView graphView)
            {

                var cyan = Application.Driver.MakeAttribute(Color.BrightCyan, Color.Black);
                var magenta = Application.Driver.MakeAttribute(Color.BrightMagenta, Color.Black);
                var red = Application.Driver.MakeAttribute(Color.BrightRed, Color.Black);
                var white = Application.Driver.MakeAttribute(Color.White, Color.Black);

                //var x1 = _GraphView.AxisY.GetAxisXPosition(_GraphView);
                //var gts = _GraphView.GraphSpaceToScreen(new PointF(x, y));

                // 1. Get orders as types
                IEnumerable<double> MakeOrders = Exchange.OrdersList.Where(x => x.OrderType == "BUY").Select(x => x.Price).ToList();
                IEnumerable<double> TakeOrders = Exchange.OrdersList.Where(x => x.OrderType == "SELL").Select(x => x.Price).ToList();

                // 2. Add Enumrables as PointF
                for (int i = 0; i < MakeOrders.Count(); i++)
                {
                    PointMakeData.Add(new PointF(i, (float)MakeOrders.ElementAt(i)));
                }
                for (int i = 0; i < TakeOrders.Count(); i++)
                {
                    PointTakeData.Add(new PointF(i, (float)TakeOrders.ElementAt(i)));
                }

                TakeSeriesGraph = new ScatterSeries {
                        Points = PointMakeData,
                        Fill = new GraphCellToRender('x')
                };

                // 4. Series of Make
                MakeSeriesGraph = new ScatterSeries()
                {
                    Points = PointTakeData,
                    Fill = new GraphCellToRender('x', red)
                };

                PricePathAnnotation = new PathAnnotation()
                {
                    LineColor = white,
                    Points = PointPriceData.OrderBy(p => p.X).ToList(),
                    BeforeSeries = true,
                }; 

                graphView.Annotations.Add(PricePathAnnotation);

                graphView.Series.Add(TakeSeriesGraph);
                graphView.Series.Add(MakeSeriesGraph);
                
                //_GraphView.SetClip(new Rect(2,2,4,4));
                // leave space for axis labels
                graphView.MarginBottom = 2;
                graphView.MarginLeft = 8;

                // One axis tick/label per
                graphView.AxisX.Increment = 1;
                graphView.AxisX.ShowLabelsEvery = 0;
                graphView.AxisX.Text = "Time →";
                //_GraphView.AxisX.l

                ///_GraphView.AxisY.Minimum = 30000;
                graphView.AxisY.Increment = 10;
                graphView.AxisY.ShowLabelsEvery = 5;
                graphView.AxisY.Text = "↑";

                //_GraphView.AutoSize = true;
                //GraphView.DrawLine(new PointF(0, 2), new PointF(2,6));
                graphView.SetNeedsDisplay();
            }
            public static Task UpdateLiveGraph(GraphView graphView)
            {
                //_GraphView.Reset();

                var y = graphView.Bounds.Height;
                var x = graphView.Bounds.Width;

                // Get Coin collection 
                IEnumerable<double> LivePrices = Exchange.CoinCollectionQueue.Select(x => x.getBaseValueRounded).ToList();
                IEnumerable<double> MakeOrders = Exchange.OrdersList.Where(x => x.OrderType == "BUY").Select(x => x.Price).ToList();
                IEnumerable<double> TakeOrders = Exchange.OrdersList.Where(x => x.OrderType == "SELL").Select(x => x.Price).ToList();

                // 1. Count Price
                int LiveCount = LivePrices.Count();
                int fromPoint = (LiveCount >= x) ? LiveCount - x : 0;
                for (int i = 0; i < x; i++)
                {
                    if (fromPoint == 0)
                        break;

                    if (PointPriceData.Count < x)
                        PointPriceData.Enqueue(new PointF(i, (float)LivePrices.ElementAt(fromPoint)));
                    else
                    {
                        PointPriceData.Dequeue();
                        PointPriceData.Enqueue(new PointF(i, (float)LivePrices.ElementAt(fromPoint)));
                    }
                    fromPoint++;
                }

                // 2. Add Enumrables as PointF
                for (int i = 0; i < MakeOrders.Count(); i++)
                {
                    PointMakeData.Add(new PointF(i, (float)MakeOrders.ElementAt(i)));
                }

                for (int i = 0; i < TakeOrders.Count(); i++)
                {
                    PointTakeData.Add(new PointF(i, (float)TakeOrders.ElementAt(i)));
                }

                // Connect refresh
                PricePathAnnotation.Points = PointPriceData.ToList();
                
                MakeSeriesGraph.Points = PointMakeData.ToList();
                TakeSeriesGraph.Points = PointTakeData.ToList();

                var yp = (y*10);

                if (PointPriceData.Count() > 0)
                {
                    graphView.ScrollOffset = new PointF(0, PointPriceData.Last().Y - yp);
                    // How much graph space each cell of the console depicts
                    graphView.CellSize = new PointF(1, y);
                }

                PricePathAnnotation.Render(graphView);
                
                graphView.SetNeedsDisplay();

                PointMakeData.Clear();
                PointTakeData.Clear();

                return Task.CompletedTask;
            }
            public static void setupLiveGraph(GraphView GraphView)
            {
                GraphView.Reset();

                //about.Text = "Metrics graph and robotstate";

                var black = Application.Driver.MakeAttribute(GraphView.ColorScheme.Normal.Foreground, Color.Black);
                var cyan = Application.Driver.MakeAttribute(Color.BrightCyan, Color.Black);
                var magenta = Application.Driver.MakeAttribute(Color.BrightMagenta, Color.Black);
                var red = Application.Driver.MakeAttribute(Color.BrightRed, Color.Black);
                var white = Application.Driver.MakeAttribute(Color.White, Color.Black);

                GraphView.GraphColor = black;

                List<PointF> OrderPoints = new List<PointF>();
                List<PointF> PriceLine = new List<PointF>();

                Random r = new Random();

                for (int i = 0; i < 25; i++)
                {
                    OrderPoints.Add(new PointF(r.Next(10000), r.Next(60000)));
                    PriceLine.Add(new PointF(r.Next(10000), r.Next(60000)));
                }

                var Buys = new ScatterSeries()
                {
                    Points = OrderPoints
                };

                var line = new PathAnnotation()
                {
                    LineColor = white,
                    Points = PriceLine.OrderBy(p => p.X).ToList(),
                    BeforeSeries = true,
                };

                GraphView.Series.Add(Buys);
                //GraphView.Annotations.Add(line);


                OrderPoints = new List<PointF>();

                for (int i = 0; i < 10; i++)
                {
                    OrderPoints.Add(new PointF(r.Next(10000), r.Next(60000)));
                }


                var Sells = new ScatterSeries()
                {
                    Points = OrderPoints,
                    Fill = new GraphCellToRender('x', red)
                };

                var line2 = new PathAnnotation()
                {
                    LineColor = magenta,
                    Points = OrderPoints.OrderBy(p => p.X).ToList(),
                    BeforeSeries = true,
                };

                GraphView.Series.Add(Sells);
                //graphView.Annotations.Add(line2);

                // How much graph space each cell of the console depicts
                GraphView.CellSize = new PointF(100, 2500);

                // leave space for axis labels
                GraphView.MarginBottom = 2;
                GraphView.MarginLeft = 3;

                // One axis tick/label per
                GraphView.AxisX.Increment = 250;
                GraphView.AxisX.ShowLabelsEvery = 10;
                GraphView.AxisX.Text = "Time →";

                GraphView.AxisY.Increment = 2500;
                GraphView.AxisY.ShowLabelsEvery = 3;
                GraphView.AxisY.Text = "↑";

                var max = line.Points.Union(line2.Points).OrderByDescending(p => p.Y).First();
                GraphView.Annotations.Add(new TextAnnotation() { Text = "(Max)", GraphPosition = new PointF(max.X + (2 * GraphView.CellSize.X), max.Y) });

                GraphView.SetNeedsDisplay();
            }
            public static void MultiBarGraph(GraphView GraphView)
            {
                GraphView.Reset();

                //about.Text = "Housing Expenditures by income thirds 1996-2003";

                var black = Application.Driver.MakeAttribute(GraphView.ColorScheme.Normal.Foreground, Color.Black);
                var cyan = Application.Driver.MakeAttribute(Color.BrightCyan, Color.Black);
                var magenta = Application.Driver.MakeAttribute(Color.BrightMagenta, Color.Black);
                var red = Application.Driver.MakeAttribute(Color.BrightRed, Color.Black);

                GraphView.GraphColor = black;

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

                GraphView.CellSize = new PointF(0.25f, 1000);
                GraphView.Series.Add(series);
                GraphView.SetNeedsDisplay();

                GraphView.MarginLeft = 3;
                GraphView.MarginBottom = 1;

                GraphView.AxisY.LabelGetter = (v) => '$' + (v.Value / 1000f).ToString("N0") + 'k';

                // Do not show x axis labels (bars draw their own labels)
                GraphView.AxisX.Increment = 0;
                GraphView.AxisX.ShowLabelsEvery = 0;
                GraphView.AxisX.Minimum = 0;


                GraphView.AxisY.Minimum = 0;

                var legend = new LegendAnnotation(new Rect(GraphView.Bounds.Width - 20, 0, 20, 5));
                legend.AddEntry(new GraphCellToRender(stiple, series.SubSeries.ElementAt(0).OverrideBarColor), "Lower Third");
                legend.AddEntry(new GraphCellToRender(stiple, series.SubSeries.ElementAt(1).OverrideBarColor), "Middle Third");
                legend.AddEntry(new GraphCellToRender(stiple, series.SubSeries.ElementAt(2).OverrideBarColor), "Upper Third");
                GraphView.Annotations.Add(legend);
            }
            public static void SetupPeriodicTableScatterPlot(GraphView GraphView)
            {
                GraphView.Reset();

                //about.Text = "This graph shows the atomic weight of each element in the periodic table.\nStarting with Hydrogen (atomic Number 1 with a weight of 1.007)";

                //AtomicNumber and AtomicMass of all elements in the periodic table
                GraphView.Series.Add(
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
                GraphView.CellSize = new PointF(1, 5);

                // leave space for axis labels
                GraphView.MarginBottom = 2;
                GraphView.MarginLeft = 3;

                // One axis tick/label per 5 atomic numbers
                GraphView.AxisX.Increment = 5;
                GraphView.AxisX.ShowLabelsEvery = 1;
                GraphView.AxisX.Text = "Atomic Number";
                GraphView.AxisX.Minimum = 0;

                // One label every 5 atomic weight
                GraphView.AxisY.Increment = 5;
                GraphView.AxisY.ShowLabelsEvery = 1;
                GraphView.AxisY.Minimum = 25;

                GraphView.SetNeedsDisplay();
            }
        }
        partial class Misc
        {
            public static void Zoom(GraphView graphView, float factor)
            {
                graphView.CellSize = new PointF(
                    graphView.CellSize.X * factor,
                    graphView.CellSize.Y * factor
                );

                graphView.AxisX.Increment *= factor;
                graphView.AxisY.Increment *= factor;

                graphView.SetNeedsDisplay();
            }
        }
        private partial class Table
        {
            public static void BuildDefaultTable(TableView tableView)
            {
                var Table = new DataTable();

                Table.Columns.Add(new DataColumn("Connection (Cl/Pb/Pr)", typeof(string)));
                Table.Columns.Add(new DataColumn("Live Bid/Ask", typeof(string)));
                Table.Columns.Add(new DataColumn("Live Profit", typeof(string)));
                Table.Columns.Add(new DataColumn("Balance ($/%)", typeof(string)));
                Table.Columns.Add(new DataColumn("Change ($/%)", typeof(string)));
                Table.Columns.Add(new DataColumn("Active (B/S/$)", typeof(string)));
                Table.Columns.Add(new DataColumn("Filled (B/S/$)", typeof(string)));
                Table.Columns.Add(new DataColumn("Pred. NN (Y/X)", typeof(string)));
                Table.Columns.Add(new DataColumn("Pred. ML (LB/UP)", typeof(string)));

                List<object> rowOne = new List<object>(){
                    "1/1/1",
                    "9999999.00",
                    "45$/-5%",
                    "44$/-23%", /*add some negatives to demo styles*/
                    "2.2/32%",
                    "8/5/9234$",
                    "8/3/4234$",
                    "45235Y/353X",
                    "23253.1/92342.2",
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
