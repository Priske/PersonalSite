import { apiRequest, apiRequestWithoutResponse } from "../api";
import type { AskQuestionRequest, AskQuestionResponse } from "./types";

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
