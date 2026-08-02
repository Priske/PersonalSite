import { useState, type FormEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { useCurrentUser } from "../auth/useCurrentUser";
import type { UpdateUserRequest } from "../users/types";
import { updateUser } from "../users/usersApi";
import { DeleteUserButton } from "../users/DeleteUserButton";
export function EditAccountPage() {
  const currentUserQuery = useCurrentUser();
  const [formError, setFormError] = useState<string | null>(null);
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const updateMutation = useMutation({
    mutationFn: (request: UpdateUserRequest) => {
      const user = currentUserQuery.data;

      if (!user) {
        throw new Error("Current user is unavailable.");
      }

      return updateUser(user.id, request);
    },

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["current-user"],
        exact: true,
      });

      navigate("/account", { replace: true });
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    const formData = new FormData(event.currentTarget);
    const name = formData.get("name")?.toString().trim() ?? "";
    const email = formData.get("email")?.toString().trim() ?? "";

    if (!name || !email) {
      setFormError("Enter a valid name and email.");
      return;
    }

    updateMutation.mutate({ name, email });
  }

  if (currentUserQuery.isPending) {
    return (
      <main className="edit-account-status">
        <p className="edit-account-status__message">Loading account...</p>
      </main>
    );
  }

  const unauthorized =
    currentUserQuery.error instanceof ApiError &&
    currentUserQuery.error.status === 401;

  if (unauthorized) {
    return <Navigate to="/login" replace />;
  }

  if (currentUserQuery.isError || !currentUserQuery.data) {
    return (
      <main className="edit-account-status">
        <p className="form-message form-message--error">
          Could not load the account.
        </p>
      </main>
    );
  }

  const user = currentUserQuery.data;

  const mutationStatus =
    updateMutation.error instanceof ApiError
      ? updateMutation.error.status
      : null;

  return (
    <article className="account-card edit-account-card">
      <header className="account-card__header edit-account-card__header">
        <p className="account-card__eyebrow">Personal details</p>

        <h2>Edit account</h2>

        <p className="edit-account-card__description">
          Update the name and email address associated with your account.
        </p>
      </header>

      <form className="edit-account-form" key={user.id} onSubmit={handleSubmit}>
        <div className="edit-account-form__field">
          <label htmlFor="edit-account-name">Name</label>

          <input
            id="edit-account-name"
            name="name"
            defaultValue={user.name}
            maxLength={100}
            autoComplete="name"
            required
          />
        </div>

        <div className="edit-account-form__field">
          <label htmlFor="edit-account-email">Email</label>

          <input
            id="edit-account-email"
            type="email"
            name="email"
            defaultValue={user.email}
            maxLength={100}
            autoComplete="email"
            required
          />
        </div>

        <div className="edit-account-form__messages" aria-live="polite">
          {formError && (
            <p className="form-message form-message--error">{formError}</p>
          )}

          {mutationStatus === 400 && (
            <p className="form-message form-message--error">
              The API rejected the account data.
            </p>
          )}

          {mutationStatus === 401 && (
            <p className="form-message form-message--error">
              Your login is missing or expired.
            </p>
          )}

          {mutationStatus === 403 && (
            <p className="form-message form-message--error">
              You cannot edit this account.
            </p>
          )}

          {mutationStatus === 404 && (
            <p className="form-message form-message--error">
              This account no longer exists.
            </p>
          )}

          {mutationStatus === 409 && (
            <p className="form-message form-message--error">
              That email address is already in use.
            </p>
          )}

          {updateMutation.isError && mutationStatus === null && (
            <p className="form-message form-message--error">
              Could not update the account.
            </p>
          )}
        </div>

        <div className="edit-account-form__actions">
          <Link className="button button--secondary" to="/account">
            Cancel
          </Link>

          <button
            className="button"
            type="submit"
            disabled={updateMutation.isPending}
          >
            {updateMutation.isPending ? "Saving..." : "Save changes"}
          </button>
        </div>
      </form>

      <section className="edit-account-danger">
        <div>
          <p className="edit-account-danger__eyebrow">Danger zone</p>

          <h3>Delete account</h3>

          <p>Permanently remove this account and its associated data.</p>
        </div>

        <div className="edit-account-danger__action">
          <DeleteUserButton userId={user.id} />
        </div>
      </section>
    </article>
  );
}
