import { apiRequest, apiRequestWithoutResponse } from "../api";

import type {
  ContactLinkAnalyticsRequest,
  ContactLinkAnalyticsResponse,
  CreateUserAnalyticsResponse,
  DeleteUserAnalyticsResponse,
  GetCreateUserAnalyticsRequest,
  GetDeleteUserAnalyticsRequest,
  GetLoginAnalyticsRequest,
  LoginAnalyticsResponse,
  ReferrerActivityRequest,
  ReferrerAnalyticsResponse,
  TrackActivityRequest,
  VideoAnalyticsRequest,
  VideoAnalyticsResponse,
} from "./types";

type AnalyticsFilterRequest = {
  search?: string;
  from?: string;
  to?: string;
  sortBy?: string;
  descending?: boolean;
};

type PagingRequest = {
  page?: number;
  pageSize?: number;
};

function addAnalyticsFilters(
  searchParams: URLSearchParams,
  request: AnalyticsFilterRequest,
) {
  if (request.search) {
    searchParams.set("search", request.search);
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
}

function addPaging(searchParams: URLSearchParams, request: PagingRequest) {
  if (request.page !== undefined) {
    searchParams.set("page", request.page.toString());
  }

  if (request.pageSize !== undefined) {
    searchParams.set("pageSize", request.pageSize.toString());
  }
}

export function getReferrerActivity(request: ReferrerActivityRequest) {
  const searchParams = new URLSearchParams();

  addAnalyticsFilters(searchParams, request);

  return apiRequest<ReferrerAnalyticsResponse>(
    `/analytics/referrers?${searchParams.toString()}`,
  );
}

export function getContactLinkAnalytics(
  request: ContactLinkAnalyticsRequest,
) {
  const searchParams = new URLSearchParams();

  addAnalyticsFilters(searchParams, request);

  return apiRequest<ContactLinkAnalyticsResponse>(
    `/analytics/contact-links?${searchParams.toString()}`,
  );
}

export function getVideoAnalytics(request: VideoAnalyticsRequest) {
  const searchParams = new URLSearchParams();

  addAnalyticsFilters(searchParams, request);

  return apiRequest<VideoAnalyticsResponse>(
    `/analytics/videos?${searchParams.toString()}`,
  );
}

export function trackActivity(request: TrackActivityRequest) {
  return apiRequestWithoutResponse("/analytics", {
    method: "POST",
    body: JSON.stringify(request),
    keepalive: true,
  });
}

export function getLoginAnalytics(request: GetLoginAnalyticsRequest) {
  const searchParams = new URLSearchParams();

  addAnalyticsFilters(searchParams, request);
  addPaging(searchParams, request);

  if (request.userId !== undefined) {
    searchParams.set("userId", request.userId.toString());
  }

  if (request.successful !== undefined) {
    searchParams.set("successful", request.successful.toString());
  }

  return apiRequest<LoginAnalyticsResponse>(
    `/analytics/login?${searchParams.toString()}`,
  );
}

export function getCreateUserAnalytics(request: GetCreateUserAnalyticsRequest) {
  const searchParams = new URLSearchParams();

  addAnalyticsFilters(searchParams, request);

  addPaging(searchParams, request);

  return apiRequest<CreateUserAnalyticsResponse>(
    `/analytics/create-users?${searchParams.toString()}`,
  );
}

export function getDeleteUserAnalytics(request: GetDeleteUserAnalyticsRequest) {
  const searchParams = new URLSearchParams();

  addAnalyticsFilters(searchParams, request);

  addPaging(searchParams, request);

  if (request.userId !== undefined) {
    searchParams.set("userId", request.userId.toString());
  }

  if (request.successful !== undefined) {
    searchParams.set("successful", request.successful.toString());
  }

  return apiRequest<DeleteUserAnalyticsResponse>(
    `/analytics/delete-users?${searchParams.toString()}`,
  );
}
