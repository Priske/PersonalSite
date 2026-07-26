
export type ProjectSummary = {
  id: number;
  title: string;
  description: string;
  repositoryUrl: string;
  liveUrl?: string;
  isFeatured: boolean;
  displayOrder: number;
};

export type GetProjectSummariesResponse = {
  items: ProjectSummary[];
};
