import Link from "next/link";

export default function ResetPasswordPage() {
  return (
    <main className="simple-shell">
      <section className="simple-card">
        <p className="eyebrow">Coming next</p>
        <h1>Reset password</h1>
        <p>This page is reserved for the token-based reset flow once the backend endpoint is added.</p>
        <Link className="button secondary" href="/login">Back to login</Link>
      </section>
    </main>
  );
}
