import { apiRequest, apiRequestWithoutResponse } from "../api";
import type {
  CreateFeaturedContentRequest,
  CreateFeaturedContentResponse,
  FeaturedContentDetails,
  FeaturedContentFile,
  GetFeaturedContentResponse,
  UpdateFeaturedContentRequest,
} from "./types";

export function getFeaturedContent() {
  return apiRequest<GetFeaturedContentResponse>("/featured-content");
}

export function getFeaturedContentDetails(id: number) {
  return apiRequest<FeaturedContentDetails>(`/featured-content/${id}`);
}

export function createFeaturedContent(request: CreateFeaturedContentRequest) {
  return apiRequest<CreateFeaturedContentResponse>("/featured-content", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function updateFeaturedContent(
  id: number,
  request: UpdateFeaturedContentRequest,
) {
  return apiRequestWithoutResponse(`/featured-content/${id}`, {
    method: "PUT",
    body: JSON.stringify(request),
  });
}

export function uploadFeaturedContentFile(id: number, file: File) {
  const body = new FormData();
  body.append("file", file);

  return apiRequest<FeaturedContentFile>(`/featured-content/${id}/files`, {
    method: "POST",
    body,
  });
}

export function removeFeaturedContentFile(id: number, fileId: number) {
  return apiRequestWithoutResponse(
    `/featured-content/${id}/files/${fileId}`,
    { method: "DELETE" },
  );
}
