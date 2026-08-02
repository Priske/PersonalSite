import { useQuery } from "@tanstack/react-query";
import { getProjects } from "./projectsApi";

export function useProjects() {
  return useQuery({
    queryKey: ["projects"],
    queryFn: getProjects,
  });
}
