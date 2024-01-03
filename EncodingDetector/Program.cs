
using System.Text;
using UtfUnknown;

namespace EncodingDetector;

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

			var file = File.ReadAllBytes(args[0]);
			if (file.Length < 5)
				throw new Exception();

			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			byte[] NewLine;
			if (file[3] == '\n')
				NewLine = "\n"u8.ToArray();
			else if (file[3] == '\r' && file[4] == '\n')
				NewLine = "\r\n"u8.ToArray();
			else if (file[3] == '\r')
				NewLine = "\r"u8.ToArray();
			else
				throw new Exception();

			var __0 = NewLine.Concat("  0"u8.ToArray().Concat(NewLine)).ToArray();
			var __0Pos = Find(file, __0);
			var __0List = Split(file, __0Pos, __0);

			var TEXT = "TEXT"u8.ToArray().Concat(NewLine).ToArray();
			var __1 = "  1"u8.ToArray().Concat(NewLine).ToArray();

			MemoryStream ms = new();
			foreach (var item in __0List)
				if (StartWith(item, TEXT))
					ms.Write(NextLine(item, __1, NewLine));
			ms.Flush();
			ms.Position = 0;

			var result = CharsetDetector.DetectFromStream(ms);
			ms.Close();
			if (result.Detected.Confidence <= 0.5)
				throw new Exception();

			Console.Write(result.Detected.EncodingName);
		}
		catch
		{
			Console.Write("utf-8");
		}
	}

	private static byte[] NextLine(byte[] arr, byte[] key, byte[] newLine)
	{
		int match;
		int start = 0;
		int end = 0;

		for (int i = 0; i < arr.Length;)
		{
			match = 0;
			while (match < key.Length)
			{
				if (i + match >= arr.Length)
					return Array.Empty<byte>();
				if (arr[i + match] == key[match])
					match++;
				else
					break;
			}
			if (match == key.Length)
			{
				start = i + key.Length;
				break;
			}
			i += match > 0 ? match : 1;
		}
		for (int i = start; i < arr.Length;)
		{
			match = 0;
			while (match < newLine.Length)
			{
				if (i + match >= arr.Length)
					return Array.Empty<byte>();
				if (arr[i + match] == newLine[match])
					match++;
				else
					break;
			}
			if (match == newLine.Length)
			{
				end = i + newLine.Length;
				break;
			}
			i += match > 0 ? match : 1;
		}
		var ret = end == arr.Length ? arr[start..] : arr[start..end];
		return ret;
	}

	private static bool StartWith(byte[] item, byte[] TEXT)
	{
		if (item.Length < TEXT.Length)
			return false;

		for (int i = 0; i < TEXT.Length; i++)
			if (item[i] != TEXT[i])
				return false;
		return true;
	}

	private static List<byte[]> Split(byte[] file, List<int> __0Pos, byte[] __0)
	{
		int pos = __0Pos[0] + __0.Length;
		List<byte[]> ret = new();

		for (int i = 1; i < __0Pos.Count - 1; i++)
		{
			ret.Add(file[pos..__0Pos[i]]);
			pos = __0Pos[i] + __0.Length;
		}

		ret.Add(file[pos..]);
		return ret;
	}

	private static List<int> Find(byte[] arr, byte[] key)
	{
		List<int> ret = new List<int>();
		int match;
		for (int i = 0; i < arr.Length;)
		{
			match = 0;
			while (match < key.Length)
			{
				if (i + match >= arr.Length)
					return ret;
				if (arr[i + match] == key[match])
					match++;
				else
					break;
			}
			if (match == key.Length)
				ret.Add(i);
			i += match > 0 ? match : 1;
		}

		return ret;
	}


}
