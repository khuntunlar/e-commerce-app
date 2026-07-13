"use client";

import Link from "next/link";
import { FormEvent, useState } from "react";
import { forgotPassword } from "@/lib/auth-api";

export default function ForgotPasswordPage() {
  const [error, setError] = useState<string | null>(null);
  const [resetToken, setResetToken] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setResetToken(null);
    setIsSubmitting(true);

    const formData = new FormData(event.currentTarget);

    try {
      const result = await forgotPassword(String(formData.get("email")));
      setResetToken(result.resetToken);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Could not request reset token");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="simple-shell">
      <section className="simple-card">
        <p className="eyebrow">Password help</p>
        <h1>Forgot password</h1>
        <form className="auth-form" onSubmit={onSubmit}>
          <label>
            Email
            <input name="email" type="email" autoComplete="email" required />
          </label>
          {error && <p className="form-error">{error}</p>}
          {resetToken && (
            <p className="form-success">
              Development reset token: <code>{resetToken}</code>
            </p>
          )}
          <button className="button primary" type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Requesting..." : "Request reset token"}
          </button>
        </form>
        <p className="form-link"><Link href="/reset-password">I have a reset token</Link></p>
      </section>
    </main>
  );
}
