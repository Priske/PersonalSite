import { useProjects } from "../projects/useProjects";
import type { ProjectSummary } from "../projects/types";
import { useEffect, useState } from "react";
import { useUpdateProjectsOrder } from "../projects/useUpdateProjectsOrder";
import { ProjectsSection } from "../home/ProjectsSection";
import { ProjectManagementItem } from "../projects/ProjectManagementitem";

export function AccountProjectsPage()
{
    const projectQuerry = useProjects();
    const updateOrderMutation = useUpdateProjectsOrder();
    const [projets, setProjects] = useState<ProjectSummary[]>([]);

    useEffect(() => {
        if(!projectQuerry.data) return;

        setProjects([...projectQuerry.data.items].sort(
            (a,b) => a.displayOrder - b.displayOrder,
        ));
    }, [projectQuerry.data]);

    async function moveProject(currentIndex: number, direction: -1|1){
        const targetIndex = currentIndex + direction;

        if(targetIndex < 0 || targetIndex >= projets.length) return;    

        const previousProjects = projets;
        const reorderedProjects = [...projets];

        [reorderedProjects[currentIndex], reorderedProjects[targetIndex]] = [reorderedProjects[targetIndex], reorderedProjects[currentIndex]];

        setProjects(reorderedProjects);

        try {
            await updateOrderMutation.mutateAsync({
                projectIds: reorderedProjects.map((project) => project.id)
            });
        }catch{
            setProjects(previousProjects);
        }
    }
    return (
        <article className="account-card">
            <header className="account-card__header account-management__header">
                <div>
                    <p className="account-card__eyebrow">
                        Website content
                    </p>

                    <h2>Projects</h2>

                    <p className="account-management__description">
                        Manage the projects displayed on your homepage.
                    </p>
                </div>

                <button className="button" type="button">
                    Add project
                </button>
            </header>

            <div className="account-management__body">
                <div className="account-management__empty">
                    {projectQuerry.isPending && (
                        <p className="account-management__status">
                            Loading projects...
                        </p>
                    )}
                    {projectQuerry.isError && (
                        <p className="form-message form-message--error">
                            Could not load projects.
                        </p>
                    )}
                    {updateOrderMutation.isError && (
                        <p className="form-message form-message--error">
                            Could not save the project order.
                        </p>
                    )}
                    {projectQuerry.isSuccess && projets.length === 0 && (
                        <div className="account-management__empty">
                            <p className="account-management__empty-title">
                                 No skill groups yet
                            </p>
                            <p>
                                Add a skill group before adding individual skills.
                            </p>
                        </div>
                    )}

                    {projectQuerry.isSuccess && ProjectsSection.length >0 &&(
                        <div className="skill-managementrr-list">
                            {projets.map((project,index) =>(
                                <ProjectManagementItem
                                key={project.id}
                                project={project}
                                index={index}
                                projectCount={ProjectsSection.length}
                                isSaving={updateOrderMutation.isPending}
                                onMove={moveProject}
                                />
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </article>
    );
    
}