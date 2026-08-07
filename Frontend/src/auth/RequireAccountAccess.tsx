import { Navigate, Outlet, useParams } from "react-router-dom";
import { useCurrentUser } from "./useCurrentUser";

export function RequireAccountAccess() {
  const { userId } = useParams();
  const currentUserQuery = useCurrentUser();

  if (currentUserQuery.isPending) {
    return <p>Checking permissions...</p>;
  }

  if (currentUserQuery.isError) {
    return <Navigate to="/login" replace />;
  }

  if (userId === undefined) {
    return <Outlet />;
  }

  const requestedUserId = Number(userId);

  if (!Number.isInteger(requestedUserId) || requestedUserId <= 0) {
    return (
      <main>
        <h1>Invalid user id</h1>
      </main>
    );
  }

  return <Outlet />;
}
