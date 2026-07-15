"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { FormEvent, useState } from "react";
import { AuthCard } from "@/components/AuthCard";
import { register } from "@/lib/auth-api";

export default function RegisterPage() {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);

    const formData = new FormData(event.currentTarget);

    try {
      await register({
        displayName: String(formData.get("displayName")),
        email: String(formData.get("email")),
        password: String(formData.get("password"))
      });
      router.push("/account");
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Registration failed");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <AuthCard eyebrow="Join the platform" title="Create your customer account">
      <form className="auth-form" onSubmit={onSubmit}>
        <label>
          Display name
          <input name="displayName" type="text" autoComplete="name" minLength={2} maxLength={100} required />
        </label>
        <label>
          Email
          <input name="email" type="email" autoComplete="email" required />
        </label>
        <label>
          Password
          <input name="password" type="password" autoComplete="new-password" minLength={8} required />
        </label>
        {error && <p className="form-error">{error}</p>}
        <button className="button primary" type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Creating account..." : "Create account"}
        </button>
      </form>
      <p className="form-link">
        Already registered? <Link href="/login">Sign in</Link>
      </p>
    </AuthCard>
  );
}
