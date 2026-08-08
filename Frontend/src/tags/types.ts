export type GetTagsRequest = {
  page: number;
  pageSize: number;
  search?: string;
};

export type TagProject = {
  id: number;
  title: string;
};

export type TagSummary = {
  id: number;
  name: string;
  source: string;
  createdByUserId: number | null;
  createdAt: string;
  lastEditedByUserId: number | null;
  lastEditedAt: string;
};

export type TagDetails = TagSummary & {
  projects: TagProject[];
};

export type CreateTagRequest = {
  name: string;
};

export type CreateTagResponse = TagSummary;

export type UpdateTagRequest = {
  id: number;
  name: string;
};
