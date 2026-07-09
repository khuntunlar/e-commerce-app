import Link from "next/link";

export default function ForgotPasswordPage() {
  return (
    <main className="simple-shell">
      <section className="simple-card">
        <p className="eyebrow">Coming next</p>
        <h1>Forgot password</h1>
        <p>Password reset email flow is documented for Phase-1, but the backend endpoint is not implemented yet.</p>
        <Link className="button secondary" href="/login">Back to login</Link>
      </section>
    </main>
  );
}
