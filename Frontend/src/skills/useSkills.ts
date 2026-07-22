import { useQuery } from "@tanstack/react-query";
import { apiRequest } from "../api";
import type { GetSkillSummariesResponse } from "./types";

export function useSkills(
  groupId: number,
  enabled = true,
) {
  return useQuery({
    queryKey: [
      "skill-groups",
      groupId,
      "skills",
    ],

    queryFn: () =>
      apiRequest<GetSkillSummariesResponse>(
        `/skill-groups/${groupId}/skills`,
      ),

    enabled,
  });
}