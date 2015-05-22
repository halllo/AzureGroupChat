using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ManuelNaujoks.VSChat
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideToolWindow(typeof(MyToolWindow), MultiInstances = true, Transient = true)]
	[Guid(GuidList.guidVSChatPkgString)]
	public sealed class VSChatPackage : Package
	{
		public VSChatPackage()
		{
		}

		private void CreateMyWindow(object sender, EventArgs e)
		{
			for (int i = 0; ; i++)
			{
				var currentWindow = this.FindToolWindow(typeof(MyToolWindow), i, false);
				if (currentWindow == null)
				{
					var window = (ToolWindowPane)this.CreateToolWindow(typeof(MyToolWindow), i);
					if ((null == window) || (null == window.Frame))
					{
						throw new NotSupportedException(Resources.CanNotCreateWindow);
					}
					IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;

					Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
					break;
				}
			}
		}

		protected override void Initialize()
		{
			base.Initialize();

			OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (null != mcs)
			{
				CommandID toolwndCommandID = new CommandID(GuidList.guidVSChatCmdSet, (int)PkgCmdIDList.cmdidGroupChat);
				MenuCommand menuToolWin = new MenuCommand(CreateMyWindow, toolwndCommandID);
				mcs.AddCommand(menuToolWin);
			}
		}
	}
}
