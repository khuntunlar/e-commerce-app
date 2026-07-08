import Link from "next/link";

export default function HomePage() {
  return (
    <main className="home-shell">
      <section className="hero-card">
        <p className="eyebrow">Phase 01 Authentication</p>
        <h1>Identity is online. Let customers step through the front door.</h1>
        <p>
          Register, sign in, inspect the current profile, change password, and sign out through
          the Identity Service API.
        </p>
        <div className="button-row">
          <Link className="button primary" href="/register">Create account</Link>
          <Link className="button secondary" href="/login">Sign in</Link>
        </div>
      </section>
    </main>
  );
}
