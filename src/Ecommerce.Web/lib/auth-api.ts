export type AuthenticatedUser = {
  id: string;
  email: string;
  displayName: string;
  roles: string[];
};

export type AuthSession = {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: AuthenticatedUser;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type RegisterRequest = LoginRequest & {
  displayName: string;
};

export type ChangePasswordRequest = {
  currentPassword: string;
  newPassword: string;
};

const identityApiUrl = process.env.NEXT_PUBLIC_IDENTITY_API_URL ?? "http://localhost:5294";
const authBaseUrl = `${identityApiUrl.replace(/\/$/, "")}/api/v1/auth`;

async function parseError(response: Response): Promise<string> {
  const fallback = `Request failed with status ${response.status}`;

  try {
    const body = await response.json();
    if (typeof body.title === "string") {
      return body.detail ? `${body.title}: ${body.detail}` : body.title;
    }

    if (typeof body.message === "string") {
      return body.message;
    }

    if (body.errors && typeof body.errors === "object") {
      return Object.values(body.errors).flat().join(" ");
    }
  } catch {
    return fallback;
  }

  return fallback;
}

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  const response = await fetch(`${authBaseUrl}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...init.headers
    }
  });

  if (!response.ok) {
    throw new Error(await parseError(response));
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

export function login(payload: LoginRequest): Promise<AuthSession> {
  return request<AuthSession>("/login", {
    method: "POST",
    body: JSON.stringify(payload)
  });
}

export function register(payload: RegisterRequest): Promise<AuthSession> {
  return request<AuthSession>("/register", {
    method: "POST",
    body: JSON.stringify(payload)
  });
}

export function getCurrentUser(accessToken: string): Promise<AuthenticatedUser> {
  return request<AuthenticatedUser>("/me", {
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });
}

export function changePassword(accessToken: string, payload: ChangePasswordRequest): Promise<void> {
  return request<void>("/change-password", {
    method: "POST",
    headers: {
      Authorization: `Bearer ${accessToken}`
    },
    body: JSON.stringify(payload)
  });
}

export function logout(refreshToken: string): Promise<void> {
  return request<void>("/logout", {
    method: "POST",
    body: JSON.stringify({ refreshToken })
  });
}
