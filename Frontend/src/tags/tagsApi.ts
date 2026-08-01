import { apiRequest } from "../api";
import type { PagedResult } from "../types";
import {
  type CreateTagRequest,
  type CreateTagResponse,
  type GetTagsRequest,
  type TagSummary,
} from "./types";

export function getTags(request: GetTagsRequest) {
  const parameters = new URLSearchParams({
    page: request.page.toString(),
    pageSize: request.pageSize.toString(),
  });
  if (request.search) {
    parameters.set("search", request.search);
  }

  return apiRequest<PagedResult<TagSummary>>(`/tags?${parameters.toString()}`);
}

export function createTag(request: CreateTagRequest) {
  return apiRequest<CreateTagResponse>("/tags", {
    method: "POST",
    body: JSON.stringify(request),
  });
}
