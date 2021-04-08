

namespace Systems
{
	public static class Helpers
	{
		public static string GetBytesReadable(long i)
		{
			long num = (i < 0) ? (-i) : i;
			string str;
			double num2;
			if (num >= 1152921504606846976L)
			{
				str = "EB";
				num2 = i >> 50;
			}
			else if (num >= 1125899906842624L)
			{
				str = "PB";
				num2 = i >> 40;
			}
			else if (num >= 1099511627776L)
			{
				str = "TB";
				num2 = i >> 30;
			}
			else if (num >= 1073741824)
			{
				str = "GB";
				num2 = i >> 20;
			}
			else if (num >= 1048576)
			{
				str = "MB";
				num2 = i >> 10;
			}
			else
			{
				if (num < 1024)
				{
					return i.ToString("0 B");
				}
				str = "KB";
				num2 = i;
			}
			return (num2 / 1024.0).ToString("0.## ") + str;
		}
	}
}
