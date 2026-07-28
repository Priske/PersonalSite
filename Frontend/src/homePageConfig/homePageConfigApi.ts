import { apiRequest} from "../api";
import type { GetHomePageConfigDetailsResponse } from "./types";

export function getHomePageConfigs(){
    return apiRequest<GetHomePageConfigDetailsResponse>("/home-page-config")
}

