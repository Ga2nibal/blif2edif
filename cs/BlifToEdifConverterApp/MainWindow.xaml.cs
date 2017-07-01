using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BLIFtoEDIF_Converter.init_calculator;
using Microsoft.Win32;

namespace BlifToEdifConverterApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly OpenFileDialog _blifOpenFileDialog = new OpenFileDialog()
		{
			Filter = "Blif Files (blif)|*.blif;*.BLIF;"
		};
		private readonly SaveFileDialog _blifSaveFileDialog = new SaveFileDialog()
		{
			Filter = "Blif Files (blif)|*.blif;*.BLIF;"
		};


		private readonly OpenFileDialog _edifOpenFileDialog = new OpenFileDialog()
		{
			Filter = "Edif Files (edif)|*.edif;*.EDIF|Init Files (init)|*.init;*.INIT;" //TODO: and init too
		};
		private readonly SaveFileDialog _edifSaveFileDialog = new SaveFileDialog()
		{
			Filter = "Edif Files (blif)|*.edif;*.EDIF|Init Files (init)|*.init;*.INIT;"
		};

		private Encoding _encoding = Encoding.UTF8;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void CalcInit_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				string blifValues = BlifTextBox.Text;
				List<string> result = Regex.Split(blifValues, "\r\n|\r|\n").Where(str => !string.IsNullOrEmpty(str)).ToList();
				List<Function> functions = FunctionPareser.GetFunctions(result);

				List<InitFuncValue> initValues = functions.Select(f => f.CalculateInit()).ToList();
				List<string> stringResults = initValues.Select(iv => iv.ToString()).ToList();
				EdifTextBox.Text = string.Join(Environment.NewLine, stringResults);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"sorry. show exception message to developer. {Environment.NewLine}Exceptiom: {ex.ToString()}", "BLIF to EDIF Converter",
					MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void LoadBlif_OnClick(object sender, RoutedEventArgs e)
		{
			bool? showDialogResult = _blifOpenFileDialog.ShowDialog();
			if (showDialogResult.HasValue && showDialogResult.Value)
			{
				string text = File.ReadAllText(_blifOpenFileDialog.FileName, _encoding);
				BlifTextBox.Text = text;
			}
		}

		private void SaveBlif_OnClick(object sender, RoutedEventArgs e)
		{
			bool? showDialogResult = _blifSaveFileDialog.ShowDialog();
			if (showDialogResult.HasValue && showDialogResult.Value)
			{
				string text = BlifTextBox.Text;
				File.WriteAllText(_blifSaveFileDialog.FileName, text, _encoding);
			}
		}

		private void LoadEdif_OnClick(object sender, RoutedEventArgs e)
		{
			bool? showDialogResult = _edifOpenFileDialog.ShowDialog();
			if (showDialogResult.HasValue && showDialogResult.Value)
			{
				string text = File.ReadAllText(_edifOpenFileDialog.FileName, _encoding);
				EdifTextBox.Text = text;
			}
		}

		private void SaveEdif_OnClick(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrEmpty(_blifOpenFileDialog.FileName))
			{
				_edifSaveFileDialog.FileName = System.IO.Path.ChangeExtension(
					_blifOpenFileDialog.FileName, String.Empty); //TODO: ".init" and edif too
			}
			bool? showDialogResult = _edifSaveFileDialog.ShowDialog();
			if (showDialogResult.HasValue && showDialogResult.Value)
			{
				string text = EdifTextBox.Text;
				File.WriteAllText(_edifSaveFileDialog.FileName, text, _encoding);
			}
		}

		private void ConvertToEdif_OnClick(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Not implemented yet");
		}

		private void RadioUTFEncoding_OnChecked(object sender, RoutedEventArgs e)
		{
			_encoding = Encoding.UTF8;
		}

		private void RadioLocalEncoding_OnChecked(object sender, RoutedEventArgs e)
		{
			_encoding = Encoding.Default;
		}
	}
}
