"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useEffect, useState } from "react";
import { AuthenticatedUser, changePassword, getCurrentUser } from "@/lib/auth-api";

export default function AccountPage() {
  const router = useRouter();
  const [user, setUser] = useState<AuthenticatedUser | null>(null);
  const [status, setStatus] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    getCurrentUser()
      .then(setUser)
      .catch(() => router.replace("/login?next=/account"))
      .finally(() => setIsLoading(false));
  }, [router]);

  async function onChangePassword(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setStatus(null);
    setError(null);
    setIsSubmitting(true);

    const form = event.currentTarget;
    const formData = new FormData(form);

    try {
      await changePassword({
        currentPassword: String(formData.get("currentPassword")),
        newPassword: String(formData.get("newPassword"))
      });
      form.reset();
      setStatus("Password changed successfully.");
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Could not change password");
    } finally {
      setIsSubmitting(false);
    }
  }

  if (isLoading) {
    return <main className="account-shell"><p>Loading account...</p></main>;
  }

  return (
    <main className="account-shell">
      <section className="account-card">
        <p className="eyebrow">Protected route</p>
        <h1>Account</h1>
        {user && (
          <div className="profile-grid">
            <span>Name</span><strong>{user.displayName}</strong>
            <span>Email</span><strong>{user.email}</strong>
            <span>Roles</span><strong>{user.roles.join(", ") || "Customer"}</strong>
          </div>
        )}
        <Link className="button secondary" href="/logout">Logout</Link>
      </section>

      <section className="account-card">
        <p className="eyebrow">Security</p>
        <h2>Change password</h2>
        <form className="auth-form" onSubmit={onChangePassword}>
          <label>
            Current password
            <input name="currentPassword" type="password" autoComplete="current-password" required />
          </label>
          <label>
            New password
            <input name="newPassword" type="password" autoComplete="new-password" minLength={8} required />
          </label>
          {status && <p className="form-success">{status}</p>}
          {error && <p className="form-error">{error}</p>}
          <button className="button primary" type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Changing..." : "Change password"}
          </button>
        </form>
      </section>
    </main>
  );
}
