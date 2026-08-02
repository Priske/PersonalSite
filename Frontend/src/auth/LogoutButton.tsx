import { useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { removeAccessToken } from "./tokenStorage";

type LogoutButtonProps = {
  className?: string;
};

export function LogoutButton({ className }: LogoutButtonProps) {
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  function handleLogout() {
    removeAccessToken();

    queryClient.cancelQueries({
      queryKey: ["current-user"],
    });

    queryClient.removeQueries({
      queryKey: ["current-user"],
      exact: true,
    });

    navigate("/login", { replace: true });
  }

  return (
    <button className={className} type="button" onClick={handleLogout}>
      Log out
    </button>
  );
}
