using System;
using System.IO;

namespace ManuelNaujoks.VSChat
{
	public class RelativeCodePosition
	{
		public RelativeCodePosition(string solutionFile, string file, int line)
		{
			SolutionFile = solutionFile;
			File = file;
			Line = line;
			Shortcut = MakeRelativePath(SolutionFile, File) + ":" + Line;
		}

		public RelativeCodePosition(string shortcut)
		{
			Shortcut = shortcut;

			var splittedLink = shortcut.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

			Line = int.Parse(splittedLink[1]);
		}

		public RelativeCodePosition(string solutionFile, string shortcut)
		{
			SolutionFile = solutionFile;
			Shortcut = shortcut;

			var splittedLink = shortcut.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
			Line = int.Parse(splittedLink[1]);
			File = Path.Combine(Path.GetDirectoryName(SolutionFile), splittedLink[0]);
		}

		public string SolutionFile { get; private set; }
		public string File { get; private set; }
		public int Line { get; private set; }
		public string Shortcut { get; private set; }

		//http://stackoverflow.com/a/340454
		/// <summary>
		/// Creates a relative path from one file or folder to another.
		/// </summary>
		/// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
		/// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
		/// <returns>The relative path from the start directory to the end path.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="UriFormatException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		private static String MakeRelativePath(String fromPath, String toPath)
		{
			if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
			if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

			Uri fromUri = new Uri(fromPath);
			Uri toUri = new Uri(toPath);

			if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (toUri.Scheme.ToUpperInvariant() == "FILE")
			{
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			return relativePath;
		}
	}
}