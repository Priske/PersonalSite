import { useState, type FormEvent } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useLocation, useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { login } from "./authApi";
import { setAccessToken } from "./tokenStorage";

type LoginLocationState = {
  registered?: boolean;
  email?: string;
  accountDeleted?: boolean;
};

export function LoginPage() {
  const location = useLocation();
  const locationState = location.state as LoginLocationState | null;

  const [email, setEmail] = useState(locationState?.email ?? "");
  const [password, setPassword] = useState("");

  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const loginMutation = useMutation({
    mutationFn: login,

    onSuccess: async (response) => {
      setAccessToken(response.accessToken);

      await queryClient.invalidateQueries({
        queryKey: ["current-user"],
      });

      navigate("/account", { replace: true });
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    loginMutation.mutate({ email, password });
  }

  const invalidCredentials =
    loginMutation.error instanceof ApiError &&
    loginMutation.error.status === 401;

  return (
    <main className="login-page">
      <section className="login-page__intro">
        <p className="section-banner">User access</p>

        <h1>
          Welcome
          <br />
          back
        </h1>

        <p className="login-page__intro-text">
          Log in to view and manage your account information.
        </p>
      </section>

      <section className="login-page__content">
        <span className="login-page__connector" aria-hidden="true">
          <span className="login-page__connector-line" />
          <span className="login-page__connector-dot" />
        </span>

        <article className="login-card">
          <header className="login-card__header">
            <p className="login-card__eyebrow">Account</p>
            <h2>Log in</h2>
          </header>

          <form className="login-form" onSubmit={handleSubmit}>
            {locationState?.registered && (
              <p className="form-message" role="status">
                Your account was created. You can now log in.
              </p>
            )}

            {locationState?.accountDeleted && (
              <p className="form-message" role="status">
                Your account was deleted successfully.
              </p>
            )}

            <div className="login-form__field">
              <label htmlFor="login-email">Email</label>

              <input
                id="login-email"
                type="email"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                autoComplete="email"
                maxLength={100}
                required
              />
            </div>

            <div className="login-form__field">
              <label htmlFor="login-password">Password</label>

              <input
                id="login-password"
                type="password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                autoComplete="current-password"
                required
              />
            </div>

            <div className="login-form__messages" aria-live="polite">
              {invalidCredentials && (
                <p className="form-message form-message--error">
                  Email or password is incorrect.
                </p>
              )}

              {loginMutation.isError && !invalidCredentials && (
                <p className="form-message form-message--error">
                  Login failed. The server may be unavailable.
                </p>
              )}
            </div>

            <div className="login-form__actions">
              <button
                className="button"
                type="submit"
                disabled={loginMutation.isPending}
              >
                {loginMutation.isPending
                  ? "Logging in..."
                  : "Log in"}
              </button>
            </div>
          </form>
        </article>
      </section>
    </main>
  );
}