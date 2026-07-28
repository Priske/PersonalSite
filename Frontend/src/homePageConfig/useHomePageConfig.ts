import { useQuery } from "@tanstack/react-query";
import {  getHomePageConfigs  } from "./homePageConfigApi";

export function useHomePageConfig(){
    return useQuery({
        queryKey: ["home-page-config"],
        queryFn: getHomePageConfigs 
    });
}