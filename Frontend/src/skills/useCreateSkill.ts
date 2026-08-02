import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequest } from "../api";

export type CreateSkillRequest = {
  name: string;
  displayOrder: number;
};

export type CreatedSkill = {
  id: number;
  name: string;
  displayOrder: number;
};

export function useCreateSkill(groupId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateSkillRequest) =>
      apiRequest<CreatedSkill>(`/skill-groups/${groupId}/skills`, {
        method: "POST",
        body: JSON.stringify(request),
      }),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["skill-groups", groupId, "skills"],
      });
    },
  });
}
