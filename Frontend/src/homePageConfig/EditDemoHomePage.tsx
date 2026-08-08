import { HomePageEditor } from "./HomePageEditor";
import {
  useDemoHomePageConfig,
  useUpdateDemoHomePageConfig,
} from "./useHomePageConfig";

export function EditDemoHomePage() {
  const query = useDemoHomePageConfig();
  const mutation = useUpdateDemoHomePageConfig();

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
