import { apiRequest } from "../api";
import type { CurrentUser, LoginRequest, LoginResponse } from "./types";

export function login(request: LoginRequest) {
  return apiRequest<LoginResponse>("/auth/login", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function getCurrentUser() {
  return apiRequest<CurrentUser>("/auth/me");
}
