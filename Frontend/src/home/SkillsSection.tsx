/*
const skills = [
  {
    category: "Backend",
    items: [
      "C#",
      "ASP.NET Core",
      "Minimal APIs",
      "Entity Framework Core",
    ],
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
*/
import { SkillList } from "../skills/SkillList";
import { useSkillGroups } from "../skills/useSkillGroups";

export function SkillsSection() {
  const groupsQuery = useSkillGroups();

  return (
    <section className="home-section" id="skills">
      <div className="home-section__heading">
        <p className="home-section__number">01</p>

        <div>
          <p className="home-section__eyebrow">
            What I work with
          </p>

          <h2>Skills</h2>
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

        <div className="skills-grid">
          {groupsQuery.isPending && (
            <p>Loading skill groups...</p>
          )}

          {groupsQuery.isError && (
            <p>Could not load skill groups.</p>
          )}

          {groupsQuery.isSuccess &&
            groupsQuery.data.items.length === 0 && (
              <p>No skill groups added yet.</p>
            )}

          {groupsQuery.isSuccess &&
            groupsQuery.data.items.map((group) => (
              <article
                className="skill-group"
                key={group.id}
              >
                <h3>{group.name}</h3>

                <SkillList groupId={group.id} />
              </article>
            ))}
        </div>
      </div>
    </section>
  );
}