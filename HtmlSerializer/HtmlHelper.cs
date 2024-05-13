using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    // Represents a helper class for HTML-related operations
    internal class HtmlHelper
    {
        // Singleton instance of the HtmlHelper class
        private readonly static HtmlHelper _instance = new HtmlHelper();

        // Public property to access the singleton instance
        public static HtmlHelper Instance => _instance;

        // Array containing standard HTML tags
        public string[] HtmlTags { get; private set; }

        // Array containing self-closing (void) HTML tags
        public string[] HtmlVoidTags { get; private set; }

        // Private constructor initializes HTML tags from JSON files
        private HtmlHelper()
        {
            try
            {
                // Read HTML tags context from JSON file
                string HTMLTagsContextJSON = File.ReadAllText("./HtmlTags.JSON");

                // Read HTML void tags context from JSON file
                string HTMLVoidTagsContextJSON = File.ReadAllText("./HtmlVoidTags.JSON");

                // Deserialize JSON context to arrays
                HtmlTags = JsonSerializer.Deserialize<string[]>(HTMLTagsContextJSON);
                HtmlVoidTags = JsonSerializer.Deserialize<string[]>(HTMLVoidTagsContextJSON);
            }
            catch (Exception ex)
            {
                // Handle exception if an error occurs while loading HTML tags
                Console.WriteLine("An error occurred while loading HTML tags: " + ex.Message);

                // Initialize with empty arrays in case of an exception
                HtmlTags = Array.Empty<string>();
                HtmlVoidTags = Array.Empty<string>();
            }
        }
    }
}
