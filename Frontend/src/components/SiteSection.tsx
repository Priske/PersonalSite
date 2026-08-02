import type { ReactNode } from "react";
import { SectionConnector } from "./SectionConnector";

type SiteSectionProps = {
  title?: string;
  eyebrow?: string;
  children: ReactNode;
  className?: string;
  connector?: "left" | "right" | "none";
};

export function SiteSection({
  title,
  eyebrow,
  children,
  className = "",
  connector = "left",
}: SiteSectionProps) {
  const classes = ["site-section", className].filter(Boolean).join(" ");

  return (
    <section className={classes}>
      {connector !== "none" && <SectionConnector side={connector} />}

      {eyebrow && <p className="site-section__eyebrow">{eyebrow}</p>}

      {title && <h2 className="site-section__title">{title}</h2>}

      <div className="site-section__content">{children}</div>
    </section>
  );
}
