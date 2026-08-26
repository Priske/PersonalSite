import { apiRequestWithoutResponse } from "../api";

export function uploadAssistantKnowledge(file: File) {
  const body = new FormData();
  body.append("file", file);

  return apiRequestWithoutResponse("/file/assistantknowledge", {
    method: "POST",
    body,
  });
}
