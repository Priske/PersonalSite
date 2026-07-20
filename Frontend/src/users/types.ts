export type RegisterUserRequest = {
  name: string;
  email: string;
  password: string;
};

export type RegisterUserResponse = {
  id: number;
  name: string;
  email: string;
};

export type UserDetails = {
  id: number;
  name: string;
  email: string;
  role: string;
};


export type UpdateUserRequest = {
  name: string;
  email: string;
};

export type GetUsersRequest = {
  page: number;
  pageSize: number;
  search?: string;
};

export type UserSummary = {
  id: number;
  name: string;
  email: string;
};
