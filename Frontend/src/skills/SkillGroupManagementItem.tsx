import { Link } from "react-router-dom";
import type { SkillGroupSummary } from "./types";
import { useSkills } from "./useSkills";

type SkillGroupManagementItemProps = {
  group: SkillGroupSummary;
};

export function SkillGroupManagementItem({
  group,
}: SkillGroupManagementItemProps) {
  const skillsQuery = useSkills(group.id);

  return (
    <section className="skill-management-group">
      <header className="skill-management-group__header">
        <div>
          <p className="skill-management-group__order">
            Group {String(group.displayOrder).padStart(2, "0")}
          </p>

          <h3>{group.name}</h3>
        </div>

        <Link
          className="button button--secondary"
          to={`/account/skills/${group.id}/edit`}
        >
          Edit group
        </Link>
      </header>

      <div className="skill-management-group__content">
        {skillsQuery.isPending && (
          <p className="account-management__status">
            Loading skills...
          </p>
        )}

        {skillsQuery.isError && (
          <p className="form-message form-message--error">
            Could not load skills for this group.
          </p>
        )}

        {skillsQuery.isSuccess &&
          skillsQuery.data.items.length === 0 && (
            <p className="skill-management-group__empty">
              No skills attached to this group.
            </p>
          )}

        {skillsQuery.isSuccess &&
          skillsQuery.data.items.length > 0 && (
            <ol className="skill-management-skills">
              {skillsQuery.data.items.map((skill) => (
                <li
                  className="skill-management-skill"
                  key={skill.id}
                >
                  <span className="skill-management-skill__order">
                    {String(skill.displayOrder).padStart(2, "0")}
                  </span>

                  <span className="skill-management-skill__name">
                    {skill.name}
                  </span>
                </li>
              ))}
            </ol>
          )}
      </div>
    </section>
  );
}