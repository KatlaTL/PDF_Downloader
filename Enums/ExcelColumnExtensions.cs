using System.Reflection;
public static class ExcelColumnExtensions
{
    public static int ColumnNumber(this ExcelColumns col) => LetterToNumber(GetColumnLetterAttribute(col).Letter);

    public static string ColumnLetter(this ExcelColumns col) => GetColumnLetterAttribute(col).Letter;

    /// <summary>
    /// Konverterer en Excel-kolonnebogstav-streng (fx "A", "AB") til kolonnenummer.
    /// </summary>
    /// <param name="letter">Excel-kolonnebogstav som streng (fx "A", "AB")</param>
    /// <returns>Kolonnens nummer (fx "A" → 1, "AB" → 28)</returns>
    private static int LetterToNumber(string letter)
    {
        int sum = 0;
        for (int i = 0; i < letter.Length; i++)
        {
            sum *= 26;
            sum += letter[i] - 'A' + 1;
        }
        return sum;
    }

    private static ColumnLetterAttribute GetColumnLetterAttribute(ExcelColumns col)
    {
        var type = col.GetType();
        var memInfo = type.GetMember(col.ToString());
        var attr = memInfo[0].GetCustomAttribute<ColumnLetterAttribute>()
                   ?? throw new Exception($"ColumnLetter not set for {col}");
        return attr;
    }
}