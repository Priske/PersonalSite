import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { getAssistantChatAnalytics } from "./assistantApi";
import type { GetAssistantChatAnalyticsRequest } from "./types";

export function useAssistantChatLogs(
  request: GetAssistantChatAnalyticsRequest,
) {
  return useQuery({
    queryKey: ["analytics", "assistant-chat", request],
    queryFn: () => getAssistantChatAnalytics(request),
    placeholderData: keepPreviousData,
  });
}
