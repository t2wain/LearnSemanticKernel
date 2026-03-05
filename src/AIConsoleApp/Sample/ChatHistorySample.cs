using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIConsoleApp.Sample
{
    /// <summary>
    /// Document many different method and data structure
    /// to construct chat messges. The ChatHistory maintain
    /// the window context of the entire chat session to be
    /// sent back to the LLNM on each request.
    /// </summary>
    public static class ChatHistorySample
    {
        /// <summary>
        /// Various type of message contruction
        /// </summary>
        public static ChatHistory EX1()
        {
            var chatHistory = new ChatHistory()
            {
                // Add system message
                new()
                {
                    Role = AuthorRole.System,
                    Content = "You are a helpful assistant" // sinple message content
                }, // ChatMessageContent

                // Add user message with an image
                new()
                {
                    Role = AuthorRole.User,
                    AuthorName = "Laimonis Dumins",
                    Items = [
                        // message consists of different type of contents
                        new TextContent { Text = "What available on this menu" }, // KernelContent
                        new ImageContent { Uri = new Uri("https://example.com/menu.jpg") } // KernelContent
                    ] // ChatMessageContentItemCollection
                },

                // Add assistant message
                new()
                {
                    Role = AuthorRole.Assistant,
                    AuthorName = "Restaurant Assistant",
                    Content = "We have pizza, pasta, and salad available to order. What would you like to order ? "
                }, // ChatMessageContent

                // Add additional message from a different user
                new()
                {
                    Role = AuthorRole.User,
                    AuthorName = "Ema Vargova",
                    Content = "I'd like to have the first option, please."
                } // ChatMessageContent

            };

            // Load an image from disk.
            byte[] bytes = File.ReadAllBytes("path/to/image.jpg");

            // Add a user message with both the image and a question
            // about the image.
            chatHistory.AddUserMessage(
                [
                    new TextContent("What’s in this image?"),
                    new ImageContent(bytes, "image/jpeg"), // binary content
                ]
            );

            return chatHistory;
        }

        /// <summary>
        /// How toolcall messages are returned and
        /// how result messages should be sent back
        /// </summary>
        /// <returns></returns>
        public static ChatHistory EX2()
        {
            var chatHistory = new ChatHistory();

            // Add a simulated function call from the assistant
            chatHistory.Add(
                new()
                {
                    Role = AuthorRole.Assistant,
                    // Toolcall message return from LLM
                    // multiple toolcall messages can be returned in a single response
                    Items = [
                        new FunctionCallContent(
                            functionName: "get_user_allergies",
                            pluginName: "User",
                            id: "0001",
                            arguments: new () { {"username", "laimonisdumins"} }
                        ),
                        new FunctionCallContent(
                            functionName: "get_user_allergies",
                            pluginName: "User",
                            id: "0002",
                            arguments: new () { {"username", "emavargova"} }
                        )
                    ]
                }
            );

            // how to construct a message of toolcall ressult
            // to be sent back to the LLM
            chatHistory.Add(
                new()
                {
                    Role = AuthorRole.Tool,
                    Items = [
                        new FunctionResultContent(
                            functionName: "get_user_allergies",
                            pluginName: "User",
                            callId : "0001", // reference the toolcall funtion
                            result: "{ \"allergies\": [\"peanuts\", \"gluten\"] }"
                        )
                    ]
                }
            );

            chatHistory.Add(
                new()
                {
                    Role = AuthorRole.Tool,
                    Items = [
                        new FunctionResultContent(
                            functionName: "get_user_allergies",
                            pluginName: "User",
                            callId: "0002", // reference the toolcall funtion
                            result: "{ \"allergies\": [\"dairy\", \"soy\"] }"
                        )
                    ]
                }
            );

            return chatHistory;
        }
    }
}
