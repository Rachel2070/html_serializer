using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    // Represents a selector used for HTML element matching
    public class Selector
    {
        // The HTML tag name to match
        public string TagName { get; set; }

        // The HTML element ID to match
        public string Id { get; set; }

        // The list of CSS classes to match
        public List<string> Classes { get; set; }

        // The parent selector in a hierarchical selector structure
        public Selector Parent { get; set; }

        // The child selector in a hierarchical selector structure
        public Selector Child { get; set; }

        // Constructor initializes the list of classes
        public Selector()
        {
            Classes = new List<string>();
        }

        // Converts a string representation of a selector to a Selector object
        public static Selector ConvertToSelector(string str)
        {
            // Get the instance of HtmlHelper for HTML tag and void tag information
            HtmlSerializer.HtmlHelper allHtmlTags = HtmlSerializer.HtmlHelper.Instance;

            // Split the string into parts based on space
            string[] parts = str.Split(' ');

            // Initialize the root and temporary selectors
            Selector root = null;
            Selector temp = null;

            // Iterate through each part of the selector
            foreach (string part in parts)
            {
                // Split each part into sub-parts based on space

                // Create a new Selector object for the current part
                Selector newSelector = new Selector();

                // Add space before '#' and '.' characters
                string newPart = AddSpaceBeforeHashAndDot(part);

                // Remove leading '#' or '.' if present
                if (part.StartsWith('.') || part.StartsWith('#'))
                {
                    newPart = newPart.Substring(1);
                }

                // Split the new part into sub-parts based on space
                string[] subParts = newPart.Split(' ');

                // Iterate through each sub-part
                for (int i = 0; i < subParts.Length; i++)
                {
                    // Check if the sub-part starts with '.'
                    if (subParts[i].StartsWith("."))
                    {
                        // Add the class (remove leading '.')
                        newSelector.Classes.Add(subParts[i].Substring(1));
                    }
                    // Check if the sub-part starts with '#'
                    else if (subParts[i].StartsWith("#"))
                    {
                        // Set the ID (remove leading '#')
                        newSelector.Id = subParts[i].Substring(1);
                    }
                    // Check if the sub-part is a valid HTML tag or void tag
                    else if (allHtmlTags.HtmlTags.Any(s => s == subParts[i]) || allHtmlTags.HtmlVoidTags.Any(s => s == subParts[i]))
                    {
                        // Set the tag name
                        newSelector.TagName = subParts[i];
                    }
                }

                // If root is null, set it as the new selector; otherwise, set it as the child of the temporary selector
                if (root == null)
                {
                    root = newSelector;
                    temp = root;
                }
                else
                {
                    temp.Child = newSelector;
                    newSelector.Parent = temp;
                    temp = newSelector;
                }
            }

            // Return the root of the selector hierarchy
            return root;
        }

        // Adds space before '#' and '.' characters in a string
        static string AddSpaceBeforeHashAndDot(string input)
        {
            StringBuilder result = new StringBuilder();

            // Iterate through each character in the input
            foreach (char character in input)
            {
                // Add space before '#' or '.' characters
                if (character == '#' || character == '.')
                {
                    result.Append(' ').Append(character);
                }
                else
                {
                    // Keep other characters as is
                    result.Append(character);
                }
            }

            // Return the modified string
            return result.ToString();
        }

        // Custom Equals method to compare with HTMLElement
        public override bool Equals(object? obj)
        {
            // Check if the object is an instance of HTMLElement
            if (obj is HTMLElement)
            {
                // Cast the object to HTMLElement
                HTMLElement? element = obj as HTMLElement;

                // Check if the cast was successful
                if (element != null)
                {
                    // Compare tag name, ID, and classes
                    bool tagNameMatches = string.IsNullOrEmpty(TagName) || TagName == element.TagName;
                    bool idMatches = string.IsNullOrEmpty(Id) || Id == element.Id;
                    bool classesMatch = Classes.All(className => element.Classes.Contains(className));

                    // Return true if all criteria match
                    return tagNameMatches && idMatches && classesMatch;
                }
            }

            // Return false if the object is not an HTMLElement
            return false;
        }

  
    }
}
