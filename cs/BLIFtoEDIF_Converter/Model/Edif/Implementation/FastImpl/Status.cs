using BLIFtoEDIF_Converter.Model.Edif.Abstraction;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl
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
			//(status (written ..))
			if (Written != null)
				return $"(status {Written.ToEdifText()})";
			else
				return "(status)";
		}

		#endregion [IStatus implementation]
	}
}
