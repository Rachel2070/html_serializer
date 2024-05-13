using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    // Extension methods for the HTMLElement class
    public static class HTMLElementExtension
    {
        // Extension method to get elements by a given selector
        public static IEnumerable<HTMLElement> GetElementsBySelector(this HTMLElement element, Selector selector)
        {
            // Check if the element or selector is null, and exit the method if true
            if (element == null || selector == null)
                yield break;

            // Get all descendants of the current element
            var descendants = element.Descendants();

            // Iterate through each descendant
            foreach (var descendant in descendants)
            {
                // Check if the current descendant matches the selector criteria
                if (selector.Equals(descendant))
                {
                    // If there is a child selector, recursively search with the child selector
                    if (selector.Child != null)
                    {
                        // Get elements matching the child selector and yield each result
                        var newList = descendant.GetElementsBySelector(selector.Child);
                        foreach (var item in newList)
                        {
                            yield return item;
                        }
                    }
                    else
                    {
                        // If this is the last selector, yield the current descendant
                        yield return descendant;
                    }
                }
            }
        }
    }
}
