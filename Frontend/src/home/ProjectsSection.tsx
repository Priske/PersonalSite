const projects = [
  {
    title: "Personal website",
    description:
      "A full-stack personal website with authentication, account management and reusable frontend components.",
    technologies: [
      "ASP.NET Core",
      "React",
      "TypeScript",
      "SQLite",
    ],
  },
  {
    title: "Project coming soon",
    description:
      "This section is ready for another project once its content, screenshots and source links are available.",
    technologies: ["C#", "React", "SQL"],
  },
];

export function ProjectsSection() {
  return (
    <section className="home-section" id="projects">
      <div className="home-section__heading">
        <p className="home-section__number">02</p>

        <div>
          <p className="home-section__eyebrow">
            Selected work
          </p>

          <h2>Projects</h2>
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

        <div className="project-list">
          {projects.map((project, index) => (
            <article
              className="project-card"
              key={project.title}
            >
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
                  <li key={technology}>
                    {technology}
                  </li>
                ))}
              </ul>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
}