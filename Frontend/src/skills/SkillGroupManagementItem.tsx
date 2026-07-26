import { Link } from "react-router-dom";
import type { SkillGroupSummary } from "./types";
import { useSkills } from "./useSkills";

type SkillGroupManagementItemProps = {
  group: SkillGroupSummary;
  index: number;
  groupCount: number;
  isSaving: boolean;
  onMove: (currentIndex: number, direction: -1 | 1) => Promise<void>;
};

export function SkillGroupManagementItem({
  group,
  index,
  groupCount,
  isSaving,
  onMove,
}: SkillGroupManagementItemProps) {
  const skillsQuery = useSkills(group.id);

  return (
    <section className="skill-management-group">
      <header className="skill-management-group__header">
        <div>
          <p className="skill-management-group__order">
            Group {String(index + 1).padStart(2, "0")}
          </p>

          <h3>{group.name}</h3>
        </div>

        <div className="skill-management-group__actions">
          <button
            className="button button--secondary"
            type="button"
            aria-label={`Move ${group.name} up`}
            title="Move up"
            onClick={() => void onMove(index, -1)}
            disabled={isSaving || index === 0}
          >
            ↑
          </button>

          <button
            className="button button--secondary"
            type="button"
            aria-label={`Move ${group.name} down`}
            title="Move down"
            onClick={() => void onMove(index, 1)}
            disabled={isSaving || index === groupCount - 1}
          >
            ↓
          </button>

          <Link
            className="button button--secondary"
            to={`/account/skills/${group.id}/edit`}
          >
            Edit group
          </Link>
        </div>
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
                <li className="skill-management-skill" key={skill.id}>
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