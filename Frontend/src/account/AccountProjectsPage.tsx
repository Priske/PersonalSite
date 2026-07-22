export function AccountProjectsPage()
{
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
                    <p className="account-management__empty-title">
                        No projects yet
                    </p>

                    <p>
                        Add your first project to start building the projects section.
                    </p>
                </div>
            </div>
        </article>
    );
}