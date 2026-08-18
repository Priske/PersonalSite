import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { getCreateUserAnalytics } from "./analyticsApi";
import type { GetCreateUserAnalyticsRequest } from "./types";

export function useCreateUserAnalytics(request: GetCreateUserAnalyticsRequest) {
  return useQuery({
    queryKey: ["analytics", "create-user", request],
    queryFn: () => getCreateUserAnalytics(request),
    placeholderData: keepPreviousData,
  });
}
