import { ContactSection } from "./home/ContactSection";
import { HeroSection } from "./home/HeroSection";
import { ProjectsSection } from "./home/ProjectsSection";
import { useDemoHomePageConfig } from "./homePageConfig/useHomePageConfig";

export function DemoHomePage() {
  const configQuery = useDemoHomePageConfig();
  if (configQuery.isPending) {
    return null;
  }

  if (configQuery.isError) {
    return null;
  }

  return (
    <main className="home-page">
      <HeroSection config={configQuery.data} />
      <ProjectsSection number="01" demo />
      <ContactSection config={configQuery.data} number="02" />
    </main>
  );
}
