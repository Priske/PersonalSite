import { apiRequestWithoutResponse } from "../api";

export async function uploadCv(file: File) {
  const formData = new FormData();

  formData.append("file", file);

  return apiRequestWithoutResponse("/files/cv", {
    method: "POST",
    body: formData,
  });
}
