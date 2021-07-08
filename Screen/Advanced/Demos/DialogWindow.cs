using NStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terminal.Gui;

namespace TMPFT.Screen.Advanced.Views
{
	[ScenarioMetadata(Name: "MessageBoxes", Description: "Demonstrates how to use MessageBoxes")]
	[ScenarioCategory("Controls")]
	[ScenarioCategory("Dialogs")]
	class MessageBoxes : Scenarios
	{
		public override void Setup()
		{
			var frame = new FrameView("MessageBox Options")
			{
				X = Pos.Center(),
				Y = 1,
				Width = Dim.Percent(75),
				Height = 10
			};
			Win.Add(frame);

			var label = new Label("width:")
			{
				X = 0,
				Y = 0,
				Width = 15,
				Height = 1,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			frame.Add(label);
			var widthEdit = new TextField("0")
			{
				X = Pos.Right(label) + 1,
				Y = Pos.Top(label),
				Width = 5,
				Height = 1
			};
			frame.Add(widthEdit);

			label = new Label("height:")
			{
				X = 0,
				Y = Pos.Bottom(label),
				Width = Dim.Width(label),
				Height = 1,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			frame.Add(label);
			var heightEdit = new TextField("0")
			{
				X = Pos.Right(label) + 1,
				Y = Pos.Top(label),
				Width = 5,
				Height = 1
			};
			frame.Add(heightEdit);

			frame.Add(new Label("If height & width are both 0,")
			{
				X = Pos.Right(widthEdit) + 2,
				Y = Pos.Top(widthEdit),
			});
			frame.Add(new Label("the MessageBox will be sized automatically.")
			{
				X = Pos.Right(heightEdit) + 2,
				Y = Pos.Top(heightEdit),
			});

			label = new Label("Title:")
			{
				X = 0,
				Y = Pos.Bottom(label),
				Width = Dim.Width(label),
				Height = 1,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			frame.Add(label);
			var titleEdit = new TextField("Title")
			{
				X = Pos.Right(label) + 1,
				Y = Pos.Top(label),
				Width = Dim.Fill(),
				Height = 1
			};
			frame.Add(titleEdit);

			label = new Label("Message:")
			{
				X = 0,
				Y = Pos.Bottom(label),
				Width = Dim.Width(label),
				Height = 1,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			frame.Add(label);
			var messageEdit = new TextView()
			{
				Text = "Message",
				X = Pos.Right(label) + 1,
				Y = Pos.Top(label),
				Width = Dim.Fill(),
				Height = 5,
				ColorScheme = Colors.Dialog,
			};
			frame.Add(messageEdit);

			label = new Label("Num Buttons:")
			{
				X = 0,
				Y = Pos.Bottom(messageEdit),
				Width = Dim.Width(label),
				Height = 1,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			frame.Add(label);
			var numButtonsEdit = new TextField("3")
			{
				X = Pos.Right(label) + 1,
				Y = Pos.Top(label),
				Width = 5,
				Height = 1
			};
			frame.Add(numButtonsEdit);

			label = new Label("Default Button:")
			{
				X = 0,
				Y = Pos.Bottom(label),
				Width = Dim.Width(label),
				Height = 1,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			frame.Add(label);
			var defaultButtonEdit = new TextField("0")
			{
				X = Pos.Right(label) + 1,
				Y = Pos.Top(label),
				Width = 5,
				Height = 1
			};
			frame.Add(defaultButtonEdit);

			label = new Label("Style:")
			{
				X = 0,
				Y = Pos.Bottom(label),
				Width = Dim.Width(label),
				Height = 1,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			frame.Add(label);
			var styleRadioGroup = new RadioGroup(new ustring[] { "Not selected" ,"_Buy", "_Sell" })
			{
				X = Pos.Right(label) + 1,
				Y = Pos.Top(label),
			};
			frame.Add(styleRadioGroup);

			void Top_Loaded()
			{
				frame.Height = Dim.Height(widthEdit) + Dim.Height(heightEdit) + Dim.Height(titleEdit) + Dim.Height(messageEdit)
				+ Dim.Height(numButtonsEdit) + Dim.Height(defaultButtonEdit) + Dim.Height(styleRadioGroup) + 2;
				Top.Loaded -= Top_Loaded;
			}
			Top.Loaded += Top_Loaded;

			label = new Label("Button Pressed:")
			{
				X = Pos.Center(),
				Y = Pos.Bottom(frame) + 4,
				Height = 1,
				TextAlignment = Terminal.Gui.TextAlignment.Right,
			};
			Win.Add(label);
			var buttonPressedLabel = new Label(" ")
			{
				X = Pos.Center(),
				Y = Pos.Bottom(frame) + 5,
				Width = 25,
				Height = 1,
				ColorScheme = Colors.Error,
				TextAlignment = Terminal.Gui.TextAlignment.Centered
			};

			//var btnText = new [] { "_Zero", "_One", "T_wo", "_Three", "_Four", "Fi_ve", "Si_x", "_Seven", "_Eight", "_Nine" };

			var showMessageBoxButton = new Button("Show MessageBox")
			{
				X = Pos.Center(),
				Y = Pos.Bottom(frame) + 2,
				IsDefault = true,
			};
			showMessageBoxButton.Clicked += () => {
				try
				{
					int width = int.Parse(widthEdit.Text.ToString());
					int height = int.Parse(heightEdit.Text.ToString());
					int numButtons = int.Parse(numButtonsEdit.Text.ToString());
					int defaultButton = int.Parse(defaultButtonEdit.Text.ToString());

					var btns = new List<ustring>();
					for (int i = 0; i < numButtons; i++)
					{
						//btns.Add(btnText[i % 10]);
						btns.Add(NumberToWords.Convert(i));
					}
					if (styleRadioGroup.SelectedItem == 0)
					{
						buttonPressedLabel.Text = $"{MessageBox.Query(width, height, titleEdit.Text.ToString(), messageEdit.Text.ToString(), defaultButton, btns.ToArray())}";
					}
					else
					{
						buttonPressedLabel.Text = $"{MessageBox.ErrorQuery(width, height, titleEdit.Text.ToString(), messageEdit.Text.ToString(), defaultButton, btns.ToArray())}";
					}
				}
				catch (FormatException)
				{
					buttonPressedLabel.Text = "Invalid Options";
				}
			};
			Win.Add(showMessageBoxButton);

			Win.Add(buttonPressedLabel);
		}
	}

	public static class NumberToWords
	{
		private static String[] units = { "Zero", "One", "Two", "Three",
			"Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven",
			"Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
			"Seventeen", "Eighteen", "Nineteen" };
		private static String[] tens = { "", "", "Twenty", "Thirty", "Forty",
			"Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

		public static String ConvertAmount(double amount)
		{
			try
			{
				Int64 amount_int = (Int64)amount;
				Int64 amount_dec = (Int64)Math.Round((amount - (double)(amount_int)) * 100);
				if (amount_dec == 0)
				{
					return Convert(amount_int) + " Only.";
				}
				else
				{
					return Convert(amount_int) + " Point " + Convert(amount_dec) + " Only.";
				}
			}
			catch (Exception e)
			{
				throw new ArgumentOutOfRangeException(e.Message);
			}
		}

		public static String Convert(Int64 i)
		{
			if (i < 20)
			{
				return units[i];
			}
			if (i < 100)
			{
				return tens[i / 10] + ((i % 10 > 0) ? " " + Convert(i % 10) : "");
			}
			if (i < 1000)
			{
				return units[i / 100] + " Hundred"
					+ ((i % 100 > 0) ? " And " + Convert(i % 100) : "");
			}
			if (i < 100000)
			{
				return Convert(i / 1000) + " Thousand "
				+ ((i % 1000 > 0) ? " " + Convert(i % 1000) : "");
			}
			if (i < 10000000)
			{
				return Convert(i / 100000) + " Lakh "
					+ ((i % 100000 > 0) ? " " + Convert(i % 100000) : "");
			}
			if (i < 1000000000)
			{
				return Convert(i / 10000000) + " Crore "
					+ ((i % 10000000 > 0) ? " " + Convert(i % 10000000) : "");
			}
			return Convert(i / 1000000000) + " Arab "
				+ ((i % 1000000000 > 0) ? " " + Convert(i % 1000000000) : "");
		}
	}
}
