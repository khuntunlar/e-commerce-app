import type { ReactNode } from "react";

export function AuthCard({ eyebrow, title, children }: { eyebrow: string; title: string; children: ReactNode }) {
  return (
    <main className="auth-shell">
      <section className="brand-panel">
        <p className="eyebrow">{eyebrow}</p>
        <h1>{title}</h1>
        <p>
          A focused identity entry point for the commerce platform. Secure sessions first,
          delightful shopping flow next.
        </p>
      </section>
      <section className="form-panel">{children}</section>
    </main>
  );
}
