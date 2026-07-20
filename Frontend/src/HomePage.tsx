import { Link } from "react-router-dom";

const skills = [
  {
    category: "Backend",
    items: ["C#", "ASP.NET Core", "Minimal APIs", "Entity Framework Core"],
  },
  {
    category: "Frontend",
    items: ["React", "TypeScript", "HTML", "CSS"],
  },
  {
    category: "Data",
    items: ["SQL", "SQLite", "Relational database design"],
  },
  {
    category: "Workflow",
    items: ["Git", "REST APIs", "Testing", "Clean architecture"],
  },
];

const projects = [
  {
    title: "Personal website",
    description:
      "A full-stack personal website with authentication, account management and reusable frontend components.",
    technologies: ["ASP.NET Core", "React", "TypeScript", "SQLite"],
  },
  {
    title: "Project coming soon",
    description:
      "This section is ready for another project once its content, screenshots and source links are available.",
    technologies: ["C#", "React", "SQL"],
  },
];

export function HomePage() {
  return (
    <main className="home-page">
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

      <section className="home-section" id="skills">
        <div className="home-section__heading">
          <p className="home-section__number">01</p>

          <div>
            <p className="home-section__eyebrow">What I work with</p>
            <h2>Skills</h2>
          </div>
        </div>

        <div className="home-section__content">
          <div className="home-section__connector" aria-hidden="true">
            <span className="home-section__connector-dot" />
            <span className="home-section__connector-line" />
          </div>

          <div className="skills-grid">
            {skills.map((skillGroup) => (
              <article
                className="skill-group"
                key={skillGroup.category}
              >
                <h3>{skillGroup.category}</h3>

                <ul>
                  {skillGroup.items.map((skill) => (
                    <li key={skill}>{skill}</li>
                  ))}
                </ul>
              </article>
            ))}
          </div>
        </div>
      </section>

      <section className="home-section" id="projects">
        <div className="home-section__heading">
          <p className="home-section__number">02</p>

          <div>
            <p className="home-section__eyebrow">Selected work</p>
            <h2>Projects</h2>
          </div>
        </div>

        <div className="home-section__content">
          <div className="home-section__connector" aria-hidden="true">
            <span className="home-section__connector-dot" />
            <span className="home-section__connector-line" />
          </div>

          <div className="project-list">
            {projects.map((project, index) => (
              <article className="project-card" key={project.title}>
                <header className="project-card__header">
                  <p className="project-card__number">
                    {String(index + 1).padStart(2, "0")}
                  </p>

                  <h3>{project.title}</h3>
                </header>

                <p className="project-card__description">
                  {project.description}
                </p>

                <ul className="project-card__technologies">
                  {project.technologies.map((technology) => (
                    <li key={technology}>{technology}</li>
                  ))}
                </ul>
              </article>
            ))}
          </div>
        </div>
      </section>

      <section className="home-section home-contact" id="contact">
        <div className="home-section__heading">
          <p className="home-section__number">03</p>

          <div>
            <p className="home-section__eyebrow">Get in touch</p>
            <h2>Contact</h2>
          </div>
        </div>

        <div className="home-section__content">
          <div className="home-section__connector" aria-hidden="true">
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

              <Link className="button button--secondary" to="/login">
                Account login
              </Link>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}