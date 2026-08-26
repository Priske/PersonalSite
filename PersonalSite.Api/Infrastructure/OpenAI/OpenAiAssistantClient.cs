using System.ClientModel;
using System.Text.Json;
using OpenAI.Responses;
using PersonalSite.Api.Domain.Exceptions.Assistant;

namespace PersonalSite.Api.Infrastructure.OpenAI;

#pragma warning disable OPENAI001

public sealed class OpenAiAssistantClient(
    ResponsesClient responsesClient,
    OpenAiSettings settings)
{
    private const string GetProjectsToolName = "get_projects";

    private static readonly FunctionTool GetProjectsTool =
        ResponseTool.CreateFunctionTool(
            functionName: GetProjectsToolName,
            functionDescription:
                "Search and retrieve official projects from the portfolio.",
            functionParameters: BinaryData.FromBytes(
                """
                {
                  "type": "object",
                  "properties": {
                    "search": {
                      "type": "string",
                      "description": "Optional text used to search the project title, description or repository URL."
                    }
                  }
                }
                """u8.ToArray()),
            strictModeEnabled: false);

    public async Task<string> AskAsync(
        string question,
        Func<string?, CancellationToken, Task<string>> getProjects,
        CancellationToken cancellationToken)
    {
        try
        {
            List<ResponseItem> inputItems =
            [
                ResponseItem.CreateUserMessageItem(question)
            ];

            for (var step = 0; step < 3; step++)
            {
                var options = new CreateResponseOptions(
                    settings.Model,
                    inputItems)
                {
                    Instructions =
                        """
                        You are the assistant for this portfolio website.

                        Answer questions about the portfolio owner and their work.
                        Use get_projects when project information is needed.
                        Do not invent project details.
                        If the available data does not answer the question, say so.
                        """,
                    ParallelToolCallsEnabled = false
                };

                options.Tools.Add(GetProjectsTool);

                ResponseResult response =
                    await responsesClient.CreateResponseAsync(
                        options,
                        cancellationToken);

                inputItems.AddRange(response.OutputItems);

                var functionCall = response.OutputItems
                    .OfType<FunctionCallResponseItem>()
                    .FirstOrDefault();

                if (functionCall is null)
                {
                    return response.GetOutputText();
                }

                if (functionCall.FunctionName != GetProjectsToolName)
                {
                    throw new InvalidOperationException(
                        $"Unknown assistant tool: {functionCall.FunctionName}");
                }

                var search = ReadSearch(functionCall.FunctionArguments);

                var functionOutput = await getProjects(
                    search,
                    cancellationToken);

                inputItems.Add(
                    new FunctionCallOutputResponseItem(
                        functionCall.CallId,
                        functionOutput));
            }

            throw new AssistantUnavailableException(
                "The assistant exceeded its allowed tool steps.",
                new InvalidOperationException(
                    "The assistant did not produce a final answer."));
        }
        catch (ClientResultException exception)
            when (exception.Status == 429)
        {
            throw new AssistantUnavailableException(
                "The assistant has no available OpenAI capacity.",
                exception);
        }
    }

    private static string? ReadSearch(BinaryData arguments)
    {
        using var json = JsonDocument.Parse(arguments.ToMemory());

        return json.RootElement.TryGetProperty(
                   "search",
                   out var search)
               && search.ValueKind == JsonValueKind.String
            ? search.GetString()
            : null;
    }
}