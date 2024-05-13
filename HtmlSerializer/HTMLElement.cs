using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    // Represents an HTML element
    public class HTMLElement
    {
        // The ID attribute of the HTML element
        public string Id { get; set; }

        // The tag name of the HTML element
        public string TagName { get; set; }

        // The list of attributes of the HTML element
        public List<string> Attributes { get; set; }

        // The list of CSS classes associated with the HTML element
        public List<string> Classes { get; set; }

        // The inner HTML content of the HTML element
        public string InnerHTML { get; set; }

        // The parent HTML element in the hierarchy
        public HTMLElement Parent { get; set; }

        // The list of child HTML elements in the hierarchy
        public List<HTMLElement> Children { get; set; }

        // Constructor initializes lists for attributes, classes, and children
        public HTMLElement()
        {
            Attributes = new List<string>();
            Classes = new List<string>();
            Children = new List<HTMLElement>();
        }

        // Retrieves all descendants of the HTML element using breadth-first traversal
        public IEnumerable<HTMLElement> Descendants()
        {
            // Queue to perform breadth-first traversal
            Queue<HTMLElement> queue = new Queue<HTMLElement>();

            // Enqueue the current element (starting point)
            queue.Enqueue(this);
            HTMLElement current;

            // Continue traversal while the queue is not empty
            while (queue.Count > 0)
            {
                // Dequeue the current element
                current = queue.Dequeue();

                // Yield return the current element
                yield return current;

                // Enqueue all children of the current element for further traversal
                foreach (HTMLElement child in current.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }

        // Retrieves all ancestors of the HTML element
        public IEnumerable<HTMLElement> Ancestors()
        {
            // Start with the current element
            HTMLElement current = this;

            // Continue until there are no more ancestors (reached the root)
            while (current != null)
            {
                // Yield return the parent of the current element
                yield return current.Parent;

                // Move to the parent for the next iteration
                current = current.Parent;
            }
        }
    }

    // Represents an HTML attribute
    public class HtmlAttribute
    {
        // The name of the HTML attribute
        public string Name { get; set; }

        // The value of the HTML attribute
        public string Value { get; set; }
    }
}
