import { useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { useCurrentUser } from "../auth/useCurrentUser";
import { removeAccessToken } from "../auth/tokenStorage";
import { deleteUser } from "./usersApi";

type DeleteUserButtonProps = {
  userId: number;
};

export function DeleteUserButton({ userId }: DeleteUserButtonProps) {
  const [confirming, setConfirming] = useState(false);
  const currentUserQuery = useCurrentUser();
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const deletingCurrentUser = currentUserQuery.data?.id === userId;

  function clearDeletedUserQueries() {
    queryClient.invalidateQueries({
      queryKey: ["users"],
      refetchType: "none",
    });

    queryClient.removeQueries({
      queryKey: ["users", "detail", userId],
      exact: true,
    });
  }

  function leaveDeletedUser() {
    clearDeletedUserQueries();

    if (deletingCurrentUser) {
      removeAccessToken();

      queryClient.removeQueries({
        queryKey: ["current-user"],
        exact: true,
      });

      navigate("/login", {
        replace: true,
        state: {
          accountDeleted: true,
        },
      });

      return;
    }

    navigate("/users", { replace: true });
  }

  const deleteMutation = useMutation({
    mutationFn: () => deleteUser(userId),
    onSuccess: leaveDeletedUser,
  });

  const canDelete =
    currentUserQuery.isSuccess &&
    (currentUserQuery.data.role === "Administrator" ||
      currentUserQuery.data.id === userId);

  if (!canDelete) {
    return null;
  }

  const mutationStatus =
    deleteMutation.error instanceof ApiError
      ? deleteMutation.error.status
      : null;

  if (!confirming) {
    return (
      <button
        className="delete-user-trigger"
        type="button"
        onClick={() => setConfirming(true)}
      >
        Delete account
      </button>
    );
  }

  return (
    <section
      className="delete-user-confirmation"
      aria-labelledby="delete-user-heading"
    >
      <div className="delete-user-confirmation__content">
        <p className="delete-user-confirmation__eyebrow">
          Confirmation required
        </p>

        <h3 id="delete-user-heading">
          {deletingCurrentUser ? "Delete your account?" : "Delete this user?"}
        </h3>

        <p className="delete-user-confirmation__description">
          This action cannot be undone. The account and its associated data will
          be permanently removed.
        </p>
      </div>

      <div className="delete-user-confirmation__actions">
        <button
          className="delete-user-confirmation__confirm"
          type="button"
          onClick={() => deleteMutation.mutate()}
          disabled={deleteMutation.isPending}
        >
          {deleteMutation.isPending
            ? "Deleting..."
            : deletingCurrentUser
              ? "Yes, delete my account"
              : "Yes, delete user"}
        </button>

        <button
          className="delete-user-confirmation__cancel"
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

      <div className="delete-user-confirmation__messages" aria-live="polite">
        {mutationStatus === 401 && (
          <p className="form-message form-message--error">
            Your login is missing or expired.
          </p>
        )}

        {mutationStatus === 403 && (
          <p className="form-message form-message--error">
            You are not allowed to delete this user.
          </p>
        )}

        {mutationStatus === 404 && (
          <div className="delete-user-confirmation__missing">
            <p className="form-message form-message--error">
              This user no longer exists. It may already have been deleted.
            </p>

            <button className="button" type="button" onClick={leaveDeletedUser}>
              {deletingCurrentUser ? "Return to login" : "Back to users"}
            </button>
          </div>
        )}

        {deleteMutation.isError &&
          mutationStatus !== 401 &&
          mutationStatus !== 403 &&
          mutationStatus !== 404 && (
            <p className="form-message form-message--error">
              Could not delete the user.
            </p>
          )}
      </div>
    </section>
  );
}
