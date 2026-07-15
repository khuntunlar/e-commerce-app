import assert from "node:assert/strict";
import { existsSync, readFileSync } from "node:fs";
import { join } from "node:path";
import test from "node:test";

const root = process.cwd();

const requiredPages = [
  "app/login/page.tsx",
  "app/register/page.tsx",
  "app/logout/page.tsx",
  "app/account/page.tsx",
  "app/forgot-password/page.tsx",
  "app/reset-password/page.tsx"
];

const requiredSessionRoutes = [
  "app/api/auth/login/route.ts",
  "app/api/auth/register/route.ts",
  "app/api/auth/logout/route.ts",
  "app/api/auth/me/route.ts",
  "app/api/auth/change-password/route.ts",
  "app/api/auth/forgot-password/route.ts",
  "app/api/auth/reset-password/route.ts"
];

test("auth pages exist", () => {
  for (const file of requiredPages) {
    assert.equal(existsSync(join(root, file)), true, `${file} should exist`);
  }
});

test("session API routes exist", () => {
  for (const file of requiredSessionRoutes) {
    assert.equal(existsSync(join(root, file)), true, `${file} should exist`);
  }
});

test("frontend no longer stores auth session in localStorage", () => {
  const files = [
    "app/login/page.tsx",
    "app/register/page.tsx",
    "app/account/page.tsx",
    "app/logout/page.tsx",
    "lib/auth-api.ts"
  ];

  for (const file of files) {
    const content = readFileSync(join(root, file), "utf8");
    assert.equal(content.includes("localStorage"), false, `${file} should not use localStorage`);
  }
});

test("server session helper uses httpOnly cookies", () => {
  const content = readFileSync(join(root, "lib/server/auth-session.ts"), "utf8");
  assert.equal(content.includes("httpOnly: true"), true);
});
