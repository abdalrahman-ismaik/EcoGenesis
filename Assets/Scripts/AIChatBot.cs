using UnityEngine;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class AIChatBot : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button sendButton;

    private OpenAIAPI api;
    private List<ChatMessage> messages;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Use environment variable for API key instead of hardcoding
        string apiKey = "put api key here";
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("OpenAI API key not found in environment variables. Please set OPENAI_API_KEY.");
            enabled = false;
            return;
        }
        api = new OpenAIAPI(apiKey);

        messages = new List<ChatMessage>
        {
            new ChatMessage(ChatMessageRole.System, @"You are ECHO, an AI companion in the environmental restoration game EcoGenesis. Here's your core knowledge:

            GAME STORY:
            - Setting: Near-future UAE where environmental challenges have reached a critical point
            - Background: Years of industrial growth led to environmental degradation affecting key regions
            - Your Role: You were created by leading UAE environmental scientists to guide restoration efforts
            - Mission: Help players restore three vital ecosystems:
            1. Mountains: Once pristine peaks now affected by waste and pollution
            2. Desert: Sacred dunes threatened by plastic waste and industrial debris
            3. City: Urban area struggling with pollution and waste management

            GAME CONTEXT:
            - Each area has 15 pieces of pollution to collect
            - Players must clean and restore each zone to progress
            - You serve as both guide and environmental educator

            KNOWLEDGE BASE:
            1. UAE Environmental Facts:
            - Local ecosystem characteristics
            - Native flora and fauna
            - Climate challenges specific to the region
            - UAE Vision 2050 environmental goals

            2. Sustainability Topics:
            - Renewable energy initiatives in UAE
            - Desert conservation methods
            - Water preservation techniques
            - Waste management solutions
            - Local environmental protection laws

            3. Game Mechanics:
            - WASD: Movement controls
            - Mouse: Look around
            - F: Call for assistance
            - Click: Collect pollution items
            - ESC: Open chat interface

            Respond in character as ECHO, maintaining a helpful, educational tone while weaving in local UAE context and environmental facts. If players ask about topics outside these themes, guide them back to environmental and game-related discussions.")
        };

        sendButton.onClick.AddListener(() => GetResponse());
        textField.text = "ECHO: Hello! I'm ECHO, your AI companion in EcoGenesis. How can I assist you today?";
    }

    private async void GetResponse()
    {
        if (inputField.text == "")
            return;

        sendButton.enabled = false;
        ChatMessage userMessage = new ChatMessage(ChatMessageRole.User, inputField.text);
        messages.Add(userMessage);

        textField.text += "\nYou: " + inputField.text;
        inputField.text = "";

        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Messages = messages,
            Temperature = 0.5f,
            MaxTokens = 50
        });

        ChatMessage responseMessage = new ChatMessage(ChatMessageRole.Assistant, chatResult.Choices[0].Message.TextContent);
        messages.Add(responseMessage);

        textField.text += "You: " + userMessage.TextContent + "\n\nECHO: " + responseMessage.TextContent;
        sendButton.enabled = true;
    }
}
