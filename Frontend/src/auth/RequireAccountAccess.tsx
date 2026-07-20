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

  const currentUser = currentUserQuery.data;

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

  const isAdministrator =
    currentUser.role === "Administrator";

  const isOwnAccount =
    currentUser.id === requestedUserId;

  if (!isAdministrator && !isOwnAccount) {
    return (
      <main>
        <h1>Forbidden</h1>
        <p>You cannot view this account.</p>
      </main>
    );
  }

  return <Outlet />;
}