namespace CoffeeExchange.Data.Response.Errors;

public class CustomErrorContent
{
    public CustomErrorContent(string title, string content)
    {
        Title = title;
        Content = content;
    }

    public string Title { get; }

    public string Content { get; }
}