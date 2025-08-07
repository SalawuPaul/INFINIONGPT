using Azure.AI.OpenAI;
using INFINIONGPT.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace INFINIONGPT.Services.OpenAI
{
    public class OpenAIService : IOpenAIService
    {
        private readonly OpenAIClient _client;
        private readonly string _deploymentName;

        public OpenAIService(OpenAIClient openAIClient, IConfiguration configuration)
        {
            _client = openAIClient;
            _deploymentName = configuration.GetValue<string>("OpenAIDeploymentName");
        }
        public async Task<ChatMessages> ChatCompletionAsync(string message, List<ChatMessages> chatMessages)
        {
            ChatCompletionsOptions options = new ()
            {
                DeploymentName = _deploymentName,
                Messages =
                 {
                     new ChatRequestSystemMessage("You are an AI assistance that helps people find solutions by using the connected knowledge store"),
                     new ChatRequestSystemMessage("Be as friendly as possible"),
                     new ChatRequestUserMessage(message)
                 },
                MaxTokens = 1000,
            };
            if (chatMessages != null )
            {
                chatMessages.Reverse();
                chatMessages.AddRange(chatMessages);
                chatMessages.ForEach(c =>
                {
                    options.Messages.Add(new ChatRequestUserMessage(c.userMessage));
                    options.Messages.Add(new ChatRequestAssistantMessage(c.BotMessage));
                });
            } 

            options.Messages.Add(new ChatRequestUserMessage(message));
            var response = await _client.GetChatCompletionsAsync(options);

            return new ChatMessages() { userMessage = message, BotMessage = response.Value.Choices[0].Message.Content};
        }

        public Task<ChatMessages> ChatCompletionAsync(string text)
        {
            throw new System.NotImplementedException();
        }
    }
}
 //testing