import { NextRequest, NextResponse } from "next/server";
import { identityFetch, readJsonOrProblem } from "@/lib/server/identity-api";
import { setSessionCookies } from "@/lib/server/auth-session";
import type { AuthSession } from "@/lib/auth-api";

export async function POST(request: NextRequest) {
  const response = await identityFetch("/register", {
    method: "POST",
    body: JSON.stringify(await request.json())
  });
  const body = await readJsonOrProblem(response);

  if (!response.ok) {
    return NextResponse.json(body, { status: response.status });
  }

  const nextResponse = NextResponse.json(body, { status: response.status });
  await setSessionCookies(nextResponse, body as AuthSession);
  return nextResponse;
}
