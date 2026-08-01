import type { TagSummary } from "../tags/types";

export type ProjectSummary = {
  id: number;
  title: string;
  description: string;
  repositoryUrl: string;
  liveUrl?: string;
  isFeatured: boolean;
  displayOrder: number;
  tags: string[];
};

export type ProjectDetails = {
  id: number;
  title: string;
  description: string;
  repositoryUrl: string;
  liveUrl?: string;
  isFeatured: boolean;
  displayOrder: number;
  tags: TagSummary[];
};

export type GetProjectSummariesResponse = {
  items: ProjectSummary[];
};

export type CreateProjectRequest = {
  title: string;
  description: string;
  repositoryUrl: string;
  liveUrl?: string;
  isFeatured: boolean;
  displayOrder: number;
  tagIds: number[];
};

export type UpdateProjectRequest = {
  title: string;
  description: string;
  repositoryUrl: string;
  liveUrl?: string;
  isFeatured: boolean;
  tagIds: number[];
};
