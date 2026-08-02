import { apiRequest, apiRequestWithoutResponse } from "../api";
import type { PagedResult } from "../types";
import {
  type CreateTagRequest,
  type CreateTagResponse,
  type GetTagsRequest,
  type TagDetails,
  type TagSummary,
  type UpdateTagRequest,
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

export function getTag(id: number) {
  return apiRequest<TagDetails>(`/tags/${id}`);
}

export function updateTag(tagId: number, request: UpdateTagRequest) {
  return apiRequestWithoutResponse(`/tags/${tagId}`, {
    method: "PUT",
    body: JSON.stringify(request),
  });
}

export function deleteTag(tagId: number) {
  return apiRequestWithoutResponse(`/tags/${tagId}`, {
    method: "DELETE",
  });
}
