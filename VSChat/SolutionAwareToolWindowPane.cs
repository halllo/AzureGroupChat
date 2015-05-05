using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ManuelNaujoks.VSChat
{
	public class SolutionAwareToolWindowPane : ToolWindowPane
	{
		public SolutionAwareToolWindowPane()
			: base(null)
		{

		}

		private EnvDTE80.DTE2 CachedDt2;
		protected EnvDTE80.DTE2 MasterObjekt
		{
			get
			{
				if (CachedDt2 == null) CachedDt2 = base.GetService(typeof(SDTE)) as EnvDTE80.DTE2;
				return CachedDt2;
			}
		}

		protected Solution RawSolution { get { return MasterObjekt.Solution; } }
	}
}