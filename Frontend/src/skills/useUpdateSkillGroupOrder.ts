import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequestWithoutResponse } from "../api";

type UpdateSkillGroupOrderRequest = {
  skillGroupIds: number[];
};

export function useUpdateSkillGroupOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: UpdateSkillGroupOrderRequest) =>
      apiRequestWithoutResponse("/skill-groups/order", {
          method: "PUT",
          body: JSON.stringify(request),
        },
      ),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["skill-groups"],
      });
    },
  });
}