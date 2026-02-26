public static class ExcelColumnConverter
{

    /// <summary>
    /// Konverterer en Excel-kolonnebogstav-streng (fx "A", "AB") til kolonnenummer.
    /// </summary>
    /// <param name="letter">Excel-kolonnebogstav som streng (fx "A", "AB")</param>
    /// <returns>Kolonnens nummer (fx "A" → 1, "AB" → 28)</returns>
    public static int LetterToNumber(string letter)
    {
        if (string.IsNullOrWhiteSpace(letter))
        {
            throw new ArgumentException("Column letter cannot be empty.");
        }

        int sum = 0;
        for (int i = 0; i < letter.Length; i++)
        {
            sum *= 26;
            sum += letter[i] - 'A' + 1;
        }
        return sum;
    }
}