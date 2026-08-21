import { useEffect, useId, useRef, useState, type FormEvent } from "react";
import { useMutation } from "@tanstack/react-query";
import { Link } from "react-router-dom";
import { trackLinkClick } from "../analytics/analytics";
import { apiPath } from "../api";
import { getAccessToken } from "../auth/tokenStorage";
import type { GetHomePageConfigDetailsResponse } from "../homePageConfig/types";
import { makeContact } from "../mails/mailsApi";

type ContactSectionProps = {
  config: GetHomePageConfigDetailsResponse;
  number: string;
};

type IconProps = {
  className?: string;
};

function MailIcon({ className }: IconProps) {
  return (
    <svg className={className} viewBox="0 0 24 24" aria-hidden="true">
      <path d="M3 5h18v14H3V5Zm2 2v.5l7 5 7-5V7H5Zm14 10V10l-7 5-7-5v7h14Z" />
    </svg>
  );
}

function PhoneIcon({ className }: IconProps) {
  return (
    <svg className={className} viewBox="0 0 24 24" aria-hidden="true">
      <path d="M6.6 2 10 5.4 8.2 7.2c1.2 2.5 3.1 4.4 5.6 5.6l1.8-1.8 3.4 3.4-2.2 2.2c-.8.8-2 1.1-3.1.7C8.1 15.5 4.5 11.9 2.7 6.3c-.4-1.1-.1-2.3.7-3.1L6.6 2Z" />
    </svg>
  );
}

function GitHubIcon({ className }: IconProps) {
  return (
    <svg className={className} viewBox="0 0 24 24" aria-hidden="true">
      <path d="M12 2a10 10 0 0 0-3.2 19.5c.5.1.7-.2.7-.5v-1.9c-2.8.6-3.4-1.2-3.4-1.2-.5-1.2-1.1-1.5-1.1-1.5-.9-.6.1-.6.1-.6 1 0 1.5 1 1.5 1 .9 1.5 2.3 1.1 2.9.8.1-.6.3-1.1.6-1.3-2.2-.3-4.6-1.1-4.6-4.9 0-1.1.4-2 1-2.7-.1-.3-.4-1.3.1-2.7 0 0 .8-.3 2.8 1a9.6 9.6 0 0 1 5.1 0c2-1.3 2.8-1 2.8-1 .5 1.4.2 2.4.1 2.7.7.7 1 1.6 1 2.7 0 3.8-2.3 4.6-4.6 4.9.4.3.7.9.7 1.7V21c0 .3.2.6.7.5A10 10 0 0 0 12 2Z" />
    </svg>
  );
}

function LinkedInIcon({ className }: IconProps) {
  return (
    <svg className={className} viewBox="0 0 24 24" aria-hidden="true">
      <path d="M5 3.5A2.5 2.5 0 1 1 5 8a2.5 2.5 0 0 1 0-5ZM3 9h4v12H3V9Zm6 0h3.8v1.6h.1c.5-.9 1.8-2 3.7-2 4 0 4.7 2.6 4.7 6V21h-4v-5.7c0-1.4 0-3.1-1.9-3.1s-2.2 1.5-2.2 3V21H9V9Z" />
    </svg>
  );
}

function CvIcon({ className }: IconProps) {
  return (
    <svg className={className} viewBox="0 0 24 24" aria-hidden="true">
      <path d="M5 2h9l5 5v15H5V2Zm9 2.5V8h3.5L14 4.5ZM8 12h8v-2H8v2Zm0 4h8v-2H8v2Zm0 4h6v-2H8v2Z" />
    </svg>
  );
}

