using System.Net.Http.Headers;
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
            htmlResponse.Address(response.RequestMessage.RequestUri).Status(response.StatusCode);

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
            .Find(element => element.TextContent.Contains(text));
    }

    public static IHtmlAnchorElement? FindAnchorByText(IHtmlDocument document, string text)
    {
        return (IHtmlAnchorElement?)
            document
                .QuerySelectorAll("a")
                .ToList()
                .Find(element => element.TextContent.Contains(text));
    }

    public static IHtmlInputElement? FindInputByLabel(IHtmlDocument document, string label)
    {
        var element = document
            .QuerySelectorAll("label")
            .ToList()
            .Find(element => element.TextContent.Contains(label));

        if (element is null)
        {
            return null;
        }
        return (IHtmlInputElement?)document.GetElementById(element.Attributes["for"].Value);
    }
}
