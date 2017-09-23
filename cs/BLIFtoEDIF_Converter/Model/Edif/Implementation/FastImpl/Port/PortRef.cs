using System;
using System.Text;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Instance;
using BLIFtoEDIF_Converter.Model.Edif.Abstraction.Port;

namespace BLIFtoEDIF_Converter.Model.Edif.Implementation.FastImpl.Port
{
	class PortRef : IPortRef
	{
		public PortRef(string name, IInstanceRef instanceRef)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException(nameof(name), $"{nameof(name)} is not defined");
			Name = name;
			InstanceRef = instanceRef;
		}

		#region [IPortRef implementation]
		
		public string Name { get; }
		public IInstanceRef InstanceRef { get; }

		public string ToEdifText()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("(portRef");
			if (Name != null)
			{
				builder.Append(" ");
				builder.Append(Name);
			}
			if (InstanceRef != null)
			{
				builder.Append(" ");
				builder.Append(InstanceRef.ToEdifText());
			}
			builder.Append(")");
			//(portRef I (instanceRef x20_IBUF_renamed_4))
			return builder.ToString();
		}

		#endregion [IPortRef implementation]
	}
}
