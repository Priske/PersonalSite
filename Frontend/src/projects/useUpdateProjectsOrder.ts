import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequestWithoutResponse } from "../api";

type UpdateProjectGroupOrderRequest = {
  projectIds: number[];
};

export function useUpdateProjectsOrder(){
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn:(request: UpdateProjectGroupOrderRequest) =>
            apiRequestWithoutResponse("/projects/order",{
                method:"PUT",
                body: JSON.stringify(request),
            }),
            onSuccess: async() =>{
                await queryClient.invalidateQueries({
                    queryKey: ["projects"],
                });
            },
    });
}