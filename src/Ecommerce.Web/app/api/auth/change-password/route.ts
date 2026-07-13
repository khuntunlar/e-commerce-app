import { NextRequest, NextResponse } from "next/server";
import { readAccessToken } from "@/lib/server/auth-session";
import { identityFetch, readJsonOrProblem } from "@/lib/server/identity-api";

export async function POST(request: NextRequest) {
  const accessToken = await readAccessToken();
  if (!accessToken) {
    return NextResponse.json({ title: "Authentication failed", detail: "Missing session." }, { status: 401 });
  }

  const response = await identityFetch("/change-password", {
    method: "POST",
    headers: {
      Authorization: `Bearer ${accessToken}`
    },
    body: JSON.stringify(await request.json())
  });

  if (response.status === 204) {
    return new NextResponse(null, { status: 204 });
  }

  return NextResponse.json(await readJsonOrProblem(response), { status: response.status });
}
