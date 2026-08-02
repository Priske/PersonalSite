import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { SkillGroupManagementItem } from "../skills/SkillGroupManagementItem";
import type { SkillGroupSummary } from "../skills/types";
import { useSkillGroups } from "../skills/useSkillGroups";
import { useUpdateSkillGroupOrder } from "../skills/useUpdateSkillGroupOrder";

export function AccountSkillsPage() {
  const groupsQuery = useSkillGroups();
  const updateOrderMutation = useUpdateSkillGroupOrder();

  const [groups, setGroups] = useState<SkillGroupSummary[]>([]);

  useEffect(() => {
    if (!groupsQuery.data) return;

    setGroups(
      [...groupsQuery.data.items].sort(
        (a, b) => a.displayOrder - b.displayOrder,
      ),
    );
  }, [groupsQuery.data]);

  async function moveGroup(currentIndex: number, direction: -1 | 1) {
    const targetIndex = currentIndex + direction;

    if (targetIndex < 0 || targetIndex >= groups.length) return;

    const previousGroups = groups;
    const reorderedGroups = [...groups];

    [reorderedGroups[currentIndex], reorderedGroups[targetIndex]] = [
      reorderedGroups[targetIndex],
      reorderedGroups[currentIndex],
    ];

    setGroups(reorderedGroups);

    try {
      await updateOrderMutation.mutateAsync({
        skillGroupIds: reorderedGroups.map((group) => group.id),
      });
    } catch {
      setGroups(previousGroups);
    }
  }
  return (
    <article className="account-card">
      <header className="account-card__header account-management__header">
        <div>
          <p className="account-card__eyebrow">Website content</p>

          <h2>Skills</h2>

          <p className="account-management__description">
            Manage the skill groups and skills displayed on your homepage.
          </p>
        </div>

        <div className="account-management__header-actions">
          <Link className="button" to="/account/skills/new">
            Add group
          </Link>
        </div>
      </header>

      <div className="account-management__body">
        {groupsQuery.isPending && (
          <p className="account-management__status">Loading skill groups...</p>
        )}

        {groupsQuery.isError && (
          <p className="form-message form-message--error">
            Could not load skill groups.
          </p>
        )}

        {updateOrderMutation.isError && (
          <p className="form-message form-message--error">
            Could not save the skill group order.
          </p>
        )}

        {groupsQuery.isSuccess && groups.length === 0 && (
          <div className="account-management__empty">
            <p className="account-management__empty-title">
              No skill groups yet
            </p>

            <p>Add a skill group before adding individual skills.</p>
          </div>
        )}

        {groupsQuery.isSuccess && groups.length > 0 && (
          <div className="skill-management-list">
            {groups.map((group, index) => (
              <SkillGroupManagementItem
                key={group.id}
                group={group}
                index={index}
                groupCount={groups.length}
                isSaving={updateOrderMutation.isPending}
                onMove={moveGroup}
              />
            ))}
          </div>
        )}
      </div>
    </article>
  );
}
