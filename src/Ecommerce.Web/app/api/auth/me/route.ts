import { NextResponse } from "next/server";
import { readAccessToken } from "@/lib/server/auth-session";
import { identityFetch, readJsonOrProblem } from "@/lib/server/identity-api";

export async function GET() {
  const accessToken = await readAccessToken();
  if (!accessToken) {
    return NextResponse.json({ title: "Authentication failed", detail: "Missing session." }, { status: 401 });
  }

  const response = await identityFetch("/me", {
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });
  const body = await readJsonOrProblem(response);
  return NextResponse.json(body, { status: response.status });
}
