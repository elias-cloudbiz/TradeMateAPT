using NStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Terminal.Gui;

namespace TMPFT.Screen.Advanced.Views
{
    [ScenarioMetadata(Name: "Client/Software", Description: "Startup/Authentication")]
    [ScenarioCategory("Client")]
    class StartupWindow : Scenarios
    {
		FrameView FrameView = new FrameView();
        internal ProgressBar ActivityProgressBar { get; private set; }
        private string ProgressState { get; set; } = "Awaiting ...";
        //
        private Timer _systemTimer;
        private uint _systemTimerTick = 100; // ms

												  //
		internal Action StartBtnClick;
		internal Action StopBtnClick;
		internal Action PulseBtnClick = null;
		//
		private Label _startedLabel;
		internal TextField Speed { get; private set; }

		internal bool Started
		{
			get
			{
				return _startedLabel.Text == "Started";
			}
			private set
			{
				_startedLabel.Text = value ? "Started" : "Stopped";
			}
		}
		public StartupWindow() { }

		internal void Start()
		{
			Started = true;
			StartBtnClick?.Invoke();
		}

		internal void Stop()
		{
			Started = false;
			StopBtnClick?.Invoke();
		}
		internal void Pulse()
		{
			if (PulseBtnClick != null)
			{
				PulseBtnClick?.Invoke();

			}
			else
			{
				if (ActivityProgressBar.Fraction + 0.01F >= 1)
				{
					ActivityProgressBar.Fraction = 0F;
				}
				else
				{
					ActivityProgressBar.Fraction += 0.01F;
				}
				ActivityProgressBar.Pulse();
			}
		}

		public override void Setup()
		{
			// Demo #1 - Use System.Timer (and threading)
			this.StartBtnClick = () => {
				_systemTimer?.Dispose();
				_systemTimer = null;

				this.ActivityProgressBar.Fraction = 0F;

				_systemTimer = new Timer((o) => {
					// Note the check for Mainloop being valid. System.Timers can run after they are Disposed.
					// This code must be defensive for that. 
					Application.MainLoop?.Invoke(() => this.Pulse());
				}, null, 0, _systemTimerTick);
			};




			

			this.StopBtnClick = () => {
				_systemTimer?.Dispose();
				_systemTimer = null;

				this.ActivityProgressBar.Fraction = 1F;
			};
/*
            Speed.Text = $"{_systemTimerTick}";
			this.Speed.TextChanged += (a) => {
				uint result;
				if (uint.TryParse(this.Speed.Text.ToString(), out result))
				{
					_systemTimerTick = result;
					System.Diagnostics.Debug.WriteLine($"{_systemTimerTick}");
					if (this.Started)
					{
						this.Start();
					}

				}
				else
				{
					System.Diagnostics.Debug.WriteLine("bad entry");
				}
			};*/
			Win.Add(FrameView);

			//FrameView.Add(ActivityProgressBar);

			var startBoth = new Button("Start Both")
			{
				X = Pos.Center(),
				Y = Pos.Bottom(FrameView) + 1,
			};
			startBoth.Clicked += () => {
				this.Start();
			};
			Win.Add(startBoth);
		}

		protected override void Dispose(bool disposing)
		{
			foreach (var v in Win.Subviews.OfType<StartupWindow>())
			{
				v?.StopBtnClick();
			}
			base.Dispose(disposing);
		}
	}
}
