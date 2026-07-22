export function HeroSection() {
  return (
    <section className="home-hero" id="about">
      <div className="home-hero__identity">
        <p className="section-banner">Software developer</p>

        <h1>
          Ben
          <br />
          Eeckman
        </h1>

        <p className="home-hero__location">
          Junior software developer
        </p>
      </div>

      <div className="home-hero__content">
        <div className="home-hero__connector" aria-hidden="true">
          <span className="home-hero__connector-dot" />
          <span className="home-hero__connector-line" />
        </div>

        <div className="home-hero__copy">
          <p className="home-hero__eyebrow">
            Practical software. Clear structure.
          </p>

          <h2>
            I build maintainable applications for the web.
          </h2>

          <p className="home-hero__summary">
            I work with C#, ASP.NET Core, React, TypeScript and SQL to
            create software that is understandable, useful and easy to
            develop further.
          </p>

          <div className="home-hero__actions">
            <a className="button" href="#projects">
              View projects
            </a>

            <a className="button button--secondary" href="#contact">
              Contact me
            </a>
          </div>
        </div>
      </div>
    </section>
  );
}