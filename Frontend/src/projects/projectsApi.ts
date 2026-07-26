import { apiRequest } from "../api";
import type { GetProjectSummariesResponse } from "./types";

export function getProjects() {
  return apiRequest<GetProjectSummariesResponse>("/projects");
}