import "./styles/global.css";
import "./styles/components.css";
import { Route, Routes } from "react-router-dom";
import { Navigation } from "./components/Navigation";
import { LoginPage } from "./auth/LoginPage";
import { AccountPage } from "./account/AccountPage";
import { RequireAccountAccess } from "./auth/RequireAccountAccess";
import { EditAccountPage } from "./account/EditAccountPage";
import { HomePage } from "./HomePage";
import { RegisterPage } from "./users/RegisterPage";
import { RequireAdministrator } from "./auth/RequireAdministrator";
import { UserListPage } from "./users/UsersListPage";
import { EditUserPage } from "./users/EditUserPage";
import { AccountLayout } from "./account/AccountLayout";
import { AccountSkillsPage } from "./account/AccountSkillsPage";
import { AccountProjectsPage } from "./account/AccountProjectsPage";
import { ManageSkillGroupPage } from "./skills/ManageSkillGroupPage";
import { AddSkillGroupPage } from "./skills/AddSkillGroupPage";
import { ManageProjectPage } from "./projects/ManageProjectPage";
import { AddProjectPage } from "./projects/AddProjectPage";
import { TagsPage } from "./tags/TagsPage";
import { EditHomePage } from "./homePageConfig/EditHomePage";

function App() {
  return (
    <>
      <Navigation />

      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        <Route element={<RequireAccountAccess />}>
          <Route path="/users" element={<UserListPage />} />
          <Route path="/users/:userId/edit" element={<EditUserPage />} />
          <Route path="/account" element={<AccountLayout />}>
            <Route index element={<AccountPage />} />
            <Route path="edit" element={<EditAccountPage />} />
            <Route
              path="skills/:groupId/edit"
              element={<ManageSkillGroupPage />}
            />
            <Route path="/account/skills/new" element={<AddSkillGroupPage />} />
            <Route
              path="projects/:projectId/edit"
              element={<ManageProjectPage />}
            />
            <Route path="projects/new" element={<AddProjectPage />} />
            <Route path="skills" element={<AccountSkillsPage />} />
            <Route path="projects" element={<AccountProjectsPage />} />
            <Route path="tags" element={<TagsPage />} />
            <Route path="homePage" element={<EditHomePage />} />
          </Route>
        </Route>

        <Route element={<RequireAdministrator />}></Route>
      </Routes>
    </>
  );
}

export default App;
