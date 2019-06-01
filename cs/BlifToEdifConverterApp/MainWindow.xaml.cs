﻿using System;
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
using BlifToEdifConverterApp.Interaction;
using BLIFtoEDIF_Converter.Logic;
using BLIFtoEDIF_Converter.Logic.InitCalculator;
using BLIFtoEDIF_Converter.Logic.Parser.Blif;
using BLIFtoEDIF_Converter.Logic.Transformations.Feedback;
using BLIFtoEDIF_Converter.Model.Blif;
using BLIFtoEDIF_Converter.Model.Blif.Function;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction;
using BLIFtoEDIF_Converter.Model.Edif.Factory;
using BLIFtoEDIF_Converter.Model.TextConverter.Blif;
using BLIFtoEDIF_Converter.Model.TextConverter.Blif.Impl;
using BLIFtoEDIF_Converter.Util;
using Microsoft.Win32;

namespace BlifToEdifConverterApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly ITextViewElementsFactory _edifFactory =
			TextViewElementsFactoryCreator.CreaTextViewElementsFactory(Implementations.FastImpl);

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
			Filter = "Edif Files (edif)|*.edif;*.EDIF|Init Files (init)|*.init;*.INIT;"
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
				List<Function> functions = BlifParser.GetFunctions(result);

				List<InitFuncValue> initValues = functions.Select(f => f.CalculateInit()).ToList();
				List<string> stringResults = initValues.Select(iv => iv.ToString()).ToList();
				EdifTextBox.Text = string.Join(Environment.NewLine, stringResults);
				_lastEdifAdditionalData = null;
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
				_edifSaveFileDialog.FileName = System.IO.Path.ChangeExtension(
					_blifOpenFileDialog.FileName, String.Empty); //TODO: ".init" and edif too
			if (_lastEdifAdditionalData != null)
				_edifSaveFileDialog.FileName = _lastEdifAdditionalData.ModelName;
			bool? showDialogResult = _edifSaveFileDialog.ShowDialog();
			if (showDialogResult.HasValue && showDialogResult.Value)
			{
				string text = EdifTextBox.Text;
				File.WriteAllText(_edifSaveFileDialog.FileName, text, _encoding);
			}
		}

		private BlifToEdifModelConverter.EdifAdditionalData _lastEdifAdditionalData;
		private void ConvertToEdif_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				string blifValues = BlifTextBox.Text;
				List<string> result = Regex.Split(blifValues, "\r\n|\r|\n").Where(str => !string.IsNullOrEmpty(str)).ToList();
				Blif blif = BlifParser.GetBlif(result);

				string edifModelName = blif.Model.Name;
				BlifToEdifModelConverter.EdifAdditionalData edifAdditionalData = new BlifToEdifModelConverter.EdifAdditionalData(edifModelName);
				EdifConstantsWindow edifConstantsWindow = new EdifConstantsWindow(edifAdditionalData);
				edifConstantsWindow.Owner = this;
				bool? showResult = edifConstantsWindow.ShowDialog();
				if(!showResult.HasValue || !showResult.Value)
					return;

				string renameLog;
				IEdif edif = blif.ToEdif(_edifFactory, edifAdditionalData, out renameLog);
				
				string edifSrc = edif.ToEdifText();
				string formattedEdifSrc = SrcCodeFormatter.FormatEdifCode(edifSrc);

				//EdifTextBox.Text = "# RenameLog: " + renameLog + Environment.NewLine + formattedEdifSrc;
				EdifTextBox.Text = formattedEdifSrc;
				_lastEdifAdditionalData = edifAdditionalData;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"sorry. show exception message to developer. {Environment.NewLine}Exceptiom: {ex.ToString()}", "BLIF to EDIF Converter",
					MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void RadioUTFEncoding_OnChecked(object sender, RoutedEventArgs e)
		{
			_encoding = Encoding.UTF8;
		}

		private void RadioLocalEncoding_OnChecked(object sender, RoutedEventArgs e)
		{
			_encoding = Encoding.Default;
		}

		private void Feedback_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				string blifValues = BlifTextBox.Text;
				List<string> result = Regex.Split(blifValues, "\r\n|\r|\n").Where(str => !string.IsNullOrEmpty(str)).ToList();
				Blif blif = BlifParser.GetBlif(result);

				Blif transformedBlif = FeedbackTransformation.AddFeedbackToFunction(blif);

				IBlifWriter blifWriter = new BlifWriter();
				string blifText = blifWriter.ToSourceCode(transformedBlif);
				EdifTextBox.Text = blifText;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"sorry. show exception message to developer. {Environment.NewLine}Exceptiom: {ex.ToString()}", "BLIF to EDIF Converter",
					MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
	}
}
