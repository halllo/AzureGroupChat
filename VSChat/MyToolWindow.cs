using System;
using System.Runtime.InteropServices;
using EnvDTE;

namespace ManuelNaujoks.VSChat
{
	[Guid("173cbcde-e728-442c-82ee-1c29ae3e00af")]
	public class MyToolWindow : SolutionAwareToolWindowPane
	{
		public MyToolWindow()
		{
			Caption = Resources.ToolWindowTitle;
			BitmapResourceID = 301;
			BitmapIndex = 1;

			base.Content = new MyControl { GetRelativeCodePosition = MakeRelativeCodePosition };
		}

		protected override void OnClose()
		{
			var chat = (MyControl)base.Content;
			chat.Closing();

			base.OnClose();
		}

		private void MakeRelativeCodePosition(Action<RelativeCodePosition> callback)
		{
			try
			{
				var activeDocument = MasterObjekt.ActiveDocument;
				if (activeDocument != null)
				{
					var selection = (TextSelection)activeDocument.Selection;
					if (selection != null)
					{
						if (RawSolution != null)
						{
							callback(new RelativeCodePosition
							{
								SolutionFile = RawSolution.FileName,
								File = activeDocument.FullName,
								Line = selection.AnchorPoint.Line
							});
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
