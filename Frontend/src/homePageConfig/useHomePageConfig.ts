import { useMutation, useQuery, useQueryClient} from "@tanstack/react-query";
import {  getHomePageConfig, updateHomePageConfig} from "./homePageConfigApi";

export const homePageConfigQueryKey =
  ["home-page-config"] as const;

export function useHomePageConfig() {
  return useQuery({
    queryKey: homePageConfigQueryKey,
    queryFn: getHomePageConfig,
  });
}

export function useUpdateHomePageConfig() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: updateHomePageConfig,
    onSuccess: () =>
      queryClient.invalidateQueries({
        queryKey: homePageConfigQueryKey,
      }),
  });
}