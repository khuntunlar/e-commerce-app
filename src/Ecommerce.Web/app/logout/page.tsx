"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { logout } from "@/lib/auth-api";
import { clearSession, readSession } from "@/lib/auth-store";

export default function LogoutPage() {
  const [message, setMessage] = useState("Signing you out...");

  useEffect(() => {
    const session = readSession();

    async function endSession() {
      try {
        if (session?.refreshToken) {
          await logout(session.refreshToken);
        }
      } finally {
        clearSession();
        setMessage("You are signed out.");
      }
    }

    void endSession();
  }, []);

  return (
    <main className="simple-shell">
      <section className="simple-card">
        <p className="eyebrow">Session</p>
        <h1>{message}</h1>
        <Link className="button primary" href="/login">Sign in again</Link>
      </section>
    </main>
  );
}
