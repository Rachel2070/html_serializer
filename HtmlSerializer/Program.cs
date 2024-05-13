using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    class Program
    {
        static async Task Main()
        {
            // Initialize the HtmlHelper instance
            HtmlSerializer.HtmlHelper allHtmlTags = HtmlSerializer.HtmlHelper.Instance;

            if (allHtmlTags != null)
            {
                // Prompt user to enter the website link
                //Console.WriteLine("Enter website link:");
                //string link = Console.ReadLine();
                string link = "https://yourweddingcountdown.com/36757";

                // Load HTML content from the provided website link
                var html = await Load(link);

                // Clean up HTML content and split it into lines
                var cleanHtml = new Regex("\\s").Replace(html, " ");
                var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0);
                htmlLines = htmlLines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();

                // Initialize variables for building the HTML element tree
                HtmlSerializer.HTMLElement root = null;
                HtmlSerializer.HTMLElement temp = root;

                // Process each line of the HTML content
                foreach (var line in htmlLines)
                {
                    // Extract the first word from the line
                    string firstWord = line.Contains(' ') ? line.Substring(0, line.IndexOf(' ')) : line;

                    // Check for the end of the HTML document
                    if (firstWord == "/html")
                        break;

                    // Handle closing tags
                    else if (firstWord.StartsWith("/"))
                    {
                        if (temp?.Parent != null)
                            temp = temp.Parent;
                    }

                    // Handle opening tags
                    else if (allHtmlTags.HtmlTags.Any(s => s == firstWord) || allHtmlTags.HtmlVoidTags.Any(s => s == firstWord))
                    {
                        HtmlSerializer.HTMLElement newElement = new HtmlSerializer.HTMLElement();

                        newElement.TagName = firstWord;

                        var attributeList = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(line);

                        // Process attributes of the HTML element
                        foreach (Match attribute in attributeList)
                        {
                            newElement.Attributes.Add(attribute.Value);
                            string attributeName = attribute.Groups[1].Value;
                            string attributeValue = attribute.Groups[2].Value;

                            // Handle special attributes like class and id
                            if (attributeName.ToLower() == "class")
                            {
                                var listClass = attributeValue.Split(' ').ToList();
                                foreach (string listItem in listClass)
                                {
                                    newElement.Classes.Add(listItem);
                                }
                            }
                            else if (attributeName.ToLower() == "id")
                            {
                                var id = attributeValue;
                                newElement.Id = id;
                            }
                        }

                        // Build the HTML element tree
                        if (root == null)
                        {
                            root = newElement;
                            temp = root;
                        }
                        else
                        {
                            newElement.Parent = temp;
                            temp.Children.Add(newElement);

                            // Move the current pointer to the new element if it's not a self-closing tag
                            if (allHtmlTags.HtmlTags.Any(s => s == firstWord) && line[line.Length - 1] != '/')
                            {
                                temp = newElement;
                            }
                        }
                    }

                    // Handle inner content of the HTML element
                    else
                    {
                        if (temp != null)
                        {
                            // Exclude script content and functions from InnerHTML
                            if (!line.Contains("<script") && !line.Contains("(function(") && firstWord != "script")
                            {
                                temp.InnerHTML += line;
                            }
                        }
                    }
                }
                //PrintTree(root, "");
                for(int i=0; i<3; i++) { 
                // Prompt user to enter the selector query
                Console.WriteLine("Enter selector query:");
                string selectorQuery = Console.ReadLine();
                Selector selector = Selector.ConvertToSelector(selectorQuery);

                // Get elements matching the selector
                var res = root.GetElementsBySelector(selector);

                // Use HashSet to remove duplicates and print the results
                var set = new HashSet<HTMLElement>(res);
                set.ToList().ForEach(r => Console.WriteLine("Tag name " + r.TagName));
                }
            }
            else
            {
                Console.WriteLine("HtmlHelper instance is not initialized.");
            }
        }

        // Function to load HTML content from a given URL
        static async Task<string> Load(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                var html = await response.Content.ReadAsStringAsync();

                // Use regular expression to format HTML tags
                html = Regex.Replace(html, "(<[^>]*>)", m => "\n" + m.Value.Trim() + "\n");

                return html;
            }
        }

        // Function to print the HTML element tree with indentation
        //static void PrintTree(HTMLElement root, string indent)
        //{
        //    Console.WriteLine();

        //    Console.WriteLine(indent + "Id: " + root.Id);

        //    Console.WriteLine(indent + "Tag name: " + root.TagName);

        //    Console.WriteLine(indent + "Attributes:");
        //    if (root.Attributes != null)
        //    {
        //        foreach (var attribute in root.Attributes)
        //        {
        //            Console.WriteLine(indent + "* " + attribute);
        //        }
        //    }
        //    Console.WriteLine(indent + "Classes:");
        //    if (root.Classes != null)
        //    {
        //        foreach (var className in root.Classes)
        //        {
        //            Console.WriteLine(indent + "* " + className);
        //        }
        //    }

        //    Console.WriteLine(indent + "Inner HTML: " + root.InnerHTML);

        //    Console.WriteLine(indent + "Parent: " + (root.Parent != null ? root.Parent.TagName : "None"));

        //    Console.WriteLine(indent + "Children:");
        //    foreach (var child in root.Children)
        //    {
        //        PrintTree(child, indent + "  ");
        //    }
        //}

        // Function to print the selector tree
        static void PrintTree2(Selector root)
        {
            Selector current = root;
            while (current != null)
            {
                Console.WriteLine("TagName: " + current.TagName);
                Console.WriteLine("Id: " + current.Id);
                Console.WriteLine("Classes:");
                foreach (var className in current.Classes)
                {
                    Console.WriteLine("- " + className);
                }
                Console.WriteLine("Parent: " + (current.Parent != null ? current.Parent.TagName : "None"));
                Console.WriteLine("Child: " + (current.Child != null ? current.Child.TagName : "None"));

                current = current.Child;
            }
        }
    }
}
