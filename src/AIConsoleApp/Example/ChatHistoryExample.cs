using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIConsoleApp.Example
{
    public static class ChatHistoryExample
    {
        public static ChatHistory EX1()
        {
            var chatHistory = new ChatHistory()
            {
                // Add system message
                new()
                {
                    Role = AuthorRole.System,
                    Content = "You are a helpful assistant"
                }, // ChatMessageContent

                // Add user message with an image
                new()
                {
                    Role = AuthorRole.User,
                    AuthorName = "Laimonis Dumins",
                    Items = [
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
                    new ImageContent(bytes, "image/jpeg"),
                ]
            );

            return chatHistory;
        }

        public static ChatHistory EX2()
        {
            var chatHistory = new ChatHistory();

            // Add a simulated function call from the assistant
            chatHistory.Add(
                new()
                {
                    Role = AuthorRole.Assistant,
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

            // Add a simulated function results from the tool role
            chatHistory.Add(
                new()
                {
                    Role = AuthorRole.Tool,
                    Items = [
                        new FunctionResultContent(
                            functionName: "get_user_allergies",
                            pluginName: "User",
                            callId : "0001",
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
                            callId: "0002",
                            result: "{ \"allergies\": [\"dairy\", \"soy\"] }"
                        )
                    ]
                }
            );

            return chatHistory;
        }
    }
}
