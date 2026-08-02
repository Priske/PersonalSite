import { useQuery } from "@tanstack/react-query";
import { apiRequest } from "../api";
import type { SkillGroupSummary } from "./types";

export function useSkillGroup(groupId: number, enabled = true) {
  return useQuery({
    queryKey: ["skill-groups", groupId],

    queryFn: () => apiRequest<SkillGroupSummary>(`/skill-groups/${groupId}`),

    enabled,
  });
}
