import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  getOfficialHomePageConfig,
  getDemoHomePageConfig,
  updateOfficialHomePageConfig,
  updateDemoHomePageConfig,
} from "./homePageConfigApi";

export const officialHomePageConfigQueryKey = [
  "home-page-config",
  "official",
] as const;

export const demoHomePageConfigQueryKey = ["home-page-config", "demo"] as const;

export function useOfficialHomePageConfig() {
  return useQuery({
    queryKey: officialHomePageConfigQueryKey,
    queryFn: getOfficialHomePageConfig,
  });
}

export function useDemoHomePageConfig() {
  return useQuery({
    queryKey: demoHomePageConfigQueryKey,
    queryFn: getDemoHomePageConfig,
  });
}

export function useUpdateOfficialHomePageConfig() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: updateOfficialHomePageConfig,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: officialHomePageConfigQueryKey,
      }),
  });
}

export function useUpdateDemoHomePageConfig() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: updateDemoHomePageConfig,

    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: demoHomePageConfigQueryKey,
      }),
  });
}
