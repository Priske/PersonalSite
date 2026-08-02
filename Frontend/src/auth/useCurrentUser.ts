import { useEffect } from "react";
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { ApiError } from "../api";
import { getCurrentUser } from "./authApi";
import { removeAccessToken, useAccessToken } from "./tokenStorage";

export function useCurrentUser() {
  const accessToken = useAccessToken();
  const queryClient = useQueryClient();

  const query = useQuery({
    queryKey: ["current-user"],
    queryFn: getCurrentUser,
    enabled: accessToken !== null,
    retry: false,
  });

  const unauthorized =
    query.error instanceof ApiError && query.error.status === 401;

  useEffect(() => {
    if (!unauthorized) {
      return;
    }

    removeAccessToken();
    queryClient.removeQueries({
      queryKey: ["current-user"],
    });
  }, [unauthorized, queryClient]);

  return query;
}
