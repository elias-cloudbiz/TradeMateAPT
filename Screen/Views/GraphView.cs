using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using Terminal.Gui.Graphs;
using TMPHFT.Screen;

namespace TMPHFT.Screen.Views
{
    class GraphView : Scenario
    {

		GraphView graphView;
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
				 ()=>SetupLifeExpectancyBarGraph(true),  //1
				 ()=>SetupLifeExpectancyBarGraph(false), //2
				 ()=>SetupPopulationPyramid(),           //3
				 ()=>SetupLineGraph(),                   //4
				 ()=>SetupSineWave(),                    //5
				 ()=>SetupDisco(),                       //6
				 ()=>MultiBarGraph()                     //7
			};


			var menu = new MenuBar(new MenuBarItem[] {
				new MenuBarItem ("_File", new MenuItem [] {
					new MenuItem ("Scatter _Plot", "",()=>graphs[currentGraph = 0]()),
					new MenuItem ("_V Bar Graph", "", ()=>graphs[currentGraph = 1]()),
					new MenuItem ("_H Bar Graph", "", ()=>graphs[currentGraph = 2]()) ,
					new MenuItem ("P_opulation Pyramid","",()=>graphs[currentGraph = 3]()),
					new MenuItem ("_Line Graph","",()=>graphs[currentGraph = 4]()),
					new MenuItem ("Sine _Wave","",()=>graphs[currentGraph = 5]()),
					new MenuItem ("Silent _Disco","",()=>graphs[currentGraph = 6]()),
					new MenuItem ("_Multi Bar Graph","",()=>graphs[currentGraph = 7]()),
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
				Width = 60,
				Height = 20,
			};


			Win.Add(graphView);


			var frameRight = new FrameView("About")
			{
				X = Pos.Right(graphView) + 1,
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill(),
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


	}
}
