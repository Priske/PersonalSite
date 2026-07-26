import { useProjects } from "../projects/useProjects";

export function ProjectsSection() {
  const projectsQuery = useProjects();

  const content = () => {
    if (projectsQuery.isPending) {
      return (
        <div className="project-card">
          <h3>Loading projects...</h3>
          <p className="project-card__description">Fetching the latest projects.</p>
        </div>
      );
    }

    if (projectsQuery.isError) {
      return (
        <div className="project-card">
          <h3>Projects unavailable</h3>
          <p className="project-card__description">The projects could not be loaded right now.</p>
        </div>
      );
    }

    const projects = projectsQuery.data.items.filter(project => project.isFeatured);

    if (projects.length === 0) {
      return (
        <div className="project-card">
          <h3>Projects coming soon</h3>
          <p className="project-card__description">New projects are currently being prepared for this section.</p>
        </div>
      );
    }

    return projects.map((project, index) => (
      <article className="project-card" key={project.id}>
        <header className="project-card__header">
          <p className="project-card__number">{String(index + 1).padStart(2, "0")}</p>
          <h3>{project.title}</h3>
        </header>

        <p className="project-card__description">{project.discription}</p>

        {project.tags.length > 0 && (
          <ul className="project-card__technologies">
            {project.tags.map(tag => <li key={tag}>{tag}</li>)}
          </ul>
        )}
      </article>
    ));
  };

  return (
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

        <div className="project-list">{content()}</div>
      </div>
    </section>
  );
}