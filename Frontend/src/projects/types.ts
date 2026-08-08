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
  source: string;
  createdByUserId: number | null;
  createdAt: string;
  lastEditedByUserId: number | null;
  lastEditedAt: string;
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
  source: string;
  createdByUserId: number | null;
  createdAt: string;
  lastEditedByUserId: number | null;
  lastEditedAt: string;
};

export type GetProjectSummariesResponse = {
  items: ProjectSummary[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
};

export type CreateProjectRequest = {
  title: string;
  description: string;
  repositoryUrl: string;
  liveUrl?: string;
  isFeatured: boolean;
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
