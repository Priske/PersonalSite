export type LoginRequest = {
  email: string;
  password: string;
};

export type LoginResponse = {
  accessToken: string;
  expiresAt: string;
};

export type CurrentUser = {
  id: number;
  name: string;
  email: string;
  role: string;
};