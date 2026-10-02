import type { DamageTypes } from "./damageTypes";

export interface DealDamageRequest {
    amount: number;
    incomingDamageType: DamageTypes;
}