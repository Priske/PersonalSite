export type GetTagsRequest = {
  page: number;
  pageSize: number;
  search?: string;
};

export type TagDetails = {
  id: number;
  name: string;
};

export type TagSummary = {
  id: number;
  name: string;
};

export type CreateTagRequest = { name: string };

export type CreateTagResponse = {
  id: number;
  name: string;
};
