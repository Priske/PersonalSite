import { HomePageEditor } from "./HomePageEditor";
import {
  useOfficialHomePageConfig,
  useUpdateOfficialHomePageConfig,
} from "./useHomePageConfig";

export function EditHomePage() {
  const query = useOfficialHomePageConfig();
  const mutation = useUpdateOfficialHomePageConfig();

  return (
    <HomePageEditor
      config={query.data}
      isLoading={query.isPending}
      isLoadError={query.isError}
      isSaving={mutation.isPending}
      saveError={mutation.error}
      isSaveSuccess={mutation.isSuccess}
      onSave={(request) => mutation.mutate(request)}
    />
  );
}
