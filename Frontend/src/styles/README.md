# Personal site CV theme

This CSS theme mirrors the visual language of Ben's CV:

- black and white palette with a light-grey sidebar
- bold, condensed, uppercase headings
- rounded black title labels
- strong divider lines and circular timeline markers
- spacious two-column layout that collapses cleanly on mobile

## Use in a Vite + React project

Copy the `styles` folder into `src`, then import:

```ts
import "./styles/global.css";
import "./styles/components.css";
```

Useful class names:

- `cv-page`, `cv-layout`, `cv-sidebar`, `cv-main`
- `cv-section`, `role-banner`
- `contact-list`, `timeline-list`, `timeline-item`
- `skill-grid`, `skill-row`, `skill-meter`
- `site-header`, `site-nav`
- `hero`, `hero__identity`, `hero__content`, `hero__eyebrow`
- `button`, `button--secondary`
- `card-grid`, `card`
- `form-field`

Example skill meter:

```jsx
<div className="skill-row">
  <span>Nederlands</span>
  <span className="skill-meter" style={{ "--level": "92%" }} />
</div>
```