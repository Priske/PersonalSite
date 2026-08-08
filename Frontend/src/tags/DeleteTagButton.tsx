import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";

import { ApiError } from "../api";
import { deleteTag } from "./tagsApi";

type DeleteTagButtonProps = {
  tagId: number;
  tagName: string;
  disabled: boolean;
  onDeleted: () => void;
};

export function DeleteTagButton({
  tagId,
  tagName,
  disabled,
  onDeleted,
}: DeleteTagButtonProps) {
  const queryClient = useQueryClient();

  const [isConfirming, setIsConfirming] = useState(false);

  const deleteMutation = useMutation({
    mutationFn: () => deleteTag(tagId),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["tags"],
      });

      queryClient.removeQueries({
        queryKey: ["tags", "detail", tagId],
      });

      onDeleted();
    },
  });

  const errorStatus =
    deleteMutation.error instanceof ApiError
      ? deleteMutation.error.status
      : null;

  function beginConfirmation() {
    deleteMutation.reset();
    setIsConfirming(true);
  }

  function cancelConfirmation() {
    deleteMutation.reset();
    setIsConfirming(false);
  }

  function confirmDelete() {
    deleteMutation.mutate();
  }

  if (!isConfirming) {
    return (
      <button
        className="button button--danger"
        type="button"
        disabled={disabled}
        onClick={beginConfirmation}
      >
        Delete tag
      </button>
    );
  }

  return (
    <div className="delete-tag-confirmation">
      <p className="delete-tag-confirmation__message">
        Permanently delete <strong>{tagName}</strong>?
      </p>

      <div className="delete-tag-confirmation__actions">
        <button
          className="button button--secondary"
          type="button"
          disabled={deleteMutation.isPending}
          onClick={cancelConfirmation}
        >
          Cancel
        </button>

        <button
          className="button button--danger"
          type="button"
          disabled={deleteMutation.isPending}
          onClick={confirmDelete}
        >
          {deleteMutation.isPending ? "Deleting..." : "Confirm delete"}
        </button>
      </div>

      <div className="delete-tag-confirmation__messages" aria-live="polite">
        {errorStatus === 401 && (
          <p className="form-message form-message--error">
            Your login is missing or expired.
          </p>
        )}

        {errorStatus === 403 && (
          <p className="form-message form-message--error">
            You can only delete tags you created yourself.
          </p>
        )}

        {errorStatus === 404 && (
          <p className="form-message form-message--error">
            This tag no longer exists.
          </p>
        )}

        {errorStatus === 409 && (
          <p className="form-message form-message--error">
            This tag cannot be deleted while it is used by a project.
          </p>
        )}

        {deleteMutation.isError &&
          errorStatus !== 401 &&
          errorStatus !== 403 &&
          errorStatus !== 404 &&
          errorStatus !== 409 && (
            <p className="form-message form-message--error">
              Could not delete the tag.
            </p>
          )}
      </div>
    </div>
  );
}
