import { useQuery } from "@tanstack/react-query";
import { getSkillGroups } from "./skillGroupApi";

export function useSkillGroups() {
  return useQuery({
    queryKey: ["skill-groups"],
    queryFn: getSkillGroups,
  });
}