using System.Text;

public class Base62Converter 
{
    private const string chars = "1ko058RSqamMAwvCB69Tdph2bNDYQcgferEHUitsL3Pn7KIylVjzWXOZxFG4Ju";

    public static string ToBase62(ulong number)
    {
        var result = new StringBuilder();

        while (number > 0)
        {
            var remainder = number % 62;
            result.Insert(0, chars[(int)remainder]);
            number /= 62;
        }

        return result.ToString();
    }

    public static ulong FromBase62(string base62String)
    {
        ulong result = 0;

        foreach (var c in base62String)
        {
            var index = chars.IndexOf(c);
            result = result * 62 + (ulong)index;
        }

        return result;
    }
}