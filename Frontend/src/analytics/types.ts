export type ActivityType =
  | "PageViewed"
  | "User_Registered"
  | "Login"
  | "Logout"
  | "DemoHomepageUpdated"
  | "DemoProjectCreated"
  | "DemoProjectUpdated"
  | "DemoProjectDeleted";

export type MetadataValue =
  | string
  | number
  | boolean
  | {
      [key: string]: MetadataValue;
    };

export type ActivityMetadataRequest = {
  key: string;
  value: MetadataValue;
};

export type TrackActivityRequest = {
  type: ActivityType;
  metadata: ActivityMetadataRequest[];
};
