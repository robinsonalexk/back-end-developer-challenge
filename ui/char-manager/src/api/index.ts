import type { Character } from "../types/character";
import type { DealDamageRequest } from "../types/dealDamageRequest";
import type { HealDamageRequest } from "../types/healDamageRequest";

export const BASE_URL = import.meta.env.VITE_API_URL ?? "";

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...init,
  });
  if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);

  return res.json() as Promise<T>;
}

export const api = {
  getCharacters: () => request<Character[]>("/api/v1/character"),
  dealDamage: (characterName: string, req: DealDamageRequest) => request(`/api/v1/character/${characterName}/damage`, { method: "PUT", body: JSON.stringify(req) }),
  healDamage: (characterName: string, req: HealDamageRequest) => request(`/api/v1/character/${characterName}/heal`, { method: "PUT", body: JSON.stringify(req) }),
  AddTempHp: (characterName: string, req: HealDamageRequest) => request(`/api/v1/character/${characterName}/temp-heal`, { method: "PUT", body: JSON.stringify(req) }),
};
