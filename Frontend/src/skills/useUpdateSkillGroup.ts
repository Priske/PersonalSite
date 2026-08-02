import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequestWithoutResponse } from "../api";

type UpdateSkillGroupRequest = {
  name: string;
  displayOrder: number;
};

export function useUpdateSkillGroup(groupId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: UpdateSkillGroupRequest) =>
      apiRequestWithoutResponse(`/skill-groups/${groupId}`, {
        method: "PUT",
        body: JSON.stringify(request),
      }),

    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({
          queryKey: ["skill-groups"],
        }),

        queryClient.invalidateQueries({
          queryKey: ["skill-groups", groupId],
        }),
      ]);
    },
  });
}
