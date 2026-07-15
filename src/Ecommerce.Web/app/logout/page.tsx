"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { logout } from "@/lib/auth-api";

export default function LogoutPage() {
  const [message, setMessage] = useState("Signing you out...");

  useEffect(() => {
    async function endSession() {
      try {
        await logout();
      } finally {
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
