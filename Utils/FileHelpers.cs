namespace PdfDownloader.Utils;

public static class FileHelpers
{
    public static bool IsHttpOrHttps(Uri uri) => uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;

    public static Uri? ConvertUrlToUri(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
           (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
           ? uri
           : null;
    }
}