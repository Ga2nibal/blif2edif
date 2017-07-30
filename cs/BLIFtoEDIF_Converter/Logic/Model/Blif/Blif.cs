using System;
using System.Collections.Generic;

namespace BLIFtoEDIF_Converter.Logic.Model.Blif
{
	public class Blif
	{
		public Blif(Model model, Inputs inputs, Outputs outputs, IReadOnlyList<Function.Function> functions)
		{
			if(null == model)
				throw new ArgumentNullException(nameof(model), $"{nameof(model)} is not defined");
			if (null == inputs)
				throw new ArgumentNullException(nameof(inputs), $"{nameof(inputs)} is not defined");
			if (null == outputs)
				throw new ArgumentNullException(nameof(outputs), $"{nameof(outputs)} is not defined");
			if (null == functions)
				throw new ArgumentNullException(nameof(functions), $"{nameof(functions)} is not defined");
			Model = model;
			Inputs = inputs;
			Outputs = outputs;
			Functions = functions;
		}

		public Model Model { get; }
		public Inputs Inputs { get; }
		public Outputs Outputs { get; }
		public IReadOnlyList<Function.Function> Functions { get; } 
	}
}
