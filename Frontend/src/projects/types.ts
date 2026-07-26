
export type ProjectSummary = {
  id: number;
  title: string;
  discription: string;
  repositoryUrl: string;
  liverUrl?: string;
  IsFeatured: boolean;
  displayOrder: number;
};

export type GetProjectSummariesResponse = {
  items: ProjectSummary[];
};
