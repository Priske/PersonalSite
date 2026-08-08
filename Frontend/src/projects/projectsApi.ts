import { apiRequest, apiRequestWithoutResponse } from "../api";
import type {
  CreateProjectRequest,
  GetProjectSummariesResponse,
  ProjectDetails,
  UpdateProjectRequest,
} from "./types";

type GetProjectsRequest = {
  page?: number;
  pageSize?: number;
  search?: string;
};

function buildProjectQuery(request: GetProjectsRequest = {}) {
  const parameters = new URLSearchParams();

  if (request.page !== undefined) {
    parameters.set("page", request.page.toString());
  }

  if (request.pageSize !== undefined) {
    parameters.set("pageSize", request.pageSize.toString());
  }

  if (request.search) {
    parameters.set("search", request.search);
  }

  const query = parameters.toString();

  return query ? `?${query}` : "";
}

export function getProjects(request: GetProjectsRequest = {}) {
  return apiRequest<GetProjectSummariesResponse>(
    `/projects${buildProjectQuery(request)}`,
  );
}

export function getDemoProjects(request: GetProjectsRequest = {}) {
  return apiRequest<GetProjectSummariesResponse>(
    `/demo-projects${buildProjectQuery(request)}`,
  );
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

export function updateProject(
  projectId: number,
  request: UpdateProjectRequest,
) {
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
