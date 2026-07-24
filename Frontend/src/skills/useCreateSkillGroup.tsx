import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiRequest } from "../api";

export type CreateSkillGroupRequest = {
    name: string;
    displayOrder: number;

};

export type CreatedSkillGroup = {
    id: number;
    name: string;
    displayOrder: number;
};

export function useCreateSkillGroup() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (
            request: CreateSkillGroupRequest,
        ) =>
            apiRequest<CreatedSkillGroup>(
                `/skill-groups`,
                {
                    method: "POST",
                    body: JSON.stringify(request),
                },
            ),

        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: [
                    "skill-groups"
                ],
            });
        },
    });
}