using System.Net.Http.Headers;
using System.Web;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Io;
using Xunit.Abstractions;

namespace Webapp.Tests.Helpers;

public class HtmlHelpers
{
    public static async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var document = await BrowsingContext
            .New()
            .OpenAsync(ResponseFactory, CancellationToken.None);
        return (IHtmlDocument)document;

        void ResponseFactory(VirtualResponse htmlResponse)
        {
            htmlResponse.Address(response.RequestMessage?.RequestUri).Status(response.StatusCode);

            MapHeaders(response.Headers);
            MapHeaders(response.Content.Headers);

            htmlResponse.Content(content);

            void MapHeaders(HttpHeaders headers)
            {
                foreach (var header in headers)
                {
                    foreach (var value in header.Value)
                    {
                        htmlResponse.Header(header.Key, value);
                    }
                }
            }
        }
    }

    public static IElement? FindElementByText(IHtmlDocument document, string text)
    {
        // TODO: Can we do better than comparing all elements ?
        return document
            .QuerySelectorAll("*")
            .ToList()
            .Find(element => element.TextContent.Trim().Equals(text));
    }

    public static IElement? FindElementByAriaLabel(IHtmlDocument document, string ariaLabel)
    {
        // TODO: Can we do better than comparing all elements ?
        return document
            .QuerySelectorAll("*")
            .ToList()
            .Find(element => element.GetAttribute("aria-label") == ariaLabel);
    }

    public static IElement? FindElementByTextAndAriaLabel(
        IHtmlDocument document,
        string text,
        string ariaLabel
    )
    {
        // TODO: Can we do better than comparing all elements ?
        return document
            .QuerySelectorAll("*")
            .ToList()
            .Find(element =>
                element.TextContent.Trim().Equals(text)
                && element.GetAttribute("aria-label") == ariaLabel
            );
    }

    public static IHtmlAnchorElement? FindAnchorByText(IHtmlDocument document, string text)
    {
        return document
            .QuerySelectorAll("a")
            .OfType<IHtmlAnchorElement>()
            .ToList()
            .Find(element => element.TextContent.Trim().Equals(text));
    }

    public static IHtmlAnchorElement? FindAnchorByTextAndHref(
        IHtmlDocument document,
        string text,
        string href
    )
    {
        var elements = document.QuerySelectorAll("a");
        var links = document.QuerySelectorAll("a").OfType<IHtmlAnchorElement>().ToList();
        return document
            .QuerySelectorAll("a")
            .OfType<IHtmlAnchorElement>()
            .ToList()
            .Find(element =>
                element.TextContent.Trim().Contains(text)
                && HttpUtility.UrlDecode(element.Href).EndsWith(href)
            );
    }

    public static IHtmlInputElement? FindInputByLabel(IHtmlDocument document, string label)
    {
        var element = document
            .QuerySelectorAll("label")
            .OfType<IHtmlLabelElement>()
            .ToList()
            .Find(element => element.TextContent.Trim().Equals(label));

        if (element is null || element.HtmlFor is null)
        {
            return null;
        }
        return (IHtmlInputElement?)document.GetElementById(element.HtmlFor);
    }
}
