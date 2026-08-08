import { useQuery } from "@tanstack/react-query";

import { getDemoProjects, getProjects } from "./projectsApi";

export function useProjects() {
  return useQuery({
    queryKey: ["projects", "official"],

    queryFn: () => getProjects(),
  });
}

export function useDemoProjects() {
  return useQuery({
    queryKey: ["projects", "demo"],

    queryFn: () => getDemoProjects(),
  });
}

export function useManageableProjects(isAdministrator: boolean) {
  return useQuery({
    queryKey: ["projects", "management", isAdministrator ? "official" : "demo"],

    queryFn: () =>
      isAdministrator
        ? getProjects({
            page: 1,
            pageSize: 50,
          })
        : getDemoProjects({
            page: 1,
            pageSize: 50,
          }),
  });
}
