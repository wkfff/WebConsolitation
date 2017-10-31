using System;
using System.Collections.Generic;
using Krista.FM.Update.Framework;

namespace Krista.FM.Update.PatchMakerLibrary.SubPrograms
{
	public abstract class ClientSubProgram : BaseSubProgram
	{
	    protected ClientSubProgram(string path)
            : base(path)
	    {
	    }

	    /// <summary>
        /// От клиентских приложений никто не зависит
        /// </summary>
	    public override List<IUpdateCondition> DependentConditions
	    {
            get { return dependentConditions; }
	    }

        /// <summary>
        /// Зависимости
        /// </summary>
	    public override List<Type> SubProgramDependentTypes
	    {
	        get { return new List<Type> {typeof(ServerSubProgram)};}
	    }
	    
	    public override bool IsHandle
	    {
            get { return true; }
	    }
	}
}
