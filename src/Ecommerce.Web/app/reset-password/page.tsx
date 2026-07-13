"use client";

import Link from "next/link";
import { FormEvent, useState } from "react";
import { resetPassword } from "@/lib/auth-api";

export default function ResetPasswordPage() {
  const [error, setError] = useState<string | null>(null);
  const [status, setStatus] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setStatus(null);
    setIsSubmitting(true);

    const form = event.currentTarget;
    const formData = new FormData(form);

    try {
      await resetPassword(
        String(formData.get("email")),
        String(formData.get("resetToken")),
        String(formData.get("newPassword")));
      form.reset();
      setStatus("Password reset complete. You can sign in now.");
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Could not reset password");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="simple-shell">
      <section className="simple-card">
        <p className="eyebrow">Password help</p>
        <h1>Reset password</h1>
        <form className="auth-form" onSubmit={onSubmit}>
          <label>
            Email
            <input name="email" type="email" autoComplete="email" required />
          </label>
          <label>
            Reset token
            <input name="resetToken" type="text" required />
          </label>
          <label>
            New password
            <input name="newPassword" type="password" autoComplete="new-password" minLength={8} required />
          </label>
          {status && <p className="form-success">{status}</p>}
          {error && <p className="form-error">{error}</p>}
          <button className="button primary" type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Resetting..." : "Reset password"}
          </button>
        </form>
        <p className="form-link"><Link href="/login">Back to login</Link></p>
      </section>
    </main>
  );
}
