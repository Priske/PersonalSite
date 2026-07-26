import { useProjects } from "../projects/useProjects";
import type { ProjectSummary } from "../projects/types";
import { useEffect, useState } from "react";
import { useUpdateProjectsOrder } from "../projects/useUpdateProjectsOrder";
import { ProjectManagementItem } from "../projects/ProjectManagementitem";
import { Link } from "react-router-dom";

export function AccountProjectsPage()
{
    const projectQuery = useProjects();
    const updateOrderMutation = useUpdateProjectsOrder();
    const [projets, setProjects] = useState<ProjectSummary[]>([]);

    useEffect(() => {
        if(!projectQuery.data) return;

        setProjects([...projectQuery.data.items].sort(
            (a,b) => a.displayOrder - b.displayOrder,
        ));
    }, [projectQuery.data]);

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

                <Link className="button" to="/account/projects/new">
                    Add project
                </Link>
            </header>

            <div className="account-management__body">
                <div className="account-management__empty">
                    {projectQuery.isPending && (
                        <p className="account-management__status">
                            Loading projects...
                        </p>
                    )}
                    {projectQuery.isError && (
                        <p className="form-message form-message--error">
                            Could not load projects.
                        </p>
                    )}
                    {updateOrderMutation.isError && (
                        <p className="form-message form-message--error">
                            Could not save the project order.
                        </p>
                    )}
                    {projectQuery.isSuccess && projets.length === 0 && (
                        <div className="account-management__empty">
                            <p className="account-management__empty-title">
                                No projects yet
                            </p>
                            <p>
                            Add a project to display it on your homepage.
                            </p>
                        </div>
                    )}

                    {projectQuery.isSuccess && projets.length > 0 && (
                    <div className="project-management-list">
                        {projets.map((project, index) => (
                        <ProjectManagementItem
                            key={project.id}
                            project={project}
                            index={index}
                            projectCount={projets.length}
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