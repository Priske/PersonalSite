import { SkillList } from "./SkillList";
import { useSkillGroups } from "./useSkillGroups";

export function SkillGroups() {
  const groupsQuery = useSkillGroups();

  if (groupsQuery.isPending) {
    return <p>Loading skill groups...</p>;
  }

  if (groupsQuery.isError) {
    return <p>Could not load skill groups.</p>;
  }

  if (groupsQuery.data.items.length === 0) {
    return <p>No skill groups added yet.</p>;
  }

  return (
    <section>
      <h2>Skills</h2>

      {groupsQuery.data.items.map((group) => (
        <article key={group.id}>
          <h3>{group.name}</h3>

          <SkillList groupId={group.id} />
        </article>
      ))}
    </section>
  );
}