namespace BookLibrary
{
	public class Utils
	{
		public static string FloatToString(float f)
		{
			string fs = f.ToString();
			return fs.Replace(',', '.');
		}
	}
}
