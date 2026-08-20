import { SkillList } from "../skills/SkillList";
import { useSkillGroups } from "../skills/useSkillGroups";

type SkillsSectionProps = {
  number: string;
};

export function SkillsSection({ number }: SkillsSectionProps) {
  const groupsQuery = useSkillGroups();

  return (
    <section className="home-section" id="skills">
      <div className="home-section__heading">
        <p className="home-section__number">{number}</p>

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
          {groupsQuery.isPending &&
            Array.from({ length: 6 }).map((_, index) => (
              <article className="skill-group" key={index}>
                <h3>Loading...</h3>
                <ul>
                  <li>Loading...</li>
                  <li>Loading...</li>
                  <li>Loading...</li>
                </ul>
              </article>
            ))}

          {groupsQuery.isError && (
            <article className="skill-group skill-group--error">
              <h3>Skills unavailable</h3>
              <p>The skills could not be loaded at the moment.</p>
            </article>
          )}

          {groupsQuery.isSuccess && groupsQuery.data.items.length === 0 && (
            <article className="skill-group">
              <h3>No skills yet</h3>
              <p>Skills will appear here soon.</p>
            </article>
          )}

          {groupsQuery.isSuccess &&
            groupsQuery.data.items.map((group) => (
              <article className="skill-group" key={group.id}>
                <h3>{group.name}</h3>

                <SkillList groupId={group.id} />
              </article>
            ))}
        </div>
      </div>
    </section>
  );
}
