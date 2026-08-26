import { apiRequest, apiRequestWithoutResponse } from "../api";
import type {
  AskQuestionRequest,
  AskQuestionResponse,
  AssistantChatAnalyticsResponse,
  GetAssistantChatAnalyticsRequest,
} from "./types";

export function uploadAssistantKnowledge(file: File) {
  const body = new FormData();

  body.append("file", file);

  return apiRequestWithoutResponse("/file/assistantknowledge", {
    method: "POST",
    body,
  });
}

export function askQuestion(request: AskQuestionRequest) {
  return apiRequest<AskQuestionResponse>("/assistant/ask", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function getAssistantChatAnalytics(
  request: GetAssistantChatAnalyticsRequest,
) {
  const parameters = new URLSearchParams();

  if (request.userId !== undefined) {
    parameters.set("userId", request.userId.toString());
  }

  if (request.search) {
    parameters.set("search", request.search);
  }

  if (request.from) {
    parameters.set("from", request.from);
  }

  if (request.to) {
    parameters.set("to", request.to);
  }

  if (request.sortBy) {
    parameters.set("sortBy", request.sortBy);
  }

  if (request.descending !== undefined) {
    parameters.set("descending", request.descending.toString());
  }

  if (request.page !== undefined) {
    parameters.set("page", request.page.toString());
  }

  if (request.pageSize !== undefined) {
    parameters.set("pageSize", request.pageSize.toString());
  }

  const queryString = parameters.toString();

  return apiRequest<AssistantChatAnalyticsResponse>(
    `/analytics/assistant-chat${queryString ? `?${queryString}` : ""}`,
  );
}
