import { useState } from "react";
import {
    useMutation,
    useQueryClient,
} from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { deleteSkillGroup } from "./skillGroupApi";

type DeleteSkillGroupButtonProps = {
    skillGroupId: number;
    skillGroupName: string;
};

export function DeleteSkillGroupButton({
    skillGroupId,
    skillGroupName,
}: DeleteSkillGroupButtonProps) {
    const [confirming, setConfirming] =
        useState(false);

    const queryClient = useQueryClient();
    const navigate = useNavigate();

    function leaveDeletedSkillGroup() {
        queryClient.invalidateQueries({
            queryKey: ["skill-groups"],
            refetchType: "none",
        });

        queryClient.removeQueries({
            queryKey: [
                "skill-groups",
                "detail",
                skillGroupId,
            ],
            exact: true,
        });

        queryClient.removeQueries({
            queryKey: ["skills", skillGroupId],
            exact: true,
        });

        navigate("/account/skills", {
            replace: true,
        });
    }

    const deleteMutation = useMutation({
        mutationFn: () =>
            deleteSkillGroup(skillGroupId),

        onSuccess: leaveDeletedSkillGroup,
    });

    const mutationStatus =
        deleteMutation.error instanceof ApiError
            ? deleteMutation.error.status
            : null;

    if (!confirming) {
        return (
            <button
                className="delete-skill-group-trigger"
                type="button"
                onClick={() => setConfirming(true)}
            >
                Delete skill group
            </button>
        );
    }

    return (
        <section
            className="delete-skill-group-confirmation"
            aria-labelledby="delete-skill-group-heading"
        >
            <div className="delete-skill-group-confirmation__content">
                <p className="delete-skill-group-confirmation__eyebrow">
                    Confirmation required
                </p>

                <h3 id="delete-skill-group-heading">
                    Delete this skill group?
                </h3>

                <p className="delete-skill-group-confirmation__description">
                    This action cannot be undone. The skill group{" "}
                    <strong>{skillGroupName}</strong> and all of its
                    attached skills will be permanently removed.
                </p>
            </div>

            <div className="delete-skill-group-confirmation__actions">
                <button
                    className="delete-skill-group-confirmation__confirm"
                    type="button"
                    onClick={() =>
                        deleteMutation.mutate()
                    }
                    disabled={deleteMutation.isPending}
                >
                    {deleteMutation.isPending
                        ? "Deleting..."
                        : "Yes, delete group"}
                </button>

                <button
                    className="delete-skill-group-confirmation__cancel"
                    type="button"
                    onClick={() => {
                        deleteMutation.reset();
                        setConfirming(false);
                    }}
                    disabled={deleteMutation.isPending}
                >
                    Cancel
                </button>
            </div>

            <div
                className="delete-skill-group-confirmation__messages"
                aria-live="polite"
            >
                {mutationStatus === 401 && (
                    <p className="form-message form-message--error">
                        Your login is missing or expired.
                    </p>
                )}

                {mutationStatus === 403 && (
                    <p className="form-message form-message--error">
                        You are not allowed to delete this skill
                        group.
                    </p>
                )}

                {mutationStatus === 404 && (
                    <div className="delete-skill-group-confirmation__missing">
                        <p className="form-message form-message--error">
                            This skill group no longer exists. It may
                            already have been deleted.
                        </p>

                        <button
                            className="button"
                            type="button"
                            onClick={leaveDeletedSkillGroup}
                        >
                            Back to skills
                        </button>
                    </div>
                )}

                {deleteMutation.isError &&
                    mutationStatus !== 401 &&
                    mutationStatus !== 403 &&
                    mutationStatus !== 404 && (
                        <p className="form-message form-message--error">
                            Could not delete the skill group.
                        </p>
                    )}
            </div>
        </section>
    );
}