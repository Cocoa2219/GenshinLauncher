namespace GenshinLauncher;

public class NotificateItem(string text, string day, string url)
{
    public string Text { get; set; } = text;
    public string Day { get; set; } = day;
    public string Url { get; set; } = url;
}