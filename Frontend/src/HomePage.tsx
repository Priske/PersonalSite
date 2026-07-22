import { ContactSection } from "./home/ContactSection";
import { HeroSection } from "./home/HeroSection";
import { ProjectsSection } from "./home/ProjectsSection";
import { SkillsSection } from "./home/SkillsSection";

export function HomePage() {
  return (
    <main className="home-page">
      <HeroSection />
      <SkillsSection />
      <ProjectsSection />
      <ContactSection />
    </main>
  );
}