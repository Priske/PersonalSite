import { apiRequest, apiRequestWithoutResponse } from "../api";
import type {CreateProjectRequest,GetProjectSummariesResponse,ProjectDetails, UpdateProjectRequest } from "./types";

export function getProjects() {
  return apiRequest<GetProjectSummariesResponse>("/projects");
}

export function getProject(projectId: number) {
  return apiRequest<ProjectDetails>(`/projects/${projectId}`);
}

export function createProject(request: CreateProjectRequest) {
  return apiRequestWithoutResponse("/projects", {
    method: "POST",
    body: JSON.stringify(request),
  });
}

export function updateProject(projectId: number, request: UpdateProjectRequest) {
  return apiRequestWithoutResponse(`/projects/${projectId}`, {
    method: "PUT",
    body: JSON.stringify(request),
  });
}

export function deleteProject(projectId: number) {
  return apiRequestWithoutResponse(`/projects/${projectId}`, {
    method: "DELETE",
  });
}