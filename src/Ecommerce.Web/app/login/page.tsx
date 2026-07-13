"use client";

import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import { FormEvent, Suspense, useState } from "react";
import { AuthCard } from "@/components/AuthCard";
import { login } from "@/lib/auth-api";

function LoginForm() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const next = searchParams.get("next") ?? "/account";
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);

    const formData = new FormData(event.currentTarget);

    try {
      await login({
        email: String(formData.get("email")),
        password: String(formData.get("password"))
      });
      router.push(next);
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : "Login failed");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <>
      <form className="auth-form" onSubmit={onSubmit}>
        <label>
          Email
          <input name="email" type="email" autoComplete="email" required />
        </label>
        <label>
          Password
          <input name="password" type="password" autoComplete="current-password" required />
        </label>
        {error && <p className="form-error">{error}</p>}
        <button className="button primary" type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Signing in..." : "Sign in"}
        </button>
      </form>
      <p className="form-link">
        New here? <Link href="/register">Create an account</Link>
      </p>
      <p className="form-link">
        <Link href="/forgot-password">Forgot password?</Link>
      </p>
    </>
  );
}

export default function LoginPage() {
  return (
    <AuthCard eyebrow="Welcome back" title="Sign in to your account">
      <Suspense fallback={<p>Preparing login...</p>}>
        <LoginForm />
      </Suspense>
    </AuthCard>
  );
}
