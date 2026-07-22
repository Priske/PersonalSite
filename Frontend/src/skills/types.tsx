
export type SkillGroupSummary = {
  id: number;
  name: string;
  displayOrder: number;
};

export type GetSkillGroupSummariesResponse = {
  items: SkillGroupSummary[];
};

export type SkillSummary = {
  id: number;
  name: string;
  displayOrder: number;
};

export type GetSkillSummariesResponse = {
  items: SkillSummary[];
};