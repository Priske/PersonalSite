import { apiRequest, apiRequestWithoutResponse } from "../api";
import type {
  GetHomePageConfigDetailsResponse,
  UpdateHomePageConfigRequest,
} from "./types";

export function getOfficialHomePageConfig() {
  return apiRequest<GetHomePageConfigDetailsResponse>(
    "/home-official-page-config",
  );
}

export function getDemoHomePageConfig() {
  return apiRequest<GetHomePageConfigDetailsResponse>("/home-demo-page-config");
}

export function updateOfficialHomePageConfig(
  request: UpdateHomePageConfigRequest,
) {
  return apiRequestWithoutResponse("/home-official-page-config", {
    method: "PUT",
    body: JSON.stringify(request),
  });
}

export function updateDemoHomePageConfig(request: UpdateHomePageConfigRequest) {
  return apiRequestWithoutResponse("/home-demo-page-config", {
    method: "PUT",
    body: JSON.stringify(request),
  });
}
