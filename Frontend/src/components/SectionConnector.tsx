type SectionConnectorProps = {
    side?: "left" | "right";
    direction?: "horizontal" | "vertical";
    className?: string;
};

export function SectionConnector({
    side = "left",
    direction = "horizontal",
    className = "",
}: SectionConnectorProps) {
    const classes = [
        "section-connector",
        `section-connector--${side}`,
        `section-connector--${direction}`,
        className,
    ]
        .filter(Boolean)
        .join(" ");

    return (
        <span className={classes} aria-hidden="true">
            <span className="section-connector__line" />
            <span className="section-connector__dot" />
        </span>
    );
}