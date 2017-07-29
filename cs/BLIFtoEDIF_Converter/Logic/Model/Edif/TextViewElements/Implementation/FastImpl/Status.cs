using BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Abstraction;

namespace BLIFtoEDIF_Converter.Logic.Model.Edif.TextViewElements.Implementation.FastImpl
{
	class Status : IStatus
	{
		public Status(IWritten written)
		{
			Written = written;
		}

		#region [IStatus implementation]
		
		public IWritten Written { get; }

		public string ToEdifText()
		{
			throw new System.NotImplementedException();
		}

		#endregion [IStatus implementation]
	}
}
