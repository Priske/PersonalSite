import { ContactSection } from "./home/ContactSection";
import { FeaturedSection } from "./home/FeaturedSection";
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
      <FeaturedSection number="01" />
      <SkillsSection number="02" />
      <ProjectsSection number="03" />
      <ContactSection config={configQuery.data} number="04" />
    </main>
  );
}
