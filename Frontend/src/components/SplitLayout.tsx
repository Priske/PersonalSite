import type { ReactNode } from "react";

type SplitLayoutProps = {
    aside: ReactNode;
    children: ReactNode;
    className?: string;
};

export function SplitLayout({
    aside,
    children,
    className = "",
}: SplitLayoutProps) {
    return (
        <main className={`split-layout ${className}`.trim()}>
            <aside className="split-layout__aside">
                {aside}
            </aside>

            <div className="split-layout__main">
                {children}
            </div>
        </main>
    );
}