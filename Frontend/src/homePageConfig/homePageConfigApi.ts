import { apiRequest } from "../api";
import type {
  GetHomePageConfigDetailsResponse,
  UpdateHomePageConfigRequest,
} from "./types";

export function getHomePageConfig() {
  return apiRequest<GetHomePageConfigDetailsResponse>("/home-page-config" );
}

export function updateHomePageConfig(
  request: UpdateHomePageConfigRequest,
) {
  return apiRequest<void>("/home-page-config", {
    method: "PUT",
    body: JSON.stringify(request),
  });
}