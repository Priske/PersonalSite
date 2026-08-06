import { apiRequest, apiRequestWithoutResponse } from "../api";
import type {
  GetHomePageConfigDetailsResponse,
  UpdateHomePageConfigRequest,
} from "./types";

export function getHomePageConfig() {
  return apiRequest<GetHomePageConfigDetailsResponse>("/home-page-config");
}

export function updateHomePageConfig(request: UpdateHomePageConfigRequest) {
  return apiRequestWithoutResponse("/home-page-config", {
    method: "PUT",
    body: JSON.stringify(request),
  });
}
