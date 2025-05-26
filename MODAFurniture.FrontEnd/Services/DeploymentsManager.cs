using Azure.AI.OpenAI;
using Azure;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MODAFurniture.FrontEnd.Services;

public class DeploymentsManager
{
    private readonly IConfiguration _configuration;
    private List<string> _deploymentNames = new();
    private IDictionary<string, IChatClient> _clients = null;

    public IReadOnlyList<string> DeploymentNames => _deploymentNames.AsReadOnly();

    public DeploymentsManager(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        LoadDeploymentNames();
    }

    private void LoadDeploymentNames()
    {
        string modelsConfig = _configuration["Deployments"];
        if (!string.IsNullOrEmpty(modelsConfig))
        {
            _deploymentNames = modelsConfig.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }

    public void CreateClients()
    {
        var key = _configuration["AzureOpenAiKey"];
        var endpoint = _configuration["AzureOpenAiEndpoint"];

        _clients = new Dictionary<string, IChatClient>();

        foreach (var deploymentName in _deploymentNames)
        {
            var azureOpenAIClient = new AzureOpenAIClient(
                    new Uri(endpoint),
                    new AzureKeyCredential(key));
            _clients.Add(deploymentName, azureOpenAIClient.AsChatClient(deploymentName));
        }
    }

    public IChatClient GetClient(string deploymentName)
    {
        if (_clients.TryGetValue(deploymentName, out var client))
        {
            return client;
        }
        throw new KeyNotFoundException($"Deployment '{deploymentName}' not found.");
    }
}
