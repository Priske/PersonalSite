import { ContactSection } from "./home/ContactSection";
import { HeroSection } from "./home/HeroSection";
import { ProjectsSection } from "./home/ProjectsSection";
import { SkillsSection } from "./home/SkillsSection";
import { useOfficialHomePageConfig } from "./homePageConfig/useHomePageConfig";

export function HomePage() {
  const configQuery = useOfficialHomePageConfig();

  if (configQuery.isPending) {
    return null;
  }

  if (configQuery.isError) {
    return null;
  }

  return (
    <main className="home-page">
      <HeroSection config={configQuery.data} />
      <SkillsSection />
      <ProjectsSection />
      <ContactSection config={configQuery.data} />
    </main>
  );
}
