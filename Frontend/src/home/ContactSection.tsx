import { Link } from "react-router-dom";

export function ContactSection() {
  return (
    <section
      className="home-section home-contact"
      id="contact"
    >
      <div className="home-section__heading">
        <p className="home-section__number">03</p>

        <div>
          <p className="home-section__eyebrow">
            Get in touch
          </p>

          <h2>Contact</h2>
        </div>
      </div>

      <div className="home-section__content">
        <div
          className="home-section__connector"
          aria-hidden="true"
        >
          <span className="home-section__connector-dot" />
          <span className="home-section__connector-line" />
        </div>

        <div className="contact-panel">
          <div>
            <p className="contact-panel__eyebrow">
              Have a project or opportunity?
            </p>

            <h3>Let&apos;s talk.</h3>

            <p>
              I am interested in junior software-development roles,
              practical projects and opportunities to continue
              developing my skills.
            </p>
          </div>

          <div className="contact-panel__actions">
            <a
              className="button"
              href="mailto:your-email@example.com"
            >
              Send an email
            </a>

            <Link
              className="button button--secondary"
              to="/login"
            >
              Account login
            </Link>
          </div>
        </div>
      </div>
    </section>
  );
}