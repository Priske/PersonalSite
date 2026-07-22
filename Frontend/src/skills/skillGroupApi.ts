import { apiRequest } from "../api";
import type {
  GetSkillGroupSummariesResponse,
  GetSkillSummariesResponse,
} from "./types";

export function getSkillGroups() {
  return apiRequest<GetSkillGroupSummariesResponse>(
    "/skill-groups",
  );
}

export function getSkills(groupId: number) {
  return apiRequest<GetSkillSummariesResponse>(
    `/skill-groups/${groupId}/skills`,
  );
}