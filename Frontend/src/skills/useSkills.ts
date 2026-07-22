import { useQuery } from "@tanstack/react-query";
import { getSkills } from "./skillGroupApi";

export function useSkills(groupId: number) {
  return useQuery({
    queryKey: ["skill-groups", groupId, "skills"],
    queryFn: () => getSkills(groupId),
  });
}