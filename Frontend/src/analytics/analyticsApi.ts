import { apiRequest, apiRequestWithoutResponse } from "../api";

import type {
  GetLoginAnalyticsRequest,
  LoginAnalyticsResponse,
  TrackActivityRequest,
} from "./types";

export function trackActivity(request: TrackActivityRequest) {
  return apiRequestWithoutResponse("/analytics", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function getLoginAnalytics(request: GetLoginAnalyticsRequest) {
  const searchParams = new URLSearchParams();

  if (request.userId !== undefined) {
    searchParams.set("userId", request.userId.toString());
  }

  if (request.search) {
    searchParams.set("search", request.search);
  }

  if (request.successful !== undefined) {
    searchParams.set("successful", request.successful.toString());
  }

  if (request.from) {
    searchParams.set("from", request.from);
  }

  if (request.to) {
    searchParams.set("to", request.to);
  }

  if (request.sortBy) {
    searchParams.set("sortBy", request.sortBy);
  }

  if (request.descending !== undefined) {
    searchParams.set("descending", request.descending.toString());
  }

  if (request.page !== undefined) {
    searchParams.set("page", request.page.toString());
  }

  if (request.pageSize !== undefined) {
    searchParams.set("pageSize", request.pageSize.toString());
  }

  return apiRequest<LoginAnalyticsResponse>(
    `/analytics/login?${searchParams.toString()}`,
  );
}
