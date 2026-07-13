const identityApiUrl = process.env.IDENTITY_API_URL
  ?? process.env.NEXT_PUBLIC_IDENTITY_API_URL
  ?? "http://localhost:5294";

export async function identityFetch(path: string, init: RequestInit = {}) {
  return fetch(`${identityApiUrl.replace(/\/$/, "")}/api/v1/auth${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...init.headers
    },
    cache: "no-store"
  });
}

export async function readJsonOrProblem(response: Response) {
  const text = await response.text();
  if (!text) {
    return null;
  }

  try {
    return JSON.parse(text);
  } catch {
    return { title: text };
  }
}
