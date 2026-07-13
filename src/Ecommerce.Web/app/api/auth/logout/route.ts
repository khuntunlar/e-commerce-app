import { NextResponse } from "next/server";
import { clearSessionCookies, readRefreshToken } from "@/lib/server/auth-session";
import { identityFetch } from "@/lib/server/identity-api";

export async function POST() {
  const refreshToken = await readRefreshToken();

  if (refreshToken) {
    await identityFetch("/logout", {
      method: "POST",
      body: JSON.stringify({ refreshToken })
    }).catch(() => undefined);
  }

  const response = new NextResponse(null, { status: 204 });
  clearSessionCookies(response);
  return response;
}
