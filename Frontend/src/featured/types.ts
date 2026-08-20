export type FeaturedContentFile = {
  id: number;
  originalFileName: string;
  contentType: string;
  sizeInBytes: number;
};

export type FeaturedContent = {
  id: number;
  title: string;
  description: string;
  files: FeaturedContentFile[];
  tags: string[];
};

export type GetFeaturedContentResponse = {
  items: FeaturedContent[];
};

export type FeaturedContentTag = {
  id: number;
  name: string;
};

export type FeaturedContentDetails = Omit<FeaturedContent, "tags"> & {
  tags: FeaturedContentTag[];
};

export type CreateFeaturedContentRequest = {
  title: string;
  description: string;
  tagIds: number[];
};

export type CreateFeaturedContentResponse = {
  id: number;
  title: string;
  description: string;
  tags: string[];
};

export type UpdateFeaturedContentRequest = CreateFeaturedContentRequest;
