using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BlifToEdifConverterApp.Annotations;

namespace BlifToEdifConverterApp
{
	public class AppViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
