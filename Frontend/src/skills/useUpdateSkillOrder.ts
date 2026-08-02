import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequestWithoutResponse } from "../api";

export type UpdateSkillOrderRequest = {
  skillIds: number[];
};

export function useUpdateSkillOrder(groupId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: UpdateSkillOrderRequest) =>
      apiRequestWithoutResponse(`/skill-groups/${groupId}/skills/order`, {
        method: "PUT",
        body: JSON.stringify(request),
      }),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["skill-groups", groupId, "skills"],
      });
    },
  });
}
