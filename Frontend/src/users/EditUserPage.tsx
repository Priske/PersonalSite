import { useState, type FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Link, useNavigate, useParams } from "react-router-dom";
import { ApiError } from "../api";
import type { UpdateUserRequest } from "./types";
import { getUser, updateUser } from "./usersApi";
import { DeleteUserButton } from "./DeleteUserButton";

function readUserId(value: string | undefined) {
  const userId = Number(value);

  return Number.isInteger(userId) && userId > 0 ? userId : null;
}

export function EditUserPage() {
  const { userId: userIdParameter } = useParams();
  const userId = readUserId(userIdParameter);

  const [formError, setFormError] = useState<string | null>(null);

  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const userQuery = useQuery({
    queryKey: ["users", "detail", userId],

    queryFn: () => {
      if (userId === null) {
        throw new Error("Invalid user id");
      }

      return getUser(userId);
    },

    enabled: userId !== null,
    retry: false,
  });

  const updateMutation = useMutation({
    mutationFn: (request: UpdateUserRequest) => {
      if (userId === null) {
        throw new Error("Invalid user id");
      }

      return updateUser(userId, request);
    },

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["users"],
      });

      navigate("/users", {
        replace: true,
      });
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    const formData = new FormData(event.currentTarget);

    const name = formData.get("name")?.toString().trim() ?? "";

    const email = formData.get("email")?.toString().trim() ?? "";
    const role = userQuery.data?.role;

    if (!name || !email || !role) {
      setFormError("Enter a valid name, role  and email address.");

      return;
    }

    updateMutation.mutate({
      name,
      email,
      role,
    });
  }

  if (userId === null) {
    return (
      <main className="admin-message-page">
        <section className="admin-message-card">
          <p className="admin-message-card__eyebrow">Invalid request</p>

          <h1>Invalid user ID</h1>

          <p>The supplied user identifier is not valid.</p>

          <Link className="button" to="/users">
            Back to users
          </Link>
        </section>
      </main>
    );
  }

  if (userQuery.isPending) {
    return (
      <main className="admin-message-page">
        <p className="form-message" role="status">
          Loading user...
        </p>
      </main>
    );
  }

  const queryNotFound =
    userQuery.error instanceof ApiError && userQuery.error.status === 404;

  if (queryNotFound) {
    return (
      <main className="admin-message-page">
        <section className="admin-message-card">
          <p className="admin-message-card__eyebrow">Not found</p>

          <h1>User not found</h1>

          <p>This user may have already been deleted.</p>

          <Link className="button" to="/users">
            Back to users
          </Link>
        </section>
      </main>
    );
  }

  if (userQuery.isError) {
    const status =
      userQuery.error instanceof Error && "status" in userQuery.error
        ? (userQuery.error as Error & { status: number }).status
        : undefined;

    if (status === 403) {
      return (
        <main className="admin-message-page">
          <section className="admin-message-card">
            <p className="admin-message-card__eyebrow">Forbidden</p>

            <h1>Access denied</h1>

            <p>You do not have permission to manage this user.</p>

            <Link className="button" to="/users">
              Back to users
            </Link>
          </section>
        </main>
      );
    }

    if (status === 404) {
      return (
        <main className="admin-message-page">
          <section className="admin-message-card">
            <p className="admin-message-card__eyebrow">Not found</p>

            <h1>User not found</h1>

            <p>This user no longer exists.</p>

            <Link className="button" to="/users">
              Back to users
            </Link>
          </section>
        </main>
      );
    }

    return (
      <main className="admin-message-page">
        <section className="admin-message-card">
          <p className="admin-message-card__eyebrow">Error</p>

          <h1>Could not load user</h1>

          <p>The server may currently be unavailable.</p>

          <Link className="button" to="/users">
            Back to users
          </Link>
        </section>
      </main>
    );
  }

  const user = userQuery.data;

  const mutationStatus =
    updateMutation.error instanceof ApiError
      ? updateMutation.error.status
      : null;

  return (
    <main className="edit-user-page">
      <section className="edit-user-page__intro">
        <p className="section-banner">Administration</p>

        <h1>
          Edit
          <br />
          user
        </h1>

        <p className="edit-user-page__intro-text">
          Update this user's account information or remove the account
          permanently.
        </p>
      </section>

      <section className="edit-user-page__content">
        <span className="edit-user-page__connector" aria-hidden="true">
          <span className="edit-user-page__connector-line" />
          <span className="edit-user-page__connector-dot" />
        </span>

        <article className="edit-user-card">
          <header className="edit-user-card__header">
            <div>
              <p className="edit-user-card__eyebrow">User #{user.id}</p>

              <h2>{user.name}</h2>
            </div>

            <Link className="edit-user-card__cancel" to="/users">
              Cancel
            </Link>
          </header>

          <div className="edit-user-card__body">
            <form
              className="edit-user-form"
              key={user.id}
              onSubmit={handleSubmit}
            >
              <div className="edit-user-form__field">
                <label htmlFor="edit-user-name">Name</label>

                <input
                  id="edit-user-name"
                  name="name"
                  defaultValue={user.name}
                  autoComplete="name"
                  maxLength={100}
                  required
                />
              </div>

              <div className="edit-user-form__field">
                <label htmlFor="edit-user-email">Email</label>
                <input
                  id="edit-user-email"
                  name="email"
                  type="email"
                  defaultValue={user.email}
                  autoComplete="email"
                  maxLength={100}
                  required
                />
              </div>

              <div className="edit-user-form__messages" aria-live="polite">
                {formError && (
                  <p className="form-message form-message--error">
                    {formError}
                  </p>
                )}

                {mutationStatus === 400 && (
                  <p className="form-message form-message--error">
                    The API rejected the user data.
                  </p>
                )}

                {mutationStatus === 401 && (
                  <p className="form-message form-message--error">
                    Your login is missing or expired.
                  </p>
                )}

                {mutationStatus === 403 && (
                  <p className="form-message form-message--error">
                    Only administrators can edit this user.
                  </p>
                )}

                {mutationStatus === 404 && (
                  <p className="form-message form-message--error">
                    This user no longer exists.
                  </p>
                )}

                {updateMutation.isError && mutationStatus === null && (
                  <p className="form-message form-message--error">
                    Could not update the user.
                  </p>
                )}
              </div>

              <div className="edit-user-form__actions">
                <Link className="button button--secondary" to="/users">
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

            <section className="edit-user-danger">
              <header className="edit-user-danger__header">
                <p className="edit-user-danger__eyebrow">Danger zone</p>

                <h3>Delete user</h3>

                <p>
                  Permanently delete this account and its associated data. This
                  action cannot be undone.
                </p>
              </header>

              <DeleteUserButton userId={user.id} targetRole={user.role} />
            </section>
          </div>
        </article>
      </section>
    </main>
  );
}
