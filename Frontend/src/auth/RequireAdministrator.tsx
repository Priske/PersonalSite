import { Navigate, Outlet } from "react-router-dom";
import { getAccessToken } from "./tokenStorage";
import { useCurrentUser } from "./useCurrentUser"; 

export function RequireAdministrator() {
  const currentUserQuery = useCurrentUser();

  if (!getAccessToken()) {
    return <Navigate to="/login" replace />;
  }

  if (currentUserQuery.isPending) {
    return <p>Checking permissions...</p>;
  }

  if (currentUserQuery.isError) {
    return <Navigate to="/login" replace />;
  }

  if (currentUserQuery.data.role !== "Administrator") {
    return (
      <main>
        <h1>Forbidden</h1>
        <p>Only administrators can manage books.</p>
      </main>
    );
  }

  return <Outlet />;
}