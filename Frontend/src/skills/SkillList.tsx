import { useSkills } from "./useSkills";

type SkillListProps = {
  groupId: number;
};

export function SkillList({
  groupId,
}: SkillListProps) {
  const skillsQuery = useSkills(groupId);

  if (skillsQuery.isPending) {
    return <p>Loading skills...</p>;
  }

  if (skillsQuery.isError) {
    return <p>Could not load skills.</p>;
  }

  if (skillsQuery.data.items.length === 0) {
    return <p>No skills added yet.</p>;
  }

  return (
    <ul>
      {skillsQuery.data.items.map((skill) => (
        <li key={skill.id}>
          {skill.name}
        </li>
      ))}
    </ul>
  );
}