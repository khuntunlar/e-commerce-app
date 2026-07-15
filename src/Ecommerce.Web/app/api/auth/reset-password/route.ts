import { NextRequest, NextResponse } from "next/server";
import { identityFetch, readJsonOrProblem } from "@/lib/server/identity-api";

export async function POST(request: NextRequest) {
  const response = await identityFetch("/reset-password", {
    method: "POST",
    body: JSON.stringify(await request.json())
  });

  if (response.status === 204) {
    return new NextResponse(null, { status: 204 });
  }

  return NextResponse.json(await readJsonOrProblem(response), { status: response.status });
}
