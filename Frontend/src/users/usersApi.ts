import { apiRequest, apiRequestWithoutResponse } from "../api";
import type { PagedResult } from "../types";
import type {
  GetUsersRequest,
  UserDetails,
  UserSummary,
  RegisterUserRequest,
  RegisterUserResponse,
  UpdateUserRequest,
} from "./types";

export function registerUser(request: RegisterUserRequest) {
  return apiRequest<RegisterUserResponse>("/users", {
    method: "POST",
    body: JSON.stringify(request),
  });
}
export function seedFakeUsers() {
  return apiRequestWithoutResponse(`/users/fake/replenish`, {
    method: "POST",
  });
}

export function getUser(userId: number) {
  return apiRequest<UserDetails>(`/users/${userId}`);
}

export function updateUser(id: number, request: UpdateUserRequest) {
  return apiRequestWithoutResponse(`/users/${id}`, {
    method: "PUT",
    body: JSON.stringify(request),
  });
}
export function updateCurrentUser(request: UpdateUserRequest) {
  return apiRequest<void>("/auth/me", {
    method: "PUT",
    body: JSON.stringify(request),
  });
}

export function getUsers(request: GetUsersRequest) {
  const parameters = new URLSearchParams({
    page: request.page.toString(),
    pageSize: request.pageSize.toString(),
  });

  if (request.search) {
    parameters.set("search", request.search);
  }

  return apiRequest<PagedResult<UserSummary>>(
    `/users?${parameters.toString()}`,
  );
}

export function deleteUser(userId: number) {
  return apiRequestWithoutResponse(`/users/${userId}`, {
    method: "DELETE",
  });
}
