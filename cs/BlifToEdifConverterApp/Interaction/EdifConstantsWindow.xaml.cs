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

		public EdifConstantsWindow(BlifToEdifModelConverter.EdifAdditionalData edifAdditionalData)
		{
			EdifAdditionalData = edifAdditionalData;
			InitializeComponent();

			this.ModelNameBox.Text = edifAdditionalData.ModelName;
			this.EdifLevelBox.Text = edifAdditionalData.EdifLevel.ToString();
			this.PropertyOwnerBox.Text = edifAdditionalData.PropertyOwner;
			this.TechnologyNameBox.Text = edifAdditionalData.TechnologyName;
			this.ExternalNameBox.Text = edifAdditionalData.ExternalName;
			//this.TimestampDateTimePicker.Value = EdifAdditionalData.StatusWrittenTimestamp;
			this.ViewNameBox.Text = edifAdditionalData.GenericViewName;
			this.DeviceComboBox.Text = edifAdditionalData.Device;
			this.PackageComboBox.Text = edifAdditionalData.Package;
			this.SpeedComboBox.Text = edifAdditionalData.Speed;
		}

		public BlifToEdifModelConverter.EdifAdditionalData EdifAdditionalData { get; }

		private void OkButton_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				EdifAdditionalData.ModelName = this.ModelNameBox.Text;
				EdifAdditionalData.EdifLevel = int.Parse(this.EdifLevelBox.Text);
				EdifAdditionalData.PropertyOwner = this.PropertyOwnerBox.Text;
				EdifAdditionalData.TechnologyName = this.TechnologyNameBox.Text;
				EdifAdditionalData.ExternalName = this.ExternalNameBox.Text;
				EdifAdditionalData.StatusWrittenTimestamp = this.TimestampDateTimePicker.Value ?? DateTime.Now;
				EdifAdditionalData.GenericViewName = this.ViewNameBox.Text;
				if(!string.IsNullOrEmpty(this.DeviceComboBox.Text))
					EdifAdditionalData.Device = this.DeviceComboBox.Text;
				if (!string.IsNullOrEmpty(this.PackageComboBox.Text))
					EdifAdditionalData.Package = this.PackageComboBox.Text;
				if (!string.IsNullOrEmpty(this.SpeedComboBox.Text))
					EdifAdditionalData.Speed = this.SpeedComboBox.Text;
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
				e.Text.Contains("-");
		}
	}
}
