using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.AI;
using MODAFurniture.FrontEnd.Services;
using System.Text.Json;

namespace MODAFurniture.FrontEnd.Pages
{
    public class ChatUsModel : PageModel
    {
        private const string SessionKeyMessages = "_Messages";

        private const string SystemMessage = "You are an helpful assistant that help people to know more about M.O.D.A. and its products.";

        public readonly DeploymentsManager _deploymentsManager;

        [BindProperty]
        public List<ChatMessage> MessageHistory { get; private set; } 
        [BindProperty]
        public IEnumerable<string> DeploymentNames { get; private set; }
        [BindProperty]
        public string SelectedDeployment { get; set; }

        [BindProperty]
        public string Message { get; set; }

        [BindProperty]
        public string ChatId { get; set; }

        public ChatUsModel(DeploymentsManager deploymentsManager)
        {
            _deploymentsManager = deploymentsManager;
            DeploymentNames = _deploymentsManager.DeploymentNames;
            MessageHistory = new List<ChatMessage>();
        }

        public void OnGet()
        {
            if (HttpContext.Request.Query.ContainsKey("chatId"))
            {
                ChatId = HttpContext.Request.Query["chatId"];
                var messagesJson = HttpContext.Session.GetString(GetSessionKeyMessages());
                if (!string.IsNullOrEmpty(messagesJson))
                {
                    MessageHistory = JsonSerializer.Deserialize<List<ChatMessage>>(messagesJson);
                }
                else
                {
                    ResetChat();
                }
            }
            else
            {
                ResetChat();
            }
        }

        private string GetSessionKeyMessages()
        {
            return $"{SessionKeyMessages}_{ChatId}";
        }

        private void ResetChat()
        {
            ChatId = Guid.NewGuid().ToString();
            MessageHistory = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System, SystemMessage)
            };

            // Save the reset chat to session
            SaveMessageHistoryToSession();
        }

        private void SaveMessageHistoryToSession()
        {
            var messagesJson = JsonSerializer.Serialize(MessageHistory);
            HttpContext.Session.SetString(GetSessionKeyMessages(), messagesJson);
        }

        private void LoadMessageHistoryFromSession()
        {
            // Load the current message history from session
            var messagesJson = HttpContext.Session.GetString(GetSessionKeyMessages());
            if (!string.IsNullOrEmpty(messagesJson))
            {
                MessageHistory = JsonSerializer.Deserialize<List<ChatMessage>>(messagesJson);
            }
            else
            {
                MessageHistory = new List<ChatMessage>
                {
                    new ChatMessage(ChatRole.System, SystemMessage)
                };
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            LoadMessageHistoryFromSession();

            var action = Request.Form["action"];

            switch (action)
            {
                case "newChat":
                    ResetChat();
                    break;
                case "sendMessage":

                    SelectedDeployment = Request.Form["selectedDeployment"];

                    var chatClient = _deploymentsManager.GetClient(SelectedDeployment);

                    if (!string.IsNullOrEmpty(Message))
                    {
                        if (MessageHistory == null)
                        {
                            MessageHistory = new List<ChatMessage>();
                        }
                        MessageHistory.Add(new ChatMessage(ChatRole.User, Message));

                        var response = await chatClient.GetResponseAsync(MessageHistory);

                        MessageHistory.Add(new ChatMessage(ChatRole.Assistant, response.Text));

                        SaveMessageHistoryToSession(); 
                    }
                    break;
                default:
                    break;
            }

            return Page();
        }
    }
}
