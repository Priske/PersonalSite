import type {
  GetHomePageConfigDetailsResponse,
} from "../homePageConfig/types";

type HeroSectionProps = {
  config: GetHomePageConfigDetailsResponse;
};

export function HeroSection({ config }: HeroSectionProps) {
  return (
    <section className="home-hero" id="about">
      <div className="home-hero__identity">
        <p className="section-banner">
          {config.heroBanner}
        </p>

        <h1>
          {config.heroFirstName}
          <br />
          {config.heroLastName}
        </h1>

        <p className="home-hero__location">
          {config.heroRole}
        </p>
      </div>

      <div className="home-hero__content">
        <div
          className="home-hero__connector"
          aria-hidden="true"
        >
          <span className="home-hero__connector-dot" />
          <span className="home-hero__connector-line" />
        </div>

        <div className="home-hero__copy">
          <p className="home-hero__eyebrow">
            {config.heroEyebrow}
          </p>

          <h2>{config.heroHeading}</h2>

          <p className="home-hero__summary">
            {config.heroSummary}
          </p>

          <div className="home-hero__actions">
            <a className="button" href="#projects">
              {config.heroPrimaryActionLabel}
            </a>

            <a
              className="button button--secondary"
              href="#contact"
            >
              {config.heroSecondaryActionLabel}
            </a>
          </div>
        </div>
      </div>
    </section>
  );
}