using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using BLIFtoEDIF_Converter.Logic;

namespace BlifToEdifConverterApp.Interaction
{
	/// <summary>
	/// Interaction logic for EdifConstantsWindow.xaml
	/// </summary>
	public partial class EdifConstantsWindow : Window
	{
		public EdifConstantsWindow()
		{
			InitializeComponent();
		}

		public EdifConstantsWindow(BlifToEdifModelConverter.EdifConstants edifConstants)
		{
			EdifConstants = edifConstants;
			InitializeComponent();

			this.ModelNameBox.Text = edifConstants.ModelName;
			this.EdifLevelBox.Text = edifConstants.EdifLevel.ToString();
			this.PropertyOwnerBox.Text = edifConstants.PropertyOwner;
			this.TechnologyNameBox.Text = edifConstants.TechnologyName;
			this.ExternalNameBox.Text = edifConstants.ExternalName;
			//this.TimestampDateTimePicker.Value = edifConstants.StatusWrittenTimestamp;
			this.ViewNameBox.Text = edifConstants.GenericViewName;
		}

		public BlifToEdifModelConverter.EdifConstants EdifConstants { get; }

		private void OkButton_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				EdifConstants.ModelName = this.ModelNameBox.Text;
				EdifConstants.EdifLevel = int.Parse(this.EdifLevelBox.Text);
				EdifConstants.PropertyOwner = this.PropertyOwnerBox.Text;
				EdifConstants.TechnologyName = this.TechnologyNameBox.Text;
				EdifConstants.ExternalName = this.ExternalNameBox.Text;
				EdifConstants.StatusWrittenTimestamp = this.TimestampDateTimePicker.Value ?? DateTime.Now;
				EdifConstants.GenericViewName = this.ViewNameBox.Text;
				this.DialogResult = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, $"Exception at data handling. Converting will be stopped. {ex}");
			}
			finally
			{
				this.Close();
			}
		}

		private void EdifLevelBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			Regex regex = new Regex("[^0-9]+");
			e.Handled = regex.IsMatch(e.Text);
		}

		private void CancelButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void ModelNameBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = e.Text == null || 
				!e.Text.Contains("-");
		}
	}
}
