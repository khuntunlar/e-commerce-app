import { NextRequest, NextResponse } from "next/server";
import { identityFetch, readJsonOrProblem } from "@/lib/server/identity-api";

export async function POST(request: NextRequest) {
  const response = await identityFetch("/forgot-password", {
    method: "POST",
    body: JSON.stringify(await request.json())
  });

  return NextResponse.json(await readJsonOrProblem(response), { status: response.status });
}
