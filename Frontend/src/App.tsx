import "./styles/global.css";
import "./styles/components.css";
import { Route, Routes } from "react-router-dom";
import { Navigation } from "./components/Navigation";
import { LoginPage } from "./auth/LoginPage";
import { AccountPage } from "./users/AccountPage";
import { RequireAccountAccess } from "./auth/RequireAccountAccess";
import { EditAccountPage } from "./users/EditAccountPage";
import { HomePage } from "./HomePage";
import { RegisterPage } from "./users/RegisterPage";

function App()
{
    return (
        <>
            <Navigation />

            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage/>}/>

                <Route element={<RequireAccountAccess />}>
                  <Route path="/account" element={<AccountPage />} />
                  <Route path="/account/edit" element={<EditAccountPage />} />
              </Route>
            </Routes>
        </>
    );
}

export default App;
