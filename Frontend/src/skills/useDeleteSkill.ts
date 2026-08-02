import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequestWithoutResponse } from "../api";

export function useDeleteSkill(groupId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (skillId: number) =>
      apiRequestWithoutResponse(`/skill-groups/${groupId}/skills/${skillId}`, {
        method: "DELETE",
      }),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["skill-groups", groupId, "skills"],
      });
    },
  });
}