export function ContactSection({ config, number }: ContactSectionProps) {
  const isAuthenticated = Boolean(getAccessToken());
  const isDemo = config.source === "Demo";
  const phoneNumber = config.phoneNumber;

  const cvUrl = isDemo ? config.cvUrl : apiPath("/files/cv");

  const contactFormId = useId();
  const formRef = useRef<HTMLFormElement>(null);

  const [isContactFormOpen, setIsContactFormOpen] = useState(false);

  const [formError, setFormError] = useState<string | null>(null);

  const [messageSent, setMessageSent] = useState(false);

  const contactMutation = useMutation({
    mutationFn: makeContact,

    onSuccess: () => {
      formRef.current?.reset();
      setFormError(null);
      setMessageSent(true);
    },

    onError: (error) => {
      setMessageSent(false);

      setFormError(
        error instanceof Error
          ? error.message
          : "The message could not be sent.",
      );
    },
  });

  useEffect(() => {
    if (!isContactFormOpen || isDemo) {
      return;
    }

    const animationFrame = window.requestAnimationFrame(() => {
      formRef.current?.scrollIntoView({
        behavior: "smooth",
        block: "center",
        inline: "nearest",
      });
    });

    return () => {
      window.cancelAnimationFrame(animationFrame);
    };
  }, [isContactFormOpen, isDemo]);

  function toggleContactForm() {
    setIsContactFormOpen((isOpen) => !isOpen);
    setFormError(null);
    setMessageSent(false);
    contactMutation.reset();
  }

  function handleContactSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    setFormError(null);
    setMessageSent(false);

    const formData = new FormData(event.currentTarget);

    const name = formData.get("name")?.toString().trim() ?? "";

    const email = formData.get("email")?.toString().trim() ?? "";

    const message = formData.get("message")?.toString().trim() ?? "";

    if (!name || !email || !message) {
      setFormError("Name, email and message are required.");
      return;
    }

    if (message.length < 10) {
      setFormError("Your message must contain at least 10 characters.");
      return;
    }

    contactMutation.mutate({
      name,
      email,
      message,
    });
  }

  return (
    <section className="home-section home-contact" id="contact">
      <div className="home-section__heading">
        <p className="home-section__number">{number}</p>

        <div>
          <p className="home-section__eyebrow">
            {config.contactSectionEyebrow}
          </p>

          <h2>{config.contactSectionHeading}</h2>
        </div>
      </div>

      <div className="home-section__content">
        <div className="home-section__connector" aria-hidden="true">
          <span className="home-section__connector-dot" />
          <span className="home-section__connector-line" />
        </div>

        <div className="contact-panel">
          <div className="contact-panel__content">
            <p className="contact-panel__eyebrow">{config.contactEyebrow}</p>

            <h3>{config.contactHeading}</h3>

            <p className="contact-panel__description">
              {config.contactDescription}
            </p>

            <div className="contact-panel__links">
              {config.email && (
                <a
                  className="contact-panel__link"
                  href={`mailto:${config.email}`}
                  title="Email"
                  aria-label="Email"
                  onClick={() =>
                    void trackLinkClick(
                      "email",
                      `mailto:${config.email}`,
                      "contact",
                    )
                  }
                >
                  <MailIcon />
                </a>
              )}

              {phoneNumber && (
                <a
                  className="contact-panel__link"
                  href={`tel:${phoneNumber.replace(/\s/g, "")}`}
                  title="Phone"
                  aria-label="Phone"
                  onClick={() =>
                    void trackLinkClick(
                      "phone",
                      `tel:${phoneNumber.replace(/\s/g, "")}`,
                      "contact",
                    )
                  }
                >
                  <PhoneIcon />
                </a>
              )}

              {config.gitHubUrl && (
                <a
                  className="contact-panel__link"
                  href={config.gitHubUrl}
                  target="_blank"
                  rel="noreferrer"
                  title="GitHub"
                  aria-label="GitHub"
                  onClick={() =>
                    void trackLinkClick(
                      "github",
                      `githubUrl:${config.gitHubUrl}`,
                      "contact",
                    )
                  }
                >
                  <GitHubIcon />
                </a>
              )}

              {config.linkedInUrl && (
                <a
                  className="contact-panel__link"
                  href={config.linkedInUrl}
                  target="_blank"
                  rel="noreferrer"
                  title="LinkedIn"
                  aria-label="LinkedIn"
                  onClick={() =>
                    void trackLinkClick(
                      "linkedin",
                      `linkedinUrl:${config.linkedInUrl}`,
                      "contact",
                    )
                  }
                >
                  <LinkedInIcon />
                </a>
              )}

              {cvUrl && (
                <a
                  className="contact-panel__link"
                  href={cvUrl}
                  target="_blank"
                  rel="noreferrer"
                  title="View CV"
                  aria-label="View CV"
                  onClick={() => void trackLinkClick("cv", cvUrl, "contact")}
                >
                  <CvIcon />
                </a>
              )}
            </div>
          </div>

          <div className="contact-panel__actions">
            {isDemo ? (
              <a
                className="button"
                href={`mailto:${config.email}`}
                onClick={() =>
                  void trackLinkClick(
                    "email",
                    `mailto:${config.email}`,
                    "contact",
                  )
                }
              >
                {config.contactEmailActionLabel}
              </a>
            ) : (
              <button
                className="button"
                type="button"
                aria-expanded={isContactFormOpen}
                aria-controls={contactFormId}
                disabled={contactMutation.isPending}
                onClick={toggleContactForm}
              >
                {isContactFormOpen
                  ? "Close form"
                  : config.contactEmailActionLabel}
              </button>
            )}

            <Link
              className="button button--secondary"
              to={isAuthenticated ? "/account" : "/login"}
            >
              {isAuthenticated ? "Account" : config.contactLoginActionLabel}
            </Link>
          </div>

          {!isDemo && isContactFormOpen && (
            <form
              ref={formRef}
              className="contact-form"
              id={contactFormId}
              onSubmit={handleContactSubmit}
            >
              <div className="contact-form__fields">
                <div className="contact-form__field">
                  <label htmlFor={`${contactFormId}-name`}>Name</label>

                  <input
                    id={`${contactFormId}-name`}
                    name="name"
                    autoComplete="name"
                    maxLength={100}
                    required
                  />
                </div>

                <div className="contact-form__field">
                  <label htmlFor={`${contactFormId}-email`}>Email</label>

                  <input
                    id={`${contactFormId}-email`}
                    name="email"
                    type="email"
                    autoComplete="email"
                    maxLength={254}
                    required
                  />
                </div>

                <div className="contact-form__field contact-form__field--message">
                  <label htmlFor={`${contactFormId}-message`}>Message</label>

                  <textarea
                    id={`${contactFormId}-message`}
                    name="message"
                    minLength={10}
                    maxLength={5000}
                    rows={8}
                    required
                  />
                </div>
              </div>

              <div className="contact-form__status" aria-live="polite">
                {formError && (
                  <p className="contact-form__error">{formError}</p>
                )}

                {messageSent && (
                  <p className="contact-form__success">
                    Your message has been sent.
                  </p>
                )}
              </div>

              <div className="contact-form__actions">
                <button
                  className="button"
                  type="submit"
                  disabled={contactMutation.isPending}
                >
                  {contactMutation.isPending ? "Sending..." : "Send message"}
                </button>
              </div>
            </form>
          )}
        </div>
      </div>
    </section>
  );
}
