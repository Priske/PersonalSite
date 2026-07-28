export type SkillSummary = {
  id: number;
  name: string;
  displayOrder: number;
};

export type SkillGroupSummary = {
  id: number;
  name: string;
  displayOrder: number;
};

export type GetSkillGroupSummariesResponse = {
  items: SkillGroupSummary[];
};

export type GetSkillSummariesResponse = {
  items: SkillSummary[];
};

export type SkillGroupDetails = {
  id: number;
  name: string;
  displayOrder: number;
  skills: SkillSummary[];
};

export type SkillDetails = {
  id: number;
  skillGroupId: number;
  name: string;
  displayOrder: number;
};

export type CreateSkillGroupRequest = {
  name: string;
  displayOrder: number;
};

export type UpdateSkillGroupRequest = {
  name: string;
  displayOrder: number;
};

export type UpdateSkillGroupOrderRequest = {
  skillGroupIds: number[];
};

export type CreateSkillRequest = {
  name: string;
  skillGroupId: number;
  displayOrder: number;
};

export type UpdateSkillRequest = {
  name: string;
  skillGroupId: number;
  displayOrder: number;
};

export type UpdateSkillOrderRequest = {
  skillIds: number[];
};
