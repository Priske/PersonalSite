import { useState, type FormEvent } from "react";
import { useMutation } from "@tanstack/react-query";
import { Link, useNavigate } from "react-router-dom";
import { ApiError } from "../api";
import { registerUser } from "./usersApi";

const PASSWORD_MIN_LENGTH = 15;
const PASSWORD_MAX_LENGTH = 128;

export function RegisterPage() {
  const [formError, setFormError] = useState<string | null>(null);
  const navigate = useNavigate();

  const registerMutation = useMutation({
    mutationFn: registerUser,

    onSuccess: (user) => {
      navigate("/login", {
        state: {
          registered: true,
          email: user.email,
        },
      });
    },
  });

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setFormError(null);

    const formData = new FormData(event.currentTarget);

    const name = formData.get("name")?.toString().trim() ?? "";
    const email = formData.get("email")?.toString().trim() ?? "";
    const password = formData.get("password")?.toString() ?? "";
    const confirmPassword =
      formData.get("confirmPassword")?.toString() ?? "";

    if (!name || !email || !password) {
      setFormError("Name, email and password are required.");
      return;
    }

    if (password.length < PASSWORD_MIN_LENGTH) {
      setFormError(
        `Password must contain at least ${PASSWORD_MIN_LENGTH} characters.`,
      );
      return;
    }

    if (password.length > PASSWORD_MAX_LENGTH) {
      setFormError(
        `Password cannot contain more than ${PASSWORD_MAX_LENGTH} characters.`,
      );
      return;
    }

    if (password !== confirmPassword) {
      setFormError("Passwords do not match.");
      return;
    }

    registerMutation.mutate({
      name,
      email,
      password,
    });
  }

  const mutationStatus =
    registerMutation.error instanceof ApiError
      ? registerMutation.error.status
      : null;

  return (
    <main className="register-page">
      <section className="register-page__intro">
        <p className="section-banner">User access</p>

        <h1>
          Create
          <br />
          account
        </h1>

        <p className="register-page__intro-text">
          Create an account using a long passphrase that is easy for you
          to remember and difficult for others to guess.
        </p>
      </section>

      <section className="register-page__content">
        <span
          className="register-page__connector"
          aria-hidden="true"
        >
          <span className="register-page__connector-line" />
          <span className="register-page__connector-dot" />
        </span>

        <article className="register-card">
          <header className="register-card__header">
            <p className="register-card__eyebrow">
              New account
            </p>

            <h2>Register</h2>
          </header>

          <form
            className="register-form"
            onSubmit={handleSubmit}
          >
            <div className="register-form__field">
              <label htmlFor="register-name">
                Name
              </label>

              <input
                id="register-name"
                name="name"
                autoComplete="name"
                maxLength={100}
                required
              />
            </div>

            <div className="register-form__field">
              <label htmlFor="register-email">
                Email
              </label>

              <input
                id="register-email"
                name="email"
                type="email"
                autoComplete="email"
                maxLength={200}
                required
              />
            </div>

            <div className="register-form__field">
              <label htmlFor="register-password">
                Passphrase
              </label>

              <input
                id="register-password"
                name="password"
                type="password"
                autoComplete="new-password"
                minLength={PASSWORD_MIN_LENGTH}
                maxLength={PASSWORD_MAX_LENGTH}
                aria-describedby="register-password-help"
                required
              />

              <p
                className="register-form__help"
                id="register-password-help"
              >
                Use between {PASSWORD_MIN_LENGTH} and{" "}
                {PASSWORD_MAX_LENGTH} characters. Common or compromised
                passwords will be rejected.
              </p>
            </div>

            <div className="register-form__field">
              <label htmlFor="register-confirm-password">
                Confirm passphrase
              </label>

              <input
                id="register-confirm-password"
                name="confirmPassword"
                type="password"
                autoComplete="new-password"
                minLength={PASSWORD_MIN_LENGTH}
                maxLength={PASSWORD_MAX_LENGTH}
                required
              />
            </div>

            <div
              className="register-form__messages"
              aria-live="polite"
            >
              {formError && (
                <p className="form-message form-message--error">
                  {formError}
                </p>
              )}

              {mutationStatus === 400 && (
                <p className="form-message form-message--error">
                  The registration data was rejected. Check that your
                  passphrase is between 15 and 128 characters and has not
                  appeared in a known data breach.
                </p>
              )}

              {mutationStatus === 409 && (
                <p className="form-message form-message--error">
                  An account with this email already exists.
                </p>
              )}

              {registerMutation.isError &&
                mutationStatus !== 400 &&
                mutationStatus !== 409 && (
                  <p className="form-message form-message--error">
                    Could not create the account. The server may be
                    unavailable.
                  </p>
                )}
            </div>

            <div className="register-form__actions">
              <Link
                className="button button--secondary"
                to="/login"
              >
                Log in
              </Link>

              <button
                className="button"
                type="submit"
                disabled={registerMutation.isPending}
              >
                {registerMutation.isPending
                  ? "Creating account..."
                  : "Create account"}
              </button>
            </div>
          </form>
        </article>
      </section>
    </main>
  );
}