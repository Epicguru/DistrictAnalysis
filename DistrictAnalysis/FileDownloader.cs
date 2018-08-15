
using System;
using System.IO;
using System.Net;
using System.Text;

public class FileDownloader
{

    public string URL { get; private set; }
    public string FilePath { get; private set; }
    public bool Downloading { get; private set; }

    public FileDownloader(string url, string filePath)
    {
        URL = url;
        FilePath = filePath;
    }

    public bool Download()
    {
        if (Downloading)
            return false;
        Downloading = true;

        CookieWebClient wc = new CookieWebClient(new CookieContainer());
        wc.Credentials = CredentialCache.DefaultNetworkCredentials;
        wc.Headers.Add("Method", "GET");
        wc.Headers.Add("Scheme", "https");
        wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");
        wc.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
        wc.Headers.Add("Accept-Encoding", "gzip, deflate, br");
        wc.Headers.Add("Accept-Language", "en-US,en;q=0.9");
        wc.Headers.Add("upgrade-insecure-requests", "1");

        Console.WriteLine(wc.Headers.ToString());

        Console.WriteLine("Downloading...");

        try
        {
            wc.DownloadProgressChanged += DownloadProgressChanged;
            wc.DownloadDataCompleted += DownloadDataCompleted;
            wc.DownloadDataAsync(new Uri(this.URL));
            return true;
        }
        catch (WebException e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        const int END = 25;

        Console.CursorLeft = 0;
        Console.Write("[");

        Console.CursorLeft = END;
        Console.Write("]");

        Console.CursorLeft = 1;
    }

    private void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
    {
        Console.WriteLine("Done! Downloaded " + e.Result.Length / 1024 + " KB of data.");
        File.WriteAllText(FilePath, Encoding.UTF8.GetString(e.Result ?? new byte[] { }));
        Downloading = false;
    }
}