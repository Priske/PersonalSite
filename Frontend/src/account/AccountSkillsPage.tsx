import { Link } from "react-router-dom";
import { SkillGroupManagementItem } from "../skills/SkillGroupManagementItem";
import { useSkillGroups } from "../skills/useSkillGroups";

export function AccountSkillsPage() {
  const groupsQuery = useSkillGroups();

  return (
    <article className="account-card">
      <header className="account-card__header account-management__header">
        <div>
          <p className="account-card__eyebrow">
            Website content
          </p>

          <h2>Skills</h2>

          <p className="account-management__description">
            Manage the skill groups and skills displayed on your
            homepage.
          </p>
        </div>
        <Link className="button" to="/account/skills/new">
          Add skill group
        </Link>
      </header>

      <div className="account-management__body">
        {groupsQuery.isPending && (
          <p className="account-management__status">
            Loading skill groups...
          </p>
        )}

        {groupsQuery.isError && (
          <p className="form-message form-message--error">
            Could not load skill groups.
          </p>
        )}

        {groupsQuery.isSuccess &&
          groupsQuery.data.items.length === 0 && (
            <div className="account-management__empty">
              <p className="account-management__empty-title">
                No skill groups yet
              </p>

              <p>
                Add a skill group before adding individual skills.
              </p>
            </div>
          )}

        {groupsQuery.isSuccess &&
          groupsQuery.data.items.length > 0 && (
            <div className="skill-management-list">
              {groupsQuery.data.items.map((group) => (
                <SkillGroupManagementItem
                  key={group.id}
                  group={group}
                />
              ))}
            </div>
          )}
      </div>
    </article>
  );
}