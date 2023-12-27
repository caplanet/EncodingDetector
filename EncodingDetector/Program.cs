using UtfUnknown;

namespace EncodingDetecter;

internal class Program
{
	static void Main(string[] args)
	{
		try
		{
			if (args.Length == 0)
				throw new Exception();

			if (!File.Exists(args[0]))
				throw new Exception();

			var result = CharsetDetector.DetectFromFile(args[0]);
			if (result.Detected == null)
				throw new Exception();
			if (result.Detected.Encoding.CodePage == 1256)
				throw new Exception();

			Console.Write(result.Detected.EncodingName);
			return;
		}
		catch
		{
			Console.Write("utf-8");
		}
	}
}
