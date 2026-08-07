export type GetHomePageConfigDetailsResponse = {
  heroBanner: string;
  heroFirstName: string;
  heroLastName: string;
  heroRole: string;

  heroEyebrow: string;
  heroHeading: string;
  heroSummary: string;

  heroPrimaryActionLabel: string;
  heroSecondaryActionLabel: string;

  contactSectionNumber: string;
  contactSectionEyebrow: string;
  contactSectionHeading: string;

  contactEyebrow: string;
  contactHeading: string;
  contactDescription: string;

  contactEmailActionLabel: string;
  contactLoginActionLabel: string;

  email: string;
  phoneNumber?: string;
  linkedInUrl?: string;
  gitHubUrl?: string;
  cvUrl?: string;

  source: string;
  createdByUserId: number | null;
  lastEditedByUserId: number | null;
  createdAt: string;
  lastEditedAt: string;
};

export type UpdateHomePageConfigRequest = Omit<
  GetHomePageConfigDetailsResponse,
  | "source"
  | "createdByUserId"
  | "lastEditedByUserId"
  | "createdAt"
  | "lastEditedAt"
>;
