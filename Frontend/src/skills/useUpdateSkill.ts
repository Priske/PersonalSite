import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequestWithoutResponse } from "../api";

type UpdateSkillRequest = {
  skillId: number;
  name: string;
  displayOrder: number;
};

export function useUpdateSkill(groupId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ skillId, name, displayOrder }: UpdateSkillRequest) =>
      apiRequestWithoutResponse(`/skill-groups/${groupId}/skills/${skillId}`, {
        method: "PUT",
        body: JSON.stringify({
          name,
          displayOrder,
        }),
      }),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["skill-groups", groupId, "skills"],
      });
    },
  });
}
